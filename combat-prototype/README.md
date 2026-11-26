# Melee Rhythm Lab

A standalone Godot 4.6 .NET UI prototype for testing lane-based melee timing and stamina rules. It contains no characters, animations, ragdolls, or physical combat. The independent .NET combat core also powers a headless deterministic balance simulator.

## Run the UI

1. Install Godot 4.6 .NET and the .NET 8 SDK.
2. Open `combat-prototype/project.godot` in Godot and allow the C# project to build.
3. Run the project.

Both fighters have stamina, an attack queue, separate attack and defense readiness, and an incoming-threat chart. The Play screen counts damage taken instead of using HP or knockouts. Reset clears both damage totals. The headless balance simulator still uses health to score matches.

## Locked Timing Playtest

From the repository root, run `.\combat-prototype\PlayLockedTiming.ps1` in PowerShell. It builds and opens a paused duel using [the saved profile](profiles/locked-timing.json). Use `-GodotPath` to point to a different Godot .NET executable. Each run gets its own Exchange log; live tuning does not modify the saved profile.

The duel starts at 0.605x. Both fighters use a 233.3 ms windup for every attack, a 58.5 ms minimum gap for Lights, and a 172.2 ms minimum gap for specials. Special recovery is 80 ms on contact or 120 ms when avoided, for the attacker only. Gaps, windup, and recovery are measured on the combat clock. Attacks are free and the waiting queue holds four per fighter. This profile keeps the trained sparring AI and leaves the regular and Classic launches unchanged.

### Two-player Latency Duel

Run `.\combat-prototype\PlayLockedTiming.ps1 -LatencyDuel` on both PCs. The host enters their own Tailscale IPv4 address and clicks **Host**. The guest enters that same address and clicks **Join**. Both use UDP port 37070 by default. For two local instances, use `127.0.0.1`. Use a private Tailscale connection with your friend; there is no public matchmaking or account system.

The host clicks **Play** after the guest joins. Both use `A/D` for spacing, `O` for attack or earned follow-up, `P` for parry/hold guard, and `I` to dodge. The host can pause with `Esc` and reset with `R`. The AI is off and both sides use the saved settings. ENet RTT and variation are shown on both screens. The host records action arrival offsets and outcomes under `user://latency-duels/`. Host arrival time decides the baseline: no prediction, extra defense grace, or rollback. Swap hosts to compare who gets the advantage.

## Controls

**Counter trial** is the default single-lane timing test. Move left or right with `A/D`, press `O` for a Light, tap `P` to parry, and press `I` to dodge. Hold `P` past the parry window to block. A block costs 8 stamina against a Light or 15 against a Heavy and earns no counter. Attacks can be queued while holding guard; they launch after release and normal readiness. You can move while an attack winds up. The spacing strip shows reach; fighters are solid, and a swing only connects if in range at impact.

The top strip outlines both bodies and the reach of any swing winding up. At impact it flashes the reach and the target's position at the time of the check, even if either fighter moves before the hit or defense is confirmed.

For 300 ms after a confirmed parry, a fresh `O` buys an Unparryable Heavy. After a dodge it buys an Undodgeable Heavy. Both cost 20 and take 400 ms by default. These Heavies reach farther than Lights, but an earlier clean Light interrupts their windup. Simultaneous hits trade. `Esc` pauses, `.` steps one frame, and `R` resets.

One early `O` press can be buffered after a timed parry or dodge input, up to 140 ms before the incoming strike resolves. The game buys the Heavy after a successful defense and its stamina reward. If the swing whiffs or the defense fails, the buffered press is a normal Light instead.

Lights start at 320 ms in Counter trial. Parry and dodge still use the same 120 ms total window (±60 ms from impact). The Light startup slider adjusts the selected mode only; Classic charts retain 180 ms by default.

Counter trial starts at 0.605x speed. Press `T` to toggle between slow and full speed without resetting. Slow speed scales the combat clock, not just the falling notes: a 320 ms windup takes about 529 ms in real time. Tuning values still show simulation time; a paused manual step remains one simulation frame. Classic charts always run at full speed.

The sparring AI varies the time between swings and its defensive choices. It can miss its timing, misread a Heavy, or choose not to defend. The first attack still uses the configured initial delay so you can practice the same opening on reset.

