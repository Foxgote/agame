using CombatPrototype.Core;

static void Check(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}

static void Near(double actual, double expected, string message, double tolerance = 1e-8)
{
    if (Math.Abs(actual - expected) > tolerance)
    {
        throw new InvalidOperationException($"{message}: expected {expected:0.000000}, got {actual:0.000000}");
    }
}

static void AdvanceTo(CombatEngine engine, double time)
{
    if (time > engine.Time)
    {
        engine.Tick(time - engine.Time);
    }
}

static CombatEngine NewEngine(CombatTuning? tuning = null)
{
    tuning ??= new CombatTuning { AiAttackEnabled = false, AiDefenseEnabled = false };
    tuning.AttackInputInterval = 0.0;
    tuning.StaminaRegenerationPerSecond = 0.0;
    return new CombatEngine(tuning);
}

var defaultEngine = new CombatEngine(new CombatTuning
{
    AiAttackEnabled = false,
    AiDefenseEnabled = false
});
Near(defaultEngine.Player.Stamina, 50.0, "player starts at the configured 50 stamina");
Near(defaultEngine.Opponent.Stamina, 50.0, "opponent starts at the configured 50 stamina");
Near(defaultEngine.Player.MaxStamina, 100.0, "stamina is capped at 100");
Near(defaultEngine.Tuning.LightCost, 10.0, "Light costs 10 stamina");
Near(defaultEngine.Tuning.HeavyCost, 20.0, "Heavy costs 20 stamina");
Near(defaultEngine.Tuning.StaminaRegenerationPerSecond, 15.0,
    "stamina regenerates continuously at 15 per second");
Near(defaultEngine.Tuning.AiParryReadChance, 0.75, "AI commits to three quarters of eligible parry reads");
Near(defaultEngine.Tuning.HeavyDefenderCleanHitGain, 0.0, "taking a Heavy does not fund a counter");
Near(defaultEngine.Tuning.LightAttackerCleanHitGain, 0.0,
    "normal attacks do not refund stamina to their attacker");
Near(defaultEngine.Tuning.LightStartup, 0.180, "light startup is 180 ms");
Near(defaultEngine.Tuning.HeavyStartup, 0.400, "heavy startup is 400 ms");
Near(defaultEngine.Tuning.HeavyDamage, 24.0, "baseline heavy damage remains 24");
Near(defaultEngine.Tuning.SpecialStartup, 0.700, "special startup is 700 ms");
Near(defaultEngine.Tuning.AttackInputInterval, 0.010, "attack input interval is 10 ms");
Near(defaultEngine.Tuning.LightDefenderParriedGain, 10.0, "Light parry reward is reduced to 10");
Near(defaultEngine.Tuning.HeavyDefenderParriedGain, 15.0, "Heavy parry reward is reduced to 15");
Near(defaultEngine.Tuning.SpecialCounterGain, 20.0, "Special counter reward exceeds Light parry reward");
Near(defaultEngine.Tuning.HitDefenderRecovery, 0.050, "clean hit recovery is 50 ms");
Near(defaultEngine.Tuning.ParriedAttackerRecovery, 0.120, "normal parry punish is 120 ms");
Near(defaultEngine.Tuning.SpecialCounterAttackerRecovery, 0.180,
    "Special counter recovery is 180 ms");
var slowerHeavyEngine = NewEngine(new CombatTuning
{
    AiAttackEnabled = false,
    AiDefenseEnabled = false,
    HeavyStartup = 0.850
});
Check(slowerHeavyEngine.QueueAttack(slowerHeavyEngine.Player, AttackKind.Heavy),
    "heavy remains queueable when its startup is tuned");
Near(slowerHeavyEngine.Player.CurrentAttack!.ImpactAt, 0.850,
    "experimental heavy startup tuning schedules the correct impact time");
var lighterHeavyEngine = NewEngine(new CombatTuning
{
    AiAttackEnabled = false,
    AiDefenseEnabled = false,
    HeavyDamage = 16.0
});
Near(lighterHeavyEngine.DescribeAttack(AttackKind.Heavy).Damage, 16.0,
    "experimental heavy damage tuning is applied without changing baseline damage");
var lightDamageHeavyEngine = NewEngine(new CombatTuning
{
    AiAttackEnabled = false,
    AiDefenseEnabled = false,
    HeavyDamage = 12.0
});
Near(lightDamageHeavyEngine.DescribeAttack(AttackKind.Heavy).Damage, 12.0,
    "experimental Heavy damage can be reduced to the Light baseline");
var staminaCostHeavyEngine = NewEngine(new CombatTuning
{
    AiAttackEnabled = false,
    AiDefenseEnabled = false,
    HeavyCost = 15.0
});
Check(staminaCostHeavyEngine.QueueAttack(staminaCostHeavyEngine.Player, AttackKind.Heavy),
    "normal Heavy remains usable when its stamina cost is tuned");
Near(staminaCostHeavyEngine.Player.Stamina, 35.0,
    "experimental Heavy cost is paid at launch");
AdvanceTo(staminaCostHeavyEngine, staminaCostHeavyEngine.Player.CurrentAttack!.ResolveAt);
Near(staminaCostHeavyEngine.Player.Stamina, 35.0,
    "a successful Heavy receives no automatic attacker refund");
var fasterLightEngine = NewEngine(new CombatTuning
{
    AiAttackEnabled = false,
    AiDefenseEnabled = false,
    LightStartup = 0.350
});
Check(fasterLightEngine.QueueAttack(fasterLightEngine.Player, AttackKind.Light),
    "normal Light remains queueable when its startup is tuned");
Near(fasterLightEngine.Player.CurrentAttack!.ImpactAt, 0.350,
    "experimental Light startup tuning schedules the correct impact time");
var baselineUnblockable = defaultEngine.DescribeAttack(AttackKind.Unblockable);
Near(baselineUnblockable.Startup, 0.700, "baseline heavy unblockable remains 700 ms");
Near(baselineUnblockable.StaminaCost, 30.0, "baseline heavy unblockable still costs 30");
Near(baselineUnblockable.Damage, 30.0, "baseline heavy unblockable damage is preserved");
var baselineUndodgeable = defaultEngine.DescribeAttack(AttackKind.Undodgeable);
Near(baselineUndodgeable.Startup, 0.700, "baseline heavy undodgeable remains 700 ms");
Near(baselineUndodgeable.StaminaCost, 30.0, "baseline heavy undodgeable still costs 30");
Near(baselineUndodgeable.Damage, 20.0, "baseline heavy undodgeable damage is preserved");
Near(defaultEngine.Tuning.ParryWindow, 0.120, "parry window is 120 ms");
Near(defaultEngine.Tuning.DodgeWindow, 0.180, "dodge window is 180 ms");
Near(defaultEngine.Tuning.FailedDefenseRecovery, 0.0,
    "baseline has no failed-defense lockout");
Check(!defaultEngine.Tuning.DisableDefenseDuringHeavyStartup,
    "baseline permits defense during Heavy startup");

var inputBufferEngine = new CombatEngine(new CombatTuning
{
    StartingStamina = 100.0,
    AiAttackEnabled = false,
    AiDefenseEnabled = false
});
Near(inputBufferEngine.Tuning.AttackInputInterval, 0.010, "attack buffer inputs are spaced by 10 ms");
Check(inputBufferEngine.QueueAttack(inputBufferEngine.Player, AttackKind.Light),
    "the first input enters the attack buffer");
Check(!inputBufferEngine.QueueAttack(inputBufferEngine.Player, AttackKind.Light,
        inputTimeSeconds: 0.009),
    "a repeated input inside the 10 ms gap is rejected");
for (var index = 1; index <= CombatEngine.MaxBufferLength; index++)
{
    Check(inputBufferEngine.QueueAttack(inputBufferEngine.Player, AttackKind.Light,
            inputTimeSeconds: index * 0.010),
        "an input is accepted after the 10 ms gap");
}

Check(inputBufferEngine.Player.AttackBuffer.Count == CombatEngine.MaxBufferLength,
    "the attack buffer holds eight queued inputs");
Check(!inputBufferEngine.QueueAttack(inputBufferEngine.Player, AttackKind.Light,
        inputTimeSeconds: 0.090),
    "the attack buffer rejects inputs above its eight-entry cap");

var failedDefenseCostEngine = NewEngine(new CombatTuning
{
    AiAttackEnabled = false,
    AiDefenseEnabled = false,
    ParryCost = 10.0,
    DodgeCost = 10.0
});
Check(failedDefenseCostEngine.ActivateDefense(failedDefenseCostEngine.Player, DefenseKind.Parry),
    "parry with a configured resource cost activates");
Near(failedDefenseCostEngine.Player.Stamina, 40.0,
    "parry cost is charged when the defense is activated");

foreach (var heavyKind in new[] { AttackKind.Heavy, AttackKind.Unblockable, AttackKind.Undodgeable })
{
    var heavyCommitEngine = NewEngine(new CombatTuning
    {
        AiAttackEnabled = false,
        AiDefenseEnabled = false,
        DisableDefenseDuringHeavyStartup = true
    });
    Check(heavyCommitEngine.QueueAttack(heavyCommitEngine.Player, heavyKind),
        $"{heavyKind} remains launchable in the commitment profile");
    Check(!heavyCommitEngine.ActivateDefense(heavyCommitEngine.Player, DefenseKind.Dodge),
        $"{heavyKind} startup disallows Dodge");
    Check(!heavyCommitEngine.ActivateDefense(heavyCommitEngine.Player, DefenseKind.Parry),
        $"{heavyKind} startup disallows Parry");
    Check(!heavyCommitEngine.ActivateDefense(heavyCommitEngine.Player, DefenseKind.Duck),
        $"{heavyKind} startup disallows Duck");
    Check(!heavyCommitEngine.ActivateDefense(heavyCommitEngine.Player, DefenseKind.Hop),
        $"{heavyKind} startup disallows Hop");
    Check(!heavyCommitEngine.SetBlockHeld(heavyCommitEngine.Player, true),
        $"{heavyKind} startup disallows starting Block");
}

var committedParryEngine = NewEngine(new CombatTuning
{
    AiAttackEnabled = false,
    AiDefenseEnabled = false,
    DisableDefenseDuringHeavyStartup = true
});
Check(committedParryEngine.QueueAttack(committedParryEngine.Opponent, AttackKind.Light),
    "opponent Light launches for the committed-defense test");
Check(committedParryEngine.QueueAttack(committedParryEngine.Player, AttackKind.Heavy),
    "Heavy can begin while an incoming Light is in startup");
AdvanceTo(committedParryEngine, 0.100);
Check(!committedParryEngine.ActivateDefense(committedParryEngine.Player, DefenseKind.Parry),
    "Parry cannot activate during the Heavy commitment");
AdvanceTo(committedParryEngine, 0.523);
var committedParryOutcome = committedParryEngine.Resolutions
    .Single(resolution => resolution.AttackKind == AttackKind.Light).Outcome;
Check(committedParryOutcome == AttackOutcome.CleanHit,
    $"the incoming Light lands when Parry is unavailable during Heavy startup ({committedParryOutcome})");

var committedBlockEngine = NewEngine(new CombatTuning
{
    AiAttackEnabled = false,
    AiDefenseEnabled = false,
    DisableDefenseDuringHeavyStartup = true
});
Check(committedBlockEngine.QueueAttack(committedBlockEngine.Opponent, AttackKind.Light),
    "opponent Light launches for the held-Block test");
Check(committedBlockEngine.QueueAttack(committedBlockEngine.Player, AttackKind.Heavy),
    "Heavy can begin while an incoming Light is in startup");
Check(!committedBlockEngine.SetBlockHeld(committedBlockEngine.Player, true),
    "Block cannot activate during the Heavy commitment");
AdvanceTo(committedBlockEngine, 0.523);
Check(committedBlockEngine.Resolutions
        .Single(resolution => resolution.AttackKind == AttackKind.Light).Outcome == AttackOutcome.CleanHit,
    "the incoming Light lands when Block is unavailable during Heavy startup");

var lightCommitEngine = NewEngine(new CombatTuning
{
    AiAttackEnabled = false,
    AiDefenseEnabled = false,
    DisableDefenseDuringHeavyStartup = true
});
Check(lightCommitEngine.QueueAttack(lightCommitEngine.Player, AttackKind.Light),
    "Light launches when Heavy commitment is enabled");
