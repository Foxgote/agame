using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CombatPrototype.Core;

namespace CombatPrototype.Simulation;

public sealed record SimulationOptions(
    int RequestedMatches = 100_000,
    ulong Seed = 20250308,
    double MatchLimitSeconds = 60.0,
    double StalemateSeconds = 12.0,
    double StepSeconds = CombatEngine.FixedStep,
    string OutputDirectory = "simulation-reports")
{
    public BalancedAgentParameters? BalancedParameters { get; init; }
    public BalancedAgentParameters? PreviousBalancedParameters { get; init; }
    public double ImpactEstimateUncertaintyMs { get; init; }
    public string? Profile { get; init; }
    public string[]? Profiles { get; init; }
    public int OpeningPairs { get; init; }
    public double ActionFailureProbability { get; init; }
}

public sealed class SimulationReport
{
    public string Engine { get; init; } = "CombatPrototype.Core";
    public ulong Seed { get; init; }
    public double ImpactEstimateUncertaintyMs { get; init; }
    public double ActionFailureProbability { get; init; }
    public int RequestedMatches { get; init; }
    public int ActualMatches { get; set; }
    public int MatchesPerPairAndProfile { get; set; }
    public List<ProfileReport> Profiles { get; init; } = [];
    public List<StrategySummary> Strategies { get; init; } = [];
    public List<ExploitExample> ExploitExamples { get; init; } = [];
}

public sealed class ProfileReport
{
    public int AttackFirstWins { get; set; }
    public int CounterFirstWins { get; set; }
    public int OpeningDraws { get; set; }
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public int Matches { get; set; }
    public int Stalemates { get; set; }
    public int Timeouts { get; set; }
    public int Draws { get; set; }
    public double AverageDurationSeconds { get; set; }
    public double AverageInitiativeChanges { get; set; }
    public double AverageSuccessfulRetaliations { get; set; }
    public double AverageUnavoidableThreats { get; set; }
    public double AverageUnavoidableHitStreak { get; set; }
    public double AverageInputsQueuedWhileDefending { get; set; }
    public double AverageQueueRejects { get; set; }
    public double AveragePredictedDefenseActions { get; set; }
    public double AverageIncorrectDefenseChoices { get; set; }
    public long ActionAttempts { get; set; }
    public long FailedActions { get; set; }
    public double ActionFailureRate => ActionAttempts == 0 ? 0.0 : FailedActions / (double)ActionAttempts;
    public List<MatchupReport> Matchups { get; init; } = [];
}

public sealed class MatchupReport
{
    public string FirstStrategy { get; init; } = "";
    public string SecondStrategy { get; init; } = "";
    public int Matches { get; set; }
    public int FirstWins { get; set; }
    public int SecondWins { get; set; }
    public int Draws { get; set; }
    public int Stalemates { get; set; }
    public double AverageDurationSeconds { get; set; }
    public double AverageFirstHealth { get; set; }
    public double AverageSecondHealth { get; set; }
    public double AverageFirstStamina { get; set; }
    public double AverageSecondStamina { get; set; }
    public double AverageFirstStaminaGained { get; set; }
    public double AverageSecondStaminaGained { get; set; }
    public double AverageFirstStaminaSpent { get; set; }
    public double AverageSecondStaminaSpent { get; set; }
    public double AverageFirstStaminaWastedAtCap { get; set; }
    public double AverageSecondStaminaWastedAtCap { get; set; }
    public double AverageInitiativeChanges { get; set; }
    public double AverageSuccessfulRetaliations { get; set; }
    public double AverageUnavoidableThreats { get; set; }
    public double AverageUnavoidableHits { get; set; }
    public int LongestConsecutiveUnavoidableHits { get; set; }
    public double AverageRepeatedAttackSequences { get; set; }
    public double AverageHeavyStartupBeatenByLight { get; set; }
    public double AverageBufferedInputs { get; set; }
    public double AverageInputsQueuedWhileDefending { get; set; }
    public double AverageQueueRejects { get; set; }
    public double AveragePredictedDefenseActions { get; set; }
    public double AverageIncorrectDefenseChoices { get; set; }
    public long ActionAttempts { get; set; }
    public long FailedActions { get; set; }
    public double ActionFailureRate => ActionAttempts == 0 ? 0.0 : FailedActions / (double)ActionAttempts;
    public int MaximumObservedQueueDepth { get; set; }
    public Dictionary<string, long> AttackCounts { get; init; } = new(StringComparer.Ordinal);
    public Dictionary<string, long> SuccessfulHitsByAttack { get; init; } = new(StringComparer.Ordinal);
    public Dictionary<string, long> OutcomeCounts { get; init; } = new(StringComparer.Ordinal);
    public Dictionary<string, long> DefenseAttempts { get; init; } = new(StringComparer.Ordinal);
    public Dictionary<string, long> DefenseSuccesses { get; init; } = new(StringComparer.Ordinal);
    public Dictionary<string, long> ActionAttemptsByType { get; init; } = new(StringComparer.Ordinal);
    public Dictionary<string, long> ActionFailuresByType { get; init; } = new(StringComparer.Ordinal);
    public Dictionary<string, double> DefenseSuccessRates { get; init; } = new(StringComparer.Ordinal);
    public Dictionary<string, long> DefenseTimingOffsetMs { get; init; } = new(StringComparer.Ordinal);
    public Dictionary<string, long> FrameAdvantageMs { get; init; } = new(StringComparer.Ordinal);
    public double GetAttackHitRate()
    {
        var attacks = AttackCounts.Values.Sum();
        return attacks == 0 ? 0.0 :
            OutcomeCounts.GetValueOrDefault(nameof(AttackOutcome.CleanHit)) / (double)attacks;
    }
}

public sealed class StrategySummary
{
    public string Strategy { get; init; } = "";
    public int Matches { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
    public double WinRate => Matches == 0 ? 0.0 : Wins / (double)Matches;
    public double AverageDamage { get; set; }
    public double AverageStaminaSpent { get; set; }
    public double AverageStaminaWastedAtCap { get; set; }
    public long AttacksLaunched { get; set; }
    public long AttacksHit { get; set; }
    public long HeavyStartupBeatenByLight { get; set; }
    public long RepeatedAttackSequences { get; set; }
    public long DefenseAttempts { get; set; }
    public long SuccessfulDefenses { get; set; }
    public long BufferedInputs { get; set; }
    public long InputsQueuedWhileDefending { get; set; }
    public long QueueRejects { get; set; }
    public int MaximumObservedQueueDepth { get; set; }
    public bool Dominant => Matches >= 100 && WinRate >= 0.65;
}

public sealed class ExploitExample
{
    public string Profile { get; init; } = "";
    public string FirstStrategy { get; init; } = "";
    public string SecondStrategy { get; init; } = "";
    public ulong MatchSeed { get; init; }
    public string Issue { get; init; } = "";
    public double DurationSeconds { get; init; }
    public List<string> EventLog { get; init; } = [];
}

internal sealed record ScenarioProfile(string Name, string Description, Action<CombatTuning> Configure, string? SeedGroup = null)
{
    // Explicit opt-in: don't inflate routine all-profile sweeps with calibration arms.
    public static readonly ScenarioProfile[] ReadStudy =
    [
        ReadProfile(100, 20),
        .. from read in new[] { 60, 75, 90 } from hit in new[] { 0, 5, 10 } select ReadProfile(read, hit),
        .. from regen in new[] { 15, 20 } from hit in new[] { 0, 2, 5 } select ReadProfile(75, hit, regen)
    ];

    private static ScenarioProfile ReadProfile(int read, int hit, int regen = 10) =>
        new($"Read{read}Hit{hit}" + (regen == 10 ? "" : $"Regen{regen}"),
            $"AI parry read {read}%; normal hit taken +{hit}; regen {regen}/s.",
            t =>
            {
                t.AiParryReadChance = read / 100.0;
                t.LightDefenderCleanHitGain = t.HeavyDefenderCleanHitGain = hit;
                t.StaminaRegenerationPerSecond = regen;
            }, "Baseline");

    private static void ConfigureFreeNormalAttacks(CombatTuning tuning)
    {
        tuning.LightCost = 0.0;
        tuning.HeavyCost = 0.0;
        tuning.StaminaRegenerationPerSecond = 0.0;
        tuning.LightDefenderCleanHitGain = tuning.HeavyDefenderCleanHitGain = 20.0;
        tuning.LightAttackerCleanHitGain = 10.0;
        tuning.LightAttackerBlockedGain = 20.0;
        tuning.LightAttackerDodgedGain = 10.0;
        tuning.HeavyAttackerCleanHitGain = 10.0;
        tuning.HeavyAttackerBlockedGain = 20.0;
        tuning.HeavyAttackerDodgedGain = 10.0;
    }