For an assisted local playtest, launch with `COMBAT_LIVE_DIR` set to a private local directory. The session starts paused. Read `status.json` for combat-only summaries and atomically replace `opponent.json` to tune upcoming Counter trial decisions. The JSON packet contains an increasing `revision`, a short `name`, and `settings` with `cadence`, `cadenceSpread`, `reactionDelay`, `readChance`, and `timingSpread`. An optional `economy` object supplies `startingStamina`, `lightCost`, `heavyCost`, `regeneration`, and `defenseReward` for both fighters. The starting reserve applies on reset; pricing changes apply only to new attacks. Time values are simulation seconds. A bad packet leaves the last valid settings in effect. Normal launches don't use these files.

For a timing-only trial, set `lightCost` and `heavyCost` to `0`. This removes the player's queue cap and input delay, including while paused. Optional `burstLights: true` makes queued Lights from either fighter launch without waiting for previous Lights to finish. The default spacing is 20 ms in simulation time; `lightLaunchInterval: 0.04` gives 40 ms spacing. Each attack still has its own windup, impact, and defense result. Optional `playerLightStartup` overrides the player's Light windup for new inputs without changing the bot's windup or either fighter's defense window.

Use `opponentLightStartup` to adjust the bot's windup independently, and `interruptHeavyStartup: false` to let committed Heavies finish after an incoming Light lands. Without these overrides, normal Counter trial timing and interruptions are unchanged.

Set `sharedAttackWindup` to `0.28` to give both fighters' Lights and earned specials the same windup. The optional `mixups` object controls the agent's chance to chain a tight follow-up (`comboChance`), the target gap for that branch (`comboGap`), how often it spends an earned special (`counterUseChance`), and its parry preference on ambiguous reads (`parryBias`). Tight incoming bursts reduce the agent's read accuracy, not the validity of player inputs.

Set `queueLimit: 4` to allow up to four waiting attacks per fighter without counting swings already in flight. Set `specialCounterRecovery: 0.12` to use 120 ms of attacker recovery when a special is parried, dodged, or blocked.

Set `disableSpecialRecovery: true` to remove special attack recovery from both sides. In zero-cost burst mode, earned specials and Lights then share the same launch interval: either can follow the other while it is still falling. Each swing keeps its full windup and defense window. Holding guard still delays attacks.

For a commitment test, set `specialTiming: { "windup": 0.32, "contactRecovery": 0.08, "avoidedRecovery": 0.12 }` and leave `disableSpecialRecovery` off. This overrides the special windup only and gives the attacker recovery from impact: 80 ms on hit or block, 120 ms on parry, dodge or whiff. There is no extra recovery for the defender. In burst mode a special can launch behind a Light without waiting for it to land, but subsequent attacks wait until the special's recovery ends.

`lightLaunchInterval` and `specialLaunchInterval` specify how long the next Light or special must wait after the last launch. If omitted, the special gap uses the Light gap. Special recovery and guard can still delay a launch beyond these minimums.

Set `clearQueueOnSpecialDefense: true` to clear the attacker's waiting attacks when their special is parried or dodged. Attacks already in flight and the defender's queue are unaffected. Landing a special or having it blocked leaves the queue intact.

In burst mode, `opponentLookahead: 0.24` lets the bot commit its next attack up to 240 ms before its scheduled launch. Its name appears in the player's `AI QUEUE` row until launch; the launch and impact timing remain separate.

Live runs also append the full Exchange history to `exchange.jsonl` in the live directory. Entries include a session ID, round number, and simulation timestamp; the history stays on disk after reset or close.

Select **Classic charts** from the Play toolbar for the original test and keybindings below.

Play shows the player on the left and the AI on the right. Counter trial uses one lane per fighter; Classic charts use three. A short, translucent upward flash confirms an accepted attack purchase on its owner's lane. It fades in 140 ms, even while paused. In Counter trial, both charts show the other fighter's queue count and named next attack above the lane. A solid note falls on the defender's lane only once the attack launches. Tuning and Exchange have their own tabs; mouse controls remain below the charts.

| Input | Action |
|---|---|
| A / S / D | Buffer a Light attack in High / Mid / Low |
| Shift + A / S / D | Buffer a Heavy attack in that lane |
| Alt + A / S / D | Buffer a quick Unblockable special in that lane |
| Ctrl + A / S / D | Buffer a quick Undodgeable special in that lane |
| Shift + Alt + A / S / D | Buffer a heavy Unblockable special in that lane |
| Shift + Ctrl + A / S / D | Buffer a heavy Undodgeable special in that lane |
| Hold Space | Hold block |
| F / G / C / V | Parry / dodge / duck / hop |
| P | Pause or resume |
| . | Advance one fixed simulation step |
| R | Reset the round |

