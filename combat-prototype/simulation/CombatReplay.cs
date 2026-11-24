using System.Reflection;
using CombatPrototype.Core;

namespace CombatPrototype.Simulation;

public sealed class CombatReplay
{
    public int SchemaVersion { get; init; } = 1;
    public ulong Seed { get; init; }
    public string Profile { get; init; } = "";
    public string PlayerAgent { get; init; } = "";
    public string OpponentAgent { get; init; } = "";
    public ReactionModel PlayerReaction { get; init; } = new();
    public ReactionModel OpponentReaction { get; init; } = new();
    public Dictionary<string, object?> Configuration { get; init; } = new(StringComparer.Ordinal);
    public string InterruptionBehavior { get; init; } =
        "Attacks are not interrupted or cancelled by hits; hitstun is not implemented.";
    public int Winner { get; set; }
    public string WinnerName { get; set; } = "Draw";
    public string EndReason { get; set; } = "";
    public double DurationMs { get; set; }
    public List<CombatReplayEvent> Events { get; } = [];
}

public sealed record ReplayQueuedAttack(
    string Kind,
    string Lane,
    double QueuedAtMs);

public sealed record ReplayFighterSnapshot(
    string Id,
    double Health,
    double Stamina,
    string CurrentAction,
    long? CurrentAttackId,
    string? CurrentAttackKind,
    string? CurrentAttackLane,
    double? CurrentAttackLaunchAtMs,
    double? CurrentAttackImpactAtMs,
    double AttackReadyAtMs,
    double BlockReadyAtMs,
    double ParryReadyAtMs,
    double DodgeReadyAtMs,
    double DuckReadyAtMs,
    double HopReadyAtMs,
    bool BlockHeld,
    IReadOnlyList<ReplayQueuedAttack> AttackQueue);

public sealed class CombatReplayEvent
{
    public int Sequence { get; init; }
    public string Type { get; init; } = "";
    public double TimeMs { get; init; }
    public long Frame { get; init; }
    public double? ScheduledTimeMs { get; init; }
    public string? FighterId { get; init; }
    public string? TargetFighterId { get; init; }
    public string? Message { get; init; }
    public long? AttackId { get; init; }
    public long? TargetAttackId { get; init; }
    public string? AttackKind { get; init; }
    public string? AttackLane { get; init; }
    public double? QueuedAtMs { get; init; }
    public double? LaunchAtMs { get; init; }
    public double? ScheduledImpactAtMs { get; init; }
    public double? ResolveAtMs { get; init; }
    public string? DefenseKind { get; init; }
    public string? DefenseOutcome { get; init; }
    public double? DefenseSelectedAtMs { get; init; }
    public double? DefenseActivatedAtMs { get; init; }
    public double? DefenseWindowStartMs { get; init; }
    public double? DefenseWindowEndMs { get; init; }
    public double? AgentEstimatedImpactAtMs { get; init; }
    public double? AgentImpactEstimateUncertaintyMs { get; init; }
    public double? VisibleAtMs { get; init; }
    public double? FirstObservedAtMs { get; init; }
    public double? SelectionLatencyMs { get; init; }
    public double? ReactionTimeMs { get; init; }
    public double? AvailableReactionTimeMs { get; init; }
    public double? ModeledReactionDelayMs { get; init; }
    public double? TimingErrorMs { get; init; }
    public string? InformationAvailable { get; init; }
    public bool? UsedFutureInformation { get; init; }
    public bool? DefenseSucceeded { get; init; }
    public int? DefenseAttemptId { get; set; }
    public int[] RelatedDefenseAttemptIds { get; init; } = [];
    public string[] AttemptedDefenseKinds { get; init; } = [];
    public double? Damage { get; init; }
    public double? StaminaRequested { get; init; }
    public double? StaminaApplied { get; init; }
    public double? StaminaWastedAtCap { get; init; }
    public string? StaminaReason { get; init; }
    public double? AttackerRecoveryStartMs { get; init; }
    public double? AttackerRecoveryEndMs { get; init; }
    public double? DefenderRecoveryStartMs { get; init; }
    public double? DefenderRecoveryEndMs { get; init; }
    public double? HitstunStartMs { get; init; }
    public double? HitstunEndMs { get; init; }
    public bool? AttackCancelled { get; init; }
    public double? FrameAdvantageMs { get; init; }
    public double Initiative { get; init; }
    public string InitiativeOwner { get; init; } = "Even";
    public ReplayFighterSnapshot Player { get; init; } = null!;
    public ReplayFighterSnapshot Opponent { get; init; } = null!;
}

