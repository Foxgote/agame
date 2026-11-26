using System.Text.Json;
using CombatPrototype.Core;

namespace CombatPrototype.Simulation;

public static class CounterAgentTraining
{
    private sealed record Policy(CounterOpponentSettings Settings, CounterMixupSettings? Mixups);
    private sealed record Style(string Name, double Cadence, double ComboChance, double ComboGap,
        double Reaction, double ReadChance, double ParryBias, double CounterChance);
    private sealed record Score(int Matches, int Wins, int Draws, double DamageDealt, double DamageTaken, double Fitness);

    private static readonly Style[] TrainingStyles =
    [
        new("Rusher", 0.34, 0.65, 0.085, 0.24, 0.66, 0.50, 0.65),
        new("Parry first", 0.49, 0.22, 0.15, 0.19, 0.90, 0.87, 0.95),
        new("Dodge first", 0.43, 0.34, 0.13, 0.21, 0.87, 0.15, 0.91),
        new("Mixed", 0.37, 0.46, 0.12, 0.21, 0.81, 0.58, 0.84)
    ];

    private static readonly Style[] EvaluationStyles =
    [
        .. TrainingStyles,
        new("Burst spam", 0.28, 0.73, 0.025, 0.23, 0.66, 0.67, 0.70),
        new("Patient counter", 0.55, 0.49, 0.09, 0.18, 0.93, 0.46, 0.95)
    ];

    public static void Run(int generations, int population, int trainingSeeds, int evaluationSeeds, ulong seed, string output)
    {
        if (generations < 1 || population < 4 || trainingSeeds < 1 || evaluationSeeds < 1)
            throw new ArgumentOutOfRangeException(nameof(population), "Use at least four candidates and one generation and seed per set.");
        var random = new DeterministicRandom(seed);
        var train = MakeSeeds(random, trainingSeeds);
        var evaluate = MakeSeeds(new DeterministicRandom(seed ^ 0xD1B54A32D192ED03UL), evaluationSeeds);
        var baseline = new Policy(new CounterOpponentSettings(0.34, 0.33, 0.19, 0.90, 0.065), null);
        var starter = baseline with { Mixups = new CounterMixupSettings(0.42, 0.14, 0.86, 0.55) };
        var candidates = Enumerable.Range(0, population).Select(i => i == 0 ? starter : Mutate(starter, random)).ToArray();
        var cache = new Dictionary<Policy, Score>();
        var trainMatches = 0;
        var best = starter;
        for (var generation = 0; generation < generations; generation++)
        {
            var ranked = candidates.OrderByDescending(candidate => TrainScore(candidate).Fitness).ToArray();
            best = ranked[0];
            Console.WriteLine($"Counter trial generation {generation + 1}/{generations}: fitness {TrainScore(best).Fitness:F3}");
            var survivors = Math.Max(2, population / 4);
            candidates = ranked.Take(survivors).Concat(Enumerable.Range(0, population - survivors)
                .Select(_ => Mutate(ranked[random.NextInt(survivors)], random))).ToArray();
        }

        var baselineScore = Evaluate(baseline, EvaluationStyles, evaluate);
        var trainedScore = Evaluate(best, EvaluationStyles, evaluate);
        var byStyle = EvaluationStyles.Select(style => new
        {
            style = style.Name,
            baseline = Evaluate(baseline, [style], evaluate),
            trained = Evaluate(best, [style], evaluate)
        }).ToArray();
        Directory.CreateDirectory(output);
        var json = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
        File.WriteAllText(Path.Combine(output, "trained-counter-agent.json"), JsonSerializer.Serialize(best, json));
        File.WriteAllText(Path.Combine(output, "counter-agent-report.json"), JsonSerializer.Serialize(new
        {
            seed, generations, population, trainMatches, trainingSeeds, evaluationSeeds, windup = CounterTrial.MatchedWindup,
            baseline = baselineScore, trained = trainedScore, byStyle
        }, json));
        Console.WriteLine($"Held-out: trained {trainedScore.Wins}/{trainedScore.Matches} wins; stock {baselineScore.Wins}/{baselineScore.Matches}.");
        Console.WriteLine($"Candidate: {Path.GetFullPath(Path.Combine(output, "trained-counter-agent.json"))}");
        return;

        Score TrainScore(Policy policy)
        {
            if (cache.TryGetValue(policy, out var score)) return score;
            score = Evaluate(policy, TrainingStyles, train);
            trainMatches += score.Matches;
            cache.Add(policy, score);
            return score;
        }
    }

    private static int[] MakeSeeds(DeterministicRandom random, int count) =>
        Enumerable.Range(0, count).Select(_ => random.NextInt(int.MaxValue)).ToArray();

