using System;
using System.Collections.Generic;
using System.Linq;

namespace CombatPrototype.Core;

public enum AttackKind
{
    Light,
    Heavy,
    LightUnblockable,
    LightUndodgeable,
    Unblockable,
    Undodgeable
}

public enum AttackLane
{
    High,
    Mid,
    Low
}

public enum DefenseKind
{
    Block,
    Parry,
    Dodge,
    Duck,
    Hop
}

public enum AttackOutcome
{
    CleanHit,
    Blocked,
    Dodged,
    Parried
}

public enum ActionPhase
{
    Startup,
    Recovery
}

public enum CombatEventKind
{
    Information,
    AttackQueued,
    AttackLaunched,
    AttackCancelled,
    Defense,
    AttackResolved,
    Stamina,
    InsufficientStamina
}

public readonly record struct AttackIntent(AttackInfo Info, double QueuedAt)
{
    public AttackKind Kind => Info.Kind;
    public AttackLane Lane => Info.Lane;
}

public sealed class CombatTuning
{
    public const double DefaultAiParryReadChance = 0.75;
    public double AiParryReadChance { get; set; } = DefaultAiParryReadChance;
    public double MaxStamina { get; set; } = 100.0;
    public double StartingStamina { get; set; } = 50.0;
    public double LightCost { get; set; } = 10.0;
    public double HeavyCost { get; set; } = 20.0;
    public double StaminaRegenerationPerSecond { get; set; } = 15.0;
    public double SpecialCost { get; set; } = 30.0;
    public double LightSpecialCost { get; set; } = 20.0;
    public double BlockCost { get; set; }
    public double ParryCost { get; set; }
    public double DodgeCost { get; set; }
    public double DuckCost { get; set; }
    public double HopCost { get; set; }

    public double LightAttackerCleanHitGain { get; set; }
    public double LightDefenderCleanHitGain { get; set; }
    public double LightAttackerBlockedGain { get; set; }
    public double LightDefenderBlockedGain { get; set; } = 10.0;
    public double LightAttackerDodgedGain { get; set; }
    public double LightDefenderDodgedGain { get; set; } = 20.0;
    public double LightAttackerParriedGain { get; set; }
    public double LightDefenderParriedGain { get; set; } = 10.0;
    public double HeavyAttackerCleanHitGain { get; set; }
    public double HeavyDefenderCleanHitGain { get; set; }
    public double HeavyAttackerBlockedGain { get; set; }
    public double HeavyDefenderBlockedGain { get; set; } = 10.0;
    public double HeavyAttackerDodgedGain { get; set; }
    public double HeavyDefenderDodgedGain { get; set; } = 20.0;
    public double HeavyAttackerParriedGain { get; set; }
    public double HeavyDefenderParriedGain { get; set; } = 15.0;
    public double SpecialCounterGain { get; set; } = 20.0;

    public double AttackInputInterval { get; set; } = 0.010;
    public double LightStartup { get; set; } = 0.180;
    public double HeavyStartup { get; set; } = 0.400;
    public double HeavyDamage { get; set; } = 24.0;
    public bool LightInterruptsHeavyStartup { get; set; }
    public double FailedDefenseRecovery { get; set; }
    public bool DisableDefenseDuringHeavyStartup { get; set; }
    public double LightSpecialStartup { get; set; } = 0.500;
    public double SpecialStartup { get; set; } = 0.700;
    public double LightUnblockableDamage { get; set; } = 16.0;
    public double LightUndodgeableDamage { get; set; } = 16.0;
    public double UnblockableDamage { get; set; } = 30.0;
    public double UndodgeableDamage { get; set; } = 20.0;
    public bool UseExpandedSpecials { get; set; }
    public double TelegraphLead { get; set; }
    public double ParryWindow { get; set; } = 0.120;
    public double DodgeWindow { get; set; } = 0.180;
    public double DirectionalCounterWindow { get; set; } = 0.180;

    public double LightBlockedAttackerRecovery { get; set; } = 0.050;
    public double LightBlockedDefenderRecovery { get; set; } = 0.050;
    public double HeavyBlockedAttackerRecovery { get; set; } = 0.050;
    public double HeavyBlockedDefenderRecovery { get; set; } = 0.050;
    public double DodgedAttackerRecovery { get; set; } = 0.050;
    public double DodgedDefenderRecovery { get; set; } = 0.050;
    public double ParriedAttackerRecovery { get; set; } = 0.120;
    public double ParriedDefenderRecovery { get; set; } = 0.050;
    public double HitAttackerRecovery { get; set; } = 0.050;
    public double HitDefenderRecovery { get; set; } = 0.050;
    public double SpecialAttackerRecovery { get; set; } = 0.050;
    public double SpecialCounterAttackerRecovery { get; set; } = 0.180;
    public double SpecialDefenderRecovery { get; set; } = 0.050;

    public double BlockActionRecovery { get; set; }
    public double ParryActionRecovery { get; set; }
    public double DodgeActionRecovery { get; set; }
    public double DuckActionRecovery { get; set; }
    public double HopActionRecovery { get; set; }
    public bool DuckCountersHigh { get; set; } = true;
    public bool HopCountersLow { get; set; } = true;

    public double AiCadence { get; set; } = 0.72;
    public double AiInitialDelay { get; set; } = 0.45;
    public double AiReactionDelay { get; set; } = 0.18;
    public bool AiAttackEnabled { get; set; } = true;
    public bool AiDefenseEnabled { get; set; } = true;
    public string AiScript { get; set; } = "Mixed";
}

public sealed class AttackInfo
{
    internal AttackInfo(
        AttackKind kind,
        AttackLane lane,
        string name,
        double damage,
        double staminaCost,
        double startup,
        bool blockable,
        bool parryable,
        bool dodgeable)
    {
        Kind = kind;
        Lane = lane;
        Name = name;
        Damage = damage;
        StaminaCost = staminaCost;
        Startup = startup;
        Blockable = blockable;
        Parryable = parryable;
        Dodgeable = dodgeable;
    }

    public AttackKind Kind { get; }
    public AttackLane Lane { get; }
    public string Name { get; }
    public double Damage { get; }
    public double StaminaCost { get; }
    public double Startup { get; }
    public bool Blockable { get; }
    public bool Parryable { get; }
    public bool Dodgeable { get; }
    public bool IsSpecial => Kind is AttackKind.LightUnblockable or AttackKind.LightUndodgeable
        or AttackKind.Unblockable or AttackKind.Undodgeable;
}

