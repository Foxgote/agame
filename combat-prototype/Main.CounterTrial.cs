using System;
using CombatPrototype.Core;
using Godot;

namespace CombatPrototype;

public partial class Main
{
    private const float ArenaHeight = 86;
    private const double StrikeFlashSeconds = 0.18;
    private Control _spacingDisplay = null!;
    private Label _rangeStatus = null!;
    private Label _playerScanStatus = null!;
    private Label _opponentScanStatus = null!;
    private Button _parryButton = null!;
    private bool _parryKeyHeld;
    private bool _parryButtonHeld;

    private CombatEngine CreateRound()
    {
        var trial = _useCounter ? new CounterTrial() : null;
        if (trial is not null) _livePlaytest?.ApplyProfile(trial);
        return new CombatEngine(_tuning, trial, trackHealth: false);
    }

    private void ClearCounterInput()
    {
        _counterKeys.Clear();
        _parryKeyHeld = _parryButtonHeld = false;
        UpdateParryHold();
        ApplyCounterInput();
    }

    private void UpdateParryHold() => _combat.SetParryHeld(_combat.Player, _parryKeyHeld || _parryButtonHeld);

    private void ApplyCounterInput()
    {
        if (_combat.CounterTrial is not { } trial) return;
        trial.SetMovement((_counterKeys.Contains(Key.D) ? 1 : 0) - (_counterKeys.Contains(Key.A) ? 1 : 0));
    }

    private void AttackInCounter() => AttackInCounter(fromKey: false);

    private void AttackInCounter(bool fromKey)
    {
        var before = _combat.Events.Count;
        var accepted = _combat.QueueTrialAttack(_combat.Player, Time.GetTicksUsec() / 1_000_000.0);
        var lastEvent = _combat.Events[^1];
        if (!accepted && lastEvent.Kind == CombatEventKind.InsufficientStamina)
            _playerUi.FlashUntil = Time.GetTicksMsec() + 180;
        if (fromKey && _livePlaytest is { } live)
        {
            var added = accepted && _combat.Events.Count > before;
            live.RecordAttackTap(added, !accepted ? lastEvent.Message : added ? null : "Follow-up already buffered");
        }
    }

    private static (PanelContainer Row, Label Count, Label Next) BuildQueueRail(bool defenderIsPlayer)
    {
        var row = new PanelContainer
        {
            Name = "IncomingQueue", CustomMinimumSize = new Vector2(0, 28), MouseFilter = MouseFilterEnum.Ignore
        };
        row.AddThemeStyleboxOverride("panel", CreateFlatStyle("#192630", "#405466", 1, 3));
        var content = new HBoxContainer { MouseFilter = MouseFilterEnum.Ignore };
        var count = SmallLabel();
        count.Name = defenderIsPlayer ? "PlayerQueuedCount" : "AiQueuedCount";
        count.CustomMinimumSize = new Vector2(135, 0);
        count.MouseFilter = MouseFilterEnum.Ignore;
        count.AddThemeColorOverride("font_color", new Color(defenderIsPlayer ? "#f4aa82" : "#85dcb9"));
        var next = SmallLabel();
        next.Name = defenderIsPlayer ? "PlayerNextAttack" : "AiNextAttack";
        next.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        next.HorizontalAlignment = HorizontalAlignment.Right;
        next.MouseFilter = MouseFilterEnum.Ignore;
        content.AddChild(count);
        content.AddChild(next);
        row.AddChild(content);
        return (row, count, next);
    }

    private void UpdateQueueRail(FighterUi ui, CombatFighter attacker)
    {
        var rail = ui.IncomingQueue;
        rail.Row.Visible = _useCounter;
        if (!_useCounter) return;
        var count = _combat.WaitingAttackCount(attacker);
        var capacity = _combat.AttackQueueLimit(attacker) is { } max ? $"{count}/{max}" : $"{count}";
        rail.Count.Text = $"{(attacker.IsPlayer ? "YOUR QUEUE" : "AI QUEUE")}  {capacity}";
        var next = attacker.NextQueuedAttack;
        rail.Next.Text = next is not null ? $"NEXT: {next.Name.ToUpperInvariant()}"
            : count > 0 ? "FOLLOW-UP" : "EMPTY";
        rail.Next.AddThemeColorOverride("font_color", next is not null ? GetAttackColor(next.Kind) : new Color("#9eafbe"));
    }

