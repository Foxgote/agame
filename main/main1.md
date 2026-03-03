# VACUUM — Simulation, Scale, and Housekeeping MD

## Pillars
- **Skill-based navigation** through extreme curvature using incomplete instruments.
- **No magical cues.** Only sensors, inference, and consequences.
- **Commitment loop:** scan → infer → commit → ride → correct → exit.
- **Failure is fair.** Consistent physics + uncertainty, not hidden rubber-banding.
- **Stressors:** tidal strain + resource decay (time, heat, consumables, structural fatigue).
- **Visual truth:** black holes are seen via lensing and instrument reconstruction (“bubbles of light”), not arcade markers.

---

## Units and Coordinate Contract
### Units
- **1 unit = 1 meter**
- seconds, radians, m/s, m/s²
- gravity sources use **μ = G*M** (m³/s²)

### Axes
- Godot: +Y up, -Z forward
- ship forward vector: `-basis.z`

### Precision policy
- **Map space (authoritative):** doubles (meters)
- **Local bubble (render/interaction):** floats derived from map space via floating origin

---

## Scale Bands (Game-Real and Human-Usable)
### Band A — Local Bubble (EVA + repairs + close navigation)
Radius: **0–25 km** (default)

Use for:
- EVA and repairs (incl. **200 m array**)
- docking / contact operations
- high-frequency sensor sweeps (rays), close tidal cues, manual correction

Rules:
- full collision and ray tests
- no timewarp
- floating origin keeps local positions near zero

### Band B — Mid Space (approach/exit corridors)
Radius: **25 km–50,000 km**

Use for:
- long approach arcs, gravity assist setup, corridor shaping
- scanning targets before committing

Rules:
- simplified proxies (spheres / impostors)
- no EVA
- limited timewarp allowed if desired (risk tradeoffs apply)

### Band C — Far Space (multi-well chain planning)
Radius: **> 50,000 km**

Use for:
- plotting multi-well sequences, choosing next refuge, “dying universe” scale

Rules:
- track bodies as double-precision ephemeris points
- render via skybox / holographic reconstruction (not literal placement)
- spawn real nodes only when entering Band B/A

---

## Floating Origin (Mandatory Housekeeping)
Goal: never let local floats drift into garbage while keeping distances “real” in map space.

- Maintain `origin_map_pos = player.map_pos`
- Every active object uses:
  - `local_pos = (map_pos - origin_map_pos)` (cast to float)
- Shift trigger: when `|player.local_pos| > 5,000 m` (5 km), update `origin_map_pos`

Never incrementally “move everything.” Always derive local transforms from map positions.

---

## Ship Simulation Authority
Player drives motion. Godot physics is not the source of truth.

### State (authoritative)
- `x` position (Vec3d, map space)
- `v` velocity (Vec3d, map space)
- `q` orientation (unit quaternion, **SU(2)**)
- `ω` angular velocity (Vec3d; define world-frame or body-frame and stick to it)
- resources (fuel, heat, integrity, sensor health, time budget)

### Time step
- Fixed-step sim loop (recommended **1/120 s**)
- `_physics_process(delta)` accumulates and steps fixed dt

### Translation integrator (baseline)
Semi-implicit Euler:
- `v += a * dt`
- `x += v * dt`

(Upgradeable later to RK2/RK4 if you want more stable high-curvature passes.)

---

## SU(2) Attitude (Quaternion Contract)
Represent attitude as unit quaternion `q ∈ SU(2)`.

Recommended convention (simple, stable):
- store `ω` in world frame
- quaternion derivative:
  - `q̇ = 0.5 * Ω(ω) * q`, where `Ω(ω) = (ωx, ωy, ωz, 0)`

Discrete update:
- `q = normalize(q + 0.5 * dt * (Ω(ω) * q))`

Node basis derives from `q` each step. Never rotate node directly.

---

## Gravity Wells and Hazards (Consistent Physics)
### Gravity sources
Each body:
- `μ` (m³/s²)
- position in map space
- optional spin parameter (for later Kerr-like effects / pseudo frame-drag)
- influence / hazard radii for gameplay gating, not magical rails