public sealed record AgentDefenseTrace(
    DefenseKind Defense,
    long? ThreatId,
    AttackKind? ThreatKind,
    AttackLane? ThreatLane,
    double? EstimatedImpactAt,
    double? ImpactEstimateUncertaintySeconds,
    double? VisibleAt,
    double SelectedAt,
    double PlannedActivationAt,
    bool IsPrediction,
    bool IsIncorrectChoice,
    double? SampledReactionDelaySeconds = null,
    double? TimingErrorSeconds = null);

public interface IAgentDecisionTraceProvider
{
    AgentDefenseTrace? LastDefenseTrace { get; }
}

public sealed class CombatReplayRecorder
{
    private readonly Dictionary<string, double> _firstObservedAt = new(StringComparer.Ordinal);
    private readonly HashSet<long> _loggedAttacks = [];
    private readonly HashSet<long> _loggedImpacts = [];
    private readonly HashSet<long> _loggedCancellations = [];
    private readonly HashSet<string> _loggedRecoveries = new(StringComparer.Ordinal);
    private readonly Dictionary<long, AttackState> _attacks = [];
    private readonly List<(long AttackId, bool IsPlayer, double ReadyAt)> _pendingRecoveries = [];
    private readonly Dictionary<string, Queue<double>> _queuedAttackTimes = new(StringComparer.Ordinal);
    private int _engineEventIndex;
    private int _staminaIndex;
    private int _resolutionIndex;
    private int _initiativeIndex;

    public CombatReplayRecorder(
        ulong seed,
        string profile,
        string playerAgent,
        string opponentAgent,
        ReactionModel playerReaction,
        ReactionModel opponentReaction,
        CombatTuning tuning)
    {
        Replay = new CombatReplay
        {
            Seed = seed,
            Profile = profile,
            PlayerAgent = playerAgent,
            OpponentAgent = opponentAgent,
            PlayerReaction = playerReaction,
            OpponentReaction = opponentReaction,
            Configuration = ReadConfiguration(tuning),
            InterruptionBehavior = tuning.LightInterruptsHeavyStartup
                ? "A clean normal Light cancels an opposing normal Heavy before its scheduled impact."
                : "Hits do not interrupt active attacks; no separate hitstun state is implemented."
        };
    }

    public CombatReplay Replay { get; }

    public void Start(CombatEngine engine)
    {
        Add(engine, 0, "match.started", "Initial state.", scheduledTime: 0.0);
        CaptureChanges(engine, 0);
    }

    public void RecordIncoming(
        CombatEngine engine,
        long frame,
        CombatFighter fighter,
        AgentThreatObservation? incoming,
        AttackPreview? actualThreat)
    {
        if (incoming is not { } threat)
        {
            return;
        }

        if (actualThreat is not { } truth)
        {
            return;
        }

        var key = ThreatKey(fighter, truth.LaunchAt, truth.Info.Kind);
        if (_firstObservedAt.ContainsKey(key))
        {
            return;
        }

        _firstObservedAt.Add(key, engine.Time);
        Add(engine, frame, "threat.visible",
            $"{fighter.Name} first observes a public incoming threat.",
            fighter: fighter,
            target: OpponentOf(engine, fighter),
            attackKind: threat.Kind,
            attackLane: threat.Lane,
            launchAt: truth.LaunchAt,
            impactAt: truth.ImpactAt,
            scheduledTime: truth.VisibleAt,
            visibleAt: truth.VisibleAt,
            firstObservedAt: engine.Time,
            availableReactionTime: truth.ImpactAt - engine.Time,
            agentEstimatedImpactAt: threat.EstimatedImpactAt,
            agentImpactEstimateUncertainty: threat.ImpactEstimateUncertaintySeconds,
            informationAvailable: $"kind={threat.Kind}; lane={threat.Lane}; " +
                $"launched={threat.Launched}; estimatedImpactAt={Ms(threat.EstimatedImpactAt):0.###}ms; " +
                $"estimateUncertainty={Ms(threat.ImpactEstimateUncertaintySeconds):0.###}ms");
    }