    private void DrawBurstNotes(LaneTrackUi track, CombatFighter attacker)
    {
        var size = track.BurstNotes.Size;
        const float startY = 3f;
        var endY = StrikeY(size.Y);
        foreach (var attack in attacker.ActiveAttacks)
        {
            if (attack.Info.Lane != track.Lane) continue;
            var startup = Math.Max(0.001, attack.Total);
            var travel = Math.Max(0, endY - startY);
            var speed = (float)(travel / startup);
            var noteHeight = Math.Clamp(speed * (float)_combat.BurstLightInterval * 0.72f, 1f, 7f);
            var progress = Math.Max(0, (_combat.Time - attack.LaunchAt) / startup);
            var y = startY + Math.Max(0, travel - noteHeight) * (float)progress;
            track.BurstNotes.DrawRect(new Rect2(4, y, Math.Max(0, size.X - 8), noteHeight), GetAttackColor(attack.Info.Kind));
        }
    }

    private Control BuildSpacingDisplay()
    {
        _spacingDisplay = new Control { Name = "Spacing", ClipContents = true, CustomMinimumSize = new Vector2(0, ArenaHeight) };
        var statusRow = new HBoxContainer { MouseFilter = MouseFilterEnum.Ignore };
        _spacingDisplay.AddChild(statusRow);
        statusRow.SetAnchorsAndOffsetsPreset(LayoutPreset.TopWide);
        statusRow.OffsetLeft = 8;
        statusRow.OffsetRight = -8;
        statusRow.OffsetTop = 3;
        _playerScanStatus = AddStatus("PlayerScan", HorizontalAlignment.Left);
        _rangeStatus = AddStatus("RangeStatus", HorizontalAlignment.Center);
        _opponentScanStatus = AddStatus("OpponentScan", HorizontalAlignment.Right);
        _spacingDisplay.Draw += DrawSpacing;
        return _spacingDisplay;

        Label AddStatus(string name, HorizontalAlignment alignment)
        {
            var label = SmallLabel();
            label.Name = name;
            label.HorizontalAlignment = alignment;
            label.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            label.MouseFilter = MouseFilterEnum.Ignore;
            statusRow.AddChild(label);
            return label;
        }
    }

    private void DrawSpacing()
    {
        if (_combat.CounterTrial is not { } trial) return;
        var width = _spacingDisplay.Size.X;
        const float floor = ArenaHeight - 9;
        const float fighterHeight = 49;
        var halfBody = (float)CounterTrial.BodyGap * width / 2;
        var player = (float)trial.PlayerPosition * width;
        var opponent = (float)trial.OpponentPosition * width;
        _spacingDisplay.DrawRect(new Rect2(0, 0, width, ArenaHeight), new Color("#162420"));
        _spacingDisplay.DrawLine(new Vector2(0, floor), new Vector2(width, floor), new Color("#56655e"));
        DrawReach(CounterTrial.HeavyReach, new Color("#c2896b"));
        DrawReach(CounterTrial.LightReach, new Color("#85dcb9"));
        DrawAttack(_combat.Player, player, 1);
        DrawAttack(_combat.Opponent, opponent, -1);
        DrawFighter(player, new Color("#85dcb9"), 1);
        DrawFighter(opponent, new Color("#f4aa82"), -1);

        void DrawReach(double reach, Color color)
        {
            var left = Math.Max(0, opponent - (float)reach * width);
            _spacingDisplay.DrawLine(new Vector2(left, floor), new Vector2(opponent, floor), color, 3);
        }

        void DrawFighter(float x, Color color, int facing)
        {
            _spacingDisplay.DrawRect(BodyAt(x), new Color(color, 0.08f));
            _spacingDisplay.DrawRect(BodyAt(x), new Color(color, 0.85f), false, 1);
            _spacingDisplay.DrawRect(new Rect2(x - 6, floor - 26, 12, 26), color);
            _spacingDisplay.DrawCircle(new Vector2(x, floor - 35), 5, color);
            _spacingDisplay.DrawLine(new Vector2(x, floor - 23), new Vector2(x + 11 * facing, floor - 23), color, 3);
        }

        Rect2 BodyAt(float x) => new(x - halfBody, floor - fighterHeight, halfBody * 2, fighterHeight);

        void DrawAttack(CombatFighter fighter, float currentX, int direction)
        {
            if (fighter.CurrentAttack is { StrikeAtImpact: null, Cancelled: false } windup)
            {
                var color = GetAttackColor(windup.Info.Kind);
                _spacingDisplay.DrawRect(ReachAt(windup, currentX, direction), new Color(color, 0.65f), false, 1);
            }
            if (RecentStrike(trial, fighter.IsPlayer) is not { StrikeAtImpact: { } sample } strike) return;
            var strikeColor = ScanColor(strike);
            var scan = ReachAt(strike, (float)sample.AttackerPosition * width, direction);
            _spacingDisplay.DrawRect(scan, new Color(strikeColor, 0.38f));
            _spacingDisplay.DrawRect(scan, strikeColor, false, 2);
            _spacingDisplay.DrawRect(BodyAt((float)sample.DefenderPosition * width), new Color(strikeColor, 0.72f), false, 1);
        }

        Rect2 ReachAt(AttackState attack, float x, int facing)
        {
            // The engine tests centers; offset the far edge so overlap with the body outline matches that test.
            var start = x + facing * halfBody;
            var end = x + facing * ((float)CounterTrial.ReachFor(attack.Info.Kind) * width - halfBody);
            return new Rect2(Math.Min(start, end), floor - 32, Math.Abs(end - start), 20);
        }
    }