    private static void ConfigureHybridTiming(CombatTuning tuning)
    {
        tuning.ParryWindow = 0.180;
        tuning.UseExpandedSpecials = true;
        tuning.LightSpecialStartup = 0.500;
        tuning.LightSpecialCost = 20.0;
        tuning.LightUnblockableDamage = 16.0;
        tuning.LightUndodgeableDamage = 16.0;
        tuning.SpecialStartup = 0.800;
        tuning.SpecialCost = 35.0;
        tuning.UnblockableDamage = 30.0;
        tuning.UndodgeableDamage = 24.0;
        tuning.DodgedDefenderRecovery = 0.175;
    }

    public static readonly ScenarioProfile[] All =
    [
        new("Baseline", "User-specified provisional rules; unmodified defaults.", _ => { }),
        new("EarnedDefense",
            "Historical no-hit-reward experiment: 10/s regen and always read eligible parries.",
            t =>
            {
                t.LightDefenderCleanHitGain = 0.0;
                t.HeavyDefenderCleanHitGain = 0.0;
                t.StaminaRegenerationPerSecond = 10.0;
                t.AiParryReadChance = 1.0;
            }, SeedGroup: "Baseline"),
        new("FreeNormalAttacks",
            "Comparison profile with free Light and Heavy attacks, no passive regeneration, and the former outcome-based attacker refunds.",
            ConfigureFreeNormalAttacks),
        new("FailedDefenseLockout250",
            "Baseline rules plus 250 ms cross-defense recovery after a Parry or Dodge window expires without countering a threat.",
            t => t.FailedDefenseRecovery = 0.250),
        new("DefenseAttemptCost5",
            "Baseline rules plus a 5-stamina cost for each accepted Parry or Dodge activation.",
            t =>
            {
                t.ParryCost = 5.0;
                t.DodgeCost = 5.0;
            }),
        new("DefenseAttemptCost10",
            "Baseline rules plus a 10-stamina cost for each accepted Parry or Dodge activation.",
            t =>
            {
                t.ParryCost = 10.0;
                t.DodgeCost = 10.0;
            }),
        new("DefenseAttemptCost15",
            "Baseline rules plus a 15-stamina cost for each accepted Parry or Dodge activation.",
            t =>
            {
                t.ParryCost = 15.0;
                t.DodgeCost = 15.0;
            }),
        new("LightInterrupt",
            "Baseline rules plus clean normal Lights interrupting a normal Heavy before impact; includes the next Light after a dodge.",
            t => t.LightInterruptsHeavyStartup = true),
        new("HeavyStartupNoDefense",
            "Baseline rules plus disabling Block, Parry, Dodge, Duck, and Hop during normal Heavy and Heavy Special startup.",
            t => t.DisableDefenseDuringHeavyStartup = true),
        new("Telegraph150ms", "Exploratory profile: 150 ms telegraph lead.", t => t.TelegraphLead = 0.150),
        new("DefenseWindow240ms", "Exploratory profile: 240 ms parry and dodge windows.", t =>
        {
            t.ParryWindow = 0.240;
            t.DodgeWindow = 0.240;
        }),
        new("RhythmParryWindow180",
            "Rhythm baseline with the total parry window widened from 120 ms to 180 ms (±90 ms).",
            t => t.ParryWindow = 0.180),
        new("RhythmParryWindow90",
            "Rhythm baseline with the total parry window narrowed from 120 ms to 90 ms (±45 ms).",
            t => t.ParryWindow = 0.090),
        new("RhythmCounterplay90",
            "Rhythm baseline with a 90 ms total parry window and clean Light interrupts against normal Heavy startup.",
            t =>
            {
                t.ParryWindow = 0.090;
                t.LightInterruptsHeavyStartup = true;
            }),
        new("RhythmCounterplay60",
            "Rhythm baseline with a 60 ms total parry window and clean Light interrupts against normal Heavy startup.",
            t =>
            {
                t.ParryWindow = 0.060;
                t.LightInterruptsHeavyStartup = true;
            }),
        new("RhythmParryCost10",
            "Rhythm baseline with a 10-stamina cost for each accepted Parry activation.",
            t => t.ParryCost = 10.0),
        new("RhythmParryRecovery150",
            "Rhythm baseline with 150 ms Parry action recovery after a successful parry.",
            t => t.ParryActionRecovery = 0.150),
        new("SpecialCost40", "Exploratory profile: heavy special attacks cost 40 stamina.", t => t.SpecialCost = 40.0),
        new("HybridTiming",
            "Experimental Sifu/OSU hybrid: 500 ms quick specials (16 damage, 20 stamina), " +
            "800 ms heavy specials (30/24 damage, 35 stamina), and 175 ms defender recovery after dodge.",
            ConfigureHybridTiming),
        new("HybridLightInterrupt",
            "Hybrid timing plus clean Lights cancelling normal Heavy attacks that have not impacted yet.",
            t =>
            {
                ConfigureHybridTiming(t);
                t.LightInterruptsHeavyStartup = true;
            }),
        new("HybridCounterplay",
            "Hybrid timing with Light interrupt on Heavy startup and normal Heavy damage reduced to 12.",
            t =>
            {
                ConfigureHybridTiming(t);
                t.LightInterruptsHeavyStartup = true;
                t.HeavyDamage = 12.0;
            }),
        new("HybridCounterplayFastLight",
            "Counterplay profile plus normal Light startup reduced from 180 ms to 120 ms.",
            t =>
            {
                ConfigureHybridTiming(t);
                t.LightInterruptsHeavyStartup = true;
                t.HeavyDamage = 12.0;
                t.LightStartup = 0.120;
            }),
        new("HybridCounterplayNarrowParry",
            "Counterplay profile plus a 120 ms total parry window (±60 ms).",
            t =>
            {
                ConfigureHybridTiming(t);
                t.LightInterruptsHeavyStartup = true;
                t.HeavyDamage = 12.0;
                t.ParryWindow = 0.120;
            }),
        new("HybridCounterplayNarrowParryFastDodge",
            "Narrow-parry counterplay profile with successful-dodge attack recovery restored to 100 ms.",
            t =>
            {
                ConfigureHybridTiming(t);
                t.LightInterruptsHeavyStartup = true;
                t.HeavyDamage = 12.0;
                t.ParryWindow = 0.120;
                t.DodgedDefenderRecovery = 0.100;
            }),
        new("HybridCounterplayParryCommitment",
            "Counterplay profile plus 600 ms recovery before a fighter may parry again.",
            t =>
            {
                ConfigureHybridTiming(t);
                t.LightInterruptsHeavyStartup = true;
                t.HeavyDamage = 12.0;
                t.ParryActionRecovery = 0.600;
            }),
        new("HybridCounterplayResourceBalance",
            "Counterplay profile with a 120 ms parry window and a 15-stamina normal Heavy cost.",
            t =>
            {
                ConfigureHybridTiming(t);
                t.LightInterruptsHeavyStartup = true;
                t.HeavyDamage = 12.0;
                t.ParryWindow = 0.120;
                t.HeavyCost = 15.0;
            }),
        new("HybridSifuRecovery",
            "Hybrid timing plus 350 ms dodge and 250 ms parry action recovery to test defensive commitment.",
            t =>
            {
                ConfigureHybridTiming(t);
                t.DodgeActionRecovery = 0.350;
                t.ParryActionRecovery = 0.250;
            }),
        new("HybridHeavyCommitment",
            "Hybrid timing with normal Heavy startup increased from 400 ms to 850 ms.",
            t =>
            {
                ConfigureHybridTiming(t);
                t.HeavyStartup = 0.850;
            }),
        new("HybridHeavyDamage16",
            "Hybrid timing with normal Heavy damage reduced from 24 to 16.",
            t =>
            {
                ConfigureHybridTiming(t);
                t.HeavyDamage = 16.0;
            }),
        new("HybridHeavyDamage12",
            "Hybrid timing with normal Heavy damage reduced from 24 to 12.",
            t =>
            {
                ConfigureHybridTiming(t);
                t.HeavyDamage = 12.0;
            })
    ];
}

internal sealed class MatchupAccumulator(string first, string second)
{
    public readonly string First = first;
    public readonly string Second = second;
    public int Matches;
    public int FirstWins;
    public int SecondWins;
    public int Draws;
    public int Stalemates;
    public double DurationSum;
    public double FirstHealthSum;
    public double SecondHealthSum;
    public double FirstStaminaSum;
    public double SecondStaminaSum;
    public double FirstGained;
    public double SecondGained;
    public double FirstSpent;
    public double SecondSpent;
    public double FirstWasted;
    public double SecondWasted;
    public double InitiativeChanges;
    public double Retaliations;
    public double UnavoidableThreats;
    public double UnavoidableHits;
    public int LongestUnanswered;
    public int RepeatedAttackSequences;
    public int HeavyStartupBeatenByLight;
    public int FirstAttacks;
    public int SecondAttacks;
    public int FirstHits;
    public int SecondHits;
    public int FirstHeavyStartupBeaten;
    public int SecondHeavyStartupBeaten;
    public int FirstRepeated;
    public int SecondRepeated;
    public int FirstBufferedWhileDefending;
    public int SecondBufferedWhileDefending;
    public int FirstBufferedInputs;
    public int SecondBufferedInputs;
    public int FirstQueueRejects;
    public int SecondQueueRejects;
    public int MaximumQueueDepth;
    public double BufferedInputs;
    public double InputsQueuedWhileDefending;
    public double QueueRejects;
    public double PredictedDefenseActions;
    public double IncorrectDefenseChoices;
    public long ActionAttempts;
    public long FailedActions;
    public readonly Dictionary<string, long> Attacks = new(StringComparer.Ordinal);
    public readonly Dictionary<string, long> SuccessfulHitsByAttack = new(StringComparer.Ordinal);
    public readonly Dictionary<string, long> Outcomes = new(StringComparer.Ordinal);
    public readonly Dictionary<string, long> DefenseAttempts = new(StringComparer.Ordinal);
    public readonly Dictionary<string, long> DefenseSuccesses = new(StringComparer.Ordinal);
    public readonly Dictionary<string, long> ActionAttemptsByType = new(StringComparer.Ordinal);
    public readonly Dictionary<string, long> ActionFailuresByType = new(StringComparer.Ordinal);
    public readonly Dictionary<string, long> DefenseTimingOffsetMs = new(StringComparer.Ordinal);
    public readonly Dictionary<string, long> FrameAdvantageMs = new(StringComparer.Ordinal);

