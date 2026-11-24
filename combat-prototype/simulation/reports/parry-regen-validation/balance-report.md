# Combat balance simulation

- Seed: `512849640039`
- Impact estimate uncertainty: 75ms (per-threat, cached; 0ms = exact ETA)
- Action input failure probability: 0%
- Failures suppress the selected attack/defense input; they do not alter core combat resolution. Held-block transitions are retried while the agent continues requesting them.
- Matches: 24,576 (requested 24,576)
- Seeds per profile/matchup: 96
- Agents receive public visible-threat fields and an impact estimate; exact launch/impact times remain internal. No omniscient agent is included.

## Strategy summary

| Strategy | Wins | Losses | Draws | Win rate | Dominant | Damage dealt | Hit rate | Stamina spent | Wasted at cap | Buffered in defense | Queue rejects |
|---|---:|---:|---:|---:|:---:|---:|---:|---:|---:|---:|---:|
| DodgeHeavy | 3,879 | 1,462 | 803 | 63.1% | no | 77.3 | 35.2% | 213.3 | 16.6 | 7,623 | 11,895 |
| ReactionDefense | 3,866 | 1,306 | 972 | 62.9% | no | 82.0 | 49.3% | 147.4 | 37.8 | 31,850 | 12,412 |
| Balanced | 3,181 | 2,224 | 739 | 51.8% | no | 75.3 | 43.4% | 155.7 | 5.8 | 7,236 | 33,614 |
| Random | 3,077 | 2,446 | 621 | 50.1% | no | 73.6 | 42.4% | 162.5 | 9.7 | 8,183 | 15,284 |
| ParryFishing | 2,954 | 2,287 | 903 | 48.1% | no | 75.8 | 53.9% | 123.4 | 6.7 | 849 | 11,180 |
| AggressiveLightSpam | 749 | 3,663 | 1,732 | 12.2% | no | 64.7 | 46.6% | 119.7 | 5.5 | 0 | 118,282 |
| HeavyBaiting | 450 | 4,473 | 1,221 | 7.3% | no | 55.6 | 42.2% | 111.4 | 4.8 | 0 | 12,210 |
| PassiveBlocking | 0 | 295 | 5,849 | 0.0% | no | 0.0 | 0.0% | 0.0 | 127.1 | 0 | 0 |

## Read100Hit20

AI parry read 100%; normal hit taken +20; regen 10/s.

Extra mirrored opening duels (outside strategy matrix): attack-first 1208, counter-first 808, draws 32.
Matches: 6,144; stalemates: 1,440; timeouts: 0; avg duration: 6.50s; avg initiative changes: 39.67; avg retaliations: 4.44; avg unavoidable threats: 0.00; avg clean-hit run without theoretical counter opportunity: 0.00; avg attacks buffered while defending: 2.53; avg rejected queue inputs: 8.09; predicted defenses: 0.02; incorrect defense choices: 1.21.
Action inputs failed: 0/270,787 (0.0%).

| Matchup | First wins | Second wins | Draws | Avg. duration | Hit rate | Unavoidable hits | Repeated attacks | Buffered while defending | Queue rejects | Max queue | Action failures | Frame advantage (top bins) | Defense timing (top bins) | Defense success | Attack hit mix | Outcomes |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---|---|---|---|---|
| AggressiveLightSpam vs AggressiveLightSpam | 0 | 0 | 96 | 2.55s | 90.0% | 0.00 | 16.00 | 0.00 | 0.00 | 8 | 0/3,264 (0%) | 0..+50ms:1728 |  |  | Light:1920/1728 | CleanHit:1728 |
| AggressiveLightSpam vs Balanced | 6 | 89 | 1 | 3.10s | 69.0% | 0.00 | 12.66 | 2.02 | 5.14 | 8 | 0/3,824 (0%) | 0..+50ms:1207, +150..+200ms:133, -100..-50ms:119 | +30..+60ms:118, 0..+30ms:70, -30..0ms:30 | Opponent.Dodge:89%, Opponent.Block:87%, Opponent.Parry:69%, Opponent.Duck:0%, Opponent.Hop:0% | Light:1569/1031, Heavy:163/152, Undodgeable:28/26, Unblockable:22/21 | CleanHit:1230, Dodged:103, Blocked:149, Parried:172 |
| AggressiveLightSpam vs DodgeHeavy | 0 | 96 | 0 | 3.15s | 54.4% | 0.00 | 13.39 | 1.44 | 8.49 | 6 | 0/3,775 (0%) | 0..+50ms:996, +150..+200ms:386, +200..+250ms:64 | +30..+60ms:133, +60..+90ms:114, 0..+30ms:85 | Opponent.Dodge:81%, Opponent.Block:100%, Opponent.Hop:0%, Opponent.Duck:0%, Opponent.Parry:0% | Light:1092/380, Heavy:490/480 | Dodged:407, CleanHit:860, Blocked:210 |
| AggressiveLightSpam vs HeavyBaiting | 55 | 13 | 28 | 2.71s | 91.9% | 0.00 | 9.20 | 0.00 | 1.76 | 8 | 0/2,437 (0%) | 0..+50ms:1156, +300..+350ms:133, +150..+200ms:97 |  |  | Light:1277/1170, Heavy:262/245 | CleanHit:1415 |
| AggressiveLightSpam vs ParryFishing | 6 | 90 | 0 | 3.08s | 74.4% | 0.00 | 15.95 | 0.02 | 0.00 | 8 | 0/3,093 (0%) | 0..+50ms:1308, -100..-50ms:366, +50..+100ms:46 | +30..+60ms:174, 0..+30ms:137, -60..-30ms:31 | Opponent.Parry:69%, Opponent.Hop:0%, Opponent.Duck:0% | Light:1820/1354 | CleanHit:1354, Parried:369 |
| AggressiveLightSpam vs PassiveBlocking | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 15.00 | 0.00 | 74.00 | 3 | 0/8,736 (0%) | 0..+50ms:1536 |  | Opponent.Block:100% | Light:1536/0 | Blocked:1536 |
| AggressiveLightSpam vs Random | 10 | 84 | 2 | 3.41s | 68.2% | 0.00 | 12.21 | 1.74 | 9.50 | 8 | 0/4,208 (0%) | 0..+50ms:1133, +150..+200ms:171, +50..+100ms:72 | +30..+60ms:84, 0..+30ms:66, +60..+90ms:46 | Opponent.Block:95%, Opponent.Hop:0%, Opponent.Duck:0%, Opponent.Parry:69%, Opponent.Dodge:83% | Light:1457/913, Heavy:206/196, Unblockable:51/47, Undodgeable:38/38 | Blocked:204, CleanHit:1194, Parried:103, Dodged:136 |
| AggressiveLightSpam vs ReactionDefense | 1 | 95 | 0 | 8.44s | 38.3% | 0.00 | 33.63 | 8.85 | 34.90 | 8 | 0/8,802 (0%) | 0..+50ms:3250, +50..+100ms:114, +100..+150ms:53 | 0..+30ms:3, -30..0ms:1 | Opponent.Duck:0%, Opponent.Block:100%, Opponent.Hop:0%, Opponent.Parry:80% | Light:3537/1355 | CleanHit:1355, Blocked:2061, Parried:4 |
| Balanced vs AggressiveLightSpam | 89 | 5 | 2 | 2.97s | 70.5% | 0.00 | 12.05 | 1.66 | 4.17 | 8 | 0/3,665 (0%) | 0..+50ms:1187, +150..+200ms:150, -100..-50ms:138 | +30..+60ms:103, 0..+30ms:81, +60..+90ms:33 | Player.Parry:68%, Player.Hop:0%, Player.Duck:0%, Player.Dodge:89%, Player.Block:85% | Light:1515/1020, Heavy:173/161, Unblockable:21/21, Undodgeable:27/22 | CleanHit:1224, Dodged:86, Parried:185, Blocked:110 |
| Balanced vs Balanced | 43 | 52 | 1 | 4.14s | 57.0% | 0.00 | 9.41 | 3.15 | 2.17 | 8 | 0/4,058 (0%) | 0..+50ms:1019, -100..-50ms:189, +150..+200ms:175 | +30..+60ms:166, 0..+30ms:125, -60..-30ms:69 | Opponent.Parry:60%, Player.Parry:58%, Opponent.Block:86%, Player.Dodge:80%, Opponent.Duck:0%, Opponent.Dodge:83%, Opponent.Hop:0%, Player.Block:92%, Player.Duck:0%, Player.Hop:0% | Light:1341/790, Heavy:489/263, Undodgeable:75/33, Unblockable:62/36 | Parried:299, CleanHit:1122, Blocked:218, Dodged:186 |
| Balanced vs DodgeHeavy | 12 | 84 | 0 | 5.17s | 40.5% | 0.00 | 14.36 | 3.44 | 2.50 | 7 | 0/4,553 (0%) | 0..+50ms:1052, +150..+200ms:393, +200..+250ms:199 | +30..+60ms:211, 0..+30ms:163, +60..+90ms:148 | Player.Parry:62%, Opponent.Hop:0%, Opponent.Dodge:78%, Player.Block:97%, Opponent.Block:100%, Player.Dodge:81%, Opponent.Duck:0%, Opponent.Parry:80%, Player.Duck:0%, Player.Hop:0% | Light:974/340, Heavy:1141/533, Unblockable:37/12, Undodgeable:36/1 | CleanHit:886, Parried:168, Dodged:647, Blocked:353 |
| Balanced vs HeavyBaiting | 89 | 5 | 2 | 2.83s | 72.5% | 0.00 | 4.88 | 1.05 | 0.25 | 8 | 0/2,044 (0%) | 0..+50ms:689, +150..+200ms:148, +50..+100ms:108 | +30..+60ms:49, -30..0ms:35, -60..-30ms:26 | Player.Duck:0%, Player.Block:92%, Player.Dodge:86%, Player.Parry:56%, Player.Hop:0% | Light:826/636, Heavy:508/320, Undodgeable:22/20, Unblockable:26/26 | CleanHit:1002, Blocked:69, Parried:102, Dodged:56 |
| Balanced vs ParryFishing | 43 | 53 | 0 | 4.18s | 59.0% | 0.00 | 15.90 | 0.96 | 0.33 | 8 | 0/4,062 (0%) | 0..+50ms:1091, -100..-50ms:432, +50..+100ms:164 | +30..+60ms:286, 0..+30ms:228, -60..-30ms:99 | Player.Dodge:98%, Opponent.Dodge:88%, Player.Parry:71%, Opponent.Duck:0%, Opponent.Parry:63%, Opponent.Hop:0%, Player.Hop:0%, Player.Duck:0%, Player.Block:97%, Opponent.Block:0% | Light:1868/1140, Unblockable:47/11, Heavy:239/125, Undodgeable:42/20 | CleanHit:1296, Dodged:154, Parried:601, Blocked:37 |
| Balanced vs PassiveBlocking | 14 | 0 | 82 | 12.14s | 1.0% | 0.00 | 12.14 | 0.03 | 28.59 | 2 | 0/4,281 (0%) | 0..+50ms:1410, -150..-100ms:7 |  | Opponent.Block:100%, Player.Parry:0%, Player.Duck:0%, Player.Dodge:0% | Undodgeable:7/0, Light:1316/0, Heavy:86/0, Unblockable:14/14 | Blocked:1403, CleanHit:14 |
| Balanced vs Random | 51 | 43 | 2 | 4.42s | 54.3% | 0.00 | 8.24 | 3.11 | 1.99 | 8 | 0/4,063 (0%) | 0..+50ms:946, +150..+200ms:164, +50..+100ms:132 | +30..+60ms:173, 0..+30ms:121, -30..0ms:77 | Opponent.Dodge:72%, Opponent.Parry:64%, Opponent.Duck:22%, Player.Dodge:81%, Opponent.Block:97%, Opponent.Hop:15%, Player.Parry:64%, Player.Duck:0%, Player.Block:99%, Player.Hop:0% | Light:1233/700, Heavy:529/262, Undodgeable:92/41, Unblockable:97/57 | Dodged:264, CleanHit:1060, Parried:244, Blocked:244 |
| Balanced vs ReactionDefense | 27 | 67 | 2 | 9.49s | 36.6% | 0.00 | 25.96 | 10.59 | 8.86 | 8 | 0/7,587 (0%) | 0..+50ms:2826, +50..+100ms:171, -100..-50ms:169 | +30..+60ms:164, 0..+30ms:128, -30..0ms:57 | Player.Parry:67%, Opponent.Block:99%, Opponent.Hop:0%, Opponent.Parry:63%, Player.Dodge:96%, Player.Duck:0%, Opponent.Dodge:89%, Player.Block:91%, Opponent.Duck:0%, Player.Hop:0% | Light:3076/1271, Heavy:483/40, Undodgeable:51/18, Unblockable:48/9 | CleanHit:1338, Parried:272, Blocked:1773, Dodged:165 |
| DodgeHeavy vs AggressiveLightSpam | 95 | 1 | 0 | 3.11s | 56.6% | 0.00 | 13.25 | 1.42 | 8.22 | 6 | 0/3,687 (0%) | 0..+50ms:979, +150..+200ms:389, +200..+250ms:66 | +60..+90ms:128, +30..+60ms:98, 0..+30ms:90 | Player.Dodge:80%, Player.Duck:0%, Player.Block:99%, Player.Parry:33%, Player.Hop:0% | Heavy:490/479, Light:1080/409 | Dodged:381, CleanHit:888, Blocked:194, Parried:1 |
| DodgeHeavy vs Balanced | 79 | 17 | 0 | 5.18s | 40.5% | 0.00 | 14.14 | 3.99 | 2.28 | 8 | 0/4,499 (0%) | 0..+50ms:1061, +150..+200ms:383, +200..+250ms:180 | +30..+60ms:201, 0..+30ms:148, -30..0ms:144 | Player.Dodge:88%, Opponent.Parry:62%, Player.Duck:0%, Player.Hop:0%, Opponent.Dodge:80%, Player.Block:100%, Opponent.Block:92%, Opponent.Duck:0%, Opponent.Hop:0%, Player.Parry:62% | Heavy:1195/526, Light:910/343, Unblockable:29/6, Undodgeable:28/0 | Dodged:622, CleanHit:875, Parried:181, Blocked:343 |
| DodgeHeavy vs DodgeHeavy | 38 | 55 | 3 | 7.23s | 29.2% | 0.00 | 25.21 | 0.02 | 0.00 | 2 | 0/5,364 (0%) | 0..+50ms:2588, -100..-50ms:19, +50..+100ms:5 | +30..+60ms:284, -30..0ms:277, -60..-30ms:252 | Opponent.Dodge:88%, Player.Block:100%, Player.Hop:0%, Player.Dodge:88%, Opponent.Duck:0%, Player.Duck:0%, Opponent.Block:100%, Opponent.Hop:0%, Opponent.Parry:46%, Player.Parry:62% | Heavy:2617/765 | Dodged:1340, Blocked:488, CleanHit:765, Parried:19 |
| DodgeHeavy vs HeavyBaiting | 96 | 0 | 0 | 2.79s | 56.1% | 0.00 | 5.47 | 0.35 | 0.00 | 2 | 0/1,808 (0%) | 0..+50ms:647, +150..+200ms:166, +200..+250ms:119 | +30..+60ms:69, +60..+90ms:68, 0..+30ms:59 | Player.Block:97%, Player.Dodge:89%, Player.Duck:0%, Player.Hop:0%, Player.Parry:100% | Heavy:824/542, Light:402/146 | CleanHit:688, Blocked:93, Dodged:279, Parried:11 |
| DodgeHeavy vs ParryFishing | 71 | 24 | 1 | 4.88s | 44.7% | 0.00 | 20.49 | 1.34 | 0.00 | 1 | 0/4,221 (0%) | 0..+50ms:837, +100..+150ms:382, -100..-50ms:283 | +30..+60ms:310, 0..+30ms:215, +60..+90ms:204 | Player.Dodge:96%, Opponent.Parry:64%, Opponent.Duck:0%, Player.Block:100%, Player.Parry:56%, Player.Duck:0%, Player.Hop:0%, Opponent.Hop:0%, Opponent.Dodge:88% | Heavy:907/436, Light:1370/581 | Dodged:638, Parried:413, CleanHit:1017, Blocked:91 |
| DodgeHeavy vs PassiveBlocking | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 7.00 | 0.00 | 15.00 | 0 | 0/2,319 (0%) | 0..+50ms:768 |  | Opponent.Block:100%, Player.Dodge:0%, Player.Parry:0%, Player.Duck:0% | Heavy:768/0 | Blocked:768 |
| DodgeHeavy vs Random | 81 | 15 | 0 | 6.41s | 35.7% | 0.00 | 13.99 | 5.14 | 1.27 | 8 | 0/5,005 (0%) | 0..+50ms:1064, +150..+200ms:239, +200..+250ms:231 | +30..+60ms:202, -30..0ms:154, 0..+30ms:150 | Player.Dodge:86%, Opponent.Hop:22%, Opponent.Parry:64%, Opponent.Duck:21%, Player.Block:100%, Player.Duck:0%, Opponent.Dodge:77%, Opponent.Block:95%, Player.Hop:0%, Player.Parry:43% | Heavy:1471/551, Light:631/233, Unblockable:85/23, Undodgeable:99/9 | CleanHit:816, Dodged:687, Parried:144, Blocked:507 |
| DodgeHeavy vs ReactionDefense | 6 | 90 | 0 | 11.80s | 23.7% | 0.00 | 42.16 | 15.07 | 1.38 | 8 | 0/10,130 (0%) | 0..+50ms:2593, +200..+250ms:988, +150..+200ms:489 | +30..+60ms:348, 0..+30ms:281, +60..+90ms:273 | Opponent.Block:100%, Player.Dodge:94%, Opponent.Hop:0%, Player.Hop:0%, Opponent.Duck:0%, Player.Parry:66%, Player.Block:99%, Player.Duck:0%, Opponent.Parry:57% | Heavy:2084/186, Light:2272/845 | CleanHit:1031, Blocked:2098, Dodged:1073, Parried:37 |
| HeavyBaiting vs AggressiveLightSpam | 14 | 53 | 29 | 2.71s | 92.0% | 0.00 | 9.15 | 0.00 | 1.75 | 8 | 0/2,441 (0%) | 0..+50ms:1162, +300..+350ms:135, +150..+200ms:91 |  |  | Light:1283/1171, Heavy:256/245 | CleanHit:1416 |
| HeavyBaiting vs Balanced | 4 | 91 | 1 | 2.85s | 71.2% | 0.00 | 4.97 | 1.08 | 0.31 | 8 | 0/2,073 (0%) | 0..+50ms:695, +50..+100ms:132, +150..+200ms:125 | +30..+60ms:66, 0..+30ms:49, -60..-30ms:24 | Opponent.Dodge:87%, Opponent.Parry:61%, Opponent.Duck:0%, Opponent.Block:91%, Opponent.Hop:0% | Light:824/629, Heavy:525/325, Unblockable:23/18, Undodgeable:20/19 | CleanHit:991, Dodged:67, Parried:112, Blocked:74 |
| HeavyBaiting vs DodgeHeavy | 0 | 96 | 0 | 2.90s | 55.6% | 0.00 | 5.63 | 0.70 | 0.00 | 3 | 0/1,843 (0%) | 0..+50ms:667, +150..+200ms:166, +50..+100ms:108 | +30..+60ms:67, +60..+90ms:53, 0..+30ms:50 | Opponent.Dodge:86%, Opponent.Block:100%, Opponent.Parry:100%, Opponent.Duck:0%, Opponent.Hop:0% | Light:412/147, Heavy:841/550 | Dodged:274, CleanHit:697, Blocked:113, Parried:7 |
| HeavyBaiting vs HeavyBaiting | 51 | 23 | 22 | 2.63s | 93.9% | 0.00 | 3.09 | 0.00 | 0.00 | 1 | 0/1,230 (0%) | 0..+50ms:956, +150..+200ms:103, +50..+100ms:64 |  |  | Light:705/654, Heavy:520/496 | CleanHit:1150 |
| HeavyBaiting vs ParryFishing | 5 | 90 | 1 | 3.12s | 75.6% | 0.00 | 10.29 | 0.29 | 0.00 | 2 | 0/2,161 (0%) | 0..+50ms:716, -100..-50ms:235, +100..+150ms:159 | +30..+60ms:88, -30..0ms:65, 0..+30ms:62 | Opponent.Parry:60%, Opponent.Duck:0%, Opponent.Hop:0%, Opponent.Dodge:100% | Light:1148/1002, Heavy:424/187 | CleanHit:1189, Parried:264, Dodged:3 |
| HeavyBaiting vs PassiveBlocking | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 10.82 | 0.00 | 13.65 | 1 | 0/2,784 (0%) | 0..+50ms:1378 |  | Opponent.Block:100% | Light:1226/0, Heavy:152/0 | Blocked:1378 |
| HeavyBaiting vs Random | 3 | 91 | 2 | 3.13s | 68.0% | 0.00 | 3.71 | 1.06 | 0.18 | 5 | 0/2,123 (0%) | 0..+50ms:694, +150..+200ms:128, +50..+100ms:109 | +30..+60ms:51, 0..+30ms:37, +60..+90ms:34 | Opponent.Dodge:78%, Opponent.Duck:26%, Opponent.Hop:23%, Opponent.Parry:59%, Opponent.Block:97% | Light:704/503, Heavy:546/326, Undodgeable:50/42, Unblockable:58/53 | CleanHit:924, Dodged:118, Parried:57, Blocked:116 |
| HeavyBaiting vs ReactionDefense | 0 | 96 | 0 | 4.48s | 57.8% | 0.00 | 10.95 | 6.33 | 0.03 | 8 | 0/3,392 (0%) | 0..+50ms:1415, +300..+350ms:174, +250..+300ms:106 | 0..+30ms:4, +30..+60ms:1, -60..-30ms:1 | Opponent.Block:100%, Opponent.Hop:0%, Opponent.Duck:0%, Opponent.Parry:100% | Light:1489/1080, Heavy:432/30 | CleanHit:1110, Blocked:686, Parried:6 |
| ParryFishing vs AggressiveLightSpam | 94 | 2 | 0 | 3.08s | 73.7% | 0.00 | 15.99 | 0.02 | 0.00 | 8 | 0/3,136 (0%) | 0..+50ms:1347, -100..-50ms:377, +50..+100ms:2 | +30..+60ms:166, 0..+30ms:127, -30..0ms:44 | Player.Parry:68%, Player.Duck:0%, Player.Hop:0%, Player.Dodge:100% | Light:1824/1345 | CleanHit:1345, Parried:378, Dodged:4 |
| ParryFishing vs Balanced | 52 | 44 | 0 | 4.16s | 58.9% | 0.00 | 15.30 | 1.14 | 0.35 | 8 | 0/4,094 (0%) | 0..+50ms:1082, -100..-50ms:424, +100..+150ms:145 | +30..+60ms:291, 0..+30ms:210, -30..0ms:99 | Player.Hop:0%, Player.Parry:63%, Opponent.Parry:68%, Player.Duck:0%, Opponent.Dodge:98%, Opponent.Block:89%, Opponent.Duck:0%, Opponent.Hop:0%, Player.Dodge:88% | Light:1812/1099, Heavy:262/134, Undodgeable:40/21, Unblockable:39/15 | CleanHit:1269, Parried:589, Dodged:153, Blocked:48 |
| ParryFishing vs DodgeHeavy | 18 | 77 | 1 | 4.83s | 43.3% | 0.00 | 20.26 | 1.54 | 0.00 | 1 | 0/4,181 (0%) | 0..+50ms:833, +100..+150ms:378, -100..-50ms:279 | +30..+60ms:342, +60..+90ms:196, 0..+30ms:184 | Player.Parry:62%, Opponent.Dodge:95%, Opponent.Block:100%, Opponent.Hop:0%, Player.Hop:0%, Player.Duck:0%, Player.Dodge:100%, Opponent.Duck:0%, Opponent.Parry:65% | Light:1359/534, Heavy:904/447 | CleanHit:981, Parried:401, Dodged:664, Blocked:91 |
| ParryFishing vs HeavyBaiting | 85 | 10 | 1 | 3.11s | 75.9% | 0.00 | 10.22 | 0.26 | 0.00 | 2 | 0/2,116 (0%) | 0..+50ms:761, -100..-50ms:225, +100..+150ms:150 | +30..+60ms:94, 0..+30ms:68, -30..0ms:49 | Player.Parry:63%, Player.Duck:0%, Player.Dodge:83%, Player.Hop:0% | Light:1151/1012, Heavy:429/188 | CleanHit:1200, Parried:250, Dodged:5 |
| ParryFishing vs ParryFishing | 48 | 42 | 6 | 4.65s | 53.9% | 0.00 | 24.94 | 0.00 | 0.00 | 1 | 0/4,465 (0%) | 0..+50ms:1380, -100..-50ms:903, +50..+100ms:296 | +30..+60ms:515, 0..+30ms:388, -30..0ms:149 | Opponent.Parry:68%, Player.Parry:72%, Player.Duck:0%, Player.Hop:0%, Opponent.Duck:0%, Opponent.Hop:0%, Opponent.Dodge:100%, Player.Dodge:100% | Light:2586/1394 | Parried:1167, CleanHit:1394, Dodged:25 |
| ParryFishing vs PassiveBlocking | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 15.00 | 0.00 | 19.00 | 0 | 0/3,463 (0%) | 0..+50ms:1536 |  | Opponent.Block:100%, Player.Duck:0% | Light:1536/0 | Blocked:1536 |
| ParryFishing vs Random | 60 | 35 | 1 | 4.60s | 55.6% | 0.00 | 15.07 | 1.16 | 0.03 | 7 | 0/4,192 (0%) | 0..+50ms:957, -100..-50ms:314, +100..+150ms:163 | +30..+60ms:283, 0..+30ms:183, -30..0ms:115 | Player.Parry:65%, Opponent.Parry:72%, Opponent.Hop:24%, Opponent.Dodge:97%, Opponent.Block:100%, Player.Dodge:88%, Opponent.Duck:25%, Player.Duck:0%, Player.Hop:0%, Player.Block:0% | Light:1740/1027, Heavy:281/130, Undodgeable:88/34, Unblockable:76/24 | CleanHit:1215, Parried:475, Dodged:304, Blocked:93 |
| ParryFishing vs ReactionDefense | 68 | 20 | 8 | 4.07s | 63.1% | 0.00 | 21.64 | 0.00 | 0.00 | 1 | 0/4,852 (0%) | 0..+50ms:1741, -100..-50ms:515, +50..+100ms:13 | +30..+60ms:242, 0..+30ms:170, -30..0ms:66 | Player.Duck:0%, Opponent.Block:100%, Player.Parry:69%, Opponent.Hop:0%, Opponent.Duck:0%, Opponent.Parry:87%, Player.Dodge:100%, Player.Hop:0% | Light:2270/1433 | Blocked:302, CleanHit:1433, Parried:520, Dodged:14 |
| PassiveBlocking vs AggressiveLightSpam | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 15.00 | 0.00 | 74.00 | 3 | 0/8,736 (0%) | 0..+50ms:1536 |  | Player.Block:100% | Light:1536/0 | Blocked:1536 |
| PassiveBlocking vs Balanced | 0 | 7 | 89 | 12.07s | 0.5% | 0.00 | 11.77 | 0.00 | 28.56 | 2 | 0/4,250 (0%) | 0..+50ms:1392, -150..-100ms:7 |  | Player.Block:100%, Opponent.Dodge:0%, Opponent.Parry:0%, Opponent.Duck:0% | Heavy:109/0, Light:1284/0, Unblockable:7/7, Undodgeable:7/0 | Blocked:1392, CleanHit:7 |
| PassiveBlocking vs DodgeHeavy | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 7.00 | 0.00 | 15.00 | 0 | 0/2,311 (0%) | 0..+50ms:768 |  | Player.Block:100%, Opponent.Dodge:0%, Opponent.Duck:0%, Opponent.Parry:0% | Heavy:768/0 | Blocked:768 |
| PassiveBlocking vs HeavyBaiting | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 10.49 | 0.00 | 13.90 | 1 | 0/2,784 (0%) | 0..+50ms:1354 |  | Player.Block:100% | Heavy:174/0, Light:1180/0 | Blocked:1354 |
| PassiveBlocking vs ParryFishing | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 15.00 | 0.00 | 19.00 | 0 | 0/3,461 (0%) | 0..+50ms:1536 |  | Player.Block:100%, Opponent.Parry:0%, Opponent.Duck:0% | Light:1536/0 | Blocked:1536 |
| PassiveBlocking vs PassiveBlocking | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 0.00 | 0.00 | 0.00 | 0 | 0/192 (0%) |  |  |  |  |  |
| PassiveBlocking vs Random | 0 | 21 | 75 | 12.23s | 1.6% | 0.00 | 10.23 | 0.00 | 16.09 | 2 | 0/2,994 (0%) | 0..+50ms:1312, -150..-100ms:14 |  | Player.Block:100%, Opponent.Dodge:0%, Opponent.Duck:0% | Heavy:140/0, Light:1168/0, Undodgeable:14/0, Unblockable:21/21 | Blocked:1305, CleanHit:21 |
| PassiveBlocking vs ReactionDefense | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 15.00 | 0.00 | 19.00 | 0 | 0/3,465 (0%) | 0..+50ms:1536 |  | Player.Block:100%, Opponent.Duck:0%, Opponent.Parry:0%, Opponent.Dodge:0% | Light:1536/0 | Blocked:1536 |
| Random vs AggressiveLightSpam | 71 | 21 | 4 | 3.25s | 71.3% | 0.00 | 11.53 | 1.27 | 8.54 | 8 | 0/3,933 (0%) | 0..+50ms:1117, +150..+200ms:181, +450..+500ms:54 | +30..+60ms:75, 0..+30ms:59, +60..+90ms:37 | Player.Block:92%, Player.Parry:65%, Player.Duck:0%, Player.Dodge:91%, Player.Hop:0% | Light:1402/939, Heavy:206/194, Undodgeable:41/32, Unblockable:57/51 | CleanHit:1216, Blocked:160, Parried:86, Dodged:116 |
| Random vs Balanced | 39 | 56 | 1 | 4.73s | 52.2% | 0.00 | 8.61 | 3.61 | 2.33 | 8 | 0/4,353 (0%) | 0..+50ms:981, +150..+200ms:190, +50..+100ms:148 | +30..+60ms:185, 0..+30ms:117, -30..0ms:103 | Player.Parry:65%, Player.Block:94%, Player.Duck:23%, Opponent.Duck:0%, Opponent.Block:93%, Player.Hop:22%, Opponent.Parry:62%, Player.Dodge:84%, Opponent.Hop:0%, Opponent.Dodge:79% | Light:1279/701, Heavy:562/261, Undodgeable:97/42, Unblockable:118/69 | CleanHit:1073, Blocked:261, Dodged:331, Parried:250 |
| Random vs DodgeHeavy | 16 | 79 | 1 | 5.81s | 38.1% | 0.00 | 11.79 | 4.15 | 1.38 | 6 | 0/4,451 (0%) | 0..+50ms:954, +150..+200ms:213, +200..+250ms:168 | +30..+60ms:179, +60..+90ms:131, -30..0ms:127 | Opponent.Dodge:78%, Player.Parry:58%, Player.Block:100%, Opponent.Block:100%, Player.Hop:24%, Opponent.Duck:0%, Player.Duck:18%, Opponent.Parry:43%, Player.Dodge:84%, Opponent.Hop:0% | Light:545/198, Heavy:1338/558, Unblockable:94/26, Undodgeable:93/7 | Dodged:634, Parried:89, Blocked:424, CleanHit:789 |
| Random vs HeavyBaiting | 94 | 2 | 0 | 3.14s | 67.3% | 0.00 | 3.55 | 0.93 | 0.21 | 6 | 0/2,123 (0%) | 0..+50ms:667, +150..+200ms:133, +50..+100ms:96 | +30..+60ms:63, 0..+30ms:38, +60..+90ms:33 | Player.Dodge:85%, Player.Hop:18%, Player.Parry:72%, Player.Duck:22%, Player.Block:98% | Light:667/465, Heavy:559/330, Undodgeable:70/64, Unblockable:57/52 | CleanHit:911, Dodged:121, Parried:81, Blocked:96 |
| Random vs ParryFishing | 39 | 57 | 0 | 4.65s | 54.4% | 0.00 | 15.22 | 1.50 | 0.03 | 7 | 0/4,284 (0%) | 0..+50ms:931, -100..-50ms:345, +50..+100ms:212 | +30..+60ms:282, 0..+30ms:223, -30..0ms:129 | Opponent.Parry:64%, Opponent.Dodge:81%, Player.Duck:27%, Player.Dodge:99%, Player.Parry:77%, Player.Hop:23%, Player.Block:96%, Opponent.Duck:0%, Opponent.Hop:0%, Opponent.Block:50% | Heavy:297/150, Light:1774/993, Undodgeable:75/42, Unblockable:88/30 | CleanHit:1215, Parried:498, Dodged:330, Blocked:88 |
| Random vs PassiveBlocking | 23 | 0 | 73 | 12.24s | 1.7% | 0.00 | 9.81 | 0.00 | 16.23 | 2 | 0/2,981 (0%) | 0..+50ms:1288, -150..-100ms:16 |  | Opponent.Block:100%, Player.Duck:0%, Player.Dodge:0%, Player.Parry:0% | Light:1133/0, Undodgeable:16/0, Heavy:148/0, Unblockable:23/23 | Blocked:1281, CleanHit:23 |
| Random vs Random | 44 | 49 | 3 | 4.59s | 54.0% | 0.00 | 6.30 | 3.29 | 1.17 | 8 | 0/3,746 (0%) | 0..+50ms:866, +150..+200ms:135, +50..+100ms:110 | +30..+60ms:123, -30..0ms:79, 0..+30ms:74 | Player.Dodge:78%, Opponent.Dodge:69%, Player.Hop:22%, Opponent.Hop:18%, Player.Duck:21%, Opponent.Duck:25%, Opponent.Block:98%, Player.Parry:59%, Player.Block:97%, Opponent.Parry:50% | Heavy:567/248, Light:948/537, Unblockable:139/99, Undodgeable:139/84 | Dodged:289, CleanHit:968, Blocked:261, Parried:143 |
| Random vs ReactionDefense | 17 | 79 | 0 | 8.37s | 41.3% | 0.00 | 18.90 | 10.19 | 2.54 | 8 | 0/6,475 (0%) | 0..+50ms:2088, +150..+200ms:138, -150..-100ms:113 | +30..+60ms:165, 0..+30ms:119, +60..+90ms:91 | Player.Duck:27%, Opponent.Block:99%, Opponent.Parry:61%, Player.Block:94%, Player.Parry:64%, Opponent.Dodge:82%, Player.Dodge:96%, Player.Hop:25%, Opponent.Hop:0%, Opponent.Duck:0% | Heavy:518/33, Light:2257/1136, Undodgeable:117/34, Unblockable:142/50 | CleanHit:1253, Blocked:1150, Parried:178, Dodged:343 |
| ReactionDefense vs AggressiveLightSpam | 77 | 17 | 2 | 6.36s | 49.9% | 0.00 | 26.88 | 5.98 | 21.46 | 8 | 0/6,903 (0%) | 0..+50ms:2697, +50..+100ms:53, +100..+150ms:20 | -60..-30ms:1, 0..+30ms:1, +30..+60ms:1 | Player.Block:100%, Player.Hop:0%, Player.Duck:0%, Player.Parry:60% | Light:2887/1442 | CleanHit:1442, Blocked:1327, Parried:3 |
| ReactionDefense vs Balanced | 53 | 41 | 2 | 8.31s | 39.2% | 0.00 | 25.17 | 12.65 | 6.31 | 8 | 0/8,109 (0%) | 0..+50ms:2580, -100..-50ms:208, +50..+100ms:189 | +30..+60ms:170, 0..+30ms:145, -30..0ms:74 | Player.Block:100%, Opponent.Dodge:44%, Opponent.Parry:39%, Player.Duck:0%, Opponent.Block:89%, Player.Parry:60%, Opponent.Hop:0%, Opponent.Duck:0%, Player.Hop:0%, Player.Dodge:85% | Light:2957/1290, Heavy:408/34, Unblockable:46/14, Undodgeable:48/19 | CleanHit:1357, Blocked:1485, Parried:341, Dodged:159 |
| ReactionDefense vs DodgeHeavy | 90 | 6 | 0 | 14.86s | 20.3% | 0.00 | 49.70 | 21.42 | 4.95 | 8 | 0/12,978 (0%) | 0..+50ms:3416, +200..+250ms:651, +150..+200ms:536 | +30..+60ms:315, +60..+90ms:263, 0..+30ms:239 | Opponent.Dodge:44%, Player.Block:100%, Opponent.Block:100%, Opponent.Duck:0%, Player.Hop:0%, Opponent.Hop:0%, Player.Duck:0%, Opponent.Parry:42%, Player.Parry:53%, Player.Dodge:0% | Light:2731/848, Heavy:2351/186 | Dodged:1052, Blocked:2849, CleanHit:1034, Parried:28 |
| ReactionDefense vs HeavyBaiting | 96 | 0 | 0 | 4.45s | 59.3% | 0.00 | 10.93 | 5.93 | 0.08 | 8 | 0/3,327 (0%) | 0..+50ms:1414, +300..+350ms:144, +250..+300ms:114 | +30..+60ms:3, -30..0ms:2, -60..-30ms:1 | Player.Block:100%, Player.Hop:0%, Player.Duck:0%, Player.Parry:67% | Light:1469/1102, Heavy:452/37 | CleanHit:1139, Blocked:650, Parried:6 |
| ReactionDefense vs ParryFishing | 31 | 57 | 8 | 4.00s | 64.6% | 0.00 | 21.23 | 0.00 | 0.00 | 1 | 0/4,710 (0%) | 0..+50ms:1344, -100..-50ms:449, +50..+100ms:437 | +30..+60ms:212, 0..+30ms:152, -30..0ms:50 | Opponent.Parry:67%, Player.Duck:0%, Opponent.Duck:0%, Player.Block:100%, Player.Hop:0%, Player.Parry:57%, Opponent.Hop:0%, Opponent.Dodge:100% | Light:2232/1441 | Parried:450, CleanHit:1441, Blocked:333, Dodged:6 |
| ReactionDefense vs PassiveBlocking | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 15.00 | 0.00 | 19.00 | 0 | 0/3,464 (0%) | 0..+50ms:1536 |  | Opponent.Block:100%, Player.Dodge:0%, Player.Parry:0% | Light:1536/0 | Blocked:1536 |
| ReactionDefense vs Random | 70 | 24 | 2 | 7.88s | 40.8% | 0.00 | 21.31 | 11.73 | 1.93 | 8 | 0/7,377 (0%) | 0..+50ms:2075, +150..+200ms:145, -100..-50ms:116 | +30..+60ms:155, 0..+30ms:148, +60..+90ms:81 | Opponent.Duck:14%, Player.Block:99%, Opponent.Parry:48%, Opponent.Dodge:50%, Opponent.Hop:12%, Player.Parry:57%, Opponent.Block:93%, Player.Dodge:81%, Player.Duck:0%, Player.Hop:0% | Light:2437/1157, Unblockable:115/41, Undodgeable:95/40, Heavy:469/34 | CleanHit:1272, Blocked:1199, Parried:219, Dodged:311 |
| ReactionDefense vs ReactionDefense | 43 | 38 | 15 | 3.76s | 70.0% | 0.00 | 19.85 | 0.00 | 0.00 | 1 | 0/5,349 (0%) | 0..+50ms:2072, -100..-50ms:14, +50..+100ms:12 | +30..+60ms:6, -60..-30ms:4, 0..+30ms:2 | Opponent.Block:100%, Player.Block:100%, Player.Duck:0%, Player.Hop:0%, Opponent.Duck:0%, Player.Parry:29%, Opponent.Hop:0%, Opponent.Parry:80% | Light:2100/1471 | Blocked:613, CleanHit:1471, Parried:14 |

