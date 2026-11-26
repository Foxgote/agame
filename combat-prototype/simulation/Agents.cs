using CombatPrototype.Core;

namespace CombatPrototype.Simulation;

public sealed class DeterministicRandom
{
    private ulong _state;
    private bool _hasGaussian;
    private double _gaussian;

    public DeterministicRandom(ulong seed)
    {
        _state = seed;
    }

    public ulong NextUInt64()
    {
        var value = (_state += 0x9E3779B97F4A7C15UL);
        value = (value ^ (value >> 30)) * 0xBF58476D1CE4E5B9UL;
        value = (value ^ (value >> 27)) * 0x94D049BB133111EBUL;
        return value ^ (value >> 31);
    }

    public double NextDouble() => (NextUInt64() >> 11) * (1.0 / 9007199254740992.0);

    public int NextInt(int exclusiveMaximum)
    {
        if (exclusiveMaximum <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(exclusiveMaximum));
        }

        return (int)(NextDouble() * exclusiveMaximum);
    }

    public bool Chance(double probability) =>
        NextDouble() < Math.Clamp(probability, 0.0, 1.0);

    public double NextGaussian()
    {
        if (_hasGaussian)
        {
            _hasGaussian = false;
            return _gaussian;
        }

        var first = Math.Max(1e-12, NextDouble());
        var second = NextDouble();
        var radius = Math.Sqrt(-2.0 * Math.Log(first));
        var angle = 2.0 * Math.PI * second;
        _gaussian = radius * Math.Sin(angle);
        _hasGaussian = true;
        return radius * Math.Cos(angle);
    }
}

public sealed record ReactionModel(
    double MeanReactionSeconds = 0.20,
    double ReactionStandardDeviation = 0.045,
    double TimingErrorStandardDeviation = 0.035,
    double PredictionChance = 0.08,
    double PredictionErrorChance = 0.12,
    double PredictedAttackInterval = 1.15);

public readonly record struct AgentThreatObservation(
    long Id,
    AttackKind Kind,
    AttackLane Lane,
    bool Blockable,
    bool Parryable,
    bool Dodgeable,
    bool Launched,
    double VisibleAt,
    double EstimatedImpactAt,
    double ImpactEstimateUncertaintySeconds);

public sealed class AgentTimingPerception
{
    private readonly DeterministicRandom _random;
    private readonly double _impactEstimateUncertaintySeconds;
    private ThreatKey? _currentThreat;
    private long _nextThreatId;
    private double _estimatedImpactAt;

    private readonly record struct ThreatKey(double LaunchAt, AttackKind Kind, AttackLane Lane);

    public AgentTimingPerception(DeterministicRandom random, double impactEstimateUncertaintyMs)
    {
        ArgumentNullException.ThrowIfNull(random);
        if (!double.IsFinite(impactEstimateUncertaintyMs) || impactEstimateUncertaintyMs < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(impactEstimateUncertaintyMs));
        }

        _random = random;
        _impactEstimateUncertaintySeconds = impactEstimateUncertaintyMs / 1000.0;
    }

    public AgentThreatObservation? Observe(AttackPreview? incoming)
    {
        if (incoming is not { } threat)
        {
            _currentThreat = null;
            return null;
        }

        var key = new ThreatKey(threat.LaunchAt, threat.Info.Kind, threat.Info.Lane);
        if (_currentThreat != key)
        {
            _currentThreat = key;
            _nextThreatId++;
            var estimateError = _impactEstimateUncertaintySeconds == 0.0
                ? 0.0
                : _random.NextGaussian() * _impactEstimateUncertaintySeconds;
            _estimatedImpactAt = threat.ImpactAt + estimateError;
        }

        return new AgentThreatObservation(
            _nextThreatId,
            threat.Info.Kind,
            threat.Info.Lane,
            threat.Info.Blockable,
            threat.Info.Parryable,
            threat.Info.Dodgeable,
            threat.Launched,
            threat.VisibleAt,
            _estimatedImpactAt,
            _impactEstimateUncertaintySeconds);
    }
}