    public void Add(MatchResult result)
    {
        Matches++;
        if (result.Winner == 1) FirstWins++;
        else if (result.Winner == 2) SecondWins++;
        else Draws++;
        if (result.EndReason == "Stalemate") Stalemates++;
        DurationSum += result.Duration;
        FirstHealthSum += result.FirstHealth;
        SecondHealthSum += result.SecondHealth;
        FirstStaminaSum += result.FirstStamina;
        SecondStaminaSum += result.SecondStamina;
        FirstGained += result.FirstGained;
        SecondGained += result.SecondGained;
        FirstSpent += result.FirstSpent;
        SecondSpent += result.SecondSpent;
        FirstWasted += result.FirstWasted;
        SecondWasted += result.SecondWasted;
        InitiativeChanges += result.InitiativeChanges;
        Retaliations += result.SuccessfulRetaliations;
        UnavoidableThreats += result.UnavoidableThreats;
        UnavoidableHits += result.UnavoidableHits;
        LongestUnanswered = Math.Max(LongestUnanswered, result.LongestUnanswered);
        RepeatedAttackSequences += result.RepeatedAttackSequences;
        HeavyStartupBeatenByLight += result.HeavyStartupBeatenByLight;
        FirstAttacks += result.FirstAttacks;
        SecondAttacks += result.SecondAttacks;
        FirstHits += result.FirstHits;
        SecondHits += result.SecondHits;
        FirstHeavyStartupBeaten += result.FirstHeavyStartupBeaten;
        SecondHeavyStartupBeaten += result.SecondHeavyStartupBeaten;
        FirstRepeated += result.FirstRepeated;
        SecondRepeated += result.SecondRepeated;
        FirstBufferedWhileDefending += result.FirstBufferedWhileDefending;
        SecondBufferedWhileDefending += result.SecondBufferedWhileDefending;
        FirstBufferedInputs += result.FirstBufferedInputs;
        SecondBufferedInputs += result.SecondBufferedInputs;
        FirstQueueRejects += result.FirstQueueRejects;
        SecondQueueRejects += result.SecondQueueRejects;
        MaximumQueueDepth = Math.Max(MaximumQueueDepth, result.MaximumQueueDepth);
        BufferedInputs += result.FirstBufferedInputs + result.SecondBufferedInputs;
        InputsQueuedWhileDefending += result.FirstBufferedWhileDefending + result.SecondBufferedWhileDefending;
        QueueRejects += result.FirstQueueRejects + result.SecondQueueRejects;
        PredictedDefenseActions += result.PredictedDefenseActions;
        IncorrectDefenseChoices += result.IncorrectDefenseChoices;
        ActionAttempts += result.FirstActionAttempts + result.SecondActionAttempts;
        FailedActions += result.FirstFailedActions + result.SecondFailedActions;
        Merge(Attacks, result.Attacks);
        Merge(SuccessfulHitsByAttack, result.SuccessfulHitsByAttack);
        Merge(Outcomes, result.Outcomes);
        Merge(DefenseAttempts, result.DefenseAttempts);
        Merge(DefenseSuccesses, result.DefenseSuccesses);
        Merge(ActionAttemptsByType, result.ActionAttemptsByType);
        Merge(ActionFailuresByType, result.ActionFailuresByType);
        Merge(DefenseTimingOffsetMs, result.DefenseTimingOffsetMs);
        Merge(FrameAdvantageMs, result.FrameAdvantageMs);
    }

    public MatchupReport ToReport() => new()
    {
        FirstStrategy = First,
        SecondStrategy = Second,
        Matches = Matches,
        FirstWins = FirstWins,
        SecondWins = SecondWins,
        Draws = Draws,
        Stalemates = Stalemates,
        AverageDurationSeconds = Average(DurationSum),
        AverageFirstHealth = Average(FirstHealthSum),
        AverageSecondHealth = Average(SecondHealthSum),
        AverageFirstStamina = Average(FirstStaminaSum),
        AverageSecondStamina = Average(SecondStaminaSum),
        AverageFirstStaminaGained = Average(FirstGained),
        AverageSecondStaminaGained = Average(SecondGained),
        AverageFirstStaminaSpent = Average(FirstSpent),
        AverageSecondStaminaSpent = Average(SecondSpent),
        AverageFirstStaminaWastedAtCap = Average(FirstWasted),
        AverageSecondStaminaWastedAtCap = Average(SecondWasted),
        AverageInitiativeChanges = Average(InitiativeChanges),
        AverageSuccessfulRetaliations = Average(Retaliations),
        AverageUnavoidableThreats = Average(UnavoidableThreats),
        AverageUnavoidableHits = Average(UnavoidableHits),
        LongestConsecutiveUnavoidableHits = LongestUnanswered,
        AverageRepeatedAttackSequences = Average(RepeatedAttackSequences),
        AverageHeavyStartupBeatenByLight = Average(HeavyStartupBeatenByLight),
        AverageBufferedInputs = Average(BufferedInputs),
        AverageInputsQueuedWhileDefending = Average(InputsQueuedWhileDefending),
        AverageQueueRejects = Average(QueueRejects),
        AveragePredictedDefenseActions = Average(PredictedDefenseActions),
        AverageIncorrectDefenseChoices = Average(IncorrectDefenseChoices),
        ActionAttempts = ActionAttempts,
        FailedActions = FailedActions,
        MaximumObservedQueueDepth = MaximumQueueDepth,
        AttackCounts = new Dictionary<string, long>(Attacks, StringComparer.Ordinal),
        SuccessfulHitsByAttack = new Dictionary<string, long>(SuccessfulHitsByAttack, StringComparer.Ordinal),
        OutcomeCounts = new Dictionary<string, long>(Outcomes, StringComparer.Ordinal),
        DefenseAttempts = new Dictionary<string, long>(DefenseAttempts, StringComparer.Ordinal),
        DefenseSuccesses = new Dictionary<string, long>(DefenseSuccesses, StringComparer.Ordinal),
        ActionAttemptsByType = new Dictionary<string, long>(ActionAttemptsByType, StringComparer.Ordinal),
        ActionFailuresByType = new Dictionary<string, long>(ActionFailuresByType, StringComparer.Ordinal),
        DefenseSuccessRates = MakeDefenseSuccessRates(),
        DefenseTimingOffsetMs = new Dictionary<string, long>(DefenseTimingOffsetMs, StringComparer.Ordinal),
        FrameAdvantageMs = new Dictionary<string, long>(FrameAdvantageMs, StringComparer.Ordinal)
    };

    private double Average(double sum) => Matches == 0 ? 0.0 : sum / Matches;

    private Dictionary<string, double> MakeDefenseSuccessRates()
    {
        var rates = new Dictionary<string, double>(StringComparer.Ordinal);
        foreach (var attempt in DefenseAttempts)
        {
            var separator = attempt.Key.IndexOf('.');
            if (separator < 0 ||
                !Enum.TryParse<DefenseKind>(attempt.Key[(separator + 1)..], out _))
            {
                continue;
            }

            rates[attempt.Key] = attempt.Value == 0 ? 0.0 :
                DefenseSuccesses.GetValueOrDefault(attempt.Key) / (double)attempt.Value;
        }

        return rates;
    }