## Read75Hit5

AI parry read 75%; normal hit taken +5; regen 10/s.

Extra mirrored opening duels (outside strategy matrix): attack-first 1125, counter-first 880, draws 43.
Matches: 6,144; stalemates: 1,441; timeouts: 0; avg duration: 6.18s; avg initiative changes: 37.30; avg retaliations: 4.67; avg unavoidable threats: 0.00; avg clean-hit run without theoretical counter opportunity: 0.00; avg attacks buffered while defending: 2.11; avg rejected queue inputs: 9.96; predicted defenses: 0.02; incorrect defense choices: 1.03.
Action inputs failed: 0/255,477 (0.0%).

| Matchup | First wins | Second wins | Draws | Avg. duration | Hit rate | Unavoidable hits | Repeated attacks | Buffered while defending | Queue rejects | Max queue | Action failures | Frame advantage (top bins) | Defense timing (top bins) | Defense success | Attack hit mix | Outcomes |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---|---|---|---|---|
| AggressiveLightSpam vs AggressiveLightSpam | 0 | 0 | 96 | 2.55s | 90.0% | 0.00 | 16.00 | 0.00 | 18.00 | 3 | 0/3,840 (0%) | 0..+50ms:1728 |  |  | Light:1920/1728 | CleanHit:1728 |
| AggressiveLightSpam vs Balanced | 3 | 92 | 1 | 2.98s | 70.8% | 0.00 | 11.82 | 1.60 | 13.25 | 7 | 0/3,794 (0%) | 0..+50ms:1161, +150..+200ms:115, -100..-50ms:111 | +30..+60ms:109, 0..+30ms:67, -30..0ms:31 | Opponent.Dodge:93%, Opponent.Block:86%, Opponent.Parry:70%, Opponent.Duck:0%, Opponent.Hop:0% | Light:1493/1009, Heavy:164/155, Undodgeable:25/24, Unblockable:18/16 | CleanHit:1204, Dodged:100, Blocked:113, Parried:162 |
| AggressiveLightSpam vs DodgeHeavy | 0 | 96 | 0 | 2.95s | 57.3% | 0.00 | 12.06 | 0.99 | 13.28 | 3 | 0/3,493 (0%) | 0..+50ms:896, +150..+200ms:366, +200..+250ms:45 | +60..+90ms:119, +30..+60ms:113, 0..+30ms:81 | Opponent.Dodge:88%, Opponent.Block:100%, Opponent.Hop:0%, Opponent.Duck:0%, Opponent.Parry:0% | Light:881/349, Heavy:565/480 | Dodged:373, CleanHit:829, Blocked:148 |
| AggressiveLightSpam vs HeavyBaiting | 55 | 13 | 28 | 2.71s | 92.0% | 0.00 | 9.20 | 0.00 | 10.66 | 3 | 0/2,623 (0%) | 0..+50ms:1156, +300..+350ms:133, +150..+200ms:97 |  |  | Light:1276/1170, Heavy:262/245 | CleanHit:1415 |
| AggressiveLightSpam vs ParryFishing | 6 | 90 | 0 | 3.08s | 79.0% | 0.00 | 15.94 | 0.00 | 12.00 | 3 | 0/3,633 (0%) | 0..+50ms:1407, -100..-50ms:284, +50..+100ms:30 | +30..+60ms:119, 0..+30ms:108, -30..0ms:36 | Opponent.Parry:69%, Opponent.Hop:0%, Opponent.Duck:0%, Opponent.Dodge:100% | Light:1818/1436 | Parried:285, CleanHit:1436, Dodged:1 |
| AggressiveLightSpam vs PassiveBlocking | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 15.00 | 0.00 | 74.00 | 3 | 0/8,736 (0%) | 0..+50ms:1536 |  | Opponent.Block:100% | Light:1536/0 | Blocked:1536 |
| AggressiveLightSpam vs Random | 4 | 91 | 1 | 3.24s | 71.9% | 0.00 | 10.68 | 1.22 | 15.16 | 5 | 0/3,961 (0%) | 0..+50ms:1062, +150..+200ms:125, +50..+100ms:71 | +30..+60ms:74, 0..+30ms:56, +60..+90ms:41 | Opponent.Block:93%, Opponent.Hop:0%, Opponent.Duck:0%, Opponent.Parry:71%, Opponent.Dodge:88% | Light:1290/872, Heavy:222/198, Unblockable:50/46, Undodgeable:37/34 | Blocked:135, CleanHit:1150, Parried:92, Dodged:120 |
| AggressiveLightSpam vs ReactionDefense | 0 | 96 | 0 | 4.54s | 61.6% | 0.00 | 19.31 | 6.16 | 21.64 | 7 | 0/5,512 (0%) | 0..+50ms:1861, +50..+100ms:112, +100..+150ms:71 | 0..+30ms:2 | Opponent.Duck:0%, Opponent.Block:100%, Opponent.Hop:0%, Opponent.Parry:67% | Light:2144/1320 | CleanHit:1320, Blocked:724, Parried:2 |
| Balanced vs AggressiveLightSpam | 94 | 2 | 0 | 2.90s | 71.3% | 0.00 | 11.50 | 1.29 | 12.66 | 7 | 0/3,715 (0%) | 0..+50ms:1155, -100..-50ms:137, +150..+200ms:136 | +30..+60ms:105, 0..+30ms:77, +60..+90ms:32 | Player.Parry:70%, Player.Hop:0%, Player.Duck:0%, Player.Dodge:93%, Player.Block:84% | Light:1462/996, Heavy:161/155, Unblockable:17/17, Undodgeable:26/20 | CleanHit:1188, Dodged:89, Parried:181, Blocked:94 |
| Balanced vs Balanced | 36 | 59 | 1 | 4.09s | 57.6% | 0.00 | 10.98 | 2.77 | 5.40 | 8 | 0/4,084 (0%) | 0..+50ms:1163, -100..-50ms:205, +150..+200ms:157 | +30..+60ms:186, 0..+30ms:133, -30..0ms:71 | Opponent.Parry:63%, Player.Parry:58%, Opponent.Block:92%, Opponent.Dodge:85%, Player.Dodge:81%, Player.Block:94%, Opponent.Duck:0%, Player.Duck:0%, Opponent.Hop:0%, Player.Hop:0% | Light:1556/902, Heavy:408/223, Unblockable:42/28, Undodgeable:35/23 | Parried:318, CleanHit:1176, Blocked:223, Dodged:194 |
| Balanced vs DodgeHeavy | 19 | 77 | 0 | 4.88s | 41.8% | 0.00 | 14.64 | 2.75 | 4.68 | 6 | 0/4,309 (0%) | 0..+50ms:1058, +150..+200ms:342, +200..+250ms:168 | +30..+60ms:202, 0..+30ms:153, +60..+90ms:150 | Player.Parry:65%, Opponent.Hop:0%, Opponent.Dodge:83%, Player.Block:95%, Opponent.Block:100%, Player.Dodge:83%, Opponent.Parry:29%, Player.Duck:0%, Opponent.Duck:0%, Player.Hop:0% | Light:989/368, Heavy:1059/497, Undodgeable:16/0, Unblockable:16/5 | CleanHit:870, Parried:161, Dodged:629, Blocked:296 |
| Balanced vs HeavyBaiting | 91 | 5 | 0 | 2.78s | 76.3% | 0.00 | 5.21 | 0.80 | 2.21 | 4 | 0/1,998 (0%) | 0..+50ms:730, +150..+200ms:128, +50..+100ms:113 | +30..+60ms:45, -30..0ms:32, -60..-30ms:26 | Player.Duck:0%, Player.Block:94%, Player.Dodge:88%, Player.Parry:55%, Player.Hop:0% | Light:877/703, Heavy:431/288, Undodgeable:16/15, Unblockable:21/20 | CleanHit:1026, Blocked:59, Parried:93, Dodged:52 |
| Balanced vs ParryFishing | 66 | 30 | 0 | 3.79s | 64.4% | 0.00 | 14.83 | 0.83 | 1.78 | 7 | 0/3,539 (0%) | 0..+50ms:1167, -100..-50ms:361, +50..+100ms:135 | +30..+60ms:220, 0..+30ms:179, -30..0ms:81 | Opponent.Parry:64%, Player.Dodge:97%, Player.Hop:0%, Opponent.Dodge:86%, Player.Parry:67%, Opponent.Hop:0%, Player.Block:97%, Opponent.Duck:0%, Player.Duck:0% | Light:1798/1179, Unblockable:24/7, Heavy:211/126, Undodgeable:14/7 | Parried:457, Dodged:136, CleanHit:1319, Blocked:36 |
| Balanced vs PassiveBlocking | 14 | 0 | 82 | 12.14s | 1.0% | 0.00 | 12.14 | 0.03 | 28.59 | 2 | 0/4,281 (0%) | 0..+50ms:1410, -150..-100ms:7 |  | Opponent.Block:100%, Player.Parry:0%, Player.Duck:0%, Player.Dodge:0% | Undodgeable:7/0, Light:1316/0, Heavy:86/0, Unblockable:14/14 | Blocked:1403, CleanHit:14 |
| Balanced vs Random | 47 | 46 | 3 | 4.49s | 55.3% | 0.00 | 9.56 | 2.67 | 4.79 | 6 | 0/4,148 (0%) | 0..+50ms:1050, +150..+200ms:170, -100..-50ms:162 | +30..+60ms:188, 0..+30ms:136, -30..0ms:75 | Opponent.Dodge:84%, Opponent.Parry:64%, Opponent.Duck:24%, Player.Dodge:84%, Opponent.Block:94%, Opponent.Hop:17%, Player.Block:97%, Player.Parry:63%, Player.Duck:0%, Player.Hop:0% | Light:1374/795, Heavy:484/249, Undodgeable:77/29, Unblockable:73/38 | Dodged:288, CleanHit:1111, Parried:255, Blocked:227 |
| Balanced vs ReactionDefense | 16 | 79 | 1 | 6.75s | 46.0% | 0.00 | 22.70 | 9.25 | 7.92 | 8 | 0/6,265 (0%) | 0..+50ms:2257, -100..-50ms:184, +50..+100ms:130 | +30..+60ms:159, 0..+30ms:123, -30..0ms:47 | Player.Parry:66%, Opponent.Block:99%, Opponent.Hop:0%, Opponent.Parry:60%, Player.Dodge:97%, Player.Hop:0%, Player.Block:89%, Opponent.Duck:0%, Player.Duck:0%, Opponent.Dodge:75% | Light:2626/1308, Heavy:254/18, Undodgeable:25/10, Unblockable:15/6 | CleanHit:1342, Parried:256, Blocked:1070, Dodged:147 |
| DodgeHeavy vs AggressiveLightSpam | 96 | 0 | 0 | 2.95s | 58.8% | 0.00 | 12.09 | 1.13 | 13.24 | 3 | 0/3,478 (0%) | 0..+50ms:904, +150..+200ms:363, +200..+250ms:44 | +60..+90ms:123, +30..+60ms:91, 0..+30ms:87 | Player.Dodge:87%, Player.Block:99%, Player.Hop:0%, Player.Duck:0%, Player.Parry:50% | Heavy:562/480, Light:886/371 | Dodged:356, CleanHit:851, Blocked:145, Parried:1 |
| DodgeHeavy vs Balanced | 73 | 23 | 0 | 5.01s | 41.8% | 0.00 | 14.61 | 3.06 | 4.89 | 8 | 0/4,342 (0%) | 0..+50ms:1083, +150..+200ms:381, +200..+250ms:170 | +30..+60ms:207, +60..+90ms:147, 0..+30ms:137 | Player.Dodge:88%, Opponent.Parry:61%, Player.Duck:0%, Player.Hop:0%, Opponent.Dodge:83%, Player.Block:100%, Opponent.Hop:0%, Opponent.Block:94%, Opponent.Duck:0%, Player.Parry:45% | Heavy:1109/498, Light:974/379, Unblockable:13/5, Undodgeable:16/1 | Dodged:596, CleanHit:883, Parried:178, Blocked:327 |
| DodgeHeavy vs DodgeHeavy | 34 | 57 | 5 | 7.48s | 29.0% | 0.00 | 25.43 | 0.01 | 0.72 | 1 | 0/5,469 (0%) | 0..+50ms:2608, -100..-50ms:20, +50..+100ms:5 | -30..0ms:284, +30..+60ms:279, -60..-30ms:255 | Opponent.Dodge:88%, Player.Block:100%, Player.Hop:0%, Player.Dodge:88%, Opponent.Duck:0%, Player.Duck:0%, Opponent.Block:100%, Opponent.Hop:0%, Player.Parry:62%, Opponent.Parry:50% | Heavy:2635/763 | Dodged:1370, Blocked:480, CleanHit:763, Parried:20 |
| DodgeHeavy vs HeavyBaiting | 96 | 0 | 0 | 2.80s | 59.3% | 0.00 | 5.35 | 0.30 | 0.86 | 1 | 0/1,795 (0%) | 0..+50ms:658, +150..+200ms:154, +200..+250ms:116 | +30..+60ms:69, +60..+90ms:67, 0..+30ms:57 | Player.Block:97%, Player.Dodge:89%, Player.Duck:0%, Player.Hop:0%, Player.Parry:100% | Heavy:791/542, Light:365/143 | CleanHit:685, Blocked:88, Dodged:268, Parried:11 |
| DodgeHeavy vs ParryFishing | 83 | 13 | 0 | 3.99s | 49.9% | 0.00 | 16.25 | 1.01 | 0.02 | 1 | 0/3,298 (0%) | 0..+50ms:728, +100..+150ms:322, +150..+200ms:256 | +30..+60ms:210, 0..+30ms:157, +60..+90ms:154 | Player.Dodge:95%, Opponent.Parry:59%, Player.Block:100%, Player.Duck:0%, Player.Parry:25%, Player.Hop:0%, Opponent.Hop:0%, Opponent.Duck:0%, Opponent.Dodge:86% | Heavy:746/465, Light:1129/470 | Dodged:524, CleanHit:935, Parried:228, Blocked:65 |
| DodgeHeavy vs PassiveBlocking | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 7.00 | 0.00 | 15.00 | 0 | 0/2,319 (0%) | 0..+50ms:768 |  | Opponent.Block:100%, Player.Dodge:0%, Player.Parry:0%, Player.Duck:0% | Heavy:768/0 | Blocked:768 |
| DodgeHeavy vs Random | 80 | 16 | 0 | 6.19s | 38.2% | 0.00 | 14.34 | 3.86 | 2.93 | 7 | 0/4,823 (0%) | 0..+50ms:1107, +150..+200ms:243, +200..+250ms:234 | +30..+60ms:197, -30..0ms:146, +60..+90ms:143 | Player.Dodge:88%, Opponent.Hop:21%, Opponent.Parry:65%, Player.Block:100%, Opponent.Duck:26%, Player.Duck:0%, Opponent.Dodge:79%, Opponent.Block:97%, Player.Hop:0%, Player.Parry:50% | Heavy:1392/548, Light:724/286, Unblockable:66/16, Undodgeable:64/7 | CleanHit:857, Dodged:690, Parried:132, Blocked:454 |
| DodgeHeavy vs ReactionDefense | 6 | 90 | 0 | 11.02s | 24.5% | 0.00 | 39.91 | 14.70 | 2.11 | 8 | 0/9,725 (0%) | 0..+50ms:2483, +200..+250ms:906, +150..+200ms:492 | +30..+60ms:338, 0..+30ms:274, +60..+90ms:269 | Opponent.Block:100%, Player.Dodge:93%, Opponent.Hop:0%, Player.Hop:0%, Player.Block:99%, Player.Duck:0%, Player.Parry:72%, Opponent.Parry:60%, Opponent.Duck:0% | Heavy:1884/167, Light:2257/847 | CleanHit:1014, Blocked:1925, Dodged:1051, Parried:33 |
| HeavyBaiting vs AggressiveLightSpam | 14 | 53 | 29 | 2.71s | 92.0% | 0.00 | 9.15 | 0.00 | 10.60 | 3 | 0/2,625 (0%) | 0..+50ms:1162, +300..+350ms:135, +150..+200ms:91 |  |  | Light:1283/1171, Heavy:256/245 | CleanHit:1416 |
| HeavyBaiting vs Balanced | 0 | 95 | 1 | 2.78s | 74.1% | 0.00 | 5.03 | 0.71 | 2.07 | 5 | 0/2,008 (0%) | 0..+50ms:711, +50..+100ms:134, +150..+200ms:113 | +30..+60ms:65, 0..+30ms:49, -60..-30ms:21 | Opponent.Dodge:88%, Opponent.Parry:60%, Opponent.Duck:0%, Opponent.Block:91%, Opponent.Hop:0% | Light:851/660, Heavy:459/304, Unblockable:15/15, Undodgeable:15/14 | CleanHit:993, Dodged:61, Parried:107, Blocked:60 |
| HeavyBaiting vs DodgeHeavy | 1 | 95 | 0 | 2.88s | 58.1% | 0.00 | 5.53 | 0.61 | 0.84 | 2 | 0/1,793 (0%) | 0..+50ms:670, +150..+200ms:155, +50..+100ms:99 | +30..+60ms:66, +60..+90ms:58, 0..+30ms:45 | Opponent.Dodge:87%, Opponent.Block:100%, Opponent.Parry:100%, Opponent.Duck:0%, Opponent.Hop:0% | Light:389/144, Heavy:791/542 | Dodged:263, CleanHit:686, Blocked:102, Parried:5 |
| HeavyBaiting vs HeavyBaiting | 42 | 26 | 28 | 2.70s | 96.5% | 0.00 | 3.19 | 0.00 | 0.31 | 1 | 0/1,252 (0%) | 0..+50ms:1005, +150..+200ms:101, +50..+100ms:50 |  |  | Light:722/701, Heavy:499/477 | CleanHit:1178 |
| HeavyBaiting vs ParryFishing | 15 | 80 | 1 | 3.08s | 79.6% | 0.00 | 9.86 | 0.18 | 0.77 | 1 | 0/1,996 (0%) | 0..+50ms:787, -100..-50ms:180, +100..+150ms:121 | +30..+60ms:77, 0..+30ms:50, -60..-30ms:44 | Opponent.Parry:65%, Opponent.Duck:0%, Opponent.Hop:0%, Opponent.Dodge:75% | Light:1165/1043, Heavy:382/189 | CleanHit:1232, Parried:201, Dodged:3 |
| HeavyBaiting vs PassiveBlocking | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 10.82 | 0.00 | 13.65 | 1 | 0/2,784 (0%) | 0..+50ms:1378 |  | Opponent.Block:100% | Light:1226/0, Heavy:152/0 | Blocked:1378 |
| HeavyBaiting vs Random | 1 | 95 | 0 | 3.08s | 70.6% | 0.00 | 3.63 | 0.84 | 1.69 | 4 | 0/2,047 (0%) | 0..+50ms:724, +50..+100ms:104, +150..+200ms:101 | +30..+60ms:52, 0..+30ms:36, +60..+90ms:34 | Opponent.Dodge:80%, Opponent.Duck:31%, Opponent.Hop:26%, Opponent.Parry:61%, Opponent.Block:97% | Light:725/524, Heavy:458/288, Undodgeable:44/41, Unblockable:43/43 | CleanHit:896, Dodged:117, Parried:51, Blocked:100 |
| HeavyBaiting vs ReactionDefense | 0 | 96 | 0 | 3.92s | 63.8% | 0.00 | 10.44 | 4.63 | 1.13 | 5 | 0/2,965 (0%) | 0..+50ms:1287, +300..+350ms:102, +250..+300ms:99 | -60..-30ms:2, 0..+30ms:2, +30..+60ms:1 | Opponent.Block:100%, Opponent.Hop:0%, Opponent.Duck:0%, Opponent.Parry:83% | Light:1433/1076, Heavy:292/24 | CleanHit:1100, Blocked:506, Parried:5 |
| ParryFishing vs AggressiveLightSpam | 90 | 6 | 0 | 3.08s | 77.8% | 0.00 | 15.94 | 0.00 | 12.00 | 3 | 0/3,627 (0%) | 0..+50ms:1416, -100..-50ms:306 | +30..+60ms:138, 0..+30ms:116, -30..0ms:31 | Player.Parry:76%, Player.Hop:0%, Player.Duck:0%, Player.Dodge:100% | Light:1818/1415 | CleanHit:1415, Parried:306, Dodged:1 |
| ParryFishing vs Balanced | 33 | 60 | 3 | 3.89s | 62.2% | 0.00 | 15.00 | 0.80 | 1.84 | 5 | 0/3,636 (0%) | 0..+50ms:1183, -100..-50ms:360, +100..+150ms:127 | +30..+60ms:232, 0..+30ms:193, -30..0ms:91 | Player.Parry:63%, Opponent.Parry:72%, Opponent.Dodge:97%, Player.Dodge:100%, Opponent.Duck:0%, Opponent.Block:91%, Player.Duck:0%, Opponent.Hop:0%, Player.Hop:0%, Player.Block:100% | Light:1839/1156, Heavy:203/125, Unblockable:27/6, Undodgeable:20/13 | CleanHit:1300, Parried:485, Dodged:143, Blocked:52 |
| ParryFishing vs DodgeHeavy | 7 | 89 | 0 | 4.07s | 48.1% | 0.00 | 16.70 | 1.13 | 0.02 | 1 | 0/3,393 (0%) | 0..+50ms:740, +100..+150ms:330, +150..+200ms:264 | +30..+60ms:270, 0..+30ms:145, +60..+90ms:143 | Player.Parry:61%, Opponent.Dodge:95%, Player.Duck:0%, Opponent.Block:100%, Opponent.Hop:0%, Player.Hop:0%, Opponent.Parry:56%, Opponent.Duck:0%, Player.Dodge:100% | Light:1156/462, Heavy:768/464 | CleanHit:926, Dodged:527, Blocked:87, Parried:255 |
| ParryFishing vs HeavyBaiting | 77 | 19 | 0 | 3.06s | 80.7% | 0.00 | 9.75 | 0.14 | 0.58 | 1 | 0/1,976 (0%) | 0..+50ms:837, -100..-50ms:169, +100..+150ms:101 | 0..+30ms:54, +30..+60ms:54, -30..0ms:44 | Player.Parry:61%, Player.Duck:0%, Player.Hop:0%, Player.Dodge:60% | Light:1187/1056, Heavy:377/206 | CleanHit:1262, Parried:184, Dodged:3 |
| ParryFishing vs ParryFishing | 49 | 39 | 8 | 3.94s | 66.8% | 0.00 | 20.90 | 0.00 | 0.00 | 1 | 0/3,351 (0%) | 0..+50ms:1366, -100..-50ms:602, +50..+100ms:229 | +30..+60ms:314, 0..+30ms:247, -30..0ms:95 | Opponent.Parry:69%, Opponent.Dodge:100%, Player.Parry:71%, Player.Duck:0%, Player.Hop:0%, Opponent.Hop:0%, Opponent.Duck:0%, Player.Dodge:100% | Light:2198/1468 | CleanHit:1468, Parried:715, Dodged:15 |
| ParryFishing vs PassiveBlocking | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 15.00 | 0.00 | 19.00 | 0 | 0/3,463 (0%) | 0..+50ms:1536 |  | Opponent.Block:100%, Player.Duck:0% | Light:1536/0 | Blocked:1536 |
| ParryFishing vs Random | 36 | 59 | 1 | 4.25s | 59.3% | 0.00 | 14.06 | 1.18 | 0.48 | 5 | 0/3,760 (0%) | 0..+50ms:1003, -100..-50ms:255, +50..+100ms:146 | +30..+60ms:231, 0..+30ms:167, -30..0ms:84 | Player.Parry:64%, Opponent.Dodge:98%, Opponent.Duck:25%, Opponent.Parry:71%, Opponent.Hop:25%, Opponent.Block:98%, Player.Duck:0%, Player.Hop:0%, Player.Dodge:83%, Player.Block:0% | Light:1668/1000, Heavy:268/163, Unblockable:55/26, Undodgeable:76/36 | CleanHit:1225, Dodged:266, Parried:379, Blocked:86 |
| ParryFishing vs ReactionDefense | 53 | 32 | 11 | 3.85s | 70.2% | 0.00 | 20.33 | 0.00 | 0.00 | 1 | 0/4,360 (0%) | 0..+50ms:1791, -100..-50ms:344, +50..+100ms:9 | +30..+60ms:150, 0..+30ms:125, -30..0ms:48 | Player.Parry:66%, Opponent.Block:100%, Opponent.Hop:0%, Player.Duck:0%, Opponent.Parry:73%, Player.Hop:0%, Opponent.Duck:0%, Player.Dodge:100% | Light:2146/1507 | Blocked:277, Parried:349, CleanHit:1507, Dodged:11 |
| PassiveBlocking vs AggressiveLightSpam | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 15.00 | 0.00 | 74.00 | 3 | 0/8,736 (0%) | 0..+50ms:1536 |  | Player.Block:100% | Light:1536/0 | Blocked:1536 |
| PassiveBlocking vs Balanced | 0 | 7 | 89 | 12.07s | 0.5% | 0.00 | 11.77 | 0.00 | 28.56 | 2 | 0/4,250 (0%) | 0..+50ms:1392, -150..-100ms:7 |  | Player.Block:100%, Opponent.Dodge:0%, Opponent.Parry:0%, Opponent.Duck:0% | Heavy:109/0, Light:1284/0, Unblockable:7/7, Undodgeable:7/0 | Blocked:1392, CleanHit:7 |
| PassiveBlocking vs DodgeHeavy | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 7.00 | 0.00 | 15.00 | 0 | 0/2,311 (0%) | 0..+50ms:768 |  | Player.Block:100%, Opponent.Dodge:0%, Opponent.Duck:0%, Opponent.Parry:0% | Heavy:768/0 | Blocked:768 |
| PassiveBlocking vs HeavyBaiting | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 10.49 | 0.00 | 13.90 | 1 | 0/2,784 (0%) | 0..+50ms:1354 |  | Player.Block:100% | Heavy:174/0, Light:1180/0 | Blocked:1354 |
| PassiveBlocking vs ParryFishing | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 15.00 | 0.00 | 19.00 | 0 | 0/3,461 (0%) | 0..+50ms:1536 |  | Player.Block:100%, Opponent.Parry:0%, Opponent.Duck:0% | Light:1536/0 | Blocked:1536 |
| PassiveBlocking vs PassiveBlocking | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 0.00 | 0.00 | 0.00 | 0 | 0/192 (0%) |  |  |  |  |  |
| PassiveBlocking vs Random | 0 | 21 | 75 | 12.23s | 1.6% | 0.00 | 10.23 | 0.00 | 16.09 | 2 | 0/2,994 (0%) | 0..+50ms:1312, -150..-100ms:14 |  | Player.Block:100%, Opponent.Dodge:0%, Opponent.Duck:0% | Heavy:140/0, Light:1168/0, Undodgeable:14/0, Unblockable:21/21 | Blocked:1305, CleanHit:21 |
| PassiveBlocking vs ReactionDefense | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 15.00 | 0.00 | 19.00 | 0 | 0/3,465 (0%) | 0..+50ms:1536 |  | Player.Block:100%, Opponent.Duck:0%, Opponent.Parry:0%, Opponent.Dodge:0% | Light:1536/0 | Blocked:1536 |
| Random vs AggressiveLightSpam | 83 | 12 | 1 | 3.14s | 74.3% | 0.00 | 10.24 | 1.00 | 14.45 | 4 | 0/3,791 (0%) | 0..+50ms:1065, +150..+200ms:158, +450..+500ms:44 | +30..+60ms:61, 0..+30ms:51, +60..+90ms:35 | Player.Block:94%, Player.Parry:62%, Player.Duck:0%, Player.Dodge:94%, Player.Hop:0% | Light:1269/893, Heavy:217/198, Undodgeable:34/29, Unblockable:52/48 | CleanHit:1168, Blocked:125, Parried:66, Dodged:104 |
| Random vs Balanced | 46 | 50 | 0 | 4.71s | 52.8% | 0.00 | 10.04 | 2.92 | 4.85 | 8 | 0/4,368 (0%) | 0..+50ms:1083, +150..+200ms:176, -100..-50ms:157 | +30..+60ms:196, 0..+30ms:126, -30..0ms:97 | Player.Parry:70%, Player.Block:97%, Player.Duck:24%, Opponent.Duck:0%, Opponent.Block:93%, Opponent.Parry:63%, Player.Dodge:87%, Opponent.Dodge:79%, Player.Hop:21%, Opponent.Hop:0% | Light:1428/783, Heavy:500/242, Undodgeable:77/41, Unblockable:89/40 | CleanHit:1106, Blocked:257, Parried:266, Dodged:344 |
| Random vs DodgeHeavy | 21 | 74 | 1 | 5.87s | 37.9% | 0.00 | 13.07 | 3.43 | 2.82 | 5 | 0/4,555 (0%) | 0..+50ms:1067, +150..+200ms:222, +200..+250ms:177 | +30..+60ms:203, +60..+90ms:135, 0..+30ms:132 | Opponent.Dodge:80%, Player.Parry:61%, Player.Block:98%, Opponent.Block:100%, Player.Hop:27%, Opponent.Duck:0%, Player.Duck:21%, Player.Dodge:86%, Opponent.Parry:36%, Opponent.Hop:0% | Light:657/230, Heavy:1324/541, Unblockable:64/21, Undodgeable:61/6 | Dodged:684, Parried:101, Blocked:409, CleanHit:798 |
| Random vs HeavyBaiting | 96 | 0 | 0 | 3.03s | 72.2% | 0.00 | 3.51 | 0.58 | 1.88 | 4 | 0/1,988 (0%) | 0..+50ms:693, +150..+200ms:112, +50..+100ms:100 | +30..+60ms:51, 0..+30ms:34, +60..+90ms:25 | Player.Dodge:88%, Player.Hop:19%, Player.Parry:68%, Player.Duck:22%, Player.Block:100% | Light:692/515, Heavy:470/304, Undodgeable:52/49, Unblockable:37/35 | CleanHit:903, Dodged:99, Parried:65, Blocked:79 |
| Random vs ParryFishing | 63 | 33 | 0 | 4.21s | 59.3% | 0.00 | 13.91 | 1.07 | 0.39 | 5 | 0/3,721 (0%) | 0..+50ms:955, -100..-50ms:246, +50..+100ms:205 | +30..+60ms:207, 0..+30ms:191, -30..0ms:109 | Player.Hop:23%, Opponent.Hop:0%, Opponent.Parry:61%, Player.Duck:26%, Player.Parry:72%, Player.Dodge:99%, Player.Block:99%, Opponent.Dodge:90%, Opponent.Duck:0% | Heavy:273/183, Light:1662/983, Undodgeable:61/39, Unblockable:67/18 | CleanHit:1223, Dodged:291, Parried:366, Blocked:71 |
| Random vs PassiveBlocking | 23 | 0 | 73 | 12.24s | 1.7% | 0.00 | 9.81 | 0.00 | 16.23 | 2 | 0/2,981 (0%) | 0..+50ms:1288, -150..-100ms:16 |  | Opponent.Block:100%, Player.Duck:0%, Player.Dodge:0%, Player.Parry:0% | Light:1133/0, Undodgeable:16/0, Heavy:148/0, Unblockable:23/23 | Blocked:1281, CleanHit:23 |
| Random vs Random | 44 | 51 | 1 | 4.84s | 53.6% | 0.00 | 7.63 | 2.80 | 3.33 | 7 | 0/3,950 (0%) | 0..+50ms:1024, +50..+100ms:131, +150..+200ms:113 | +30..+60ms:156, 0..+30ms:99, +60..+90ms:87 | Player.Dodge:83%, Opponent.Dodge:74%, Player.Hop:21%, Opponent.Hop:23%, Player.Duck:21%, Opponent.Duck:24%, Player.Parry:58%, Player.Block:94%, Opponent.Parry:58%, Opponent.Block:97% | Heavy:548/248, Light:1125/623, Unblockable:100/70, Undodgeable:112/70 | Dodged:326, CleanHit:1011, Parried:163, Blocked:270 |
| Random vs ReactionDefense | 17 | 79 | 0 | 7.03s | 44.7% | 0.00 | 19.17 | 9.46 | 2.91 | 8 | 0/6,116 (0%) | 0..+50ms:1933, +50..+100ms:133, +150..+200ms:115 | +30..+60ms:177, 0..+30ms:112, +60..+90ms:73 | Player.Duck:26%, Opponent.Block:99%, Opponent.Parry:58%, Player.Block:97%, Player.Hop:22%, Player.Parry:71%, Opponent.Dodge:80%, Opponent.Duck:0%, Player.Dodge:95%, Opponent.Hop:0% | Heavy:407/33, Light:2275/1171, Undodgeable:72/28, Unblockable:70/29 | CleanHit:1261, Blocked:965, Parried:173, Dodged:303 |
| ReactionDefense vs AggressiveLightSpam | 79 | 17 | 0 | 4.02s | 68.9% | 0.00 | 17.99 | 4.27 | 18.31 | 7 | 0/5,111 (0%) | 0..+50ms:1818, +50..+100ms:58, +100..+150ms:42 | +30..+60ms:1 | Player.Block:100%, Player.Hop:0%, Player.Duck:0%, Player.Parry:25% | Light:2012/1387 | CleanHit:1387, Blocked:531, Parried:1 |
| ReactionDefense vs Balanced | 61 | 33 | 2 | 6.68s | 45.2% | 0.00 | 23.19 | 9.61 | 6.64 | 8 | 0/6,921 (0%) | 0..+50ms:2238, -100..-50ms:191, +50..+100ms:172 | +30..+60ms:148, 0..+30ms:135, -30..0ms:57 | Player.Block:100%, Opponent.Dodge:57%, Opponent.Parry:42%, Player.Duck:0%, Opponent.Block:85%, Player.Hop:0%, Opponent.Duck:0%, Opponent.Hop:0%, Player.Parry:58%, Player.Dodge:80% | Light:2676/1308, Heavy:267/25, Undodgeable:27/12, Unblockable:15/3 | CleanHit:1348, Blocked:1098, Parried:286, Dodged:154 |
| ReactionDefense vs DodgeHeavy | 89 | 7 | 0 | 14.16s | 20.7% | 0.00 | 48.11 | 20.61 | 6.28 | 8 | 0/12,638 (0%) | 0..+50ms:3303, +200..+250ms:634, +150..+200ms:523 | +30..+60ms:332, 0..+30ms:251, +60..+90ms:246 | Opponent.Dodge:47%, Player.Block:100%, Opponent.Block:99%, Opponent.Duck:0%, Opponent.Hop:0%, Player.Duck:0%, Player.Dodge:0%, Player.Hop:0%, Opponent.Parry:36%, Player.Parry:46% | Light:2749/839, Heavy:2186/184 | Dodged:1086, Blocked:2680, CleanHit:1023, Parried:22 |
| ReactionDefense vs HeavyBaiting | 96 | 0 | 0 | 3.85s | 65.8% | 0.00 | 10.25 | 4.24 | 1.11 | 5 | 0/2,877 (0%) | 0..+50ms:1259, +250..+300ms:102, +300..+350ms:94 | -30..0ms:2, +30..+60ms:2, 0..+30ms:1 | Player.Block:100%, Player.Hop:0%, Player.Duck:0%, Player.Parry:86% | Light:1388/1090, Heavy:313/30 | CleanHit:1120, Blocked:460, Parried:6 |
| ReactionDefense vs ParryFishing | 37 | 55 | 4 | 3.80s | 69.4% | 0.00 | 20.06 | 0.00 | 0.00 | 1 | 0/4,274 (0%) | 0..+50ms:1454, -100..-50ms:337, +50..+100ms:327 | +30..+60ms:149, 0..+30ms:126, -30..0ms:44 | Opponent.Parry:68%, Player.Duck:0%, Player.Block:100%, Opponent.Hop:0%, Player.Parry:67%, Opponent.Dodge:100%, Player.Hop:0%, Opponent.Duck:0% | Light:2120/1471 | Parried:339, CleanHit:1471, Blocked:300, Dodged:8 |
| ReactionDefense vs PassiveBlocking | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 15.00 | 0.00 | 19.00 | 0 | 0/3,464 (0%) | 0..+50ms:1536 |  | Opponent.Block:100%, Player.Dodge:0%, Player.Parry:0% | Light:1536/0 | Blocked:1536 |
| ReactionDefense vs Random | 71 | 24 | 1 | 7.24s | 42.3% | 0.00 | 20.56 | 10.41 | 2.98 | 8 | 0/6,964 (0%) | 0..+50ms:2008, +50..+100ms:127, +150..+200ms:119 | +30..+60ms:154, 0..+30ms:122, +60..+90ms:84 | Opponent.Duck:16%, Player.Block:100%, Opponent.Parry:47%, Opponent.Dodge:55%, Opponent.Hop:16%, Player.Parry:66%, Opponent.Block:94%, Player.Dodge:85%, Player.Duck:0%, Player.Hop:0% | Light:2379/1160, Unblockable:94/28, Undodgeable:65/19, Heavy:409/39 | CleanHit:1246, Blocked:1084, Parried:186, Dodged:320 |
| ReactionDefense vs ReactionDefense | 43 | 38 | 15 | 3.76s | 70.0% | 0.00 | 19.85 | 0.00 | 0.00 | 1 | 0/5,349 (0%) | 0..+50ms:2072, -100..-50ms:14, +50..+100ms:12 | +30..+60ms:6, -60..-30ms:4, 0..+30ms:2 | Opponent.Block:100%, Player.Block:100%, Player.Duck:0%, Player.Hop:0%, Opponent.Duck:0%, Player.Parry:29%, Opponent.Hop:0%, Opponent.Parry:80% | Light:2100/1471 | Blocked:613, CleanHit:1471, Parried:14 |

