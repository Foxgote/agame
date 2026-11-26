using System.Text;
using System.Text.Json;
using CombatPrototype.Core;

namespace CombatPrototype.Simulation;

public sealed record EvolutionOptions(
    int Generations = 5,
    int Population = 10,
    int TrainingSeeds = 4,
    int EvaluationSeeds = 12,
    ulong Seed = 0xC0B47UL,
    string OutputDirectory = "combat-prototype/simulation/reports");

public sealed class EvolutionReport
{
    public ulong Seed { get; init; }
    public int Generations { get; init; }
    public int Population { get; init; }
    public string TrainingOpponents { get; init; } = "";
    public string EvaluationOpponents { get; init; } = "";
    public bool IncludedPreviousAgent { get; init; }
    public BalancedAgentParameters BestParameters { get; init; } = new();
    public double TrainingFitness { get; init; }
    public double EvaluationFitness { get; init; }
    public int TrainingMatches { get; init; }
    public int EvaluationMatches { get; init; }
    public double EvaluationWinRate { get; init; }
    public double EvaluationDrawRate { get; init; }
    public double EvaluationDefenseSuccessRate { get; init; }
    public double EvaluationStalemateRate { get; init; }
    public double EvaluationUnavoidableHitRate { get; init; }
}

public static class EvolutionaryOptimizer
{
    private static readonly string[] TrainingOpponents =
    [
        "AggressiveLightSpam",
        "PassiveBlocking",
        "ReactionDefense",
        "DodgeHeavy",
        "ParryFishing"
    ];

