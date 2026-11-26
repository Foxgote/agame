using System.Globalization;
using System.Text.Json;
using CombatPrototype.Core;
using CombatPrototype.Simulation;

try
{
    var arguments = ParseArguments(args);
    if (arguments.ContainsKey("help"))
    {
        PrintUsage();
        return;
    }

    if (arguments.ContainsKey("self-test"))
    {
        RunSelfTests();
        Console.WriteLine("Simulation self-tests passed.");
        return;
    }

    var output = GetString(arguments, "out",
        Path.Combine("combat-prototype", "simulation", "reports"));
    var seed = GetULong(arguments, "seed", 20250308);
    if (arguments.ContainsKey("train-counter"))
    {
        CounterAgentTraining.Run(GetInt(arguments, "generations", 8), GetInt(arguments, "population", 14),
            GetInt(arguments, "training-seeds", 6), GetInt(arguments, "evaluation-seeds", 24), seed,
            GetString(arguments, "out", Path.Combine(output, "counter-trial")));
        return;
    }
    if (arguments.ContainsKey("capture-replays"))
    {
        var replayDirectory = GetString(arguments, "out",
            Path.Combine("combat-prototype", "simulation", "replays"));
        var summary = await ReplayCapture.RunAsync(replayDirectory, seed);
        Console.WriteLine($"Captured {summary.CapturedReplays} replays after {summary.AttemptedMatches:N0} " +
            $"deterministic candidate matches.");
        Console.WriteLine($"Replay index: {Path.GetFullPath(Path.Combine(replayDirectory, "replay-index.md"))}");
        return;
    }

    if (arguments.ContainsKey("train"))
    {
        var trainingReport = await EvolutionaryOptimizer.RunAsync(new EvolutionOptions(
            Generations: GetInt(arguments, "generations", 5),
            Population: GetInt(arguments, "population", 10),
            TrainingSeeds: GetInt(arguments, "training-seeds", 4),
            EvaluationSeeds: GetInt(arguments, "evaluation-seeds", 12),
            Seed: seed,
            OutputDirectory: output));
        Console.WriteLine($"Evolution complete: training fitness {trainingReport.TrainingFitness:0.000}, " +
            $"unseen evaluation fitness {trainingReport.EvaluationFitness:0.000}.");
        Console.WriteLine($"Candidate saved to {Path.GetFullPath(Path.Combine(output, "trained-balanced-agent.json"))}");
        return;
    }

    var matches = GetInt(arguments, "matches", 100_000);
    var balancedParameters = arguments.TryGetValue("balanced-agent", out var balancedAgentPath)
        ? JsonSerializer.Deserialize<BalancedAgentParameters>(File.ReadAllText(balancedAgentPath!))
            ?? throw new InvalidDataException("The balanced-agent file did not contain valid parameters.")
        : null;
    var options = new SimulationOptions(RequestedMatches: matches, Seed: seed, OutputDirectory: output)
    {
        ImpactEstimateUncertaintyMs = GetDouble(arguments, "impact-uncertainty-ms", 0.0),
        OpeningPairs = GetInt(arguments, "opening-pairs", 0),
        ActionFailureProbability = GetDouble(arguments, "action-failure-probability", 0.0),
        Profile = arguments.TryGetValue("profile", out var selectedProfile) ? selectedProfile : null,
        Profiles = arguments.TryGetValue("profiles", out var selectedProfiles)
            ? selectedProfiles!.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            : null,
        BalancedParameters = balancedParameters
    };
    var report = await SimulationRunner.RunAsync(options);
    Console.WriteLine($"Completed {report.ActualMatches:N0} matches across {report.Profiles.Count} configurations.");
    Console.WriteLine($"Results: {Path.GetFullPath(Path.Combine(output, "balance-report.md"))}");
    Console.WriteLine($"Seed: {report.Seed}");
}
catch (Exception exception)
{
    Console.Error.WriteLine(exception.Message);
    Environment.ExitCode = 1;
}

static Dictionary<string, string?> ParseArguments(string[] arguments)
{
    var result = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
    for (var index = 0; index < arguments.Length; index++)
    {
        var current = arguments[index];
        if (!current.StartsWith("--", StringComparison.Ordinal))
        {
            throw new ArgumentException($"Unexpected argument: {current}");
        }

        var key = current[2..];
        if (key is "help" or "self-test" or "train" or "train-counter" or "capture-replays")
        {
            result[key] = null;
            continue;
        }

        if (++index >= arguments.Length || arguments[index].StartsWith("--", StringComparison.Ordinal))
        {
            throw new ArgumentException($"Missing value for --{key}.");
        }

        result[key] = arguments[index];
    }

    return result;
}