public readonly record struct AgentObservation(
    double Time,
    bool IsPlayer,
    double Health,
    double Stamina,
    int AttackQueueCount,
    bool AttackActive,
    AttackKind? ActiveAttackKind,
    double AttackReadyIn,
    bool BlockHeld,
    double BlockReadyIn,
    double ParryReadyIn,
    double DodgeReadyIn,
    double DuckReadyIn,
    double HopReadyIn,
    string LastDefenseResponse,
    double OpponentHealth,
    double OpponentStamina,
    int OpponentAttackQueueCount,
    bool OpponentAttackActive,
    AttackKind? OpponentActiveAttackKind,
    bool OpponentBlockHeld,
    string OpponentLastDefenseResponse,
    AgentThreatObservation? Incoming,
    double Initiative,
    bool UseExpandedSpecials,
    double LightSpecialCost,
    double HeavySpecialCost);

public readonly record struct AgentDecision(
    AttackKind? Attack = null,
    AttackLane AttackLane = AttackLane.Mid,
    DefenseKind? Defense = null,
    bool? HoldBlock = null,
    bool IsPrediction = false,
    bool IsIncorrectChoice = false);

public interface ICombatAgent
{
    string Name { get; }
    ReactionModel Reaction { get; }
    AgentDecision Decide(in AgentObservation observation, DeterministicRandom random);
}

public static class AgentObservationFactory
{
    public static AgentObservation Create(
        CombatEngine engine,
        CombatFighter fighter,
        AgentTimingPerception timingPerception)
    {
        ArgumentNullException.ThrowIfNull(timingPerception);
        var opponent = ReferenceEquals(fighter, engine.Player) ? engine.Opponent : engine.Player;
        var incoming = timingPerception.Observe(engine.GetIncomingThreat(fighter));
        return new AgentObservation(
            engine.Time,
            fighter.IsPlayer,
            fighter.Health,
            fighter.AvailableStamina,
            fighter.BufferedAttackCount,
            fighter.CurrentAttack is not null,
            fighter.CurrentAttack?.Info.Kind,
            Math.Max(0.0, fighter.AttackReadyAt - engine.Time),
            fighter.BlockHeld,
            Math.Max(0.0, fighter.BlockReadyAt - engine.Time),
            Math.Max(0.0, fighter.ParryReadyAt - engine.Time),
            Math.Max(0.0, fighter.DodgeReadyAt - engine.Time),
            Math.Max(0.0, fighter.DuckReadyAt - engine.Time),
            Math.Max(0.0, fighter.HopReadyAt - engine.Time),
            fighter.LastDefenseResponse,
            opponent.Health,
            opponent.AvailableStamina,
            opponent.BufferedAttackCount,
            incoming is { Launched: true },
            incoming?.Kind,
            opponent.BlockHeld,
            opponent.LastDefenseResponse,
            incoming,
            engine.Initiative,
            engine.Tuning.UseExpandedSpecials,
            engine.Tuning.LightSpecialCost,
            engine.Tuning.SpecialCost);
    }
}

public sealed record BalancedAgentParameters(
    double AttackInterval = 0.28,
    double HeavyProbability = 0.28,
    double SpecialProbability = 0.08,
    double DefensiveAggression = 0.72,
    double ParryProbability = 0.35,
    double DodgeProbability = 0.25,
    double BlockBias = 0.40,
    double ReactionDelay = 0.20,
    double TimingError = 0.035);

public static class AgentFactory
{
    public static readonly string[] StrategyNames =
    [
        "AggressiveLightSpam",
        "HeavyBaiting",
        "PassiveBlocking",
        "ReactionDefense",
        "ParryFishing",
        "DodgeHeavy",
        "Random",
        "Balanced"
    ];