On-screen buttons provide the same actions. Attack inputs can be queued at 10 ms intervals, up to eight pending attacks; startup and recovery determine when they execute. The UI measures the input gap with a monotonic clock, while attack launches and resolutions remain quantized to the 60 Hz simulation. Attacks do not cancel each other automatically, and neither fighter is forced to wait for the other. Blocking, parrying, dodging, ducking, and hopping have readiness values independent of attack readiness.

## Provisional core rules

- Both fighters start at 50 stamina; the cap is 100. Light costs 10, Heavy costs 20, and baseline heavy-special Unblockable and Undodgeable attacks cost 30. Inputs reserve their cost from available stamina and spend it on launch. Unaffordable inputs never enter the queue or fire later. Stripes on the stamina bar show reservations. Regen and defense rewards replenish the unreserved portion, up to the total cap.
- Stamina continuously regenerates at 15 per second, including while attacking or defending. Taking damage grants no bonus stamina. Baseline Light startup is 180 ms and Heavy startup is 400 ms; both heavy Specials start in 700 ms.
- The default parry window is 120 ms (±60 ms), while dodge remains 180 ms (±90 ms). Holding block has no timing window.
- A Light parry grants 10 stamina, a Heavy parry 15, and countering a Special 20. Normal attacks and Specials provide no attacker stamina refund.
- The parry specialist commits to 75% of eligible reads before its usual timing and choice errors. The scripted opponent applies the same commitment setting when its script calls for a parry. Player inputs are never randomly discarded by this rule. See the [balance study](parry_balance.md).
- Ordinary hit, block, and dodge recovery is 50 ms; a normal parry leaves the attacker 120 ms and defender 50 ms recovery. Countering a Special leaves its attacker 180 ms and defender 50 ms recovery. These short values replace the former long recovery delays; the timing window still delays final resolution enough to allow late defense.
- The engine has no separate hitstun state. A Light landing before a Heavy's scheduled impact does not interrupt it unless the opt-in Light-interrupt profile is enabled.
- Defensive-action cooldowns remain separately tunable. Duck and hop remain candidate directional counters.
- Attack lanes are High, Mid, and Low. Incoming Light notes are white, Undodgeable notes blue, and Unblockable notes orange. Each fighter has a separate three-lane threat display.

All numerical values remain experimental. The UI sliders change live tuning; reset the round to apply a changed starting-stamina value to both fighters.

## Future idea: weapon and build timing

Optional, not implemented. Fast weapons could trade damage for shorter startup, giving the AI less time to read and defend. Let any lower defense success rate come from the shorter warning rather than a flat weapon-specific miss chance. Heavy weapons could trade speed for higher damage and hyperarmor during part of the committed swing: hits can still deal damage, but do not interrupt that portion of the attack. Try distinct timings between builds only after the shared timing feels good.

## Automated simulation

`Core\CombatCore.csproj` is independent of Godot. `simulation\CombatSimulation.csproj` runs fully headless and has no Godot dependency.

Run deterministic self-tests:

```powershell
dotnet run --project combat-prototype\simulation\CombatSimulation.csproj -- --self-test
```

Run the all-strategy sweep (eight strategies, all 64 pairings including self-play, twenty-eight parameter profiles, and enough distinct seeds to reach at least the requested match count):

```powershell
dotnet run --project combat-prototype\simulation\CombatSimulation.csproj -- --matches 100000 --seed 20250308
```

`--profile FreeNormalAttacks` restores free normal attacks, attacker outcome rewards, and zero regeneration for comparison with the former rules.

`--profile EarnedDefense` reproduces the earlier no-hit-reward experiment at 10/s regeneration and full AI parry commitment. `--profile Read100Hit20` reproduces the former +20 control.