Check(lightCommitEngine.ActivateDefense(lightCommitEngine.Player, DefenseKind.Dodge),
    "Light startup still allows defensive actions");

var failedDefenseLockoutEngine = NewEngine(new CombatTuning
{
    AiAttackEnabled = false,
    AiDefenseEnabled = false,
    FailedDefenseRecovery = 0.250
});
Check(failedDefenseLockoutEngine.ActivateDefense(
        failedDefenseLockoutEngine.Player, DefenseKind.Dodge),
    "dodge for a failed-defense-lockout test activates");
var missedDodge = failedDefenseLockoutEngine.Player.DefenseActivations.Single();
failedDefenseLockoutEngine.Tick(0.100);
Check(missedDodge.Failed, "an unused dodge window is marked as a failed defense");
Near(failedDefenseLockoutEngine.Player.ParryReadyAt, 0.340,
    "failed dodge locks out parry until window end plus recovery");
Near(failedDefenseLockoutEngine.Player.DodgeReadyAt, 0.340,
    "failed dodge locks out a second dodge until window end plus recovery");
Check(failedDefenseLockoutEngine.Events.Any(item =>
        item.Kind == CombatEventKind.Defense && item.Message.Contains("missed", StringComparison.Ordinal)),
    "failed-defense recovery is visible in the event log");

var pendingParryLockoutEngine = NewEngine(new CombatTuning
{
    AiAttackEnabled = false,
    AiDefenseEnabled = false,
    FailedDefenseRecovery = 0.250
});
Check(pendingParryLockoutEngine.QueueAttack(
        pendingParryLockoutEngine.Opponent, AttackKind.Light),
    "opponent Light launches for the pending parry lockout test");
var pendingParryThreat = pendingParryLockoutEngine.Opponent.CurrentAttack!;
AdvanceTo(pendingParryLockoutEngine, pendingParryThreat.ImpactAt - 0.060);
Check(pendingParryLockoutEngine.ActivateDefense(
        pendingParryLockoutEngine.Player, DefenseKind.Parry),
    "early-boundary parry activates while missed-defense recovery is enabled");
var pendingParry = pendingParryLockoutEngine.Player.DefenseActivations.Single();
AdvanceTo(pendingParryLockoutEngine, pendingParryThreat.ImpactAt + 0.050);
Check(!pendingParry.Failed,
    "a valid parry remains pending until the attack completes its late-input grace period");
AdvanceTo(pendingParryLockoutEngine, pendingParryThreat.ResolveAt);
Check(pendingParry.Consumed && !pendingParry.Failed,
    "the pending parry counters the attack instead of being classified as a miss");
Check(pendingParryLockoutEngine.Resolutions.Single().Outcome == AttackOutcome.Parried,
    "failed-defense recovery preserves valid early parries");

var staminaQueueEngine = NewEngine(new CombatTuning
{
    AiAttackEnabled = false,
    AiDefenseEnabled = false,
    StartingStamina = 25.0,
    ParryCost = 6.0
});
var guarding = staminaQueueEngine.Player;
var confirmations = new List<(bool player, AttackKind kind, AttackLane lane)>();
staminaQueueEngine.AttackQueued += (owner, info) => confirmations.Add((owner.IsPlayer, info.Kind, info.Lane));
staminaQueueEngine.SetBlockHeld(guarding, true);
Check(staminaQueueEngine.QueueAttack(guarding, AttackKind.Heavy), "Heavy reserves during block");
Near(guarding.Stamina, 25.0, "reservation is not a spend");
Near(guarding.AvailableStamina, 5.0, "reserved stamina is unavailable");
Check(!staminaQueueEngine.QueueAttack(guarding, AttackKind.Light), "cannot overbook the queue");
Check(!staminaQueueEngine.ActivateDefense(guarding, DefenseKind.Parry), "paid defense cannot spend a reservation");
staminaQueueEngine.QueueAttack(staminaQueueEngine.Opponent, AttackKind.Light);
AdvanceTo(staminaQueueEngine, staminaQueueEngine.Opponent.CurrentAttack!.ResolveAt);
Near(guarding.AvailableStamina, 15.0, "a successful block gives 10 spendable stamina on top of the reservation");
Check(guarding.BufferedAttackCount == 1, "defense reward does not resurrect the rejected Light");
Check(staminaQueueEngine.QueueAttack(guarding, AttackKind.Light), "fresh input can use the block reward immediately");
staminaQueueEngine.Tuning.HeavyCost = 35.0;
staminaQueueEngine.SetBlockHeld(guarding, false);
Check(guarding.CurrentAttack?.Info.Kind == AttackKind.Heavy, "first reservation launches first");
Near(guarding.Stamina, 15.0, "launch spends its original price once");
Near(guarding.ReservedStamina, 10.0, "later reservations are preserved");
Check(confirmations.SequenceEqual(new[] { (true, AttackKind.Heavy, AttackLane.Mid),
    (false, AttackKind.Light, AttackLane.Mid), (true, AttackKind.Light, AttackLane.Mid) }),
    "purchase feedback fires once on acceptance for either fighter, never on rejection or later launch");

var regenerationEngine = new CombatEngine(new CombatTuning
{
    AiAttackEnabled = false,
    AiDefenseEnabled = false,
    StartingStamina = 0.0,
    LightCost = 10.0,
    StaminaRegenerationPerSecond = 10.0,
    AttackInputInterval = 0.0
});
regenerationEngine.SetBlockHeld(regenerationEngine.Opponent, true);
Check(!regenerationEngine.QueueAttack(regenerationEngine.Player, AttackKind.Light),
    "an exhausted attacker cannot buffer a Light");
regenerationEngine.Tick(0.99);
Check(!regenerationEngine.QueueAttack(regenerationEngine.Player, AttackKind.Light),
    "partial regeneration is not enough for a Light");
regenerationEngine.Tick(0.01);
Check(regenerationEngine.Player.CurrentAttack is null && regenerationEngine.Player.BufferedAttackCount == 0,
    "regeneration never launches a rejected input");
Check(regenerationEngine.QueueAttack(regenerationEngine.Player, AttackKind.Light),
    "regeneration makes a fresh Light input affordable");
Check(regenerationEngine.Player.CurrentAttack?.Info.Kind == AttackKind.Light,
    "the new input launches immediately");
Near(regenerationEngine.Player.Stamina, 0.0,
    "the launched attack pays only once");
Near(regenerationEngine.Opponent.Stamina, 10.0,
    "a defender regenerates on the same clock while holding block");

var disabledQuickSpecialEngine = NewEngine();
Check(!disabledQuickSpecialEngine.QueueAttack(
        disabledQuickSpecialEngine.Player, AttackKind.LightUnblockable),
    "quick specials are unavailable in the baseline timing profile");

var hybridTuning = new CombatTuning
{
    AiAttackEnabled = false,
    AiDefenseEnabled = false,
    UseExpandedSpecials = true,
    LightSpecialStartup = 0.500,
    LightSpecialCost = 20.0,
    LightUnblockableDamage = 16.0,
    LightUndodgeableDamage = 16.0,
    SpecialStartup = 0.800,
    SpecialCost = 35.0,
    UnblockableDamage = 30.0,
    UndodgeableDamage = 24.0,
    DodgedDefenderRecovery = 0.175
};
var hybridEngine = NewEngine(hybridTuning);
var quickUnblockable = hybridEngine.DescribeAttack(AttackKind.LightUnblockable);
Near(quickUnblockable.Startup, 0.500, "quick unblockable startup is 500 ms in the hybrid test");
Near(quickUnblockable.StaminaCost, 20.0, "quick special cost is 20");
Near(quickUnblockable.Damage, 16.0, "quick special damage is 16");
Check(!quickUnblockable.Blockable && !quickUnblockable.Parryable && quickUnblockable.Dodgeable,
    "quick unblockable retains dodge as its valid counter");
var quickUndodgeable = hybridEngine.DescribeAttack(AttackKind.LightUndodgeable);
Near(quickUndodgeable.Startup, 0.500, "quick undodgeable startup is 500 ms");
Near(quickUndodgeable.StaminaCost, 20.0, "quick undodgeable costs 20");
Check(quickUndodgeable.Blockable && quickUndodgeable.Parryable && !quickUndodgeable.Dodgeable,
    "quick undodgeable retains block and parry as valid counters");
Near(hybridEngine.DescribeAttack(AttackKind.Unblockable).Startup, 0.800,
    "heavy unblockable startup is 800 ms in the hybrid test");
Near(hybridEngine.DescribeAttack(AttackKind.Undodgeable).Startup, 0.800,
    "heavy undodgeable startup is 800 ms in the hybrid test");
Near(hybridEngine.DescribeAttack(AttackKind.Undodgeable).StaminaCost, 35.0,
    "heavy special cost is 35");
Near(hybridEngine.DescribeAttack(AttackKind.Undodgeable).Damage, 24.0,
    "heavy undodgeable damage exceeds its quick variant");

Check(hybridEngine.QueueAttack(hybridEngine.Player, AttackKind.LightUnblockable),
    "hybrid quick unblockable input is accepted");
var quickSpecial = hybridEngine.Player.CurrentAttack!;
Near(quickSpecial.ImpactAt, 0.500, "quick special lands after 500 ms");
Near(hybridEngine.Player.Stamina, 30.0, "quick special spends 20 stamina at launch");
AdvanceTo(hybridEngine, quickSpecial.ImpactAt);
hybridEngine.ActivateDefense(hybridEngine.Opponent, DefenseKind.Dodge);
AdvanceTo(hybridEngine, quickSpecial.ResolveAt);
Check(hybridEngine.Resolutions.Single().Outcome == AttackOutcome.Dodged,
    "quick unblockable can be dodged");
Near(hybridEngine.Player.Stamina, 30.0, "countered special does not refund its attacker");
Near(hybridEngine.Opponent.Stamina, 70.0, "valid special counter grants 20 stamina");
Near(hybridEngine.Player.AttackReadyAt, quickSpecial.ImpactAt + 0.180,
    "countering a Special leaves 180 ms attacker recovery");
Near(hybridEngine.Opponent.AttackReadyAt, quickSpecial.ImpactAt + 0.050,
    "countering a Special leaves 50 ms defender recovery");

var lightEngine = NewEngine();
Check(lightEngine.QueueAttack(lightEngine.Player, AttackKind.Light), "light attack input is accepted");
Check(lightEngine.Player.AttackBuffer.Count == 0, "ready light input launches immediately");
var lightAttack = lightEngine.Player.CurrentAttack ?? throw new InvalidOperationException("light attack is active");
Near(lightAttack.LaunchAt, 0.0, "ready attack launches at input time");
Near(lightAttack.ImpactAt, 0.180, "light lands after exactly 180 ms");
Check(lightAttack.Info.Lane == AttackLane.Mid, "light defaults to the middle lane");
Near(lightEngine.Player.Stamina, 40.0, "light attack costs 10 stamina");
AdvanceTo(lightEngine, lightAttack.ResolveAt);
Check(lightEngine.Opponent.Health == 88.0, "clean light hit applies its existing damage value");
Near(lightEngine.Player.Stamina, 40.0, "clean light hit does not refund its attacker");
Near(lightEngine.Opponent.Stamina, 50.0, "taking a Light does not fund a counter");
Near(lightEngine.Player.AttackReadyAt, 0.230, "light-hit attacker recovers 50 ms after impact");
Near(lightEngine.Opponent.AttackReadyAt, 0.230, "light-hit defender recovers 50 ms after impact");
Near(lightEngine.LastFrameAdvantage, 0.0, "quick Light hit has neutral recovery advantage");

var bufferedLightChain = NewEngine();
Check(bufferedLightChain.QueueAttack(bufferedLightChain.Player, AttackKind.Light),
    "first Light starts a buffered chain");
var firstChainLight = bufferedLightChain.Player.CurrentAttack!;
Check(bufferedLightChain.QueueAttack(bufferedLightChain.Player, AttackKind.Light),
    "second Light can be queued during startup");
AdvanceTo(bufferedLightChain, firstChainLight.ResolveAt);
var secondChainLight = bufferedLightChain.Player.CurrentAttack!;
Near(secondChainLight.LaunchAt, firstChainLight.ResolveAt,
    "queued Light launches when the prior attack resolves");