    private static void Merge(Dictionary<string, long> target, Dictionary<string, long> source)
    {
        foreach (var pair in source)
        {
            target[pair.Key] = target.GetValueOrDefault(pair.Key) + pair.Value;
        }
    }
}

internal sealed class MatchResult
{
    public int Winner;
    public string EndReason = "";
    public double Duration;
    public double FirstHealth;
    public double SecondHealth;
    public double FirstStamina;
    public double SecondStamina;
    public double FirstGained;
    public double SecondGained;
    public double FirstSpent;
    public double SecondSpent;
    public double FirstWasted;
    public double SecondWasted;
    public int InitiativeChanges;
    public int SuccessfulRetaliations;
    public int FirstSuccessfulRetaliations;
    public int SecondSuccessfulRetaliations;
    public int UnavoidableThreats;
    public int UnavoidableHits;
    public int FirstUnavoidableHits;
    public int SecondUnavoidableHits;
    public int LongestUnanswered;
    public int RepeatedAttackSequences;
    public int HeavyStartupBeatenByLight;
    public int FirstAttacks;
    public int SecondAttacks;
    public int FirstHits;
    public int SecondHits;
    public int FirstHeavyStartupBeaten;
    public int SecondHeavyStartupBeaten;
    public int FirstRepeated;
    public int SecondRepeated;
    public int FirstBufferedWhileDefending;
    public int SecondBufferedWhileDefending;
    public int FirstBufferedInputs;
    public int SecondBufferedInputs;
    public int FirstQueueRejects;
    public int SecondQueueRejects;
    public int MaximumQueueDepth;
    public int PredictedDefenseActions;
    public int IncorrectDefenseChoices;
    public long FirstActionAttempts;
    public long SecondActionAttempts;
    public long FirstFailedActions;
    public long SecondFailedActions;
    public CombatReplay? Replay;
    public readonly Dictionary<string, long> Attacks = new(StringComparer.Ordinal);
    public readonly Dictionary<string, long> SuccessfulHitsByAttack = new(StringComparer.Ordinal);
    public readonly Dictionary<string, long> Outcomes = new(StringComparer.Ordinal);
    public readonly Dictionary<string, long> DefenseAttempts = new(StringComparer.Ordinal);
    public readonly Dictionary<string, long> DefenseSuccesses = new(StringComparer.Ordinal);
    public readonly Dictionary<string, long> ActionAttemptsByType = new(StringComparer.Ordinal);
    public readonly Dictionary<string, long> ActionFailuresByType = new(StringComparer.Ordinal);
    public readonly List<(double Time, string Message)> ActionFailureEvents = [];
    public readonly Dictionary<string, long> DefenseTimingOffsetMs = new(StringComparer.Ordinal);
    public readonly Dictionary<string, long> FrameAdvantageMs = new(StringComparer.Ordinal);
    public readonly List<string> EventLog = [];
}

public static class SimulationRunner
{
    private readonly record struct ThreatKey(bool AttackerIsPlayer, double LaunchAt, AttackKind Kind);

    public static async Task<SimulationReport> RunAsync(
        SimulationOptions options,
        CancellationToken cancellationToken = default)
    {
        if (options.RequestedMatches <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options.RequestedMatches));
        }

        if (options.StepSeconds <= 0.0 || options.MatchLimitSeconds <= 0.0 || options.StalemateSeconds <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Simulation time steps and limits must be positive.");
        }

