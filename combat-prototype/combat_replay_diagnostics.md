# Combat replay diagnostics

**Scope:** forensic review of the existing simulator and selected baseline replays. No combat values or agent strategies were rebalanced.

**Historical note:** this report predates the `HybridTiming` experiment and quick-special variants; its four-profile counts and special-attack conclusions describe the earlier ruleset.

**Current-rules notice (2026-11-24):** the figures below are historical. For the current queue, stamina and parry rules and their evaluation, see the [parry balance study](parry_balance.md).

## Executive summary

- The 433 ms Light, 700 ms Heavy/Special, and 180 ms total parry/dodge windows are implemented as configured. Attacks are resolved after their defense grace period and the next 60 Hz simulation tick.
- There is no hitstun or hit-based attack cancellation. A Light arriving before a Heavy is an arrival-order result, not an interruption.
- The refreshed four-profile sweep completed 100,096 matches. Across all profiles, DodgeHeavy won 70.9%, ParryFishing 65.0%, ReactionDefense 59.3%, and AggressiveLightSpam 0.0%.
- DodgeHeavy's high results are not explained by an automatic “dodge then Heavy” response. It queues Heavy attacks on a fixed cadence while independently defending. In six outcome-selected DodgeHeavy/Balanced replays, 9 of 55 resolved Heavy attacks launched after a successful Dodge hit cleanly; 46 were blocked, dodged, or parried. The broader matchup aggregate still strongly favors DodgeHeavy.
- ParryFishing's results align with unusually accurate timing against a predictable, fully described public attack preview, plus the 300 ms nominal attack-readiness advantage after a parry. Stamina rewards are frequently wasted at the cap in captured matches, so they are not by themselves a sufficient explanation.
- AggressiveLightSpam never selects defense. Its attacks can connect, but it is consistently punished by agents that defend. PassiveBlocking vs DodgeHeavy stalemates because DodgeHeavy only selects blockable Heavy attacks; Unblockable specials exist, but that agent does not choose them.
- No captured event is marked as using future information. However, the agent API provides the exact scheduled impact time as soon as an attack is visible, which is an unusually precise timing aid for human-reaction experiments.
- A visibility timestamp defect was fixed during this diagnostic work: an already-launched attack could carry a `VisibleAt` value backdated by the configured telegraph lead even though no pre-launch preview had been shown. The active attack now becomes visible at launch; the actual pre-launch telegraph remains configurable.

## 1. Current implementation summary

The authoritative rules are in `Core/CombatEngine.cs`; strategy decisions are in `simulation/Agents.cs`; `SimulationRunner` runs both agents independently at a fixed 60 Hz without rendering. Simulation matches disable the engine's separate scripted UI opponent.