    public void RecordDefenseSelection(
        CombatEngine engine,
        long frame,
        CombatFighter fighter,
        AgentDefenseTrace trace)
    {
        var opponent = OpponentOf(engine, fighter);
        var actualThreat = trace.ThreatId is null ? null : engine.GetIncomingThreat(fighter);
        var matchesThreat = actualThreat is { } threat &&
            threat.Info.Kind == trace.ThreatKind &&
            threat.Info.Lane == trace.ThreatLane &&
            Math.Abs(threat.VisibleAt - trace.VisibleAt.GetValueOrDefault()) < 1e-9;
        var launchAt = matchesThreat ? actualThreat!.Value.LaunchAt : (double?)null;
        var impactAt = matchesThreat ? actualThreat!.Value.ImpactAt : (double?)null;
        var firstObservedAt = launchAt is { } observedLaunch &&
            trace.ThreatKind is { } threatKind
                ? FirstObservedAt(fighter, observedLaunch, threatKind)
                : null;
        Add(engine, frame, trace.IsPrediction ? "defense.predicted" : "defense.selected",
            trace.IsIncorrectChoice ? "Agent selected an intentionally incorrect response." :
                "Agent selected a defensive response.",
            fighter: fighter,
            target: opponent,
            attackKind: trace.ThreatKind,
            attackLane: trace.ThreatLane,
            launchAt: launchAt,
            impactAt: impactAt,
            defenseKind: trace.Defense,
            defenseSelectedAt: trace.SelectedAt,
            defenseActivatedAt: trace.PlannedActivationAt,
            visibleAt: trace.VisibleAt,
            firstObservedAt: firstObservedAt,
            selectionLatency: firstObservedAt is { } selectedAt
                ? trace.SelectedAt - selectedAt
                : null,
            reactionTime: firstObservedAt is { } observationTime
                ? trace.PlannedActivationAt - observationTime
                : null,
            agentEstimatedImpactAt: trace.EstimatedImpactAt,
            agentImpactEstimateUncertainty: trace.ImpactEstimateUncertaintySeconds,
            modeledReactionDelay: trace.SampledReactionDelaySeconds,
            timingError: trace.TimingErrorSeconds,
            availableReactionTime: firstObservedAt is { } observationAt &&
                impactAt is { } actualImpactAt
                ? actualImpactAt - observationAt
                : null,
            informationAvailable: trace.IsPrediction
                ? "No incoming threat was visible; decision was a prediction."
                : $"kind={trace.ThreatKind}; lane={trace.ThreatLane}; " +
                    $"estimatedImpactAt={FormatMs(trace.EstimatedImpactAt)}ms; " +
                    $"uncertainty={FormatMs(trace.ImpactEstimateUncertaintySeconds)}ms",
            usedFutureInformation: firstObservedAt is { } observedAt &&
                trace.SelectedAt < observedAt - 1e-9);
    }

    public void RecordAttackQueued(
        CombatEngine engine,
        long frame,
        CombatFighter fighter,
        AttackKind kind,
        AttackLane lane,
        bool accepted)
    {
        if (accepted)
        {
            var key = QueueKey(fighter, kind, lane);
            if (!_queuedAttackTimes.TryGetValue(key, out var times))
            {
                times = new Queue<double>();
                _queuedAttackTimes.Add(key, times);
            }

            times.Enqueue(engine.Time);
        }

        Add(engine, frame, accepted ? "attack.queued" : "attack.queue_rejected",
            accepted ? "Attack input entered the buffer." : "Attack input was rejected.",
            fighter: fighter,
            target: OpponentOf(engine, fighter),
            attackKind: kind,
            attackLane: lane,
            queuedAt: engine.Time);
    }

