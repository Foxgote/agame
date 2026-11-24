using System.Text;
using System.Text.Json;
using CombatPrototype.Core;

namespace CombatPrototype.Simulation;

public sealed record ReplayCaptureSummary(
    int AttemptedMatches,
    int CapturedReplays,
    IReadOnlyList<string> MissingOutcomes,
    string OutputDirectory);

public static class ReplayCapture
{
    private const int SeedSearchLimit = 256;

    private static readonly (string First, string Second)[] Matchups =
    [
        ("DodgeHeavy", "Balanced"),
        ("ParryFishing", "Balanced"),
        ("ReactionDefense", "Balanced"),
        ("AggressiveLightSpam", "Balanced"),
        ("AggressiveLightSpam", "AggressiveLightSpam"),
        ("PassiveBlocking", "DodgeHeavy"),
        ("Balanced", "Balanced"),
        ("DodgeHeavy", "ParryFishing")
    ];

    private sealed record SelectedReplay(
        string First,
        string Second,
        string Order,
        int Winner,
        ulong Seed,
        MatchResult Result);

    public static async Task<ReplayCaptureSummary> RunAsync(
        string outputDirectory,
        ulong baseSeed,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(outputDirectory);
        var options = new SimulationOptions(
            RequestedMatches: 1,
            Seed: baseSeed,
            OutputDirectory: outputDirectory);
        var selected = new List<SelectedReplay>();
        var missing = new List<string>();
        var attempted = 0;
        var caseIndex = 0;

        foreach (var matchup in Matchups)
        {
            foreach (var order in new[]
                {
                    (First: matchup.First, Second: matchup.Second, Name: "forward"),
                    (First: matchup.Second, Second: matchup.First, Name: "reverse")
                })
            {
                cancellationToken.ThrowIfCancellationRequested();
                var categories = new Dictionary<int, SelectedReplay>();
                for (var searchIndex = 0; searchIndex < SeedSearchLimit && categories.Count < 3; searchIndex++)
                {
                    var seed = DeriveSeed(baseSeed, caseIndex, searchIndex);
                    var result = SimulationRunner.RunMatch(
                        ScenarioProfile.All[0], order.First, order.Second, seed, options);
                    attempted++;
                    if (!categories.ContainsKey(result.Winner))
                    {
                        categories.Add(result.Winner, new SelectedReplay(
                            order.First, order.Second, order.Name, result.Winner, seed, result));
                    }
                }

                foreach (var winner in new[] { 1, 2, 0 })
                {
                    if (categories.TryGetValue(winner, out var item))
                    {
                        selected.Add(item);
                    }
                    else
                    {
                        missing.Add($"{ItemName(order.First, order.Second, order.Name)}: " +
                            $"{WinnerLabel(winner)} was not observed in {SeedSearchLimit} fixed seeds.");
                    }
                }

                caseIndex++;
            }
        }

        var index = new StringBuilder();
        index.AppendLine("# Deterministic combat replay captures");
        index.AppendLine();
        index.AppendLine($"- Base seed: `{baseSeed}`");
        index.AppendLine("- Profile: `Baseline` (default combat values; no balance changes)");
        index.AppendLine($"- Seed search limit per ordered matchup: {SeedSearchLimit}");
        index.AppendLine($"- Candidate matches simulated: {attempted:N0}");
        index.AppendLine($"- Captured replays: {selected.Count}");
        index.AppendLine("- Every logged match was compared with the unlogged deterministic result for the same seed.");
        index.AppendLine();
        index.AppendLine("| Matchup | Order | Result | Seed | Timeline | JSON replay |");
        index.AppendLine("|---|---|---|---:|---|---|");

        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var replayNumber = 0;
        foreach (var item in selected)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var replay = new CombatReplayRecorder(
                item.Seed,
                "Baseline",
                item.First,
                item.Second,
                SimulationRunner.ReactionFor(item.Seed, 1),
                SimulationRunner.ReactionFor(item.Seed, 2),
                new CombatTuning
                {
                    AiAttackEnabled = false,
                    AiDefenseEnabled = false
                });
            var captured = SimulationRunner.RunMatch(
                ScenarioProfile.All[0], item.First, item.Second, item.Seed, options, replay);
            EnsureEquivalent(item.Result, captured, item);

            var resultName = WinnerLabel(item.Winner);
            var stem = $"{replayNumber:D2}-{Slug(item.First)}-vs-{Slug(item.Second)}-" +
                $"{item.Order}-{resultName}-{item.Seed}";
            var jsonName = $"{stem}.json";
            var timelineName = $"{stem}.md";
            var jsonPath = Path.Combine(outputDirectory, jsonName);
            var timelinePath = Path.Combine(outputDirectory, timelineName);
            await File.WriteAllTextAsync(jsonPath,
                JsonSerializer.Serialize(captured.Replay, jsonOptions), cancellationToken);
            await File.WriteAllTextAsync(timelinePath,
                CreateTimeline(captured.Replay!), cancellationToken);
            index.AppendLine($"| {item.First} vs {item.Second} | {item.Order} | {resultName} | " +
                $"`{item.Seed}` | [{timelineName}]({timelineName}) | [{jsonName}]({jsonName}) |");
            replayNumber++;
        }