Near(secondChainLight.LaunchAt - firstChainLight.LaunchAt, 0.270,
    "the 180 ms Light plus defense-resolution grace gives a 270 ms minimum chain interval");
Near(secondChainLight.ImpactAt, 0.450,
    "the buffered follow-up uses the same 180 ms Light startup");

var blockedLightEngine = NewEngine();
blockedLightEngine.SetBlockHeld(blockedLightEngine.Opponent, true);
blockedLightEngine.QueueAttack(blockedLightEngine.Player, AttackKind.Light);
var blockedLight = blockedLightEngine.Player.CurrentAttack!;
AdvanceTo(blockedLightEngine, blockedLight.ResolveAt);
Near(blockedLightEngine.Player.Stamina, 40.0, "blocked light does not refund its attacker");
Near(blockedLightEngine.Opponent.Stamina, 60.0, "blocked light rewards defender by 10");
Near(blockedLightEngine.Player.AttackReadyAt, 0.230, "blocked Light attacker recovers after 50 ms");
Near(blockedLightEngine.Opponent.AttackReadyAt, 0.230, "blocked Light defender recovers after 50 ms");
Near(blockedLightEngine.LastFrameAdvantage, 0.0, "blocked Light has neutral recovery advantage");

var blockedRetaliation = NewEngine();
blockedRetaliation.SetBlockHeld(blockedRetaliation.Player, true);
blockedRetaliation.QueueAttack(blockedRetaliation.Opponent, AttackKind.Light, AttackLane.Mid);
blockedRetaliation.QueueAttack(blockedRetaliation.Player, AttackKind.Heavy, AttackLane.Low);
Check(blockedRetaliation.Player.AttackBuffer.Count == 1,
    "defender can buffer a heavy while holding block");
var initialThreat = blockedRetaliation.Opponent.CurrentAttack!;
AdvanceTo(blockedRetaliation, initialThreat.ImpactAt);
blockedRetaliation.SetBlockHeld(blockedRetaliation.Player, false);
AdvanceTo(blockedRetaliation, initialThreat.ResolveAt);
var defenderHeavy = blockedRetaliation.Player.CurrentAttack!;
Near(defenderHeavy.LaunchAt, 0.270, "buffered Heavy launches after the attack resolves");
Near(defenderHeavy.ImpactAt, 0.670, "retaliation Heavy impact is 670 ms");
blockedRetaliation.QueueAttack(blockedRetaliation.Opponent, AttackKind.Light, AttackLane.High);
var originalAttackerLight = blockedRetaliation.Opponent.CurrentAttack!;
Near(originalAttackerLight.LaunchAt, 0.270, "blocked-light attacker can immediately queue another Light");
Near(originalAttackerLight.ImpactAt, 0.450, "retaliation Light impacts at 450 ms");
Near(defenderHeavy.ImpactAt - originalAttackerLight.ImpactAt, 0.220,
    "the faster buffered Light impacts 220 ms before Heavy");
AdvanceTo(blockedRetaliation, originalAttackerLight.ResolveAt);
Check(blockedRetaliation.Resolutions.Any(resolution =>
        resolution.AttackKind == AttackKind.Light && !resolution.AttackerIsPlayer &&
        resolution.Outcome == AttackOutcome.CleanHit),
    "the follow-up Light can hit during Heavy startup without an interrupt rule");

var blockedHeavyEngine = NewEngine();
blockedHeavyEngine.SetBlockHeld(blockedHeavyEngine.Opponent, true);
blockedHeavyEngine.QueueAttack(blockedHeavyEngine.Player, AttackKind.Heavy, AttackLane.High);
var blockedHeavy = blockedHeavyEngine.Player.CurrentAttack!;
AdvanceTo(blockedHeavyEngine, blockedHeavy.ResolveAt);
Near(blockedHeavyEngine.Player.AttackReadyAt, 0.450, "blocked Heavy attacker recovers after 50 ms");
Near(blockedHeavyEngine.Opponent.AttackReadyAt, 0.450, "blocked Heavy defender recovers after 50 ms");
Near(blockedHeavyEngine.LastFrameAdvantage, 0.0, "blocked Heavy has neutral recovery advantage");

var noInterruptEngine = NewEngine();
noInterruptEngine.QueueAttack(noInterruptEngine.Opponent, AttackKind.Heavy);
noInterruptEngine.QueueAttack(noInterruptEngine.Player, AttackKind.Light);
var uninterruptibleHeavy = noInterruptEngine.Opponent.CurrentAttack!;
var noInterruptLight = noInterruptEngine.Player.CurrentAttack!;
AdvanceTo(noInterruptEngine, noInterruptLight.ResolveAt);
Check(!uninterruptibleHeavy.Cancelled && ReferenceEquals(noInterruptEngine.Opponent.CurrentAttack, uninterruptibleHeavy),
    "baseline keeps Heavy startup non-interruptible");
AdvanceTo(noInterruptEngine, uninterruptibleHeavy.ResolveAt);
Check(noInterruptEngine.Resolutions.Count == 2,
    "baseline allows both attacks to resolve when Light lands first");

var lightInterruptTuning = new CombatTuning
{
    AiAttackEnabled = false,
    AiDefenseEnabled = false,
    LightInterruptsHeavyStartup = true
};
var lightInterruptEngine = NewEngine(lightInterruptTuning);
lightInterruptEngine.QueueAttack(lightInterruptEngine.Opponent, AttackKind.Heavy);
lightInterruptEngine.QueueAttack(lightInterruptEngine.Opponent, AttackKind.Light);
lightInterruptEngine.QueueAttack(lightInterruptEngine.Player, AttackKind.Light);
var interruptedHeavy = lightInterruptEngine.Opponent.CurrentAttack!;
var interruptingLight = lightInterruptEngine.Player.CurrentAttack!;
Check(lightInterruptEngine.Opponent.AttackBuffer.Count == 1,
    "a follow-up remains buffered behind Heavy startup");
AdvanceTo(lightInterruptEngine, interruptingLight.ResolveAt);
Check(interruptedHeavy.Cancelled && interruptedHeavy.Resolved &&
      !ReferenceEquals(lightInterruptEngine.Opponent.CurrentAttack, interruptedHeavy),
    "a clean Light cancels an opposing Heavy still in startup");
Check(!lightInterruptEngine.Resolutions.Any(resolution => resolution.AttackId == interruptedHeavy.Id),
    "a cancelled Heavy does not resolve or deal damage");
Near(lightInterruptEngine.Opponent.Health, 88.0, "the interrupting Light still deals normal damage");
Near(lightInterruptEngine.Resolutions.Single().DefenderReadyAt, interruptingLight.ImpactAt + 0.050,
    "the interrupted Heavy fighter recovers from the Light hit");
Check(lightInterruptEngine.Events.Any(item => item.Kind == CombatEventKind.AttackCancelled),
    "Heavy cancellation is visible in the event log");
var bufferedFollowup = lightInterruptEngine.Opponent.CurrentAttack!;
Near(bufferedFollowup.LaunchAt, 0.270,
    "buffered follow-up launches after the short interruption recovery");
Near(bufferedFollowup.ImpactAt, 0.450,
    "buffered follow-up Light uses the fast baseline startup");
AdvanceTo(lightInterruptEngine, interruptedHeavy.ResolveAt);
Check(!lightInterruptEngine.Resolutions.Any(resolution => resolution.AttackId == interruptedHeavy.Id),
    "cancelled Heavy cannot resolve at its old scheduled impact");

var parriedInterruptEngine = NewEngine(lightInterruptTuning);
parriedInterruptEngine.QueueAttack(parriedInterruptEngine.Opponent, AttackKind.Heavy);
parriedInterruptEngine.QueueAttack(parriedInterruptEngine.Player, AttackKind.Light);
var parryProtectedHeavy = parriedInterruptEngine.Opponent.CurrentAttack!;
var parriedInterruptingLight = parriedInterruptEngine.Player.CurrentAttack!;
AdvanceTo(parriedInterruptEngine, parriedInterruptingLight.ImpactAt);
parriedInterruptEngine.ActivateDefense(parriedInterruptEngine.Opponent, DefenseKind.Parry);
AdvanceTo(parriedInterruptEngine, parriedInterruptingLight.ResolveAt);
Check(!parryProtectedHeavy.Cancelled,
    "a parried Light does not interrupt Heavy startup");

var dodgeThenNextLightEngine = NewEngine(lightInterruptTuning);
dodgeThenNextLightEngine.QueueAttack(dodgeThenNextLightEngine.Player, AttackKind.Light);
var dodgedLight = dodgeThenNextLightEngine.Player.CurrentAttack!;
AdvanceTo(dodgeThenNextLightEngine, dodgedLight.ImpactAt - 0.090);
Check(dodgeThenNextLightEngine.ActivateDefense(
        dodgeThenNextLightEngine.Opponent, DefenseKind.Dodge),
    "DodgeHeavy's dodge input is accepted at the Light timing-window boundary");
Check(dodgeThenNextLightEngine.QueueAttack(
        dodgeThenNextLightEngine.Opponent, AttackKind.Heavy),
    "DodgeHeavy buffers Heavy behind its dodge");
Check(dodgeThenNextLightEngine.QueueAttack(
        dodgeThenNextLightEngine.Player, AttackKind.Light),
    "the original attacker buffers its next Light while the first is in flight");
AdvanceTo(dodgeThenNextLightEngine, dodgedLight.ResolveAt);
Check(dodgeThenNextLightEngine.Resolutions.Single().Outcome == AttackOutcome.Dodged,
    "the first Light is successfully dodged");
var dodgeHeavy = dodgeThenNextLightEngine.Opponent.CurrentAttack!;
Near(dodgeHeavy.LaunchAt, 0.270, "DodgeHeavy launches after its dodge window resolves");
Near(dodgeHeavy.ImpactAt, 0.670, "DodgeHeavy schedules its Heavy impact");
var nextLight = dodgeThenNextLightEngine.Player.CurrentAttack!;
Near(nextLight.LaunchAt, 0.270, "the next Light launches after the first attack resolves");
Near(nextLight.ImpactAt, 0.450, "the next Light arrives before DodgeHeavy's Heavy");
AdvanceTo(dodgeThenNextLightEngine, nextLight.ResolveAt);
Check(dodgeHeavy.Cancelled,
    "a clean next Light cancels DodgeHeavy's normal Heavy startup");

var simultaneousInterruptEngine = NewEngine(new CombatTuning
{
    AiAttackEnabled = false,
    AiDefenseEnabled = false,
    LightStartup = 0.700,
    HeavyStartup = 0.700,
    LightInterruptsHeavyStartup = true
});
simultaneousInterruptEngine.QueueAttack(simultaneousInterruptEngine.Opponent, AttackKind.Heavy);
simultaneousInterruptEngine.QueueAttack(simultaneousInterruptEngine.Player, AttackKind.Light);
var simultaneousHeavy = simultaneousInterruptEngine.Opponent.CurrentAttack!;
var simultaneousLight = simultaneousInterruptEngine.Player.CurrentAttack!;
AdvanceTo(simultaneousInterruptEngine, simultaneousLight.ResolveAt);
Check(!simultaneousHeavy.Cancelled && simultaneousInterruptEngine.Resolutions.Count == 2,
    "simultaneous impacts trade instead of triggering an interrupt");

var parryEngine = NewEngine();
parryEngine.QueueAttack(parryEngine.Player, AttackKind.Light);
var parriedLight = parryEngine.Player.CurrentAttack!;
AdvanceTo(parryEngine, parriedLight.ImpactAt);
Check(parryEngine.ActivateDefense(parryEngine.Opponent, DefenseKind.Parry),
    "parry activation at impact is accepted");
AdvanceTo(parryEngine, parriedLight.ResolveAt);
Check(parryEngine.Resolutions.Single().Outcome == AttackOutcome.Parried, "accurate parry prevents the hit");
Near(parryEngine.Player.Stamina, 40.0, "parried light attacker receives no stamina refund");
Near(parryEngine.Opponent.Stamina, 60.0, "Light parry rewards defender by 10");
Near(parryEngine.Player.AttackReadyAt, 0.300, "parried attacker recovers after 120 ms");
Near(parryEngine.Opponent.AttackReadyAt, 0.230, "parry defender recovers after 50 ms");
Near(parryEngine.LastFrameAdvantage, -0.070, "Light parry preserves a short retaliation opening");