    public void RecordDefenseInput(
        CombatEngine engine,
        long frame,
        CombatFighter fighter,
        DefenseKind kind,
        bool accepted,
        bool? holdBlock = null,
        AttackPreview? incoming = null)
    {
        var window = kind switch
        {
            DefenseKind.Parry => engine.Tuning.ParryWindow,
            DefenseKind.Dodge => engine.Tuning.DodgeWindow,
            DefenseKind.Duck or DefenseKind.Hop => engine.Tuning.DirectionalCounterWindow,
            _ => 0.0
        };
        var activationAt = engine.Time;
        var firstObservedAt = incoming is { } threat
            ? FirstObservedAt(fighter, threat.LaunchAt, threat.Info.Kind)
            : null;
        var attemptId = Replay.Events.Count;
        Add(engine, frame, accepted ? "defense.input" : "defense.input_rejected",
            holdBlock is { } held ? (held ? "Block hold requested." : "Block release requested.") :
                "Timed or directional defense requested.",
            fighter: fighter,
            target: OpponentOf(engine, fighter),
            attackKind: incoming?.Info.Kind,
            attackLane: incoming?.Info.Lane,
            launchAt: incoming?.LaunchAt,
            impactAt: incoming?.ImpactAt,
            defenseKind: kind,
            defenseActivatedAt: activationAt,
            defenseWindowStart: window > 0 ? activationAt - window / 2.0 : null,
            defenseWindowEnd: window > 0 ? activationAt + window / 2.0 : null,
            visibleAt: incoming?.VisibleAt,
            firstObservedAt: firstObservedAt,
            reactionTime: firstObservedAt is { } observedAt ? activationAt - observedAt : null,
            availableReactionTime: firstObservedAt is { } firstSeen && incoming is { } visibleThreatValue
                ? visibleThreatValue.ImpactAt - firstSeen
                : null,
            informationAvailable: incoming is { } visibleThreat
                ? $"kind={visibleThreat.Info.Kind}; lane={visibleThreat.Info.Lane}; " +
                    $"launchAt={Ms(visibleThreat.LaunchAt):0.###}ms; impactAt={Ms(visibleThreat.ImpactAt):0.###}ms"
                : "No incoming threat was visible to this decision.",
            usedFutureInformation: incoming is { } publicThreat && engine.Time < publicThreat.VisibleAt - 1e-9);
        Replay.Events[^1].DefenseAttemptId = attemptId;
        CaptureChanges(engine, frame);
    }