static int GetInt(IReadOnlyDictionary<string, string?> arguments, string key, int fallback) =>
    arguments.TryGetValue(key, out var value)
        ? int.Parse(value!, NumberStyles.Integer, CultureInfo.InvariantCulture)
        : fallback;

static ulong GetULong(IReadOnlyDictionary<string, string?> arguments, string key, ulong fallback) =>
    arguments.TryGetValue(key, out var value)
        ? ulong.Parse(value!, NumberStyles.Integer, CultureInfo.InvariantCulture)
        : fallback;

static double GetDouble(IReadOnlyDictionary<string, string?> arguments, string key, double fallback) =>
    arguments.TryGetValue(key, out var value)
        ? double.Parse(value!, NumberStyles.Float, CultureInfo.InvariantCulture)
        : fallback;

static string GetString(IReadOnlyDictionary<string, string?> arguments, string key, string fallback) =>
    arguments.TryGetValue(key, out var value) ? value! : fallback;

static void PrintUsage()
{
    Console.WriteLine("Headless deterministic combat simulator");
    Console.WriteLine("  --matches <count>   Requested match count across all profiles (default 100000)");
    Console.WriteLine("  --seed <integer>    Reproducible base seed");
    Console.WriteLine("  --impact-uncertainty-ms <n>  Per-threat impact estimate error (default 0, exact ETA)");
    Console.WriteLine("  --profile <name>   Run one profile only (for example HybridTiming or HybridSifuRecovery)");
    Console.WriteLine("  --profiles <names> Comma-separated profile selection");
    Console.WriteLine("  --opening-pairs <n>  Extra mirrored attack-first vs counter-first seed pairs per profile");
    Console.WriteLine("  --action-failure-probability <0..1>  Probability each selected action input fails (default 0)");
    Console.WriteLine("  --balanced-agent <path>   Evaluate a trained agent's parameters in place of Balanced");
    Console.WriteLine("  --out <directory>   Report output directory");
    Console.WriteLine("  --self-test         Run deterministic RNG, strategy, and match replay checks");
    Console.WriteLine("  --capture-replays   Save deterministic representative match replays");
    Console.WriteLine("  --train             Run evolutionary training/evaluation (separate from balance sweeps)");
    Console.WriteLine("  --train-counter     Train the live Counter trial policy with matched 280 ms windups");
    Console.WriteLine("  --generations <n>   Training generations (default 5)");
    Console.WriteLine("  --population <n>    Candidate count per generation (default 10)");
}