var heavyParryEngine = NewEngine();
Check(heavyParryEngine.QueueAttack(heavyParryEngine.Player, AttackKind.Heavy),
    "Heavy attack is accepted for the Heavy parry reward test");
var parriedHeavy = heavyParryEngine.Player.CurrentAttack!;
AdvanceTo(heavyParryEngine, parriedHeavy.ImpactAt);
Check(heavyParryEngine.ActivateDefense(heavyParryEngine.Opponent, DefenseKind.Parry),
    "Heavy can be parried at impact");
AdvanceTo(heavyParryEngine, parriedHeavy.ResolveAt);
Check(heavyParryEngine.Resolutions.Single().Outcome == AttackOutcome.Parried,
    "accurate Heavy parry counters the attack");
Near(heavyParryEngine.Opponent.Stamina, 65.0, "Heavy parry rewards defender by 15");

var successfulDefenseLockoutEngine = NewEngine(new CombatTuning
{
    AiAttackEnabled = false,
    AiDefenseEnabled = false,
    FailedDefenseRecovery = 0.250
});
successfulDefenseLockoutEngine.QueueAttack(successfulDefenseLockoutEngine.Player, AttackKind.Light);
var lockoutTestAttack = successfulDefenseLockoutEngine.Player.CurrentAttack!;
AdvanceTo(successfulDefenseLockoutEngine, lockoutTestAttack.ImpactAt - 0.050);
Check(successfulDefenseLockoutEngine.ActivateDefense(
        successfulDefenseLockoutEngine.Opponent, DefenseKind.Parry),
    "parry inside the lockout test's valid window activates");
var successfulParryActivation = successfulDefenseLockoutEngine.Opponent.DefenseActivations.Single();
AdvanceTo(successfulDefenseLockoutEngine, lockoutTestAttack.ResolveAt);
Check(successfulParryActivation.Consumed && !successfulParryActivation.Failed,
    "a successful parry does not trigger failed-defense recovery");
Near(successfulDefenseLockoutEngine.Opponent.BlockReadyAt, 0.0,
    "successful parry leaves unrelated defense readiness unchanged");

var parryRetaliation = NewEngine();
parryRetaliation.QueueAttack(parryRetaliation.Player, AttackKind.Light);
var initialParryThreat = parryRetaliation.Player.CurrentAttack!;
AdvanceTo(parryRetaliation, initialParryThreat.ImpactAt);
parryRetaliation.ActivateDefense(parryRetaliation.Opponent, DefenseKind.Parry);
parryRetaliation.QueueAttack(parryRetaliation.Opponent, AttackKind.Heavy, AttackLane.Low);
parryRetaliation.QueueAttack(parryRetaliation.Player, AttackKind.Light, AttackLane.High);
Check(parryRetaliation.Opponent.AttackBuffer.Count == 1,
    "heavy queues during parry and waits for attack readiness");
AdvanceTo(parryRetaliation, initialParryThreat.ResolveAt);
var parryDefenderHeavy = parryRetaliation.Opponent.CurrentAttack!;
Near(parryDefenderHeavy.LaunchAt, 0.270, "parry defender launches Heavy after resolution");
Near(parryDefenderHeavy.ImpactAt, 0.670, "parry retaliation Heavy impact is 670 ms");
AdvanceTo(parryRetaliation, 0.300);
var parryAttackerLight = parryRetaliation.Player.CurrentAttack!;
Near(parryAttackerLight.LaunchAt, 0.300, "parry attacker can launch a buffered Light after recovery");
Near(parryAttackerLight.ImpactAt, 0.480, "parry retaliation Light impact is 480 ms");
Near(parryDefenderHeavy.ImpactAt - parryAttackerLight.ImpactAt, 0.190,
    "buffered Light arrives before the parry defender's Heavy");

var parryToleranceEngine = NewEngine();
parryToleranceEngine.QueueAttack(parryToleranceEngine.Opponent, AttackKind.Light);
var toleranceThreat = parryToleranceEngine.Opponent.CurrentAttack!;
AdvanceTo(parryToleranceEngine, toleranceThreat.ImpactAt - 0.060);
Check(parryToleranceEngine.ActivateDefense(parryToleranceEngine.Player, DefenseKind.Parry),
    "parry input 60 ms early is accepted");
AdvanceTo(parryToleranceEngine, toleranceThreat.ResolveAt);
Check(parryToleranceEngine.Resolutions.Single().Outcome == AttackOutcome.Parried,
    "parry interval is centered on impact with a 60 ms early boundary");

var narrowParryTuning = new CombatTuning
{
    AiAttackEnabled = false,
    AiDefenseEnabled = false,
    ParryWindow = 0.120
};
var narrowParryBoundaryEngine = NewEngine(narrowParryTuning);
narrowParryBoundaryEngine.QueueAttack(narrowParryBoundaryEngine.Opponent, AttackKind.Light);
var narrowParryThreat = narrowParryBoundaryEngine.Opponent.CurrentAttack!;
AdvanceTo(narrowParryBoundaryEngine, narrowParryThreat.ImpactAt - 0.060);
narrowParryBoundaryEngine.ActivateDefense(narrowParryBoundaryEngine.Player, DefenseKind.Parry);
AdvanceTo(narrowParryBoundaryEngine, narrowParryThreat.ResolveAt);
Check(narrowParryBoundaryEngine.Resolutions.Single().Outcome == AttackOutcome.Parried,
    "a 120 ms parry window includes its 60 ms early boundary");

var outsideNarrowParryEngine = NewEngine(narrowParryTuning);
outsideNarrowParryEngine.QueueAttack(outsideNarrowParryEngine.Opponent, AttackKind.Light);
var outsideNarrowParryThreat = outsideNarrowParryEngine.Opponent.CurrentAttack!;
AdvanceTo(outsideNarrowParryEngine, outsideNarrowParryThreat.ImpactAt - 0.061);
outsideNarrowParryEngine.ActivateDefense(outsideNarrowParryEngine.Player, DefenseKind.Parry);
AdvanceTo(outsideNarrowParryEngine, outsideNarrowParryThreat.ResolveAt);
Check(outsideNarrowParryEngine.Resolutions.Single().Outcome == AttackOutcome.CleanHit,
    "a parry 61 ms before impact falls outside the 120 ms window");

var lateParryEngine = NewEngine();
lateParryEngine.QueueAttack(lateParryEngine.Opponent, AttackKind.Light);
var lateThreat = lateParryEngine.Opponent.CurrentAttack!;
AdvanceTo(lateParryEngine, lateThreat.ImpactAt + 0.050);
Check(lateParryEngine.ActivateDefense(lateParryEngine.Player, DefenseKind.Parry),
    "parry input 50 ms after impact is accepted during the impact grace period");
AdvanceTo(lateParryEngine, lateThreat.ResolveAt);
Check(lateParryEngine.Resolutions.Single().Outcome == AttackOutcome.Parried,
    "late input inside the symmetric 60 ms tolerance still parries");

var lateBoundaryEngine = NewEngine();
lateBoundaryEngine.QueueAttack(lateBoundaryEngine.Opponent, AttackKind.Light);
var lateBoundaryThreat = lateBoundaryEngine.Opponent.CurrentAttack!;
AdvanceTo(lateBoundaryEngine, lateBoundaryThreat.ImpactAt + 0.059);
Check(lateBoundaryEngine.ActivateDefense(lateBoundaryEngine.Player, DefenseKind.Parry),
    "parry input immediately inside the 60 ms late boundary is accepted");
AdvanceTo(lateBoundaryEngine, lateBoundaryThreat.ResolveAt);
Check(lateBoundaryEngine.Resolutions.Single().Outcome == AttackOutcome.Parried,
    "parry remains valid immediately inside the late boundary");

var beyondLateBoundaryEngine = NewEngine();
beyondLateBoundaryEngine.QueueAttack(beyondLateBoundaryEngine.Opponent, AttackKind.Light);
var beyondLateBoundaryThreat = beyondLateBoundaryEngine.Opponent.CurrentAttack!;
AdvanceTo(beyondLateBoundaryEngine, beyondLateBoundaryThreat.ResolveAt + 0.001);
Check(beyondLateBoundaryEngine.ActivateDefense(beyondLateBoundaryEngine.Player, DefenseKind.Parry),
    "a parry can be entered after the attack has already resolved");
Check(beyondLateBoundaryEngine.Resolutions.Single().Outcome == AttackOutcome.CleanHit,
    "a parry after the impact grace period cannot retroactively prevent damage");

var dodgeEngine = NewEngine();
dodgeEngine.QueueAttack(dodgeEngine.Opponent, AttackKind.Unblockable, AttackLane.Low);
var dodgeThreat = dodgeEngine.Opponent.CurrentAttack!;
AdvanceTo(dodgeEngine, dodgeThreat.ImpactAt);
Check(dodgeEngine.ActivateDefense(dodgeEngine.Player, DefenseKind.Dodge), "dodge activates on demand");
AdvanceTo(dodgeEngine, dodgeThreat.ResolveAt);
Check(dodgeEngine.Resolutions.Single().Outcome == AttackOutcome.Dodged,
    "dodge avoids an unblockable strike");
Near(dodgeEngine.Player.Stamina, 70.0, "successful special counter rewards defender by 20");
Near(dodgeEngine.Opponent.Stamina, 20.0, "special attack receives no attacker refund");

var undodgeableEngine = NewEngine();
undodgeableEngine.QueueAttack(undodgeableEngine.Opponent, AttackKind.Undodgeable);
var undodgeable = undodgeableEngine.Opponent.CurrentAttack!;
AdvanceTo(undodgeableEngine, undodgeable.ImpactAt);
undodgeableEngine.ActivateDefense(undodgeableEngine.Player, DefenseKind.Dodge);
AdvanceTo(undodgeableEngine, undodgeable.ResolveAt);
Check(undodgeableEngine.Resolutions.Single().Outcome == AttackOutcome.CleanHit,
    "dodge does not counter an undodgeable special");
Near(undodgeableEngine.Player.Stamina, 50.0, "invalid defense outcome grants no stamina");
Near(undodgeableEngine.Opponent.Stamina, 20.0, "undodgeable special pays exactly 30 stamina");

hybridTuning.DodgeActionRecovery = 0.350;
hybridTuning.ParryActionRecovery = 0.250;
var dodgeRecoveryEngine = NewEngine(hybridTuning);
dodgeRecoveryEngine.QueueAttack(dodgeRecoveryEngine.Opponent, AttackKind.Light);
var dodgeRecoveryAttack = dodgeRecoveryEngine.Opponent.CurrentAttack!;
AdvanceTo(dodgeRecoveryEngine, dodgeRecoveryAttack.ImpactAt);
dodgeRecoveryEngine.ActivateDefense(dodgeRecoveryEngine.Player, DefenseKind.Dodge);
AdvanceTo(dodgeRecoveryEngine, dodgeRecoveryAttack.ResolveAt);
Near(dodgeRecoveryEngine.Player.AttackReadyAt, dodgeRecoveryAttack.ImpactAt + 0.175,
    "hybrid successful-dodge recovery is 175 ms");
Near(dodgeRecoveryEngine.Player.DodgeReadyAt, dodgeRecoveryAttack.ImpactAt + 0.350,
    "Sifu recovery profile keeps dodge unavailable for 350 ms");

var parryActionRecoveryEngine = NewEngine(hybridTuning);
parryActionRecoveryEngine.QueueAttack(parryActionRecoveryEngine.Opponent, AttackKind.Light);
var parryActionRecoveryAttack = parryActionRecoveryEngine.Opponent.CurrentAttack!;
AdvanceTo(parryActionRecoveryEngine, parryActionRecoveryAttack.ImpactAt);
parryActionRecoveryEngine.ActivateDefense(parryActionRecoveryEngine.Player, DefenseKind.Parry);
AdvanceTo(parryActionRecoveryEngine, parryActionRecoveryAttack.ResolveAt);
Near(parryActionRecoveryEngine.Player.ParryReadyAt, parryActionRecoveryAttack.ImpactAt + 0.250,
    "Sifu recovery profile keeps parry unavailable for 250 ms");

