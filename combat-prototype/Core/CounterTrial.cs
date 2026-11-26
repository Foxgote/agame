using System;

namespace CombatPrototype.Core;

public readonly record struct StrikeSample(double AttackerPosition, double DefenderPosition, bool InReach, bool Guarded);
public readonly record struct CounterOpponentSettings(
    double Cadence, double CadenceSpread, double ReactionDelay, double ReadChance, double TimingSpread);
public readonly record struct CounterEconomySettings(
    double StartingStamina, double LightCost, double HeavyCost, double Regeneration, double DefenseReward);
public readonly record struct CounterMixupSettings(
    double ComboChance, double ComboGap, double CounterUseChance, double ParryBias);
public readonly record struct CounterSpecialTiming(double Windup, double ContactRecovery, double AvoidedRecovery);

public sealed class CounterTrial
{
    public const double CounterSeconds = 0.30;
    public const double CounterBufferSeconds = 0.14;
    public const double LightReach = 0.16;
    public const double HeavyReach = 0.24;
    public const double BodyGap = 0.07;
    public const double GuardLightCost = 8;
    public const double GuardHeavyCost = 15;
    public const double MatchedWindup = 0.280;
    private readonly (AttackKind Kind, double Expires)?[] _counter = new (AttackKind, double)?[2];
    private readonly (AttackState Threat, double InputTime, int QueueIndex)?[] _pendingCounters =
        new (AttackState, double, int)?[2];
    private readonly AttackState?[] _lastStrikes = new AttackState?[2];
    private readonly double?[] _guardStarts = new double?[2];
    private readonly Random _attackRhythm;
    private readonly Random _defenseRead;
    private double _lastReadImpact = double.NegativeInfinity;
    private double _lastCounterExpiry = double.NegativeInfinity;
    private bool _useCounter;
    private double _movement;
    private double _opponentMovement;

    public CounterTrial(int rhythmSeed = 731, int defenseSeed = 193)
    {
        _attackRhythm = new Random(rhythmSeed);
        _defenseRead = new Random(defenseSeed);
    }
    public double PlayerPosition { get; private set; } = 0.40;
    public double OpponentPosition { get; private set; } = 0.54;
    public double Gap => OpponentPosition - PlayerPosition;
    public CounterOpponentSettings? OpponentSettings { get; set; }
    public CounterEconomySettings? Economy { get; set; }
    public double? PlayerLightStartup { get; set; }
    public double? OpponentLightStartup { get; set; }
    public double? SharedAttackWindup { get; set; }
    public CounterMixupSettings? Mixups { get; set; }
    public bool InterruptHeavyStartup { get; set; } = true;
    public bool BurstLights { get; set; }
    public double? LightLaunchInterval { get; set; }
    public double? SpecialLaunchInterval { get; set; }
    public int? QueueLimit { get; set; }
    public double? SpecialCounterRecovery { get; set; }
    public CounterSpecialTiming? SpecialTiming { get; set; }
    public bool DisableSpecialRecovery { get; set; }
    public double OpponentLookahead { get; set; }
    public bool ClearQueueOnSpecialDefense { get; set; }

    public static double ReachFor(AttackKind kind) => kind == AttackKind.Light ? LightReach : HeavyReach;
    public bool InReach(AttackKind kind) => Gap <= ReachFor(kind) + 1e-9;
    public AttackState? LastStrike(bool player) => _lastStrikes[Index(player)];
    public void SetMovement(int direction) => _movement = Math.Clamp(direction, -1, 1);
    public bool HumanOpponent { get; set; }
    public void SetOpponentMovement(int direction) => _opponentMovement = Math.Clamp(direction, -1, 1);
    public bool IsParryHeld(bool player) => _guardStarts[Index(player)].HasValue;
    public bool IsGuarding(bool player, double time) =>
        _guardStarts[Index(player)] is { } starts && time + 1e-9 >= starts;
    public bool HasBufferedCounterInput(bool player) => _pendingCounters[Index(player)].HasValue;

    internal bool DiscardBufferedFollowUp(bool player)
    {
        var index = Index(player);
        if (_pendingCounters[index] is null) return false;
        _pendingCounters[index] = null;
        return true;
    }

    internal void SetParryHeld(bool player, double? guardStarts) => _guardStarts[Index(player)] = guardStarts;

    internal void BufferCounterInput(bool player, AttackState threat, double inputTime, int queueIndex) =>
        _pendingCounters[Index(player)] = (threat, inputTime, queueIndex);

    internal (AttackState Threat, double InputTime, int QueueIndex)? TakeResolvedCounterInput(bool player)
    {
        var index = Index(player);
        if (_pendingCounters[index] is not { Threat.Resolved: true } input) return null;
        _pendingCounters[index] = null;
        return input;
    }

    internal double NextAttackDelay(double cadence)
    {
        if (Mixups is { } mix && _attackRhythm.NextDouble() < mix.ComboChance)
            return Math.Max(0.09, mix.ComboGap * (0.86 + 0.28 * _attackRhythm.NextDouble()));
        return Math.Max(0.15, (OpponentSettings?.Cadence ?? cadence) *
            (0.95 + (OpponentSettings?.CadenceSpread ?? 0.33) * (2 * _attackRhythm.NextDouble() - 1)));
    }