## Read75Hit0Regen15

AI parry read 75%; normal hit taken +0; regen 15/s.

Extra mirrored opening duels (outside strategy matrix): attack-first 1109, counter-first 903, draws 36.
Matches: 6,144; stalemates: 1,442; timeouts: 0; avg duration: 6.23s; avg initiative changes: 38.99; avg retaliations: 4.59; avg unavoidable threats: 0.00; avg clean-hit run without theoretical counter opportunity: 0.00; avg attacks buffered while defending: 2.01; avg rejected queue inputs: 9.69; predicted defenses: 0.02; incorrect defense choices: 1.01.
Action inputs failed: 0/256,084 (0.0%).

| Matchup | First wins | Second wins | Draws | Avg. duration | Hit rate | Unavoidable hits | Repeated attacks | Buffered while defending | Queue rejects | Max queue | Action failures | Frame advantage (top bins) | Defense timing (top bins) | Defense success | Attack hit mix | Outcomes |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---|---|---|---|---|
| AggressiveLightSpam vs AggressiveLightSpam | 0 | 0 | 96 | 2.95s | 100.0% | 0.00 | 16.00 | 0.00 | 28.00 | 3 | 0/4,416 (0%) | 0..+50ms:1728 |  |  | Light:1728/1728 | CleanHit:1728 |
| AggressiveLightSpam vs Balanced | 4 | 92 | 0 | 2.95s | 72.4% | 0.00 | 11.15 | 1.34 | 15.10 | 7 | 0/3,713 (0%) | 0..+50ms:1113, -100..-50ms:104, +50..+100ms:94 | +30..+60ms:102, 0..+30ms:61, +60..+90ms:29 | Opponent.Dodge:95%, Opponent.Block:88%, Opponent.Parry:70%, Opponent.Hop:0%, Opponent.Duck:0% | Light:1412/980, Heavy:159/150, Unblockable:15/14, Undodgeable:21/19 | CleanHit:1163, Dodged:91, Blocked:99, Parried:149 |
| AggressiveLightSpam vs DodgeHeavy | 1 | 95 | 0 | 2.93s | 56.5% | 0.00 | 11.45 | 0.81 | 13.28 | 3 | 0/3,415 (0%) | 0..+50ms:845, +150..+200ms:316, +100..+150ms:58 | +30..+60ms:108, +60..+90ms:103, 0..+30ms:82 | Opponent.Dodge:88%, Opponent.Block:100%, Opponent.Hop:0%, Opponent.Duck:0%, Opponent.Parry:0% | Light:869/332, Heavy:566/479 | Dodged:350, CleanHit:811, Blocked:130 |
| AggressiveLightSpam vs HeavyBaiting | 96 | 0 | 0 | 2.95s | 100.0% | 0.00 | 9.71 | 0.00 | 15.19 | 3 | 0/2,880 (0%) | 0..+50ms:1202, +300..+350ms:136, +150..+200ms:59 |  |  | Light:1217/1217, Heavy:205/205 | CleanHit:1422 |
| AggressiveLightSpam vs ParryFishing | 7 | 89 | 0 | 3.08s | 83.6% | 0.00 | 15.93 | 0.00 | 14.93 | 3 | 0/3,626 (0%) | 0..+50ms:1415, -100..-50ms:275, +50..+100ms:30 | +30..+60ms:116, 0..+30ms:105, -30..0ms:32 | Opponent.Parry:68%, Opponent.Hop:0%, Opponent.Duck:0%, Opponent.Dodge:100% | Light:1728/1444 | Parried:276, CleanHit:1444, Dodged:1 |
| AggressiveLightSpam vs PassiveBlocking | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 21.00 | 0.00 | 68.00 | 3 | 0/8,736 (0%) | 0..+50ms:2112 |  | Opponent.Block:100% | Light:2112/0 | Blocked:2112 |
| AggressiveLightSpam vs Random | 4 | 92 | 0 | 3.19s | 72.6% | 0.00 | 9.98 | 1.06 | 15.81 | 4 | 0/3,868 (0%) | 0..+50ms:995, +150..+200ms:122, +50..+100ms:91 | +30..+60ms:72, 0..+30ms:53, +60..+90ms:38 | Opponent.Block:93%, Opponent.Hop:0%, Opponent.Duck:0%, Opponent.Parry:69%, Opponent.Dodge:90% | Light:1249/849, Heavy:207/187, Unblockable:48/48, Undodgeable:34/33 | Blocked:121, CleanHit:1117, Parried:86, Dodged:114 |
| AggressiveLightSpam vs ReactionDefense | 1 | 95 | 0 | 4.34s | 63.9% | 0.00 | 17.55 | 5.13 | 22.43 | 5 | 0/5,258 (0%) | 0..+50ms:1851, +50..+100ms:19, +100..+150ms:6 | 0..+30ms:1 | Opponent.Duck:0%, Opponent.Block:100%, Opponent.Hop:0%, Opponent.Parry:50% | Light:1971/1259 | CleanHit:1259, Blocked:617, Parried:1 |
| Balanced vs AggressiveLightSpam | 96 | 0 | 0 | 2.89s | 73.1% | 0.00 | 10.86 | 1.09 | 14.92 | 6 | 0/3,660 (0%) | 0..+50ms:1122, -100..-50ms:124, +150..+200ms:116 | +30..+60ms:91, 0..+30ms:70, +60..+90ms:29 | Player.Parry:71%, Player.Hop:0%, Player.Duck:0%, Player.Dodge:92%, Player.Block:86% | Light:1401/984, Heavy:150/144, Unblockable:19/16, Undodgeable:21/19 | CleanHit:1163, Dodged:81, Parried:156, Blocked:83 |
| Balanced vs Balanced | 47 | 47 | 2 | 4.09s | 58.4% | 0.00 | 10.86 | 2.58 | 6.46 | 8 | 0/4,046 (0%) | 0..+50ms:1174, -100..-50ms:211, +150..+200ms:160 | +30..+60ms:180, 0..+30ms:127, -30..0ms:78 | Opponent.Parry:62%, Player.Parry:60%, Opponent.Block:89%, Player.Dodge:85%, Player.Block:93%, Player.Duck:0%, Opponent.Dodge:84%, Opponent.Duck:0%, Opponent.Hop:0%, Player.Hop:0% | Light:1524/911, Heavy:397/208, Unblockable:40/23, Undodgeable:35/24 | Parried:319, CleanHit:1166, Blocked:212, Dodged:192 |
| Balanced vs DodgeHeavy | 20 | 76 | 0 | 5.16s | 40.8% | 0.00 | 15.16 | 3.25 | 4.97 | 8 | 0/4,551 (0%) | 0..+50ms:1109, +150..+200ms:338, +200..+250ms:198 | +30..+60ms:205, +60..+90ms:157, 0..+30ms:156 | Player.Parry:65%, Opponent.Hop:0%, Opponent.Dodge:79%, Player.Block:95%, Opponent.Block:100%, Player.Dodge:83%, Opponent.Duck:0%, Player.Hop:0%, Opponent.Parry:50%, Player.Duck:0% | Light:1010/369, Heavy:1098/498, Undodgeable:20/2, Unblockable:18/6 | CleanHit:875, Parried:175, Dodged:635, Blocked:346 |
| Balanced vs HeavyBaiting | 96 | 0 | 0 | 2.86s | 78.7% | 0.00 | 5.29 | 0.64 | 3.53 | 4 | 0/2,012 (0%) | 0..+50ms:760, +50..+100ms:107, +150..+200ms:98 | +30..+60ms:49, -30..0ms:28, 0..+30ms:21 | Player.Duck:0%, Player.Block:94%, Player.Dodge:91%, Player.Parry:56%, Player.Hop:0% | Light:877/721, Heavy:365/249, Undodgeable:16/16, Unblockable:21/21 | CleanHit:1007, Blocked:50, Parried:86, Dodged:50 |
| Balanced vs ParryFishing | 60 | 36 | 0 | 3.92s | 64.0% | 0.00 | 15.53 | 0.77 | 3.04 | 7 | 0/3,647 (0%) | 0..+50ms:1216, -100..-50ms:380, +50..+100ms:126 | +30..+60ms:229, 0..+30ms:189, -30..0ms:86 | Opponent.Parry:64%, Player.Dodge:98%, Player.Hop:0%, Opponent.Dodge:88%, Player.Parry:69%, Opponent.Hop:0%, Player.Duck:0%, Player.Block:98%, Opponent.Duck:0% | Light:1858/1212, Unblockable:22/6, Heavy:195/110, Undodgeable:14/9 | Parried:473, Dodged:143, CleanHit:1337, Blocked:42 |
| Balanced vs PassiveBlocking | 14 | 0 | 82 | 12.14s | 0.7% | 0.00 | 16.83 | 0.03 | 23.02 | 3 | 0/4,281 (0%) | 0..+50ms:1940, -150..-100ms:8 |  | Opponent.Block:100%, Player.Parry:0%, Player.Duck:0%, Player.Dodge:0% | Undodgeable:8/0, Light:1808/0, Heavy:128/0, Unblockable:14/14 | Blocked:1934, CleanHit:14 |
| Balanced vs Random | 54 | 40 | 2 | 4.59s | 54.7% | 0.00 | 9.76 | 2.78 | 5.19 | 8 | 0/4,247 (0%) | 0..+50ms:1112, -100..-50ms:156, +150..+200ms:153 | +30..+60ms:180, 0..+30ms:130, -60..-30ms:78 | Opponent.Dodge:88%, Opponent.Parry:61%, Opponent.Duck:21%, Player.Dodge:87%, Opponent.Block:94%, Opponent.Hop:19%, Player.Block:95%, Player.Parry:63%, Player.Duck:0%, Player.Hop:0% | Light:1379/789, Heavy:494/245, Undodgeable:76/30, Unblockable:71/41 | Dodged:293, CleanHit:1105, Parried:248, Blocked:259 |
| Balanced vs ReactionDefense | 17 | 77 | 2 | 6.76s | 46.0% | 0.00 | 22.61 | 8.98 | 8.15 | 8 | 0/6,153 (0%) | 0..+50ms:2248, -100..-50ms:187, +50..+100ms:122 | +30..+60ms:167, 0..+30ms:108, -30..0ms:38 | Player.Parry:69%, Opponent.Block:99%, Opponent.Hop:0%, Player.Dodge:88%, Player.Duck:0%, Player.Block:90%, Player.Hop:0%, Opponent.Parry:58%, Opponent.Duck:0%, Opponent.Dodge:92% | Light:2603/1290, Heavy:232/17, Undodgeable:23/11, Unblockable:15/4 | CleanHit:1322, Parried:255, Blocked:1079, Dodged:124 |
| DodgeHeavy vs AggressiveLightSpam | 95 | 1 | 0 | 2.93s | 57.9% | 0.00 | 11.38 | 0.93 | 13.33 | 3 | 0/3,376 (0%) | 0..+50ms:834, +150..+200ms:315, +100..+150ms:61 | +60..+90ms:106, +30..+60ms:92, 0..+30ms:76 | Player.Dodge:88%, Player.Block:99%, Player.Hop:0%, Player.Duck:0%, Player.Parry:50% | Heavy:563/479, Light:871/352 | Dodged:325, CleanHit:831, Blocked:127, Parried:1 |
| DodgeHeavy vs Balanced | 77 | 19 | 0 | 5.11s | 41.1% | 0.00 | 14.67 | 3.24 | 4.73 | 8 | 0/4,389 (0%) | 0..+50ms:1098, +150..+200ms:365, +200..+250ms:164 | +30..+60ms:190, +60..+90ms:153, 0..+30ms:151 | Player.Dodge:87%, Opponent.Parry:60%, Player.Duck:0%, Player.Hop:0%, Opponent.Dodge:84%, Player.Block:100%, Opponent.Hop:0%, Opponent.Block:96%, Opponent.Duck:0%, Player.Parry:80% | Heavy:1108/498, Light:969/365, Unblockable:19/6, Undodgeable:19/1 | Dodged:612, CleanHit:870, Parried:172, Blocked:338 |
| DodgeHeavy vs DodgeHeavy | 38 | 54 | 4 | 7.23s | 29.6% | 0.00 | 24.80 | 0.01 | 0.41 | 1 | 0/5,306 (0%) | 0..+50ms:2547, -100..-50ms:20, +50..+100ms:6 | -30..0ms:279, +30..+60ms:271, -60..-30ms:249 | Opponent.Dodge:88%, Player.Block:100%, Player.Hop:0%, Player.Dodge:88%, Opponent.Duck:0%, Player.Duck:0%, Opponent.Block:100%, Opponent.Hop:0%, Opponent.Parry:50%, Player.Parry:63% | Heavy:2575/762 | Dodged:1322, Blocked:469, CleanHit:762, Parried:20 |
| DodgeHeavy vs HeavyBaiting | 95 | 1 | 0 | 2.78s | 61.5% | 0.00 | 5.33 | 0.17 | 1.36 | 1 | 0/1,726 (0%) | 0..+50ms:675, +150..+200ms:122, +200..+250ms:100 | +30..+60ms:71, 0..+30ms:60, +60..+90ms:56 | Player.Block:96%, Player.Duck:0%, Player.Dodge:91%, Player.Hop:0%, Player.Parry:100% | Heavy:737/529, Light:360/146 | CleanHit:675, Blocked:67, Dodged:249, Parried:11 |
| DodgeHeavy vs ParryFishing | 83 | 13 | 0 | 3.99s | 50.0% | 0.00 | 16.17 | 0.98 | 0.19 | 1 | 0/3,292 (0%) | 0..+50ms:739, +100..+150ms:315, +150..+200ms:248 | +30..+60ms:210, 0..+30ms:158, +60..+90ms:152 | Player.Dodge:95%, Opponent.Parry:58%, Player.Block:100%, Player.Duck:0%, Player.Parry:25%, Player.Hop:0%, Opponent.Hop:0%, Opponent.Duck:0%, Opponent.Dodge:86% | Heavy:739/464, Light:1125/468 | Dodged:524, CleanHit:932, Parried:222, Blocked:66 |
| DodgeHeavy vs PassiveBlocking | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 9.00 | 0.00 | 12.00 | 0 | 0/2,319 (0%) | 0..+50ms:960 |  | Opponent.Block:100%, Player.Dodge:0%, Player.Parry:0%, Player.Duck:0% | Heavy:1056/0 | Blocked:960 |
| DodgeHeavy vs Random | 79 | 17 | 0 | 6.22s | 37.6% | 0.00 | 14.19 | 4.15 | 2.42 | 8 | 0/4,863 (0%) | 0..+50ms:1076, +150..+200ms:264, +200..+250ms:229 | +30..+60ms:192, 0..+30ms:165, -30..0ms:152 | Player.Dodge:88%, Opponent.Hop:23%, Opponent.Parry:65%, Player.Block:100%, Opponent.Duck:25%, Player.Duck:0%, Opponent.Dodge:79%, Opponent.Block:96%, Player.Hop:0%, Player.Parry:67% | Heavy:1388/534, Light:726/286, Unblockable:75/21, Undodgeable:71/8 | CleanHit:849, Dodged:719, Parried:133, Blocked:440 |
| DodgeHeavy vs ReactionDefense | 4 | 92 | 0 | 11.34s | 24.5% | 0.00 | 40.65 | 14.27 | 1.72 | 8 | 0/9,787 (0%) | 0..+50ms:2534, +200..+250ms:913, +150..+200ms:493 | +30..+60ms:339, +60..+90ms:280, 0..+30ms:272 | Opponent.Block:100%, Player.Dodge:94%, Opponent.Duck:0%, Opponent.Hop:0%, Player.Block:99%, Player.Duck:0%, Player.Parry:63%, Player.Hop:0%, Opponent.Parry:60% | Heavy:1942/183, Light:2270/851 | CleanHit:1034, Blocked:1958, Dodged:1070, Parried:32 |
| HeavyBaiting vs AggressiveLightSpam | 0 | 96 | 0 | 2.95s | 100.0% | 0.00 | 9.63 | 0.00 | 15.14 | 3 | 0/2,880 (0%) | 0..+50ms:1211, +300..+350ms:137, +150..+200ms:51 |  |  | Light:1225/1225, Heavy:202/202 | CleanHit:1427 |
| HeavyBaiting vs Balanced | 0 | 96 | 0 | 2.80s | 76.9% | 0.00 | 5.11 | 0.67 | 3.33 | 6 | 0/1,969 (0%) | 0..+50ms:731, +50..+100ms:116, +150..+200ms:90 | +30..+60ms:55, 0..+30ms:43, -60..-30ms:22 | Opponent.Dodge:87%, Opponent.Parry:63%, Opponent.Duck:0%, Opponent.Block:93%, Opponent.Hop:0% | Light:840/673, Heavy:391/267, Unblockable:13/13, Undodgeable:14/14 | CleanHit:967, Dodged:54, Parried:98, Blocked:53 |
| HeavyBaiting vs DodgeHeavy | 0 | 96 | 0 | 2.83s | 61.0% | 0.00 | 5.32 | 0.48 | 1.42 | 2 | 0/1,731 (0%) | 0..+50ms:685, +150..+200ms:132, +200..+250ms:85 | +30..+60ms:60, +60..+90ms:56, 0..+30ms:45 | Opponent.Dodge:88%, Opponent.Block:100%, Opponent.Parry:100%, Opponent.Duck:0%, Opponent.Hop:0% | Light:358/141, Heavy:755/538 | Dodged:247, CleanHit:679, Blocked:87, Parried:5 |
| HeavyBaiting vs HeavyBaiting | 17 | 18 | 61 | 3.72s | 96.4% | 0.00 | 4.49 | 0.00 | 4.29 | 1 | 0/1,728 (0%) | 0..+50ms:1105, +150..+200ms:103, +50..+100ms:57 |  |  | Light:871/845, Heavy:445/424 | CleanHit:1269 |
| HeavyBaiting vs ParryFishing | 0 | 96 | 0 | 3.11s | 84.7% | 0.00 | 9.55 | 0.14 | 2.59 | 1 | 0/1,948 (0%) | 0..+50ms:850, -100..-50ms:156, +50..+100ms:98 | +30..+60ms:63, 0..+30ms:45, -60..-30ms:40 | Opponent.Parry:68%, Opponent.Duck:0%, Opponent.Hop:0%, Opponent.Dodge:100% | Light:1147/1044, Heavy:262/149 | CleanHit:1193, Parried:176, Dodged:1 |
| HeavyBaiting vs PassiveBlocking | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 10.98 | 0.00 | 9.80 | 1 | 0/2,784 (0%) | 0..+50ms:1744 |  | Opponent.Block:100% | Light:1403/0, Heavy:344/0 | Blocked:1744 |
| HeavyBaiting vs Random | 0 | 96 | 0 | 3.06s | 73.7% | 0.00 | 3.50 | 0.67 | 2.46 | 4 | 0/1,978 (0%) | 0..+50ms:745, +50..+100ms:93, +150..+200ms:92 | +30..+60ms:46, 0..+30ms:38, +60..+90ms:28 | Opponent.Dodge:81%, Opponent.Duck:31%, Opponent.Hop:24%, Opponent.Block:98%, Opponent.Parry:58% | Light:710/527, Heavy:404/274, Undodgeable:41/39, Unblockable:46/45 | CleanHit:885, Dodged:109, Blocked:87, Parried:46 |
| HeavyBaiting vs ReactionDefense | 0 | 96 | 0 | 3.58s | 68.0% | 0.00 | 9.57 | 3.42 | 2.10 | 4 | 0/2,633 (0%) | 0..+50ms:1169, +250..+300ms:98, +300..+350ms:57 | +30..+60ms:3, -60..-30ms:1, 0..+30ms:1 | Opponent.Block:100%, Opponent.Hop:0%, Opponent.Duck:0%, Opponent.Parry:100% | Light:1347/1044, Heavy:212/16 | CleanHit:1060, Blocked:374, Parried:5 |
| ParryFishing vs AggressiveLightSpam | 90 | 6 | 0 | 3.08s | 81.9% | 0.00 | 15.94 | 0.00 | 14.94 | 3 | 0/3,627 (0%) | 0..+50ms:1416, -100..-50ms:306 | +30..+60ms:138, 0..+30ms:116, -30..0ms:31 | Player.Parry:76%, Player.Hop:0%, Player.Duck:0%, Player.Dodge:100% | Light:1728/1415 | CleanHit:1415, Parried:306, Dodged:1 |
| ParryFishing vs Balanced | 40 | 55 | 1 | 3.93s | 62.5% | 0.00 | 15.28 | 0.83 | 2.93 | 5 | 0/3,671 (0%) | 0..+50ms:1178, -100..-50ms:370, +100..+150ms:129 | +30..+60ms:240, 0..+30ms:188, -30..0ms:100 | Player.Parry:65%, Opponent.Parry:74%, Opponent.Dodge:97%, Player.Dodge:100%, Player.Hop:0%, Opponent.Duck:0%, Player.Duck:0%, Opponent.Block:92%, Opponent.Hop:0% | Light:1847/1166, Heavy:183/116, Unblockable:28/5, Undodgeable:21/12 | CleanHit:1299, Parried:495, Dodged:142, Blocked:47 |
| ParryFishing vs DodgeHeavy | 9 | 87 | 0 | 4.07s | 48.6% | 0.00 | 16.55 | 1.10 | 0.23 | 1 | 0/3,385 (0%) | 0..+50ms:751, +100..+150ms:320, +150..+200ms:258 | +30..+60ms:255, +60..+90ms:145, 0..+30ms:139 | Player.Parry:60%, Opponent.Dodge:95%, Player.Duck:0%, Opponent.Block:100%, Opponent.Hop:0%, Player.Hop:0%, Opponent.Parry:56%, Opponent.Duck:0%, Player.Dodge:100% | Light:1157/469, Heavy:753/459 | CleanHit:928, Dodged:518, Blocked:88, Parried:247 |
| ParryFishing vs HeavyBaiting | 96 | 0 | 0 | 3.12s | 86.3% | 0.00 | 9.49 | 0.15 | 2.51 | 1 | 0/1,944 (0%) | 0..+50ms:913, -100..-50ms:132, +250..+300ms:82 | 0..+30ms:51, +30..+60ms:45, -30..0ms:27 | Player.Parry:60%, Player.Duck:0%, Player.Dodge:60%, Player.Hop:0% | Light:1166/1067, Heavy:255/160 | CleanHit:1227, Parried:147, Dodged:3 |
| ParryFishing vs ParryFishing | 49 | 39 | 8 | 3.94s | 66.8% | 0.00 | 20.90 | 0.00 | 0.00 | 1 | 0/3,351 (0%) | 0..+50ms:1366, -100..-50ms:602, +50..+100ms:229 | +30..+60ms:314, 0..+30ms:247, -30..0ms:95 | Opponent.Parry:69%, Opponent.Dodge:100%, Player.Parry:71%, Player.Duck:0%, Player.Hop:0%, Opponent.Hop:0%, Opponent.Duck:0%, Player.Dodge:100% | Light:2198/1468 | CleanHit:1468, Parried:715, Dodged:15 |
| ParryFishing vs PassiveBlocking | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 21.00 | 0.00 | 13.00 | 0 | 0/3,463 (0%) | 0..+50ms:2112 |  | Opponent.Block:100%, Player.Duck:0% | Light:2112/0 | Blocked:2112 |
| ParryFishing vs Random | 41 | 55 | 0 | 4.33s | 59.2% | 0.00 | 14.45 | 1.17 | 0.96 | 5 | 0/3,819 (0%) | 0..+50ms:1028, -100..-50ms:263, +50..+100ms:157 | +30..+60ms:238, 0..+30ms:175, -60..-30ms:86 | Player.Parry:65%, Opponent.Dodge:98%, Opponent.Duck:28%, Player.Dodge:83%, Opponent.Hop:25%, Opponent.Parry:71%, Player.Hop:0%, Player.Duck:0%, Opponent.Block:98%, Player.Block:0% | Light:1709/1022, Heavy:259/161, Unblockable:54/23, Undodgeable:71/33 | CleanHit:1239, Dodged:274, Parried:392, Blocked:82 |
| ParryFishing vs ReactionDefense | 53 | 32 | 11 | 3.85s | 70.2% | 0.00 | 20.33 | 0.00 | 0.00 | 1 | 0/4,360 (0%) | 0..+50ms:1791, -100..-50ms:344, +50..+100ms:9 | +30..+60ms:150, 0..+30ms:125, -30..0ms:48 | Player.Parry:66%, Opponent.Block:100%, Opponent.Hop:0%, Player.Duck:0%, Opponent.Parry:73%, Player.Hop:0%, Opponent.Duck:0%, Player.Dodge:100% | Light:2146/1507 | Blocked:277, Parried:349, CleanHit:1507, Dodged:11 |
| PassiveBlocking vs AggressiveLightSpam | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 21.00 | 0.00 | 68.00 | 3 | 0/8,736 (0%) | 0..+50ms:2112 |  | Player.Block:100% | Light:2112/0 | Blocked:2112 |
| PassiveBlocking vs Balanced | 0 | 8 | 88 | 12.09s | 0.4% | 0.00 | 16.73 | 0.00 | 22.97 | 3 | 0/4,255 (0%) | 0..+50ms:1930, -150..-100ms:7 |  | Player.Block:100%, Opponent.Dodge:0%, Opponent.Parry:0%, Opponent.Duck:0% | Heavy:144/0, Light:1790/0, Unblockable:8/8, Undodgeable:7/0 | Blocked:1929, CleanHit:8 |
| PassiveBlocking vs DodgeHeavy | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 9.00 | 0.00 | 12.00 | 0 | 0/2,311 (0%) | 0..+50ms:960 |  | Player.Block:100%, Opponent.Dodge:0%, Opponent.Duck:0%, Opponent.Parry:0% | Heavy:1056/0 | Blocked:960 |
| PassiveBlocking vs HeavyBaiting | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 10.50 | 0.00 | 10.15 | 1 | 0/2,784 (0%) | 0..+50ms:1711 |  | Player.Block:100% | Heavy:378/0, Light:1336/0 | Blocked:1711 |
| PassiveBlocking vs ParryFishing | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 21.00 | 0.00 | 13.00 | 0 | 0/3,461 (0%) | 0..+50ms:2112 |  | Player.Block:100%, Opponent.Parry:0%, Opponent.Duck:0% | Light:2112/0 | Blocked:2112 |
| PassiveBlocking vs PassiveBlocking | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 0.00 | 0.00 | 0.00 | 0 | 0/192 (0%) |  |  |  |  |  |
| PassiveBlocking vs Random | 0 | 27 | 69 | 12.33s | 1.5% | 0.00 | 12.05 | 0.00 | 11.89 | 2 | 0/3,016 (0%) | 0..+50ms:1718, -150..-100ms:20 |  | Player.Block:100%, Opponent.Dodge:0%, Opponent.Duck:0% | Heavy:273/0, Light:1449/0, Unblockable:27/27, Undodgeable:20/0 | Blocked:1711, CleanHit:27 |
| PassiveBlocking vs ReactionDefense | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 21.00 | 0.00 | 13.00 | 0 | 0/3,465 (0%) | 0..+50ms:2112 |  | Player.Block:100%, Opponent.Duck:0%, Opponent.Parry:0%, Opponent.Dodge:0% | Light:2112/0 | Blocked:2112 |
| Random vs AggressiveLightSpam | 86 | 10 | 0 | 3.18s | 75.7% | 0.00 | 9.93 | 0.81 | 15.82 | 5 | 0/3,794 (0%) | 0..+50ms:1053, +150..+200ms:148, +50..+100ms:44 | +30..+60ms:57, 0..+30ms:50, +60..+90ms:39 | Player.Block:93%, Player.Parry:67%, Player.Duck:0%, Player.Dodge:93%, Player.Hop:0% | Light:1243/896, Heavy:206/184, Undodgeable:32/31, Unblockable:51/49 | CleanHit:1160, Blocked:113, Parried:66, Dodged:98 |
| Random vs Balanced | 46 | 49 | 1 | 4.74s | 52.9% | 0.00 | 10.10 | 3.09 | 5.23 | 8 | 0/4,414 (0%) | 0..+50ms:1065, +150..+200ms:173, -100..-50ms:163 | +30..+60ms:189, 0..+30ms:127, +60..+90ms:96 | Player.Parry:66%, Player.Block:96%, Player.Duck:25%, Opponent.Parry:64%, Opponent.Dodge:80%, Player.Dodge:88%, Player.Hop:20%, Opponent.Block:88%, Opponent.Hop:0%, Opponent.Duck:0% | Light:1427/788, Heavy:499/238, Undodgeable:81/39, Unblockable:84/41 | CleanHit:1106, Blocked:257, Dodged:337, Parried:268 |
| Random vs DodgeHeavy | 17 | 79 | 0 | 5.83s | 38.5% | 0.00 | 12.82 | 3.46 | 2.69 | 6 | 0/4,483 (0%) | 0..+50ms:1043, +150..+200ms:214, +200..+250ms:179 | +30..+60ms:201, 0..+30ms:132, +60..+90ms:128 | Opponent.Dodge:80%, Player.Parry:64%, Player.Block:97%, Opponent.Block:100%, Player.Hop:24%, Opponent.Duck:0%, Player.Duck:20%, Opponent.Parry:25%, Opponent.Hop:0%, Player.Dodge:89% | Light:620/228, Heavy:1328/546, Unblockable:71/24, Undodgeable:64/4 | Dodged:650, Parried:115, Blocked:397, CleanHit:802 |
| Random vs HeavyBaiting | 93 | 3 | 0 | 3.05s | 74.4% | 0.00 | 3.47 | 0.49 | 2.55 | 4 | 0/1,956 (0%) | 0..+50ms:701, +150..+200ms:99, +50..+100ms:95 | +30..+60ms:46, 0..+30ms:37, -60..-30ms:25 | Player.Dodge:87%, Player.Hop:19%, Player.Parry:73%, Player.Duck:19%, Player.Block:100% | Light:683/512, Heavy:431/299, Undodgeable:50/49, Unblockable:33/31 | CleanHit:891, Dodged:93, Parried:64, Blocked:68 |
| Random vs ParryFishing | 62 | 34 | 0 | 4.24s | 59.7% | 0.00 | 13.97 | 1.06 | 0.88 | 4 | 0/3,747 (0%) | 0..+50ms:974, -100..-50ms:243, +50..+100ms:197 | +30..+60ms:205, 0..+30ms:188, -30..0ms:105 | Player.Hop:23%, Opponent.Parry:60%, Player.Parry:73%, Player.Duck:25%, Player.Dodge:98%, Player.Block:99%, Opponent.Hop:0%, Opponent.Dodge:88%, Opponent.Duck:0% | Heavy:259/167, Light:1667/997, Undodgeable:58/41, Unblockable:69/20 | CleanHit:1225, Dodged:292, Parried:362, Blocked:70 |
| Random vs PassiveBlocking | 27 | 0 | 69 | 12.32s | 1.5% | 0.00 | 11.74 | 0.00 | 11.97 | 2 | 0/3,000 (0%) | 0..+50ms:1695, -150..-100ms:22 |  | Opponent.Block:100%, Player.Duck:0%, Player.Dodge:0%, Player.Parry:0% | Light:1412/0, Undodgeable:22/0, Heavy:287/0, Unblockable:27/27 | Blocked:1690, CleanHit:27 |
| Random vs Random | 54 | 42 | 0 | 4.89s | 53.7% | 0.00 | 7.33 | 3.01 | 3.59 | 7 | 0/4,002 (0%) | 0..+50ms:1016, +50..+100ms:130, +150..+200ms:112 | +30..+60ms:145, +60..+90ms:84, 0..+30ms:84 | Player.Dodge:86%, Opponent.Dodge:72%, Player.Hop:23%, Opponent.Hop:20%, Player.Duck:21%, Opponent.Duck:24%, Player.Parry:62%, Player.Block:96%, Opponent.Parry:52%, Opponent.Block:95% | Heavy:564/239, Light:1086/619, Unblockable:100/74, Undodgeable:114/69 | Dodged:318, CleanHit:1001, Parried:155, Blocked:281 |
| Random vs ReactionDefense | 16 | 80 | 0 | 6.98s | 45.1% | 0.00 | 18.76 | 8.94 | 3.48 | 8 | 0/5,952 (0%) | 0..+50ms:1927, +50..+100ms:123, +150..+200ms:103 | +30..+60ms:165, 0..+30ms:109, +60..+90ms:78 | Player.Duck:24%, Opponent.Block:99%, Opponent.Parry:71%, Player.Block:96%, Player.Dodge:94%, Player.Parry:70%, Player.Hop:20%, Opponent.Duck:0%, Opponent.Dodge:74%, Opponent.Hop:0% | Heavy:386/25, Light:2243/1174, Undodgeable:68/21, Unblockable:70/29 | CleanHit:1249, Blocked:936, Parried:175, Dodged:290 |
| ReactionDefense vs AggressiveLightSpam | 79 | 17 | 0 | 3.84s | 72.5% | 0.00 | 16.76 | 3.40 | 19.40 | 5 | 0/4,874 (0%) | 0..+50ms:1786, +50..+100ms:8, +100..+150ms:6 | +30..+60ms:1 | Player.Block:100%, Player.Hop:0%, Player.Duck:0%, Player.Parry:33% | Light:1881/1364 | CleanHit:1364, Blocked:436, Parried:1 |
| ReactionDefense vs Balanced | 59 | 36 | 1 | 6.82s | 44.2% | 0.00 | 23.50 | 9.55 | 7.08 | 8 | 0/7,056 (0%) | 0..+50ms:2269, -100..-50ms:226, +50..+100ms:169 | +30..+60ms:175, 0..+30ms:137, -30..0ms:65 | Player.Block:99%, Opponent.Dodge:52%, Opponent.Parry:47%, Player.Duck:0%, Opponent.Block:84%, Player.Hop:0%, Opponent.Duck:0%, Opponent.Hop:0%, Player.Parry:73%, Player.Dodge:78% | Light:2708/1303, Heavy:283/23, Undodgeable:27/9, Unblockable:10/3 | CleanHit:1338, Blocked:1123, Parried:324, Dodged:145 |
| ReactionDefense vs DodgeHeavy | 88 | 8 | 0 | 14.72s | 19.9% | 0.00 | 49.60 | 20.34 | 4.29 | 8 | 0/12,823 (0%) | 0..+50ms:3408, +200..+250ms:624, +150..+200ms:551 | +30..+60ms:319, +60..+90ms:265, 0..+30ms:251 | Opponent.Dodge:45%, Player.Block:100%, Opponent.Block:100%, Opponent.Duck:0%, Opponent.Hop:0%, Player.Duck:0%, Player.Hop:0%, Player.Parry:68%, Opponent.Parry:40%, Player.Dodge:0% | Light:2795/830, Heavy:2281/178 | Dodged:1075, Blocked:2837, CleanHit:1008, Parried:34 |
| ReactionDefense vs HeavyBaiting | 96 | 0 | 0 | 3.52s | 69.7% | 0.00 | 9.48 | 3.18 | 2.17 | 5 | 0/2,563 (0%) | 0..+50ms:1173, +250..+300ms:101, +50..+100ms:50 | -30..0ms:1, 0..+30ms:1 | Player.Block:100%, Player.Hop:0%, Player.Duck:0%, Player.Parry:67% | Light:1318/1050, Heavy:219/22 | CleanHit:1072, Blocked:358, Parried:2 |
| ReactionDefense vs ParryFishing | 37 | 55 | 4 | 3.80s | 69.4% | 0.00 | 20.06 | 0.00 | 0.00 | 1 | 0/4,274 (0%) | 0..+50ms:1454, -100..-50ms:337, +50..+100ms:327 | +30..+60ms:149, 0..+30ms:126, -30..0ms:44 | Opponent.Parry:68%, Player.Duck:0%, Player.Block:100%, Opponent.Hop:0%, Player.Parry:67%, Opponent.Dodge:100%, Player.Hop:0%, Opponent.Duck:0% | Light:2120/1471 | Parried:339, CleanHit:1471, Blocked:300, Dodged:8 |
| ReactionDefense vs PassiveBlocking | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 21.00 | 0.00 | 13.00 | 0 | 0/3,464 (0%) | 0..+50ms:2112 |  | Opponent.Block:100%, Player.Dodge:0%, Player.Parry:0% | Light:2112/0 | Blocked:2112 |
| ReactionDefense vs Random | 65 | 28 | 3 | 7.61s | 41.4% | 0.00 | 21.36 | 10.67 | 3.10 | 8 | 0/7,275 (0%) | 0..+50ms:2116, +150..+200ms:141, +50..+100ms:131 | +30..+60ms:157, 0..+30ms:149, +60..+90ms:94 | Opponent.Duck:16%, Player.Block:100%, Opponent.Parry:47%, Opponent.Dodge:55%, Opponent.Hop:15%, Player.Parry:60%, Opponent.Block:93%, Player.Dodge:79%, Player.Duck:0%, Player.Hop:0% | Light:2478/1179, Unblockable:93/32, Undodgeable:65/24, Heavy:445/42 | CleanHit:1277, Blocked:1152, Parried:199, Dodged:342 |
| ReactionDefense vs ReactionDefense | 43 | 38 | 15 | 3.76s | 70.0% | 0.00 | 19.85 | 0.00 | 0.00 | 1 | 0/5,349 (0%) | 0..+50ms:2072, -100..-50ms:14, +50..+100ms:12 | +30..+60ms:6, -60..-30ms:4, 0..+30ms:2 | Opponent.Block:100%, Player.Block:100%, Player.Duck:0%, Player.Hop:0%, Opponent.Duck:0%, Player.Parry:29%, Opponent.Hop:0%, Opponent.Parry:80% | Light:2100/1471 | Blocked:613, CleanHit:1471, Parried:14 |