### Point-mass acceleration (baseline)
For source at `xs`:
- `p = x - xs`
- `a = - μ * p / |p|^3`

### Horizon / singularity management (game rule)
To avoid numeric blowup and keep consequences fair:
- define `r_horizon` per black hole
- if `r <= r_horizon`: trigger fail/transition (blackout, termination, or narrative consequence)
- never integrate deeper than horizon

### Tidal stress (structural + cognitive pressure)
For point mass:
- tidal gradient: `T(r) = 2μ / r^3`
Ship differential acceleration across length `L`:
- `Δa = T(r) * L`

Orientation factor:
- `f(θ) = 0.3 + 0.7 cos²(θ)`
- `Δa_eff = Δa * f(θ)`

Strain ratio:
- `strain = Δa_eff / Smax`

Effects:
- strain > 0.5: instrument noise increases, control latency rises (player “feels” stress through degraded data)
- strain > 1.0: structural damage tick + resource decay accelerates
- strain ramps with **1/r³**, so near passes are brutal but predictable

---

## Sensors (No Magical Cues)
Sensors are the player’s only truth. They are noisy, incomplete, and degrade.

### Scan outputs (examples)
- range returns (ray bundles)
- lensing silhouette / light “bubble” reconstruction
- inferred class (mass scale bands), spin hints, distance uncertainty

### Player inference targets
- body class (μ scale)
- distance and approach geometry
- spin / frame effect likelihood (later)
- safe exit corridors

### Representation
- holographic curvature model reconstructed from scans
- uncertainty displayed as noisy volume / probabilistic surfaces, not “warning text”

---

## Commitment Model (Core Loop Enforcement)
Scan → infer → commit → ride → correct → exit.

### “Commit”
Once the player locks a burn solution:
- no safety warnings
- limited mid-arc assistance
- consequences are physics + resource realities

### “Ride / Correct”
Mid-arc corrections happen under:
- tidal stress
- drifting sensor quality
- resource decay
- potentially cascading multi-well perturbations

### “Exit”
Escape is earned by:
- correct inference
- tight execution
- conservative margins (or gambling with skill)

---

## Streaming and Multi-Well Chains
Bodies are tracked in map space always. Only near bodies are fully realized.

- Enter Band B: spawn proxies + enable long-range scan effects
- Enter Band A: spawn full nodes, enable EVA/collision/high-rate rays
- Leave Band A: despawn detail, keep tracks + summaries

Multi-well chain difficulty ramps by:
- increasing approach speeds
- adding rotating wells (spin)
- chaining overlapping influence regions
- reducing sensor quality while increasing consequence

---

## EVA and 200 m Array Repair Rule
EVA exists only in Band A.

- repair structures sized 10–500 m are fully physical in meters
- normal EVA speeds (0.2–3 m/s) with limited boost
- EVA allowed only if relative frame is stable (recommend gate: `|v_rel| < 2 m/s`)

Repairs are a **resource decision**, not a minigame:
- time spent increases decay risk
- proximity may increase tidal strain
- leaving the ship is a commitment under imperfect information

---

## Dev Housekeeping Checklist (Before Adding More Objects)
1. Lock Units/Axes/Forward definition.
2. Implement map-space doubles + floating origin.
3. Implement fixed-step loop.
4. Implement SU(2) quaternion integrator and verify no drift.
5. Implement point-mass gravity + horizon cutoff.
6. Add tidal strain computation and expose debug readouts.
7. Only then add sensors/rays and reconstruction visuals.

---

## Minimal Playtest Scenario (First Validation)
- 1 black hole (μ, r_horizon)
- 1 ship (Node3D-owned motion, SU(2))
- start with tangential velocity for a spiral approach
- confirm:
  - stable integration
  - predictable 1/r² accel behavior
  - 1/r³ tidal ramp
  - horizon trigger behavior
  - sensor rays still function in Band A