        if (!double.IsFinite(options.ImpactEstimateUncertaintyMs) || options.ImpactEstimateUncertaintyMs < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(options.ImpactEstimateUncertaintyMs),
                "Impact-estimate uncertainty must be finite and non-negative.");
        }

        if (!double.IsFinite(options.ActionFailureProbability) ||
            options.ActionFailureProbability is < 0.0 or > 1.0)
        {
            throw new ArgumentOutOfRangeException(nameof(options.ActionFailureProbability),
                "Action-failure probability must be between 0 and 1.");
        }

        if (options.Profile is not null && options.Profiles is not null)
        {
            throw new ArgumentException("Specify either Profile or Profiles, not both.", nameof(options));
        }

        ArgumentOutOfRangeException.ThrowIfNegative(options.OpeningPairs);
        var strategies = AgentFactory.StrategyNames;
        string[]? selectedProfileNames = options.Profiles;
        if (selectedProfileNames is null && options.Profile is not null)
        {
            selectedProfileNames = [options.Profile];
        }

        var profiles = selectedProfileNames is null
            ? ScenarioProfile.All
            : ScenarioProfile.All.Concat(ScenarioProfile.ReadStudy).Where(profile => selectedProfileNames.Any(name =>
                string.Equals(profile.Name, name, StringComparison.OrdinalIgnoreCase))).ToArray();
        if (profiles.Length == 0)
        {
            throw new ArgumentException(
                $"Unknown simulation profile selection '{string.Join(",", selectedProfileNames ?? Array.Empty<string>())}'.",
                nameof(options));
        }

        var unmatchedProfiles = (selectedProfileNames ?? Array.Empty<string>())
            .Where(name => !profiles.Any(profile =>
                string.Equals(profile.Name, name, StringComparison.OrdinalIgnoreCase))).ToArray();
        if (unmatchedProfiles.Length > 0)
        {
            throw new ArgumentException(
                $"Unknown simulation profile(s): {string.Join(", ", unmatchedProfiles)}.",
                nameof(options));
        }

        var matchesPerPair = Math.Max(1, (int)Math.Ceiling(
            options.RequestedMatches / (double)(profiles.Length * strategies.Length * strategies.Length)));
        var report = new SimulationReport
        {
            Seed = options.Seed,
            ImpactEstimateUncertaintyMs = options.ImpactEstimateUncertaintyMs,
            ActionFailureProbability = options.ActionFailureProbability,
            RequestedMatches = options.RequestedMatches,
            MatchesPerPairAndProfile = matchesPerPair
        };
        var strategyStats = strategies.ToDictionary(name => name, _ => new StrategySummary { Strategy = _ },
            StringComparer.Ordinal);

        foreach (var profile in profiles)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var profileReport = new ProfileReport
            {
                Name = profile.Name,
                Description = profile.Description
            };
            double durationSum = 0.0;
            double initiativeSum = 0.0;
            double retaliationSum = 0.0;
            double unavoidableSum = 0.0;
            double unansweredSum = 0.0;
            double bufferedWhileDefendingSum = 0.0;
            double queueRejectSum = 0.0;
            long actionAttemptsSum = 0;
            long failedActionsSum = 0;
            var matchups = new Dictionary<(string, string), MatchupAccumulator>();

            for (var firstIndex = 0; firstIndex < strategies.Length; firstIndex++)
            for (var secondIndex = 0; secondIndex < strategies.Length; secondIndex++)
            {
                var firstName = strategies[firstIndex];
                var secondName = strategies[secondIndex];
                var accumulator = new MatchupAccumulator(firstName, secondName);
                matchups[(firstName, secondName)] = accumulator;

                for (var matchIndex = 0; matchIndex < matchesPerPair; matchIndex++)
                {
                    if ((matchIndex & 255) == 0)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    var seed = DeriveSeed(options.Seed, profile.SeedGroup ?? profile.Name, firstIndex, secondIndex, matchIndex);
                    var result = RunMatch(profile, firstName, secondName, seed, options);
                    accumulator.Add(result);
                    profileReport.Matches++;
                    if (result.EndReason == "Stalemate") profileReport.Stalemates++;
                    if (result.EndReason == "Timeout") profileReport.Timeouts++;
                    if (result.Winner == 0) profileReport.Draws++;
                    durationSum += result.Duration;
                    initiativeSum += result.InitiativeChanges;
                    retaliationSum += result.SuccessfulRetaliations;
                    unavoidableSum += result.UnavoidableThreats;
                    unansweredSum += result.LongestUnanswered;
                    bufferedWhileDefendingSum += result.FirstBufferedWhileDefending +
                        result.SecondBufferedWhileDefending;
                    queueRejectSum += result.FirstQueueRejects + result.SecondQueueRejects;
                    actionAttemptsSum += result.FirstActionAttempts + result.SecondActionAttempts;
                    failedActionsSum += result.FirstFailedActions + result.SecondFailedActions;
                    UpdateStrategyStats(strategyStats[firstName], result, true);
                    UpdateStrategyStats(strategyStats[secondName], result, false);

                    if (report.ExploitExamples.Count < 20 &&
                        (result.UnavoidableHits > 0 || result.LongestUnanswered >= 3 ||
                         result.FirstQueueRejects + result.SecondQueueRejects >= 5 ||
                         result.EndReason == "Stalemate"))
                    {
                        report.ExploitExamples.Add(new ExploitExample
                        {
                            Profile = profile.Name,
                            FirstStrategy = firstName,
                            SecondStrategy = secondName,
                            MatchSeed = seed,
                            Issue = result.UnavoidableHits > 0
                                ? $"{result.UnavoidableHits} unavoidable clean hit(s)"
                                : result.LongestUnanswered >= 3
                                    ? $"clean hits with no theoretical counter opportunity: {result.LongestUnanswered}"
                                    : result.FirstQueueRejects + result.SecondQueueRejects >= 5
                                        ? $"repeated attack-buffer overflow: {result.FirstQueueRejects + result.SecondQueueRejects} rejected inputs"
                                    : "defensive stalemate",
                            DurationSeconds = result.Duration,
                            EventLog = result.EventLog
                        });
                    }
                }

                profileReport.Matchups.Add(accumulator.ToReport());
            }

            profileReport.AverageDurationSeconds = profileReport.Matches == 0 ? 0 : durationSum / profileReport.Matches;
            profileReport.AverageInitiativeChanges = profileReport.Matches == 0 ? 0 : initiativeSum / profileReport.Matches;
            profileReport.AverageSuccessfulRetaliations = profileReport.Matches == 0 ? 0 : retaliationSum / profileReport.Matches;
            profileReport.AverageUnavoidableThreats = profileReport.Matches == 0 ? 0 : unavoidableSum / profileReport.Matches;
            profileReport.AverageUnavoidableHitStreak = profileReport.Matches == 0 ? 0 : unansweredSum / profileReport.Matches;
            profileReport.AverageInputsQueuedWhileDefending = profileReport.Matches == 0
                ? 0 : bufferedWhileDefendingSum / profileReport.Matches;
            profileReport.AverageQueueRejects = profileReport.Matches == 0
                ? 0 : queueRejectSum / profileReport.Matches;
            profileReport.ActionAttempts = actionAttemptsSum;
            profileReport.FailedActions = failedActionsSum;
            profileReport.AveragePredictedDefenseActions = profileReport.Matches == 0
                ? 0 : (double)matchups.Values.Sum(m => m.PredictedDefenseActions) / profileReport.Matches;
            profileReport.AverageIncorrectDefenseChoices = profileReport.Matches == 0
                ? 0 : (double)matchups.Values.Sum(m => m.IncorrectDefenseChoices) / profileReport.Matches;
            // Use the same seed in both seats. Only the opening policy differs.
            for (int i = 0; i < options.OpeningPairs; i++)
            {
                var seed = DeriveSeed(options.Seed, "opening-duel", 0, 0, i);
                for (int seat = 1; seat <= 2; seat++)
                {
                    var duel = RunMatch(profile, seat == 1 ? "Balanced" : "CounterFirst",
                        seat == 2 ? "Balanced" : "CounterFirst", seed, options);
                    if (duel.Winner == 0) profileReport.OpeningDraws++;
                    else if (duel.Winner == seat) profileReport.AttackFirstWins++;
                    else profileReport.CounterFirstWins++;
                }
            }
            report.Profiles.Add(profileReport);
        }

        report.ActualMatches = report.Profiles.Sum(p => p.Matches);
        report.Strategies.AddRange(strategyStats.Values);
        await WriteReportsAsync(report, options.OutputDirectory, cancellationToken);
        return report;
    }

    internal static MatchResult RunMatch(
        ScenarioProfile profile,
        string firstName,
        string secondName,
        ulong seed,
        SimulationOptions options,
        CombatReplayRecorder? replayRecorder = null)
    {
        var tuning = new CombatTuning
        {
            AiAttackEnabled = false,
            AiDefenseEnabled = false
        };
        profile.Configure(tuning);
        var engine = new CombatEngine(tuning);
        var first = AgentFactory.Create(firstName, ReactionFor(seed, 1),
            firstName is "Balanced" or "CounterFirst" ? options.BalancedParameters : null,
            firstName == "PreviousBalanced" ? options.PreviousBalancedParameters : null, tuning.AiParryReadChance);
        var second = AgentFactory.Create(secondName, ReactionFor(seed, 2),
            secondName is "Balanced" or "CounterFirst" ? options.BalancedParameters : null,
            secondName == "PreviousBalanced" ? options.PreviousBalancedParameters : null, tuning.AiParryReadChance);
        var firstRandom = new DeterministicRandom(Mix(seed ^ 0xA0761D6478BD642FUL));
        var secondRandom = new DeterministicRandom(Mix(seed ^ 0xE7037ED1A0B428DBUL));
        var firstActionRandom = new DeterministicRandom(Mix(seed ^ 0xD1B54A32D192ED03UL));
        var secondActionRandom = new DeterministicRandom(Mix(seed ^ 0x94D049BB133111EBUL));
        var firstTimingPerception = new AgentTimingPerception(
            new DeterministicRandom(Mix(seed ^ 0x8EBC6AF09C88C6E3UL)),
            options.ImpactEstimateUncertaintyMs);
        var secondTimingPerception = new AgentTimingPerception(
            new DeterministicRandom(Mix(seed ^ 0x589965CC75374CC3UL)),
            options.ImpactEstimateUncertaintyMs);
        var result = new MatchResult();
        replayRecorder?.Start(engine);
        var unavoidable = new Dictionary<ThreatKey, bool>();
        var observed = new HashSet<ThreatKey>();
        var lastDamageAt = 0.0;
        var resolutionIndex = 0;
        var firstUnanswered = 0;
        var secondUnanswered = 0;
        var previousResolutionAttacker = -1;
        double firstStaminaSum = 0;
        double secondStaminaSum = 0;
        long staminaSamples = 0;
        var consecutiveAttacksBySide = new Dictionary<bool, int>();
        var previousAttackKind = new Dictionary<bool, AttackKind>();
        var heavyStartupBeaten = new HashSet<long>();
        var attackById = new Dictionary<long, AttackState>();
        var knownAttackCount = 0;
        var startAt = engine.Time;
        var endReason = "Timeout";
        long frame = 0;

        while (engine.Time - startAt < options.MatchLimitSeconds)
        {
            var playerObservation = AgentObservationFactory.Create(engine, engine.Player, firstTimingPerception);
            var opponentObservation = AgentObservationFactory.Create(engine, engine.Opponent, secondTimingPerception);
            var playerIncomingTruth = engine.GetIncomingThreat(engine.Player);
            var opponentIncomingTruth = engine.GetIncomingThreat(engine.Opponent);
            replayRecorder?.RecordIncoming(engine, frame, engine.Player, playerObservation.Incoming, playerIncomingTruth);
            replayRecorder?.RecordIncoming(engine, frame, engine.Opponent, opponentObservation.Incoming, opponentIncomingTruth);
            MarkThreat(playerObservation, playerIncomingTruth, true, tuning, observed, unavoidable, result);
            MarkThreat(opponentObservation, opponentIncomingTruth, false, tuning, observed, unavoidable, result);

            var playerDecision = first.Decide(playerObservation, firstRandom);
            if (replayRecorder is not null &&
                first is IAgentDecisionTraceProvider { LastDefenseTrace: { } playerTrace })
            {
                replayRecorder.RecordDefenseSelection(engine, frame, engine.Player, playerTrace);
            }

            var opponentDecision = second.Decide(opponentObservation, secondRandom);
            if (replayRecorder is not null &&
                second is IAgentDecisionTraceProvider { LastDefenseTrace: { } opponentTrace })
            {
                replayRecorder.RecordDefenseSelection(engine, frame, engine.Opponent, opponentTrace);
            }

            ApplyDecision(engine, engine.Player, playerDecision, result, firstActionRandom,
                options.ActionFailureProbability, replayRecorder, frame, playerIncomingTruth);
            ApplyDecision(engine, engine.Opponent, opponentDecision, result, secondActionRandom,
                options.ActionFailureProbability, replayRecorder, frame, opponentIncomingTruth);
            replayRecorder?.CaptureChanges(engine, frame);
            engine.Tick(Math.Min(options.StepSeconds, options.MatchLimitSeconds - (engine.Time - startAt)));
            frame++;
            replayRecorder?.CaptureChanges(engine, frame);

            while (knownAttackCount < engine.AttackHistory.Count)
            {
                var newlyAdded = engine.AttackHistory[knownAttackCount++];
                attackById[newlyAdded.Id] = newlyAdded;
            }

            firstStaminaSum += engine.Player.Stamina;
            secondStaminaSum += engine.Opponent.Stamina;
            staminaSamples++;

            while (resolutionIndex < engine.Resolutions.Count)
            {
                var resolution = engine.Resolutions[resolutionIndex++];
                if (resolution.Damage > 0.0)
                {
                    lastDamageAt = resolution.ImpactTime;
                }

                result.Outcomes[resolution.Outcome.ToString()] =
                    result.Outcomes.GetValueOrDefault(resolution.Outcome.ToString()) + 1;
                if (resolution.Outcome == AttackOutcome.CleanHit)
                {
                    var kind = resolution.AttackKind.ToString();
                    result.SuccessfulHitsByAttack[kind] =
                        result.SuccessfulHitsByAttack.GetValueOrDefault(kind) + 1;
                }
                AddHistogram(result.FrameAdvantageMs, resolution.FrameAdvantage * 1000.0, 50.0);
                if (resolution.DefenseTimingOffset is { } timingOffset)
                {
                    AddHistogram(result.DefenseTimingOffsetMs, timingOffset * 1000.0, 30.0);
                }
                if (resolution.Defense is { } defense)
                {
                    var defenderSide = resolution.AttackerIsPlayer ? "Opponent" : "Player";
                    var defenseKey = $"{defenderSide}.{defense}";
                    result.DefenseSuccesses[defenseKey] =
                        result.DefenseSuccesses.GetValueOrDefault(defenseKey) + 1;
                }

                if (attackById.TryGetValue(resolution.AttackId, out var attack))
                {
                    if (attack.Info.Blockable && attack.Defender.WasBlockingAt(attack.ImpactAt))
                    {
                        var defenderName = attack.Defender.IsPlayer ? "Player" : "Opponent";
                        var blockKey = $"{defenderName}.Block";
                        result.DefenseAttempts[blockKey] =
                            result.DefenseAttempts.GetValueOrDefault(blockKey) + 1;
                    }

                    var threatKey = new ThreatKey(resolution.AttackerIsPlayer, attack.LaunchAt, resolution.AttackKind);
                    var noCounterAvailable =
                        unavoidable.TryGetValue(threatKey, out var counterUnavailable) && counterUnavailable;
                    if (resolution.Outcome == AttackOutcome.CleanHit && noCounterAvailable)
                    {
                        result.UnavoidableHits++;
                        if (resolution.AttackerIsPlayer) result.FirstUnavoidableHits++;
                        else result.SecondUnavoidableHits++;
                    }

                    if (resolution.Outcome == AttackOutcome.CleanHit)
                    {
                        var attackerSide = resolution.AttackerIsPlayer;
                        var previous = noCounterAvailable
                            ? consecutiveAttacksBySide.GetValueOrDefault(attackerSide)
                            : 0;
                        var current = noCounterAvailable ? previous + 1 : 0;
                        consecutiveAttacksBySide[attackerSide] = current;
                        consecutiveAttacksBySide[!attackerSide] = 0;
                        if (current > (attackerSide ? firstUnanswered : secondUnanswered))
                        {
                            if (attackerSide) firstUnanswered = current;
                            else secondUnanswered = current;
                        }

                        if (previousResolutionAttacker != -1 && previousResolutionAttacker != (attackerSide ? 1 : 2))
                        {
                            result.SuccessfulRetaliations++;
                            if (attackerSide) result.FirstSuccessfulRetaliations++;
                            else result.SecondSuccessfulRetaliations++;
                        }

                        previousResolutionAttacker = attackerSide ? 1 : 2;
                    }
                    else
                    {
                        consecutiveAttacksBySide[resolution.AttackerIsPlayer] = 0;
                        previousResolutionAttacker = -1;
                    }

                    if (previousAttackKind.TryGetValue(resolution.AttackerIsPlayer, out var priorKind) &&
                        priorKind == resolution.AttackKind)
                    {
                        result.RepeatedAttackSequences++;
                        if (resolution.AttackerIsPlayer) result.FirstRepeated++;
                        else result.SecondRepeated++;
                    }

                    previousAttackKind[resolution.AttackerIsPlayer] = resolution.AttackKind;
                    if (resolution.AttackKind == AttackKind.Light && resolution.Outcome == AttackOutcome.CleanHit)
                    {
                        foreach (var active in engine.AttackHistory)
                        {
                            if (!active.Resolved && active.Info.Kind == AttackKind.Heavy &&
                                active.Attacker.IsPlayer != resolution.AttackerIsPlayer &&
                                resolution.ImpactTime < active.ImpactAt)
                            {
                                if (heavyStartupBeaten.Add(active.Id))
                                {
                                    result.HeavyStartupBeatenByLight++;
                                    if (active.Attacker.IsPlayer) result.FirstHeavyStartupBeaten++;
                                    else result.SecondHeavyStartupBeaten++;
                                }
                            }
                        }
                    }
                }
            }

            if (engine.Player.Health <= 0.0 || engine.Opponent.Health <= 0.0)
            {
                endReason = "Knockout";
                break;
            }

            if (engine.Time - lastDamageAt >= options.StalemateSeconds)
            {
                endReason = "Stalemate";
                break;
            }

            if (engine.Time - startAt >= options.MatchLimitSeconds)
            {
                endReason = "Timeout";
                break;
            }
        }

        if (endReason == "Timeout" && engine.Time - startAt < options.MatchLimitSeconds)
        {
            endReason = "Stalemate";
        }

        var healthDifference = engine.Player.Health - engine.Opponent.Health;
        result.Winner = engine.Player.Health <= 0.0 && engine.Opponent.Health <= 0.0 ? 0 :
            engine.Player.Health <= 0.0 ? 2 :
            engine.Opponent.Health <= 0.0 ? 1 :
            Math.Abs(healthDifference) < 1e-9 ? 0 :
            healthDifference > 0.0 ? 1 : 2;
        result.EndReason = endReason;
        result.Duration = engine.Time - startAt;
        result.FirstHealth = engine.Player.Health;
        result.SecondHealth = engine.Opponent.Health;
        result.FirstStamina = staminaSamples == 0 ? engine.Player.Stamina : firstStaminaSum / staminaSamples;
        result.SecondStamina = staminaSamples == 0 ? engine.Opponent.Stamina : secondStaminaSum / staminaSamples;
        result.InitiativeChanges = engine.InitiativeHistory.Count;
        result.LongestUnanswered = Math.Max(firstUnanswered, secondUnanswered);
        result.FirstHits = engine.Resolutions.Count(r => r.AttackerIsPlayer && r.Outcome == AttackOutcome.CleanHit);
        result.SecondHits = engine.Resolutions.Count(r => !r.AttackerIsPlayer && r.Outcome == AttackOutcome.CleanHit);

        foreach (var attack in engine.AttackHistory)
        {
            var key = attack.Info.Kind.ToString();
            result.Attacks[key] = result.Attacks.GetValueOrDefault(key) + 1;
            if (attack.Attacker.IsPlayer) result.FirstAttacks++;
            else result.SecondAttacks++;
        }

        foreach (var change in engine.StaminaLedger)
        {
            if (change.Requested > 0.0)
            {
                if (change.IsPlayer) result.FirstGained += Math.Max(0.0, change.Applied);
                else result.SecondGained += Math.Max(0.0, change.Applied);
            }
            else if (change.Requested < 0.0)
            {
                if (change.IsPlayer) result.FirstSpent += -change.Requested;
                else result.SecondSpent += -change.Requested;
            }

            if (change.IsPlayer) result.FirstWasted += change.WastedAtCap;
            else result.SecondWasted += change.WastedAtCap;
        }

        result.InitiativeChanges = engine.InitiativeHistory.Count;
        if (result.ActionFailureEvents.Count == 0)
        {
            result.EventLog.AddRange(engine.Events.TakeLast(160).Select(e => $"{e.Time:0.000}s {e.Message}"));
        }
        else
        {
            result.EventLog.AddRange(engine.Events.TakeLast(160).Select(e => (e.Time, e.Message))
                .Concat(result.ActionFailureEvents)
                .OrderBy(e => e.Time)
                .TakeLast(160)
                .Select(e => $"{e.Time:0.000}s {e.Message}"));
        }
        if (replayRecorder is not null)
        {
            replayRecorder.Complete(engine, frame, result);
            result.Replay = replayRecorder.Replay;
        }

        return result;
    }

    private static void MarkThreat(
        in AgentObservation observation,
        AttackPreview? incoming,
        bool defenderIsPlayer,
        CombatTuning tuning,
        HashSet<ThreatKey> observed,
        Dictionary<ThreatKey, bool> unavoidable,
        MatchResult result)
    {
        if (incoming is not { } threat)
        {
            return;
        }

        var key = new ThreatKey(!defenderIsPlayer, threat.LaunchAt, threat.Info.Kind);
        if (!observed.Add(key))
        {
            return;
        }

        var remaining = Math.Max(0.0, threat.ImpactAt - observation.Time);
        var hasCounter =
            threat.Info.Blockable && observation.BlockReadyIn <= remaining + 1e-9 ||
            threat.Info.Parryable && observation.ParryReadyIn <= remaining + Math.Max(0.0, tuning.ParryWindow) / 2.0 + 1e-9 ||
            threat.Info.Dodgeable && observation.DodgeReadyIn <= remaining + Math.Max(0.0, tuning.DodgeWindow) / 2.0 + 1e-9 ||
            threat.Info.Dodgeable && threat.Info.Lane == AttackLane.High && tuning.DuckCountersHigh &&
                observation.DuckReadyIn <= remaining + Math.Max(0.0, tuning.DirectionalCounterWindow) / 2.0 + 1e-9 ||
            threat.Info.Dodgeable && threat.Info.Lane == AttackLane.Low && tuning.HopCountersLow &&
                observation.HopReadyIn <= remaining + Math.Max(0.0, tuning.DirectionalCounterWindow) / 2.0 + 1e-9;
        unavoidable[key] = !hasCounter;
        if (!hasCounter)
        {
            result.UnavoidableThreats++;
        }
    }

    private static void ApplyDecision(
        CombatEngine engine,
        CombatFighter fighter,
        AgentDecision decision,
        MatchResult result,
        DeterministicRandom actionFailureRandom,
        double actionFailureProbability,
        CombatReplayRecorder? replayRecorder = null,
        long frame = 0,
        AttackPreview? incoming = null)
    {
        if (decision.HoldBlock is { } holdBlock)
        {
            var wasHeld = fighter.BlockHeld;
            var inputAccepted = wasHeld == holdBlock ||
                TryAcceptAction(fighter, holdBlock ? "BlockOn" : "BlockOff", engine.Time,
                    actionFailureRandom, actionFailureProbability, result);
            var accepted = inputAccepted && engine.SetBlockHeld(fighter, holdBlock);
            if (replayRecorder is not null && wasHeld != holdBlock)
            {
                replayRecorder.RecordDefenseInput(engine, frame, fighter, DefenseKind.Block,
                    accepted, holdBlock, incoming);
            }
        }

        if (decision.Defense is { } defense)
        {
            var key = $"{(fighter.IsPlayer ? "Player" : "Opponent")}.{defense}";
            result.DefenseAttempts[key] = result.DefenseAttempts.GetValueOrDefault(key) + 1;
            var accepted = TryAcceptAction(fighter, $"Defense.{defense}", engine.Time,
                actionFailureRandom, actionFailureProbability, result) &&
                engine.ActivateDefense(fighter, defense);
            replayRecorder?.RecordDefenseInput(engine, frame, fighter, defense, accepted, incoming: incoming);
        }

        if (decision.Attack is { } attack)
        {
            var wasWaitingForReadiness = fighter.CurrentAttack is not null ||
                fighter.AttackReadyAt > engine.Time + 1e-9;
            var queuedWhileDefending = fighter.BlockHeld ||
                decision.HoldBlock == true || decision.Defense is not null;
            var inputAccepted = TryAcceptAction(fighter, $"Attack.{attack}", engine.Time,
                actionFailureRandom, actionFailureProbability, result);
            var accepted = inputAccepted && engine.QueueAttack(fighter, attack, decision.AttackLane);
            replayRecorder?.RecordAttackQueued(engine, frame, fighter, attack, decision.AttackLane, accepted);
            if (accepted)
            {
                if (fighter.IsPlayer)
                {
                    if (wasWaitingForReadiness) result.FirstBufferedInputs++;
                    if (queuedWhileDefending) result.FirstBufferedWhileDefending++;
                }
                else
                {
                    if (wasWaitingForReadiness) result.SecondBufferedInputs++;
                    if (queuedWhileDefending) result.SecondBufferedWhileDefending++;
                }

                result.MaximumQueueDepth = Math.Max(result.MaximumQueueDepth, fighter.BufferedAttackCount);
            }
            else if (inputAccepted && fighter.IsPlayer)
            {
                result.FirstQueueRejects++;
            }
            else if (inputAccepted)
            {
                result.SecondQueueRejects++;
            }
        }

        if (decision.IsPrediction)
        {
            result.PredictedDefenseActions++;
        }

        if (decision.IsIncorrectChoice)
        {
            result.IncorrectDefenseChoices++;
        }
    }

    private static bool TryAcceptAction(
        CombatFighter fighter,
        string action,
        double time,
        DeterministicRandom random,
        double failureProbability,
        MatchResult result)
    {
        var side = fighter.IsPlayer ? "Player" : "Opponent";
        var key = $"{side}.{action}";
        result.ActionAttemptsByType[key] = result.ActionAttemptsByType.GetValueOrDefault(key) + 1;
        if (fighter.IsPlayer) result.FirstActionAttempts++;
        else result.SecondActionAttempts++;

        if (failureProbability <= 0.0 || !random.Chance(failureProbability))
        {
            return true;
        }

        result.ActionFailuresByType[key] = result.ActionFailuresByType.GetValueOrDefault(key) + 1;
        if (fighter.IsPlayer) result.FirstFailedActions++;
        else result.SecondFailedActions++;
        if (result.ActionFailureEvents.Count == 160)
        {
            result.ActionFailureEvents.RemoveAt(0);
        }

        result.ActionFailureEvents.Add((time, $"action_failed fighter={side} action={action}"));
        return false;
    }

    internal static ReactionModel ReactionFor(ulong seed, int side)
    {
        var random = new DeterministicRandom(Mix(seed ^ (ulong)(side * 0x9E37)));
        var reaction = new[] { 0.16, 0.20, 0.26 }[random.NextInt(3)];
        return new ReactionModel(
            MeanReactionSeconds: reaction,
            ReactionStandardDeviation: 0.045,
            TimingErrorStandardDeviation: 0.035,
            PredictionChance: 0.08,
            PredictionErrorChance: 0.12,
            PredictedAttackInterval: 1.15);
    }

    private static ulong DeriveSeed(ulong seed, string profile, int first, int second, int matchIndex)
    {
        var profileHash = 14695981039346656037UL;
        foreach (var character in profile)
        {
            profileHash = (profileHash ^ character) * 1099511628211UL;
        }

        return Mix(seed ^ profileHash ^ ((ulong)first << 40) ^ ((ulong)second << 24) ^ (uint)matchIndex);
    }

    private static ulong Mix(ulong value)
    {
        value += 0x9E3779B97F4A7C15UL;
        value = (value ^ (value >> 30)) * 0xBF58476D1CE4E5B9UL;
        value = (value ^ (value >> 27)) * 0x94D049BB133111EBUL;
        return value ^ (value >> 31);
    }

    private static void UpdateStrategyStats(StrategySummary summary, MatchResult result, bool first)
    {
        summary.Matches++;
        var ownWinner = first ? 1 : 2;
        var otherWinner = first ? 2 : 1;
        if (result.Winner == ownWinner) summary.Wins++;
        else if (result.Winner == otherWinner) summary.Losses++;
        else summary.Draws++;
        var opponentHealth = first ? result.SecondHealth : result.FirstHealth;
        summary.AverageDamage += (100.0 - opponentHealth);
        summary.AverageStaminaSpent += first ? result.FirstSpent : result.SecondSpent;
        summary.AverageStaminaWastedAtCap += first ? result.FirstWasted : result.SecondWasted;
        summary.AttacksLaunched += first ? result.FirstAttacks : result.SecondAttacks;
        summary.AttacksHit += first ? result.FirstHits : result.SecondHits;
        summary.HeavyStartupBeatenByLight += first
            ? result.FirstHeavyStartupBeaten
            : result.SecondHeavyStartupBeaten;
        summary.RepeatedAttackSequences += first ? result.FirstRepeated : result.SecondRepeated;
        var sidePrefix = first ? "Player." : "Opponent.";
        summary.DefenseAttempts += result.DefenseAttempts
            .Where(pair => pair.Key.StartsWith(sidePrefix, StringComparison.Ordinal))
            .Sum(pair => pair.Value);
        summary.SuccessfulDefenses += result.DefenseSuccesses
            .Where(pair => pair.Key.StartsWith(sidePrefix, StringComparison.Ordinal))
            .Sum(pair => pair.Value);
        summary.BufferedInputs += first ? result.FirstBufferedInputs : result.SecondBufferedInputs;
        summary.InputsQueuedWhileDefending += first
            ? result.FirstBufferedWhileDefending
            : result.SecondBufferedWhileDefending;
        summary.QueueRejects += first ? result.FirstQueueRejects : result.SecondQueueRejects;
        summary.MaximumObservedQueueDepth = Math.Max(
            summary.MaximumObservedQueueDepth, result.MaximumQueueDepth);
    }

    private static void AddHistogram(Dictionary<string, long> histogram, double value, double bucketWidth)
    {
        var lower = Math.Floor(value / bucketWidth) * bucketWidth;
        var key = $"{lower:+0;-0;0}..{lower + bucketWidth:+0;-0;0}ms";
        histogram[key] = histogram.GetValueOrDefault(key) + 1;
    }

    private static string FormatTopBuckets(Dictionary<string, long> histogram) =>
        string.Join(", ", histogram.OrderByDescending(pair => pair.Value).Take(3)
            .Select(pair => $"{pair.Key}:{pair.Value}"));

    private static string FormatAttackMix(MatchupReport matchup) =>
        string.Join(", ", matchup.AttackCounts.Select(pair =>
            $"{pair.Key}:{pair.Value}/" +
            $"{matchup.SuccessfulHitsByAttack.GetValueOrDefault(pair.Key)}"));

    private static string FormatDefenseRates(MatchupReport matchup) =>
        string.Join(", ", matchup.DefenseSuccessRates.Select(pair => $"{pair.Key}:{pair.Value:P0}"));

    private static async Task WriteReportsAsync(
        SimulationReport report,
        string outputDirectory,
        CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(outputDirectory);
        var jsonPath = Path.Combine(outputDirectory, "balance-report.json");
        var markdownPath = Path.Combine(outputDirectory, "balance-report.md");
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };
        await File.WriteAllTextAsync(jsonPath,
            JsonSerializer.Serialize(report, jsonOptions), cancellationToken);
        await File.WriteAllTextAsync(markdownPath, CreateMarkdown(report), cancellationToken);
    }

    private static string CreateMarkdown(SimulationReport report)
    {
        var text = new StringBuilder();
        text.AppendLine("# Combat balance simulation");
        text.AppendLine();
        text.AppendLine($"- Seed: `{report.Seed}`");
        text.AppendLine($"- Impact estimate uncertainty: {report.ImpactEstimateUncertaintyMs:0.###}ms (per-threat, cached; 0ms = exact ETA)");
        text.AppendLine($"- Action input failure probability: {report.ActionFailureProbability:P0}");
        text.AppendLine("- Failures suppress the selected attack/defense input; they do not alter core combat resolution. Held-block transitions are retried while the agent continues requesting them.");
        text.AppendLine($"- Matches: {report.ActualMatches:N0} (requested {report.RequestedMatches:N0})");
        text.AppendLine($"- Seeds per profile/matchup: {report.MatchesPerPairAndProfile:N0}");
        text.AppendLine("- Agents receive public visible-threat fields and an impact estimate; exact launch/impact times remain internal. No omniscient agent is included.");
        text.AppendLine();
        text.AppendLine("## Strategy summary");
        text.AppendLine();
        text.AppendLine("| Strategy | Wins | Losses | Draws | Win rate | Dominant | Damage dealt | Hit rate | Stamina spent | Wasted at cap | Buffered in defense | Queue rejects |");
        text.AppendLine("|---|---:|---:|---:|---:|:---:|---:|---:|---:|---:|---:|---:|");
        foreach (var strategy in report.Strategies.OrderByDescending(s => s.WinRate))
        {
            text.AppendLine($"| {strategy.Strategy} | {strategy.Wins:N0} | {strategy.Losses:N0} | {strategy.Draws:N0} | " +
                $"{strategy.WinRate:P1} | {(strategy.Dominant ? "YES" : "no")} | " +
                $"{(strategy.Matches == 0 ? 0 : strategy.AverageDamage / strategy.Matches):0.0} | " +
                $"{(strategy.AttacksLaunched == 0 ? 0 : strategy.AttacksHit / (double)strategy.AttacksLaunched):P1} | " +
                $"{(strategy.Matches == 0 ? 0 : strategy.AverageStaminaSpent / strategy.Matches):0.0} | " +
                $"{(strategy.Matches == 0 ? 0 : strategy.AverageStaminaWastedAtCap / strategy.Matches):0.0} | " +
                $"{strategy.InputsQueuedWhileDefending:N0} | {strategy.QueueRejects:N0} |");
        }

        foreach (var profile in report.Profiles)
        {
            text.AppendLine();
            text.AppendLine($"## {profile.Name}");
            text.AppendLine();
            text.AppendLine(profile.Description);
            text.AppendLine();
            if (profile.AttackFirstWins + profile.CounterFirstWins + profile.OpeningDraws > 0)
                text.AppendLine($"Extra mirrored opening duels (outside strategy matrix): attack-first {profile.AttackFirstWins}, counter-first {profile.CounterFirstWins}, draws {profile.OpeningDraws}.");
            text.AppendLine($"Matches: {profile.Matches:N0}; stalemates: {profile.Stalemates:N0}; " +
                $"timeouts: {profile.Timeouts:N0}; avg duration: {profile.AverageDurationSeconds:0.00}s; " +
                $"avg initiative changes: {profile.AverageInitiativeChanges:0.00}; " +
                $"avg retaliations: {profile.AverageSuccessfulRetaliations:0.00}; " +
                $"avg unavoidable threats: {profile.AverageUnavoidableThreats:0.00}; " +
                $"avg clean-hit run without theoretical counter opportunity: {profile.AverageUnavoidableHitStreak:0.00}; " +
                $"avg attacks buffered while defending: {profile.AverageInputsQueuedWhileDefending:0.00}; " +
                $"avg rejected queue inputs: {profile.AverageQueueRejects:0.00}; " +
                $"predicted defenses: {profile.AveragePredictedDefenseActions:0.00}; " +
                $"incorrect defense choices: {profile.AverageIncorrectDefenseChoices:0.00}.");
            text.AppendLine($"Action inputs failed: {profile.FailedActions:N0}/{profile.ActionAttempts:N0} " +
                $"({profile.ActionFailureRate:P1}).");
            text.AppendLine();
            text.AppendLine("| Matchup | First wins | Second wins | Draws | Avg. duration | Hit rate | Unavoidable hits | Repeated attacks | Buffered while defending | Queue rejects | Max queue | Action failures | Frame advantage (top bins) | Defense timing (top bins) | Defense success | Attack hit mix | Outcomes |");
            text.AppendLine("|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---|---|---|---|---|");
            foreach (var matchup in profile.Matchups.OrderBy(m => m.FirstStrategy).ThenBy(m => m.SecondStrategy))
            {
                text.AppendLine($"| {matchup.FirstStrategy} vs {matchup.SecondStrategy} | {matchup.FirstWins:N0} | " +
                    $"{matchup.SecondWins:N0} | {matchup.Draws:N0} | {matchup.AverageDurationSeconds:0.00}s | " +
                    $"{matchup.GetAttackHitRate():P1} | {matchup.AverageUnavoidableHits:0.00} | " +
                    $"{matchup.AverageRepeatedAttackSequences:0.00} | " +
                    $"{matchup.AverageInputsQueuedWhileDefending:0.00} | {matchup.AverageQueueRejects:0.00} | " +
                    $"{matchup.MaximumObservedQueueDepth} | " +
                    $"{matchup.FailedActions:N0}/{matchup.ActionAttempts:N0} ({matchup.ActionFailureRate:P0}) | " +
                    $"{FormatTopBuckets(matchup.FrameAdvantageMs)} | " +
                    $"{FormatTopBuckets(matchup.DefenseTimingOffsetMs)} | " +
                    $"{FormatDefenseRates(matchup)} | " +
                    $"{FormatAttackMix(matchup)} | " +
                    $"{string.Join(", ", matchup.OutcomeCounts.Select(pair => $"{pair.Key}:{pair.Value}"))} |");
            }
        }

        if (report.ExploitExamples.Count > 0)
        {
            text.AppendLine();
            text.AppendLine("## Reproducible exploit / edge-case examples");
            foreach (var example in report.ExploitExamples)
            {
                text.AppendLine();
                text.AppendLine($"### {example.Issue}: {example.FirstStrategy} vs {example.SecondStrategy}");
                text.AppendLine($"Profile `{example.Profile}`, seed `{example.MatchSeed}`, duration {example.DurationSeconds:0.00}s.");
                text.AppendLine();
                text.AppendLine("```text");
                foreach (var entry in example.EventLog.TakeLast(80))
                {
                    text.AppendLine(entry);
                }

                text.AppendLine("```");
            }
        }

        text.AppendLine();
        text.AppendLine("Experimental profiles do not modify the prototype's default rules. Win rate is only one diagnostic; interpret it alongside hit rates, defensive counterplay, stamina waste, stalemates, unanswered sequences, and reversals.");
        return text.ToString();
    }
}