## Read75Hit2Regen20

AI parry read 75%; normal hit taken +2; regen 20/s.

Extra mirrored opening duels (outside strategy matrix): attack-first 1240, counter-first 774, draws 34.
Matches: 6,144; stalemates: 1,441; timeouts: 0; avg duration: 6.33s; avg initiative changes: 42.63; avg retaliations: 4.75; avg unavoidable threats: 0.00; avg clean-hit run without theoretical counter opportunity: 0.00; avg attacks buffered while defending: 2.42; avg rejected queue inputs: 7.23; predicted defenses: 0.02; incorrect defense choices: 1.10.
Action inputs failed: 0/262,238 (0.0%).

| Matchup | First wins | Second wins | Draws | Avg. duration | Hit rate | Unavoidable hits | Repeated attacks | Buffered while defending | Queue rejects | Max queue | Action failures | Frame advantage (top bins) | Defense timing (top bins) | Defense success | Attack hit mix | Outcomes |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---|---|---|---|---|
| AggressiveLightSpam vs AggressiveLightSpam | 0 | 0 | 96 | 2.55s | 90.0% | 0.00 | 16.00 | 0.00 | 18.00 | 4 | 0/3,840 (0%) | 0..+50ms:1728 |  |  | Light:1920/1728 | CleanHit:1728 |
| AggressiveLightSpam vs Balanced | 3 | 90 | 3 | 3.07s | 69.4% | 0.00 | 12.51 | 1.95 | 12.40 | 8 | 0/3,929 (0%) | 0..+50ms:1202, +150..+200ms:123, -100..-50ms:114 | +30..+60ms:114, 0..+30ms:69, -30..0ms:31 | Opponent.Dodge:90%, Opponent.Block:87%, Opponent.Parry:69%, Opponent.Duck:0%, Opponent.Hop:0% | Light:1569/1037, Heavy:164/154, Undodgeable:25/24, Unblockable:21/19 | CleanHit:1234, Dodged:100, Blocked:141, Parried:171 |
| AggressiveLightSpam vs DodgeHeavy | 0 | 96 | 0 | 3.09s | 54.7% | 0.00 | 13.07 | 1.30 | 12.04 | 5 | 0/3,704 (0%) | 0..+50ms:950, +150..+200ms:380, +200..+250ms:60 | +30..+60ms:130, +60..+90ms:110, 0..+30ms:86 | Opponent.Dodge:82%, Opponent.Block:100%, Opponent.Hop:0%, Opponent.Duck:0%, Opponent.Parry:0% | Light:1058/376, Heavy:506/480 | Dodged:400, CleanHit:856, Blocked:191 |
| AggressiveLightSpam vs HeavyBaiting | 55 | 13 | 28 | 2.71s | 91.9% | 0.00 | 9.20 | 0.00 | 9.66 | 3 | 0/2,623 (0%) | 0..+50ms:1156, +300..+350ms:133, +150..+200ms:97 |  |  | Light:1277/1170, Heavy:262/245 | CleanHit:1415 |
| AggressiveLightSpam vs ParryFishing | 6 | 90 | 0 | 3.08s | 79.0% | 0.00 | 15.94 | 0.00 | 11.94 | 4 | 0/3,633 (0%) | 0..+50ms:1407, -100..-50ms:284, +50..+100ms:30 | +30..+60ms:119, 0..+30ms:108, -30..0ms:36 | Opponent.Parry:69%, Opponent.Hop:0%, Opponent.Duck:0%, Opponent.Dodge:100% | Light:1818/1436 | Parried:285, CleanHit:1436, Dodged:1 |
| AggressiveLightSpam vs PassiveBlocking | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 27.00 | 0.00 | 62.00 | 3 | 0/8,736 (0%) | 0..+50ms:2688 |  | Opponent.Block:100% | Light:2688/0 | Blocked:2688 |
| AggressiveLightSpam vs Random | 11 | 83 | 2 | 3.37s | 68.9% | 0.00 | 12.06 | 1.55 | 13.89 | 6 | 0/4,196 (0%) | 0..+50ms:1126, +150..+200ms:159, +50..+100ms:79 | +30..+60ms:82, 0..+30ms:64, +60..+90ms:43 | Opponent.Block:95%, Opponent.Hop:0%, Opponent.Duck:0%, Opponent.Parry:69%, Opponent.Dodge:83% | Light:1451/930, Heavy:213/196, Unblockable:50/46, Undodgeable:37/35 | Blocked:188, CleanHit:1207, Parried:98, Dodged:132 |
| AggressiveLightSpam vs ReactionDefense | 0 | 96 | 0 | 5.36s | 54.7% | 0.00 | 22.90 | 9.24 | 24.02 | 8 | 0/6,494 (0%) | 0..+50ms:2155, +50..+100ms:150, +100..+150ms:83 | 0..+30ms:1, +30..+60ms:1 | Opponent.Duck:0%, Opponent.Block:100%, Opponent.Hop:0%, Opponent.Parry:67% | Light:2494/1364 | CleanHit:1364, Blocked:1024, Parried:2 |
| Balanced vs AggressiveLightSpam | 91 | 3 | 2 | 2.97s | 70.6% | 0.00 | 12.04 | 1.53 | 11.82 | 8 | 0/3,820 (0%) | 0..+50ms:1196, +150..+200ms:142, -100..-50ms:139 | +30..+60ms:103, 0..+30ms:82, +60..+90ms:33 | Player.Parry:69%, Player.Hop:0%, Player.Duck:0%, Player.Dodge:90%, Player.Block:85% | Light:1523/1025, Heavy:166/158, Unblockable:18/18, Undodgeable:26/22 | CleanHit:1223, Dodged:87, Parried:186, Blocked:111 |
| Balanced vs Balanced | 45 | 49 | 2 | 4.04s | 57.5% | 0.00 | 10.33 | 3.18 | 3.59 | 8 | 0/4,009 (0%) | 0..+50ms:1118, -100..-50ms:203, +150..+200ms:174 | +30..+60ms:172, 0..+30ms:130, -60..-30ms:70 | Opponent.Parry:62%, Player.Parry:59%, Opponent.Block:89%, Player.Dodge:83%, Opponent.Dodge:84%, Opponent.Duck:0%, Player.Block:92%, Player.Duck:0%, Opponent.Hop:0%, Player.Hop:0% | Light:1461/868, Heavy:447/227, Undodgeable:50/27, Unblockable:48/31 | Parried:311, CleanHit:1153, Blocked:222, Dodged:183 |
| Balanced vs DodgeHeavy | 16 | 80 | 0 | 5.07s | 41.2% | 0.00 | 14.74 | 3.67 | 2.85 | 8 | 0/4,510 (0%) | 0..+50ms:1062, +150..+200ms:399, +200..+250ms:207 | +30..+60ms:216, 0..+30ms:152, +60..+90ms:144 | Player.Parry:59%, Opponent.Hop:0%, Opponent.Dodge:80%, Player.Block:96%, Opponent.Block:100%, Player.Dodge:83%, Opponent.Parry:44%, Opponent.Duck:0%, Player.Duck:0%, Player.Hop:0% | Light:1030/376, Heavy:1099/513, Unblockable:24/8, Undodgeable:24/0 | CleanHit:897, Parried:154, Dodged:656, Blocked:344 |
| Balanced vs HeavyBaiting | 90 | 5 | 1 | 2.81s | 73.7% | 0.00 | 5.02 | 0.98 | 1.22 | 7 | 0/2,035 (0%) | 0..+50ms:704, +150..+200ms:141, +50..+100ms:99 | +30..+60ms:47, -30..0ms:36, 0..+30ms:26 | Player.Duck:0%, Player.Block:94%, Player.Dodge:83%, Player.Parry:55%, Player.Hop:0% | Light:846/663, Heavy:479/302, Undodgeable:20/20, Unblockable:23/23 | CleanHit:1008, Blocked:66, Parried:102, Dodged:53 |
| Balanced vs ParryFishing | 63 | 33 | 0 | 3.79s | 64.3% | 0.00 | 14.57 | 0.90 | 1.17 | 7 | 0/3,516 (0%) | 0..+50ms:1134, -100..-50ms:352, +50..+100ms:132 | +30..+60ms:225, 0..+30ms:166, -30..0ms:90 | Opponent.Parry:65%, Player.Dodge:97%, Player.Hop:0%, Opponent.Dodge:86%, Player.Parry:68%, Player.Block:97%, Player.Duck:0%, Opponent.Hop:0%, Opponent.Duck:0% | Light:1762/1152, Unblockable:27/8, Heavy:226/137, Undodgeable:20/11 | Parried:453, Dodged:135, CleanHit:1308, Blocked:37 |
| Balanced vs PassiveBlocking | 15 | 0 | 81 | 12.16s | 0.6% | 0.00 | 20.83 | 0.03 | 17.80 | 3 | 0/4,287 (0%) | 0..+50ms:2426, -150..-100ms:10 |  | Opponent.Block:100%, Player.Parry:0%, Player.Duck:0%, Player.Dodge:0% | Undodgeable:10/0, Light:2255/0, Heavy:185/0, Unblockable:15/15 | Blocked:2421, CleanHit:15 |
| Balanced vs Random | 53 | 42 | 1 | 4.35s | 54.4% | 0.00 | 8.77 | 2.88 | 2.60 | 7 | 0/4,040 (0%) | 0..+50ms:987, +150..+200ms:173, -100..-50ms:149 | +30..+60ms:182, 0..+30ms:126, -30..0ms:82 | Opponent.Dodge:77%, Opponent.Parry:66%, Opponent.Duck:27%, Player.Dodge:83%, Opponent.Block:95%, Opponent.Hop:15%, Player.Parry:64%, Player.Duck:0%, Player.Block:96%, Player.Hop:0% | Light:1277/724, Heavy:516/252, Undodgeable:85/41, Unblockable:83/49 | Dodged:294, CleanHit:1066, Parried:254, Blocked:218 |
| Balanced vs ReactionDefense | 30 | 65 | 1 | 8.16s | 40.4% | 0.00 | 26.13 | 10.06 | 6.88 | 8 | 0/6,961 (0%) | 0..+50ms:2644, +50..+100ms:194, -100..-50ms:156 | +30..+60ms:156, 0..+30ms:120, -30..0ms:49 | Player.Parry:67%, Opponent.Block:99%, Opponent.Hop:0%, Opponent.Duck:0%, Player.Dodge:97%, Opponent.Parry:53%, Player.Block:88%, Opponent.Dodge:81%, Player.Duck:0%, Player.Hop:0% | Light:2995/1312, Heavy:318/27, Undodgeable:31/15, Unblockable:26/8 | CleanHit:1362, Parried:241, Blocked:1526, Dodged:147 |
| DodgeHeavy vs AggressiveLightSpam | 95 | 1 | 0 | 3.09s | 56.4% | 0.00 | 13.08 | 1.34 | 12.02 | 4 | 0/3,659 (0%) | 0..+50ms:951, +150..+200ms:383, +200..+250ms:58 | +60..+90ms:126, +30..+60ms:100, 0..+30ms:86 | Player.Dodge:81%, Player.Duck:0%, Player.Block:99%, Player.Parry:33%, Player.Hop:0% | Heavy:507/479, Light:1057/403 | Dodged:376, CleanHit:882, Blocked:189, Parried:1 |
| DodgeHeavy vs Balanced | 84 | 12 | 0 | 5.28s | 40.0% | 0.00 | 14.84 | 4.09 | 3.10 | 8 | 0/4,582 (0%) | 0..+50ms:1122, +150..+200ms:389, +200..+250ms:169 | +30..+60ms:202, 0..+30ms:158, +60..+90ms:144 | Player.Dodge:85%, Opponent.Parry:61%, Player.Duck:0%, Player.Hop:0%, Opponent.Dodge:83%, Player.Block:100%, Opponent.Block:95%, Player.Parry:80%, Opponent.Duck:0%, Opponent.Hop:0% | Heavy:1171/520, Light:978/352, Unblockable:25/8, Undodgeable:28/0 | Dodged:633, CleanHit:880, Parried:172, Blocked:385 |
| DodgeHeavy vs DodgeHeavy | 38 | 55 | 3 | 7.23s | 29.2% | 0.00 | 25.21 | 0.02 | 0.00 | 2 | 0/5,364 (0%) | 0..+50ms:2588, -100..-50ms:19, +50..+100ms:5 | +30..+60ms:284, -30..0ms:277, -60..-30ms:252 | Opponent.Dodge:88%, Player.Block:100%, Player.Hop:0%, Player.Dodge:88%, Opponent.Duck:0%, Player.Duck:0%, Opponent.Block:100%, Opponent.Hop:0%, Opponent.Parry:46%, Player.Parry:62% | Heavy:2617/765 | Dodged:1340, Blocked:488, CleanHit:765, Parried:19 |
| DodgeHeavy vs HeavyBaiting | 96 | 0 | 0 | 2.78s | 56.7% | 0.00 | 5.45 | 0.33 | 0.11 | 2 | 0/1,799 (0%) | 0..+50ms:645, +150..+200ms:166, +200..+250ms:119 | +30..+60ms:69, +60..+90ms:66, 0..+30ms:59 | Player.Block:97%, Player.Dodge:89%, Player.Duck:0%, Player.Hop:0%, Player.Parry:100% | Heavy:814/542, Light:401/147 | CleanHit:689, Blocked:90, Dodged:277, Parried:11 |
| DodgeHeavy vs ParryFishing | 84 | 12 | 0 | 3.99s | 49.9% | 0.00 | 16.25 | 1.01 | 0.01 | 1 | 0/3,298 (0%) | 0..+50ms:726, +100..+150ms:324, +150..+200ms:255 | +30..+60ms:210, 0..+30ms:157, +60..+90ms:154 | Player.Dodge:95%, Opponent.Parry:59%, Player.Block:100%, Player.Duck:0%, Player.Parry:25%, Player.Hop:0%, Opponent.Hop:0%, Opponent.Duck:0%, Opponent.Dodge:86% | Heavy:747/466, Light:1127/469 | Dodged:524, CleanHit:935, Parried:228, Blocked:65 |
| DodgeHeavy vs PassiveBlocking | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 12.00 | 0.00 | 9.00 | 0 | 0/2,319 (0%) | 0..+50ms:1248 |  | Opponent.Block:100%, Player.Dodge:0%, Player.Parry:0%, Player.Duck:0% | Heavy:1344/0 | Blocked:1248 |
| DodgeHeavy vs Random | 83 | 13 | 0 | 6.27s | 36.6% | 0.00 | 13.81 | 5.09 | 1.22 | 8 | 0/4,899 (0%) | 0..+50ms:1006, +150..+200ms:246, +200..+250ms:225 | +30..+60ms:188, -30..0ms:150, 0..+30ms:142 | Player.Dodge:86%, Opponent.Hop:20%, Opponent.Parry:65%, Opponent.Duck:23%, Player.Block:100%, Player.Duck:0%, Opponent.Dodge:78%, Opponent.Block:96%, Player.Hop:0%, Player.Parry:54% | Heavy:1443/547, Light:640/249, Unblockable:84/23, Undodgeable:89/7 | CleanHit:826, Dodged:671, Parried:141, Blocked:482 |
| DodgeHeavy vs ReactionDefense | 8 | 88 | 0 | 11.84s | 23.9% | 0.00 | 41.85 | 14.24 | 0.68 | 8 | 0/9,865 (0%) | 0..+50ms:2604, +200..+250ms:932, +150..+200ms:517 | +30..+60ms:356, 0..+30ms:270, +60..+90ms:262 | Opponent.Block:100%, Player.Dodge:94%, Opponent.Hop:0%, Player.Hop:0%, Opponent.Duck:0%, Player.Parry:67%, Player.Block:99%, Player.Duck:0%, Opponent.Parry:61% | Heavy:2103/200, Light:2227/837 | CleanHit:1037, Blocked:2086, Dodged:1055, Parried:32 |
| HeavyBaiting vs AggressiveLightSpam | 14 | 53 | 29 | 2.71s | 92.0% | 0.00 | 9.15 | 0.00 | 9.60 | 3 | 0/2,625 (0%) | 0..+50ms:1162, +300..+350ms:135, +150..+200ms:91 |  |  | Light:1283/1171, Heavy:256/245 | CleanHit:1416 |
| HeavyBaiting vs Balanced | 1 | 92 | 3 | 2.83s | 72.3% | 0.00 | 5.03 | 0.91 | 1.09 | 7 | 0/2,059 (0%) | 0..+50ms:709, +50..+100ms:133, +150..+200ms:120 | +30..+60ms:67, 0..+30ms:50, -60..-30ms:25 | Opponent.Dodge:87%, Opponent.Parry:60%, Opponent.Duck:0%, Opponent.Block:90%, Opponent.Hop:0% | Light:836/641, Heavy:510/324, Unblockable:17/16, Undodgeable:17/17 | CleanHit:998, Dodged:67, Parried:111, Blocked:66 |
| HeavyBaiting vs DodgeHeavy | 0 | 96 | 0 | 2.89s | 56.2% | 0.00 | 5.54 | 0.67 | 0.24 | 2 | 0/1,820 (0%) | 0..+50ms:663, +150..+200ms:164, +50..+100ms:107 | +30..+60ms:69, +60..+90ms:54, 0..+30ms:50 | Opponent.Dodge:87%, Opponent.Block:100%, Opponent.Parry:100%, Opponent.Duck:0%, Opponent.Hop:0% | Light:412/145, Heavy:819/547 | Dodged:271, CleanHit:692, Blocked:110, Parried:6 |
| HeavyBaiting vs HeavyBaiting | 51 | 23 | 22 | 2.63s | 94.3% | 0.00 | 3.06 | 0.00 | 0.06 | 1 | 0/1,230 (0%) | 0..+50ms:957, +150..+200ms:104, +50..+100ms:63 |  |  | Light:706/657, Heavy:514/493 | CleanHit:1150 |
| HeavyBaiting vs ParryFishing | 17 | 78 | 1 | 3.08s | 80.0% | 0.00 | 10.02 | 0.22 | 0.70 | 1 | 0/1,998 (0%) | 0..+50ms:791, -100..-50ms:188, +100..+150ms:133 | +30..+60ms:77, 0..+30ms:53, -60..-30ms:47 | Opponent.Parry:67%, Opponent.Duck:0%, Opponent.Hop:0%, Opponent.Dodge:100% | Light:1173/1040, Heavy:385/206 | CleanHit:1246, Parried:209, Dodged:3 |
| HeavyBaiting vs PassiveBlocking | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 9.93 | 0.00 | 6.67 | 1 | 0/2,784 (0%) | 0..+50ms:2029 |  | Opponent.Block:100% | Light:1471/0, Heavy:577/0 | Blocked:2029 |
| HeavyBaiting vs Random | 1 | 93 | 2 | 3.09s | 68.7% | 0.00 | 3.69 | 0.95 | 0.65 | 4 | 0/2,084 (0%) | 0..+50ms:721, +150..+200ms:119, +50..+100ms:106 | +30..+60ms:50, 0..+30ms:38, +60..+90ms:35 | Opponent.Dodge:81%, Opponent.Duck:26%, Opponent.Hop:24%, Opponent.Parry:58%, Opponent.Block:97% | Light:731/521, Heavy:518/319, Undodgeable:44/36, Unblockable:49/46 | CleanHit:922, Dodged:121, Parried:53, Blocked:110 |
| HeavyBaiting vs ReactionDefense | 0 | 96 | 0 | 4.21s | 60.4% | 0.00 | 10.74 | 5.43 | 0.56 | 6 | 0/3,213 (0%) | 0..+50ms:1382, +300..+350ms:146, +250..+300ms:102 | 0..+30ms:4, +30..+60ms:1, -60..-30ms:1 | Opponent.Block:100%, Opponent.Hop:0%, Opponent.Duck:0%, Opponent.Parry:100% | Light:1486/1088, Heavy:358/26 | CleanHit:1114, Blocked:608, Parried:6 |
| ParryFishing vs AggressiveLightSpam | 90 | 6 | 0 | 3.08s | 77.8% | 0.00 | 15.94 | 0.00 | 11.94 | 4 | 0/3,627 (0%) | 0..+50ms:1416, -100..-50ms:306 | +30..+60ms:138, 0..+30ms:116, -30..0ms:31 | Player.Parry:76%, Player.Hop:0%, Player.Duck:0%, Player.Dodge:100% | Light:1818/1415 | CleanHit:1415, Parried:306, Dodged:1 |
| ParryFishing vs Balanced | 33 | 62 | 1 | 3.91s | 61.8% | 0.00 | 14.71 | 0.95 | 1.20 | 7 | 0/3,663 (0%) | 0..+50ms:1145, -100..-50ms:352, +100..+150ms:131 | +30..+60ms:238, 0..+30ms:181, -30..0ms:98 | Player.Parry:65%, Opponent.Parry:73%, Opponent.Dodge:95%, Player.Duck:0%, Opponent.Duck:0%, Player.Dodge:97%, Opponent.Block:92%, Opponent.Hop:0%, Player.Hop:0% | Light:1807/1130, Heavy:221/132, Undodgeable:25/17, Unblockable:30/8 | CleanHit:1287, Parried:491, Dodged:149, Blocked:49 |
| ParryFishing vs DodgeHeavy | 7 | 89 | 0 | 4.08s | 48.1% | 0.00 | 16.71 | 1.14 | 0.01 | 1 | 0/3,396 (0%) | 0..+50ms:739, +100..+150ms:331, +150..+200ms:264 | +30..+60ms:270, 0..+30ms:145, +60..+90ms:143 | Player.Parry:61%, Opponent.Dodge:95%, Player.Duck:0%, Opponent.Block:100%, Opponent.Hop:0%, Player.Hop:0%, Opponent.Parry:56%, Opponent.Duck:0%, Player.Dodge:100% | Light:1157/462, Heavy:769/464 | CleanHit:926, Dodged:527, Blocked:87, Parried:256 |
| ParryFishing vs HeavyBaiting | 76 | 20 | 0 | 3.07s | 80.5% | 0.00 | 9.81 | 0.17 | 0.54 | 1 | 0/1,984 (0%) | 0..+50ms:835, -100..-50ms:172, +100..+150ms:108 | +30..+60ms:59, 0..+30ms:52, -30..0ms:43 | Player.Parry:61%, Player.Duck:0%, Player.Hop:0%, Player.Dodge:60% | Light:1195/1057, Heavy:380/211 | CleanHit:1268, Parried:187, Dodged:3 |
| ParryFishing vs ParryFishing | 49 | 39 | 8 | 3.94s | 66.8% | 0.00 | 20.90 | 0.00 | 0.00 | 1 | 0/3,351 (0%) | 0..+50ms:1366, -100..-50ms:602, +50..+100ms:229 | +30..+60ms:314, 0..+30ms:247, -30..0ms:95 | Opponent.Parry:69%, Opponent.Dodge:100%, Player.Parry:71%, Player.Duck:0%, Player.Hop:0%, Opponent.Hop:0%, Opponent.Duck:0%, Player.Dodge:100% | Light:2198/1468 | CleanHit:1468, Parried:715, Dodged:15 |
| ParryFishing vs PassiveBlocking | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 27.00 | 0.00 | 7.00 | 0 | 0/3,463 (0%) | 0..+50ms:2688 |  | Opponent.Block:100%, Player.Duck:0% | Light:2688/0 | Blocked:2688 |
| ParryFishing vs Random | 39 | 56 | 1 | 4.30s | 59.4% | 0.00 | 14.13 | 1.19 | 0.19 | 7 | 0/3,790 (0%) | 0..+50ms:983, -100..-50ms:252, +50..+100ms:155 | +30..+60ms:235, 0..+30ms:156, -30..0ms:87 | Player.Parry:65%, Opponent.Dodge:98%, Opponent.Duck:25%, Opponent.Parry:69%, Opponent.Hop:24%, Opponent.Block:98%, Player.Duck:0%, Player.Hop:0%, Player.Dodge:83%, Player.Block:0% | Light:1664/1002, Heavy:278/170, Unblockable:59/26, Undodgeable:80/38 | CleanHit:1236, Dodged:269, Parried:377, Blocked:89 |
| ParryFishing vs ReactionDefense | 53 | 32 | 11 | 3.85s | 70.2% | 0.00 | 20.33 | 0.00 | 0.00 | 1 | 0/4,360 (0%) | 0..+50ms:1791, -100..-50ms:344, +50..+100ms:9 | +30..+60ms:150, 0..+30ms:125, -30..0ms:48 | Player.Parry:66%, Opponent.Block:100%, Opponent.Hop:0%, Player.Duck:0%, Opponent.Parry:73%, Player.Hop:0%, Opponent.Duck:0%, Player.Dodge:100% | Light:2146/1507 | Blocked:277, Parried:349, CleanHit:1507, Dodged:11 |
| PassiveBlocking vs AggressiveLightSpam | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 27.00 | 0.00 | 62.00 | 3 | 0/8,736 (0%) | 0..+50ms:2688 |  | Player.Block:100% | Light:2688/0 | Blocked:2688 |
| PassiveBlocking vs Balanced | 0 | 8 | 88 | 12.09s | 0.3% | 0.00 | 20.51 | 0.00 | 17.77 | 3 | 0/4,255 (0%) | 0..+50ms:2399, -150..-100ms:11 |  | Player.Block:100%, Opponent.Dodge:0%, Opponent.Parry:0%, Opponent.Duck:0% | Heavy:215/0, Light:2214/0, Unblockable:8/8, Undodgeable:11/0 | Blocked:2402, CleanHit:8 |
| PassiveBlocking vs DodgeHeavy | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 12.00 | 0.00 | 9.00 | 0 | 0/2,311 (0%) | 0..+50ms:1248 |  | Player.Block:100%, Opponent.Dodge:0%, Opponent.Duck:0%, Opponent.Parry:0% | Heavy:1344/0 | Blocked:1248 |
| PassiveBlocking vs HeavyBaiting | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 9.97 | 0.00 | 6.88 | 1 | 0/2,784 (0%) | 0..+50ms:2012 |  | Player.Block:100% | Heavy:598/0, Light:1430/0 | Blocked:2012 |
| PassiveBlocking vs ParryFishing | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 27.00 | 0.00 | 7.00 | 0 | 0/3,461 (0%) | 0..+50ms:2688 |  | Player.Block:100%, Opponent.Parry:0%, Opponent.Duck:0% | Light:2688/0 | Blocked:2688 |
| PassiveBlocking vs PassiveBlocking | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 0.00 | 0.00 | 0.00 | 0 | 0/192 (0%) |  |  |  |  |  |
| PassiveBlocking vs Random | 0 | 34 | 62 | 12.49s | 1.6% | 0.00 | 12.44 | 0.00 | 8.33 | 2 | 0/3,053 (0%) | 0..+50ms:2067, -150..-100ms:30 |  | Player.Block:100%, Opponent.Dodge:0%, Opponent.Duck:0% | Heavy:463/0, Light:1619/0, Unblockable:34/34, Undodgeable:30/0 | Blocked:2063, CleanHit:34 |
| PassiveBlocking vs ReactionDefense | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 27.00 | 0.00 | 7.00 | 0 | 0/3,465 (0%) | 0..+50ms:2688 |  | Player.Block:100%, Opponent.Duck:0%, Opponent.Parry:0%, Opponent.Dodge:0% | Light:2688/0 | Blocked:2688 |
| Random vs AggressiveLightSpam | 77 | 16 | 3 | 3.21s | 71.7% | 0.00 | 11.36 | 1.25 | 13.06 | 6 | 0/3,949 (0%) | 0..+50ms:1123, +150..+200ms:181, +450..+500ms:49 | +30..+60ms:75, 0..+30ms:58, +60..+90ms:38 | Player.Block:92%, Player.Parry:63%, Player.Duck:0%, Player.Dodge:91%, Player.Hop:0% | Light:1391/933, Heavy:209/199, Undodgeable:34/29, Unblockable:52/48 | CleanHit:1209, Blocked:159, Parried:83, Dodged:117 |
| Random vs Balanced | 38 | 55 | 3 | 4.77s | 52.0% | 0.00 | 9.20 | 3.73 | 3.05 | 8 | 0/4,413 (0%) | 0..+50ms:1014, +150..+200ms:204, -100..-50ms:150 | +30..+60ms:174, 0..+30ms:110, -30..0ms:94 | Player.Parry:63%, Player.Block:93%, Player.Duck:26%, Opponent.Duck:0%, Opponent.Block:95%, Opponent.Parry:58%, Player.Dodge:85%, Player.Hop:19%, Opponent.Hop:0%, Opponent.Dodge:79% | Light:1350/753, Heavy:550/249, Undodgeable:90/36, Unblockable:108/54 | CleanHit:1092, Blocked:288, Parried:244, Dodged:336 |
| Random vs DodgeHeavy | 19 | 76 | 1 | 5.77s | 38.4% | 0.00 | 12.04 | 3.85 | 1.29 | 6 | 0/4,472 (0%) | 0..+50ms:994, +150..+200ms:238, +200..+250ms:178 | +30..+60ms:188, -30..0ms:138, 0..+30ms:134 | Opponent.Dodge:79%, Player.Parry:58%, Player.Block:99%, Opponent.Block:100%, Player.Hop:25%, Opponent.Parry:33%, Player.Duck:22%, Opponent.Hop:0%, Opponent.Duck:0%, Player.Dodge:87% | Light:595/219, Heavy:1333/555, Unblockable:92/26, Undodgeable:77/6 | Dodged:658, Parried:98, Blocked:402, CleanHit:806 |
| Random vs HeavyBaiting | 96 | 0 | 0 | 3.10s | 68.7% | 0.00 | 3.55 | 0.85 | 0.86 | 6 | 0/2,081 (0%) | 0..+50ms:675, +150..+200ms:129, +50..+100ms:103 | +30..+60ms:56, 0..+30ms:41, +60..+90ms:34 | Player.Dodge:85%, Player.Hop:17%, Player.Parry:71%, Player.Duck:23%, Player.Block:98% | Light:691/484, Heavy:519/323, Undodgeable:62/57, Unblockable:49/44 | CleanHit:908, Dodged:114, Parried:79, Blocked:97 |
| Random vs ParryFishing | 59 | 37 | 0 | 4.28s | 58.3% | 0.00 | 13.96 | 1.10 | 0.24 | 5 | 0/3,791 (0%) | 0..+50ms:933, -100..-50ms:247, +50..+100ms:209 | +30..+60ms:208, 0..+30ms:191, -30..0ms:112 | Player.Hop:24%, Opponent.Hop:0%, Opponent.Parry:63%, Player.Duck:27%, Player.Parry:74%, Player.Dodge:99%, Player.Block:99%, Opponent.Dodge:85%, Opponent.Duck:0% | Heavy:281/172, Light:1664/977, Undodgeable:64/41, Unblockable:75/25 | CleanHit:1215, Dodged:300, Parried:378, Blocked:73 |
| Random vs PassiveBlocking | 32 | 0 | 64 | 12.46s | 1.5% | 0.00 | 12.26 | 0.00 | 8.31 | 2 | 0/3,031 (0%) | 0..+50ms:2047, -150..-100ms:31 |  | Opponent.Block:100%, Player.Duck:0%, Player.Dodge:0%, Player.Parry:0% | Light:1589/0, Undodgeable:31/0, Heavy:478/0, Unblockable:32/32 | Blocked:2046, CleanHit:32 |
| Random vs Random | 38 | 54 | 4 | 4.74s | 53.2% | 0.00 | 6.70 | 3.55 | 1.26 | 7 | 0/3,894 (0%) | 0..+50ms:951, +150..+200ms:124, +50..+100ms:116 | +30..+60ms:143, 0..+30ms:80, +60..+90ms:75 | Player.Dodge:79%, Opponent.Dodge:73%, Player.Hop:24%, Opponent.Hop:18%, Player.Duck:22%, Opponent.Duck:27%, Opponent.Block:98%, Player.Parry:54%, Player.Block:97%, Opponent.Parry:51% | Heavy:572/246, Light:1025/574, Unblockable:123/91, Undodgeable:144/81 | Dodged:314, CleanHit:992, Blocked:281, Parried:148 |
| Random vs ReactionDefense | 19 | 77 | 0 | 7.94s | 41.4% | 0.00 | 19.28 | 9.89 | 1.83 | 8 | 0/6,278 (0%) | 0..+50ms:2098, +50..+100ms:128, +150..+200ms:120 | +30..+60ms:161, 0..+30ms:110, +60..+90ms:84 | Player.Duck:24%, Opponent.Block:99%, Opponent.Parry:68%, Player.Block:94%, Player.Dodge:94%, Opponent.Dodge:77%, Player.Parry:61%, Player.Hop:26%, Opponent.Hop:0%, Opponent.Duck:0% | Heavy:500/42, Light:2314/1137, Undodgeable:89/23, Unblockable:95/39 | CleanHit:1241, Blocked:1169, Parried:172, Dodged:302 |
| ReactionDefense vs AggressiveLightSpam | 78 | 17 | 1 | 4.44s | 64.5% | 0.00 | 20.03 | 5.82 | 18.99 | 8 | 0/5,640 (0%) | 0..+50ms:1941, +50..+100ms:111, +100..+150ms:61 | 0..+30ms:1, +30..+60ms:1 | Player.Block:100%, Player.Hop:0%, Player.Duck:0%, Player.Parry:40% | Light:2230/1438 | CleanHit:1438, Blocked:675, Parried:2 |
| ReactionDefense vs Balanced | 59 | 37 | 0 | 8.35s | 38.6% | 0.00 | 27.06 | 12.60 | 6.02 | 8 | 0/8,256 (0%) | 0..+50ms:2735, -100..-50ms:214, +50..+100ms:169 | 0..+30ms:153, +30..+60ms:151, -30..0ms:72 | Player.Block:99%, Opponent.Dodge:44%, Opponent.Parry:36%, Player.Duck:0%, Opponent.Block:86%, Player.Parry:78%, Opponent.Duck:0%, Opponent.Hop:0%, Player.Hop:0%, Player.Dodge:89% | Light:3118/1319, Heavy:372/33, Undodgeable:34/13, Unblockable:21/5 | CleanHit:1370, Blocked:1581, Parried:323, Dodged:159 |
| ReactionDefense vs DodgeHeavy | 85 | 11 | 0 | 14.97s | 20.1% | 0.00 | 49.40 | 20.33 | 2.13 | 8 | 0/12,762 (0%) | 0..+50ms:3391, +200..+250ms:617, +150..+200ms:547 | +30..+60ms:299, 0..+30ms:244, +60..+90ms:233 | Opponent.Dodge:41%, Player.Block:100%, Opponent.Block:99%, Opponent.Duck:0%, Opponent.Hop:0%, Player.Duck:0%, Player.Hop:0%, Player.Parry:55%, Opponent.Parry:37%, Player.Dodge:0% | Light:2633/826, Heavy:2422/192 | Dodged:1007, Blocked:2879, CleanHit:1018, Parried:30 |
| ReactionDefense vs HeavyBaiting | 96 | 0 | 0 | 4.09s | 63.6% | 0.00 | 10.55 | 4.98 | 0.59 | 6 | 0/3,113 (0%) | 0..+50ms:1349, +300..+350ms:122, +250..+300ms:107 | -30..0ms:2, +30..+60ms:2, -60..-30ms:1 | Player.Block:100%, Player.Hop:0%, Player.Duck:0%, Player.Parry:62% | Light:1439/1114, Heavy:366/34 | CleanHit:1148, Blocked:540, Parried:5 |
| ReactionDefense vs ParryFishing | 37 | 55 | 4 | 3.80s | 69.4% | 0.00 | 20.06 | 0.00 | 0.00 | 1 | 0/4,274 (0%) | 0..+50ms:1454, -100..-50ms:337, +50..+100ms:327 | +30..+60ms:149, 0..+30ms:126, -30..0ms:44 | Opponent.Parry:68%, Player.Duck:0%, Player.Block:100%, Opponent.Hop:0%, Player.Parry:67%, Opponent.Dodge:100%, Player.Hop:0%, Opponent.Duck:0% | Light:2120/1471 | Parried:339, CleanHit:1471, Blocked:300, Dodged:8 |
| ReactionDefense vs PassiveBlocking | 0 | 0 | 96 | 12.00s | 0.0% | 0.00 | 27.00 | 0.00 | 7.00 | 0 | 0/3,464 (0%) | 0..+50ms:2688 |  | Opponent.Block:100%, Player.Dodge:0%, Player.Parry:0% | Light:2688/0 | Blocked:2688 |
| ReactionDefense vs Random | 68 | 25 | 3 | 8.14s | 39.5% | 0.00 | 22.29 | 12.18 | 1.80 | 8 | 0/7,649 (0%) | 0..+50ms:2178, +50..+100ms:153, +150..+200ms:136 | 0..+30ms:166, +30..+60ms:154, +60..+90ms:88 | Opponent.Duck:15%, Player.Block:99%, Opponent.Parry:49%, Opponent.Dodge:50%, Opponent.Hop:15%, Player.Parry:64%, Opponent.Block:93%, Player.Dodge:85%, Player.Duck:0%, Player.Hop:0% | Light:2548/1171, Unblockable:112/39, Undodgeable:75/28, Heavy:498/39 | CleanHit:1277, Blocked:1278, Parried:221, Dodged:347 |
| ReactionDefense vs ReactionDefense | 43 | 38 | 15 | 3.76s | 70.0% | 0.00 | 19.85 | 0.00 | 0.00 | 1 | 0/5,349 (0%) | 0..+50ms:2072, -100..-50ms:14, +50..+100ms:12 | +30..+60ms:6, -60..-30ms:4, 0..+30ms:2 | Opponent.Block:100%, Player.Block:100%, Player.Duck:0%, Player.Hop:0%, Opponent.Duck:0%, Player.Parry:29%, Opponent.Hop:0%, Opponent.Parry:80% | Light:2100/1471 | Blocked:613, CleanHit:1471, Parried:14 |