    public static ICombatAgent Create(
        string strategy,
        ReactionModel? reaction = null,
        BalancedAgentParameters? parameters = null,
        BalancedAgentParameters? previousParameters = null,
        double parryReadChance = CombatTuning.DefaultAiParryReadChance) =>
        strategy switch
        {
            "AggressiveLightSpam" => new AggressiveLightSpamAgent(reaction),
            "HeavyBaiting" => new HeavyBaitingAgent(reaction),
            "PassiveBlocking" => new PassiveBlockingAgent(reaction),
            "ReactionDefense" => new ReactionDefenseAgent(reaction),
            "ParryFishing" => new ParryFishingAgent(reaction, parryReadChance),
            "DodgeHeavy" => new DodgeHeavyAgent(reaction),
            "Random" => new RandomPlayAgent(reaction),
            "Balanced" => new BalancedAgent(reaction, parameters),
            "CounterFirst" => new BalancedAgent(reaction, parameters, "CounterFirst", waitForFirstExchange: true),
            "PreviousBalanced" => new BalancedAgent(reaction, previousParameters, "PreviousBalanced"),
            _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, "Unknown combat strategy.")
        };
}

public abstract class CombatAgentBase : ICombatAgent
{
    private double _nextAttackAt;

    protected CombatAgentBase(string name, ReactionModel? reaction)
    {
        Name = name;
        Reaction = reaction ?? new ReactionModel();
    }

    public string Name { get; }
    public ReactionModel Reaction { get; }
    public abstract AgentDecision Decide(in AgentObservation observation, DeterministicRandom random);

    protected AttackKind? QueueAttack(
        in AgentObservation observation,
        DeterministicRandom random,
        double interval,
        AttackKind preferred,
        double heavyProbability = 0.0,
        double specialProbability = 0.0)
    {
        if (observation.AttackQueueCount >= CombatEngine.MaxBufferLength ||
            observation.Time + 1e-9 < _nextAttackAt)
        {
            return null;
        }

        _nextAttackAt = observation.Time + Math.Max(0.04, interval);
        if (observation.Stamina < observation.HeavySpecialCost &&
            preferred is AttackKind.Unblockable or AttackKind.Undodgeable)
        {
            preferred = AttackKind.Light;
        }

        if (random.Chance(specialProbability))
        {
            if (observation.UseExpandedSpecials)
            {
                var canUseQuickSpecial = observation.Stamina >= observation.LightSpecialCost;
                var canUseHeavySpecial = observation.Stamina >= observation.HeavySpecialCost;
                if (canUseQuickSpecial || canUseHeavySpecial)
                {
                    var useHeavySpecial = canUseHeavySpecial &&
                        (!canUseQuickSpecial || random.Chance(0.5));
                    var unblockable = observation.OpponentBlockHeld || random.Chance(0.5);
                    return (useHeavySpecial, unblockable) switch
                    {
                        (true, true) => AttackKind.Unblockable,
                        (true, false) => AttackKind.Undodgeable,
                        (false, true) => AttackKind.LightUnblockable,
                        _ => AttackKind.LightUndodgeable
                    };
                }
            }
            else if (observation.Stamina >= observation.HeavySpecialCost)
            {
                return random.Chance(0.5) ? AttackKind.Unblockable : AttackKind.Undodgeable;
            }
        }

        if (random.Chance(heavyProbability))
        {
            return AttackKind.Heavy;
        }

        return preferred;
    }

    protected static AttackLane RandomLane(DeterministicRandom random) =>
        (AttackLane)random.NextInt(3);

    protected static AgentDecision NoAction => new();
}

public sealed class AggressiveLightSpamAgent(ReactionModel? reaction = null)
    : CombatAgentBase("AggressiveLightSpam", reaction)
{
    public override AgentDecision Decide(in AgentObservation observation, DeterministicRandom random)
    {
        var attack = QueueAttack(observation, random, 0.12, AttackKind.Light);
        return attack is null ? NoAction : new AgentDecision(attack, AttackLane.Mid);
    }
}

public sealed class HeavyBaitingAgent(ReactionModel? reaction = null)
    : CombatAgentBase("HeavyBaiting", reaction)
{
    private int _sequence;

    public override AgentDecision Decide(in AgentObservation observation, DeterministicRandom random)
    {
        var opponentParried = observation.OpponentLastDefenseResponse.Contains("Parried", StringComparison.OrdinalIgnoreCase);
        var preferred = opponentParried || _sequence % 3 == 1 ? AttackKind.Heavy : AttackKind.Light;
        var attack = QueueAttack(observation, random, 0.42, preferred, heavyProbability: opponentParried ? 0.8 : 0.15);
        if (attack is not null)
        {
            _sequence++;
        }

        return attack is null ? NoAction : new AgentDecision(attack, RandomLane(random));
    }
}