The default sweep runs 129,024 matches. `--matches 384` is a smaller development run. Use `--profile FailedDefenseLockout250`, `--profile DefenseAttemptCost5`, `--profile DefenseAttemptCost10`, `--profile DefenseAttemptCost15`, `--profile LightInterrupt`, `--profile HeavyStartupNoDefense`, `--profile RhythmParryWindow180`, `--profile RhythmParryWindow90`, `--profile RhythmCounterplay90`, `--profile RhythmCounterplay60`, `--profile RhythmParryCost10`, `--profile RhythmParryRecovery150`, `--profile HybridTiming`, `--profile HybridLightInterrupt`, `--profile HybridCounterplay`, `--profile HybridCounterplayFastLight`, `--profile HybridCounterplayNarrowParry`, `--profile HybridCounterplayNarrowParryFastDodge`, `--profile HybridCounterplayParryCommitment`, `--profile HybridCounterplayResourceBalance`, `--profile HybridSifuRecovery`, `--profile HybridHeavyCommitment`, `--profile HybridHeavyDamage16`, or `--profile HybridHeavyDamage12` to devote the requested match count to one profile (about 1,563 seeds per ordered matchup at 100,000 matches). Use `--profiles <name1,name2,...>` to run a selected set. Outputs go to `combat-prototype\simulation\reports\balance-report.json` and `balance-report.md` by default; `--out <directory>` changes the destination.

`--action-failure-probability <0..1>` experimentally suppresses selected attack, defense, or block-transition inputs at the specified probability; its default is 0, preserving prior behavior. The failure rolls use independent deterministic RNG streams, so they do not consume agent decision or reaction randomness. Held-block state transitions are retried while the agent continues requesting them. To rerun the four profiles from the attached 100,096-match report with 50% action-input failures and the same 391 seeds per matchup:

```powershell
dotnet run --project combat-prototype\simulation\CombatSimulation.csproj -- --matches 100000 --seed 20250308 --profiles Baseline,Telegraph150ms,DefenseWindow240ms,SpecialCost40 --impact-uncertainty-ms 0 --action-failure-probability 0.5 --out combat-prototype\simulation\reports\action-failure-50
```

Matches end on knockout, after 12 seconds without damage (stalemate), or at the 60-second time limit. Reports include per-profile and per-matchup win rates, duration, attack and outcome distributions, stamina gained/spent/wasted and average reserves, defensive attempts/successes, timing and frame-advantage histograms, initiative changes, retaliations, repeated sequences, buffered attacks while defending, queue rejections/depth, heavy startups beaten by faster Lights, theoretically unavoidable threats/hits, and reproducible event logs for selected edge cases. A hit is classified as theoretically unavoidable only when no currently ready block/counter can reach its valid window; this is a diagnostic definition, not an automatic-hit rule.

The strategy agents are AggressiveLightSpam, HeavyBaiting, PassiveBlocking, ReactionDefense, ParryFishing, DodgeHeavy, Random, and Balanced. Reaction models vary human-like delays, timing error, prediction and occasional incorrect choices. Agents see public state and currently visible telegraphs, but not the engine's exact launch or impact timestamps. By default, the agent receives an exact impact estimate to preserve the prior evaluation baseline. Use `--impact-uncertainty-ms <n>` to add a deterministic, per-threat Gaussian estimate error; the estimate is cached for that threat, and perception randomness uses a separate per-fighter seed stream so it does not perturb decision randomness. For example, compare exact-ETA and provisional 75 ms uncertainty runs with identical seeds and separate output directories:

```powershell
dotnet run --project combat-prototype\simulation\CombatSimulation.csproj -- --matches 256 --seed 20250308 --out combat-prototype\simulation\reports\exact-eta
dotnet run --project combat-prototype\simulation\CombatSimulation.csproj -- --matches 256 --seed 20250308 --impact-uncertainty-ms 75 --out combat-prototype\simulation\reports\uncertain-eta
```

Reports record the estimate uncertainty. Replay events retain the engine's actual scheduled impact for diagnosis and separately record the estimate provided to the agent. The uncertainty value is experimental, not a balance change. The agent interface is designed for later RL/evolutionary methods.