var capEngine = NewEngine();
capEngine.Tuning.HeavyCost = 0.0;
capEngine.Tuning.HeavyAttackerCleanHitGain = 10.0;
capEngine.Player.Stamina = 95.0;
capEngine.QueueAttack(capEngine.Player, AttackKind.Heavy);
var cappedHeavy = capEngine.Player.CurrentAttack!;
AdvanceTo(capEngine, cappedHeavy.ResolveAt);
Near(capEngine.Player.Stamina, 100.0, "stamina rewards clamp at the 100 cap");
Near(capEngine.StaminaLedger.Where(change => change.IsPlayer && change.WastedAtCap > 0.0)
    .Sum(change => change.WastedAtCap), 5.0, "wasted stamina at cap is measured");

var defenseReadiness = NewEngine(new CombatTuning
{
    AiAttackEnabled = false,
    AiDefenseEnabled = false,
    BlockActionRecovery = 0.500
});
defenseReadiness.SetBlockHeld(defenseReadiness.Opponent, true);
defenseReadiness.QueueAttack(defenseReadiness.Player, AttackKind.Light);
var blockReadinessAttack = defenseReadiness.Player.CurrentAttack!;
AdvanceTo(defenseReadiness, blockReadinessAttack.ResolveAt);
Near(defenseReadiness.Opponent.BlockReadyAt, blockReadinessAttack.ImpactAt + 0.500,
    "block readiness has its own configurable recovery timestamp");
Check(defenseReadiness.ActivateDefense(defenseReadiness.Opponent, DefenseKind.Dodge),
    "a defender can dodge while their attack readiness is still on recovery");

var simultaneous = NewEngine();
simultaneous.QueueAttack(simultaneous.Opponent, AttackKind.Light, AttackLane.High);
simultaneous.QueueAttack(simultaneous.Player, AttackKind.Light, AttackLane.Mid);
var playerAttack = simultaneous.Player.CurrentAttack!;
var opponentAttack = simultaneous.Opponent.CurrentAttack!;
simultaneous.SetBlockHeld(simultaneous.Player, true);
AdvanceTo(simultaneous, 0.100);
Check(simultaneous.ActivateDefense(simultaneous.Opponent, DefenseKind.Dodge),
    "opponent can defend while its own attack remains active");
AdvanceTo(simultaneous, Math.Max(playerAttack.ResolveAt, opponentAttack.ResolveAt));
Check(simultaneous.Resolutions.Count == 2, "both attacks resolve independently");
Check(simultaneous.Resolutions.Select(result => result.AttackId)
    .SequenceEqual(new[] { opponentAttack.Id, playerAttack.Id }),
    "simultaneous impacts resolve in deterministic attack-ID order");
Check(simultaneous.Resolutions.Any(result => result.Outcome == AttackOutcome.Blocked),
    "player's held block counters the incoming attack");
Check(simultaneous.Resolutions.Any(result => result.Outcome == AttackOutcome.Dodged),
    "opponent's timed dodge counters the incoming attack");
Check(simultaneous.Player.CurrentAttack is null && simultaneous.Opponent.CurrentAttack is null,
    "simultaneous attacks are not canceled by the opposing hit");

var heavyNotCanceled = NewEngine();
heavyNotCanceled.QueueAttack(heavyNotCanceled.Opponent, AttackKind.Heavy);
heavyNotCanceled.QueueAttack(heavyNotCanceled.Player, AttackKind.Light);
var pendingHeavy = heavyNotCanceled.Opponent.CurrentAttack!;
AdvanceTo(heavyNotCanceled, heavyNotCanceled.Player.CurrentAttack!.ResolveAt);
Check(heavyNotCanceled.Opponent.CurrentAttack == pendingHeavy && !pendingHeavy.Resolved,
    "a light hit does not automatically interrupt an incoming heavy");

var telegraphEngine = NewEngine(new CombatTuning
{
    AiAttackEnabled = true,
    AiDefenseEnabled = false,
    AiInitialDelay = 0.450,
    TelegraphLead = 0.200
});
Check(telegraphEngine.GetIncomingThreat(telegraphEngine.Player) is null,
    "the opponent's future script is hidden before the configured telegraph lead");
AdvanceTo(telegraphEngine, 0.250);
var preview = telegraphEngine.GetIncomingThreat(telegraphEngine.Player);
var previewValue = preview ?? throw new InvalidOperationException("configurable preview becomes visible");
Check(!previewValue.Launched, "configurable preview reveals only the visible telegraph");
Check(previewValue.Info.Kind == AttackKind.Light && previewValue.Info.Lane == AttackLane.Mid,
    "the scripted opponent starts with a middle-lane light");
AdvanceTo(telegraphEngine, 0.450);
Check(telegraphEngine.Opponent.CurrentAttack is { } aiAttack &&
    aiAttack.Info.Kind == previewValue.Info.Kind && aiAttack.Info.Lane == previewValue.Info.Lane,
    "the launched deterministic attack matches its earlier public preview");

var activeTelegraph = NewEngine(new CombatTuning
{
    AiAttackEnabled = false,
    AiDefenseEnabled = false,
    TelegraphLead = 0.150
});
activeTelegraph.QueueAttack(activeTelegraph.Player, AttackKind.Light);
var launchedThreat = activeTelegraph.GetIncomingThreat(activeTelegraph.Opponent);
Check(launchedThreat is { Launched: true }, "a launched attack is publicly visible");
Near(launchedThreat!.Value.VisibleAt, launchedThreat.Value.LaunchAt,
    "active-attack visibility is not backdated by an unavailable telegraph lead");

var firstScript = NewEngine(new CombatTuning { AiDefenseEnabled = false });
foreach (var readChance in new[] { 0.0, 1.0 })
{
    var scripted = NewEngine(new CombatTuning { AiAttackEnabled = false, AiParryReadChance = readChance });
    scripted.QueueAttack(scripted.Player, AttackKind.Undodgeable);
    var resolveAt = scripted.Player.CurrentAttack!.ResolveAt;
    while (scripted.Time < resolveAt) scripted.Tick(CombatEngine.FixedStep);
    Check(scripted.Resolutions.Single().Outcome == (readChance == 0 ? AttackOutcome.CleanHit : AttackOutcome.Parried),
        "scripted parry reads can be skipped; a committed on-time parry is deterministic");
}
var secondScript = NewEngine(new CombatTuning { AiDefenseEnabled = false });
for (var index = 0; index < 600; index++)
{
    firstScript.Tick(CombatEngine.FixedStep);
    secondScript.Tick(CombatEngine.FixedStep);
}
Check(firstScript.Events.Select(item => item.Message).SequenceEqual(secondScript.Events.Select(item => item.Message)),
    "scripted opponent sequences are deterministic");

static CombatEngine NewCounterTrial()
{
    return new CombatEngine(new CombatTuning
    {
        AiAttackEnabled = false,
        AiDefenseEnabled = false,
        StaminaRegenerationPerSecond = 0,
        AttackInputInterval = 0
    }, new CounterTrial(), trackHealth: false);
}

foreach (var (defense, special) in new[] { (DefenseKind.Parry, AttackKind.Unparryable), (DefenseKind.Dodge, AttackKind.Undodgeable) })
{
    var counter = NewCounterTrial();
    var trial = counter.CounterTrial!;
    Check(!counter.QueueAttack(counter.Player, special), "counter moves can't be bought in neutral");
    Near(counter.DescribeAttack(AttackKind.Light).Startup, 0.320, "Counter trial Lights provide 320 ms of warning");
    Near(counter.DescribeAttack(special).Startup, 0.400, "Counter trial Heavy startup is unchanged");
    Near(counter.EffectiveDodgeWindow, 0.120, "Counter trial defense window is unchanged");
    Check(counter.EffectiveDodgeWindow == counter.Tuning.ParryWindow, "both active defense windows match");
    Check(counter.ActivateDefense(counter.Player, defense), "empty defense input does not buy a counter");
    Check(trial.Counter(true, counter.Time) is null, "only a successful counter earns a token");
    counter.Tick(0.28);
    counter.QueueAttack(counter.Opponent, AttackKind.Light);
    var incoming = counter.Opponent.CurrentAttack!;
    Near(incoming.ImpactAt - incoming.LaunchAt, 0.320, "both sides use the Counter trial Light startup");
    AdvanceTo(counter, incoming.ImpactAt);
    Check(counter.ActivateDefense(counter.Player, defense), "an in-range Light can be defended with either input");
    AdvanceTo(counter, incoming.ResolveAt);
    Near(counter.Player.Stamina, 60, "active defenses reward equally");
    Near(trial.CounterRemaining(true, counter.Time), CounterTrial.CounterSeconds, "counter window starts on confirmation");
    var response = counter.DescribeAttack(special);
    Check(response.Blockable && response.Dodgeable == (special == AttackKind.Unparryable) &&
        response.Parryable == (special == AttackKind.Undodgeable), "each special beats its own defense");
    if (defense == DefenseKind.Dodge)
    {
        counter.Tick(CounterTrial.CounterSeconds);
        Check(!counter.QueueAttack(counter.Player, special), "counter expires without an attack");
    }
    else
    {
        counter.Player.Stamina = 0;
        Check(!counter.QueueAttack(counter.Player, special) && trial.Counter(true, counter.Time) == special,
            "unaffordable follow-up does not consume the opportunity");
        counter.Player.Stamina = 60;
        Check(counter.QueueAttack(counter.Player, special) && trial.Counter(true, counter.Time) is null,
            "one accepted attack spends the opportunity");
        Check(!counter.QueueAttack(counter.Player, special), "one counter cannot fund two specials");
    }
}

var spacing = NewCounterTrial();
var footwork = spacing.CounterTrial!;
Check(!spacing.SetBlockHeld(spacing.Player, true), "Counter trial guard is entered through the parry input");
spacing.QueueAttack(spacing.Opponent, AttackKind.Light, AttackLane.High);
var lightAtRange = spacing.Opponent.CurrentAttack!;
Check(lightAtRange.Info.Lane == AttackLane.Mid, "all attacks use the single lane");
AdvanceTo(spacing, lightAtRange.ImpactAt - 0.06);
footwork.SetMovement(-1);
AdvanceTo(spacing, lightAtRange.ImpactAt);
Check(!footwork.InReach(AttackKind.Light), "a backstep can make a Light miss");
Check(lightAtRange.StrikeAtImpact is { InReach: false } && ReferenceEquals(footwork.LastStrike(false), lightAtRange),
    "the arena receives the same missed range sample used at impact");
var missedScan = lightAtRange.StrikeAtImpact!.Value;
Near(missedScan.AttackerPosition, footwork.OpponentPosition, "the scan stores the attacker's impact position");
Near(missedScan.DefenderPosition, footwork.PlayerPosition, "the scan stores the defender's impact position");
spacing.ActivateDefense(spacing.Player, DefenseKind.Parry);
footwork.SetMovement(1);
AdvanceTo(spacing, lightAtRange.ResolveAt);
Check(footwork.InReach(AttackKind.Light) && spacing.Resolutions.Single().Outcome == AttackOutcome.Whiffed,
    "stepping back in during the late parry window does not change reach at impact");
Check(lightAtRange.StrikeAtImpact == missedScan, "moving after impact does not move the displayed scan");
Near(spacing.Player.DamageTaken, 0, "a whiff does not deal damage");
Near(spacing.Player.Stamina, 50, "a whiff cannot pay out a defense reward");
Check(footwork.Counter(true, spacing.Time) is null, "parrying a whiff does not grant a special");
spacing.Tick(2);
Near(footwork.Gap, CounterTrial.BodyGap, "fighters stay solid and cannot pass through each other");

var reach = NewCounterTrial();
reach.CounterTrial!.SetMovement(-1);
reach.Tick(0.15);
reach.CounterTrial.SetMovement(0);
Check(!reach.CounterTrial.InReach(AttackKind.Light) && reach.CounterTrial.InReach(AttackKind.Unparryable),
    "counter Heavy has longer reach than Light");
reach.CounterTrial.Resolve(false, AttackOutcome.Parried, reach.Time);
reach.QueueAttack(reach.Opponent, AttackKind.Unparryable);
AdvanceTo(reach, reach.Opponent.CurrentAttack!.ResolveAt);
Check(reach.Resolutions.Single().Outcome == AttackOutcome.CleanHit, "the longer Heavy can land from outside Light range");