    public static double FollowUpPressure(double impactGap) => Math.Clamp(1.0 - impactGap / 0.18, 0.0, 1.0);

    internal AttackKind? ChooseOpponentCounter(double now)
    {
        if (Counter(false, now) is not { } kind) return null;
        if (Mixups is not { } mix) return kind;
        var expiry = _counter[1]!.Value.Expires;
        if (expiry != _lastCounterExpiry)
        {
            _lastCounterExpiry = expiry;
            _useCounter = _attackRhythm.NextDouble() < mix.CounterUseChance;
        }
        return _useCounter ? kind : null;
    }

    internal (DefenseKind? Kind, double At) ReadDefense(AttackState incoming, bool busy, double reactionDelay)
    {
        var readChance = OpponentSettings?.ReadChance ?? 0.90;
        var timingSpread = OpponentSettings?.TimingSpread ?? 0.0825;
        reactionDelay = OpponentSettings?.ReactionDelay ?? reactionDelay;
        var pressure = 0.0;
        if (Mixups is not null)
        {
            pressure = FollowUpPressure(incoming.ImpactAt - _lastReadImpact);
            _lastReadImpact = incoming.ImpactAt;
        }
        if (_defenseRead.NextDouble() >= Math.Clamp(readChance - (busy ? 0.12 : 0) - 0.26 * pressure, 0, 1))
            return (null, 0);

        var recognizesAttack = _defenseRead.NextDouble() < 0.82 - 0.18 * pressure;
        var defense = recognizesAttack && incoming.Info.Parryable != incoming.Info.Dodgeable
            ? incoming.Info.Parryable ? DefenseKind.Parry : DefenseKind.Dodge
            : _defenseRead.NextDouble() < (Mixups?.ParryBias ?? 0.5) ? DefenseKind.Parry : DefenseKind.Dodge;
        var earliest = incoming.LaunchAt + Math.Max(0.12,
            reactionDelay + (_defenseRead.NextDouble() - 0.5) * 0.09 + (busy ? 0.035 : 0) + 0.035 * pressure);
        var estimate = incoming.LaunchAt + incoming.Info.Startup - 0.0075 +
            (_defenseRead.NextDouble() * 2 - 1) * timingSpread;
        return (defense, Math.Max(earliest, estimate));
    }

    internal void SampleStrike(AttackState attack)
    {
        var player = attack.Attacker.IsPlayer;
        attack.StrikeAtImpact = new StrikeSample(
            player ? PlayerPosition : OpponentPosition,
            player ? OpponentPosition : PlayerPosition,
            InReach(attack.Info.Kind),
            IsGuarding(attack.Defender.IsPlayer, attack.ImpactAt));
        _lastStrikes[Index(player)] = attack;
    }

    internal void Advance(double delta, bool opponentApproaches)
    {
        const double playerSpeed = 0.40;
        const double approachSpeed = 0.28;
        const double approachGap = 0.14;
        if (HumanOpponent)
        {
            var left = Math.Clamp(PlayerPosition + _movement * playerSpeed * delta, 0.06, 0.94 - BodyGap);
            var right = Math.Clamp(OpponentPosition + _opponentMovement * playerSpeed * delta, 0.06 + BodyGap, 0.94);
            if (right - left < BodyGap)
            {
                // Spend only the space available on inward movement; neither fighter pushes the other.
                var leftIn = Math.Max(0, left - PlayerPosition);
                var rightIn = Math.Max(0, OpponentPosition - right);
                var overlap = BodyGap - (right - left);
                if (leftIn + rightIn > 0)
                {
                    left -= overlap * leftIn / (leftIn + rightIn);
                    right += overlap * rightIn / (leftIn + rightIn);
                }
            }
            PlayerPosition = left;
            OpponentPosition = right;
            return;
        }
        // Positions are normalized to the arena. Neither fighter can push through a stationary body.
        if (opponentApproaches && Gap > approachGap)
            OpponentPosition = Math.Max(PlayerPosition + approachGap, OpponentPosition - approachSpeed * delta);
        PlayerPosition = Math.Clamp(PlayerPosition + _movement * playerSpeed * delta, 0.06, OpponentPosition - BodyGap);
    }

    public AttackKind? Counter(bool player, double now) =>
        _counter[Index(player)] is { } token && token.Expires > now + 1e-9 ? token.Kind : null;

    public double CounterRemaining(bool player, double now) =>
        Math.Max(0, (_counter[Index(player)]?.Expires ?? now) - now);

    internal void Resolve(bool player, AttackOutcome outcome, double now)
    {
        if (outcome is AttackOutcome.Parried or AttackOutcome.Dodged)
            _counter[Index(player)] = (outcome == AttackOutcome.Parried ? AttackKind.Unparryable : AttackKind.Undodgeable,
                now + CounterSeconds);
    }

    internal void ConsumeCounter(bool player) => _counter[Index(player)] = null;
    private static int Index(bool player) => player ? 0 : 1;
}