public sealed class AttackState
{
    internal AttackState(
        long id,
        CombatFighter attacker,
        CombatFighter defender,
        AttackInfo info,
        double launchAt,
        double impactAt,
        double resolveAt)
    {
        Id = id;
        Attacker = attacker;
        Defender = defender;
        Info = info;
        LaunchAt = launchAt;
        ImpactAt = impactAt;
        ResolveAt = resolveAt;
        Remaining = impactAt - launchAt;
        Total = impactAt - launchAt;
    }

    public long Id { get; }
    public CombatFighter Attacker { get; }
    public CombatFighter Defender { get; }
    public AttackInfo Info { get; }
    public double LaunchAt { get; }
    public double ImpactAt { get; }
    public double ResolveAt { get; }
    public ActionPhase Phase { get; internal set; } = ActionPhase.Startup;
    public double Remaining { get; internal set; }
    public double Total { get; }
    public bool Resolved { get; internal set; }
    public bool Cancelled { get; internal set; }
    public double? CancelledAt { get; internal set; }
    public AttackOutcome? Outcome { get; internal set; }
    public double Progress => Total <= 0.0 ? 1.0 : Math.Clamp(1.0 - Remaining / Total, 0.0, 1.0);
}

public readonly record struct AttackPreview(AttackInfo Info, double LaunchAt, double ImpactAt, double VisibleAt, bool Launched);

public sealed class DefenseActivation
{
    internal DefenseActivation(DefenseKind kind, double activatedAt)
    {
        Kind = kind;
        ActivatedAt = activatedAt;
    }

    public DefenseKind Kind { get; }
    public double ActivatedAt { get; }
    public bool Consumed { get; internal set; }
    public bool Failed { get; internal set; }
}

public sealed record CombatEvent(double Time, string Message, CombatEventKind Kind);

public sealed record AttackResolution(
    double ImpactTime,
    long AttackId,
    bool AttackerIsPlayer,
    AttackKind AttackKind,
    AttackLane Lane,
    AttackOutcome Outcome,
    DefenseKind? Defense,
    double AttackerStaminaRequested,
    double DefenderStaminaRequested,
    double AttackerReadyAt,
    double DefenderReadyAt,
    double FrameAdvantage,
    double? DefenseTimingOffset,
    double Damage);

public sealed record StaminaChange(
    double Time,
    bool IsPlayer,
    double Requested,
    double Applied,
    double WastedAtCap,
    string Reason);

public sealed record InitiativeChange(double Time, double Delta, double Value, bool IsPlayerOwner, string Reason);

public sealed class CombatFighter
{
    private readonly Queue<AttackIntent> _attackBuffer = new();
    private readonly List<DefenseActivation> _defenseActivations = new();
    private readonly List<(double Start, double End)> _blockHistory = new();

    internal CombatFighter(string name, bool isPlayer, double maxStamina, double startingStamina)
    {
        Name = name;
        IsPlayer = isPlayer;
        MaxStamina = Math.Max(0.0, maxStamina);
        Stamina = Math.Clamp(startingStamina, 0.0, MaxStamina);
        Health = 100.0;
    }

    public string Name { get; }
    public bool IsPlayer { get; }
    public double MaxStamina { get; internal set; }
    public double Stamina { get; internal set; }
    public double Health { get; internal set; }
    public double AttackReadyAt { get; internal set; }
    internal double AttackInputReadyAt { get; set; }
    public double BlockReadyAt { get; internal set; }
    public double ParryReadyAt { get; internal set; }
    public double DodgeReadyAt { get; internal set; }
    public double DuckReadyAt { get; internal set; }
    public double HopReadyAt { get; internal set; }
    public bool BlockHeld { get; internal set; }
    public double BlockHeldSince { get; internal set; }
    public AttackState? CurrentAttack { get; internal set; }
    public string LastDefenseResponse { get; internal set; } = "No response yet";
    public double ReservedStamina { get; internal set; }
    public double AvailableStamina => Math.Max(0.0, Stamina - ReservedStamina);
    public int BufferedAttackCount => _attackBuffer.Count;
    public IReadOnlyCollection<AttackIntent> AttackBuffer => _attackBuffer.ToArray();
    public IReadOnlyCollection<DefenseActivation> DefenseActivations => _defenseActivations.ToArray();
    internal Queue<AttackIntent> MutableAttackBuffer => _attackBuffer;
    internal List<DefenseActivation> MutableDefenseActivations => _defenseActivations;

    internal void CloseBlockInterval(double endAt)
    {
        if (BlockHeld)
        {
            _blockHistory.Add((BlockHeldSince, Math.Max(BlockHeldSince, endAt)));
            BlockHeld = false;
        }
    }

    public bool WasBlockingAt(double time)
    {
        if (BlockHeld && BlockHeldSince <= time)
        {
            return true;
        }

        for (var index = _blockHistory.Count - 1; index >= 0; index--)
        {
            var interval = _blockHistory[index];
            if (interval.Start <= time && time <= interval.End)
            {
                return true;
            }
        }

        return false;
    }

    internal void PruneHistory(double beforeTime)
    {
        _blockHistory.RemoveAll(interval => interval.End < beforeTime);
        _defenseActivations.RemoveAll(action => (action.Consumed || action.Failed) && action.ActivatedAt < beforeTime);
    }

    internal double GetDefenseReadyAt(DefenseKind kind) => kind switch
    {
        DefenseKind.Block => BlockReadyAt,
        DefenseKind.Parry => ParryReadyAt,
        DefenseKind.Dodge => DodgeReadyAt,
        DefenseKind.Duck => DuckReadyAt,
        DefenseKind.Hop => HopReadyAt,
        _ => 0.0
    };

    internal void SetDefenseReadyAt(DefenseKind kind, double value)
    {
        switch (kind)
        {
            case DefenseKind.Block: BlockReadyAt = value; break;
            case DefenseKind.Parry: ParryReadyAt = value; break;
            case DefenseKind.Dodge: DodgeReadyAt = value; break;
            case DefenseKind.Duck: DuckReadyAt = value; break;
            case DefenseKind.Hop: HopReadyAt = value; break;
        }
    }
}

public sealed class CombatEngine
{
    public const double FixedStep = 1.0 / 60.0;
    public const int MaxBufferLength = 8;
    private const double Epsilon = 1e-9;
    private readonly List<CombatEvent> _events = new();
    private readonly List<AttackState> _attackHistory = new();
    private readonly List<AttackResolution> _resolutions = new();
    private readonly List<StaminaChange> _staminaLedger = new();
    private readonly List<InitiativeChange> _initiativeHistory = new();
    private readonly HashSet<long> _aiRespondedTo = new();
    private readonly Random _aiParryReads = new(731);
    private long _nextAttackId;
    private int _aiSequenceIndex;
    private double _aiNextAttackAt;
    private bool _aiOwnsBlock;