    public void CaptureChanges(CombatEngine engine, long frame)
    {
        while (_engineEventIndex < engine.Events.Count)
        {
            var item = engine.Events[_engineEventIndex++];
            Add(engine, frame, $"engine.{item.Kind.ToString().ToLowerInvariant()}", item.Message,
                scheduledTime: item.Time);
        }

        while (_staminaIndex < engine.StaminaLedger.Count)
        {
            var change = engine.StaminaLedger[_staminaIndex++];
            var fighter = change.IsPlayer ? engine.Player : engine.Opponent;
            Add(engine, frame, "stamina.changed", change.Reason,
                fighter: fighter,
                scheduledTime: change.Time,
                staminaRequested: change.Requested,
                staminaApplied: change.Applied,
                staminaWasted: change.WastedAtCap,
                staminaReason: change.Reason);
        }

        var newlyLaunched = new List<AttackState>();
        foreach (var attack in engine.AttackHistory)
        {
            if (_loggedAttacks.Add(attack.Id))
            {
                _attacks[attack.Id] = attack;
                newlyLaunched.Add(attack);
            }
        }

        foreach (var attack in engine.AttackHistory)
        {
            if (attack.Cancelled && _loggedCancellations.Add(attack.Id))
            {
                Add(engine, frame, "attack.cancelled",
                    $"Attack cancelled before its scheduled impact at {attack.ImpactAt:0.000}s.",
                    fighter: attack.Attacker,
                    target: attack.Defender,
                    attackId: attack.Id,
                    attackKind: attack.Info.Kind,
                    attackLane: attack.Info.Lane,
                    launchAt: attack.LaunchAt,
                    impactAt: attack.ImpactAt,
                    resolveAt: attack.ResolveAt,
                    scheduledTime: attack.CancelledAt ?? engine.Time);
            }

            if (!attack.Cancelled &&
                engine.Time + 1e-9 >= attack.ImpactAt &&
                _loggedImpacts.Add(attack.Id))
            {
                Add(engine, frame, "attack.impact_reached",
                    "Scheduled impact time reached; outcome is not yet resolved.",
                    fighter: attack.Attacker,
                    target: attack.Defender,
                    attackId: attack.Id,
                    attackKind: attack.Info.Kind,
                    attackLane: attack.Info.Lane,
                    launchAt: attack.LaunchAt,
                    impactAt: attack.ImpactAt,
                    resolveAt: attack.ResolveAt,
                    scheduledTime: attack.ImpactAt);
            }
        }

        while (_resolutionIndex < engine.Resolutions.Count)
        {
            var resolution = engine.Resolutions[_resolutionIndex++];
            if (!_attacks.TryGetValue(resolution.AttackId, out var attack))
            {
                attack = engine.AttackHistory.Single(item => item.Id == resolution.AttackId);
                _attacks[attack.Id] = attack;
            }

            Add(engine, frame, "attack.resolved",
                $"Scheduled impact resolved as {resolution.Outcome}; damage={resolution.Damage:0.###}.",
                fighter: attack.Attacker,
                target: attack.Defender,
                attackId: attack.Id,
                attackKind: resolution.AttackKind,
                attackLane: resolution.Lane,
                launchAt: attack.LaunchAt,
                impactAt: resolution.ImpactTime,
                resolveAt: attack.ResolveAt,
                defenseKind: resolution.Defense,
                defenseOutcome: resolution.Outcome,
                damage: resolution.Damage,
                attackerRecoveryStart: resolution.ImpactTime,
                attackerRecoveryEnd: resolution.AttackerReadyAt,
                defenderRecoveryStart: resolution.ImpactTime,
                defenderRecoveryEnd: resolution.DefenderReadyAt,
                hitstunStart: null,
                hitstunEnd: null,
                frameAdvantage: resolution.FrameAdvantage,
                defenseSucceeded: resolution.Defense is not null,
                relatedDefenseAttemptIds: FindDefenseAttempts(attack, resolution.ImpactTime),
                attemptedDefenseKinds: FindDefenseAttempts(attack, resolution.ImpactTime)
                    .Select(attemptId => Replay.Events[attemptId].DefenseKind ?? "")
                    .Where(kind => kind.Length > 0)
                    .Distinct(StringComparer.Ordinal)
                    .ToArray());

            _pendingRecoveries.Add((attack.Id, attack.Attacker.IsPlayer, resolution.AttackerReadyAt));
            _pendingRecoveries.Add((attack.Id, attack.Defender.IsPlayer, resolution.DefenderReadyAt));

            Add(engine, frame, "recovery.started",
                "Attacker recovery is measured from the scheduled impact.",
                fighter: attack.Attacker,
                attackId: attack.Id,
                scheduledTime: resolution.ImpactTime,
                attackerRecoveryStart: resolution.ImpactTime,
                attackerRecoveryEnd: resolution.AttackerReadyAt);
            Add(engine, frame, "recovery.started",
                "Defender attack-readiness recovery is measured from the scheduled impact.",
                fighter: attack.Defender,
                attackId: attack.Id,
                scheduledTime: resolution.ImpactTime,
                defenderRecoveryStart: resolution.ImpactTime,
                defenderRecoveryEnd: resolution.DefenderReadyAt);

            if (resolution.Outcome == AttackOutcome.CleanHit &&
                attack.Defender.CurrentAttack is { Resolved: false } targetAttack &&
                targetAttack.LaunchAt <= resolution.ImpactTime + 1e-9 &&
                targetAttack.ImpactAt > resolution.ImpactTime + 1e-9)
            {
                Add(engine, frame, "attack.interruption_not_applied",
                    "Clean hit did not cancel the defender's active attack.",
                    fighter: attack.Attacker,
                    target: attack.Defender,
                    attackId: attack.Id,
                    targetAttackId: targetAttack.Id,
                    attackCancelled: false);
            }
        }

        foreach (var attack in newlyLaunched)
        {
            Add(engine, frame, "attack.launched",
                "Attack began startup; this is not an impact or a confirmed hit.",
                fighter: attack.Attacker,
                target: attack.Defender,
                attackId: attack.Id,
                attackKind: attack.Info.Kind,
                attackLane: attack.Info.Lane,
                queuedAt: ConsumeQueuedAt(attack),
                launchAt: attack.LaunchAt,
                impactAt: attack.ImpactAt,
                resolveAt: attack.ResolveAt);
        }

        while (_initiativeIndex < engine.InitiativeHistory.Count)
        {
            var change = engine.InitiativeHistory[_initiativeIndex++];
            Add(engine, frame, "initiative.changed", change.Reason,
                scheduledTime: change.Time,
                frameAdvantage: null);
        }

        for (var index = _pendingRecoveries.Count - 1; index >= 0; index--)
        {
            var recovery = _pendingRecoveries[index];
            if (engine.Time + 1e-9 < recovery.ReadyAt)
            {
                continue;
            }

            var key = $"{recovery.AttackId}:{recovery.IsPlayer}";
            if (_loggedRecoveries.Add(key))
            {
                Add(engine, frame, "recovery.completed",
                    "Attack readiness timestamp reached.",
                    fighter: recovery.IsPlayer ? engine.Player : engine.Opponent,
                    attackId: recovery.AttackId,
                    scheduledTime: recovery.ReadyAt);
            }

            _pendingRecoveries.RemoveAt(index);
        }
    }