## Reproducible exploit / edge-case examples

### repeated attack-buffer overflow: 5 rejected inputs: AggressiveLightSpam vs HeavyBaiting
Profile `Read100Hit20`, seed `15624546100296051410`, duration 2.57s.

```text
0.283s Opponent's Low Light HIT at 0.180s; frame advantage 0ms (simultaneous readiness).
0.283s Player stamina -10 (Light; 52.8/100).
0.283s Player launched Mid Light at 0.283s; impact scheduled for 0.463s.
0.400s Player queued Mid Light (2/8).
0.433s Opponent queued Mid Heavy (1/8).
0.433s Opponent stamina -20 (Heavy; 44.3/100).
0.433s Opponent launched Mid Heavy at 0.433s; impact scheduled for 0.833s.
0.533s Player queued Mid Light (3/8).
0.567s Opponent stamina +20 (Light CleanHit; 65.7/100).
0.567s Initiative → Player (+6).
0.567s Player's Mid Light HIT at 0.463s; frame advantage +320ms (attacker ready first).
0.567s Player stamina -10 (Light; 45.7/100).
0.567s Player launched Mid Light at 0.567s; impact scheduled for 0.747s.
0.667s Player queued Mid Light (3/8).
0.800s Player queued Mid Light (4/8).
0.850s Opponent stamina +20 (Light CleanHit; 88.5/100).
0.850s Initiative → Player (+14).
0.850s Player's Mid Light HIT at 0.747s; frame advantage +37ms (attacker ready first).
0.867s Opponent queued High Heavy (1/8).
0.933s Player stamina +20 (Heavy CleanHit; 69.3/100).
0.933s Initiative → Player (+8).
0.933s Opponent's Mid Heavy HIT at 0.833s; frame advantage 0ms (simultaneous readiness).
0.933s Player stamina -10 (Light; 59.3/100).
0.933s Player launched Mid Light at 0.933s; impact scheduled for 1.113s.
0.933s Opponent stamina -20 (Heavy; 69.3/100).
0.933s Opponent launched High Heavy at 0.933s; impact scheduled for 1.333s.
0.933s Player queued Mid Light (4/8).
1.067s Player queued Mid Light (5/8).
1.200s Player queued Mid Light (6/8).
1.217s Opponent stamina +20 (Light CleanHit; 92.2/100).
1.217s Initiative → Player (+14).
1.217s Player's Mid Light HIT at 1.113s; frame advantage +170ms (attacker ready first).
1.217s Player stamina -10 (Light; 52.2/100).
1.217s Player launched Mid Light at 1.217s; impact scheduled for 1.397s.
1.300s Opponent queued High Heavy (1/8).
1.333s Player cannot queue Light: needs 10, 3.3 available.
1.433s Player stamina +20 (Heavy CleanHit; 74.3/100).
1.433s Initiative → Player (+10).
1.433s Opponent's High Heavy HIT at 1.333s; frame advantage +13ms (attacker ready first).
1.467s Player queued Mid Light (6/8).
1.500s Opponent stamina +5 (Light CleanHit; 100/100, 15 wasted at cap).
1.500s Initiative → Player (+16).
1.500s Player's Mid Light HIT at 1.397s; frame advantage 0ms (simultaneous readiness).
1.500s Player stamina -10 (Light; 65/100).
1.500s Player launched Mid Light at 1.500s; impact scheduled for 1.680s.
1.500s Opponent stamina -20 (Heavy; 80/100).
1.500s Opponent launched High Heavy at 1.500s; impact scheduled for 1.900s.
1.600s Player queued Mid Light (6/8).
1.733s Player cannot queue Light: needs 10, 7.3 available.
1.733s Opponent queued High Heavy (1/8).
1.783s Opponent stamina +17.2 (Light CleanHit; 100/100, 2.8 wasted at cap).
1.783s Initiative → Player (+22).
1.783s Player's Mid Light HIT at 1.680s; frame advantage +170ms (attacker ready first).
1.783s Player stamina -10 (Light; 57.8/100).
1.783s Player launched Mid Light at 1.783s; impact scheduled for 1.963s.
1.867s Player cannot queue Light: needs 10, 8.7 available.
2.000s Player stamina +20 (Heavy CleanHit; 80/100).
2.000s Initiative → Player (+18).
2.000s Opponent's High Heavy HIT at 1.900s; frame advantage +13ms (attacker ready first).
2.000s Player queued Mid Light (6/8).
2.067s Opponent stamina +0 (Light CleanHit; 100/100, 20 wasted at cap).
2.067s Initiative → Player (+24).
2.067s Player's Mid Light HIT at 1.963s; frame advantage 0ms (simultaneous readiness).
2.067s Player stamina -10 (Light; 70.7/100).
2.067s Player launched Mid Light at 2.067s; impact scheduled for 2.247s.
2.067s Opponent stamina -20 (Heavy; 80/100).
2.067s Opponent launched High Heavy at 2.067s; impact scheduled for 2.467s.
2.133s Player queued Mid Light (6/8).
2.167s Opponent queued Low Light (1/8).
2.267s Player queued Mid Light (7/8).
2.350s Opponent stamina +17.2 (Light CleanHit; 100/100, 2.8 wasted at cap).
2.350s Initiative → Player (+30).
2.350s Player's Mid Light HIT at 2.247s; frame advantage +170ms (attacker ready first).
2.350s Player stamina -10 (Light; 63.5/100).
2.350s Player launched Mid Light at 2.350s; impact scheduled for 2.530s.
2.400s Player cannot queue Light: needs 10, 4 available.
2.533s Player cannot queue Light: needs 10, 5.3 available.
2.567s Player stamina +20 (Heavy CleanHit; 85.7/100).
2.567s Initiative → Player (+26).
2.567s Opponent's High Heavy HIT at 2.467s; frame advantage +13ms (attacker ready first).
```

### repeated attack-buffer overflow: 5 rejected inputs: AggressiveLightSpam vs HeavyBaiting
Profile `Read100Hit20`, seed `3580683999970202515`, duration 2.55s.