    private readonly AttackKind[] _mixedScript =
    [
        AttackKind.Light,
        AttackKind.Undodgeable,
        AttackKind.Unblockable,
        AttackKind.Heavy
    ];
    private readonly AttackKind[] _pressureScript =
    [
        AttackKind.Light,
        AttackKind.Light,
        AttackKind.Heavy,
        AttackKind.Unblockable
    ];
    private readonly AttackKind[] _mixupScript =
    [
        AttackKind.Unblockable,
        AttackKind.Undodgeable,
        AttackKind.Heavy,
        AttackKind.Light
    ];
    private readonly AttackKind[] _expandedMixedScript =
    [
        AttackKind.Light,
        AttackKind.LightUndodgeable,
        AttackKind.Unblockable,
        AttackKind.Heavy,
        AttackKind.LightUnblockable,
        AttackKind.Undodgeable
    ];
    private readonly AttackKind[] _expandedPressureScript =
    [
        AttackKind.Light,
        AttackKind.Light,
        AttackKind.LightUnblockable,
        AttackKind.Heavy,
        AttackKind.Unblockable,
        AttackKind.LightUndodgeable
    ];
    private readonly AttackKind[] _expandedMixupScript =
    [
        AttackKind.LightUnblockable,
        AttackKind.Undodgeable,
        AttackKind.Heavy,
        AttackKind.Light,
        AttackKind.LightUndodgeable,
        AttackKind.Unblockable
    ];

    public CombatEngine(CombatTuning? tuning = null)
    {
        Tuning = tuning ?? new CombatTuning();
        Player = new CombatFighter("Player", true, Tuning.MaxStamina, Tuning.StartingStamina);
        Opponent = new CombatFighter("Opponent", false, Tuning.MaxStamina, Tuning.StartingStamina);
        _aiNextAttackAt = Math.Max(0.0, Tuning.AiInitialDelay);
        Record("Round ready. Both fighters begin at 50 stamina; attack and defense readiness are independent.");
    }

    public CombatTuning Tuning { get; }
    public CombatFighter Player { get; }
    public CombatFighter Opponent { get; }
    public double Time { get; private set; }
    public double Initiative { get; private set; }
    public double LastFrameAdvantage { get; private set; }
    public string InitiativeOwner => Initiative switch
    {
        > 3.0 => "Player",
        < -3.0 => "Opponent",
        _ => "Even"
    };
    public IReadOnlyList<CombatEvent> Events => _events;
    public IReadOnlyList<AttackState> AttackHistory => _attackHistory;
    public IReadOnlyList<AttackResolution> Resolutions => _resolutions;
    public IReadOnlyList<StaminaChange> StaminaLedger => _staminaLedger;
    public IReadOnlyList<InitiativeChange> InitiativeHistory => _initiativeHistory;

    private bool IsDefenseLockedByHeavyCommit(CombatFighter fighter, double at) =>
        Tuning.DisableDefenseDuringHeavyStartup &&
        fighter.CurrentAttack is { Resolved: false, Cancelled: false } attack &&
        attack.Info.Kind is AttackKind.Heavy or AttackKind.Unblockable or AttackKind.Undodgeable &&
        attack.ImpactAt > at + Epsilon;

    public AttackInfo DescribeAttack(AttackKind kind, AttackLane lane = AttackLane.Mid)
    {
        return kind switch
        {
            AttackKind.Light => new AttackInfo(kind, lane, "Light", 12.0,
                Math.Max(0.0, Tuning.LightCost), Math.Max(0.0, Tuning.LightStartup),
                true, true, true),
            AttackKind.Heavy => new AttackInfo(kind, lane, "Heavy",
                Math.Max(0.0, Tuning.HeavyDamage),
                Math.Max(0.0, Tuning.HeavyCost), Math.Max(0.0, Tuning.HeavyStartup),
                true, true, true),
            AttackKind.LightUnblockable => new AttackInfo(kind, lane, "Quick Unblockable",
                Math.Max(0.0, Tuning.LightUnblockableDamage), Math.Max(0.0, Tuning.LightSpecialCost),
                Math.Max(0.0, Tuning.LightSpecialStartup), false, false, true),
            AttackKind.LightUndodgeable => new AttackInfo(kind, lane, "Quick Undodgeable",
                Math.Max(0.0, Tuning.LightUndodgeableDamage), Math.Max(0.0, Tuning.LightSpecialCost),
                Math.Max(0.0, Tuning.LightSpecialStartup), true, true, false),
            AttackKind.Unblockable => new AttackInfo(kind, lane, "Heavy Unblockable",
                Math.Max(0.0, Tuning.UnblockableDamage),
                Math.Max(0.0, Tuning.SpecialCost), Math.Max(0.0, Tuning.SpecialStartup),
                false, false, true),
            AttackKind.Undodgeable => new AttackInfo(kind, lane, "Heavy Undodgeable",
                Math.Max(0.0, Tuning.UndodgeableDamage),
                Math.Max(0.0, Tuning.SpecialCost), Math.Max(0.0, Tuning.SpecialStartup),
                true, true, false),
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };
    }

    public bool QueueAttack(
        CombatFighter fighter,
        AttackKind kind,
        AttackLane lane = AttackLane.Mid,
        double? inputTimeSeconds = null)
    {
        ArgumentNullException.ThrowIfNull(fighter);
        var inputTime = inputTimeSeconds ?? Time;
        if (!double.IsFinite(inputTime) || inputTime < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(inputTimeSeconds), "Input time must be finite and non-negative.");
        }

        if ((kind is AttackKind.LightUnblockable or AttackKind.LightUndodgeable) &&
            !Tuning.UseExpandedSpecials)
        {
            Record($"{fighter.Name} cannot use {kind}; select the expanded-specials timing profile first.",
                CombatEventKind.Information);
            return false;
        }

        if (inputTime + Epsilon < fighter.AttackInputReadyAt)
        {
            Record($"{fighter.Name} cannot queue another attack yet; wait at least " +
                $"{Math.Max(0.0, Tuning.AttackInputInterval) * 1000.0:0.#} ms.", CombatEventKind.Information);
            return false;
        }