    internal void Complete(CombatEngine engine, long frame, MatchResult result)
    {
        Replay.Winner = result.Winner;
        Replay.WinnerName = result.Winner switch
        {
            1 => Replay.PlayerAgent,
            2 => Replay.OpponentAgent,
            _ => "Draw"
        };
        Replay.EndReason = result.EndReason;
        Replay.DurationMs = Ms(result.Duration);
        CaptureChanges(engine, frame);
        Add(engine, frame, "match.completed",
            $"Winner={Replay.WinnerName}; endReason={Replay.EndReason}.",
            scheduledTime: engine.Time);
    }

    private void Add(
        CombatEngine engine,
        long frame,
        string type,
        string message,
        CombatFighter? fighter = null,
        CombatFighter? target = null,
        long? attackId = null,
        long? targetAttackId = null,
        AttackKind? attackKind = null,
        AttackLane? attackLane = null,
        double? queuedAt = null,
        double? launchAt = null,
        double? impactAt = null,
        double? resolveAt = null,
        DefenseKind? defenseKind = null,
        AttackOutcome? defenseOutcome = null,
        double? defenseSelectedAt = null,
        double? defenseActivatedAt = null,
        double? defenseWindowStart = null,
        double? defenseWindowEnd = null,
        double? agentEstimatedImpactAt = null,
        double? agentImpactEstimateUncertainty = null,
        double? visibleAt = null,
        double? reactionTime = null,
        double? availableReactionTime = null,
        double? modeledReactionDelay = null,
        double? timingError = null,
        string? informationAvailable = null,
        bool? usedFutureInformation = null,
        double? firstObservedAt = null,
        double? selectionLatency = null,
        bool? defenseSucceeded = null,
        int? defenseAttemptId = null,
        int[]? relatedDefenseAttemptIds = null,
        string[]? attemptedDefenseKinds = null,
        double? damage = null,
        double? staminaRequested = null,
        double? staminaApplied = null,
        double? staminaWasted = null,
        string? staminaReason = null,
        double? attackerRecoveryStart = null,
        double? attackerRecoveryEnd = null,
        double? defenderRecoveryStart = null,
        double? defenderRecoveryEnd = null,
        double? hitstunStart = null,
        double? hitstunEnd = null,
        bool? attackCancelled = null,
        double? frameAdvantage = null,
        double? scheduledTime = null)
    {
        Replay.Events.Add(new CombatReplayEvent
        {
            Sequence = Replay.Events.Count,
            Type = type,
            TimeMs = Ms(engine.Time),
            Frame = frame,
            ScheduledTimeMs = scheduledTime is { } scheduled ? Ms(scheduled) : null,
            FighterId = fighter is null ? "System" : Id(fighter),
            TargetFighterId = target is null ? null : Id(target),
            Message = message,
            AttackId = attackId,
            TargetAttackId = targetAttackId,
            AttackKind = attackKind?.ToString(),
            AttackLane = attackLane?.ToString(),
            QueuedAtMs = queuedAt is { } queued ? Ms(queued) : null,
            LaunchAtMs = launchAt is { } launch ? Ms(launch) : null,
            ScheduledImpactAtMs = impactAt is { } impact ? Ms(impact) : null,
            ResolveAtMs = resolveAt is { } resolve ? Ms(resolve) : null,
            DefenseKind = defenseKind?.ToString(),
            DefenseOutcome = defenseOutcome?.ToString(),
            DefenseSelectedAtMs = defenseSelectedAt is { } selected ? Ms(selected) : null,
            DefenseActivatedAtMs = defenseActivatedAt is { } activated ? Ms(activated) : null,
            DefenseWindowStartMs = defenseWindowStart is { } windowStart ? Ms(windowStart) : null,
            DefenseWindowEndMs = defenseWindowEnd is { } windowEnd ? Ms(windowEnd) : null,
            AgentEstimatedImpactAtMs = agentEstimatedImpactAt is { } estimate ? Ms(estimate) : null,
            AgentImpactEstimateUncertaintyMs = agentImpactEstimateUncertainty is { } uncertainty
                ? Ms(uncertainty) : null,
            VisibleAtMs = visibleAt is { } visible ? Ms(visible) : null,
            FirstObservedAtMs = firstObservedAt is { } firstSeen ? Ms(firstSeen) : null,
            SelectionLatencyMs = selectionLatency is { } selectedLatency ? Ms(selectedLatency) : null,
            ReactionTimeMs = reactionTime is { } reaction ? Ms(reaction) : null,
            AvailableReactionTimeMs = availableReactionTime is { } available ? Ms(available) : null,
            ModeledReactionDelayMs = modeledReactionDelay is { } delay ? Ms(delay) : null,
            TimingErrorMs = timingError is { } error ? Ms(error) : null,
            InformationAvailable = informationAvailable,
            UsedFutureInformation = usedFutureInformation,
            DefenseSucceeded = defenseSucceeded,
            DefenseAttemptId = defenseAttemptId,
            RelatedDefenseAttemptIds = relatedDefenseAttemptIds ?? [],
            AttemptedDefenseKinds = attemptedDefenseKinds ?? [],
            Damage = damage,
            StaminaRequested = staminaRequested,
            StaminaApplied = staminaApplied,
            StaminaWastedAtCap = staminaWasted,
            StaminaReason = staminaReason,
            AttackerRecoveryStartMs = attackerRecoveryStart is { } attackerStart ? Ms(attackerStart) : null,
            AttackerRecoveryEndMs = attackerRecoveryEnd is { } attackerEnd ? Ms(attackerEnd) : null,
            DefenderRecoveryStartMs = defenderRecoveryStart is { } defenderStart ? Ms(defenderStart) : null,
            DefenderRecoveryEndMs = defenderRecoveryEnd is { } defenderEnd ? Ms(defenderEnd) : null,
            HitstunStartMs = hitstunStart is { } hitStart ? Ms(hitStart) : null,
            HitstunEndMs = hitstunEnd is { } hitEnd ? Ms(hitEnd) : null,
            AttackCancelled = attackCancelled,
            FrameAdvantageMs = frameAdvantage is { } advantage ? Ms(advantage) : null,
            Initiative = engine.Initiative,
            InitiativeOwner = engine.InitiativeOwner,
            Player = Snapshot(engine, engine.Player),
            Opponent = Snapshot(engine, engine.Opponent)
        });
    }