```text
0.500s Player stamina +20 (Heavy CleanHit; 55/100).
0.500s Initiative → Even (+2).
0.500s Opponent's Low Heavy HIT at 0.400s; frame advantage +13ms (attacker ready first).
0.533s Player queued Mid Light (3/8).
0.567s Opponent stamina +20 (Light CleanHit; 75.7/100).
0.567s Initiative → Player (+8).
0.567s Player's Mid Light HIT at 0.463s; frame advantage 0ms (simultaneous readiness).
0.567s Player stamina -10 (Light; 45.7/100).
0.567s Player launched Mid Light at 0.567s; impact scheduled for 0.747s.
0.567s Opponent stamina -20 (Heavy; 55.7/100).
0.567s Opponent launched High Heavy at 0.567s; impact scheduled for 0.967s.
0.667s Player queued Mid Light (3/8).
0.800s Player queued Mid Light (4/8).
0.850s Opponent stamina +20 (Light CleanHit; 78.5/100).
0.850s Initiative → Player (+14).
0.850s Player's Mid Light HIT at 0.747s; frame advantage +170ms (attacker ready first).
0.850s Player stamina -10 (Light; 38.5/100).
0.850s Player launched Mid Light at 0.850s; impact scheduled for 1.030s.
0.867s Opponent queued Low Heavy (1/8).
0.933s Player cannot queue Light: needs 10, 9.3 available.
1.067s Player stamina +20 (Heavy CleanHit; 60.7/100).
1.067s Initiative → Player (+10).
1.067s Opponent's High Heavy HIT at 0.967s; frame advantage +13ms (attacker ready first).
1.067s Player queued Mid Light (4/8).
1.133s Opponent stamina +18.7 (Light CleanHit; 100/100, 1.3 wasted at cap).
1.133s Initiative → Player (+16).
1.133s Player's Mid Light HIT at 1.030s; frame advantage 0ms (simultaneous readiness).
1.133s Player stamina -10 (Light; 51.3/100).
1.133s Player launched Mid Light at 1.133s; impact scheduled for 1.313s.
1.133s Opponent stamina -20 (Heavy; 80/100).
1.133s Opponent launched Low Heavy at 1.133s; impact scheduled for 1.533s.
1.200s Player queued Mid Light (4/8).
1.300s Opponent queued Mid Heavy (1/8).
1.333s Player queued Mid Light (5/8).
1.417s Opponent stamina +17.2 (Light CleanHit; 100/100, 2.8 wasted at cap).
1.417s Initiative → Player (+22).
1.417s Player's Mid Light HIT at 1.313s; frame advantage +170ms (attacker ready first).
1.417s Player stamina -10 (Light; 44.2/100).
1.417s Player launched Mid Light at 1.417s; impact scheduled for 1.597s.
1.467s Player cannot queue Light: needs 10, 4.7 available.
1.600s Player cannot queue Light: needs 10, 6 available.
1.633s Player stamina +20 (Heavy CleanHit; 66.3/100).
1.633s Initiative → Player (+18).
1.633s Opponent's Low Heavy HIT at 1.533s; frame advantage +13ms (attacker ready first).
1.700s Opponent stamina +0 (Light CleanHit; 100/100, 20 wasted at cap).
1.700s Initiative → Player (+24).
1.700s Player's Mid Light HIT at 1.597s; frame advantage 0ms (simultaneous readiness).
1.700s Player stamina -10 (Light; 57/100).
1.700s Player launched Mid Light at 1.700s; impact scheduled for 1.880s.
1.700s Opponent stamina -20 (Heavy; 80/100).
1.700s Opponent launched Mid Heavy at 1.700s; impact scheduled for 2.100s.
1.733s Player queued Mid Light (4/8).
1.733s Opponent queued Low Heavy (1/8).
1.867s Player queued Mid Light (5/8).
1.983s Opponent stamina +17.2 (Light CleanHit; 100/100, 2.8 wasted at cap).
1.983s Initiative → Player (+30).
1.983s Player's Mid Light HIT at 1.880s; frame advantage +170ms (attacker ready first).
1.983s Player stamina -10 (Light; 49.8/100).
1.983s Player launched Mid Light at 1.983s; impact scheduled for 2.163s.
2.000s Player queued Mid Light (5/8).
2.133s Player cannot queue Light: needs 10, 1.3 available.
2.167s Opponent queued Mid Heavy (2/8).
2.200s Player stamina +20 (Heavy CleanHit; 72/100).
2.200s Initiative → Player (+26).
2.200s Opponent's Mid Heavy HIT at 2.100s; frame advantage +13ms (attacker ready first).
2.267s Opponent stamina +0 (Light CleanHit; 100/100, 20 wasted at cap).
2.267s Initiative → Player (+32).
2.267s Player's Mid Light HIT at 2.163s; frame advantage 0ms (simultaneous readiness).
2.267s Player stamina -10 (Light; 62.7/100).
2.267s Player launched Mid Light at 2.267s; impact scheduled for 2.447s.
2.267s Opponent stamina -20 (Heavy; 80/100).
2.267s Opponent launched Low Heavy at 2.267s; impact scheduled for 2.667s.
2.267s Player queued Mid Light (5/8).
2.400s Player queued Mid Light (6/8).
2.533s Player cannot queue Light: needs 10, 5.3 available.
2.550s Opponent stamina +17.2 (Light CleanHit; 100/100, 2.8 wasted at cap).
2.550s Initiative → Player (+38).
2.550s Player's Mid Light HIT at 2.447s; frame advantage +170ms (attacker ready first).
2.550s Player stamina -10 (Light; 55.5/100).
2.550s Player launched Mid Light at 2.550s; impact scheduled for 2.730s.
```

### repeated attack-buffer overflow: 5 rejected inputs: AggressiveLightSpam vs HeavyBaiting
Profile `Read100Hit20`, seed `1632192385739882797`, duration 2.57s.

```text
0.283s Opponent's Low Light HIT at 0.180s; frame advantage 0ms (simultaneous readiness).
0.283s Player stamina -10 (Light; 52.8/100).
0.283s Player launched Mid Light at 0.283s; impact scheduled for 0.463s.
0.400s Player queued Mid Light (2/8).
0.433s Opponent queued Low Heavy (1/8).
0.433s Opponent stamina -20 (Heavy; 44.3/100).
0.433s Opponent launched Low Heavy at 0.433s; impact scheduled for 0.833s.
0.533s Player queued Mid Light (3/8).
0.567s Opponent stamina +20 (Light CleanHit; 65.7/100).
0.567s Initiative → Player (+6).
0.567s Player's Mid Light HIT at 0.463s; frame advantage +320ms (attacker ready first).
0.567s Player stamina -10 (Light; 45.7/100).
0.567s Player launched Mid Light at 0.567s; impact scheduled for 0.747s.
0.667s Player queued Mid Light (3/8).
0.800s Player queued Mid Light (4/8).
0.850s Opponent stamina +20 (Light CleanHit; 88.5/100).
0.850s Initiative → Player (+14).
0.850s Player's Mid Light HIT at 0.747s; frame advantage +37ms (attacker ready first).
0.867s Opponent queued High Heavy (1/8).
0.933s Player stamina +20 (Heavy CleanHit; 69.3/100).
0.933s Initiative → Player (+8).
0.933s Opponent's Low Heavy HIT at 0.833s; frame advantage 0ms (simultaneous readiness).
0.933s Player stamina -10 (Light; 59.3/100).
0.933s Player launched Mid Light at 0.933s; impact scheduled for 1.113s.
0.933s Opponent stamina -20 (Heavy; 69.3/100).
0.933s Opponent launched High Heavy at 0.933s; impact scheduled for 1.333s.
0.933s Player queued Mid Light (4/8).
1.067s Player queued Mid Light (5/8).
1.200s Player queued Mid Light (6/8).
1.217s Opponent stamina +20 (Light CleanHit; 92.2/100).
1.217s Initiative → Player (+14).
1.217s Player's Mid Light HIT at 1.113s; frame advantage +170ms (attacker ready first).
1.217s Player stamina -10 (Light; 52.2/100).
1.217s Player launched Mid Light at 1.217s; impact scheduled for 1.397s.
1.300s Opponent queued Low Heavy (1/8).
1.333s Player cannot queue Light: needs 10, 3.3 available.
1.433s Player stamina +20 (Heavy CleanHit; 74.3/100).
1.433s Initiative → Player (+10).
1.433s Opponent's High Heavy HIT at 1.333s; frame advantage +13ms (attacker ready first).
1.467s Player queued Mid Light (6/8).
1.500s Opponent stamina +5 (Light CleanHit; 100/100, 15 wasted at cap).
1.500s Initiative → Player (+16).
1.500s Player's Mid Light HIT at 1.397s; frame advantage 0ms (simultaneous readiness).
1.500s Player stamina -10 (Light; 65/100).
1.500s Player launched Mid Light at 1.500s; impact scheduled for 1.680s.
1.500s Opponent stamina -20 (Heavy; 80/100).
1.500s Opponent launched Low Heavy at 1.500s; impact scheduled for 1.900s.
1.600s Player queued Mid Light (6/8).
1.733s Player cannot queue Light: needs 10, 7.3 available.
1.733s Opponent queued High Heavy (1/8).
1.783s Opponent stamina +17.2 (Light CleanHit; 100/100, 2.8 wasted at cap).
1.783s Initiative → Player (+22).
1.783s Player's Mid Light HIT at 1.680s; frame advantage +170ms (attacker ready first).
1.783s Player stamina -10 (Light; 57.8/100).
1.783s Player launched Mid Light at 1.783s; impact scheduled for 1.963s.
1.867s Player cannot queue Light: needs 10, 8.7 available.
2.000s Player stamina +20 (Heavy CleanHit; 80/100).
2.000s Initiative → Player (+18).
2.000s Opponent's Low Heavy HIT at 1.900s; frame advantage +13ms (attacker ready first).
2.000s Player queued Mid Light (6/8).
2.067s Opponent stamina +0 (Light CleanHit; 100/100, 20 wasted at cap).
2.067s Initiative → Player (+24).
2.067s Player's Mid Light HIT at 1.963s; frame advantage 0ms (simultaneous readiness).
2.067s Player stamina -10 (Light; 70.7/100).
2.067s Player launched Mid Light at 2.067s; impact scheduled for 2.247s.
2.067s Opponent stamina -20 (Heavy; 80/100).
2.067s Opponent launched High Heavy at 2.067s; impact scheduled for 2.467s.
2.133s Player queued Mid Light (6/8).
2.167s Opponent queued High Light (1/8).
2.267s Player queued Mid Light (7/8).
2.350s Opponent stamina +17.2 (Light CleanHit; 100/100, 2.8 wasted at cap).
2.350s Initiative → Player (+30).
2.350s Player's Mid Light HIT at 2.247s; frame advantage +170ms (attacker ready first).
2.350s Player stamina -10 (Light; 63.5/100).
2.350s Player launched Mid Light at 2.350s; impact scheduled for 2.530s.
2.400s Player cannot queue Light: needs 10, 4 available.
2.533s Player cannot queue Light: needs 10, 5.3 available.
2.567s Player stamina +20 (Heavy CleanHit; 85.7/100).
2.567s Initiative → Player (+26).
2.567s Opponent's High Heavy HIT at 2.467s; frame advantage +13ms (attacker ready first).
```

### repeated attack-buffer overflow: 74 rejected inputs: AggressiveLightSpam vs PassiveBlocking
Profile `Read100Hit20`, seed `10134396874346711864`, duration 12.00s.

```text
5.350s Player's Mid Light BLOCKED at 5.247s; frame advantage 0ms (simultaneous readiness).
5.467s Player cannot queue Light: needs 10, 4.7 available.
5.600s Player cannot queue Light: needs 10, 6 available.
5.733s Player cannot queue Light: needs 10, 7.3 available.
5.867s Player cannot queue Light: needs 10, 8.7 available.
6.000s Player queued Mid Light (1/8).
6.000s Player stamina -10 (Light; 0/100).
6.000s Player launched Mid Light at 6.000s; impact scheduled for 6.180s.
6.133s Player cannot queue Light: needs 10, 1.3 available.
6.267s Player cannot queue Light: needs 10, 2.7 available.
6.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
6.283s Initiative → Opponent (-66).
6.283s Player's Mid Light BLOCKED at 6.180s; frame advantage 0ms (simultaneous readiness).
6.400s Player cannot queue Light: needs 10, 4 available.
6.533s Player cannot queue Light: needs 10, 5.3 available.
6.667s Player cannot queue Light: needs 10, 6.7 available.
6.800s Player cannot queue Light: needs 10, 8 available.
6.933s Player cannot queue Light: needs 10, 9.3 available.
7.067s Player queued Mid Light (1/8).
7.067s Player stamina -10 (Light; 0.7/100).
7.067s Player launched Mid Light at 7.067s; impact scheduled for 7.247s.
7.200s Player cannot queue Light: needs 10, 2 available.
7.333s Player cannot queue Light: needs 10, 3.3 available.
7.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
7.350s Initiative → Opponent (-72).
7.350s Player's Mid Light BLOCKED at 7.247s; frame advantage 0ms (simultaneous readiness).
7.467s Player cannot queue Light: needs 10, 4.7 available.
7.600s Player cannot queue Light: needs 10, 6 available.
7.733s Player cannot queue Light: needs 10, 7.3 available.
7.867s Player cannot queue Light: needs 10, 8.7 available.
8.000s Player queued Mid Light (1/8).
8.000s Player stamina -10 (Light; 0/100).
8.000s Player launched Mid Light at 8.000s; impact scheduled for 8.180s.
8.133s Player cannot queue Light: needs 10, 1.3 available.
8.267s Player cannot queue Light: needs 10, 2.7 available.
8.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
8.283s Initiative → Opponent (-78).
8.283s Player's Mid Light BLOCKED at 8.180s; frame advantage 0ms (simultaneous readiness).
8.400s Player cannot queue Light: needs 10, 4 available.
8.533s Player cannot queue Light: needs 10, 5.3 available.
8.667s Player cannot queue Light: needs 10, 6.7 available.
8.800s Player cannot queue Light: needs 10, 8 available.
8.933s Player cannot queue Light: needs 10, 9.3 available.
9.067s Player queued Mid Light (1/8).
9.067s Player stamina -10 (Light; 0.7/100).
9.067s Player launched Mid Light at 9.067s; impact scheduled for 9.247s.
9.200s Player cannot queue Light: needs 10, 2 available.
9.333s Player cannot queue Light: needs 10, 3.3 available.
9.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
9.350s Initiative → Opponent (-84).
9.350s Player's Mid Light BLOCKED at 9.247s; frame advantage 0ms (simultaneous readiness).
9.467s Player cannot queue Light: needs 10, 4.7 available.
9.600s Player cannot queue Light: needs 10, 6 available.
9.733s Player cannot queue Light: needs 10, 7.3 available.
9.867s Player cannot queue Light: needs 10, 8.7 available.
10.000s Player queued Mid Light (1/8).
10.000s Player stamina -10 (Light; 0/100).
10.000s Player launched Mid Light at 10.000s; impact scheduled for 10.180s.
10.133s Player cannot queue Light: needs 10, 1.3 available.
10.267s Player cannot queue Light: needs 10, 2.7 available.
10.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
10.283s Initiative → Opponent (-90).
10.283s Player's Mid Light BLOCKED at 10.180s; frame advantage 0ms (simultaneous readiness).
10.400s Player cannot queue Light: needs 10, 4 available.
10.533s Player cannot queue Light: needs 10, 5.3 available.
10.667s Player cannot queue Light: needs 10, 6.7 available.
10.800s Player cannot queue Light: needs 10, 8 available.
10.933s Player cannot queue Light: needs 10, 9.3 available.
11.067s Player queued Mid Light (1/8).
11.067s Player stamina -10 (Light; 0.7/100).
11.067s Player launched Mid Light at 11.067s; impact scheduled for 11.247s.
11.200s Player cannot queue Light: needs 10, 2 available.
11.333s Player cannot queue Light: needs 10, 3.3 available.
11.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
11.350s Initiative → Opponent (-96).
11.350s Player's Mid Light BLOCKED at 11.247s; frame advantage 0ms (simultaneous readiness).
11.467s Player cannot queue Light: needs 10, 4.7 available.
11.600s Player cannot queue Light: needs 10, 6 available.
11.733s Player cannot queue Light: needs 10, 7.3 available.
11.867s Player cannot queue Light: needs 10, 8.7 available.
```

### repeated attack-buffer overflow: 74 rejected inputs: AggressiveLightSpam vs PassiveBlocking
Profile `Read100Hit20`, seed `326157930810486800`, duration 12.00s.

```text
5.350s Player's Mid Light BLOCKED at 5.247s; frame advantage 0ms (simultaneous readiness).
5.467s Player cannot queue Light: needs 10, 4.7 available.
5.600s Player cannot queue Light: needs 10, 6 available.
5.733s Player cannot queue Light: needs 10, 7.3 available.
5.867s Player cannot queue Light: needs 10, 8.7 available.
6.000s Player queued Mid Light (1/8).
6.000s Player stamina -10 (Light; 0/100).
6.000s Player launched Mid Light at 6.000s; impact scheduled for 6.180s.
6.133s Player cannot queue Light: needs 10, 1.3 available.
6.267s Player cannot queue Light: needs 10, 2.7 available.
6.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
6.283s Initiative → Opponent (-66).
6.283s Player's Mid Light BLOCKED at 6.180s; frame advantage 0ms (simultaneous readiness).
6.400s Player cannot queue Light: needs 10, 4 available.
6.533s Player cannot queue Light: needs 10, 5.3 available.
6.667s Player cannot queue Light: needs 10, 6.7 available.
6.800s Player cannot queue Light: needs 10, 8 available.
6.933s Player cannot queue Light: needs 10, 9.3 available.
7.067s Player queued Mid Light (1/8).
7.067s Player stamina -10 (Light; 0.7/100).
7.067s Player launched Mid Light at 7.067s; impact scheduled for 7.247s.
7.200s Player cannot queue Light: needs 10, 2 available.
7.333s Player cannot queue Light: needs 10, 3.3 available.
7.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
7.350s Initiative → Opponent (-72).
7.350s Player's Mid Light BLOCKED at 7.247s; frame advantage 0ms (simultaneous readiness).
7.467s Player cannot queue Light: needs 10, 4.7 available.
7.600s Player cannot queue Light: needs 10, 6 available.
7.733s Player cannot queue Light: needs 10, 7.3 available.
7.867s Player cannot queue Light: needs 10, 8.7 available.
8.000s Player queued Mid Light (1/8).
8.000s Player stamina -10 (Light; 0/100).
8.000s Player launched Mid Light at 8.000s; impact scheduled for 8.180s.
8.133s Player cannot queue Light: needs 10, 1.3 available.
8.267s Player cannot queue Light: needs 10, 2.7 available.
8.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
8.283s Initiative → Opponent (-78).
8.283s Player's Mid Light BLOCKED at 8.180s; frame advantage 0ms (simultaneous readiness).
8.400s Player cannot queue Light: needs 10, 4 available.
8.533s Player cannot queue Light: needs 10, 5.3 available.
8.667s Player cannot queue Light: needs 10, 6.7 available.
8.800s Player cannot queue Light: needs 10, 8 available.
8.933s Player cannot queue Light: needs 10, 9.3 available.
9.067s Player queued Mid Light (1/8).
9.067s Player stamina -10 (Light; 0.7/100).
9.067s Player launched Mid Light at 9.067s; impact scheduled for 9.247s.
9.200s Player cannot queue Light: needs 10, 2 available.
9.333s Player cannot queue Light: needs 10, 3.3 available.
9.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
9.350s Initiative → Opponent (-84).
9.350s Player's Mid Light BLOCKED at 9.247s; frame advantage 0ms (simultaneous readiness).
9.467s Player cannot queue Light: needs 10, 4.7 available.
9.600s Player cannot queue Light: needs 10, 6 available.
9.733s Player cannot queue Light: needs 10, 7.3 available.
9.867s Player cannot queue Light: needs 10, 8.7 available.
10.000s Player queued Mid Light (1/8).
10.000s Player stamina -10 (Light; 0/100).
10.000s Player launched Mid Light at 10.000s; impact scheduled for 10.180s.
10.133s Player cannot queue Light: needs 10, 1.3 available.
10.267s Player cannot queue Light: needs 10, 2.7 available.
10.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
10.283s Initiative → Opponent (-90).
10.283s Player's Mid Light BLOCKED at 10.180s; frame advantage 0ms (simultaneous readiness).
10.400s Player cannot queue Light: needs 10, 4 available.
10.533s Player cannot queue Light: needs 10, 5.3 available.
10.667s Player cannot queue Light: needs 10, 6.7 available.
10.800s Player cannot queue Light: needs 10, 8 available.
10.933s Player cannot queue Light: needs 10, 9.3 available.
11.067s Player queued Mid Light (1/8).
11.067s Player stamina -10 (Light; 0.7/100).
11.067s Player launched Mid Light at 11.067s; impact scheduled for 11.247s.
11.200s Player cannot queue Light: needs 10, 2 available.
11.333s Player cannot queue Light: needs 10, 3.3 available.
11.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
11.350s Initiative → Opponent (-96).
11.350s Player's Mid Light BLOCKED at 11.247s; frame advantage 0ms (simultaneous readiness).
11.467s Player cannot queue Light: needs 10, 4.7 available.
11.600s Player cannot queue Light: needs 10, 6 available.
11.733s Player cannot queue Light: needs 10, 7.3 available.
11.867s Player cannot queue Light: needs 10, 8.7 available.
```

### repeated attack-buffer overflow: 74 rejected inputs: AggressiveLightSpam vs PassiveBlocking
Profile `Read100Hit20`, seed `2785425917384929038`, duration 12.00s.

```text
5.350s Player's Mid Light BLOCKED at 5.247s; frame advantage 0ms (simultaneous readiness).
5.467s Player cannot queue Light: needs 10, 4.7 available.
5.600s Player cannot queue Light: needs 10, 6 available.
5.733s Player cannot queue Light: needs 10, 7.3 available.
5.867s Player cannot queue Light: needs 10, 8.7 available.
6.000s Player queued Mid Light (1/8).
6.000s Player stamina -10 (Light; 0/100).
6.000s Player launched Mid Light at 6.000s; impact scheduled for 6.180s.
6.133s Player cannot queue Light: needs 10, 1.3 available.
6.267s Player cannot queue Light: needs 10, 2.7 available.
6.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
6.283s Initiative → Opponent (-66).
6.283s Player's Mid Light BLOCKED at 6.180s; frame advantage 0ms (simultaneous readiness).
6.400s Player cannot queue Light: needs 10, 4 available.
6.533s Player cannot queue Light: needs 10, 5.3 available.
6.667s Player cannot queue Light: needs 10, 6.7 available.
6.800s Player cannot queue Light: needs 10, 8 available.
6.933s Player cannot queue Light: needs 10, 9.3 available.
7.067s Player queued Mid Light (1/8).
7.067s Player stamina -10 (Light; 0.7/100).
7.067s Player launched Mid Light at 7.067s; impact scheduled for 7.247s.
7.200s Player cannot queue Light: needs 10, 2 available.
7.333s Player cannot queue Light: needs 10, 3.3 available.
7.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
7.350s Initiative → Opponent (-72).
7.350s Player's Mid Light BLOCKED at 7.247s; frame advantage 0ms (simultaneous readiness).
7.467s Player cannot queue Light: needs 10, 4.7 available.
7.600s Player cannot queue Light: needs 10, 6 available.
7.733s Player cannot queue Light: needs 10, 7.3 available.
7.867s Player cannot queue Light: needs 10, 8.7 available.
8.000s Player queued Mid Light (1/8).
8.000s Player stamina -10 (Light; 0/100).
8.000s Player launched Mid Light at 8.000s; impact scheduled for 8.180s.
8.133s Player cannot queue Light: needs 10, 1.3 available.
8.267s Player cannot queue Light: needs 10, 2.7 available.
8.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
8.283s Initiative → Opponent (-78).
8.283s Player's Mid Light BLOCKED at 8.180s; frame advantage 0ms (simultaneous readiness).
8.400s Player cannot queue Light: needs 10, 4 available.
8.533s Player cannot queue Light: needs 10, 5.3 available.
8.667s Player cannot queue Light: needs 10, 6.7 available.
8.800s Player cannot queue Light: needs 10, 8 available.
8.933s Player cannot queue Light: needs 10, 9.3 available.
9.067s Player queued Mid Light (1/8).
9.067s Player stamina -10 (Light; 0.7/100).
9.067s Player launched Mid Light at 9.067s; impact scheduled for 9.247s.
9.200s Player cannot queue Light: needs 10, 2 available.
9.333s Player cannot queue Light: needs 10, 3.3 available.
9.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
9.350s Initiative → Opponent (-84).
9.350s Player's Mid Light BLOCKED at 9.247s; frame advantage 0ms (simultaneous readiness).
9.467s Player cannot queue Light: needs 10, 4.7 available.
9.600s Player cannot queue Light: needs 10, 6 available.
9.733s Player cannot queue Light: needs 10, 7.3 available.
9.867s Player cannot queue Light: needs 10, 8.7 available.
10.000s Player queued Mid Light (1/8).
10.000s Player stamina -10 (Light; 0/100).
10.000s Player launched Mid Light at 10.000s; impact scheduled for 10.180s.
10.133s Player cannot queue Light: needs 10, 1.3 available.
10.267s Player cannot queue Light: needs 10, 2.7 available.
10.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
10.283s Initiative → Opponent (-90).
10.283s Player's Mid Light BLOCKED at 10.180s; frame advantage 0ms (simultaneous readiness).
10.400s Player cannot queue Light: needs 10, 4 available.
10.533s Player cannot queue Light: needs 10, 5.3 available.
10.667s Player cannot queue Light: needs 10, 6.7 available.
10.800s Player cannot queue Light: needs 10, 8 available.
10.933s Player cannot queue Light: needs 10, 9.3 available.
11.067s Player queued Mid Light (1/8).
11.067s Player stamina -10 (Light; 0.7/100).
11.067s Player launched Mid Light at 11.067s; impact scheduled for 11.247s.
11.200s Player cannot queue Light: needs 10, 2 available.
11.333s Player cannot queue Light: needs 10, 3.3 available.
11.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
11.350s Initiative → Opponent (-96).
11.350s Player's Mid Light BLOCKED at 11.247s; frame advantage 0ms (simultaneous readiness).
11.467s Player cannot queue Light: needs 10, 4.7 available.
11.600s Player cannot queue Light: needs 10, 6 available.
11.733s Player cannot queue Light: needs 10, 7.3 available.
11.867s Player cannot queue Light: needs 10, 8.7 available.
```

### repeated attack-buffer overflow: 74 rejected inputs: AggressiveLightSpam vs PassiveBlocking
Profile `Read100Hit20`, seed `2407760388135473087`, duration 12.00s.

```text
5.350s Player's Mid Light BLOCKED at 5.247s; frame advantage 0ms (simultaneous readiness).
5.467s Player cannot queue Light: needs 10, 4.7 available.
5.600s Player cannot queue Light: needs 10, 6 available.
5.733s Player cannot queue Light: needs 10, 7.3 available.
5.867s Player cannot queue Light: needs 10, 8.7 available.
6.000s Player queued Mid Light (1/8).
6.000s Player stamina -10 (Light; 0/100).
6.000s Player launched Mid Light at 6.000s; impact scheduled for 6.180s.
6.133s Player cannot queue Light: needs 10, 1.3 available.
6.267s Player cannot queue Light: needs 10, 2.7 available.
6.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
6.283s Initiative → Opponent (-66).
6.283s Player's Mid Light BLOCKED at 6.180s; frame advantage 0ms (simultaneous readiness).
6.400s Player cannot queue Light: needs 10, 4 available.
6.533s Player cannot queue Light: needs 10, 5.3 available.
6.667s Player cannot queue Light: needs 10, 6.7 available.
6.800s Player cannot queue Light: needs 10, 8 available.
6.933s Player cannot queue Light: needs 10, 9.3 available.
7.067s Player queued Mid Light (1/8).
7.067s Player stamina -10 (Light; 0.7/100).
7.067s Player launched Mid Light at 7.067s; impact scheduled for 7.247s.
7.200s Player cannot queue Light: needs 10, 2 available.
7.333s Player cannot queue Light: needs 10, 3.3 available.
7.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
7.350s Initiative → Opponent (-72).
7.350s Player's Mid Light BLOCKED at 7.247s; frame advantage 0ms (simultaneous readiness).
7.467s Player cannot queue Light: needs 10, 4.7 available.
7.600s Player cannot queue Light: needs 10, 6 available.
7.733s Player cannot queue Light: needs 10, 7.3 available.
7.867s Player cannot queue Light: needs 10, 8.7 available.
8.000s Player queued Mid Light (1/8).
8.000s Player stamina -10 (Light; 0/100).
8.000s Player launched Mid Light at 8.000s; impact scheduled for 8.180s.
8.133s Player cannot queue Light: needs 10, 1.3 available.
8.267s Player cannot queue Light: needs 10, 2.7 available.
8.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
8.283s Initiative → Opponent (-78).
8.283s Player's Mid Light BLOCKED at 8.180s; frame advantage 0ms (simultaneous readiness).
8.400s Player cannot queue Light: needs 10, 4 available.
8.533s Player cannot queue Light: needs 10, 5.3 available.
8.667s Player cannot queue Light: needs 10, 6.7 available.
8.800s Player cannot queue Light: needs 10, 8 available.
8.933s Player cannot queue Light: needs 10, 9.3 available.
9.067s Player queued Mid Light (1/8).
9.067s Player stamina -10 (Light; 0.7/100).
9.067s Player launched Mid Light at 9.067s; impact scheduled for 9.247s.
9.200s Player cannot queue Light: needs 10, 2 available.
9.333s Player cannot queue Light: needs 10, 3.3 available.
9.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
9.350s Initiative → Opponent (-84).
9.350s Player's Mid Light BLOCKED at 9.247s; frame advantage 0ms (simultaneous readiness).
9.467s Player cannot queue Light: needs 10, 4.7 available.
9.600s Player cannot queue Light: needs 10, 6 available.
9.733s Player cannot queue Light: needs 10, 7.3 available.
9.867s Player cannot queue Light: needs 10, 8.7 available.
10.000s Player queued Mid Light (1/8).
10.000s Player stamina -10 (Light; 0/100).
10.000s Player launched Mid Light at 10.000s; impact scheduled for 10.180s.
10.133s Player cannot queue Light: needs 10, 1.3 available.
10.267s Player cannot queue Light: needs 10, 2.7 available.
10.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
10.283s Initiative → Opponent (-90).
10.283s Player's Mid Light BLOCKED at 10.180s; frame advantage 0ms (simultaneous readiness).
10.400s Player cannot queue Light: needs 10, 4 available.
10.533s Player cannot queue Light: needs 10, 5.3 available.
10.667s Player cannot queue Light: needs 10, 6.7 available.
10.800s Player cannot queue Light: needs 10, 8 available.
10.933s Player cannot queue Light: needs 10, 9.3 available.
11.067s Player queued Mid Light (1/8).
11.067s Player stamina -10 (Light; 0.7/100).
11.067s Player launched Mid Light at 11.067s; impact scheduled for 11.247s.
11.200s Player cannot queue Light: needs 10, 2 available.
11.333s Player cannot queue Light: needs 10, 3.3 available.
11.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
11.350s Initiative → Opponent (-96).
11.350s Player's Mid Light BLOCKED at 11.247s; frame advantage 0ms (simultaneous readiness).
11.467s Player cannot queue Light: needs 10, 4.7 available.
11.600s Player cannot queue Light: needs 10, 6 available.
11.733s Player cannot queue Light: needs 10, 7.3 available.
11.867s Player cannot queue Light: needs 10, 8.7 available.
```

### repeated attack-buffer overflow: 74 rejected inputs: AggressiveLightSpam vs PassiveBlocking
Profile `Read100Hit20`, seed `12462575240730592753`, duration 12.00s.

```text
5.350s Player's Mid Light BLOCKED at 5.247s; frame advantage 0ms (simultaneous readiness).
5.467s Player cannot queue Light: needs 10, 4.7 available.
5.600s Player cannot queue Light: needs 10, 6 available.
5.733s Player cannot queue Light: needs 10, 7.3 available.
5.867s Player cannot queue Light: needs 10, 8.7 available.
6.000s Player queued Mid Light (1/8).
6.000s Player stamina -10 (Light; 0/100).
6.000s Player launched Mid Light at 6.000s; impact scheduled for 6.180s.
6.133s Player cannot queue Light: needs 10, 1.3 available.
6.267s Player cannot queue Light: needs 10, 2.7 available.
6.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
6.283s Initiative → Opponent (-66).
6.283s Player's Mid Light BLOCKED at 6.180s; frame advantage 0ms (simultaneous readiness).
6.400s Player cannot queue Light: needs 10, 4 available.
6.533s Player cannot queue Light: needs 10, 5.3 available.
6.667s Player cannot queue Light: needs 10, 6.7 available.
6.800s Player cannot queue Light: needs 10, 8 available.
6.933s Player cannot queue Light: needs 10, 9.3 available.
7.067s Player queued Mid Light (1/8).
7.067s Player stamina -10 (Light; 0.7/100).
7.067s Player launched Mid Light at 7.067s; impact scheduled for 7.247s.
7.200s Player cannot queue Light: needs 10, 2 available.
7.333s Player cannot queue Light: needs 10, 3.3 available.
7.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
7.350s Initiative → Opponent (-72).
7.350s Player's Mid Light BLOCKED at 7.247s; frame advantage 0ms (simultaneous readiness).
7.467s Player cannot queue Light: needs 10, 4.7 available.
7.600s Player cannot queue Light: needs 10, 6 available.
7.733s Player cannot queue Light: needs 10, 7.3 available.
7.867s Player cannot queue Light: needs 10, 8.7 available.
8.000s Player queued Mid Light (1/8).
8.000s Player stamina -10 (Light; 0/100).
8.000s Player launched Mid Light at 8.000s; impact scheduled for 8.180s.
8.133s Player cannot queue Light: needs 10, 1.3 available.
8.267s Player cannot queue Light: needs 10, 2.7 available.
8.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
8.283s Initiative → Opponent (-78).
8.283s Player's Mid Light BLOCKED at 8.180s; frame advantage 0ms (simultaneous readiness).
8.400s Player cannot queue Light: needs 10, 4 available.
8.533s Player cannot queue Light: needs 10, 5.3 available.
8.667s Player cannot queue Light: needs 10, 6.7 available.
8.800s Player cannot queue Light: needs 10, 8 available.
8.933s Player cannot queue Light: needs 10, 9.3 available.
9.067s Player queued Mid Light (1/8).
9.067s Player stamina -10 (Light; 0.7/100).
9.067s Player launched Mid Light at 9.067s; impact scheduled for 9.247s.
9.200s Player cannot queue Light: needs 10, 2 available.
9.333s Player cannot queue Light: needs 10, 3.3 available.
9.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
9.350s Initiative → Opponent (-84).
9.350s Player's Mid Light BLOCKED at 9.247s; frame advantage 0ms (simultaneous readiness).
9.467s Player cannot queue Light: needs 10, 4.7 available.
9.600s Player cannot queue Light: needs 10, 6 available.
9.733s Player cannot queue Light: needs 10, 7.3 available.
9.867s Player cannot queue Light: needs 10, 8.7 available.
10.000s Player queued Mid Light (1/8).
10.000s Player stamina -10 (Light; 0/100).
10.000s Player launched Mid Light at 10.000s; impact scheduled for 10.180s.
10.133s Player cannot queue Light: needs 10, 1.3 available.
10.267s Player cannot queue Light: needs 10, 2.7 available.
10.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
10.283s Initiative → Opponent (-90).
10.283s Player's Mid Light BLOCKED at 10.180s; frame advantage 0ms (simultaneous readiness).
10.400s Player cannot queue Light: needs 10, 4 available.
10.533s Player cannot queue Light: needs 10, 5.3 available.
10.667s Player cannot queue Light: needs 10, 6.7 available.
10.800s Player cannot queue Light: needs 10, 8 available.
10.933s Player cannot queue Light: needs 10, 9.3 available.
11.067s Player queued Mid Light (1/8).
11.067s Player stamina -10 (Light; 0.7/100).
11.067s Player launched Mid Light at 11.067s; impact scheduled for 11.247s.
11.200s Player cannot queue Light: needs 10, 2 available.
11.333s Player cannot queue Light: needs 10, 3.3 available.
11.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
11.350s Initiative → Opponent (-96).
11.350s Player's Mid Light BLOCKED at 11.247s; frame advantage 0ms (simultaneous readiness).
11.467s Player cannot queue Light: needs 10, 4.7 available.
11.600s Player cannot queue Light: needs 10, 6 available.
11.733s Player cannot queue Light: needs 10, 7.3 available.
11.867s Player cannot queue Light: needs 10, 8.7 available.
```