        index.AppendLine();
        index.AppendLine("## Outcomes not found");
        index.AppendLine();
        if (missing.Count == 0)
        {
            index.AppendLine("All three winner categories were observed for every ordered matchup.");
        }
        else
        {
            foreach (var message in missing)
            {
                index.AppendLine($"- {message}");
            }
        }

        await File.WriteAllTextAsync(Path.Combine(outputDirectory, "replay-index.md"),
            index.ToString(), cancellationToken);
        return new ReplayCaptureSummary(attempted, selected.Count, missing, Path.GetFullPath(outputDirectory));
    }

    private static void EnsureEquivalent(MatchResult expected, MatchResult actual, SelectedReplay selected)
    {
        if (expected.Winner != actual.Winner ||
            expected.EndReason != actual.EndReason ||
            expected.Duration != actual.Duration ||
            expected.FirstHealth != actual.FirstHealth ||
            expected.SecondHealth != actual.SecondHealth ||
            expected.FirstStamina != actual.FirstStamina ||
            expected.SecondStamina != actual.SecondStamina ||
            expected.FirstAttacks != actual.FirstAttacks ||
            expected.SecondAttacks != actual.SecondAttacks ||
            !DictionaryEqual(expected.Attacks, actual.Attacks) ||
            !DictionaryEqual(expected.Outcomes, actual.Outcomes) ||
            !DictionaryEqual(expected.DefenseAttempts, actual.DefenseAttempts) ||
            !DictionaryEqual(expected.DefenseSuccesses, actual.DefenseSuccesses) ||
            !expected.EventLog.SequenceEqual(actual.EventLog))
        {
            throw new InvalidOperationException(
                $"Replay recording changed deterministic results for {selected.First} vs " +
                $"{selected.Second}, seed {selected.Seed}.");
        }
    }

    private static bool DictionaryEqual(
        IReadOnlyDictionary<string, long> first,
        IReadOnlyDictionary<string, long> second) =>
        first.Count == second.Count &&
        first.All(pair => second.TryGetValue(pair.Key, out var value) && pair.Value == value);

    private static string CreateTimeline(CombatReplay replay)
    {
        var text = new StringBuilder();
        text.AppendLine($"# {replay.PlayerAgent} vs {replay.OpponentAgent} — {replay.WinnerName}");
        text.AppendLine();
        text.AppendLine($"- Seed: `{replay.Seed}`");
        text.AppendLine($"- Profile: `{replay.Profile}`");
        text.AppendLine($"- End reason: `{replay.EndReason}`");
        text.AppendLine($"- Duration: `{replay.DurationMs:0.###} ms`");
        text.AppendLine($"- Player reaction model: `{replay.PlayerReaction}`");
        text.AppendLine($"- Opponent reaction model: `{replay.OpponentReaction}`");
        text.AppendLine();
        text.AppendLine("| Time | Frame | Event | Fighter | Attack / defense | Defense timing | Outcome / resources | Detail |");
        text.AppendLine("|---:|---:|---|---|---|---|---|---|");
        foreach (var item in replay.Events)
        {
            var attack = string.Join(" ", new[]
            {
                item.AttackKind,
                item.AttackLane,
                item.DefenseKind
            }.Where(value => !string.IsNullOrWhiteSpace(value)));
            var result = string.Join("; ", new[]
            {
                item.DefenseOutcome,
                item.Damage is { } damage ? $"damage {damage:0.###}" : null,
                item.StaminaApplied is { } applied ? $"stamina {applied:+0.###;-0.###;0}" : null,
                item.StaminaWastedAtCap is > 0 ? $"wasted {item.StaminaWastedAtCap:0.###}" : null
            }.Where(value => !string.IsNullOrWhiteSpace(value)));
            var timing = string.Join("; ", new[]
            {
                item.ReactionTimeMs is { } reaction ? $"reaction {reaction:0.###}ms" : null,
                item.SelectionLatencyMs is { } selection ? $"selection {selection:0.###}ms" : null,
                item.AvailableReactionTimeMs is { } available ? $"available {available:0.###}ms" : null,
                item.ModeledReactionDelayMs is { } modeled ? $"model delay {modeled:0.###}ms" : null,
                item.TimingErrorMs is { } error ? $"timing error {error:+0.###;-0.###;0}ms" : null,
                item.DefenseActivatedAtMs is { } activation ? $"activate {activation:0.###}ms" : null,
                item.ScheduledImpactAtMs is { } impact && item.DefenseWindowStartMs is not null
                    ? $"impact offset {ActivationOffset(item, impact):+0.###;-0.###;0}ms"
                    : null
            }.Where(value => !string.IsNullOrWhiteSpace(value)));
            var detail = item.Message ?? "";
            text.AppendLine($"| {item.TimeMs:0.###} | {item.Frame} | {item.Type} | " +
                $"{item.FighterId ?? "—"} → {item.TargetFighterId ?? "—"} | " +
                $"{Escape(attack)} | {Escape(timing)} | {Escape(result)} | {Escape(detail)} |");
        }

        text.AppendLine();
        text.AppendLine("Each event embeds both fighters' health, stamina, action/readiness state, and attack queue. " +
            "Scheduled impact and actual resolution are separate event fields. The engine has no hitstun or " +
            "attack cancellation.");
        return text.ToString();
    }

    private static ulong DeriveSeed(ulong baseSeed, int caseIndex, int searchIndex)
    {
        var value = baseSeed ^ ((ulong)(uint)caseIndex << 32) ^ (uint)searchIndex;
        value += 0x9E3779B97F4A7C15UL;
        value = (value ^ (value >> 30)) * 0xBF58476D1CE4E5B9UL;
        value = (value ^ (value >> 27)) * 0x94D049BB133111EBUL;
        return value ^ (value >> 31);
    }

    private static string WinnerLabel(int winner) => winner switch
    {
        1 => "PlayerWin",
        2 => "OpponentWin",
        _ => "Draw"
    };

    private static string Slug(string value) =>
        string.Concat(value.Select(character =>
            char.IsLetterOrDigit(character) ? char.ToLowerInvariant(character) : '-')).Trim('-');

    private static string Escape(string value) =>
        value.Replace("|", "\\|", StringComparison.Ordinal)
            .Replace("\r", " ", StringComparison.Ordinal)
            .Replace("\n", " ", StringComparison.Ordinal);

    private static double ActivationOffset(CombatReplayEvent item, double impact) =>
        (item.DefenseActivatedAtMs ?? 0.0) - impact;

    private static string ItemName(string first, string second, string order) =>
        $"{first} vs {second} ({order})";
}