        if (!QueueAttackAt(fighter, kind, lane, Time))
            return false;
        fighter.AttackInputReadyAt = inputTime + Math.Max(0.0, Tuning.AttackInputInterval);
        return true;
    }

    public bool SetBlockHeld(CombatFighter fighter, bool held)
    {
        ArgumentNullException.ThrowIfNull(fighter);
        if (held == fighter.BlockHeld)
        {
            return true;
        }

        if (held)
        {
            if (IsDefenseLockedByHeavyCommit(fighter, Time))
            {
                Record($"{fighter.Name} cannot block during Heavy startup until " +
                    $"{fighter.CurrentAttack!.ImpactAt:0.000}s.", CombatEventKind.Defense);
                return false;
            }

            if (Time + Epsilon < fighter.BlockReadyAt)
            {
                Record($"{fighter.Name} cannot block until {fighter.BlockReadyAt:0.000}s.", CombatEventKind.Defense);
                return false;
            }

            if (!TrySpendStamina(fighter, Math.Max(0.0, Tuning.BlockCost), "block"))
            {
                return false;
            }

            fighter.BlockHeld = true;
            fighter.BlockHeldSince = Time;
            Record($"{fighter.Name} is holding block.", CombatEventKind.Defense);
            return true;
        }

        fighter.CloseBlockInterval(Time);
        Record($"{fighter.Name} released block.", CombatEventKind.Defense);
        TryStartBufferedAttack(fighter);
        return true;
    }

    public bool ActivateDefense(CombatFighter fighter, DefenseKind kind)
    {
        ArgumentNullException.ThrowIfNull(fighter);
        if (kind == DefenseKind.Block)
        {
            return SetBlockHeld(fighter, true);
        }

        return ActivateDefenseAt(fighter, kind, Time);
    }

    public void Tick(double delta)
    {
        if (delta <= 0.0)
        {
            return;
        }

        var intervalStart = Time;
        Time += delta;
        SyncStaminaCaps();
        RegenerateStamina(Player, delta);
        RegenerateStamina(Opponent, delta);
        UpdateAttackProgress(Player);
        UpdateAttackProgress(Opponent);
        UpdateScriptedAiDefense(intervalStart);
        ResolveDueAttacks();
        ApplyFailedDefenseRecovery(Player);
        ApplyFailedDefenseRecovery(Opponent);
        UpdateScriptedAiAttack();
        TryStartBufferedAttack(Player);
        TryStartBufferedAttack(Opponent);
    }

    public AttackPreview? GetIncomingThreat(CombatFighter defender)
    {
        var attacker = ReferenceEquals(defender, Player) ? Opponent : Player;
        if (attacker.CurrentAttack is { Resolved: false } active)
        {
            return new AttackPreview(active.Info, active.LaunchAt, active.ImpactAt, active.LaunchAt, true);
        }

        AttackInfo? info = null;
        double launchAt = 0.0;
        if (ReferenceEquals(defender, Player) && Tuning.AiAttackEnabled)
        {
            var script = GetCurrentAiScript();
            var kind = script[_aiSequenceIndex % script.Length];
            var lane = GetScriptLane(_aiSequenceIndex);
            info = DescribeAttack(kind, lane);
            launchAt = Math.Max(_aiNextAttackAt, Opponent.AttackReadyAt);
        }
        else if (ReferenceEquals(defender, Opponent) && Player.MutableAttackBuffer.Count > 0)
        {
            var intent = Player.MutableAttackBuffer.Peek();
            info = intent.Info;
            launchAt = Math.Max(intent.QueuedAt, Player.AttackReadyAt);
        }

        if (info is null)
        {
            return null;
        }

        var lead = Math.Max(0.0, Tuning.TelegraphLead);
        var visibleAtPreview = launchAt - lead;
        return Time + Epsilon >= visibleAtPreview
            ? new AttackPreview(info, launchAt, launchAt + info.Startup, visibleAtPreview, false)
            : null;
    }

    private bool ActivateDefenseAt(CombatFighter fighter, DefenseKind kind, double activatedAt)
    {
        if (kind == DefenseKind.Block)
        {
            return SetBlockHeldAt(fighter, true, activatedAt);
        }

        if (IsDefenseLockedByHeavyCommit(fighter, activatedAt))
        {
            Record($"{fighter.Name} cannot use {kind} during Heavy startup until " +
                $"{fighter.CurrentAttack!.ImpactAt:0.000}s.", CombatEventKind.Defense);
            return false;
        }

        var readyAt = fighter.GetDefenseReadyAt(kind);
        if (activatedAt + Epsilon < readyAt)
        {
            Record($"{fighter.Name} {kind} is not ready until {readyAt:0.000}s.", CombatEventKind.Defense);
            return false;
        }

        var cost = GetDefenseCost(kind);
        if (!TrySpendStamina(fighter, cost, kind.ToString()))
        {
            return false;
        }

        var duration = GetDefenseWindow(kind);
        var action = new DefenseActivation(kind, activatedAt);
        fighter.MutableDefenseActivations.Add(action);
        fighter.SetDefenseReadyAt(kind, activatedAt + GetDefenseRecovery(kind));
        Record($"{fighter.Name} activated {kind}; timing window ±{duration / 2.0:0.000}s around input.",
            CombatEventKind.Defense);
        return true;
    }

    private bool SetBlockHeldAt(CombatFighter fighter, bool held, double at)
    {
        if (held == fighter.BlockHeld)
        {
            return true;
        }

        if (held)
        {
            if (IsDefenseLockedByHeavyCommit(fighter, at))
            {
                Record($"{fighter.Name} cannot block during Heavy startup until " +
                    $"{fighter.CurrentAttack!.ImpactAt:0.000}s.", CombatEventKind.Defense);
                return false;
            }

            if (at + Epsilon < fighter.BlockReadyAt || !TrySpendStamina(fighter, Tuning.BlockCost, "block"))
            {
                return false;
            }

            fighter.BlockHeld = true;
            fighter.BlockHeldSince = at;
            Record($"{fighter.Name} is holding block.", CombatEventKind.Defense);
        }
        else
        {
            fighter.CloseBlockInterval(at);
            Record($"{fighter.Name} released block.", CombatEventKind.Defense);
        }

        return true;
    }

    private void UpdateAttackProgress(CombatFighter fighter)
    {
        if (fighter.CurrentAttack is { } attack)
        {
            attack.Remaining = Math.Max(0.0, attack.ImpactAt - Time);
        }
    }

    private void ResolveDueAttacks()
    {
        while (true)
        {
            var playerAttack = Player.CurrentAttack;
            var opponentAttack = Opponent.CurrentAttack;
            var next = playerAttack is not null && playerAttack.ResolveAt <= Time + Epsilon
                ? playerAttack
                : null;
            if (opponentAttack is not null && opponentAttack.ResolveAt <= Time + Epsilon &&
                (next is null || opponentAttack.ImpactAt < next.ImpactAt ||
                    Math.Abs(opponentAttack.ImpactAt - next.ImpactAt) <= Epsilon && opponentAttack.Id < next.Id))
            {
                next = opponentAttack;
            }

            if (next is null)
            {
                return;
            }

            ResolveAttack(next);
        }
    }

    private void ApplyFailedDefenseRecovery(CombatFighter fighter)
    {
        var recovery = Math.Max(0.0, Tuning.FailedDefenseRecovery);
        if (recovery <= 0.0)
        {
            return;
        }

        foreach (var action in fighter.MutableDefenseActivations)
        {
            if (action.Consumed || action.Failed ||
                action.Kind is not (DefenseKind.Parry or DefenseKind.Dodge))
            {
                continue;
            }

            var failureAt = action.ActivatedAt + GetDefenseWindow(action.Kind) / 2.0;
            if (Time + Epsilon < failureAt)
            {
                continue;
            }

            if (CanPendingAttackConsumeDefense(fighter, action))
            {
                continue;
            }

            action.Failed = true;
            var defenseReadyAt = failureAt + recovery;
            foreach (var kind in Enum.GetValues<DefenseKind>())
            {
                fighter.SetDefenseReadyAt(kind, Math.Max(fighter.GetDefenseReadyAt(kind), defenseReadyAt));
            }

            Record($"{fighter.Name}'s {action.Kind} missed; all defenses are unavailable until " +
                $"{defenseReadyAt:0.000}s.", CombatEventKind.Defense);
        }
    }

    private bool CanPendingAttackConsumeDefense(CombatFighter fighter, DefenseActivation action)
    {
        var attacker = ReferenceEquals(fighter, Player) ? Opponent : Player;
        if (attacker.CurrentAttack is not { Resolved: false, Cancelled: false } attack)
        {
            return false;
        }

        var halfWindow = GetDefenseWindow(action.Kind) / 2.0;
        if (Math.Abs(action.ActivatedAt - attack.ImpactAt) > halfWindow + Epsilon)
        {
            return false;
        }

        return action.Kind switch
        {
            DefenseKind.Parry => attack.Info.Parryable,
            DefenseKind.Dodge => attack.Info.Dodgeable,
            DefenseKind.Duck => attack.Info.Dodgeable && Tuning.DuckCountersHigh &&
                attack.Info.Lane == AttackLane.High,
            DefenseKind.Hop => attack.Info.Dodgeable && Tuning.HopCountersLow &&
                attack.Info.Lane == AttackLane.Low,
            _ => false
        };
    }

    private void ResolveAttack(AttackState attack)
    {
        var attacker = attack.Attacker;
        var defender = attack.Defender;
        var defense = FindDefenseFor(attack);
        var outcome = defense?.Outcome ?? AttackOutcome.CleanHit;
        if (defense?.Activation is { } activation)
        {
            activation.Consumed = true;
        }

        if (outcome == AttackOutcome.CleanHit && TryInterruptHeavyStartup(defender, attack))
        {
            defender.AttackReadyAt = attack.ImpactAt;
        }

        var attackerReadyDelay = GetAttackerRecovery(attack.Info.Kind, outcome);
        var defenderReadyDelay = GetDefenderRecovery(attack.Info.Kind, outcome);
        attacker.AttackReadyAt = Math.Max(attacker.AttackReadyAt, attack.ImpactAt + attackerReadyDelay);
        defender.AttackReadyAt = Math.Max(defender.AttackReadyAt, attack.ImpactAt + defenderReadyDelay);
        if (outcome == AttackOutcome.Blocked)
        {
            defender.BlockReadyAt = Math.Max(defender.BlockReadyAt,
                attack.ImpactAt + Math.Max(0.0, Tuning.BlockActionRecovery));
        }
        else if (outcome == AttackOutcome.Parried)
        {
            defender.ParryReadyAt = Math.Max(defender.ParryReadyAt,
                attack.ImpactAt + Math.Max(0.0, Tuning.ParryActionRecovery));
        }
        else if (outcome == AttackOutcome.Dodged)
        {
            var actionKind = defense?.Name ?? DefenseKind.Dodge;
            defender.SetDefenseReadyAt(actionKind,
                Math.Max(defender.GetDefenseReadyAt(actionKind), attack.ImpactAt + GetDefenseRecovery(actionKind)));
        }

        var damage = outcome == AttackOutcome.CleanHit ? attack.Info.Damage : 0.0;
        defender.LastDefenseResponse = outcome switch
        {
            AttackOutcome.Blocked => $"Blocked {attack.Info.Name}",
            AttackOutcome.Dodged => $"{defense?.Name ?? DefenseKind.Dodge} countered {attack.Info.Name}",
            AttackOutcome.Parried => $"Parried {attack.Info.Name}",
            _ => defender.BlockHeld && !attack.Info.Blockable
                ? "Block failed against an unblockable strike"
                : "No successful defense"
        };
        if (damage > 0.0)
        {
            defender.Health = Math.Max(0.0, defender.Health - damage);
        }

        var (attackerGain, defenderGain) = GetRewards(attack.Info.Kind, outcome);
        var attackerApplied = ChangeStamina(attacker, attackerGain, $"{attack.Info.Name} {outcome}", attack.ImpactAt);
        var defenderApplied = ChangeStamina(defender, defenderGain, $"{attack.Info.Name} {outcome}", attack.ImpactAt);
        ApplyInitiative(attacker, defender, outcome);

        var frameAdvantage = defender.AttackReadyAt - attacker.AttackReadyAt;
        LastFrameAdvantage = frameAdvantage;
        var defenseName = defense?.Name;
        double? defenseTimingOffset = defense?.Activation is { } timedDefense
            ? timedDefense.ActivatedAt - attack.ImpactAt
            : null;
        attack.Resolved = true;
        attack.Outcome = outcome;
        attack.Phase = ActionPhase.Recovery;
        if (ReferenceEquals(attacker.CurrentAttack, attack))
        {
            attacker.CurrentAttack = null;
        }

        if (ReferenceEquals(defender, Opponent) && _aiOwnsBlock)
        {
            SetBlockHeldAt(Opponent, false, Time);
            _aiOwnsBlock = false;
        }

        var resultText = outcome switch
        {
            AttackOutcome.CleanHit => "HIT",
            AttackOutcome.Blocked => "BLOCKED",
            AttackOutcome.Dodged => "DODGED",
            AttackOutcome.Parried => "PARRIED",
            _ => throw new ArgumentOutOfRangeException()
        };
        Record($"{attacker.Name}'s {attack.Info.Lane} {attack.Info.Name} {resultText} at " +
            $"{attack.ImpactAt:0.000}s; frame advantage {frameAdvantage * 1000.0:+0;-0;0}ms " +
            (frameAdvantage > 0.0 ? "(attacker ready first)." : frameAdvantage < 0.0
                ? "(defender ready first)." : "(simultaneous readiness)."), CombatEventKind.AttackResolved);

        _resolutions.Add(new AttackResolution(
            attack.ImpactAt,
            attack.Id,
            attacker.IsPlayer,
            attack.Info.Kind,
            attack.Info.Lane,
            outcome,
            defenseName,
            attackerGain,
            defenderGain,
            attacker.AttackReadyAt,
            defender.AttackReadyAt,
            frameAdvantage,
            defenseTimingOffset,
            damage));
        _ = attackerApplied;
        _ = defenderApplied;
    }

    private bool TryInterruptHeavyStartup(CombatFighter defender, AttackState cleanHit)
    {
        if (!Tuning.LightInterruptsHeavyStartup ||
            cleanHit.Info.Kind != AttackKind.Light ||
            defender.CurrentAttack is not { Resolved: false, Cancelled: false } heavy ||
            heavy.Info.Kind != AttackKind.Heavy ||
            cleanHit.ImpactAt + Epsilon >= heavy.ImpactAt)
        {
            return false;
        }

        heavy.Cancelled = true;
        heavy.CancelledAt = Time;
        heavy.Resolved = true;
        heavy.Phase = ActionPhase.Recovery;
        defender.CurrentAttack = null;
        Record($"{defender.Name}'s Heavy was cancelled by {cleanHit.Attacker.Name}'s clean Light " +
            $"before its {heavy.ImpactAt:0.000}s impact.", CombatEventKind.AttackCancelled);
        return true;
    }

    private DefenseChoice? FindDefenseFor(AttackState attack)
    {
        var defender = attack.Defender;
        var impact = attack.ImpactAt;
        if (IsDefenseLockedByHeavyCommit(defender, impact))
        {
            Record($"{defender.Name} could not defend during Heavy startup.", CombatEventKind.Defense);
            return null;
        }

        var parry = FindTimedDefense(defender, DefenseKind.Parry, impact, Tuning.ParryWindow);
        if (attack.Info.Parryable && parry is not null)
        {
            return new DefenseChoice(AttackOutcome.Parried, DefenseKind.Parry, parry);
        }

        if (attack.Info.Blockable && defender.BlockReadyAt <= impact + Epsilon && defender.WasBlockingAt(impact))
        {
            return new DefenseChoice(AttackOutcome.Blocked, DefenseKind.Block, null);
        }

        var dodge = FindTimedDefense(defender, DefenseKind.Dodge, impact, Tuning.DodgeWindow);
        if (attack.Info.Dodgeable && dodge is not null)
        {
            return new DefenseChoice(AttackOutcome.Dodged, DefenseKind.Dodge, dodge);
        }

        var duck = FindTimedDefense(defender, DefenseKind.Duck, impact, Tuning.DirectionalCounterWindow);
        if (attack.Info.Dodgeable && Tuning.DuckCountersHigh &&
            attack.Info.Lane == AttackLane.High && duck is not null)
        {
            return new DefenseChoice(AttackOutcome.Dodged, DefenseKind.Duck, duck);
        }

        var hop = FindTimedDefense(defender, DefenseKind.Hop, impact, Tuning.DirectionalCounterWindow);
        if (attack.Info.Dodgeable && Tuning.HopCountersLow &&
            attack.Info.Lane == AttackLane.Low && hop is not null)
        {
            return new DefenseChoice(AttackOutcome.Dodged, DefenseKind.Hop, hop);
        }

        if (defender.BlockHeld && !attack.Info.Blockable)
        {
            defender.LastDefenseResponse = "Block failed against an unblockable strike";
        }
        else
        {
            defender.LastDefenseResponse = outcomeTextNone;
        }

        return null;
    }

    private const string outcomeTextNone = "No successful defense";

    private static DefenseActivation? FindTimedDefense(
        CombatFighter fighter,
        DefenseKind kind,
        double impactTime,
        double configuredWindow)
    {
        var halfWindow = Math.Max(0.0, configuredWindow) / 2.0;
        var actions = fighter.MutableDefenseActivations;
        for (var index = actions.Count - 1; index >= 0; index--)
        {
            var action = actions[index];
            if (!action.Consumed && !action.Failed && action.Kind == kind &&
                Math.Abs(action.ActivatedAt - impactTime) <= halfWindow + Epsilon)
            {
                return action;
            }
        }

        return null;
    }

    private (double Attacker, double Defender) GetRewards(AttackKind kind, AttackOutcome outcome)
    {
        if (IsSpecial(kind))
        {
            return outcome == AttackOutcome.CleanHit
                ? (0.0, 0.0)
                : (0.0, Math.Max(0.0, Tuning.SpecialCounterGain));
        }

        return (kind, outcome) switch
        {
            (AttackKind.Light, AttackOutcome.CleanHit) =>
                (Tuning.LightAttackerCleanHitGain, Tuning.LightDefenderCleanHitGain),
            (AttackKind.Light, AttackOutcome.Blocked) =>
                (Tuning.LightAttackerBlockedGain, Tuning.LightDefenderBlockedGain),
            (AttackKind.Light, AttackOutcome.Dodged) =>
                (Tuning.LightAttackerDodgedGain, Tuning.LightDefenderDodgedGain),
            (AttackKind.Light, AttackOutcome.Parried) =>
                (Tuning.LightAttackerParriedGain, Tuning.LightDefenderParriedGain),
            (AttackKind.Heavy, AttackOutcome.CleanHit) =>
                (Tuning.HeavyAttackerCleanHitGain, Tuning.HeavyDefenderCleanHitGain),
            (AttackKind.Heavy, AttackOutcome.Blocked) =>
                (Tuning.HeavyAttackerBlockedGain, Tuning.HeavyDefenderBlockedGain),
            (AttackKind.Heavy, AttackOutcome.Dodged) =>
                (Tuning.HeavyAttackerDodgedGain, Tuning.HeavyDefenderDodgedGain),
            (AttackKind.Heavy, AttackOutcome.Parried) =>
                (Tuning.HeavyAttackerParriedGain, Tuning.HeavyDefenderParriedGain),
            _ => (0.0, 0.0)
        };
    }

    private double GetAttackerRecovery(AttackKind kind, AttackOutcome outcome)
    {
        if (IsSpecial(kind))
        {
            return Math.Max(0.0, outcome == AttackOutcome.CleanHit
                ? Tuning.SpecialAttackerRecovery
                : Tuning.SpecialCounterAttackerRecovery);
        }

        return outcome switch
        {
            AttackOutcome.Blocked when kind == AttackKind.Light => Math.Max(0.0, Tuning.LightBlockedAttackerRecovery),
            AttackOutcome.Blocked => Math.Max(0.0, Tuning.HeavyBlockedAttackerRecovery),
            AttackOutcome.Dodged => Math.Max(0.0, Tuning.DodgedAttackerRecovery),
            AttackOutcome.Parried => Math.Max(0.0, Tuning.ParriedAttackerRecovery),
            _ => Math.Max(0.0, Tuning.HitAttackerRecovery)
        };
    }

    private double GetDefenderRecovery(AttackKind kind, AttackOutcome outcome)
    {
        if (IsSpecial(kind))
        {
            return Math.Max(0.0, Tuning.SpecialDefenderRecovery);
        }

        return outcome switch
        {
            AttackOutcome.Blocked when kind == AttackKind.Light => Math.Max(0.0, Tuning.LightBlockedDefenderRecovery),
            AttackOutcome.Blocked => Math.Max(0.0, Tuning.HeavyBlockedDefenderRecovery),
            AttackOutcome.Dodged => Math.Max(0.0, Tuning.DodgedDefenderRecovery),
            AttackOutcome.Parried => Math.Max(0.0, Tuning.ParriedDefenderRecovery),
            _ => Math.Max(0.0, Tuning.HitDefenderRecovery)
        };
    }

    private static bool IsSpecial(AttackKind kind) =>
        kind is AttackKind.LightUnblockable or AttackKind.LightUndodgeable
            or AttackKind.Unblockable or AttackKind.Undodgeable;

    private void UpdateScriptedAiAttack()
    {
        if (!Tuning.AiAttackEnabled || Opponent.CurrentAttack is not null ||
            Opponent.MutableAttackBuffer.Count > 0 || Time + Epsilon < _aiNextAttackAt)
        {
            return;
        }

        var script = GetCurrentAiScript();
        var kind = script[_aiSequenceIndex % script.Length];
        var lane = GetScriptLane(_aiSequenceIndex);
        var scheduledAt = Math.Max(_aiNextAttackAt, Opponent.AttackReadyAt);
        QueueAttackAt(Opponent, kind, lane, scheduledAt);
        _aiSequenceIndex++;
        _aiNextAttackAt = Math.Max(scheduledAt, Time) + Math.Max(0.15, Tuning.AiCadence);
    }

    private void UpdateScriptedAiDefense(double intervalStart)
    {
        if (!Tuning.AiDefenseEnabled || Player.CurrentAttack is not { Resolved: false } incoming ||
            _aiRespondedTo.Contains(incoming.Id))
        {
            return;
        }

        var preferredActionTime = Math.Min(Math.Max(0.0, Tuning.AiReactionDelay),
            Math.Max(0.0, Tuning.ParryWindow) / 2.0);
        var scheduledAt = incoming.ImpactAt - preferredActionTime;
        if (Time + Epsilon < scheduledAt)
        {
            return;
        }

        _aiRespondedTo.Add(incoming.Id);
        var activationAt = Math.Max(intervalStart, scheduledAt);
        var defense = incoming.Info.Kind switch
        {
            AttackKind.LightUnblockable or AttackKind.Unblockable => DefenseKind.Dodge,
            AttackKind.LightUndodgeable or AttackKind.Undodgeable => DefenseKind.Parry,
            _ => DefenseKind.Block
        };
        if (defense == DefenseKind.Block)
        {
            _aiOwnsBlock = SetBlockHeldAt(Opponent, true, activationAt);
        }
        else
        {
            if (defense == DefenseKind.Parry && Tuning.AiParryReadChance < 1.0 &&
                _aiParryReads.NextDouble() >= Tuning.AiParryReadChance)
                return;
            ActivateDefenseAt(Opponent, defense, activationAt);
        }
    }

    private bool QueueAttackAt(
        CombatFighter fighter,
        AttackKind kind,
        AttackLane lane,
        double queuedAt)
    {
        if (fighter.MutableAttackBuffer.Count >= MaxBufferLength)
        {
            Record($"{fighter.Name} attack queue full; {lane} {kind} was not queued.", CombatEventKind.Information);
            return false;
        }

        var info = DescribeAttack(kind, lane);
        if (fighter.AvailableStamina + Epsilon < info.StaminaCost)
        {
            Record($"{fighter.Name} cannot queue {kind}: needs {info.StaminaCost:0.#}, " +
                $"{fighter.AvailableStamina:0.#} available.", CombatEventKind.InsufficientStamina);
            return false;
        }
        fighter.ReservedStamina += info.StaminaCost;
        fighter.MutableAttackBuffer.Enqueue(new AttackIntent(info, queuedAt));
        Record($"{fighter.Name} queued {lane} {kind} ({fighter.MutableAttackBuffer.Count}/{MaxBufferLength}).",
            CombatEventKind.AttackQueued);
        TryStartBufferedAttack(fighter);
        return true;
    }

    private void TryStartBufferedAttack(CombatFighter fighter)
    {
        if (fighter.CurrentAttack is not null || fighter.BlockHeld || IsDefenseWindowActive(fighter, Time))
        {
            return;
        }

        var incoming = ReferenceEquals(fighter, Player) ? Opponent.CurrentAttack : Player.CurrentAttack;
        if (incoming is { Resolved: false } && incoming.ImpactAt <= Time + Epsilon)
        {
            return;
        }

        if (fighter.MutableAttackBuffer.TryPeek(out var intent))
        {
            var launchAt = Math.Max(Time, Math.Max(intent.QueuedAt, fighter.AttackReadyAt));
            if (launchAt > Time + Epsilon)
            {
                return;
            }

            var info = intent.Info;
            fighter.MutableAttackBuffer.Dequeue();
            fighter.ReservedStamina = fighter.BufferedAttackCount == 0 ? 0.0 :
                Math.Max(0.0, fighter.ReservedStamina - info.StaminaCost);
            if (info.StaminaCost > 0.0)
                ChangeStamina(fighter, -info.StaminaCost, info.Name, Time);
            var impactAt = launchAt + info.Startup;
            var grace = Math.Max(
                Math.Max(0.0, Tuning.ParryWindow),
                Math.Max(Math.Max(0.0, Tuning.DodgeWindow), Math.Max(0.0, Tuning.DirectionalCounterWindow))) / 2.0;
            var attack = new AttackState(++_nextAttackId, fighter,
                ReferenceEquals(fighter, Player) ? Opponent : Player,
                info, launchAt, impactAt, impactAt + grace);
            fighter.CurrentAttack = attack;
            fighter.AttackReadyAt = impactAt;
            _attackHistory.Add(attack);
            ApplyInitiative(fighter, null, null);
            Record($"{fighter.Name} launched {info.Lane} {info.Name} at {launchAt:0.000}s; " +
                $"impact scheduled for {impactAt:0.000}s.", CombatEventKind.AttackLaunched);
        }
    }

    private bool IsDefenseWindowActive(CombatFighter fighter, double at)
    {
        var actions = fighter.MutableDefenseActivations;
        for (var index = 0; index < actions.Count; index++)
        {
            var action = actions[index];
            if (action.Consumed || action.Failed)
            {
                continue;
            }

            var halfWindow = GetDefenseWindow(action.Kind) / 2.0;
            if (Math.Abs(action.ActivatedAt - at) <= halfWindow + Epsilon)
            {
                return true;
            }
        }

        return false;
    }

    private bool TrySpendStamina(CombatFighter fighter, double cost, string reason)
    {
        if (cost <= 0.0)
        {
            return true;
        }

        if (fighter.AvailableStamina + Epsilon < cost)
        {
            Record($"{fighter.Name} cannot afford {reason}: needs {cost:0.#}, has {fighter.AvailableStamina:0.#} available.",
                CombatEventKind.Information);
            return false;
        }

        ChangeStamina(fighter, -cost, reason, Time);
        return true;
    }

    private double ChangeStamina(CombatFighter fighter, double requested, string reason, double eventTime)
    {
        var before = Math.Clamp(fighter.Stamina, 0.0, fighter.MaxStamina);
        var after = Math.Clamp(before + requested, 0.0, fighter.MaxStamina);
        fighter.Stamina = after;
        var applied = after - before;
        var wasted = requested > 0.0 ? requested - applied : 0.0;
        _staminaLedger.Add(new StaminaChange(eventTime, fighter.IsPlayer, requested, applied, wasted, reason));
        if (Math.Abs(applied) > Epsilon || wasted > Epsilon)
        {
            var changeText = applied >= 0.0 ? $"+{applied:0.#}" : $"{applied:0.#}";
            var wasteText = wasted > Epsilon ? $", {wasted:0.#} wasted at cap" : "";
            Record($"{fighter.Name} stamina {changeText} ({reason}; {fighter.Stamina:0.#}/{fighter.MaxStamina:0.#}{wasteText}).",
                CombatEventKind.Stamina);
        }

        return applied;
    }

    private void SyncStaminaCaps()
    {
        var maximum = Math.Max(0.0, Tuning.MaxStamina);
        foreach (var fighter in new[] { Player, Opponent })
        {
            // A live cap edit cannot revoke a reservation already promised to the queue.
            fighter.MaxStamina = Math.Max(maximum, fighter.ReservedStamina);
            fighter.Stamina = Math.Clamp(fighter.Stamina, 0.0, fighter.MaxStamina);
        }
    }

    private void RegenerateStamina(CombatFighter fighter, double delta)
    {
        var regeneration = Math.Max(0.0, Tuning.StaminaRegenerationPerSecond) * delta;
        fighter.Stamina = Math.Min(fighter.MaxStamina, fighter.Stamina + regeneration);
    }

    private void ApplyInitiative(CombatFighter attacker, CombatFighter? defender, AttackOutcome? outcome)
    {
        var amount = outcome switch
        {
            AttackOutcome.Parried => 18.0,
            AttackOutcome.Blocked => 8.0,
            AttackOutcome.Dodged => 13.0,
            AttackOutcome.CleanHit => 6.0,
            _ => 2.0
        };
        var owner = outcome is AttackOutcome.Parried or AttackOutcome.Blocked or AttackOutcome.Dodged
            ? defender
            : attacker;
        if (owner is null)
        {
            owner = attacker;
        }

        var delta = owner.IsPlayer ? amount : -amount;
        var previous = Initiative;
        Initiative = Math.Clamp(Initiative + delta, -100.0, 100.0);
        var applied = Initiative - previous;
        _initiativeHistory.Add(new InitiativeChange(Time, applied, Initiative, InitiativeOwner == "Player",
            outcome?.ToString() ?? "Attack launched"));
        if (outcome is not null)
        {
            Record($"Initiative → {InitiativeOwner} ({Initiative:+0;-0;0}).", CombatEventKind.Information);
        }
    }

    private double GetDefenseCost(DefenseKind kind) => Math.Max(0.0, kind switch
    {
        DefenseKind.Block => Tuning.BlockCost,
        DefenseKind.Parry => Tuning.ParryCost,
        DefenseKind.Dodge => Tuning.DodgeCost,
        DefenseKind.Duck => Tuning.DuckCost,
        DefenseKind.Hop => Tuning.HopCost,
        _ => 0.0
    });

    private double GetDefenseWindow(DefenseKind kind) => Math.Max(0.0, kind switch
    {
        DefenseKind.Parry => Tuning.ParryWindow,
        DefenseKind.Dodge => Tuning.DodgeWindow,
        DefenseKind.Duck or DefenseKind.Hop => Tuning.DirectionalCounterWindow,
        _ => 0.0
    });

    private double GetDefenseRecovery(DefenseKind kind) => Math.Max(0.0, kind switch
    {
        DefenseKind.Block => Tuning.BlockActionRecovery,
        DefenseKind.Parry => Tuning.ParryActionRecovery,
        DefenseKind.Dodge => Tuning.DodgeActionRecovery,
        DefenseKind.Duck => Tuning.DuckActionRecovery,
        DefenseKind.Hop => Tuning.HopActionRecovery,
        _ => 0.0
    });

    private AttackKind[] GetCurrentAiScript()
    {
        if (Tuning.UseExpandedSpecials)
        {
            return Tuning.AiScript switch
            {
                "Pressure" => _expandedPressureScript,
                "Mix-up" => _expandedMixupScript,
                _ => _expandedMixedScript
            };
        }

        return Tuning.AiScript switch
        {
            "Pressure" => _pressureScript,
            "Mix-up" => _mixupScript,
            _ => _mixedScript
        };
    }

    private static AttackLane GetScriptLane(int index) => (index % 3) switch
    {
        0 => AttackLane.Mid,
        1 => AttackLane.High,
        _ => AttackLane.Low
    };

    private void Record(string message, CombatEventKind kind = CombatEventKind.Information) =>
        _events.Add(new CombatEvent(Time, message, kind));

    private sealed record DefenseChoice(
        AttackOutcome Outcome,
        DefenseKind Name,
        DefenseActivation? Activation);
}