### repeated attack-buffer overflow: 74 rejected inputs: AggressiveLightSpam vs PassiveBlocking
Profile `Read100Hit20`, seed `11858984221418826580`, duration 12.00s.

```text
5.350s Player's Mid Light BLOCKED at 5.247s; frame advantage 0ms (simultaneous readiness).
5.467s Player cannot queue Light: needs 10, 4.7 available.
5.600s Player cannot queue Light: needs 10, 6 available.
5.733s Player cannot queue Light: needs 10, 7.3 available.
5.867s Player cannot queue Light: needs 10, 8.7 available.
6.000s Player queued Mid Light (1/8).
6.000s Player stamina -10 (Light; 0/100).
6.000s Player launched Mid Light at 6.000s; impact scheduled for 6.180s.
6.133s Player cannot queue Light: needs 10, 1.3 available.
6.267s Player cannot queue Light: needs 10, 2.7 available.
6.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
6.283s Initiative → Opponent (-66).
6.283s Player's Mid Light BLOCKED at 6.180s; frame advantage 0ms (simultaneous readiness).
6.400s Player cannot queue Light: needs 10, 4 available.
6.533s Player cannot queue Light: needs 10, 5.3 available.
6.667s Player cannot queue Light: needs 10, 6.7 available.
6.800s Player cannot queue Light: needs 10, 8 available.
6.933s Player cannot queue Light: needs 10, 9.3 available.
7.067s Player queued Mid Light (1/8).
7.067s Player stamina -10 (Light; 0.7/100).
7.067s Player launched Mid Light at 7.067s; impact scheduled for 7.247s.
7.200s Player cannot queue Light: needs 10, 2 available.
7.333s Player cannot queue Light: needs 10, 3.3 available.
7.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
7.350s Initiative → Opponent (-72).
7.350s Player's Mid Light BLOCKED at 7.247s; frame advantage 0ms (simultaneous readiness).
7.467s Player cannot queue Light: needs 10, 4.7 available.
7.600s Player cannot queue Light: needs 10, 6 available.
7.733s Player cannot queue Light: needs 10, 7.3 available.
7.867s Player cannot queue Light: needs 10, 8.7 available.
8.000s Player queued Mid Light (1/8).
8.000s Player stamina -10 (Light; 0/100).
8.000s Player launched Mid Light at 8.000s; impact scheduled for 8.180s.
8.133s Player cannot queue Light: needs 10, 1.3 available.
8.267s Player cannot queue Light: needs 10, 2.7 available.
8.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
8.283s Initiative → Opponent (-78).
8.283s Player's Mid Light BLOCKED at 8.180s; frame advantage 0ms (simultaneous readiness).
8.400s Player cannot queue Light: needs 10, 4 available.
8.533s Player cannot queue Light: needs 10, 5.3 available.
8.667s Player cannot queue Light: needs 10, 6.7 available.
8.800s Player cannot queue Light: needs 10, 8 available.
8.933s Player cannot queue Light: needs 10, 9.3 available.
9.067s Player queued Mid Light (1/8).
9.067s Player stamina -10 (Light; 0.7/100).
9.067s Player launched Mid Light at 9.067s; impact scheduled for 9.247s.
9.200s Player cannot queue Light: needs 10, 2 available.
9.333s Player cannot queue Light: needs 10, 3.3 available.
9.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
9.350s Initiative → Opponent (-84).
9.350s Player's Mid Light BLOCKED at 9.247s; frame advantage 0ms (simultaneous readiness).
9.467s Player cannot queue Light: needs 10, 4.7 available.
9.600s Player cannot queue Light: needs 10, 6 available.
9.733s Player cannot queue Light: needs 10, 7.3 available.
9.867s Player cannot queue Light: needs 10, 8.7 available.
10.000s Player queued Mid Light (1/8).
10.000s Player stamina -10 (Light; 0/100).
10.000s Player launched Mid Light at 10.000s; impact scheduled for 10.180s.
10.133s Player cannot queue Light: needs 10, 1.3 available.
10.267s Player cannot queue Light: needs 10, 2.7 available.
10.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
10.283s Initiative → Opponent (-90).
10.283s Player's Mid Light BLOCKED at 10.180s; frame advantage 0ms (simultaneous readiness).
10.400s Player cannot queue Light: needs 10, 4 available.
10.533s Player cannot queue Light: needs 10, 5.3 available.
10.667s Player cannot queue Light: needs 10, 6.7 available.
10.800s Player cannot queue Light: needs 10, 8 available.
10.933s Player cannot queue Light: needs 10, 9.3 available.
11.067s Player queued Mid Light (1/8).
11.067s Player stamina -10 (Light; 0.7/100).
11.067s Player launched Mid Light at 11.067s; impact scheduled for 11.247s.
11.200s Player cannot queue Light: needs 10, 2 available.
11.333s Player cannot queue Light: needs 10, 3.3 available.
11.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
11.350s Initiative → Opponent (-96).
11.350s Player's Mid Light BLOCKED at 11.247s; frame advantage 0ms (simultaneous readiness).
11.467s Player cannot queue Light: needs 10, 4.7 available.
11.600s Player cannot queue Light: needs 10, 6 available.
11.733s Player cannot queue Light: needs 10, 7.3 available.
11.867s Player cannot queue Light: needs 10, 8.7 available.
```

### repeated attack-buffer overflow: 74 rejected inputs: AggressiveLightSpam vs PassiveBlocking
Profile `Read100Hit20`, seed `8653611778163979611`, duration 12.00s.

```text
5.350s Player's Mid Light BLOCKED at 5.247s; frame advantage 0ms (simultaneous readiness).
5.467s Player cannot queue Light: needs 10, 4.7 available.
5.600s Player cannot queue Light: needs 10, 6 available.
5.733s Player cannot queue Light: needs 10, 7.3 available.
5.867s Player cannot queue Light: needs 10, 8.7 available.
6.000s Player queued Mid Light (1/8).
6.000s Player stamina -10 (Light; 0/100).
6.000s Player launched Mid Light at 6.000s; impact scheduled for 6.180s.
6.133s Player cannot queue Light: needs 10, 1.3 available.
6.267s Player cannot queue Light: needs 10, 2.7 available.
6.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
6.283s Initiative → Opponent (-66).
6.283s Player's Mid Light BLOCKED at 6.180s; frame advantage 0ms (simultaneous readiness).
6.400s Player cannot queue Light: needs 10, 4 available.
6.533s Player cannot queue Light: needs 10, 5.3 available.
6.667s Player cannot queue Light: needs 10, 6.7 available.
6.800s Player cannot queue Light: needs 10, 8 available.
6.933s Player cannot queue Light: needs 10, 9.3 available.
7.067s Player queued Mid Light (1/8).
7.067s Player stamina -10 (Light; 0.7/100).
7.067s Player launched Mid Light at 7.067s; impact scheduled for 7.247s.
7.200s Player cannot queue Light: needs 10, 2 available.
7.333s Player cannot queue Light: needs 10, 3.3 available.
7.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
7.350s Initiative → Opponent (-72).
7.350s Player's Mid Light BLOCKED at 7.247s; frame advantage 0ms (simultaneous readiness).
7.467s Player cannot queue Light: needs 10, 4.7 available.
7.600s Player cannot queue Light: needs 10, 6 available.
7.733s Player cannot queue Light: needs 10, 7.3 available.
7.867s Player cannot queue Light: needs 10, 8.7 available.
8.000s Player queued Mid Light (1/8).
8.000s Player stamina -10 (Light; 0/100).
8.000s Player launched Mid Light at 8.000s; impact scheduled for 8.180s.
8.133s Player cannot queue Light: needs 10, 1.3 available.
8.267s Player cannot queue Light: needs 10, 2.7 available.
8.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
8.283s Initiative → Opponent (-78).
8.283s Player's Mid Light BLOCKED at 8.180s; frame advantage 0ms (simultaneous readiness).
8.400s Player cannot queue Light: needs 10, 4 available.
8.533s Player cannot queue Light: needs 10, 5.3 available.
8.667s Player cannot queue Light: needs 10, 6.7 available.
8.800s Player cannot queue Light: needs 10, 8 available.
8.933s Player cannot queue Light: needs 10, 9.3 available.
9.067s Player queued Mid Light (1/8).
9.067s Player stamina -10 (Light; 0.7/100).
9.067s Player launched Mid Light at 9.067s; impact scheduled for 9.247s.
9.200s Player cannot queue Light: needs 10, 2 available.
9.333s Player cannot queue Light: needs 10, 3.3 available.
9.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
9.350s Initiative → Opponent (-84).
9.350s Player's Mid Light BLOCKED at 9.247s; frame advantage 0ms (simultaneous readiness).
9.467s Player cannot queue Light: needs 10, 4.7 available.
9.600s Player cannot queue Light: needs 10, 6 available.
9.733s Player cannot queue Light: needs 10, 7.3 available.
9.867s Player cannot queue Light: needs 10, 8.7 available.
10.000s Player queued Mid Light (1/8).
10.000s Player stamina -10 (Light; 0/100).
10.000s Player launched Mid Light at 10.000s; impact scheduled for 10.180s.
10.133s Player cannot queue Light: needs 10, 1.3 available.
10.267s Player cannot queue Light: needs 10, 2.7 available.
10.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
10.283s Initiative → Opponent (-90).
10.283s Player's Mid Light BLOCKED at 10.180s; frame advantage 0ms (simultaneous readiness).
10.400s Player cannot queue Light: needs 10, 4 available.
10.533s Player cannot queue Light: needs 10, 5.3 available.
10.667s Player cannot queue Light: needs 10, 6.7 available.
10.800s Player cannot queue Light: needs 10, 8 available.
10.933s Player cannot queue Light: needs 10, 9.3 available.
11.067s Player queued Mid Light (1/8).
11.067s Player stamina -10 (Light; 0.7/100).
11.067s Player launched Mid Light at 11.067s; impact scheduled for 11.247s.
11.200s Player cannot queue Light: needs 10, 2 available.
11.333s Player cannot queue Light: needs 10, 3.3 available.
11.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
11.350s Initiative → Opponent (-96).
11.350s Player's Mid Light BLOCKED at 11.247s; frame advantage 0ms (simultaneous readiness).
11.467s Player cannot queue Light: needs 10, 4.7 available.
11.600s Player cannot queue Light: needs 10, 6 available.
11.733s Player cannot queue Light: needs 10, 7.3 available.
11.867s Player cannot queue Light: needs 10, 8.7 available.
```

### repeated attack-buffer overflow: 74 rejected inputs: AggressiveLightSpam vs PassiveBlocking
Profile `Read100Hit20`, seed `1372369552177954369`, duration 12.00s.

```text
5.350s Player's Mid Light BLOCKED at 5.247s; frame advantage 0ms (simultaneous readiness).
5.467s Player cannot queue Light: needs 10, 4.7 available.
5.600s Player cannot queue Light: needs 10, 6 available.
5.733s Player cannot queue Light: needs 10, 7.3 available.
5.867s Player cannot queue Light: needs 10, 8.7 available.
6.000s Player queued Mid Light (1/8).
6.000s Player stamina -10 (Light; 0/100).
6.000s Player launched Mid Light at 6.000s; impact scheduled for 6.180s.
6.133s Player cannot queue Light: needs 10, 1.3 available.
6.267s Player cannot queue Light: needs 10, 2.7 available.
6.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
6.283s Initiative → Opponent (-66).
6.283s Player's Mid Light BLOCKED at 6.180s; frame advantage 0ms (simultaneous readiness).
6.400s Player cannot queue Light: needs 10, 4 available.
6.533s Player cannot queue Light: needs 10, 5.3 available.
6.667s Player cannot queue Light: needs 10, 6.7 available.
6.800s Player cannot queue Light: needs 10, 8 available.
6.933s Player cannot queue Light: needs 10, 9.3 available.
7.067s Player queued Mid Light (1/8).
7.067s Player stamina -10 (Light; 0.7/100).
7.067s Player launched Mid Light at 7.067s; impact scheduled for 7.247s.
7.200s Player cannot queue Light: needs 10, 2 available.
7.333s Player cannot queue Light: needs 10, 3.3 available.
7.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
7.350s Initiative → Opponent (-72).
7.350s Player's Mid Light BLOCKED at 7.247s; frame advantage 0ms (simultaneous readiness).
7.467s Player cannot queue Light: needs 10, 4.7 available.
7.600s Player cannot queue Light: needs 10, 6 available.
7.733s Player cannot queue Light: needs 10, 7.3 available.
7.867s Player cannot queue Light: needs 10, 8.7 available.
8.000s Player queued Mid Light (1/8).
8.000s Player stamina -10 (Light; 0/100).
8.000s Player launched Mid Light at 8.000s; impact scheduled for 8.180s.
8.133s Player cannot queue Light: needs 10, 1.3 available.
8.267s Player cannot queue Light: needs 10, 2.7 available.
8.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
8.283s Initiative → Opponent (-78).
8.283s Player's Mid Light BLOCKED at 8.180s; frame advantage 0ms (simultaneous readiness).
8.400s Player cannot queue Light: needs 10, 4 available.
8.533s Player cannot queue Light: needs 10, 5.3 available.
8.667s Player cannot queue Light: needs 10, 6.7 available.
8.800s Player cannot queue Light: needs 10, 8 available.
8.933s Player cannot queue Light: needs 10, 9.3 available.
9.067s Player queued Mid Light (1/8).
9.067s Player stamina -10 (Light; 0.7/100).
9.067s Player launched Mid Light at 9.067s; impact scheduled for 9.247s.
9.200s Player cannot queue Light: needs 10, 2 available.
9.333s Player cannot queue Light: needs 10, 3.3 available.
9.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
9.350s Initiative → Opponent (-84).
9.350s Player's Mid Light BLOCKED at 9.247s; frame advantage 0ms (simultaneous readiness).
9.467s Player cannot queue Light: needs 10, 4.7 available.
9.600s Player cannot queue Light: needs 10, 6 available.
9.733s Player cannot queue Light: needs 10, 7.3 available.
9.867s Player cannot queue Light: needs 10, 8.7 available.
10.000s Player queued Mid Light (1/8).
10.000s Player stamina -10 (Light; 0/100).
10.000s Player launched Mid Light at 10.000s; impact scheduled for 10.180s.
10.133s Player cannot queue Light: needs 10, 1.3 available.
10.267s Player cannot queue Light: needs 10, 2.7 available.
10.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
10.283s Initiative → Opponent (-90).
10.283s Player's Mid Light BLOCKED at 10.180s; frame advantage 0ms (simultaneous readiness).
10.400s Player cannot queue Light: needs 10, 4 available.
10.533s Player cannot queue Light: needs 10, 5.3 available.
10.667s Player cannot queue Light: needs 10, 6.7 available.
10.800s Player cannot queue Light: needs 10, 8 available.
10.933s Player cannot queue Light: needs 10, 9.3 available.
11.067s Player queued Mid Light (1/8).
11.067s Player stamina -10 (Light; 0.7/100).
11.067s Player launched Mid Light at 11.067s; impact scheduled for 11.247s.
11.200s Player cannot queue Light: needs 10, 2 available.
11.333s Player cannot queue Light: needs 10, 3.3 available.
11.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
11.350s Initiative → Opponent (-96).
11.350s Player's Mid Light BLOCKED at 11.247s; frame advantage 0ms (simultaneous readiness).
11.467s Player cannot queue Light: needs 10, 4.7 available.
11.600s Player cannot queue Light: needs 10, 6 available.
11.733s Player cannot queue Light: needs 10, 7.3 available.
11.867s Player cannot queue Light: needs 10, 8.7 available.
```

### repeated attack-buffer overflow: 74 rejected inputs: AggressiveLightSpam vs PassiveBlocking
Profile `Read100Hit20`, seed `5554772419727089210`, duration 12.00s.

```text
5.350s Player's Mid Light BLOCKED at 5.247s; frame advantage 0ms (simultaneous readiness).
5.467s Player cannot queue Light: needs 10, 4.7 available.
5.600s Player cannot queue Light: needs 10, 6 available.
5.733s Player cannot queue Light: needs 10, 7.3 available.
5.867s Player cannot queue Light: needs 10, 8.7 available.
6.000s Player queued Mid Light (1/8).
6.000s Player stamina -10 (Light; 0/100).
6.000s Player launched Mid Light at 6.000s; impact scheduled for 6.180s.
6.133s Player cannot queue Light: needs 10, 1.3 available.
6.267s Player cannot queue Light: needs 10, 2.7 available.
6.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
6.283s Initiative → Opponent (-66).
6.283s Player's Mid Light BLOCKED at 6.180s; frame advantage 0ms (simultaneous readiness).
6.400s Player cannot queue Light: needs 10, 4 available.
6.533s Player cannot queue Light: needs 10, 5.3 available.
6.667s Player cannot queue Light: needs 10, 6.7 available.
6.800s Player cannot queue Light: needs 10, 8 available.
6.933s Player cannot queue Light: needs 10, 9.3 available.
7.067s Player queued Mid Light (1/8).
7.067s Player stamina -10 (Light; 0.7/100).
7.067s Player launched Mid Light at 7.067s; impact scheduled for 7.247s.
7.200s Player cannot queue Light: needs 10, 2 available.
7.333s Player cannot queue Light: needs 10, 3.3 available.
7.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
7.350s Initiative → Opponent (-72).
7.350s Player's Mid Light BLOCKED at 7.247s; frame advantage 0ms (simultaneous readiness).
7.467s Player cannot queue Light: needs 10, 4.7 available.
7.600s Player cannot queue Light: needs 10, 6 available.
7.733s Player cannot queue Light: needs 10, 7.3 available.
7.867s Player cannot queue Light: needs 10, 8.7 available.
8.000s Player queued Mid Light (1/8).
8.000s Player stamina -10 (Light; 0/100).
8.000s Player launched Mid Light at 8.000s; impact scheduled for 8.180s.
8.133s Player cannot queue Light: needs 10, 1.3 available.
8.267s Player cannot queue Light: needs 10, 2.7 available.
8.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
8.283s Initiative → Opponent (-78).
8.283s Player's Mid Light BLOCKED at 8.180s; frame advantage 0ms (simultaneous readiness).
8.400s Player cannot queue Light: needs 10, 4 available.
8.533s Player cannot queue Light: needs 10, 5.3 available.
8.667s Player cannot queue Light: needs 10, 6.7 available.
8.800s Player cannot queue Light: needs 10, 8 available.
8.933s Player cannot queue Light: needs 10, 9.3 available.
9.067s Player queued Mid Light (1/8).
9.067s Player stamina -10 (Light; 0.7/100).
9.067s Player launched Mid Light at 9.067s; impact scheduled for 9.247s.
9.200s Player cannot queue Light: needs 10, 2 available.
9.333s Player cannot queue Light: needs 10, 3.3 available.
9.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
9.350s Initiative → Opponent (-84).
9.350s Player's Mid Light BLOCKED at 9.247s; frame advantage 0ms (simultaneous readiness).
9.467s Player cannot queue Light: needs 10, 4.7 available.
9.600s Player cannot queue Light: needs 10, 6 available.
9.733s Player cannot queue Light: needs 10, 7.3 available.
9.867s Player cannot queue Light: needs 10, 8.7 available.
10.000s Player queued Mid Light (1/8).
10.000s Player stamina -10 (Light; 0/100).
10.000s Player launched Mid Light at 10.000s; impact scheduled for 10.180s.
10.133s Player cannot queue Light: needs 10, 1.3 available.
10.267s Player cannot queue Light: needs 10, 2.7 available.
10.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
10.283s Initiative → Opponent (-90).
10.283s Player's Mid Light BLOCKED at 10.180s; frame advantage 0ms (simultaneous readiness).
10.400s Player cannot queue Light: needs 10, 4 available.
10.533s Player cannot queue Light: needs 10, 5.3 available.
10.667s Player cannot queue Light: needs 10, 6.7 available.
10.800s Player cannot queue Light: needs 10, 8 available.
10.933s Player cannot queue Light: needs 10, 9.3 available.
11.067s Player queued Mid Light (1/8).
11.067s Player stamina -10 (Light; 0.7/100).
11.067s Player launched Mid Light at 11.067s; impact scheduled for 11.247s.
11.200s Player cannot queue Light: needs 10, 2 available.
11.333s Player cannot queue Light: needs 10, 3.3 available.
11.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
11.350s Initiative → Opponent (-96).
11.350s Player's Mid Light BLOCKED at 11.247s; frame advantage 0ms (simultaneous readiness).
11.467s Player cannot queue Light: needs 10, 4.7 available.
11.600s Player cannot queue Light: needs 10, 6 available.
11.733s Player cannot queue Light: needs 10, 7.3 available.
11.867s Player cannot queue Light: needs 10, 8.7 available.
```

### repeated attack-buffer overflow: 74 rejected inputs: AggressiveLightSpam vs PassiveBlocking
Profile `Read100Hit20`, seed `10224245235941486793`, duration 12.00s.

```text
5.350s Player's Mid Light BLOCKED at 5.247s; frame advantage 0ms (simultaneous readiness).
5.467s Player cannot queue Light: needs 10, 4.7 available.
5.600s Player cannot queue Light: needs 10, 6 available.
5.733s Player cannot queue Light: needs 10, 7.3 available.
5.867s Player cannot queue Light: needs 10, 8.7 available.
6.000s Player queued Mid Light (1/8).
6.000s Player stamina -10 (Light; 0/100).
6.000s Player launched Mid Light at 6.000s; impact scheduled for 6.180s.
6.133s Player cannot queue Light: needs 10, 1.3 available.
6.267s Player cannot queue Light: needs 10, 2.7 available.
6.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
6.283s Initiative → Opponent (-66).
6.283s Player's Mid Light BLOCKED at 6.180s; frame advantage 0ms (simultaneous readiness).
6.400s Player cannot queue Light: needs 10, 4 available.
6.533s Player cannot queue Light: needs 10, 5.3 available.
6.667s Player cannot queue Light: needs 10, 6.7 available.
6.800s Player cannot queue Light: needs 10, 8 available.
6.933s Player cannot queue Light: needs 10, 9.3 available.
7.067s Player queued Mid Light (1/8).
7.067s Player stamina -10 (Light; 0.7/100).
7.067s Player launched Mid Light at 7.067s; impact scheduled for 7.247s.
7.200s Player cannot queue Light: needs 10, 2 available.
7.333s Player cannot queue Light: needs 10, 3.3 available.
7.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
7.350s Initiative → Opponent (-72).
7.350s Player's Mid Light BLOCKED at 7.247s; frame advantage 0ms (simultaneous readiness).
7.467s Player cannot queue Light: needs 10, 4.7 available.
7.600s Player cannot queue Light: needs 10, 6 available.
7.733s Player cannot queue Light: needs 10, 7.3 available.
7.867s Player cannot queue Light: needs 10, 8.7 available.
8.000s Player queued Mid Light (1/8).
8.000s Player stamina -10 (Light; 0/100).
8.000s Player launched Mid Light at 8.000s; impact scheduled for 8.180s.
8.133s Player cannot queue Light: needs 10, 1.3 available.
8.267s Player cannot queue Light: needs 10, 2.7 available.
8.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
8.283s Initiative → Opponent (-78).
8.283s Player's Mid Light BLOCKED at 8.180s; frame advantage 0ms (simultaneous readiness).
8.400s Player cannot queue Light: needs 10, 4 available.
8.533s Player cannot queue Light: needs 10, 5.3 available.
8.667s Player cannot queue Light: needs 10, 6.7 available.
8.800s Player cannot queue Light: needs 10, 8 available.
8.933s Player cannot queue Light: needs 10, 9.3 available.
9.067s Player queued Mid Light (1/8).
9.067s Player stamina -10 (Light; 0.7/100).
9.067s Player launched Mid Light at 9.067s; impact scheduled for 9.247s.
9.200s Player cannot queue Light: needs 10, 2 available.
9.333s Player cannot queue Light: needs 10, 3.3 available.
9.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
9.350s Initiative → Opponent (-84).
9.350s Player's Mid Light BLOCKED at 9.247s; frame advantage 0ms (simultaneous readiness).
9.467s Player cannot queue Light: needs 10, 4.7 available.
9.600s Player cannot queue Light: needs 10, 6 available.
9.733s Player cannot queue Light: needs 10, 7.3 available.
9.867s Player cannot queue Light: needs 10, 8.7 available.
10.000s Player queued Mid Light (1/8).
10.000s Player stamina -10 (Light; 0/100).
10.000s Player launched Mid Light at 10.000s; impact scheduled for 10.180s.
10.133s Player cannot queue Light: needs 10, 1.3 available.
10.267s Player cannot queue Light: needs 10, 2.7 available.
10.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
10.283s Initiative → Opponent (-90).
10.283s Player's Mid Light BLOCKED at 10.180s; frame advantage 0ms (simultaneous readiness).
10.400s Player cannot queue Light: needs 10, 4 available.
10.533s Player cannot queue Light: needs 10, 5.3 available.
10.667s Player cannot queue Light: needs 10, 6.7 available.
10.800s Player cannot queue Light: needs 10, 8 available.
10.933s Player cannot queue Light: needs 10, 9.3 available.
11.067s Player queued Mid Light (1/8).
11.067s Player stamina -10 (Light; 0.7/100).
11.067s Player launched Mid Light at 11.067s; impact scheduled for 11.247s.
11.200s Player cannot queue Light: needs 10, 2 available.
11.333s Player cannot queue Light: needs 10, 3.3 available.
11.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
11.350s Initiative → Opponent (-96).
11.350s Player's Mid Light BLOCKED at 11.247s; frame advantage 0ms (simultaneous readiness).
11.467s Player cannot queue Light: needs 10, 4.7 available.
11.600s Player cannot queue Light: needs 10, 6 available.
11.733s Player cannot queue Light: needs 10, 7.3 available.
11.867s Player cannot queue Light: needs 10, 8.7 available.
```

### repeated attack-buffer overflow: 74 rejected inputs: AggressiveLightSpam vs PassiveBlocking
Profile `Read100Hit20`, seed `3116724023294252488`, duration 12.00s.

```text
5.350s Player's Mid Light BLOCKED at 5.247s; frame advantage 0ms (simultaneous readiness).
5.467s Player cannot queue Light: needs 10, 4.7 available.
5.600s Player cannot queue Light: needs 10, 6 available.
5.733s Player cannot queue Light: needs 10, 7.3 available.
5.867s Player cannot queue Light: needs 10, 8.7 available.
6.000s Player queued Mid Light (1/8).
6.000s Player stamina -10 (Light; 0/100).
6.000s Player launched Mid Light at 6.000s; impact scheduled for 6.180s.
6.133s Player cannot queue Light: needs 10, 1.3 available.
6.267s Player cannot queue Light: needs 10, 2.7 available.
6.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
6.283s Initiative → Opponent (-66).
6.283s Player's Mid Light BLOCKED at 6.180s; frame advantage 0ms (simultaneous readiness).
6.400s Player cannot queue Light: needs 10, 4 available.
6.533s Player cannot queue Light: needs 10, 5.3 available.
6.667s Player cannot queue Light: needs 10, 6.7 available.
6.800s Player cannot queue Light: needs 10, 8 available.
6.933s Player cannot queue Light: needs 10, 9.3 available.
7.067s Player queued Mid Light (1/8).
7.067s Player stamina -10 (Light; 0.7/100).
7.067s Player launched Mid Light at 7.067s; impact scheduled for 7.247s.
7.200s Player cannot queue Light: needs 10, 2 available.
7.333s Player cannot queue Light: needs 10, 3.3 available.
7.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
7.350s Initiative → Opponent (-72).
7.350s Player's Mid Light BLOCKED at 7.247s; frame advantage 0ms (simultaneous readiness).
7.467s Player cannot queue Light: needs 10, 4.7 available.
7.600s Player cannot queue Light: needs 10, 6 available.
7.733s Player cannot queue Light: needs 10, 7.3 available.
7.867s Player cannot queue Light: needs 10, 8.7 available.
8.000s Player queued Mid Light (1/8).
8.000s Player stamina -10 (Light; 0/100).
8.000s Player launched Mid Light at 8.000s; impact scheduled for 8.180s.
8.133s Player cannot queue Light: needs 10, 1.3 available.
8.267s Player cannot queue Light: needs 10, 2.7 available.
8.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
8.283s Initiative → Opponent (-78).
8.283s Player's Mid Light BLOCKED at 8.180s; frame advantage 0ms (simultaneous readiness).
8.400s Player cannot queue Light: needs 10, 4 available.
8.533s Player cannot queue Light: needs 10, 5.3 available.
8.667s Player cannot queue Light: needs 10, 6.7 available.
8.800s Player cannot queue Light: needs 10, 8 available.
8.933s Player cannot queue Light: needs 10, 9.3 available.
9.067s Player queued Mid Light (1/8).
9.067s Player stamina -10 (Light; 0.7/100).
9.067s Player launched Mid Light at 9.067s; impact scheduled for 9.247s.
9.200s Player cannot queue Light: needs 10, 2 available.
9.333s Player cannot queue Light: needs 10, 3.3 available.
9.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
9.350s Initiative → Opponent (-84).
9.350s Player's Mid Light BLOCKED at 9.247s; frame advantage 0ms (simultaneous readiness).
9.467s Player cannot queue Light: needs 10, 4.7 available.
9.600s Player cannot queue Light: needs 10, 6 available.
9.733s Player cannot queue Light: needs 10, 7.3 available.
9.867s Player cannot queue Light: needs 10, 8.7 available.
10.000s Player queued Mid Light (1/8).
10.000s Player stamina -10 (Light; 0/100).
10.000s Player launched Mid Light at 10.000s; impact scheduled for 10.180s.
10.133s Player cannot queue Light: needs 10, 1.3 available.
10.267s Player cannot queue Light: needs 10, 2.7 available.
10.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
10.283s Initiative → Opponent (-90).
10.283s Player's Mid Light BLOCKED at 10.180s; frame advantage 0ms (simultaneous readiness).
10.400s Player cannot queue Light: needs 10, 4 available.
10.533s Player cannot queue Light: needs 10, 5.3 available.
10.667s Player cannot queue Light: needs 10, 6.7 available.
10.800s Player cannot queue Light: needs 10, 8 available.
10.933s Player cannot queue Light: needs 10, 9.3 available.
11.067s Player queued Mid Light (1/8).
11.067s Player stamina -10 (Light; 0.7/100).
11.067s Player launched Mid Light at 11.067s; impact scheduled for 11.247s.
11.200s Player cannot queue Light: needs 10, 2 available.
11.333s Player cannot queue Light: needs 10, 3.3 available.
11.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
11.350s Initiative → Opponent (-96).
11.350s Player's Mid Light BLOCKED at 11.247s; frame advantage 0ms (simultaneous readiness).
11.467s Player cannot queue Light: needs 10, 4.7 available.
11.600s Player cannot queue Light: needs 10, 6 available.
11.733s Player cannot queue Light: needs 10, 7.3 available.
11.867s Player cannot queue Light: needs 10, 8.7 available.
```