foreach (var sameImpact in new[] { false, true })
{
    var duel = NewCounterTrial();
    if (sameImpact) duel.Tuning.CounterTrialLightStartup = duel.Tuning.HeavyStartup;
    duel.CounterTrial!.Resolve(false, AttackOutcome.Dodged, duel.Time);
    duel.QueueAttack(duel.Opponent, AttackKind.Undodgeable);
    var heavySwing = duel.Opponent.CurrentAttack!;
    var interrupts = new List<long>();
    duel.AttackInterrupted += attack => interrupts.Add(attack.Id);
    duel.QueueAttack(duel.Player, AttackKind.Light);
    AdvanceTo(duel, heavySwing.ResolveAt);
    Check(heavySwing.Cancelled != sameImpact, "only an earlier clean Light interrupts a counter Heavy");
    Check(duel.Resolutions.Count == (sameImpact ? 2 : 1), "equal impact times trade; interrupted attacks do not hit");
    Check(sameImpact ? interrupts.Count == 0 : interrupts.SequenceEqual(new[] { heavySwing.Id }),
        "the chart gets one interruption signal for the canceled swing, not for a trade");
}

var noInterruptDuel = NewCounterTrial();
noInterruptDuel.CounterTrial!.InterruptHeavyStartup = false;
noInterruptDuel.CounterTrial.OpponentLightStartup = 0.110;
noInterruptDuel.CounterTrial.PlayerLightStartup = 0.160;
noInterruptDuel.CounterTrial.Resolve(true, AttackOutcome.Dodged, noInterruptDuel.Time);
Check(noInterruptDuel.QueueAttack(noInterruptDuel.Player, AttackKind.Undodgeable), "player starts a committed Heavy");
var liveTrialHeavy = noInterruptDuel.Player.CurrentAttack!;
Check(noInterruptDuel.QueueAttack(noInterruptDuel.Opponent, AttackKind.Light), "bot starts a faster Light");
Near(noInterruptDuel.Opponent.CurrentAttack!.Info.Startup, 0.110, "bot windup uses its own override");
Near(noInterruptDuel.DescribeAttack(AttackKind.Light).Startup, 0.160, "player windup stays independent");
AdvanceTo(noInterruptDuel, liveTrialHeavy.ResolveAt);
Check(!liveTrialHeavy.Cancelled && noInterruptDuel.Resolutions.Count(r => r.Outcome == AttackOutcome.CleanHit) == 2,
    "when interruptions are disabled an earlier Light and the ongoing Heavy both land");

foreach (var (defense, special) in new[] { (DefenseKind.Parry, AttackKind.Unparryable), (DefenseKind.Dodge, AttackKind.Undodgeable) })
{
    var duel = NewCounterTrial();
    duel.Player.Stamina = 10;
    duel.QueueAttack(duel.Opponent, AttackKind.Light);
    var incoming = duel.Opponent.CurrentAttack!;
    AdvanceTo(duel, incoming.ImpactAt - 0.05);
    duel.ActivateDefense(duel.Player, defense);
    Check(duel.QueueTrialAttack(duel.Player), "a quick follow-up press can wait for confirmation");
    duel.QueueTrialAttack(duel.Player);
    Check(duel.CounterTrial!.HasBufferedCounterInput(true) && duel.Player.CurrentAttack is null,
        "early presses buy nothing until the defense resolves");
    AdvanceTo(duel, incoming.ResolveAt);
    Check(duel.AttackHistory.Count(a => a.Attacker.IsPlayer && a.Info.Kind == special) == 1 &&
        !duel.CounterTrial.HasBufferedCounterInput(true) && duel.Player.BufferedAttackCount == 0,
        "early input launches exactly one eligible Heavy");
    Near(duel.Player.Stamina, 0, "the follow-up can spend the stamina from the successful defense");
}

var earlyWhiff = NewCounterTrial();
earlyWhiff.QueueAttack(earlyWhiff.Opponent, AttackKind.Light);
var whiffedAttack = earlyWhiff.Opponent.CurrentAttack!;
earlyWhiff.CounterTrial!.SetMovement(-1);
AdvanceTo(earlyWhiff, whiffedAttack.ImpactAt - 0.04);
earlyWhiff.ActivateDefense(earlyWhiff.Player, DefenseKind.Parry);
earlyWhiff.QueueTrialAttack(earlyWhiff.Player);
AdvanceTo(earlyWhiff, whiffedAttack.ResolveAt);
Check(whiffedAttack.Outcome == AttackOutcome.Whiffed &&
    earlyWhiff.Player.CurrentAttack?.Info.Kind == AttackKind.Light, "a buffered attack falls back to a Light when the defense does not land");

var damageTrial = NewCounterTrial();
damageTrial.Tuning.LightCost = 0;
for (var i = 0; i < 10; i++)
{
    Check(damageTrial.QueueAttack(damageTrial.Opponent, AttackKind.Light, AttackLane.High), "practice continues after 100 damage");
    AdvanceTo(damageTrial, damageTrial.Opponent.CurrentAttack!.ResolveAt);
}
Near(damageTrial.Player.DamageTaken, 120, "damage taken is cumulative and not capped at 100");
Near(damageTrial.Opponent.DamageTaken, 0, "taking damage only affects the defender's counter");
Near(damageTrial.Player.Health, 100, "the practice engine does not deduct HP");

var holdGuard = NewCounterTrial();
holdGuard.SetParryHeld(holdGuard.Player, true);
Check(!holdGuard.CounterTrial!.IsGuarding(true, holdGuard.Time), "the press starts as a parry, not a block");
holdGuard.QueueAttack(holdGuard.Opponent, AttackKind.Light);
var guardedLight = holdGuard.Opponent.CurrentAttack!;
AdvanceTo(holdGuard, guardedLight.ImpactAt);
Check(guardedLight.StrikeAtImpact is { Guarded: true }, "holding through impact enables the fallback guard");
holdGuard.SetParryHeld(holdGuard.Player, false);
AdvanceTo(holdGuard, guardedLight.ResolveAt);
Check(guardedLight.Outcome == AttackOutcome.Blocked, "releasing during the late-input grace does not undo an on-time guard");
Near(holdGuard.Player.Stamina, 42, "a held Light block costs 8");
Check(holdGuard.CounterTrial.Counter(true, holdGuard.Time) is null, "blocks don't grant a counter");

var tapGuard = NewCounterTrial();
tapGuard.SetParryHeld(tapGuard.Player, true);
tapGuard.SetParryHeld(tapGuard.Player, false);
tapGuard.QueueAttack(tapGuard.Opponent, AttackKind.Light);
AdvanceTo(tapGuard, tapGuard.Opponent.CurrentAttack!.ResolveAt);
Near(tapGuard.Player.DamageTaken, 12, "an early tap doesn't leave a block active");

var guardQueue = NewCounterTrial();
guardQueue.SetParryHeld(guardQueue.Player, true);
Check(guardQueue.QueueAttack(guardQueue.Player, AttackKind.Light), "attack can be bought while holding guard");
guardQueue.Tick(0.10);
Check(guardQueue.Player.CurrentAttack is null, "guard holds the attack in the queue");
guardQueue.SetParryHeld(guardQueue.Player, false);
Check(guardQueue.Player.CurrentAttack is not null, "dropping guard releases the queued swing");

foreach (var canPay in new[] { false, true })
{
    var duel = NewCounterTrial();
    duel.Player.Stamina = canPay ? 15 : 14;
    duel.SetParryHeld(duel.Player, true);
    duel.CounterTrial!.Resolve(false, AttackOutcome.Parried, duel.Time);
    duel.QueueAttack(duel.Opponent, AttackKind.Unparryable);
    var heavy = duel.Opponent.CurrentAttack!;
    AdvanceTo(duel, heavy.ResolveAt);
    Check(heavy.Outcome == (canPay ? AttackOutcome.Blocked : AttackOutcome.CleanHit),
        "a held guard only stops the Heavy if it can afford 15 stamina");
    Near(duel.Player.Stamina, canPay ? 0 : 14, "Heavy guard only charges on success");
}

var sparring = new CombatEngine(new CombatTuning { AiDefenseEnabled = false }, new CounterTrial(), trackHealth: false);
for (var frame = 0; frame < 600; frame++) sparring.Tick(CombatEngine.FixedStep);
var aiLaunchTimes = sparring.AttackHistory.Select(attack => attack.LaunchAt).ToArray();
var intervals = aiLaunchTimes.Zip(aiLaunchTimes.Skip(1), (a, b) => Math.Round(b - a, 2)).ToArray();
Check(intervals.Distinct().Count() >= 4 && intervals.Min() < sparring.Tuning.AiCadence &&
    intervals.Max() > sparring.Tuning.AiCadence, "sparring bot mixes faster follow-ups and slower openings");
var reads = Enumerable.Range(0, 48).Select(_ =>
    sparring.CounterTrial!.ReadDefense(guardedLight, false, sparring.Tuning.AiReactionDelay)).ToArray();
Check(reads.Any(read => read.Kind == DefenseKind.Parry) && reads.Any(read => read.Kind == DefenseKind.Dodge) &&
    reads.Any(read => read.Kind is null) &&
    reads.Any(read => read.Kind is not null && Math.Abs(read.At - guardedLight.ImpactAt) > 0.06),
    "bot doesn't force a perfectly timed counter on every read");

var liveEconomy = new CounterTrial { Economy = new CounterEconomySettings(100, 6, 12, 15, 10) };
var economyTrial = new CombatEngine(new CombatTuning { AiAttackEnabled = false, AiDefenseEnabled = false },
    liveEconomy, trackHealth: false);
Near(economyTrial.Player.Stamina, 100, "configured reserve applies at the start");
Near(economyTrial.DescribeAttack(AttackKind.Unparryable).StaminaCost, 12, "both counters use the configured Heavy cost");
economyTrial.SetParryHeld(economyTrial.Player, true);
economyTrial.QueueAttack(economyTrial.Player, AttackKind.Light);
liveEconomy.Economy = liveEconomy.Economy!.Value with { StartingStamina = 75, LightCost = 4, Regeneration = 20, DefenseReward = 12 };
economyTrial.Tick(0.10);
economyTrial.SetParryHeld(economyTrial.Player, false);
Near(economyTrial.Player.Stamina, 94, "an already reserved attack keeps the original cost");
economyTrial.Tick(0.05);
Near(economyTrial.Player.Stamina, 95, "the updated regen rate applies immediately");
economyTrial.QueueAttack(economyTrial.Player, AttackKind.Light);
Near(economyTrial.Player.ReservedStamina, 4, "new attacks reserve the updated cost");
economyTrial.QueueAttack(economyTrial.Opponent, AttackKind.Light);
var economyIncoming = economyTrial.Opponent.CurrentAttack!;
AdvanceTo(economyTrial, economyIncoming.ImpactAt);
economyTrial.Player.Stamina = 60;
economyTrial.ActivateDefense(economyTrial.Player, DefenseKind.Parry);
AdvanceTo(economyTrial, economyIncoming.ResolveAt);
Check(economyTrial.Resolutions.Last().Outcome == AttackOutcome.Parried, "the parry still resolves normally under the live economy");
Near(economyTrial.Resolutions.Last().DefenderStaminaRequested, 12, "the next successful defense uses the live reward");

var lightChain = new CombatEngine(new CombatTuning { AiAttackEnabled = false, AiDefenseEnabled = false },
    new CounterTrial { PlayerLightStartup = 0.200, Economy = new CounterEconomySettings(0, 0, 0, 0, 0) });
for (var tap = 0; tap < 4; tap++)
    Check(lightChain.QueueTrialAttack(lightChain.Player, inputTimeSeconds: 0.06 * tap),
        "rapid Light taps queue without stamina");
Check(lightChain.Player.BufferedAttackCount == 3, "the first Light launches and the remaining three buffer");
var firstFastLight = lightChain.Player.CurrentAttack!;
Near(firstFastLight.Info.Startup, 0.200, "the live override shortens the player's windup");
Check(lightChain.QueueAttack(lightChain.Opponent, AttackKind.Light), "the bot can still swing");
Near(lightChain.Opponent.CurrentAttack!.Info.Startup, 0.320, "the bot keeps its original windup");
for (var frame = 0; frame < 90; frame++) lightChain.Tick(CombatEngine.FixedStep);
var fastLights = lightChain.AttackHistory.Where(a => a.Attacker.IsPlayer).ToArray();
Check(fastLights.Length == 4 && fastLights.All(a => a.Info.Kind == AttackKind.Light),
    "all four rapid taps launch as Lights");