| Mechanic | Implemented behavior |
|---|---|
| Resources | 50 starting stamina, 100 cap, no passive regeneration; Light and Heavy are free; specials cost 30 by default. No stamina-based speed penalty. |
| Attacks | Light 12 damage / 433 ms startup; Heavy 24 damage / 700 ms; Unblockable 30 damage / 700 ms; Undodgeable 20 damage / 700 ms. |
| Attack properties | Light and Heavy are blockable, parryable, and dodgeable. Unblockable is not blockable or parryable, but is dodgeable. Undodgeable is blockable and parryable, but not dodgeable. |
| Defense | Block is held with no timing window. Parry and Dodge each use a 180 ms total window (±90 ms); Duck and Hop use the configurable directional window and default to countering High and Low attacks respectively. Defense actions cost no stamina by default. |
| Rewards | Light/Heavy rewards match the specified table. A Special gives no attacker reward; any non-clean result grants its defender the configured 20 stamina. Stamina is clamped, and overflow is recorded. |
| Recovery | Light/Heavy outcome recovery matches the requested values: blocked Light 300/200 ms, blocked Heavy 200/300 ms, Dodge 300/100 ms, Parry 400/100 ms, clean hit 200/300 ms (attacker/defender). Special recovery defaults to 0/0 and remains configurable. Recovery is applied from scheduled impact. |
| Buffering | Each fighter has an independent attack queue (maximum depth 4). Accepted inputs wait for attack readiness; queued attacks are not launched while the fighter is holding Block or inside an active timed-defense window. Queue-full inputs are rejected and logged. |
| Readiness | Attack, Block, Parry, Dodge, Duck, and Hop have separate readiness timestamps. A fighter can defend during attack startup; attacking is not restricted to alternating turns. |
| Resolution | `ScheduledImpactAtMs` is distinct from actual resolution. The engine waits until the largest active defense-window half-width has elapsed (90 ms at baseline), then resolves on a simulation tick. Simultaneous impacts are processed by impact time, then attack ID; the ordering is deterministic. |
| Interruption | There is no hitstun, hit-based interruption, or attack cancellation. A clean hit during an opponent's startup does not cancel that attack. |
| Initiative | The core also maintains a bounded initiative score, adjusted on launches and outcomes. It is a diagnostic/UI heuristic; it does not gate attacks or replace readiness timestamps. Frame advantage is computed as defender attack-ready time minus attacker attack-ready time. |

### Simulation profiles

The source defines four profiles. The first is the unmodified baseline; the others are comparison-only variants.

1. `Baseline` — default values.
2. `Telegraph150ms` — 150 ms public pre-launch telegraph.
3. `DefenseWindow240ms` — 240 ms parry and dodge windows.
4. `SpecialCost40` — 40 stamina special cost.

No profile is applied automatically to the gameplay UI.

### Refreshed population results

The refreshed sweep used seed `20250308`, 391 seeds per matchup/profile, and completed 100,096 matches (25,024 per profile). Strategy summary percentages below aggregate all four profiles; they are not baseline-only rates.

| Strategy | Wins | Losses | Draws | Win rate | Avg. damage dealt | Avg. stamina wasted at cap |
|---|---:|---:|---:|---:|---:|---:|
| DodgeHeavy | 17,749 | 3,019 | 4,256 | 70.9% | 74.5 | 464.8 |
| ParryFishing | 16,271 | 4,598 | 4,155 | 65.0% | 73.4 | 1,071.5 |
| ReactionDefense | 14,847 | 5,911 | 4,266 | 59.3% | 69.9 | 557.0 |
| Random | 12,441 | 11,995 | 588 | 49.7% | 68.1 | 309.2 |
| Balanced | 11,021 | 12,473 | 1,530 | 44.0% | 62.1 | 420.6 |
| HeavyBaiting | 4,196 | 16,679 | 4,149 | 16.8% | 37.5 | 160.8 |
| PassiveBlocking | 564 | 3,646 | 20,814 | 2.3% | 0.0 | 104.3 |
| AggressiveLightSpam | 0 | 18,768 | 6,256 | 0.0% | 34.4 | 175.5 |

Baseline-specific matchup results:

| First vs second | First wins | Second wins | Draws |
|---|---:|---:|---:|
| DodgeHeavy vs Balanced | 385 | 6 | 0 |
| Balanced vs DodgeHeavy | 10 | 378 | 3 |
| ParryFishing vs Balanced | 382 | 8 | 1 |
| Balanced vs ParryFishing | 9 | 381 | 1 |
| ReactionDefense vs Balanced | 365 | 26 | 0 |
| Balanced vs ReactionDefense | 23 | 368 | 0 |
| AggressiveLightSpam vs Balanced | 0 | 391 | 0 |
| Balanced vs AggressiveLightSpam | 391 | 0 | 0 |
| PassiveBlocking vs DodgeHeavy | 0 | 0 | 391 |
| DodgeHeavy vs PassiveBlocking | 0 | 0 | 391 |