    private AttackState? RecentStrike(CounterTrial trial, bool player) =>
        trial.LastStrike(player) is { Cancelled: false } attack && _combat.Time - attack.ImpactAt < StrikeFlashSeconds
            ? attack : null;

    private static Color ScanColor(AttackState strike) => strike.Outcome switch
    {
        AttackOutcome.Whiffed => new Color("#e89782"),
        AttackOutcome.Parried => new Color("#8aeee3"),
        AttackOutcome.Blocked => new Color("#d5d8df"),
        AttackOutcome.Dodged => new Color("#b9a8f2"),
        AttackOutcome.CleanHit => new Color("#ffc87f"),
        _ => GetAttackColor(strike.Info.Kind)
    };

    private void UpdateScanStatus(CounterTrial trial, CombatFighter fighter, Label label)
    {
        if (RecentStrike(trial, fighter.IsPlayer) is { } strike)
        {
            var status = strike.Outcome switch
            {
                AttackOutcome.Whiffed => "MISS",
                AttackOutcome.Parried => "PARRIED",
                AttackOutcome.Blocked => "BLOCKED",
                AttackOutcome.Dodged => "DODGED",
                AttackOutcome.CleanHit => "HIT",
                _ => strike.StrikeAtImpact is { InReach: true } ? "IN REACH" : "OUT OF REACH"
            };
            label.Text = $"{fighter.Name.ToUpperInvariant()} · {status}";
            label.Modulate = ScanColor(strike);
        }
        else if (fighter.CurrentAttack is { StrikeAtImpact: null, Cancelled: false } windup)
        {
            label.Text = $"{fighter.Name.ToUpperInvariant()} · {windup.Info.Name}";
            label.Modulate = GetAttackColor(windup.Info.Kind);
        }
        else label.Text = "";
    }

    private Control BuildCounterControls()
    {
        var row = new HBoxContainer { Name = "CounterControls", SizeFlagsHorizontal = SizeFlags.ExpandFill };
        _counterButton = MakeButton("Attack [O]", AttackInCounter, 205);
        row.AddChild(_counterButton);
        _parryButton = new Button
        {
            Text = "Parry [P]", FocusMode = FocusModeEnum.None,
            CustomMinimumSize = new Vector2(109, 0),
            TooltipText = "Tap to parry. Hold to block."
        };
        _parryButton.ButtonDown += () => { _parryButtonHeld = true; UpdateParryHold(); };
        _parryButton.ButtonUp += () => { _parryButtonHeld = false; UpdateParryHold(); };
        row.AddChild(_parryButton);
        row.AddChild(MakeButton("Dodge [I]", () => ActivatePlayerDefense(DefenseKind.Dodge), 0));
        return row;
    }

    private void UpdateCounterUi()
    {
        var trial = _combat.CounterTrial;
        _counterControls.Visible = trial is not null;
        _classicControls.Visible = trial is null;
        _spacingDisplay.Visible = trial is not null;
        foreach (var track in _playerThreatTracks) UpdateLane(track);
        foreach (var track in _opponentThreatTracks) UpdateLane(track);
        if (trial is null) return;

        _rangeStatus.Text = trial.InReach(AttackKind.Light) ? "LIGHT RANGE" :
            trial.InReach(AttackKind.Unparryable) ? "HEAVY RANGE" : "OUT OF RANGE";
        _parryButton.Text = trial.IsGuarding(true, _combat.Time) ? "Block [P]" : "Parry [P]";
        UpdateScanStatus(trial, _combat.Player, _playerScanStatus);
        UpdateScanStatus(trial, _combat.Opponent, _opponentScanStatus);
        _spacingDisplay.QueueRedraw();
        var counter = trial.Counter(true, _combat.Time);
        _counterButton.Text = trial.HasBufferedCounterInput(true) ? "Follow-up buffered" :
            counter is { } ready ? $"{ready} [O] · {Math.Ceiling(trial.CounterRemaining(true, _combat.Time) * 1000):0} ms" :
            "Light [O]";
        _counterButton.Modulate = counter == AttackKind.Unparryable ? new Color("#f4aa82") :
            counter == AttackKind.Undodgeable ? new Color("#9dc9ff") : Colors.White;
        void UpdateLane(LaneTrackUi track)
        {
            track.Column.Visible = trial is null || track.Lane == AttackLane.Mid;
            track.Heading.Visible = trial is null;
        }
    }
}