Check(fastLights[1].LaunchAt - firstFastLight.ResolveAt is >= -1e-9 and <= CombatEngine.FixedStep + 1e-9,
    "the second Light starts on the first frame after resolution");
Near(lightChain.Player.Stamina, 0, "the complete chain is free");

var freeQueue = NewCounterTrial();
freeQueue.CounterTrial!.Economy = new CounterEconomySettings(100, 0, 0, 0, 0);
freeQueue.Tuning.AttackInputInterval = 0.010;
freeQueue.QueueAttack(freeQueue.Opponent, AttackKind.Light);
var freeThreat = freeQueue.Opponent.CurrentAttack!;
AdvanceTo(freeQueue, freeThreat.ImpactAt - 0.05);
freeQueue.ActivateDefense(freeQueue.Player, DefenseKind.Parry);
for (var tap = 0; tap < 64; tap++)
    Check(freeQueue.QueueTrialAttack(freeQueue.Player), "every free tap buffers even at the same combat timestamp");
Check(freeQueue.Player.BufferedAttackCount == 63 && freeQueue.CounterTrial.HasBufferedCounterInput(true),
    "the first tap waits for defense confirmation; the rest are all Lights");
AdvanceTo(freeQueue, freeThreat.ResolveAt);
Check(freeQueue.Player.CurrentAttack?.Info.Kind == AttackKind.Unparryable,
    "the confirmed counter still launches before the later taps");
for (var frame = 0; frame < 1800; frame++) freeQueue.Tick(CombatEngine.FixedStep);
Check(freeQueue.AttackHistory.Count(a => a.Attacker.IsPlayer) == 64 && freeQueue.Player.BufferedAttackCount == 0,
    "all attacks in the unlimited backlog eventually launch");
Check(freeQueue.AttackQueueLimit(freeQueue.Opponent) == CombatEngine.MaxBufferLength,
    "free practice does not remove the opponent's limit");

foreach (var interval in new double?[] { null, 0.040 })
{
    var burst = NewCounterTrial();
    burst.CounterTrial!.Economy = new CounterEconomySettings(100, 0, 0, 0, 0);
    burst.CounterTrial.BurstLights = true;
    burst.CounterTrial.LightLaunchInterval = interval;
    burst.CounterTrial.PlayerLightStartup = 0.200;
    var tapCount = interval is null ? 8 : 5;
    for (var tap = 0; tap < tapCount; tap++)
    {
        Check(burst.QueueTrialAttack(burst.Player), "player Light enters the burst");
        Check(burst.QueueAttack(burst.Opponent, AttackKind.Light), "opponent Light enters the burst");
    }
    for (var frame = 0; frame < 10; frame++) burst.Tick(CombatEngine.FixedStep);
    Check(burst.Player.ActiveAttacks.Count == tapCount && burst.Opponent.ActiveAttacks.Count == tapCount && burst.Resolutions.Count == 0,
        "both streams launch every queued Light before the first swing reaches impact");
    foreach (var fighter in new[] { burst.Player, burst.Opponent })
        for (var index = 1; index < fighter.ActiveAttacks.Count; index++)
            Near(fighter.ActiveAttacks[index].LaunchAt - fighter.ActiveAttacks[index - 1].LaunchAt, interval ?? 0.020,
                "overlapping Lights keep the exact configured spacing at a 60 Hz simulation step");
    Check(burst.ActivateDefense(burst.Opponent, DefenseKind.Parry), "one parry can challenge the incoming stream");
    for (var frame = 0; frame < 30; frame++) burst.Tick(CombatEngine.FixedStep);
    Check(burst.Resolutions.Count == 2 * tapCount && burst.AttackHistory.All(attack => attack.StrikeAtImpact is not null),
        "every overlapping strike samples spacing and resolves exactly once");
    Check(burst.Resolutions.Count(r => r.AttackerIsPlayer && r.Outcome == AttackOutcome.Parried) == 1,
        "one parry consumes only one Light in the burst");
}

var matchedTrial = NewCounterTrial();
var matchRules = matchedTrial.CounterTrial!;
matchRules.SharedAttackWindup = CounterTrial.MatchedWindup;
matchRules.PlayerLightStartup = 0.16;
matchRules.OpponentLightStartup = 0.12;
foreach (var kind in new[] { AttackKind.Light, AttackKind.Undodgeable, AttackKind.Unparryable })
    Near(matchedTrial.DescribeAttack(kind).Startup, 0.280, "shared windup overrides both Lights and earned specials");
matchedTrial.QueueAttack(matchedTrial.Opponent, AttackKind.Light);
Near(matchedTrial.Opponent.CurrentAttack!.Info.Startup, 0.280, "the opponent has the same windup as the player");

matchRules.Mixups = new CounterMixupSettings(1, 0.12, 0, 0.60);
Check(Enumerable.Range(0, 32).Select(_ => matchRules.NextAttackDelay(0.45)).All(gap => gap is > 0.10 and < 0.14),
    "a guaranteed combo uses a tight but variable follow-up gap");
matchRules.Resolve(false, AttackOutcome.Parried, 0);
Check(matchRules.ChooseOpponentCounter(0) is null && matchRules.ChooseOpponentCounter(0.10) is null,
    "a counter branch is decided once, not rerolled every frame");
matchRules.Mixups = matchRules.Mixups.Value with { CounterUseChance = 1 };
matchRules.Resolve(false, AttackOutcome.Dodged, 0.11);
Check(matchRules.ChooseOpponentCounter(0.11) == AttackKind.Undodgeable,
    "a fresh dodge token can take the special branch");

int ReadCount(double gap)
{
    var defender = new CounterTrial { Mixups = new CounterMixupSettings(0.5, 0.12, 0.85, 0.50) };
    var info = matchedTrial.DescribeAttack(AttackKind.Light);
    return Enumerable.Range(0, 400).Count(i => defender.ReadDefense(
        new AttackState(i, matchedTrial.Player, matchedTrial.Opponent, info, i * gap, i * gap + 0.28, i * gap + 0.34),
        false, 0.18).Kind is not null);
}
Check(ReadCount(0.04) + 30 < ReadCount(0.30), "tight follow-ups overload the simulated defender's reads");

var cappedTrial = NewCounterTrial();
var capRules = cappedTrial.CounterTrial!;
capRules.Economy = new CounterEconomySettings(100, 0, 0, 0, 0);
capRules.BurstLights = true;
capRules.SharedAttackWindup = CounterTrial.MatchedWindup;
capRules.LightLaunchInterval = 0.04;
capRules.QueueLimit = 4;
capRules.SpecialCounterRecovery = 0.12;
capRules.Resolve(true, AttackOutcome.Parried, 0);
Check(cappedTrial.QueueTrialAttack(cappedTrial.Player), "the earned special launches");
var cappedSpecial = cappedTrial.Player.CurrentAttack!;
for (var attack = 0; attack < 4; attack++)
    Check(cappedTrial.QueueTrialAttack(cappedTrial.Player), "four Lights can queue behind the special");
Check(cappedTrial.WaitingAttackCount(cappedTrial.Player) == 4 &&
    !cappedTrial.QueueTrialAttack(cappedTrial.Player) && cappedTrial.AttackQueueLimit(cappedTrial.Opponent) == 4,
    "the four-attack cap excludes the in-flight swing and applies to both fighters");
AdvanceTo(cappedTrial, cappedSpecial.ImpactAt);
cappedTrial.ActivateDefense(cappedTrial.Opponent, DefenseKind.Dodge);
AdvanceTo(cappedTrial, cappedSpecial.ResolveAt);
Check(cappedSpecial.Outcome == AttackOutcome.Dodged, "the special was defended");
var recoveredAt = cappedSpecial.ImpactAt + 0.12;
Near(cappedTrial.Player.AttackReadyAt, recoveredAt, "defending a special incurs 120 ms of recovery");
AdvanceTo(cappedTrial, recoveredAt - 0.001);
Check(cappedTrial.Player.CurrentAttack is null, "waiting Lights do not skip the special recovery");
AdvanceTo(cappedTrial, recoveredAt);
Near(cappedTrial.Player.CurrentAttack!.LaunchAt, recoveredAt, "the first queued Light launches when recovery ends");

var reservedTrial = NewCounterTrial();
reservedTrial.CounterTrial!.Economy = capRules.Economy;
reservedTrial.CounterTrial.QueueLimit = 4;
reservedTrial.QueueAttack(reservedTrial.Opponent, AttackKind.Light);
var reserveThreat = reservedTrial.Opponent.CurrentAttack!;
AdvanceTo(reservedTrial, reserveThreat.ImpactAt - 0.05);
reservedTrial.ActivateDefense(reservedTrial.Player, DefenseKind.Parry);
for (var attack = 0; attack < 4; attack++)
    Check(reservedTrial.QueueTrialAttack(reservedTrial.Player), "the early special and three Lights fit");
Check(reservedTrial.WaitingAttackCount(reservedTrial.Player) == 4 &&
    !reservedTrial.QueueTrialAttack(reservedTrial.Player), "an early special reserves one of the four waiting slots");
AdvanceTo(reservedTrial, reserveThreat.ResolveAt);
Check(reservedTrial.Player.CurrentAttack?.Info.Kind == AttackKind.Unparryable &&
    reservedTrial.WaitingAttackCount(reservedTrial.Player) == 3,
    "the reserved counter launches first without losing the remaining Lights");

var planner = new CombatEngine(new CombatTuning { AiDefenseEnabled = false }, new CounterTrial
{
    Economy = new CounterEconomySettings(100, 0, 0, 0, 0), BurstLights = true,
    SharedAttackWindup = 0.28, OpponentLookahead = 0.12,
    OpponentSettings = new CounterOpponentSettings(0.28, 0, 0.18, 0.85, 0.06)
});
AdvanceTo(planner, 0.32);
Check(planner.WaitingAttackCount(planner.Opponent) == 0, "the bot does not queue before its planning window");
AdvanceTo(planner, 0.34);
Check(planner.WaitingAttackCount(planner.Opponent) == 1 && planner.GetIncomingThreat(planner.Player) is null,
    "a committed bot Light is queued before it becomes an incoming swing");
AdvanceTo(planner, 0.45);
Near(planner.Opponent.CurrentAttack!.LaunchAt, 0.45, "planning does not bring forward the first launch");
AdvanceTo(planner, 0.60);
Check(planner.WaitingAttackCount(planner.Opponent) == 1 && planner.Opponent.ActiveAttacks.Count == 1,
    "the next planned Light waits on the queue while the previous one is in flight");
AdvanceTo(planner, 0.716);
Check(planner.Opponent.ActiveAttacks.Count == 2 && planner.WaitingAttackCount(planner.Opponent) == 0,
    "the queued follow-up enters flight at the original cadence");
Near(planner.Opponent.ActiveAttacks[1].LaunchAt, 0.716, "planning preserves the follow-up's scheduled launch");