The Baseline profile recorded 6,917 stalemates among 25,024 matches, zero average theoretically unavoidable threats, and zero average clean-hit runs without a theoretical counter opportunity. Against PassiveBlocking, Balanced selected 293 Unblockable attacks and landed 277. The complete matchup/profile tables and event examples are in the [refreshed balance report](./simulation/reports/balance-report.md).

## 2. Specification discrepancies and interpretation

1. **No hitstun/interruption is implemented.** This is explicit in the current README and engine behavior. It means “Light beat Heavy” metrics describe arrival ordering only; they do not represent interrupted Heavy attacks.
2. **The buffer is capped at four.** Queue-full inputs are rejected. The request requires immediate input registration and buffering but does not state a queue capacity; this is a concrete implementation limit worth keeping visible in future UX/tests.
3. **Special recovery defaults to zero.** Special attack recovery was not specified in the provisional table; zero is the current configurable implementation default, not a settled design decision.
4. **An initiative score exists in addition to frame advantage.** It is observational and does not create turns or alter attack legality.
5. **Defense priority is defined in code.** At resolution, Parry takes priority over Block, then Dodge, Duck, and Hop. This priority is deterministic but was not separately specified.
6. **Resolution is later than scheduled impact.** The impact timestamp remains the anchor for defense windows and recovery, but outcome/resource events occur after the defense grace interval and a fixed-step tick. Replays preserve both timestamps.
7. **Stamina cannot limit Light/Heavy attack frequency.** This is the requested provisional rule, not a simulator defect. The capped rewards can therefore be repeatedly requested and wasted without funding the free attacks.

## 3. Replay methodology and evidence

Replay captures were generated with base seed `20250308`, the `Baseline` profile, and a bounded search of 256 deterministic seeds per ordered matchup. The capture ran **3,323 candidate matches** and saved **30 observed-outcome matches**. Both fighter orders were searched. Each saved replay was rerun with logging enabled and compared against its unlogged result; the simulator self-test also checks byte-for-byte replay reproduction. Outcomes not found within the seed limit are listed rather than fabricated.

Every event records timestamp, tick/frame, actor (`Player`, `Opponent`, or `System`), target when applicable, and both fighter snapshots. The events distinguish queued, launched, scheduled-impact, and resolved attacks; record defense selection and input separately; link defense attempts to outcomes; include stamina requested/applied/wasted, recovery, initiative, and reaction information; and leave hitstun/cancellation fields empty when those mechanics do not exist. See the [replay index](./simulation/replays/replay-index.md) for all JSON logs, per-match Markdown timelines, seeds, and unobserved outcome categories.

The replay examples are selected by outcome, not sampled randomly. Their rates and timing distributions are diagnostic case evidence, not population estimates. Population results are separately reported by the full sweep.

### Matchup timelines

Each linked matchup row opens the recorded human-readable timelines; the index supplies corresponding JSON links and seed/result metadata.