    private ReplayFighterSnapshot Snapshot(CombatEngine engine, CombatFighter fighter)
    {
        var attack = fighter.CurrentAttack;
        var action = attack is not null
            ? $"{attack.Info.Kind} {attack.Phase}"
            : fighter.BlockHeld
                ? "Block"
                : fighter.AttackReadyAt > engine.Time + 1e-9
                    ? "Recovery"
                    : fighter.BufferedAttackCount > 0
                        ? "Queued"
                        : "Ready";

        return new ReplayFighterSnapshot(
            Id(fighter),
            fighter.Health,
            fighter.Stamina,
            action,
            attack?.Id,
            attack?.Info.Kind.ToString(),
            attack?.Info.Lane.ToString(),
            attack is null ? null : Ms(attack.LaunchAt),
            attack is null ? null : Ms(attack.ImpactAt),
            Ms(fighter.AttackReadyAt),
            Ms(fighter.BlockReadyAt),
            Ms(fighter.ParryReadyAt),
            Ms(fighter.DodgeReadyAt),
            Ms(fighter.DuckReadyAt),
            Ms(fighter.HopReadyAt),
            fighter.BlockHeld,
            fighter.AttackBuffer.Select(intent =>
                new ReplayQueuedAttack(intent.Kind.ToString(), intent.Lane.ToString(), Ms(intent.QueuedAt))).ToArray());
    }