    private static Policy Mutate(Policy parent, DeterministicRandom random)
    {
        var mix = parent.Mixups!.Value;
        return new Policy(parent.Settings with
        {
            Cadence = Change(parent.Settings.Cadence, 0.24, 0.52, 0.065),
            ReactionDelay = Change(parent.Settings.ReactionDelay, 0.15, 0.24, 0.024),
            ReadChance = Change(parent.Settings.ReadChance, 0.72, 0.95, 0.075),
            TimingSpread = Change(parent.Settings.TimingSpread, 0.04, 0.10, 0.017)
        }, new CounterMixupSettings(
            Change(mix.ComboChance, 0.18, 0.72, 0.15),
            Change(mix.ComboGap, 0.10, 0.22, 0.036),
            Change(mix.CounterUseChance, 0.48, 0.98, 0.13),
            Change(mix.ParryBias, 0.18, 0.82, 0.17)));

        double Change(double value, double min, double max, double range) =>
            Math.Clamp(value + random.NextGaussian() * range, min, max);
    }

    private static Score Evaluate(Policy policy, IReadOnlyList<Style> styles, int[] seeds)
    {
        var (wins, draws) = (0, 0);
        var (dealt, taken, fitness) = (0.0, 0.0, 0.0);
        foreach (var style in styles)
        foreach (var seed in seeds)
        {
            var engine = Play(policy, style, seed);
            var dealtHere = engine.Player.DamageTaken;
            var takenHere = engine.Opponent.DamageTaken;
            if (dealtHere > takenHere) wins++;
            else if (dealtHere == takenHere) draws++;
            dealt += dealtHere;
            taken += takenHere;
            var variety = engine.AttackHistory.Where(attack => !attack.Attacker.IsPlayer)
                .Select(attack => attack.Info.Kind).Distinct().Count() / 3.0;
            fitness += Math.Sign(dealtHere - takenHere) +
                0.80 * (dealtHere - takenHere) / Math.Max(1, dealtHere + takenHere) + 0.10 * variety;
        }
        var matches = styles.Count * seeds.Length;
        return new Score(matches, wins, draws, dealt / matches, taken / matches, fitness / matches);
    }

    private static CombatEngine Play(Policy policy, Style style, int seed)
    {
        var random = new DeterministicRandom((ulong)seed);
        var trial = new CounterTrial(random.NextInt(int.MaxValue), random.NextInt(int.MaxValue))
        {
            OpponentSettings = policy.Settings, Mixups = policy.Mixups,
            Economy = new CounterEconomySettings(100, 0, 0, 45, 10),
            SharedAttackWindup = CounterTrial.MatchedWindup, BurstLights = true, InterruptHeavyStartup = false
        };
        var engine = new CombatEngine(counterTrial: trial, trackHealth: false);
        var plans = new Dictionary<long, (DefenseKind? Kind, double At)>();
        var lastImpact = double.NegativeInfinity;
        var nextAttack = 0.10 + random.NextDouble() * 0.45;
        for (var frame = 0; frame < 12 * 60; frame++)
        {
            foreach (var attack in engine.Opponent.ActiveAttacks)
            {
                if (!plans.TryGetValue(attack.Id, out var plan))
                {
                    var pressure = CounterTrial.FollowUpPressure(attack.ImpactAt - lastImpact);
                    lastImpact = attack.ImpactAt;
                    DefenseKind? kind = null;
                    if (random.NextDouble() < style.ReadChance - 0.26 * pressure)
                    {
                        kind = attack.Info.IsSpecial && random.NextDouble() < 0.82 - 0.18 * pressure
                            ? (attack.Info.Parryable ? DefenseKind.Parry : DefenseKind.Dodge)
                            : (random.NextDouble() < style.ParryBias ? DefenseKind.Parry : DefenseKind.Dodge);
                    }
                    var reactionAt = attack.LaunchAt + style.Reaction + 0.035 * pressure + random.NextGaussian() * 0.030;
                    plan = (kind, Math.Max(reactionAt, attack.ImpactAt - 0.006 + random.NextGaussian() * 0.043));
                    plans.Add(attack.Id, plan);
                }
                if (plan.Kind is { } defense && engine.Time >= plan.At)
                {
                    engine.ActivateDefense(engine.Player, defense);
                    plans[attack.Id] = (null, 0);
                }
            }
            if (engine.Time >= nextAttack && engine.Player.BufferedAttackCount < 4)
            {
                var kind = trial.Counter(true, engine.Time) is { } counter && random.NextDouble() < style.CounterChance
                    ? counter : AttackKind.Light;
                engine.QueueAttack(engine.Player, kind);
                var gap = random.NextDouble() < style.ComboChance ? style.ComboGap : style.Cadence;
                nextAttack = engine.Time + gap * (0.70 + 0.60 * random.NextDouble());
            }
            engine.Tick(CombatEngine.FixedStep);
        }
        return engine;
    }
}