| Requested matchup | Saved timelines |
|---|---|
| DodgeHeavy vs Balanced (both orders; wins, losses, draws observed) | [forward](./simulation/replays/00-dodgeheavy-vs-balanced-forward-PlayerWin-1628613955044636080.md), [reverse](./simulation/replays/03-balanced-vs-dodgeheavy-reverse-PlayerWin-2847239461180055954.md); see the index for all six outcomes. |
| ParryFishing vs Balanced (both orders; wins/losses observed, no draw found) | [forward](./simulation/replays/06-parryfishing-vs-balanced-forward-PlayerWin-5575929271435280785.md), [reverse](./simulation/replays/08-balanced-vs-parryfishing-reverse-PlayerWin-11162182713307126612.md). |
| ReactionDefense vs Balanced (both orders; wins/losses observed, no draw found) | [forward](./simulation/replays/10-reactiondefense-vs-balanced-forward-PlayerWin-11118718332778313759.md), [reverse](./simulation/replays/12-balanced-vs-reactiondefense-reverse-PlayerWin-3766122131446508002.md). |
| AggressiveLightSpam vs Balanced (both orders; only Balanced wins observed) | [forward](./simulation/replays/14-aggressivelightspam-vs-balanced-forward-OpponentWin-5004326759624913081.md), [reverse](./simulation/replays/15-balanced-vs-aggressivelightspam-reverse-PlayerWin-4032892909199930565.md). |
| AggressiveLightSpam vs AggressiveLightSpam (both orders; draws) | [forward](./simulation/replays/16-aggressivelightspam-vs-aggressivelightspam-forward-Draw-10815369770624821921.md), [reverse](./simulation/replays/17-aggressivelightspam-vs-aggressivelightspam-reverse-Draw-1026698191399422967.md). |
| PassiveBlocking vs DodgeHeavy (both orders; draws) | [forward](./simulation/replays/18-passiveblocking-vs-dodgeheavy-forward-Draw-4899519742988786775.md), [reverse](./simulation/replays/19-dodgeheavy-vs-passiveblocking-reverse-Draw-17116511967994839875.md). |
| Balanced vs Balanced (both orders; wins observed, no draw found) | [forward](./simulation/replays/20-balanced-vs-balanced-forward-PlayerWin-7825950486270035281.md), [reverse](./simulation/replays/22-balanced-vs-balanced-reverse-PlayerWin-5073906906452274288.md). |
| DodgeHeavy vs ParryFishing (both orders; wins, losses, draws observed) | [forward](./simulation/replays/24-dodgeheavy-vs-parryfishing-forward-PlayerWin-15566092634232219503.md), [reverse](./simulation/replays/27-parryfishing-vs-dodgeheavy-reverse-PlayerWin-2886652756180256584.md); see the index for all six outcomes. |

## 4. DodgeHeavy dominance

`DodgeHeavy` is not a “dodge, then Heavy” trigger. It queues its preferred Heavy on a fixed 520 ms interval and independently chooses a defense when a public threat is observed. Its defensive preference is Dodge on 78% of dodgeable threats, otherwise Block when possible. Both attack and defense decisions can be made in the same decision cycle.

In the six outcome-selected DodgeHeavy/Balanced captures:

- 60 DodgeHeavy dodges succeeded; 58 were followed by a later DodgeHeavy Heavy launch.
- Successful-Dodge-to-next-Heavy launch delay: **100–1,300 ms**, median **509.5 ms**. The delay is not a direct Dodge-trigger reward.
- Every Heavy took **700 ms** from launch to scheduled impact.
- Of the 58 post-Dodge Heavies, 55 resolved: **9 clean hits, 33 parries, 10 dodges, 3 blocks**; 3 had not resolved by match end. Thus 9/55 (16.4%) of resolved post-Dodge Heavies hit cleanly in this selected sample.
- Across all resolved DodgeHeavy Heavies in these six matches: 17 clean hits, 37 parries, 13 dodges, and 5 blocks.
- At Heavy launch, the target was already in an attack startup in **27/58** cases. Retrospectively using that attack's resolution and recovery timestamps, the Heavy's scheduled impact was **633 ms earlier to 217 ms later** than the target's earliest projected next Light impact (median 67 ms earlier). This is only an attack-arrival comparison; it is not a guaranteed hit.
- A Heavy remains blockable, parryable, and dodgeable. Separate defense readiness means the opponent can defend while attacking or recovering. No replay evidence shows a Dodge granting an extra, hidden attack turn or preventing a defensive response.

The baseline sweep is strongly favorable to DodgeHeavy in its direct Balanced matchup in both fighter orders (385–6–0 and 378–10–3 when DodgeHeavy is second). The trace evidence points to the combination of fixed Heavy cadence, the configured +200 ms attack-readiness advantage after a Dodge, and opponents sometimes already being committed to attack startup. It does **not** show an unavoidable post-Dodge Heavy. The clean-hit share in the selected traces is evidence against treating frame advantage as guaranteed damage.

## 5. ParryFishing dominance