    public static async Task<EvolutionReport> RunAsync(
        EvolutionOptions options,
        CancellationToken cancellationToken = default)
    {
        Validate(options);
        Directory.CreateDirectory(options.OutputDirectory);
        var savedAgentPath = Path.Combine(options.OutputDirectory, "trained-balanced-agent.json");
        BalancedAgentParameters? previousAgent = null;
        if (File.Exists(savedAgentPath))
        {
            await using var previousAgentFile = File.OpenRead(savedAgentPath);
            previousAgent = await JsonSerializer.DeserializeAsync<BalancedAgentParameters>(
                previousAgentFile, cancellationToken: cancellationToken);
        }

        var random = new DeterministicRandom(options.Seed);
        var population = new List<BalancedAgentParameters> { new() };
        while (population.Count < options.Population)
        {
            population.Add(RandomParameters(random));
        }

        var trainingSeeds = Enumerable.Range(0, options.TrainingSeeds)
            .Select(index => Mix(options.Seed ^ (ulong)index))
            .ToArray();
        var previousBest = population[0];
        var previousFitness = double.NegativeInfinity;
        for (var generation = 0; generation < options.Generations; generation++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var ranked = new List<(BalancedAgentParameters Parameters, CandidateScore Score)>();
            foreach (var candidate in population)
            {
                var score = Evaluate(candidate, TrainingOpponents, trainingSeeds, null, options.Seed, cancellationToken);
                ranked.Add((candidate, score));
            }

            ranked.Sort((left, right) => right.Score.Fitness.CompareTo(left.Score.Fitness));
            if (ranked[0].Score.Fitness > previousFitness)
            {
                previousBest = ranked[0].Parameters;
                previousFitness = ranked[0].Score.Fitness;
            }

            Console.WriteLine($"Generation {generation + 1}/{options.Generations}: " +
                $"training fitness {ranked[0].Score.Fitness:0.000}");
            var next = ranked.Take(Math.Max(2, options.Population / 4))
                .Select(entry => entry.Parameters).ToList();
            while (next.Count < options.Population)
            {
                var parent = next[random.NextInt(Math.Min(4, next.Count))];
                next.Add(Mutate(parent, random));
            }

            population = next;
        }

        var evaluationSeeds = Enumerable.Range(0, options.EvaluationSeeds)
            .Select(index => Mix(options.Seed ^ 0xD1B54A32D192ED03UL ^ (ulong)index))
            .ToArray();
        var evaluationOpponents = previousAgent is null
            ? TrainingOpponents
            : TrainingOpponents.Append("PreviousBalanced").ToArray();
        var evaluation = Evaluate(previousBest, evaluationOpponents, evaluationSeeds, previousAgent,
            options.Seed ^ 0xD1B54A32D192ED03UL, cancellationToken);
        var training = Evaluate(previousBest, TrainingOpponents, trainingSeeds, null,
            options.Seed, cancellationToken);

        var report = new EvolutionReport
        {
            Seed = options.Seed,
            Generations = options.Generations,
            Population = options.Population,
            TrainingOpponents = string.Join(", ", TrainingOpponents),
            EvaluationOpponents = string.Join(", ", evaluationOpponents),
            IncludedPreviousAgent = previousAgent is not null,
            BestParameters = previousBest,
            TrainingFitness = training.Fitness,
            EvaluationFitness = evaluation.Fitness,
            TrainingMatches = training.Matches,
            EvaluationMatches = evaluation.Matches,
            EvaluationWinRate = evaluation.WinRate,
            EvaluationDrawRate = evaluation.DrawRate,
            EvaluationDefenseSuccessRate = evaluation.DefenseSuccessRate,
            EvaluationStalemateRate = evaluation.StalemateRate,
            EvaluationUnavoidableHitRate = evaluation.UnavoidableHitRate
        };

        await File.WriteAllTextAsync(savedAgentPath,
            JsonSerializer.Serialize(previousBest, new JsonSerializerOptions { WriteIndented = true }),
            cancellationToken);
        await File.WriteAllTextAsync(Path.Combine(options.OutputDirectory, "evolution-report.json"),
            JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }),
            cancellationToken);
        await File.WriteAllTextAsync(Path.Combine(options.OutputDirectory, "evolution-report.md"),
            CreateMarkdown(report), cancellationToken);
        return report;
    }

    private static CandidateScore Evaluate(
        BalancedAgentParameters candidate,
        IReadOnlyList<string> opponents,
        IReadOnlyList<ulong> seeds,
        BalancedAgentParameters? previousAgent,
        ulong baseSeed,
        CancellationToken cancellationToken)
    {
        var score = new CandidateScore();
        var tuning = new SimulationOptions(
            RequestedMatches: 1,
            Seed: baseSeed,
            MatchLimitSeconds: 30,
            StalemateSeconds: 8)
        {
            BalancedParameters = candidate,
            PreviousBalancedParameters = previousAgent
        };
        var matchCount = 0;
        foreach (var opponent in opponents)
        foreach (var seed in seeds)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var candidateFirst = (matchCount++ & 1) == 0;
            var result = SimulationRunner.RunMatch(
                ScenarioProfile.All[0],
                candidateFirst ? "Balanced" : opponent,
                candidateFirst ? opponent : "Balanced",
                Mix(seed ^ (ulong)matchCount),
                tuning);
            var candidateWon = candidateFirst ? result.Winner == 1 : result.Winner == 2;
            var opponentWon = candidateFirst ? result.Winner == 2 : result.Winner == 1;
            score.Matches++;
            if (candidateWon) score.Wins++;
            else if (opponentWon) score.Losses++;
            else score.Draws++;
            if (result.EndReason == "Stalemate") score.Stalemates++;
            score.UnavoidableHits += candidateFirst
                ? result.FirstUnavoidableHits
                : result.SecondUnavoidableHits;
            score.Attacks += candidateFirst ? result.FirstAttacks : result.SecondAttacks;
            score.Retaliations += candidateFirst
                ? result.FirstSuccessfulRetaliations
                : result.SecondSuccessfulRetaliations;
            score.Repeated += candidateFirst ? result.FirstRepeated : result.SecondRepeated;
            var prefix = candidateFirst ? "Player." : "Opponent.";
            score.DefenseAttempts += result.DefenseAttempts
                .Where(pair => pair.Key.StartsWith(prefix, StringComparison.Ordinal))
                .Sum(pair => pair.Value);
            score.DefenseSuccesses += result.DefenseSuccesses
                .Where(pair => pair.Key.StartsWith(prefix, StringComparison.Ordinal))
                .Sum(pair => pair.Value);
        }

        score.Finish();
        return score;
    }

    private static BalancedAgentParameters RandomParameters(DeterministicRandom random) => new(
        AttackInterval: Between(random, 0.12, 0.65),
        HeavyProbability: Between(random, 0.0, 0.70),
        SpecialProbability: Between(random, 0.0, 0.30),
        DefensiveAggression: Between(random, 0.25, 1.0),
        ParryProbability: Between(random, 0.05, 0.75),
        DodgeProbability: Between(random, 0.05, 0.60),
        BlockBias: Between(random, 0.1, 0.9),
        ReactionDelay: Between(random, 0.12, 0.32),
        TimingError: Between(random, 0.01, 0.08));

    private static BalancedAgentParameters Mutate(
        BalancedAgentParameters parent,
        DeterministicRandom random) =>
        new(
            AttackInterval: Mutate(parent.AttackInterval, 0.12, 0.65, random, 0.09),
            HeavyProbability: Mutate(parent.HeavyProbability, 0, 0.70, random, 0.12),
            SpecialProbability: Mutate(parent.SpecialProbability, 0, 0.30, random, 0.07),
            DefensiveAggression: Mutate(parent.DefensiveAggression, 0.25, 1.0, random, 0.12),
            ParryProbability: Mutate(parent.ParryProbability, 0.05, 0.75, random, 0.12),
            DodgeProbability: Mutate(parent.DodgeProbability, 0.05, 0.60, random, 0.10),
            BlockBias: Mutate(parent.BlockBias, 0.1, 0.9, random, 0.12),
            ReactionDelay: Mutate(parent.ReactionDelay, 0.12, 0.32, random, 0.035),
            TimingError: Mutate(parent.TimingError, 0.01, 0.08, random, 0.015));

    private static double Mutate(
        double value,
        double minimum,
        double maximum,
        DeterministicRandom random,
        double scale) =>
        Math.Clamp(value + random.NextGaussian() * scale, minimum, maximum);

    private static double Between(DeterministicRandom random, double minimum, double maximum) =>
        minimum + random.NextDouble() * (maximum - minimum);

    private static ulong Mix(ulong value)
    {
        value += 0x9E3779B97F4A7C15UL;
        value = (value ^ (value >> 30)) * 0xBF58476D1CE4E5B9UL;
        value = (value ^ (value >> 27)) * 0x94D049BB133111EBUL;
        return value ^ (value >> 31);
    }

    private static void Validate(EvolutionOptions options)
    {
        if (options.Generations < 1 || options.Population < 4 ||
            options.TrainingSeeds < 1 || options.EvaluationSeeds < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(options),
                "Use at least one generation, four candidates, and one seed per evaluation set.");
        }
    }

    private static string CreateMarkdown(EvolutionReport report)
    {
        var text = new StringBuilder();
        text.AppendLine("# Balanced agent evolutionary experiment");
        text.AppendLine();
        text.AppendLine($"- Seed: `{report.Seed}`");
        text.AppendLine($"- Training: {report.TrainingMatches} matches against {report.TrainingOpponents}");
        text.AppendLine($"- Unseen evaluation: {report.EvaluationMatches} matches against {report.EvaluationOpponents}");
        text.AppendLine($"- Training fitness: {report.TrainingFitness:0.000}");
        text.AppendLine($"- Evaluation fitness: {report.EvaluationFitness:0.000}");
        text.AppendLine($"- Evaluation win/draw: {report.EvaluationWinRate:P1} / {report.EvaluationDrawRate:P1}");
        text.AppendLine($"- Defensive success rate: {report.EvaluationDefenseSuccessRate:P1}");
        text.AppendLine($"- Stalemate rate: {report.EvaluationStalemateRate:P1}");
        text.AppendLine($"- Unavoidable clean hits per attack: {report.EvaluationUnavoidableHitRate:P2}");
        text.AppendLine();
        text.AppendLine("## Candidate parameters");
        text.AppendLine();
        foreach (var property in typeof(BalancedAgentParameters).GetProperties())
        {
            text.AppendLine($"- {property.Name}: `{property.GetValue(report.BestParameters)}`");
        }

        text.AppendLine();
        text.AppendLine("Fitness combines match points, defensive success, initiative reversals, stalemate avoidance, and penalties for unavoidable hits and repetitive sequences. This report does not apply the candidate to prototype defaults.");
        return text.ToString();
    }

    private sealed class CandidateScore
    {
        public int Matches;
        public int Wins;
        public int Losses;
        public int Draws;
        public int Stalemates;
        public long UnavoidableHits;
        public long Attacks;
        public long Retaliations;
        public long Repeated;
        public long DefenseAttempts;
        public long DefenseSuccesses;
        public double Fitness;
        public double WinRate;
        public double DrawRate;
        public double DefenseSuccessRate;
        public double StalemateRate;
        public double UnavoidableHitRate;

        public void Finish()
        {
            if (Matches == 0)
            {
                return;
            }

            WinRate = Wins / (double)Matches;
            DrawRate = Draws / (double)Matches;
            StalemateRate = Stalemates / (double)Matches;
            DefenseSuccessRate = DefenseAttempts == 0 ? 0.0 : DefenseSuccesses / (double)DefenseAttempts;
            UnavoidableHitRate = Attacks == 0 ? 0.0 : UnavoidableHits / (double)Attacks;
            var points = (Wins + Draws * 0.5) / Matches;
            var retaliationRate = Math.Min(1.0, Retaliations / (double)Matches);
            var repetitionRate = Attacks == 0 ? 0.0 : Math.Min(1.0, Repeated / (double)Attacks);
            Fitness = 0.40 * points +
                0.20 * DefenseSuccessRate +
                0.20 * retaliationRate +
                0.10 * (1.0 - StalemateRate) -
                0.10 * UnavoidableHitRate -
                0.10 * repetitionRate;
        }
    }
}