static void RunSelfTests()
{
    var firstRandom = new DeterministicRandom(12345);
    var secondRandom = new DeterministicRandom(12345);
    for (var index = 0; index < 1000; index++)
    {
        if (firstRandom.NextUInt64() != secondRandom.NextUInt64())
        {
            throw new InvalidOperationException("The deterministic random generator did not reproduce its sequence.");
        }
    }

    var observation = new AgentObservation(
        0, true, 100, 50, 0, false, null, 0, false, 0, 0, 0, 0, 0,
        "No response yet", 100, 50, 0, false, null, false, "No response yet", null, 0,
        false, 20, 30);
    foreach (var strategy in AgentFactory.StrategyNames)
    {
        var agent = AgentFactory.Create(strategy);
        _ = agent.Decide(observation, new DeterministicRandom(42));
    }

    var perfectTiming = new ReactionModel(0, 0, 0, 0, 0);
    var incomingLight = new AgentThreatObservation(1, AttackKind.Light, AttackLane.Mid,
        true, true, true, true, 0, 0, 0);
    var counterFirst = AgentFactory.Create("CounterFirst", perfectTiming);
    var openingRandom = new DeterministicRandom(42);
    if (counterFirst.Decide(observation, openingRandom).Attack.HasValue ||
        counterFirst.Decide(observation with { Incoming = incomingLight }, openingRandom).Attack.HasValue ||
        !counterFirst.Decide(observation with { Time = 0.3 }, openingRandom).Attack.HasValue)
        throw new InvalidOperationException("Counter-first must wait only through the opening threat.");
    foreach (var chance in new[] { 0.0, 0.75, 1.0 })
    {
        var fisher = new ParryFishingAgent(perfectTiming, chance);
        var random = new DeterministicRandom(618);
        var reads = 0;
        for (int i = 0; i < 1000; i++)
        {
            var seen = observation with { Incoming = incomingLight with { Id = i } };
            if (fisher.Decide(seen, random).Defense == DefenseKind.Parry) reads++;
            if (fisher.Decide(seen, random).Defense.HasValue)
                throw new InvalidOperationException("ParryFishing rerolled a threat.");
        }
        if (chance == 0 && reads != 0 || chance == 1 && reads != 1000 ||
            chance == 0.75 && Math.Abs(reads - 750) > 50)
            throw new InvalidOperationException($"Unexpected parry choices: {reads} at {chance}.");
    }

    var options = new SimulationOptions(
        RequestedMatches: 1,
        Seed: 987654321,
        MatchLimitSeconds: 8,
        StalemateSeconds: 3,
        OutputDirectory: Path.GetTempPath());
    var seed = 77665544UL;
    var first = SimulationRunner.RunMatch(
        ScenarioProfile.All[0], "Balanced", "ReactionDefense", seed, options);
    var replay = SimulationRunner.RunMatch(
        ScenarioProfile.All[0], "Balanced", "ReactionDefense", seed, options);
    if (first.Winner != replay.Winner ||
        first.Duration != replay.Duration ||
        !first.Attacks.OrderBy(pair => pair.Key).SequenceEqual(replay.Attacks.OrderBy(pair => pair.Key)) ||
        !first.Outcomes.OrderBy(pair => pair.Key).SequenceEqual(replay.Outcomes.OrderBy(pair => pair.Key)) ||
        !first.EventLog.SequenceEqual(replay.EventLog))
    {
        throw new InvalidOperationException("A fixed match seed did not reproduce its result and event log.");
    }

    var tuning = new CombatTuning
    {
        AiAttackEnabled = false,
        AiDefenseEnabled = false
    };
    var recorder = new CombatReplayRecorder(
        seed,
        "Baseline",
        "Balanced",
        "ReactionDefense",
        SimulationRunner.ReactionFor(seed, 1),
        SimulationRunner.ReactionFor(seed, 2),
        tuning);
    var recorded = SimulationRunner.RunMatch(
        ScenarioProfile.All[0], "Balanced", "ReactionDefense", seed, options, recorder);
    if (!ResultsEqual(first, recorded) || recorded.Replay is null || recorded.Replay.Events.Count == 0)
    {
        throw new InvalidOperationException("Enabling replay recording changed a deterministic match result.");
    }

    var requiredReplayEvents = new[]
    {
        "attack.queued",
        "attack.launched",
        "attack.impact_reached",
        "attack.resolved",
        "stamina.changed"
    };
    if (requiredReplayEvents.Any(type => !recorded.Replay.Events.Any(item => item.Type == type)) ||
        recorded.Replay.Events.Any(item => item.Player is null || item.Opponent is null) ||
        recorded.Replay.Events.Any(item => string.IsNullOrWhiteSpace(item.FighterId)) ||
        recorded.Replay.Events.Any(item => item.UsedFutureInformation == true) ||
        !recorded.Replay.Events.Any(item => item.Type == "attack.resolved" &&
            item.RelatedDefenseAttemptIds.Length > 0))
    {
        throw new InvalidOperationException("The replay is missing event-state or defense-attempt evidence.");
    }

    var interruptTuning = new CombatTuning
    {
        AiAttackEnabled = false,
        AiDefenseEnabled = false,
        LightInterruptsHeavyStartup = true
    };
    var interruptEngine = new CombatEngine(interruptTuning);
    var interruptRecorder = new CombatReplayRecorder(
        seed,
        "HybridLightInterrupt",
        "Balanced",
        "DodgeHeavy",
        SimulationRunner.ReactionFor(seed, 1),
        SimulationRunner.ReactionFor(seed, 2),
        interruptTuning);
    interruptRecorder.Start(interruptEngine);
    interruptEngine.QueueAttack(interruptEngine.Opponent, AttackKind.Heavy);
    interruptEngine.QueueAttack(interruptEngine.Player, AttackKind.Light);
    var cancelledHeavy = interruptEngine.Opponent.CurrentAttack!;
    var interruptingLight = interruptEngine.Player.CurrentAttack!;
    interruptRecorder.CaptureChanges(interruptEngine, 0);
    interruptEngine.Tick(interruptingLight.ResolveAt - interruptEngine.Time);
    interruptRecorder.CaptureChanges(interruptEngine, 1);
    if (!cancelledHeavy.Cancelled ||
        !interruptRecorder.Replay.Events.Any(item =>
            item.Type == "attack.cancelled" && item.AttackId == cancelledHeavy.Id) ||
        interruptRecorder.Replay.Events.Any(item =>
            item.Type == "attack.impact_reached" && item.AttackId == cancelledHeavy.Id) ||
        !interruptRecorder.Replay.InterruptionBehavior.Contains(
            "clean normal Light cancels", StringComparison.Ordinal))
    {
        throw new InvalidOperationException("A cancelled Heavy was not recorded as a cancellation.");
    }

    var secondRecorder = new CombatReplayRecorder(
        seed,
        "Baseline",
        "Balanced",
        "ReactionDefense",
        SimulationRunner.ReactionFor(seed, 1),
        SimulationRunner.ReactionFor(seed, 2),
        new CombatTuning
        {
            AiAttackEnabled = false,
            AiDefenseEnabled = false
        });
    var recordedAgain = SimulationRunner.RunMatch(
        ScenarioProfile.All[0], "Balanced", "ReactionDefense", seed, options, secondRecorder);
    if (recordedAgain.Replay is null ||
        JsonSerializer.Serialize(recorded.Replay) != JsonSerializer.Serialize(recordedAgain.Replay))
    {
        throw new InvalidOperationException("A recorded replay did not reproduce byte-for-byte.");
    }

    var uncertainOptions = options with { ImpactEstimateUncertaintyMs = 75.0 };
    var uncertainResult = SimulationRunner.RunMatch(
        ScenarioProfile.All[0], "Balanced", "ReactionDefense", seed, uncertainOptions);
    var uncertainRecorder = new CombatReplayRecorder(
        seed,
        "Baseline",
        "Balanced",
        "ReactionDefense",
        SimulationRunner.ReactionFor(seed, 1),
        SimulationRunner.ReactionFor(seed, 2),
        tuning);
    var uncertainRecorded = SimulationRunner.RunMatch(
        ScenarioProfile.All[0], "Balanced", "ReactionDefense", seed, uncertainOptions, uncertainRecorder);
    if (!ResultsEqual(uncertainResult, uncertainRecorded) ||
        uncertainRecorded.Replay is null ||
        !uncertainRecorded.Replay.Events.Any(e =>
            e.Type == "defense.selected" && e.AgentEstimatedImpactAtMs is not null))
    {
        throw new InvalidOperationException(
            "Noisy timing changed a match outcome when replay logging was enabled or omitted the timing estimate.");
    }

    var uncertainAgain = SimulationRunner.RunMatch(
        ScenarioProfile.All[0], "Balanced", "ReactionDefense", seed, uncertainOptions);
    if (!ResultsEqual(uncertainResult, uncertainAgain))
    {
        throw new InvalidOperationException("Noisy impact estimates did not reproduce for a fixed seed.");
    }

    var hiddenTelegraphEngine = new CombatEngine(new CombatTuning
    {
        AiAttackEnabled = true,
        AiDefenseEnabled = false,
        AiInitialDelay = 0.500,
        TelegraphLead = 0.150
    });
    var hiddenTelegraphPerception = new AgentTimingPerception(new DeterministicRandom(11), 0);
    var hiddenTelegraph = AgentObservationFactory.Create(
        hiddenTelegraphEngine, hiddenTelegraphEngine.Player, hiddenTelegraphPerception);
    if (hiddenTelegraph.Incoming is not null ||
        hiddenTelegraph.OpponentAttackActive ||
        hiddenTelegraph.OpponentActiveAttackKind is not null)
    {
        throw new InvalidOperationException("An agent received attack details before the public telegraph.");
    }

    hiddenTelegraphEngine.Tick(0.350);
    var visibleTelegraph = AgentObservationFactory.Create(
        hiddenTelegraphEngine, hiddenTelegraphEngine.Player, hiddenTelegraphPerception);
    if (visibleTelegraph.Incoming is not { Launched: false, Kind: AttackKind.Light })
    {
        throw new InvalidOperationException("A public pre-launch telegraph was not exposed to the agent.");
    }

    var noisyPerception = new AgentTimingPerception(new DeterministicRandom(987), 75);
    var noisyThreatFirst = noisyPerception.Observe(hiddenTelegraphEngine.GetIncomingThreat(hiddenTelegraphEngine.Player));
    var noisyThreatAgain = noisyPerception.Observe(hiddenTelegraphEngine.GetIncomingThreat(hiddenTelegraphEngine.Player));
    var sameSeedPerception = new AgentTimingPerception(new DeterministicRandom(987), 75);
    var noisyThreatReproduced = sameSeedPerception.Observe(hiddenTelegraphEngine.GetIncomingThreat(hiddenTelegraphEngine.Player));
    if (noisyThreatFirst is not { } firstEstimate ||
        noisyThreatAgain?.EstimatedImpactAt != firstEstimate.EstimatedImpactAt ||
        noisyThreatReproduced?.EstimatedImpactAt != firstEstimate.EstimatedImpactAt)
    {
        throw new InvalidOperationException("Per-threat impact estimates were not cached and reproducible.");
    }

    if (typeof(AgentThreatObservation).GetProperty("ImpactAt") is not null ||
        typeof(AgentThreatObservation).GetProperty("LaunchAt") is not null)
    {
        throw new InvalidOperationException("Agent threat observations expose exact attack timing.");
    }

    var reactiveDefense = SimulationRunner.RunMatch(
        ScenarioProfile.All[0], "ReactionDefense", "AggressiveLightSpam", seed + 1, options);
    if (reactiveDefense.Outcomes.GetValueOrDefault(nameof(AttackOutcome.Blocked)) == 0)
    {
        throw new InvalidOperationException("Reaction agents failed to use the untimed held-block response. " +
            string.Join(" | ", reactiveDefense.EventLog.Take(25)));
    }

    var uncertainActionOptions = options with { ActionFailureProbability = 0.5 };
    var actionFailureFirst = SimulationRunner.RunMatch(
        ScenarioProfile.All[0], "AggressiveLightSpam", "PassiveBlocking", seed + 2, uncertainActionOptions);
    var actionFailureReplay = SimulationRunner.RunMatch(
        ScenarioProfile.All[0], "AggressiveLightSpam", "PassiveBlocking", seed + 2, uncertainActionOptions);
    var actionAttempts = actionFailureFirst.FirstActionAttempts + actionFailureFirst.SecondActionAttempts;
    var actionFailures = actionFailureFirst.FirstFailedActions + actionFailureFirst.SecondFailedActions;
    if (!ResultsEqual(actionFailureFirst, actionFailureReplay) ||
        actionAttempts == 0 || actionFailures == 0 || actionFailures >= actionAttempts ||
        actionFailureFirst.ActionAttemptsByType.Count == 0 ||
        actionFailureFirst.ActionFailuresByType.Count == 0 ||
        !actionFailureFirst.EventLog.Any(entry => entry.Contains("action_failed", StringComparison.Ordinal)))
    {
        throw new InvalidOperationException("The 50% action-failure model did not reproduce or record failed inputs.");
    }

    var certainFailureOptions = options with { ActionFailureProbability = 1.0 };
    var certainFailure = SimulationRunner.RunMatch(
        ScenarioProfile.All[0], "AggressiveLightSpam", "PassiveBlocking", seed + 3, certainFailureOptions);
    if (certainFailure.FirstFailedActions + certainFailure.SecondFailedActions !=
            certainFailure.FirstActionAttempts + certainFailure.SecondActionAttempts ||
        certainFailure.FirstAttacks + certainFailure.SecondAttacks != 0)
    {
        throw new InvalidOperationException("A 100% action-failure probability allowed an action to succeed.");
    }
}