    private double? ConsumeQueuedAt(AttackState attack)
    {
        var key = QueueKey(attack.Attacker, attack.Info.Kind, attack.Info.Lane);
        if (!_queuedAttackTimes.TryGetValue(key, out var times) || times.Count == 0)
        {
            return null;
        }

        return Ms(times.Dequeue());
    }

    private double? FirstObservedAt(CombatFighter defender, double launchAt, AttackKind kind) =>
        _firstObservedAt.TryGetValue(ThreatKey(defender, launchAt, kind), out var time) ? time : null;

    private int[] FindDefenseAttempts(AttackState attack, double impactAt) =>
        Replay.Events
            .Where(item =>
                (item.Type is "defense.input" or "defense.input_rejected") &&
                item.FighterId == Id(attack.Defender) &&
                item.TargetFighterId == Id(attack.Attacker) &&
                item.ScheduledImpactAtMs is { } scheduledImpact &&
                Math.Abs(scheduledImpact - Ms(impactAt)) <= 1e-6 &&
                (item.DefenseKind != nameof(DefenseKind.Block) ||
                    item.Message == "Block hold requested."))
            .Select(item => item.DefenseAttemptId ?? item.Sequence)
            .ToArray();

    private static Dictionary<string, object?> ReadConfiguration(CombatTuning tuning)
    {
        var values = new Dictionary<string, object?>(StringComparer.Ordinal);
        foreach (var property in typeof(CombatTuning).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (property.CanRead)
            {
                values[property.Name] = property.GetValue(tuning);
            }
        }

        return values;
    }

    private static CombatFighter OpponentOf(CombatEngine engine, CombatFighter fighter) =>
        ReferenceEquals(fighter, engine.Player) ? engine.Opponent : engine.Player;

    private static string ThreatKey(CombatFighter defender, double launchAt, AttackKind kind) =>
        $"{Id(defender)}:{launchAt:R}:{kind}";

    private static string QueueKey(CombatFighter fighter, AttackKind kind, AttackLane lane) =>
        $"{Id(fighter)}:{kind}:{lane}";

    private static string Id(CombatFighter fighter) => fighter.IsPlayer ? "Player" : "Opponent";
    private static double Ms(double seconds) => seconds * 1000.0;
    private static string FormatMs(double? seconds) => seconds is { } value ? Ms(value).ToString("0.###") : "unknown";
}
