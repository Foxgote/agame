# Melee balance: parry commitment and stamina

**Playtest default:** 75% AI parry commitment, 15 stamina/second, zero stamina for taking damage. Successful normal block +10, dodge +20, Light parry +10, Heavy parry +15; successful Special counter +20. Attack costs remain 10/20/30. No change to player parry timing or resolution.

## What we tested

Eight fixed strategies, 64 ordered matchups including mirrors, 75 ms impact estimate uncertainty and no forced input failures. Candidates shared seeds within each sweep. A separate opening duel used the same Balanced policy on both sides, except that one side withheld attacks until the first visible threat ended. Each seed was run in both seats. This measures the value of opening pressure; it does not measure human skill or fun.

The first screen crossed 60/75/90% read with +0/5/10 stamina for taking a normal hit, against the former 100% read / +20 control. At +5 the specialist won 41.1%, 48.2%, and 55.2% respectively. A fresh-seed run confirmed that +0 gave closer opening duels than +5, but too few attacks queued while defending at 10/s regen. The second screen held read at 75% and tried 15/20 regen with +0/2/5 on hit. We selected +0 and 15/s before the final holdout: it kept over two concurrent inputs per match without paying the defender for a mistake.

## Final holdout

Each row: 6,144 matrix matches and 2,048 additional mirrored opening duels. Opening results are attack-first wins / counter-first wins / draws. The specialist success column counts parries landed per parry input, not per incoming attack.

| AI read / hit reward / regen | Opening results | Specialist wins | Specialist parry success | Attacks queued while defending per match |
|---|---:|---:|---:|---:|
| 100% / +20 / 10 | 1208 / 808 / 32 | 54.6% | 65.9% | 2.53 |
| 75% / +5 / 10 | 1125 / 880 / 43 | 44.6% | 65.7% | 2.11 |
| **75% / +0 / 15** | **1109 / 903 / 36** | **48.2%** | **65.8%** | **2.01** |
| 75% / +2 / 20 | 1240 / 774 / 34 | 45.0% | 66.2% | 2.42 |

The selected rule made parries less frequent, not randomly less reliable: 3,798 landed from 5,773 attempts, versus 5,634 from 8,543 control attempts. Attacking first still has an edge. Permanent block remains a separate problem: 1,440 of 1,442 stalemates under the selected rule involved PassiveBlocking. The HeavyBaiting bot, which never defends, won only 2.5%; these results are not evidence that every fighting style is viable.

## Reproduce

From the repo root, run the following. For the preceding stages, substitute the rows below. The match count excludes the extra opening duels.

```powershell
dotnet run -c Release --project combat-prototype/simulation -- --profiles Read100Hit20,Read75Hit5,Read75Hit0Regen15,Read75Hit2Regen20 --matches 24576 --opening-pairs 1024 --seed 512849640039 --impact-uncertainty-ms 75 --out combat-prototype/simulation/reports/parry-regen-validation
```

| Stage | Profiles | Matrix matches | Opening pairs per profile | Base seed |
|---|---|---:|---:|---:|
| Read screen | Read100Hit20 plus Read{60,75,90}Hit{0,5,10} | 15360 | 128 | 20261124 |
| Read holdout | Read100Hit20, Read75Hit0, Read75Hit5 | 18432 | 768 | 132517311340 |
| Regen screen | Read75Hit{0,2,5}Regen{15,20} | 9216 | 128 | 20261124 |

Brace groups above denote combinations, not literal CLI arguments. [Full final matchup report](simulation/reports/parry-regen-validation/balance-report.md).