Exploratory sweep profiles are compared against the new baseline. `FailedDefenseLockout250` adds 250 ms cross-defense recovery when a Parry or Dodge window expires without countering a threat. `DefenseAttemptCost5`, `DefenseAttemptCost10`, and `DefenseAttemptCost15` charge the same amount for each accepted Parry or Dodge activation. `LightInterrupt` enables a clean normal Light to cancel a normal Heavy before its scheduled impact. `HeavyStartupNoDefense` disables all defense inputs during normal Heavy and Heavy Special startup, while preserving attack buffering. `RhythmParryWindow180` widens the baseline parry window to 180 ms (±90 ms); `RhythmParryWindow90` narrows it to 90 ms (±45 ms). `RhythmCounterplay90` and `RhythmCounterplay60` combine 90 ms or 60 ms parry windows with the Light-interrupt experiment. `RhythmParryCost10` adds only a 10-stamina cost per accepted parry activation; `RhythmParryRecovery150` adds only 150 ms parry action recovery after a successful parry. The hybrid profiles test 150 ms telegraph lead, 240 ms parry/dodge windows, 40 stamina heavy-special cost, alternative timing, defensive recovery, and Heavy-focused damage/startup. The hybrid profile enables 500 ms quick specials (16 damage, 20 stamina), 800 ms heavy specials (30 damage Unblockable / 24 damage Undodgeable, 35 stamina), a 180 ms parry window, and 175 ms defender recovery after a successful Dodge. DodgeHeavy uses Unblockable attacks against a held block in this profile, and scripted sequences include both special speeds. `HybridLightInterrupt` tests whether a clean normal Light cancels a normal Heavy that has not reached impact; simultaneous impacts still trade. `HybridCounterplay` combines that interrupt with normal Heavy damage reduced to 12; `HybridCounterplayFastLight` instead tests a 120 ms Light startup; `HybridCounterplayNarrowParry` narrows the parry window to 120 ms total (±60 ms); `HybridCounterplayNarrowParryFastDodge` also restores successful-dodge attack recovery to 100 ms; `HybridCounterplayParryCommitment` adds 600 ms parry action recovery while retaining the 180 ms window; `HybridCounterplayResourceBalance` combines the narrow window with a 15-stamina normal Heavy cost. `HybridSifuRecovery` additionally tests 350 ms dodge and 250 ms parry action recovery. `HybridHeavyCommitment` tests an 850 ms normal Heavy startup; `HybridHeavyDamage16` and `HybridHeavyDamage12` test normal Heavy damage reduced to 16 or 12 respectively. All retain the other hybrid values.

The UI starts on the new 180/400 ms baseline timing. Select **Hybrid timing experiment** in the tuning panel to try the quick and heavy Specials and adjusted Dodge recovery; selecting **Baseline timing** restores the baseline Special and Dodge values. The quick-Special buttons are disabled in baseline mode.

Capture deterministic machine-readable replays and per-replay human timelines for the requested matchups:

```powershell
dotnet run --project combat-prototype\simulation\CombatSimulation.csproj -- --capture-replays --seed 20250308 --out combat-prototype\simulation\replays
```

The capture searches up to 256 deterministic seeds per ordered matchup and records only observed wins, losses, and draws. `replay-index.md` lists captures and outcome categories not found in that search. Each JSON event includes both fighters' state snapshots, simulation frame/time, attack queue/launch/impact/resolution, defense selection/input and reaction timing, stamina deltas/overflow, recovery, initiative, and defense-attempt links. Replay recording is optional and is checked against the unlogged result for the same seed. The simulator self-test also verifies that recordings reproduce byte-for-byte.

Run the optional evolutionary trainer separately from evaluation:

```powershell
dotnet run --project combat-prototype\simulation\CombatSimulation.csproj -- --train --generations 5 --population 10 --seed 789
```

The separate Counter trial trainer uses the actual single-lane combat core, shared 280 ms windups, and a damage counter. It trains against rusher, parry, dodge, and mixed styles, then evaluates on fresh seeds with two extra styles:

```powershell
dotnet run -c Release --project combat-prototype\simulation\CombatSimulation.csproj -- --train-counter --seed 2601126
```

It writes `trained-counter-agent.json` and `counter-agent-report.json` in `combat-prototype/simulation/reports/counter-trial` by default. The trained `settings` and `mixups` can be used in a live playtest profile.

Training uses fixed baseline opponents and training seeds. Evaluation uses disjoint unseen seeds and, when present, the previously saved candidate. It writes `evolution-report.json`, `evolution-report.md`, and `trained-balanced-agent.json`. The candidate is not automatically applied to the UI or baseline rules. Fitness combines match points, counter success, initiative reversals, stalemate avoidance, and exploit/repetition penalties rather than optimizing win rate alone. The interface is ready for future learning agents; no RL training mode is included.

To compare a trained candidate in the full strategy matrix, pass its parameter file with `--balanced-agent <path>`; this replaces only the `Balanced` agent's parameters for that run, without changing combat rules.

## Validation

```powershell
dotnet run --project combat-prototype\tests\CombatModelChecks.csproj
dotnet run --project combat-prototype\simulation\CombatSimulation.csproj -- --self-test
dotnet build combat-prototype\CombatPrototype.csproj
dotnet build combat-prototype\simulation\CombatSimulation.csproj
```
