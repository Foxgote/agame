using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using CombatPrototype.Core;
using Godot;

namespace CombatPrototype;

public partial class LatencyDuel : Control
{
    private const double PlaySpeed = 0.605;
    private const int DefaultPort = 37070;
    private const int SnapshotChannel = 1;
    private readonly HashSet<Key> _movementKeys = [];
    private ENetMultiplayerPeer? _peer;
    private bool _hosting;
    private bool _connected;
    private bool _paused = true;
    private int _guest;
    private int _round;
    private int _resultIndex;
    private double _accumulator;
    private ulong _nextSnapshotAt;
    private ulong _nextSampleAt;
    private ulong _joinDeadline;
    private double? _rtt;
    private double? _rttVariation;
    private CombatEngine? _fight;
    private Frame? _frame;
    private StreamWriter? _log;
    private string? _profileJson;
    private string _status = "Offline";
    private readonly string[] _results = ["", ""];

    private enum Action { Attack, ParryDown, ParryUp, Dodge, Move }
    private sealed record Note(AttackKind Kind, double Launch, double Impact);
    private sealed record Side(double Position, double Damage, double Stamina, int Waiting, string? Next,
        bool Guard, string? Counter, string Result, Note[] Notes);
    private sealed record Frame(int Round, double Clock, bool Paused, int QueueLimit, double Window, Side Host, Side Guest);

    public override void _Ready()
    {
        BuildDuelInterface();
        GetWindow().Title = "Combat Rhythm Lab - Latency Duel";
        GetWindow().MinSize = new Vector2I(800, 500);
        GetWindow().FocusExited += ClearHeldActions;
        Multiplayer.PeerConnected += PeerJoined;
        Multiplayer.PeerDisconnected += PeerLeft;
        Multiplayer.ConnectedToServer += JoinedHost;
        Multiplayer.ConnectionFailed += JoinFailed;
        Multiplayer.ServerDisconnected += HostLeft;
        UpdateDuelInterface();
    }

    public override void _ExitTree()
    {
        GetWindow().FocusExited -= ClearHeldActions;
        Multiplayer.PeerConnected -= PeerJoined;
        Multiplayer.PeerDisconnected -= PeerLeft;
        Multiplayer.ConnectedToServer -= JoinedHost;
        Multiplayer.ConnectionFailed -= JoinFailed;
        Multiplayer.ServerDisconnected -= HostLeft;
        _peer?.Close();
        CloseLog();
    }

    private void Connect(bool host)
    {
        if (_peer is not null) return;
        if (!IPAddress.TryParse(_address.Text.Trim(), out var address) || address.AddressFamily != AddressFamily.InterNetwork)
        {
            _status = "Enter an IPv4 address";
            return;
        }
        if (host)
        {
            try
            {
                _profileJson = Godot.FileAccess.GetFileAsString("res://profiles/locked-timing.json");
                LivePlaytest.ApplySavedProfile(new CounterTrial(), _profileJson);
            }
            catch (Exception ex) when (ex is IOException or InvalidDataException or JsonException)
            {
                _status = "Locked Timing profile could not be loaded";
                return;
            }
        }

        var peer = new ENetMultiplayerPeer();
        var port = (int)_port.Value;
        if (host) peer.SetBindIP(address.ToString());
        var error = host ? peer.CreateServer(port, 1, 2) : peer.CreateClient(address.ToString(), port, 2);
        if (error != Error.Ok)
        {
            peer.Dispose();
            _status = host ? $"Cannot bind {address}:{port} ({error})" : $"Cannot join ({error})";
            return;
        }

        _peer = peer;
        _hosting = host;
        _joinDeadline = Time.GetTicksMsec() + 10_000;
        Multiplayer.MultiplayerPeer = peer;
        _status = host ? $"Waiting at {address}:{port}" : $"Joining {address}:{port}";
        GetViewport().GuiGetFocusOwner()?.ReleaseFocus();
    }

    private void PeerJoined(long id)
    {
        if (!_hosting || _guest != 0) return;
        _guest = (int)id;
        _connected = true;
        _status = "Hosting";
        OpenLog();
        ResetDuel();
    }

    private void PeerLeft(long id)
    {
        if (!_hosting || id != _guest) return;
        _guest = 0;
        _connected = false;
        _fight = null;
        _frame = null;
        _rtt = _rttVariation = null;
        _movementKeys.Clear();
        _status = "Guest left - waiting";
        CloseLog();
    }

    private void JoinedHost()
    {
        _connected = true;
        _status = "Joined";
    }

    private void JoinFailed() => Disconnect("Join failed");
    private void HostLeft() => Disconnect("Host disconnected");