### repeated attack-buffer overflow: 74 rejected inputs: AggressiveLightSpam vs PassiveBlocking
Profile `Read100Hit20`, seed `15807628287867817592`, duration 12.00s.

```text
5.350s Player's Mid Light BLOCKED at 5.247s; frame advantage 0ms (simultaneous readiness).
5.467s Player cannot queue Light: needs 10, 4.7 available.
5.600s Player cannot queue Light: needs 10, 6 available.
5.733s Player cannot queue Light: needs 10, 7.3 available.
5.867s Player cannot queue Light: needs 10, 8.7 available.
6.000s Player queued Mid Light (1/8).
6.000s Player stamina -10 (Light; 0/100).
6.000s Player launched Mid Light at 6.000s; impact scheduled for 6.180s.
6.133s Player cannot queue Light: needs 10, 1.3 available.
6.267s Player cannot queue Light: needs 10, 2.7 available.
6.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
6.283s Initiative → Opponent (-66).
6.283s Player's Mid Light BLOCKED at 6.180s; frame advantage 0ms (simultaneous readiness).
6.400s Player cannot queue Light: needs 10, 4 available.
6.533s Player cannot queue Light: needs 10, 5.3 available.
6.667s Player cannot queue Light: needs 10, 6.7 available.
6.800s Player cannot queue Light: needs 10, 8 available.
6.933s Player cannot queue Light: needs 10, 9.3 available.
7.067s Player queued Mid Light (1/8).
7.067s Player stamina -10 (Light; 0.7/100).
7.067s Player launched Mid Light at 7.067s; impact scheduled for 7.247s.
7.200s Player cannot queue Light: needs 10, 2 available.
7.333s Player cannot queue Light: needs 10, 3.3 available.
7.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
7.350s Initiative → Opponent (-72).
7.350s Player's Mid Light BLOCKED at 7.247s; frame advantage 0ms (simultaneous readiness).
7.467s Player cannot queue Light: needs 10, 4.7 available.
7.600s Player cannot queue Light: needs 10, 6 available.
7.733s Player cannot queue Light: needs 10, 7.3 available.
7.867s Player cannot queue Light: needs 10, 8.7 available.
8.000s Player queued Mid Light (1/8).
8.000s Player stamina -10 (Light; 0/100).
8.000s Player launched Mid Light at 8.000s; impact scheduled for 8.180s.
8.133s Player cannot queue Light: needs 10, 1.3 available.
8.267s Player cannot queue Light: needs 10, 2.7 available.
8.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
8.283s Initiative → Opponent (-78).
8.283s Player's Mid Light BLOCKED at 8.180s; frame advantage 0ms (simultaneous readiness).
8.400s Player cannot queue Light: needs 10, 4 available.
8.533s Player cannot queue Light: needs 10, 5.3 available.
8.667s Player cannot queue Light: needs 10, 6.7 available.
8.800s Player cannot queue Light: needs 10, 8 available.
8.933s Player cannot queue Light: needs 10, 9.3 available.
9.067s Player queued Mid Light (1/8).
9.067s Player stamina -10 (Light; 0.7/100).
9.067s Player launched Mid Light at 9.067s; impact scheduled for 9.247s.
9.200s Player cannot queue Light: needs 10, 2 available.
9.333s Player cannot queue Light: needs 10, 3.3 available.
9.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
9.350s Initiative → Opponent (-84).
9.350s Player's Mid Light BLOCKED at 9.247s; frame advantage 0ms (simultaneous readiness).
9.467s Player cannot queue Light: needs 10, 4.7 available.
9.600s Player cannot queue Light: needs 10, 6 available.
9.733s Player cannot queue Light: needs 10, 7.3 available.
9.867s Player cannot queue Light: needs 10, 8.7 available.
10.000s Player queued Mid Light (1/8).
10.000s Player stamina -10 (Light; 0/100).
10.000s Player launched Mid Light at 10.000s; impact scheduled for 10.180s.
10.133s Player cannot queue Light: needs 10, 1.3 available.
10.267s Player cannot queue Light: needs 10, 2.7 available.
10.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
10.283s Initiative → Opponent (-90).
10.283s Player's Mid Light BLOCKED at 10.180s; frame advantage 0ms (simultaneous readiness).
10.400s Player cannot queue Light: needs 10, 4 available.
10.533s Player cannot queue Light: needs 10, 5.3 available.
10.667s Player cannot queue Light: needs 10, 6.7 available.
10.800s Player cannot queue Light: needs 10, 8 available.
10.933s Player cannot queue Light: needs 10, 9.3 available.
11.067s Player queued Mid Light (1/8).
11.067s Player stamina -10 (Light; 0.7/100).
11.067s Player launched Mid Light at 11.067s; impact scheduled for 11.247s.
11.200s Player cannot queue Light: needs 10, 2 available.
11.333s Player cannot queue Light: needs 10, 3.3 available.
11.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
11.350s Initiative → Opponent (-96).
11.350s Player's Mid Light BLOCKED at 11.247s; frame advantage 0ms (simultaneous readiness).
11.467s Player cannot queue Light: needs 10, 4.7 available.
11.600s Player cannot queue Light: needs 10, 6 available.
11.733s Player cannot queue Light: needs 10, 7.3 available.
11.867s Player cannot queue Light: needs 10, 8.7 available.
```

### repeated attack-buffer overflow: 74 rejected inputs: AggressiveLightSpam vs PassiveBlocking
Profile `Read100Hit20`, seed `830927689754903554`, duration 12.00s.

```text
5.350s Player's Mid Light BLOCKED at 5.247s; frame advantage 0ms (simultaneous readiness).
5.467s Player cannot queue Light: needs 10, 4.7 available.
5.600s Player cannot queue Light: needs 10, 6 available.
5.733s Player cannot queue Light: needs 10, 7.3 available.
5.867s Player cannot queue Light: needs 10, 8.7 available.
6.000s Player queued Mid Light (1/8).
6.000s Player stamina -10 (Light; 0/100).
6.000s Player launched Mid Light at 6.000s; impact scheduled for 6.180s.
6.133s Player cannot queue Light: needs 10, 1.3 available.
6.267s Player cannot queue Light: needs 10, 2.7 available.
6.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
6.283s Initiative → Opponent (-66).
6.283s Player's Mid Light BLOCKED at 6.180s; frame advantage 0ms (simultaneous readiness).
6.400s Player cannot queue Light: needs 10, 4 available.
6.533s Player cannot queue Light: needs 10, 5.3 available.
6.667s Player cannot queue Light: needs 10, 6.7 available.
6.800s Player cannot queue Light: needs 10, 8 available.
6.933s Player cannot queue Light: needs 10, 9.3 available.
7.067s Player queued Mid Light (1/8).
7.067s Player stamina -10 (Light; 0.7/100).
7.067s Player launched Mid Light at 7.067s; impact scheduled for 7.247s.
7.200s Player cannot queue Light: needs 10, 2 available.
7.333s Player cannot queue Light: needs 10, 3.3 available.
7.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
7.350s Initiative → Opponent (-72).
7.350s Player's Mid Light BLOCKED at 7.247s; frame advantage 0ms (simultaneous readiness).
7.467s Player cannot queue Light: needs 10, 4.7 available.
7.600s Player cannot queue Light: needs 10, 6 available.
7.733s Player cannot queue Light: needs 10, 7.3 available.
7.867s Player cannot queue Light: needs 10, 8.7 available.
8.000s Player queued Mid Light (1/8).
8.000s Player stamina -10 (Light; 0/100).
8.000s Player launched Mid Light at 8.000s; impact scheduled for 8.180s.
8.133s Player cannot queue Light: needs 10, 1.3 available.
8.267s Player cannot queue Light: needs 10, 2.7 available.
8.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
8.283s Initiative → Opponent (-78).
8.283s Player's Mid Light BLOCKED at 8.180s; frame advantage 0ms (simultaneous readiness).
8.400s Player cannot queue Light: needs 10, 4 available.
8.533s Player cannot queue Light: needs 10, 5.3 available.
8.667s Player cannot queue Light: needs 10, 6.7 available.
8.800s Player cannot queue Light: needs 10, 8 available.
8.933s Player cannot queue Light: needs 10, 9.3 available.
9.067s Player queued Mid Light (1/8).
9.067s Player stamina -10 (Light; 0.7/100).
9.067s Player launched Mid Light at 9.067s; impact scheduled for 9.247s.
9.200s Player cannot queue Light: needs 10, 2 available.
9.333s Player cannot queue Light: needs 10, 3.3 available.
9.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
9.350s Initiative → Opponent (-84).
9.350s Player's Mid Light BLOCKED at 9.247s; frame advantage 0ms (simultaneous readiness).
9.467s Player cannot queue Light: needs 10, 4.7 available.
9.600s Player cannot queue Light: needs 10, 6 available.
9.733s Player cannot queue Light: needs 10, 7.3 available.
9.867s Player cannot queue Light: needs 10, 8.7 available.
10.000s Player queued Mid Light (1/8).
10.000s Player stamina -10 (Light; 0/100).
10.000s Player launched Mid Light at 10.000s; impact scheduled for 10.180s.
10.133s Player cannot queue Light: needs 10, 1.3 available.
10.267s Player cannot queue Light: needs 10, 2.7 available.
10.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
10.283s Initiative → Opponent (-90).
10.283s Player's Mid Light BLOCKED at 10.180s; frame advantage 0ms (simultaneous readiness).
10.400s Player cannot queue Light: needs 10, 4 available.
10.533s Player cannot queue Light: needs 10, 5.3 available.
10.667s Player cannot queue Light: needs 10, 6.7 available.
10.800s Player cannot queue Light: needs 10, 8 available.
10.933s Player cannot queue Light: needs 10, 9.3 available.
11.067s Player queued Mid Light (1/8).
11.067s Player stamina -10 (Light; 0.7/100).
11.067s Player launched Mid Light at 11.067s; impact scheduled for 11.247s.
11.200s Player cannot queue Light: needs 10, 2 available.
11.333s Player cannot queue Light: needs 10, 3.3 available.
11.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
11.350s Initiative → Opponent (-96).
11.350s Player's Mid Light BLOCKED at 11.247s; frame advantage 0ms (simultaneous readiness).
11.467s Player cannot queue Light: needs 10, 4.7 available.
11.600s Player cannot queue Light: needs 10, 6 available.
11.733s Player cannot queue Light: needs 10, 7.3 available.
11.867s Player cannot queue Light: needs 10, 8.7 available.
```

### repeated attack-buffer overflow: 74 rejected inputs: AggressiveLightSpam vs PassiveBlocking
Profile `Read100Hit20`, seed `12896774464658847878`, duration 12.00s.

```text
5.350s Player's Mid Light BLOCKED at 5.247s; frame advantage 0ms (simultaneous readiness).
5.467s Player cannot queue Light: needs 10, 4.7 available.
5.600s Player cannot queue Light: needs 10, 6 available.
5.733s Player cannot queue Light: needs 10, 7.3 available.
5.867s Player cannot queue Light: needs 10, 8.7 available.
6.000s Player queued Mid Light (1/8).
6.000s Player stamina -10 (Light; 0/100).
6.000s Player launched Mid Light at 6.000s; impact scheduled for 6.180s.
6.133s Player cannot queue Light: needs 10, 1.3 available.
6.267s Player cannot queue Light: needs 10, 2.7 available.
6.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
6.283s Initiative → Opponent (-66).
6.283s Player's Mid Light BLOCKED at 6.180s; frame advantage 0ms (simultaneous readiness).
6.400s Player cannot queue Light: needs 10, 4 available.
6.533s Player cannot queue Light: needs 10, 5.3 available.
6.667s Player cannot queue Light: needs 10, 6.7 available.
6.800s Player cannot queue Light: needs 10, 8 available.
6.933s Player cannot queue Light: needs 10, 9.3 available.
7.067s Player queued Mid Light (1/8).
7.067s Player stamina -10 (Light; 0.7/100).
7.067s Player launched Mid Light at 7.067s; impact scheduled for 7.247s.
7.200s Player cannot queue Light: needs 10, 2 available.
7.333s Player cannot queue Light: needs 10, 3.3 available.
7.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
7.350s Initiative → Opponent (-72).
7.350s Player's Mid Light BLOCKED at 7.247s; frame advantage 0ms (simultaneous readiness).
7.467s Player cannot queue Light: needs 10, 4.7 available.
7.600s Player cannot queue Light: needs 10, 6 available.
7.733s Player cannot queue Light: needs 10, 7.3 available.
7.867s Player cannot queue Light: needs 10, 8.7 available.
8.000s Player queued Mid Light (1/8).
8.000s Player stamina -10 (Light; 0/100).
8.000s Player launched Mid Light at 8.000s; impact scheduled for 8.180s.
8.133s Player cannot queue Light: needs 10, 1.3 available.
8.267s Player cannot queue Light: needs 10, 2.7 available.
8.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
8.283s Initiative → Opponent (-78).
8.283s Player's Mid Light BLOCKED at 8.180s; frame advantage 0ms (simultaneous readiness).
8.400s Player cannot queue Light: needs 10, 4 available.
8.533s Player cannot queue Light: needs 10, 5.3 available.
8.667s Player cannot queue Light: needs 10, 6.7 available.
8.800s Player cannot queue Light: needs 10, 8 available.
8.933s Player cannot queue Light: needs 10, 9.3 available.
9.067s Player queued Mid Light (1/8).
9.067s Player stamina -10 (Light; 0.7/100).
9.067s Player launched Mid Light at 9.067s; impact scheduled for 9.247s.
9.200s Player cannot queue Light: needs 10, 2 available.
9.333s Player cannot queue Light: needs 10, 3.3 available.
9.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
9.350s Initiative → Opponent (-84).
9.350s Player's Mid Light BLOCKED at 9.247s; frame advantage 0ms (simultaneous readiness).
9.467s Player cannot queue Light: needs 10, 4.7 available.
9.600s Player cannot queue Light: needs 10, 6 available.
9.733s Player cannot queue Light: needs 10, 7.3 available.
9.867s Player cannot queue Light: needs 10, 8.7 available.
10.000s Player queued Mid Light (1/8).
10.000s Player stamina -10 (Light; 0/100).
10.000s Player launched Mid Light at 10.000s; impact scheduled for 10.180s.
10.133s Player cannot queue Light: needs 10, 1.3 available.
10.267s Player cannot queue Light: needs 10, 2.7 available.
10.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
10.283s Initiative → Opponent (-90).
10.283s Player's Mid Light BLOCKED at 10.180s; frame advantage 0ms (simultaneous readiness).
10.400s Player cannot queue Light: needs 10, 4 available.
10.533s Player cannot queue Light: needs 10, 5.3 available.
10.667s Player cannot queue Light: needs 10, 6.7 available.
10.800s Player cannot queue Light: needs 10, 8 available.
10.933s Player cannot queue Light: needs 10, 9.3 available.
11.067s Player queued Mid Light (1/8).
11.067s Player stamina -10 (Light; 0.7/100).
11.067s Player launched Mid Light at 11.067s; impact scheduled for 11.247s.
11.200s Player cannot queue Light: needs 10, 2 available.
11.333s Player cannot queue Light: needs 10, 3.3 available.
11.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
11.350s Initiative → Opponent (-96).
11.350s Player's Mid Light BLOCKED at 11.247s; frame advantage 0ms (simultaneous readiness).
11.467s Player cannot queue Light: needs 10, 4.7 available.
11.600s Player cannot queue Light: needs 10, 6 available.
11.733s Player cannot queue Light: needs 10, 7.3 available.
11.867s Player cannot queue Light: needs 10, 8.7 available.
```

### repeated attack-buffer overflow: 74 rejected inputs: AggressiveLightSpam vs PassiveBlocking
Profile `Read100Hit20`, seed `13591157199986047121`, duration 12.00s.

```text
5.350s Player's Mid Light BLOCKED at 5.247s; frame advantage 0ms (simultaneous readiness).
5.467s Player cannot queue Light: needs 10, 4.7 available.
5.600s Player cannot queue Light: needs 10, 6 available.
5.733s Player cannot queue Light: needs 10, 7.3 available.
5.867s Player cannot queue Light: needs 10, 8.7 available.
6.000s Player queued Mid Light (1/8).
6.000s Player stamina -10 (Light; 0/100).
6.000s Player launched Mid Light at 6.000s; impact scheduled for 6.180s.
6.133s Player cannot queue Light: needs 10, 1.3 available.
6.267s Player cannot queue Light: needs 10, 2.7 available.
6.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
6.283s Initiative → Opponent (-66).
6.283s Player's Mid Light BLOCKED at 6.180s; frame advantage 0ms (simultaneous readiness).
6.400s Player cannot queue Light: needs 10, 4 available.
6.533s Player cannot queue Light: needs 10, 5.3 available.
6.667s Player cannot queue Light: needs 10, 6.7 available.
6.800s Player cannot queue Light: needs 10, 8 available.
6.933s Player cannot queue Light: needs 10, 9.3 available.
7.067s Player queued Mid Light (1/8).
7.067s Player stamina -10 (Light; 0.7/100).
7.067s Player launched Mid Light at 7.067s; impact scheduled for 7.247s.
7.200s Player cannot queue Light: needs 10, 2 available.
7.333s Player cannot queue Light: needs 10, 3.3 available.
7.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
7.350s Initiative → Opponent (-72).
7.350s Player's Mid Light BLOCKED at 7.247s; frame advantage 0ms (simultaneous readiness).
7.467s Player cannot queue Light: needs 10, 4.7 available.
7.600s Player cannot queue Light: needs 10, 6 available.
7.733s Player cannot queue Light: needs 10, 7.3 available.
7.867s Player cannot queue Light: needs 10, 8.7 available.
8.000s Player queued Mid Light (1/8).
8.000s Player stamina -10 (Light; 0/100).
8.000s Player launched Mid Light at 8.000s; impact scheduled for 8.180s.
8.133s Player cannot queue Light: needs 10, 1.3 available.
8.267s Player cannot queue Light: needs 10, 2.7 available.
8.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
8.283s Initiative → Opponent (-78).
8.283s Player's Mid Light BLOCKED at 8.180s; frame advantage 0ms (simultaneous readiness).
8.400s Player cannot queue Light: needs 10, 4 available.
8.533s Player cannot queue Light: needs 10, 5.3 available.
8.667s Player cannot queue Light: needs 10, 6.7 available.
8.800s Player cannot queue Light: needs 10, 8 available.
8.933s Player cannot queue Light: needs 10, 9.3 available.
9.067s Player queued Mid Light (1/8).
9.067s Player stamina -10 (Light; 0.7/100).
9.067s Player launched Mid Light at 9.067s; impact scheduled for 9.247s.
9.200s Player cannot queue Light: needs 10, 2 available.
9.333s Player cannot queue Light: needs 10, 3.3 available.
9.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
9.350s Initiative → Opponent (-84).
9.350s Player's Mid Light BLOCKED at 9.247s; frame advantage 0ms (simultaneous readiness).
9.467s Player cannot queue Light: needs 10, 4.7 available.
9.600s Player cannot queue Light: needs 10, 6 available.
9.733s Player cannot queue Light: needs 10, 7.3 available.
9.867s Player cannot queue Light: needs 10, 8.7 available.
10.000s Player queued Mid Light (1/8).
10.000s Player stamina -10 (Light; 0/100).
10.000s Player launched Mid Light at 10.000s; impact scheduled for 10.180s.
10.133s Player cannot queue Light: needs 10, 1.3 available.
10.267s Player cannot queue Light: needs 10, 2.7 available.
10.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
10.283s Initiative → Opponent (-90).
10.283s Player's Mid Light BLOCKED at 10.180s; frame advantage 0ms (simultaneous readiness).
10.400s Player cannot queue Light: needs 10, 4 available.
10.533s Player cannot queue Light: needs 10, 5.3 available.
10.667s Player cannot queue Light: needs 10, 6.7 available.
10.800s Player cannot queue Light: needs 10, 8 available.
10.933s Player cannot queue Light: needs 10, 9.3 available.
11.067s Player queued Mid Light (1/8).
11.067s Player stamina -10 (Light; 0.7/100).
11.067s Player launched Mid Light at 11.067s; impact scheduled for 11.247s.
11.200s Player cannot queue Light: needs 10, 2 available.
11.333s Player cannot queue Light: needs 10, 3.3 available.
11.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
11.350s Initiative → Opponent (-96).
11.350s Player's Mid Light BLOCKED at 11.247s; frame advantage 0ms (simultaneous readiness).
11.467s Player cannot queue Light: needs 10, 4.7 available.
11.600s Player cannot queue Light: needs 10, 6 available.
11.733s Player cannot queue Light: needs 10, 7.3 available.
11.867s Player cannot queue Light: needs 10, 8.7 available.
```

### repeated attack-buffer overflow: 74 rejected inputs: AggressiveLightSpam vs PassiveBlocking
Profile `Read100Hit20`, seed `2105788660963126054`, duration 12.00s.

```text
5.350s Player's Mid Light BLOCKED at 5.247s; frame advantage 0ms (simultaneous readiness).
5.467s Player cannot queue Light: needs 10, 4.7 available.
5.600s Player cannot queue Light: needs 10, 6 available.
5.733s Player cannot queue Light: needs 10, 7.3 available.
5.867s Player cannot queue Light: needs 10, 8.7 available.
6.000s Player queued Mid Light (1/8).
6.000s Player stamina -10 (Light; 0/100).
6.000s Player launched Mid Light at 6.000s; impact scheduled for 6.180s.
6.133s Player cannot queue Light: needs 10, 1.3 available.
6.267s Player cannot queue Light: needs 10, 2.7 available.
6.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
6.283s Initiative → Opponent (-66).
6.283s Player's Mid Light BLOCKED at 6.180s; frame advantage 0ms (simultaneous readiness).
6.400s Player cannot queue Light: needs 10, 4 available.
6.533s Player cannot queue Light: needs 10, 5.3 available.
6.667s Player cannot queue Light: needs 10, 6.7 available.
6.800s Player cannot queue Light: needs 10, 8 available.
6.933s Player cannot queue Light: needs 10, 9.3 available.
7.067s Player queued Mid Light (1/8).
7.067s Player stamina -10 (Light; 0.7/100).
7.067s Player launched Mid Light at 7.067s; impact scheduled for 7.247s.
7.200s Player cannot queue Light: needs 10, 2 available.
7.333s Player cannot queue Light: needs 10, 3.3 available.
7.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
7.350s Initiative → Opponent (-72).
7.350s Player's Mid Light BLOCKED at 7.247s; frame advantage 0ms (simultaneous readiness).
7.467s Player cannot queue Light: needs 10, 4.7 available.
7.600s Player cannot queue Light: needs 10, 6 available.
7.733s Player cannot queue Light: needs 10, 7.3 available.
7.867s Player cannot queue Light: needs 10, 8.7 available.
8.000s Player queued Mid Light (1/8).
8.000s Player stamina -10 (Light; 0/100).
8.000s Player launched Mid Light at 8.000s; impact scheduled for 8.180s.
8.133s Player cannot queue Light: needs 10, 1.3 available.
8.267s Player cannot queue Light: needs 10, 2.7 available.
8.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
8.283s Initiative → Opponent (-78).
8.283s Player's Mid Light BLOCKED at 8.180s; frame advantage 0ms (simultaneous readiness).
8.400s Player cannot queue Light: needs 10, 4 available.
8.533s Player cannot queue Light: needs 10, 5.3 available.
8.667s Player cannot queue Light: needs 10, 6.7 available.
8.800s Player cannot queue Light: needs 10, 8 available.
8.933s Player cannot queue Light: needs 10, 9.3 available.
9.067s Player queued Mid Light (1/8).
9.067s Player stamina -10 (Light; 0.7/100).
9.067s Player launched Mid Light at 9.067s; impact scheduled for 9.247s.
9.200s Player cannot queue Light: needs 10, 2 available.
9.333s Player cannot queue Light: needs 10, 3.3 available.
9.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
9.350s Initiative → Opponent (-84).
9.350s Player's Mid Light BLOCKED at 9.247s; frame advantage 0ms (simultaneous readiness).
9.467s Player cannot queue Light: needs 10, 4.7 available.
9.600s Player cannot queue Light: needs 10, 6 available.
9.733s Player cannot queue Light: needs 10, 7.3 available.
9.867s Player cannot queue Light: needs 10, 8.7 available.
10.000s Player queued Mid Light (1/8).
10.000s Player stamina -10 (Light; 0/100).
10.000s Player launched Mid Light at 10.000s; impact scheduled for 10.180s.
10.133s Player cannot queue Light: needs 10, 1.3 available.
10.267s Player cannot queue Light: needs 10, 2.7 available.
10.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
10.283s Initiative → Opponent (-90).
10.283s Player's Mid Light BLOCKED at 10.180s; frame advantage 0ms (simultaneous readiness).
10.400s Player cannot queue Light: needs 10, 4 available.
10.533s Player cannot queue Light: needs 10, 5.3 available.
10.667s Player cannot queue Light: needs 10, 6.7 available.
10.800s Player cannot queue Light: needs 10, 8 available.
10.933s Player cannot queue Light: needs 10, 9.3 available.
11.067s Player queued Mid Light (1/8).
11.067s Player stamina -10 (Light; 0.7/100).
11.067s Player launched Mid Light at 11.067s; impact scheduled for 11.247s.
11.200s Player cannot queue Light: needs 10, 2 available.
11.333s Player cannot queue Light: needs 10, 3.3 available.
11.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
11.350s Initiative → Opponent (-96).
11.350s Player's Mid Light BLOCKED at 11.247s; frame advantage 0ms (simultaneous readiness).
11.467s Player cannot queue Light: needs 10, 4.7 available.
11.600s Player cannot queue Light: needs 10, 6 available.
11.733s Player cannot queue Light: needs 10, 7.3 available.
11.867s Player cannot queue Light: needs 10, 8.7 available.
```

### repeated attack-buffer overflow: 74 rejected inputs: AggressiveLightSpam vs PassiveBlocking
Profile `Read100Hit20`, seed `9887979342700528362`, duration 12.00s.

```text
5.350s Player's Mid Light BLOCKED at 5.247s; frame advantage 0ms (simultaneous readiness).
5.467s Player cannot queue Light: needs 10, 4.7 available.
5.600s Player cannot queue Light: needs 10, 6 available.
5.733s Player cannot queue Light: needs 10, 7.3 available.
5.867s Player cannot queue Light: needs 10, 8.7 available.
6.000s Player queued Mid Light (1/8).
6.000s Player stamina -10 (Light; 0/100).
6.000s Player launched Mid Light at 6.000s; impact scheduled for 6.180s.
6.133s Player cannot queue Light: needs 10, 1.3 available.
6.267s Player cannot queue Light: needs 10, 2.7 available.
6.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
6.283s Initiative → Opponent (-66).
6.283s Player's Mid Light BLOCKED at 6.180s; frame advantage 0ms (simultaneous readiness).
6.400s Player cannot queue Light: needs 10, 4 available.
6.533s Player cannot queue Light: needs 10, 5.3 available.
6.667s Player cannot queue Light: needs 10, 6.7 available.
6.800s Player cannot queue Light: needs 10, 8 available.
6.933s Player cannot queue Light: needs 10, 9.3 available.
7.067s Player queued Mid Light (1/8).
7.067s Player stamina -10 (Light; 0.7/100).
7.067s Player launched Mid Light at 7.067s; impact scheduled for 7.247s.
7.200s Player cannot queue Light: needs 10, 2 available.
7.333s Player cannot queue Light: needs 10, 3.3 available.
7.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
7.350s Initiative → Opponent (-72).
7.350s Player's Mid Light BLOCKED at 7.247s; frame advantage 0ms (simultaneous readiness).
7.467s Player cannot queue Light: needs 10, 4.7 available.
7.600s Player cannot queue Light: needs 10, 6 available.
7.733s Player cannot queue Light: needs 10, 7.3 available.
7.867s Player cannot queue Light: needs 10, 8.7 available.
8.000s Player queued Mid Light (1/8).
8.000s Player stamina -10 (Light; 0/100).
8.000s Player launched Mid Light at 8.000s; impact scheduled for 8.180s.
8.133s Player cannot queue Light: needs 10, 1.3 available.
8.267s Player cannot queue Light: needs 10, 2.7 available.
8.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
8.283s Initiative → Opponent (-78).
8.283s Player's Mid Light BLOCKED at 8.180s; frame advantage 0ms (simultaneous readiness).
8.400s Player cannot queue Light: needs 10, 4 available.
8.533s Player cannot queue Light: needs 10, 5.3 available.
8.667s Player cannot queue Light: needs 10, 6.7 available.
8.800s Player cannot queue Light: needs 10, 8 available.
8.933s Player cannot queue Light: needs 10, 9.3 available.
9.067s Player queued Mid Light (1/8).
9.067s Player stamina -10 (Light; 0.7/100).
9.067s Player launched Mid Light at 9.067s; impact scheduled for 9.247s.
9.200s Player cannot queue Light: needs 10, 2 available.
9.333s Player cannot queue Light: needs 10, 3.3 available.
9.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
9.350s Initiative → Opponent (-84).
9.350s Player's Mid Light BLOCKED at 9.247s; frame advantage 0ms (simultaneous readiness).
9.467s Player cannot queue Light: needs 10, 4.7 available.
9.600s Player cannot queue Light: needs 10, 6 available.
9.733s Player cannot queue Light: needs 10, 7.3 available.
9.867s Player cannot queue Light: needs 10, 8.7 available.
10.000s Player queued Mid Light (1/8).
10.000s Player stamina -10 (Light; 0/100).
10.000s Player launched Mid Light at 10.000s; impact scheduled for 10.180s.
10.133s Player cannot queue Light: needs 10, 1.3 available.
10.267s Player cannot queue Light: needs 10, 2.7 available.
10.283s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
10.283s Initiative → Opponent (-90).
10.283s Player's Mid Light BLOCKED at 10.180s; frame advantage 0ms (simultaneous readiness).
10.400s Player cannot queue Light: needs 10, 4 available.
10.533s Player cannot queue Light: needs 10, 5.3 available.
10.667s Player cannot queue Light: needs 10, 6.7 available.
10.800s Player cannot queue Light: needs 10, 8 available.
10.933s Player cannot queue Light: needs 10, 9.3 available.
11.067s Player queued Mid Light (1/8).
11.067s Player stamina -10 (Light; 0.7/100).
11.067s Player launched Mid Light at 11.067s; impact scheduled for 11.247s.
11.200s Player cannot queue Light: needs 10, 2 available.
11.333s Player cannot queue Light: needs 10, 3.3 available.
11.350s Opponent stamina +0 (Light Blocked; 100/100, 10 wasted at cap).
11.350s Initiative → Opponent (-96).
11.350s Player's Mid Light BLOCKED at 11.247s; frame advantage 0ms (simultaneous readiness).
11.467s Player cannot queue Light: needs 10, 4.7 available.
11.600s Player cannot queue Light: needs 10, 6 available.
11.733s Player cannot queue Light: needs 10, 7.3 available.
11.867s Player cannot queue Light: needs 10, 8.7 available.
```

Experimental profiles do not modify the prototype's default rules. Win rate is only one diagnostic; interpret it alongside hit rates, defensive counterplay, stamina waste, stalemates, unanswered sequences, and reversals.