public sealed class PassiveBlockingAgent(ReactionModel? reaction = null)
    : CombatAgentBase("PassiveBlocking", reaction)
{
    public override AgentDecision Decide(in AgentObservation observation, DeterministicRandom random) =>
        new(HoldBlock: true);
}

public sealed class ReactionDefenseAgent(ReactionModel? reaction = null)
    : ReactiveAgentBase("ReactionDefense", reaction)
{
    protected override DefenseKind ChooseDefense(AgentThreatObservation incoming, DeterministicRandom random) =>
        incoming.Kind switch
        {
            AttackKind.LightUnblockable or AttackKind.Unblockable => DefenseKind.Dodge,
            AttackKind.LightUndodgeable or AttackKind.Undodgeable => DefenseKind.Parry,
            _ => DefenseKind.Block
        };

    protected override double AttackInterval => 0.34;
    protected override AttackKind PreferredAttack => AttackKind.Light;
}

public sealed class ParryFishingAgent(ReactionModel? reaction = null,
    double parryReadChance = CombatTuning.DefaultAiParryReadChance)
    : ReactiveAgentBase("ParryFishing", reaction)
{
    protected override bool ShouldDefend(AgentObservation observation, DeterministicRandom random) =>
        observation.Incoming is not { Parryable: true } || parryReadChance >= 1.0 || random.Chance(parryReadChance);

    protected override DefenseKind ChooseDefense(AgentThreatObservation incoming, DeterministicRandom random) =>
        incoming.Kind is AttackKind.LightUnblockable or AttackKind.Unblockable
            ? DefenseKind.Dodge
            : DefenseKind.Parry;

    protected override double AttackInterval => 0.34;
    protected override AttackKind PreferredAttack => AttackKind.Light;
}

public sealed class DodgeHeavyAgent(ReactionModel? reaction = null)
    : ReactiveAgentBase("DodgeHeavy", reaction)
{
    protected override DefenseKind ChooseDefense(AgentThreatObservation incoming, DeterministicRandom random)
    {
        if (incoming.Dodgeable && random.Chance(0.78))
        {
            return DefenseKind.Dodge;
        }

        return incoming.Blockable ? DefenseKind.Block : DefenseKind.Dodge;
    }

    protected override double AttackInterval => 0.52;
    protected override AttackKind PreferredAttack => AttackKind.Heavy;
    protected override double GetSpecialChance(in AgentObservation observation) =>
        observation.UseExpandedSpecials && observation.OpponentBlockHeld ? 0.35 : 0.0;
}

public sealed class RandomPlayAgent(ReactionModel? reaction = null)
    : ReactiveAgentBase("Random", reaction)
{
    protected override DefenseKind ChooseDefense(AgentThreatObservation incoming, DeterministicRandom random) =>
        (DefenseKind)random.NextInt(5);

    protected override double AttackInterval => 0.23 + randomInterval;
    protected override AttackKind PreferredAttack => AttackKind.Light;

    private double randomInterval;

    public override AgentDecision Decide(in AgentObservation observation, DeterministicRandom random)
    {
        randomInterval = random.NextDouble() * 0.35;
        return base.Decide(observation, random);
    }

    protected override double HeavyChance => 0.40;
    protected override double SpecialChance => 0.18;
}

public sealed class BalancedAgent : ReactiveAgentBase
{
    private readonly BalancedAgentParameters _parameters;
    private bool _waitingForExchange;
    private long? _openingThreat;

    public BalancedAgent(
        ReactionModel? reaction = null,
        BalancedAgentParameters? parameters = null,
        string name = "Balanced", bool waitForFirstExchange = false)
        : base(name, ApplyReaction(reaction, parameters))
    {
        _parameters = parameters ?? new BalancedAgentParameters();
        _waitingForExchange = waitForFirstExchange;
    }

    protected override bool CanQueueAttack(in AgentObservation observation)
    {
        if (!_waitingForExchange) return true;
        if (_openingThreat is null && observation.Incoming is { } incoming)
            _openingThreat = incoming.Id;
        if (_openingThreat is not null && _openingThreat != observation.Incoming?.Id)
            _waitingForExchange = false;
        return !_waitingForExchange;
    }