foreach (var (earnedBy, response, expected, playerAttacks) in new (AttackOutcome, DefenseKind?, AttackOutcome, bool)[]
{
    (AttackOutcome.Parried, DefenseKind.Dodge, AttackOutcome.Dodged, true),
    (AttackOutcome.Dodged, DefenseKind.Parry, AttackOutcome.Parried, false),
    (AttackOutcome.Parried, DefenseKind.Block, AttackOutcome.Blocked, true),
    (AttackOutcome.Parried, null, AttackOutcome.CleanHit, true)
})
{
    var duel = NewCounterTrial();
    var trial = duel.CounterTrial!;
    trial.ClearQueueOnSpecialDefense = true;
    trial.SharedAttackWindup = 0.28;
    var attacker = playerAttacks ? duel.Player : duel.Opponent;
    var defender = playerAttacks ? duel.Opponent : duel.Player;
    trial.Resolve(playerAttacks, earnedBy, duel.Time);
    Check(duel.QueueAttack(attacker, earnedBy == AttackOutcome.Parried ? AttackKind.Unparryable : AttackKind.Undodgeable),
        "the special is committed before setting up the waiting attacks");
    var special = attacker.CurrentAttack!;
    Check(duel.QueueAttack(attacker, AttackKind.Light) && duel.QueueAttack(attacker, AttackKind.Light),
        "the attacker has two waiting Lights");
    Near(attacker.ReservedStamina, 20, "waiting Lights reserve stamina before a possible clear");
    AdvanceTo(duel, 0.10);
    Check(duel.QueueAttack(defender, AttackKind.Light) && duel.QueueAttack(defender, AttackKind.Light),
        "the defender has a Light in flight and one waiting");
    var returningLight = defender.CurrentAttack!;
    if (response == DefenseKind.Block) duel.SetParryHeld(defender, true);
    AdvanceTo(duel, special.ImpactAt);
    if (response is DefenseKind.Dodge or DefenseKind.Parry) duel.ActivateDefense(defender, response.Value);
    if (response == DefenseKind.Dodge)
    {
        AdvanceTo(duel, special.ImpactAt + 0.041);
        duel.ActivateDefense(attacker, DefenseKind.Parry);
        Check(duel.QueueTrialAttack(attacker) && trial.HasBufferedCounterInput(playerAttacks),
            "an unresolved early follow-up is also waiting when the special is dodged");
    }
    AdvanceTo(duel, special.ResolveAt);
    Check(special.Outcome == expected, "the expected special result resolves");
    Check(defender.CurrentAttack == returningLight && defender.BufferedAttackCount == 1,
        "the defender's waiting and in-flight attacks are untouched");
    var cleared = expected is AttackOutcome.Parried or AttackOutcome.Dodged;
    if (cleared)
    {
        Check(attacker.BufferedAttackCount == 0 && !trial.HasBufferedCounterInput(playerAttacks),
            "dodging or parrying a special clears only the attacker's waiting attacks");
        Near(attacker.ReservedStamina, 0, "the discarded attacks no longer reserve stamina");
    }
    else
        Check(attacker.BufferedAttackCount + attacker.ActiveAttacks.Count == 2,
            "a clean hit or block does not discard the next two attacks");
    Check(duel.Events.Count(item => item.Kind == CombatEventKind.AttackQueueCleared) == (cleared ? 1 : 0),
        "the Exchange log records one clear only when attacks are discarded");
}

foreach (var outcome in new[] { AttackOutcome.CleanHit, AttackOutcome.Parried, AttackOutcome.Dodged, AttackOutcome.Blocked, AttackOutcome.Whiffed })
{
    var duel = NewCounterTrial();
    var trial = duel.CounterTrial!;
    trial.Economy = new CounterEconomySettings(100, 0, 0, 0, 0);
    trial.DisableSpecialRecovery = true;
    trial.SharedAttackWindup = 0.28;
    trial.Resolve(true, outcome == AttackOutcome.Parried ? AttackOutcome.Dodged : AttackOutcome.Parried, duel.Time);
    Check(duel.QueueTrialAttack(duel.Player), "the special launches in the no-recovery test");
    var special = duel.Player.CurrentAttack!;
    Check(duel.QueueTrialAttack(duel.Player), "a Light waits for the special result");
    if (outcome == AttackOutcome.Blocked) duel.SetParryHeld(duel.Opponent, true);
    if (outcome == AttackOutcome.Whiffed) trial.SetMovement(-1);
    AdvanceTo(duel, special.ImpactAt);
    Check(duel.Player.CurrentAttack == special, "the special still waits through the defense window");
    if (outcome == AttackOutcome.Parried) duel.ActivateDefense(duel.Opponent, DefenseKind.Parry);
    if (outcome == AttackOutcome.Dodged) duel.ActivateDefense(duel.Opponent, DefenseKind.Dodge);
    AdvanceTo(duel, special.ResolveAt);
    var resolution = duel.Resolutions.Single();
    Check(resolution.Outcome == outcome, "the special resolves as expected");
    Near(resolution.AttackerReadyAt, special.ImpactAt, "no special attack recovery is added to the attacker");
    Near(resolution.DefenderReadyAt, outcome == AttackOutcome.Whiffed ? 0 : special.ImpactAt,
        "no special attack recovery is added to the defender");
    Near(duel.Player.CurrentAttack!.LaunchAt, special.ResolveAt, "the follow-up Light launches at confirmation, not before or after");
}

foreach (var playerAttacks in new[] { true, false })
{
    var duel = NewCounterTrial();
    var rules = duel.CounterTrial!;
    rules.Economy = new CounterEconomySettings(100, 0, 0, 0, 0);
    rules.DisableSpecialRecovery = rules.BurstLights = true;
    rules.SharedAttackWindup = 0.28;
    rules.LightLaunchInterval = 0.04;
    rules.QueueLimit = 4;
    var attacker = playerAttacks ? duel.Player : duel.Opponent;
    var kinds = new[] { AttackKind.Light, AttackKind.Unparryable, AttackKind.Undodgeable, AttackKind.Light, AttackKind.Light };
    foreach (var kind in kinds)
    {
        if (kind != AttackKind.Light)
            rules.Resolve(playerAttacks, kind == AttackKind.Undodgeable ? AttackOutcome.Dodged : AttackOutcome.Parried, duel.Time);
        Check(duel.QueueAttack(attacker, kind), "each earned special or Light fits in the four waiting slots");
    }
    Check(!duel.QueueAttack(attacker, AttackKind.Light), "overlap still respects the queue cap");
    for (var frame = 0; frame < 10; frame++) duel.Tick(CombatEngine.FixedStep);
    Check(attacker.ActiveAttacks.Select(a => a.Info.Kind).SequenceEqual(kinds) && duel.Resolutions.Count == 0,
        "both fighters can launch the full mixed burst in order before the first impact");
    for (var i = 0; i < kinds.Length; i++)
    {
        Near(attacker.ActiveAttacks[i].LaunchAt, i * 0.04, "specials share the same launch clock as Lights");
        Near(attacker.ActiveAttacks[i].ImpactAt, i * 0.04 + 0.28, "overlap keeps the full warning for each swing");
    }
    AdvanceTo(duel, 0.55);
    Check(duel.Resolutions.Select(r => r.AttackKind).SequenceEqual(kinds), "each overlapping attack resolves once");
}

var burstBot = NewCounterTrial();
var botRules = burstBot.CounterTrial!;
botRules.Economy = new CounterEconomySettings(100, 0, 0, 0, 0);
botRules.DisableSpecialRecovery = botRules.BurstLights = true;
botRules.SharedAttackWindup = 0.28;
botRules.LightLaunchInterval = 0.04;
botRules.Mixups = new CounterMixupSettings(1, 0.12, 1, 0.5);
burstBot.Tuning.AiAttackEnabled = true;
AdvanceTo(burstBot, 0.45);
botRules.Resolve(false, AttackOutcome.Dodged, burstBot.Time);
for (var frame = 0; frame < 14; frame++) burstBot.Tick(CombatEngine.FixedStep);
Check(burstBot.Opponent.ActiveAttacks.Select(a => a.Info.Kind).Take(3).SequenceEqual(
    new[] { AttackKind.Light, AttackKind.Undodgeable, AttackKind.Light }),
    "the autonomous bot schedules through an in-flight special without waiting for impact");

foreach (var (outcome, playerAttacks) in new[]
{
    (AttackOutcome.CleanHit, true), (AttackOutcome.Blocked, true), (AttackOutcome.Parried, false),
    (AttackOutcome.Dodged, false), (AttackOutcome.Whiffed, true)
})
{
    var duel = NewCounterTrial();
    var rules = duel.CounterTrial!;
    rules.Economy = new CounterEconomySettings(100, 0, 0, 0, 0);
    rules.BurstLights = true;
    rules.SharedAttackWindup = 0.28;
    rules.SpecialTiming = new CounterSpecialTiming(0.32, 0.08, 0.12);
    rules.LightLaunchInterval = 0.04;
    rules.QueueLimit = 4;
    var attacker = playerAttacks ? duel.Player : duel.Opponent;
    var defender = playerAttacks ? duel.Opponent : duel.Player;
    duel.QueueAttack(attacker, AttackKind.Light);
    var parry = outcome == AttackOutcome.Parried;
    rules.Resolve(playerAttacks, parry ? AttackOutcome.Dodged : AttackOutcome.Parried, duel.Time);
    Check(duel.QueueAttack(attacker, parry ? AttackKind.Undodgeable : AttackKind.Unparryable) &&
        duel.QueueAttack(attacker, AttackKind.Light), "the special and its follow-up both fit in the queue");
    AdvanceTo(duel, 0.04);
    var light = attacker.ActiveAttacks[0];
    var special = attacker.ActiveAttacks[1];
    Near(light.ImpactAt, 0.28, "the special windup override leaves Light windup alone");
    Near(special.LaunchAt, 0.04, "the special launches behind an in-flight Light");
    Near(special.ImpactAt, 0.36, "the special gets a 320 ms windup");
    AdvanceTo(duel, 0.08);
    Check(attacker.BufferedAttackCount == 1 && attacker.CurrentAttack == light,
        "a special locks later launches even when an earlier Light is the current attack");
    if (outcome == AttackOutcome.Blocked) duel.SetParryHeld(defender, true);
    if (outcome == AttackOutcome.Whiffed) rules.SetMovement(-1);
    AdvanceTo(duel, special.ImpactAt);
    if (parry) duel.ActivateDefense(defender, DefenseKind.Parry);
    if (outcome == AttackOutcome.Dodged) duel.ActivateDefense(defender, DefenseKind.Dodge);
    AdvanceTo(duel, special.ResolveAt);
    Check(special.Outcome == outcome && attacker.BufferedAttackCount == 1,
        "the special resolves as expected without discarding the waiting Light");
    var recovery = outcome is AttackOutcome.CleanHit or AttackOutcome.Blocked ? 0.08 : 0.12;
    Near(duel.GetAttackLaunchReadyAt(attacker), special.ImpactAt + recovery,
        "special recovery is counted from impact, not added after the defense grace period");
    Near(duel.SpecialDefenderRecovery, 0, "the defender has no special-induced recovery");
    AdvanceTo(duel, special.ImpactAt + recovery - 0.001);
    Check(attacker.CurrentAttack is null, "the next Light stays queued until recovery expires");
    AdvanceTo(duel, special.ImpactAt + recovery);
    Near(attacker.CurrentAttack!.LaunchAt, special.ImpactAt + recovery, "the queued Light launches as recovery ends");
}

foreach (var playerAttacks in new[] { true, false })
{
    var duel = NewCounterTrial();
    var rules = duel.CounterTrial!;
    rules.Economy = new CounterEconomySettings(100, 0, 0, 0, 0);
    rules.BurstLights = true;
    rules.SharedAttackWindup = 0.28;
    rules.SpecialTiming = new CounterSpecialTiming(0.32, 0.08, 0.12);
    rules.LightLaunchInterval = 0.108;
    rules.SpecialLaunchInterval = 0.216;
    var attacker = playerAttacks ? duel.Player : duel.Opponent;
    Check(duel.QueueAttack(attacker, AttackKind.Light) && duel.QueueAttack(attacker, AttackKind.Light),
        "both Lights are accepted immediately");
    rules.Resolve(playerAttacks, AttackOutcome.Parried, duel.Time);
    Check(duel.QueueAttack(attacker, AttackKind.Unparryable) && duel.QueueAttack(attacker, AttackKind.Light),
        "the special and a Light wait behind the Light pair");
    AdvanceTo(duel, 0.107);
    Check(duel.AttackHistory.Count == 1, "the second Light waits for the 108 ms gap");
    AdvanceTo(duel, 0.108);
    Near(duel.AttackHistory[1].LaunchAt, 0.108, "the Light launches exactly 108 ms after the previous swing");
    AdvanceTo(duel, 0.323);
    Check(duel.AttackHistory.Count == 2, "the special waits for its own 216 ms gap");
    AdvanceTo(duel, 0.324);
    Near(duel.AttackHistory[2].LaunchAt, 0.324, "the special launches exactly 216 ms after the previous swing");
    Check(!duel.AttackHistory[1].Resolved, "the special can overlap a Light that is still falling");
    AdvanceTo(duel, 0.723);
    Check(duel.AttackHistory.Count == 3, "the Light gap cannot bypass the special's recovery");
    AdvanceTo(duel, 0.724);
    Near(duel.AttackHistory[3].LaunchAt, 0.724, "the next Light launches as special recovery ends");
}

Console.WriteLine("Combat model checks passed.");
