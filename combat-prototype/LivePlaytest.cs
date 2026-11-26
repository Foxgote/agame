using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using CombatPrototype.Core;

namespace CombatPrototype;

// Opt-in bridge for tuning a local run without sending any keyboard or mouse input.
internal sealed class LivePlaytest
{
    private static readonly JsonSerializerOptions Json = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private readonly string _settingsFile;
    private readonly string _statusFile;
    private readonly string _exchangeFile;
    private ulong _nextRead;
    private ulong _nextWrite;
    private string? _lastPacket;
    private CombatEngine? _round;
    private int _roundNumber;
    private int _resolutionIndex;
    private int _exchangeIndex;
    private readonly string _sessionId = Guid.NewGuid().ToString("N");
    private readonly int[][] _outcomes = [new int[5], new int[5]];
    private Packet? _active;
    private string? _error;
    private int _attackTaps;
    private int _acceptedAttackTaps;
    private string? _lastTapBlock;

    public LivePlaytest(string directory)
    {
        Directory.CreateDirectory(directory);
        _settingsFile = Path.Combine(directory, "opponent.json");
        _statusFile = Path.Combine(directory, "status.json");
        _exchangeFile = Path.Combine(directory, "exchange.jsonl");
    }

    public void ApplyProfile(CounterTrial trial)
    {
        ReadSettings();
        if (_active is not { } packet) return;
        ApplyPacket(trial, packet);
    }

    internal static void ApplySavedProfile(CounterTrial trial, string json)
    {
        var packet = JsonSerializer.Deserialize<Packet>(json, Json);
        if (!IsValid(packet)) throw new InvalidDataException("Invalid saved combat profile.");
        ApplyPacket(trial, packet!);
    }

    private static void ApplyPacket(CounterTrial trial, Packet packet)
    {
        trial.OpponentSettings = packet.Settings;
        trial.Economy = packet.Economy;
        trial.PlayerLightStartup = packet.PlayerLightStartup;
        trial.OpponentLightStartup = packet.OpponentLightStartup;
        trial.SharedAttackWindup = packet.SharedAttackWindup;
        trial.Mixups = packet.Mixups;
        trial.InterruptHeavyStartup = packet.InterruptHeavyStartup;
        trial.BurstLights = packet.BurstLights;
        trial.LightLaunchInterval = packet.LightLaunchInterval;
        trial.SpecialLaunchInterval = packet.SpecialLaunchInterval;
        trial.QueueLimit = packet.QueueLimit;
        trial.SpecialCounterRecovery = packet.SpecialCounterRecovery;
        trial.SpecialTiming = packet.SpecialTiming;
        trial.DisableSpecialRecovery = packet.DisableSpecialRecovery;
        trial.OpponentLookahead = packet.OpponentLookahead;
        trial.ClearQueueOnSpecialDefense = packet.ClearQueueOnSpecialDefense;
    }

    public void RecordAttackTap(bool added, string? reason)
    {
        _attackTaps++;
        if (added) _acceptedAttackTaps++;
        _lastTapBlock = reason;
    }