    protected override double AttackInterval => _parameters.AttackInterval;
    protected override AttackKind PreferredAttack => AttackKind.Light;
    protected override double HeavyChance => _parameters.HeavyProbability;
    protected override double SpecialChance => _parameters.SpecialProbability;

    private static ReactionModel? ApplyReaction(
        ReactionModel? reaction,
        BalancedAgentParameters? parameters) =>
        parameters is null
            ? reaction
            : (reaction ?? new ReactionModel()) with
            {
                MeanReactionSeconds = parameters.ReactionDelay,
                TimingErrorStandardDeviation = parameters.TimingError
            };

    protected override DefenseKind ChooseDefense(AgentThreatObservation incoming, DeterministicRandom random)
    {
        if (incoming.Kind is AttackKind.LightUnblockable or AttackKind.Unblockable)
        {
            return DefenseKind.Dodge;
        }

        if (incoming.Kind is AttackKind.LightUndodgeable or AttackKind.Undodgeable)
        {
            return random.Chance(_parameters.ParryProbability) ? DefenseKind.Parry : DefenseKind.Block;
        }

        var choice = random.NextDouble();
        if (choice < _parameters.ParryProbability)
        {
            return DefenseKind.Parry;
        }

        if (choice < _parameters.ParryProbability + _parameters.DodgeProbability && incoming.Dodgeable)
        {
            return DefenseKind.Dodge;
        }

        return random.Chance(_parameters.BlockBias) ? DefenseKind.Block : DefenseKind.Parry;
    }

    protected override bool ShouldDefend(AgentObservation observation, DeterministicRandom random) =>
        random.Chance(_parameters.DefensiveAggression);
}

public abstract class ReactiveAgentBase : CombatAgentBase, IAgentDecisionTraceProvider
{
    private long? _plannedThreatId;
    private double _plannedDefenseAt;
    private DefenseKind? _plannedDefense;
    private bool _responseIssued;
    private bool _incorrectChoice;
    private double _predictionAt = double.PositiveInfinity;
    private DefenseKind _predictionDefense = DefenseKind.Parry;
    private bool _predictionIssued;
    private bool _predictionConsidered;

    protected ReactiveAgentBase(string name, ReactionModel? reaction) : base(name, reaction) { }

    public AgentDefenseTrace? LastDefenseTrace { get; private set; }

    protected abstract DefenseKind ChooseDefense(AgentThreatObservation incoming, DeterministicRandom random);
    protected abstract double AttackInterval { get; }
    protected abstract AttackKind PreferredAttack { get; }
    protected virtual double HeavyChance => 0.0;
    protected virtual double SpecialChance => 0.0;
    protected virtual double GetSpecialChance(in AgentObservation observation) => SpecialChance;
    protected virtual bool ShouldDefend(AgentObservation observation, DeterministicRandom random) => true;
    protected virtual bool CanQueueAttack(in AgentObservation observation) => true;