static bool ResultsEqual(MatchResult first, MatchResult second) =>
    first.Winner == second.Winner &&
    first.EndReason == second.EndReason &&
    first.Duration == second.Duration &&
    first.FirstHealth == second.FirstHealth &&
    first.SecondHealth == second.SecondHealth &&
    first.FirstStamina == second.FirstStamina &&
    first.SecondStamina == second.SecondStamina &&
    first.FirstAttacks == second.FirstAttacks &&
    first.SecondAttacks == second.SecondAttacks &&
    first.EventLog.SequenceEqual(second.EventLog) &&
    first.FirstActionAttempts == second.FirstActionAttempts &&
    first.SecondActionAttempts == second.SecondActionAttempts &&
    first.FirstFailedActions == second.FirstFailedActions &&
    first.SecondFailedActions == second.SecondFailedActions &&
    first.Attacks.OrderBy(pair => pair.Key).SequenceEqual(second.Attacks.OrderBy(pair => pair.Key)) &&
    first.Outcomes.OrderBy(pair => pair.Key).SequenceEqual(second.Outcomes.OrderBy(pair => pair.Key)) &&
    first.DefenseAttempts.OrderBy(pair => pair.Key).SequenceEqual(second.DefenseAttempts.OrderBy(pair => pair.Key)) &&
    first.DefenseSuccesses.OrderBy(pair => pair.Key).SequenceEqual(second.DefenseSuccesses.OrderBy(pair => pair.Key)) &&
    first.ActionAttemptsByType.OrderBy(pair => pair.Key).SequenceEqual(second.ActionAttemptsByType.OrderBy(pair => pair.Key)) &&
    first.ActionFailuresByType.OrderBy(pair => pair.Key).SequenceEqual(second.ActionFailuresByType.OrderBy(pair => pair.Key));