In four outcome-selected ParryFishing/Balanced captures:

- ParryFishing made **53** Parry inputs: **52 succeeded, 1 failed**.
- Activation offset from scheduled impact was **−83.0 to +83.3 ms**, median **+17.0 ms**, inside the configured ±90 ms window for the observed range.
- Stamina applied to ParryFishing across these captures totaled **200**; cap overflow totaled **2,760**. The agent spent no stamina on Parry. These are capture totals, not per-match population rates.
- There were 32 unique first follow-up attacks after successful parries; **14 were clean hits** (43.8%). A successful parry therefore did not guarantee a successful counterattack.
- The observed attacker-ready minus defender-ready gap after Parry ranged from **−66 to +300 ms**, median **+256.3 ms**. The nominal table gives 400 ms attacker recovery and 100 ms defender recovery, a 300 ms advantage when neither fighter already has a later readiness timestamp. Existing commitments can reduce or reverse the effective timestamp gap.

The engine grants the defender +40 stamina for parrying Light/Heavy, but the selected ParryFishing traces are already heavily cap-saturated; much of the requested reward is wasted. The stronger direct mechanical incentive is the nominal 300 ms readiness gap. The agent also knows the exact public scheduled impact and deliberately schedules Parry near it. Thus the results are consistent with **both** the configured frame advantage and a highly favorable prediction/timing model. These captures do not isolate the causal contribution of each factor.

## 6. AggressiveLightSpam failure

`AggressiveLightSpam` queues a Mid Light every 120 ms and never chooses Block, Parry, Dodge, Duck, or Hop. That is a strategy limitation, not an attack-interruption rule.

The two captured AggressiveLightSpam/Balanced matches ended in Balanced wins. Across them, 11 AggressiveLightSpam attacks resolved: **4 clean hits, 5 parries, 2 dodges**. This small, outcome-selected sample shows that its offense can connect, while its inputs are systematically countered and it offers no defensive response to the opponent's attacks. The existing population report records a 0% win rate against the full strategy pool; this is not explained by Lights being automatically interrupted—attacks are never canceled by hits.

AggressiveLightSpam mirror matches draw because both sides use the same free attack at a fixed cadence and neither defends; the baseline report records all 391 such matches as draws. This is a strategy-induced repetitive exchange, not a hidden turn lock.

## 7. PassiveBlocking stalemates

In the two saved PassiveBlocking/DodgeHeavy draws, each lasted 12.00 seconds and DodgeHeavy launched 13 Heavies; all 13 were blocked. Heavy is blockable. DodgeHeavy's agent has no special-attack probability, so it never selects the available Unblockable special against a defender who holds Block continuously.

The engine does include Unblockable attacks, and they can hit PassiveBlocking. In the baseline population report, Balanced selected 293 Unblockables against PassiveBlocking and 277 connected cleanly. The draw is therefore caused by **missing counterplay in the attacking strategy**, not missing unblockable mechanics. Stalemate ends after the configured 12 seconds without damage. No balance change is justified by these draws alone.

## 8. Reaction-time and information-access audit

At baseline, a launched attack is publicly visible immediately, with its kind, lane, launch timestamp, and exact scheduled impact timestamp. For a standard attack, that exposes the deterministic startup duration (433 or 700 ms). The 150 ms telegraph profile exposes a pre-launch preview only after its configured visibility time.

Reactive agents choose a defense on the first observation (selection latency is zero in these simulated decisions), then schedule its input. Block waits for visibility plus the sampled reaction delay. Timed defenses use the later of visibility plus sampled reaction delay and scheduled impact plus timing error. The model samples reaction means of 160/200/260 ms, 45 ms reaction standard deviation, 35 ms timing-error standard deviation, 8% prediction chance, and 12% incorrect-choice chance.

Across the outcome-selected captures, incoming-defense **input** reaction-time distributions were:

| Strategy | Inputs | Time from first observation to defense input, min / median / max | Available time to scheduled impact, min / median / max |
|---|---:|---:|---:|
| Balanced | 286 | 116.7 / 450.0 / 783.3 ms | 416.3 / 432.3 / 700.0 ms |
| DodgeHeavy | 348 | 66.7 / 433.3 / 766.7 ms | 416.3 / 431.0 / 700.0 ms |
| ParryFishing | 231 | 333.3 / 700.0 / 783.3 ms | 416.3 / 696.0 / 700.0 ms |
| ReactionDefense | 83 | 66.7 / 233.3 / 750.0 ms | 416.3 / 432.3 / 699.7 ms |

These are not human-response measurements: the “reaction” interval includes the agent's deliberate wait until its known impact timer. In the saved replays, no event is marked `UsedFutureInformation=true`; predictions without an incoming threat are logged separately. The exact ETA is public rather than hidden, but it is still an unusually precise aid and makes this simulation an optimistic test of defense timing. High defense success alone is not proof of an information leak.

The previous `VisibleAt` defect affected the telegraph comparison: an already-launched attack could be returned with `VisibleAt = LaunchAt - TelegraphLead`, even though the agent had not received the pre-launch telegraph. That could make a reactive Block appear to have started earlier than the actual public visibility time. It is corrected to `VisibleAt = LaunchAt` for active attacks; an actual pre-launch preview retains its configured lead. The regression test checks both hidden-before-telegraph and active-attack visibility.

## 9. Buffered retaliation and timing

The implemented startup values and defense-window boundaries match the provisional values: Light 433 ms; Heavy/Special 700 ms; Parry/Dodge 180 ms total, inclusive of the ±90 ms boundary (with engine epsilon).

For uncomplicated readiness timestamps, recovery calculations reproduce the specified arrival-order examples:

- **Blocked Light at t=433 ms:** defender is ready at t=633; buffered Heavy lands at t=1,333. Attacker is ready at t=733; next Light lands at t=1,166. The Light arrives 167 ms earlier.
- **Parry at t=433 ms:** defender is ready at t=533; buffered Heavy lands at t=1,233. Attacker is ready at t=833; next Light lands at t=1,266. The Heavy arrives 33 ms earlier.
- A Heavy needs more than **267 ms** of launch advantage to land before a Light. At exactly 267 ms, arrival timestamps tie; the engine's deterministic attack-ID ordering resolves the tie.

These are arrival-order calculations, not guaranteed damage. For all attack types, a fighter can defend during the opposing attack's startup when the relevant defense is ready. A successful CleanHit does not cancel an in-flight attack, so simultaneous or crossing attacks can both resolve. Equal-impact attacks are processed by impact timestamp and then attack ID; both remain independent.

Buffered inputs execute no earlier than the fighter's readiness and any active action constraints allow. Recovery timestamps are extended with `max(existing readiness, impact + outcome recovery)`, so the simple tables are not sufficient when a fighter already has an outstanding attack or later readiness timestamp. The replay logs preserve both the current attack and readiness values needed to analyze those cases.

## 10. Confirmed defects vs. balance hypotheses