    public void Update(CombatEngine engine, bool paused, double speed, bool focused, ulong now)
    {
        BeginRound(engine);
        var trial = engine.CounterTrial;
        if (trial is not null && now >= _nextRead)
        {
            _nextRead = now + 250;
            ApplyProfile(trial);
        }
        if (now < _nextWrite) return;
        _nextWrite = now + 1000;
        FlushExchange();

        var resolutions = engine.Resolutions;
        for (; _resolutionIndex < resolutions.Count; _resolutionIndex++)
        {
            var hit = resolutions[_resolutionIndex];
            _outcomes[hit.AttackerIsPlayer ? 1 : 0][(int)hit.Outcome]++;
        }
        var snapshot = new
        {
            updatedUtc = DateTimeOffset.UtcNow,
            mode = trial is null ? "classic" : "counter",
            appliedRevision = trial?.OpponentSettings is not null ? _active?.Revision : null,
            profile = trial?.OpponentSettings is not null ? _active?.Name : null,
            settings = trial?.OpponentSettings,
            economy = trial?.Economy,
            playerLightStartup = trial?.PlayerLightStartup,
            opponentLightStartup = trial?.OpponentLightStartup,
            sharedAttackWindup = trial?.SharedAttackWindup,
            mixups = trial?.Mixups,
            interruptHeavyStartup = trial?.InterruptHeavyStartup,
            lightLaunchInterval = engine.BurstLights ? engine.BurstLightInterval : (double?)null,
            specialLaunchInterval = engine.BurstLights ? engine.BurstSpecialInterval : (double?)null,
            queueLimit = trial?.QueueLimit,
            specialCounterRecovery = engine.SpecialCounterRecovery,
            specialAttackerRecovery = engine.SpecialAttackerRecovery,
            specialDefenderRecovery = engine.SpecialDefenderRecovery,
            specialWindup = trial is not null ? engine.DescribeAttack(AttackKind.Unparryable).Startup : (double?)null,
            opponentLookahead = trial?.OpponentLookahead,
            clearQueueOnSpecialDefense = trial?.ClearQueueOnSpecialDefense,
            error = _error,
            round = _roundNumber, combatTime = Math.Round(engine.Time, 3), paused, speed,
            focused,
            attackInput = new { taps = _attackTaps, accepted = _acceptedAttackTaps, lastTapBlock = _lastTapBlock },
            player = Fighter(engine.Player), opponent = Fighter(engine.Opponent),
            recent = resolutions.Skip(Math.Max(0, resolutions.Count - 8)).Select(r => new
            {
                time = Math.Round(r.ImpactTime, 3), attacker = r.AttackerIsPlayer ? "player" : "bot",
                attack = r.AttackKind.ToString(), outcome = r.Outcome.ToString()
            }),
            gap = trial?.Gap
        };
        try
        {
            File.WriteAllText(_statusFile + ".tmp", JsonSerializer.Serialize(snapshot, Json));
            File.Move(_statusFile + ".tmp", _statusFile, overwrite: true);
        }
        catch (IOException) { } // A concurrent reader can briefly hold the file open on Windows.
        catch (UnauthorizedAccessException) { }
    }

    public void Complete(CombatEngine engine)
    {
        BeginRound(engine);
        FlushExchange();
    }

    private void BeginRound(CombatEngine engine)
    {
        if (ReferenceEquals(_round, engine)) return;
        FlushExchange();
        _round = engine;
        _roundNumber++;
        _resolutionIndex = 0;
        _exchangeIndex = 0;
        foreach (var side in _outcomes) Array.Clear(side);
        _nextRead = 0;
        _nextWrite = 0;
    }

    private void FlushExchange()
    {
        if (_round is not { } round || _exchangeIndex >= round.Events.Count) return;
        var events = round.Events;
        var end = events.Count;
        var batch = new StringBuilder();
        for (var index = _exchangeIndex; index < end; index++)
        {
            var item = events[index];
            batch.AppendLine(JsonSerializer.Serialize(new
            {
                session = _sessionId, round = _roundNumber, index,
                time = item.Time, kind = item.Kind.ToString(), message = item.Message
            }, Json));
        }
        try
        {
            File.AppendAllText(_exchangeFile, batch.ToString());
            _exchangeIndex = end;
        }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }

    private object Fighter(CombatFighter fighter)
    {
        var outcomes = _outcomes[fighter.IsPlayer ? 0 : 1];
        return new
        {
            damage = fighter.DamageTaken, stamina = Math.Round(fighter.AvailableStamina, 1),
            queuedAttacks = fighter.BufferedAttackCount,
            attack = fighter.CurrentAttack?.Info.Kind.ToString(),
            attacksInFlight = fighter.ActiveAttacks.Count,
            timedDefenseAttempts = fighter.TimedDefenseAttempts,
            hitsTaken = outcomes[(int)AttackOutcome.CleanHit], blocks = outcomes[(int)AttackOutcome.Blocked],
            parries = outcomes[(int)AttackOutcome.Parried], dodges = outcomes[(int)AttackOutcome.Dodged],
            incomingWhiffs = outcomes[(int)AttackOutcome.Whiffed]
        };
    }