    private void Disconnect(string status = "Offline")
    {
        var peer = _peer;
        _peer = null;
        _connected = _hosting = false;
        _guest = 0;
        Multiplayer.MultiplayerPeer = new OfflineMultiplayerPeer();
        peer?.Close();
        peer?.Dispose();
        _fight = null;
        _frame = null;
        _rtt = _rttVariation = null;
        _movementKeys.Clear();
        _status = status;
        CloseLog();
    }

    private void ResetDuel()
    {
        if (!_hosting || !_connected || _profileJson is null) return;
        var trial = new CounterTrial { HumanOpponent = true };
        LivePlaytest.ApplySavedProfile(trial, _profileJson);
        _fight = new CombatEngine(new CombatTuning { AiAttackEnabled = false, AiDefenseEnabled = false },
            trial, trackHealth: false);
        _round++;
        _resultIndex = 0;
        _accumulator = 0;
        _paused = true;
        _results[0] = _results[1] = "";
        _movementKeys.Clear();
        Log("round", new { speed = PlaySpeed, trial.LightLaunchInterval, trial.SpecialLaunchInterval, trial.SharedAttackWindup });
        _nextSnapshotAt = 0;
    }

    private void ToggleDuel()
    {
        if (!_hosting || !_connected) return;
        _paused = !_paused;
        _accumulator = 0;
        if (_paused && _fight is { } engine)
        {
            engine.SetParryHeld(engine.Player, false);
            engine.SetParryHeld(engine.Opponent, false);
            engine.CounterTrial!.SetMovement(0);
            engine.CounterTrial.SetOpponentMovement(0);
        }
        _movementKeys.Clear();
        _nextSnapshotAt = 0;
    }

    public override void _Process(double delta)
    {
        var now = Time.GetTicksMsec();
        if (_peer is not null && !_hosting && !_connected && now >= _joinDeadline)
            Disconnect("Join timed out");

        if (_connected && _peer is { } peer && now >= _nextSampleAt)
        {
            _nextSampleAt = now + 1000;
            var link = peer.GetPeer(_hosting ? _guest : 1);
            if (link is not null)
            {
                _rtt = link.GetStatistic(ENetPacketPeer.PeerStatistic.RoundTripTime);
                _rttVariation = link.GetStatistic(ENetPacketPeer.PeerStatistic.RoundTripTimeVariance);
                Log("network", new { rttMs = _rtt, variationMs = _rttVariation });
                _log?.Flush();
            }
        }
        if (_fight is { } engine && _connected)
        {
            if (!_paused)
            {
                _accumulator += Math.Min(delta, 0.15) * PlaySpeed;
                while (_accumulator >= CombatEngine.FixedStep)
                {
                    _accumulator -= CombatEngine.FixedStep;
                    engine.Tick(CombatEngine.FixedStep);
                }
                RecordResults(engine);
            }
            _frame = BuildFrame(engine);
            if (now >= _nextSnapshotAt)
            {
                _nextSnapshotAt = now + 16;
                RpcId(_guest, MethodName.ReceiveFrame, JsonSerializer.Serialize(_frame));
            }
        }
        UpdateDuelInterface();
    }