| Rank | Finding | Severity / confidence | Diagnosis |
|---:|---|---|---|
| 1 | Active-attack `VisibleAt` was backdated by telegraph lead. | Moderate / high | Confirmed telemetry/timing-model defect; fixed and regression-tested. It does not change baseline outcomes when telegraph lead is zero. |
| 2 | Reaction agents receive the exact scheduled impact time and schedule timed defenses around it. | High evaluation impact / high confidence | Confirmed model behavior, not hidden future access. It can overstate human parry/dodge reliability; the size of that effect on wins remains a hypothesis. |
| 3 | DodgeHeavy is strong against Balanced, but post-Dodge Heavies are often defended. | High gameplay impact / high confidence in traces | No guaranteed-hit or attack-cancellation bug found. The effect likely combines Heavy cadence, intended Dodge recovery advantage, and opponent startup commitments; causal shares remain unisolated. |
| 4 | ParryFishing succeeds often and the Parry reward is frequently wasted at cap. | High gameplay impact / high confidence in captures | The readiness advantage and exact-ETA defense model are plausible drivers. Stamina rewards may contribute but are not isolated; do not infer that stamina alone causes dominance. |
| 5 | AggressiveLightSpam never defends; PassiveBlocking never attacks. | High matchup impact / high confidence | Confirmed strategy omissions explain losses/stalemates without requiring missing mechanics. |
| 6 | Heavy startup “interruption” is absent. | High model limitation / high confidence | Confirmed and intentional in the current prototype. Any report of a Heavy being “beaten” by Light must be read as arrival ordering, not cancellation. |
| 7 | Free Light/Heavy attacks can continue after stamina rewards saturate. | Balance hypothesis / high confidence in implementation | Confirmed provisional rule. Whether it creates an undesirable economy is an experiment question, not a diagnosed defect. |

No balance change is recommended by this audit.

## 11. Recommended next experiments

Items 1–4 and 6 are evaluation/agent experiments, not recommended combat-rule changes. The only proposed balance-parameter experiment is resource saturation:

- **Evidence:** In the selected ParryFishing/Balanced captures, cap overflow was 2,760 stamina while only 200 stamina was applied.
- **Expected effect to test:** A reward/cap variant should reduce wasted reward and may alter the value of repeated successful defenses.
- **Possible unintended consequence:** Reducing defensive rewards or raising the effective cost of counters could make defense less attractive and worsen offensive snowballing. No value or production change is proposed; compare it only in a clearly isolated profile.

1. **Separate timing knowledge from reaction ability.** Compare current exact-ETA agents with agents that observe the same public launch/kind/lane but receive an uncertain impact estimate; keep the engine and balance profile fixed. This should estimate the contribution of precise ETA to ParryFishing and ReactionDefense. Too much uncertainty could make the agents unrealistically weak, so include human-readable timing distributions.
2. **Isolate initiative components.** Use paired seeds to compare the same agent decisions with only attack cadence changed, only defense success timing changed, or only post-defense readiness changed in analysis profiles. Do not mix these with a new stamina schedule.
3. **Test commitment explicitly.** Preserve attacks as non-interruptible, but report outcomes conditioned on whether the defender was already in startup, recovering, holding Block, or had a buffered response. This will quantify whether DHeavy's favorable pressure comes from commitment windows.
4. **Test defensive counterplay against PassiveBlocking.** Compare DodgeHeavy as-is with an experimental agent that selects existing Unblockable attacks. This tests strategy coverage without adding a mechanic.
5. **Test resource saturation separately.** In isolated, clearly labeled profiles, compare current cap/waste totals and special use against a reward/cost variant. Report viability, stalemates, reversals, and hit/defense distributions—not win rate alone. Do not promote any variant to baseline without review.
6. **Keep both ordered matchups and unseen seeds.** The capture set is outcome-selected; use the full matrix and fresh evaluation seeds for any conclusion about population strategy performance.

## 12. Validation

- `dotnet run --project combat-prototype\tests\CombatModelChecks.csproj` — passed.
- `dotnet run --project combat-prototype\simulation\CombatSimulation.csproj -- --self-test` — passed, including deterministic replay reproduction, enabled/disabled result equivalence, and visibility checks.
- Replay capture — 30 logs generated after 3,323 deterministic candidates; each recorded outcome was checked against its unlogged counterpart.
- `dotnet build combat-prototype\CombatPrototype.csproj --no-restore` — passed with 0 warnings and 0 errors.
- `dotnet run --project combat-prototype\simulation\CombatSimulation.csproj -- --matches 100000 --seed 20250308 --out combat-prototype\simulation\reports` — completed **100,096 matches across four profiles**; refreshed results are in [balance-report.md](./simulation/reports/balance-report.md).