    public override AgentDecision Decide(in AgentObservation observation, DeterministicRandom random)
    {
        LastDefenseTrace = null;
        var attack = CanQueueAttack(observation) ? QueueAttack(
            observation, random, AttackInterval, PreferredAttack, HeavyChance, GetSpecialChance(observation)) : null;
        var lane = attack is AttackKind.Light or AttackKind.Heavy
            ? RandomLane(random)
            : RandomLane(random);

        if (observation.Incoming is { } incoming)
        {
            if (_plannedThreatId != incoming.Id)
            {
                _plannedThreatId = incoming.Id;
                _plannedDefense = ShouldDefend(observation, random)
                    ? ChooseDefense(incoming, random)
                    : null;
                _incorrectChoice = false;
                if (_plannedDefense is { } selected && random.Chance(Reaction.PredictionErrorChance))
                {
                    _plannedDefense = SelectIncorrectDefense(selected, incoming, random);
                    _incorrectChoice = true;
                }

                var reactionDelay = Math.Max(0.0, Reaction.MeanReactionSeconds +
                    random.NextGaussian() * Math.Max(0.0, Reaction.ReactionStandardDeviation));
                var timingError = random.NextGaussian() * Math.Max(0.0, Reaction.TimingErrorStandardDeviation);
                var earliest = incoming.VisibleAt + reactionDelay;
                _plannedDefenseAt = _plannedDefense == DefenseKind.Block
                    ? earliest
                    : Math.Max(earliest, incoming.EstimatedImpactAt + timingError);
                if (_plannedDefense is { } plannedDefense)
                {
                    LastDefenseTrace = new AgentDefenseTrace(
                        plannedDefense,
                        incoming.Id,
                        incoming.Kind,
                        incoming.Lane,
                        incoming.EstimatedImpactAt,
                        incoming.ImpactEstimateUncertaintySeconds,
                        incoming.VisibleAt,
                        observation.Time,
                        _plannedDefenseAt,
                        false,
                        _incorrectChoice,
                        reactionDelay,
                        timingError);
                }

                _responseIssued = false;
                _predictionAt = double.PositiveInfinity;
                _predictionIssued = false;
                _predictionConsidered = false;
            }

            if (!_responseIssued && _plannedDefense is { } defense && observation.Time + 1e-9 >= _plannedDefenseAt)
            {
                _responseIssued = true;
                return ComposeDecision(attack, lane, defense, _incorrectChoice, false);
            }
        }
        else
        {
            _plannedThreatId = null;
            _plannedDefense = null;
            if (!_predictionConsidered)
            {
                _predictionConsidered = true;
                if (random.Chance(Reaction.PredictionChance))
                {
                    var predictionError = random.NextGaussian() * Reaction.TimingErrorStandardDeviation;
                    _predictionAt = observation.Time + Math.Max(0.25, Reaction.PredictedAttackInterval +
                        predictionError);
                    _predictionDefense = (DefenseKind)random.NextInt(4);
                    LastDefenseTrace = new AgentDefenseTrace(
                        _predictionDefense,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        observation.Time,
                        _predictionAt,
                        true,
                        false,
                        TimingErrorSeconds: predictionError);
                }
            }

            if (!_predictionIssued && observation.Time + 1e-9 >= _predictionAt)
            {
                _predictionIssued = true;
                _predictionAt = double.PositiveInfinity;
                return ComposeDecision(attack, lane, _predictionDefense, false, true);
            }
        }

        if (attack is not null)
        {
            return new AgentDecision(attack, lane, HoldBlock: observation.Incoming is null ? false : null);
        }

        return observation.Incoming is null
            ? new AgentDecision(HoldBlock: false)
            : NoAction;
    }

    private AgentDecision ComposeDecision(
        AttackKind? attack,
        AttackLane lane,
        DefenseKind defense,
        bool incorrectChoice,
        bool prediction)
    {
        return defense == DefenseKind.Block
            ? new AgentDecision(attack, lane, HoldBlock: true, IsPrediction: prediction,
                IsIncorrectChoice: incorrectChoice)
            : new AgentDecision(attack, lane, defense, HoldBlock: false, IsPrediction: prediction,
                IsIncorrectChoice: incorrectChoice);
    }

    private static DefenseKind SelectIncorrectDefense(
        DefenseKind selected,
        AgentThreatObservation incoming,
        DeterministicRandom random)
    {
        for (var attempt = 0; attempt < 8; attempt++)
        {
            var candidate = (DefenseKind)random.NextInt(5);
            if (candidate == selected)
            {
                continue;
            }

            if (candidate == DefenseKind.Block && !incoming.Blockable ||
                candidate == DefenseKind.Dodge && !incoming.Dodgeable ||
                candidate == DefenseKind.Parry && !incoming.Parryable ||
                candidate == DefenseKind.Duck &&
                    (incoming.Lane != AttackLane.High || !incoming.Dodgeable) ||
                candidate == DefenseKind.Hop &&
                    (incoming.Lane != AttackLane.Low || !incoming.Dodgeable))
            {
                return candidate;
            }
        }

        return selected == DefenseKind.Parry ? DefenseKind.Dodge : DefenseKind.Parry;
    }
}