    private Frame BuildFrame(CombatEngine engine)
    {
        var trial = engine.CounterTrial!;
        return new Frame(_round, engine.Time, _paused, trial.QueueLimit ?? CombatEngine.MaxBufferLength,
            engine.Tuning.ParryWindow, Fighter(engine.Player, trial.PlayerPosition), Fighter(engine.Opponent, trial.OpponentPosition));

        Side Fighter(CombatFighter fighter, double position) => new(position, fighter.DamageTaken, fighter.Stamina,
            engine.WaitingAttackCount(fighter), fighter.NextQueuedAttack?.Name,
            trial.IsGuarding(fighter.IsPlayer, engine.Time), trial.Counter(fighter.IsPlayer, engine.Time)?.ToString(),
            _results[fighter.IsPlayer ? 0 : 1],
            fighter.ActiveAttacks.Select(a => new Note(a.Info.Kind, a.LaunchAt, a.ImpactAt)).ToArray());
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.UnreliableOrdered,
        TransferChannel = SnapshotChannel)]
    private void ReceiveFrame(string snapshot)
    {
        if (_hosting || !_connected || snapshot.Length > 8192) return;
        try
        {
            if (JsonSerializer.Deserialize<Frame>(snapshot) is not { } frame) return;
            if (_frame?.Round != frame.Round || _frame.Paused != frame.Paused) _movementKeys.Clear();
            _frame = frame;
        }
        catch (JsonException) { Disconnect("Incompatible duel packet"); }
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is not InputEventKey { Echo: false } key || !_connected) return;
        var code = key.PhysicalKeycode == Key.None ? key.Keycode : key.PhysicalKeycode;
        if (key.Pressed && _hosting)
        {
            if (code == Key.Escape) ToggleDuel();
            if (code == Key.R) ResetDuel();
        }
        if (code is Key.A or Key.D)
        {
            if (key.Pressed && _frame is { Paused: false }) _movementKeys.Add(code);
            else _movementKeys.Remove(code);
            SendAction(Action.Move, (_movementKeys.Contains(Key.D) ? 1 : 0) - (_movementKeys.Contains(Key.A) ? 1 : 0));
        }
        else if (code == Key.P) SendAction(key.Pressed ? Action.ParryDown : Action.ParryUp);
        else if (code == Key.O && key.Pressed) SendAction(Action.Attack);
        else if (code == Key.I && key.Pressed) SendAction(Action.Dodge);
        else if (code is not (Key.Escape or Key.R)) return;
        GetViewport().SetInputAsHandled();
    }

    private void ClearHeldActions()
    {
        _movementKeys.Clear();
        SendAction(Action.ParryUp);
        SendAction(Action.Move);
    }

    private void SendAction(Action action, int value = 0)
    {
        if (!_connected || _frame is not { Paused: false } frame) return;
        if (_hosting) ApplyAction(true, action, value);
        else RpcId(1, MethodName.ReceiveAction, frame.Round, (int)action, value);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void ReceiveAction(int round, int action, int value)
    {
        if (!_hosting || !_connected || _paused || round != _round ||
            Multiplayer.GetRemoteSenderId() != _guest || action < 0 || action > (int)Action.Move) return;
        ApplyAction(false, (Action)action, Math.Clamp(value, -1, 1));
    }

    private void ApplyAction(bool host, Action action, int value)
    {
        if (_fight is not { CounterTrial: { } trial } engine || _paused) return;
        var fighter = host ? engine.Player : engine.Opponent;
        if (action is Action.Attack or Action.ParryDown or Action.Dodge)
        {
            var target = (host ? engine.Opponent : engine.Player).ActiveAttacks
                .MinBy(attack => Math.Abs(attack.ImpactAt - engine.Time));
            var lastStrike = trial.LastStrike(!host);
            if (lastStrike is not null && (target is null || engine.Time - lastStrike.ImpactAt < Math.Abs(target.ImpactAt - engine.Time)))
                target = lastStrike;
            if (target is not null && Math.Abs(engine.Time - target.ImpactAt) > 0.30) target = null;
            Log("action", new
            {
                side = host ? "host" : "guest", action = action.ToString(), target = target?.Id,
                offsetMs = target is null ? (double?)null : (engine.Time - target.ImpactAt) / PlaySpeed * 1000
            });
        }
        switch (action)
        {
            case Action.Attack: engine.QueueTrialAttack(fighter); break;
            case Action.ParryDown: engine.SetParryHeld(fighter, true); break;
            case Action.ParryUp: engine.SetParryHeld(fighter, false); break;
            case Action.Dodge: engine.ActivateDefense(fighter, DefenseKind.Dodge); break;
            case Action.Move:
                if (host) trial.SetMovement(value);
                else trial.SetOpponentMovement(-value);
                break;
        }
    }

    private void RecordResults(CombatEngine engine)
    {
        for (; _resultIndex < engine.Resolutions.Count; _resultIndex++)
        {
            var result = engine.Resolutions[_resultIndex];
            _results[result.AttackerIsPlayer ? 1 : 0] = $"{result.Outcome.ToString().ToUpperInvariant()} - {result.AttackKind}";
            Log("result", new
            {
                attack = result.AttackKind.ToString(), side = result.AttackerIsPlayer ? "host" : "guest",
                outcome = result.Outcome.ToString(), result.AttackId, result.ImpactTime, result.Damage,
                defenseOffsetMs = result.DefenseTimingOffset / PlaySpeed * 1000
            });
        }
    }

    private void OpenLog()
    {
        CloseLog();
        try
        {
            var dir = Path.Combine(OS.GetUserDataDir(), "latency-duels");
            Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, $"duel-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}.jsonl");
            _log = new StreamWriter(path);
            GD.Print($"Duel log: {path}");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { GD.PushWarning(ex.Message); }
    }

    private void CloseLog()
    {
        _log?.Dispose();
        _log = null;
    }

    private void Log(string type, object details)
    {
        _log?.WriteLine(JsonSerializer.Serialize(new { type, round = _round, time = _fight?.Time, details }));
    }
}