    private void ReadSettings()
    {
        try
        {
            var info = new FileInfo(_settingsFile);
            if (!info.Exists || info.Length > 4096) return;
            var text = File.ReadAllText(_settingsFile);
            if (text == _lastPacket) return;
            _lastPacket = text;
            var packet = JsonSerializer.Deserialize<Packet>(text, Json);
            if (!IsValid(packet))
            {
                _error = "Invalid playtest settings; retained last profile.";
                return;
            }
            if (packet!.Revision <= (_active?.Revision ?? 0)) return;
            _active = packet;
            _error = null;
        }
        catch (JsonException) { _error = "Invalid JSON; retained last profile."; }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }

    private static bool IsValid(Packet? packet)
    {
        var tune = packet?.Settings;
        return !(packet is not { Revision: > 0, Name.Length: > 0 and <= 32 } ||
                tune is not { } value ||
                !Within(value.Cadence, 0.15, 1.70) || !Within(value.CadenceSpread, 0, 0.50) ||
                !Within(value.ReactionDelay, 0.12, 0.40) || !Within(value.ReadChance, 0.10, 0.95) ||
                !Within(value.TimingSpread, 0.04, 0.18) ||
                (packet.Economy is { } economy && !ValidEconomy(economy)) ||
                (packet.PlayerLightStartup is { } startup && !Within(startup, 0.10, 1.20)) ||
                (packet.OpponentLightStartup is { } opponentStartup && !Within(opponentStartup, 0.06, 1.20)) ||
                (packet.SharedAttackWindup is { } shared && !Within(shared, 0.10, 1.20)) ||
                (packet.LightLaunchInterval is { } launchInterval && !Within(launchInterval, 0.02, 0.20)) ||
                (packet.SpecialLaunchInterval is { } specialInterval && !Within(specialInterval, 0.02, 0.60)) ||
                (packet.QueueLimit is { } queueLimit && (queueLimit < 1 || queueLimit > CombatEngine.MaxBufferLength)) ||
                (packet.SpecialCounterRecovery is { } recovery && !Within(recovery, 0, 1.20)) ||
                (packet.SpecialTiming is { } timing && (!Within(timing.Windup, 0.10, 1.20) ||
                    !Within(timing.ContactRecovery, 0, 1.20) || !Within(timing.AvoidedRecovery, 0, 1.20))) ||
                !Within(packet.OpponentLookahead, 0, 0.30) ||
                (packet.Mixups is { } mix && (!Within(mix.ComboChance, 0, 1) || !Within(mix.ComboGap, 0.09, 0.24) ||
                    !Within(mix.CounterUseChance, 0, 1) || !Within(mix.ParryBias, 0, 1))));
    }

    private static bool Within(double value, double min, double max) => value >= min && value <= max;
    private static bool ValidEconomy(CounterEconomySettings economy) =>
        Within(economy.StartingStamina, 0, 100) && Within(economy.LightCost, 0, 30) &&
        Within(economy.HeavyCost, economy.LightCost, 50) && Within(economy.Regeneration, 0, 45) &&
        Within(economy.DefenseReward, 0, 25);

    private sealed record Packet(int Revision, string Name, CounterOpponentSettings Settings,
        CounterEconomySettings? Economy = null, double? PlayerLightStartup = null, bool BurstLights = false,
        double? OpponentLightStartup = null, bool InterruptHeavyStartup = true,
        double? SharedAttackWindup = null, CounterMixupSettings? Mixups = null, double? LightLaunchInterval = null,
        int? QueueLimit = null, double? SpecialCounterRecovery = null, double OpponentLookahead = 0,
        bool ClearQueueOnSpecialDefense = false, bool DisableSpecialRecovery = false,
        CounterSpecialTiming? SpecialTiming = null, double? SpecialLaunchInterval = null);
}
