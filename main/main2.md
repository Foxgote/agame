# VACUUM - Implementation Contracts (C++ / Godot 4 GDExtension)

This document is the executable contract: class list, data layout, function signatures, and update order. It assumes:
- Godot Nodes are presentation + input.
- Simulation state is authoritative.
- Map space uses doubles; local space is derived floats.
- Ship motion is owned by the sim (Node3D), not Godot physics.
- Orientation uses SU(2) (unit quaternions).

---

## File/Module Layout (recommended)
`res://` (Godot project)
- `bin/` (DLLs)
- `scenes/`
- `scripts/` (optional, thin glue only)

C++ (extension)
- `src/`
- `src/sim/`
- `src/sim/vec3d.hpp`
- `src/sim/quatd.hpp`
- `src/sim/sim_clock.hpp`
- `src/sim/sim_frames.hpp`
- `src/sim/gravity_math.hpp/.cpp`
- `src/sim/tidal_math.hpp/.cpp`
- `src/sim/integrators.hpp/.cpp`
- `src/sim/sensor_rays.hpp/.cpp`
- `src/nodes/`
- `src/nodes/black_hole_node.hpp/.cpp`
- `src/nodes/ship_sim_node.hpp/.cpp`
- `src/nodes/sim_world_node.hpp/.cpp`
- `src/register_types.cpp/.h`

---

## Core Types

### Vec3d (double precision)
```cpp
struct Vec3d {
  double x, y, z;
  Vec3d operator+(const Vec3d&) const;
  Vec3d operator-(const Vec3d&) const;
  Vec3d operator*(double) const;
};
double dot(const Vec3d&, const Vec3d&);
double length(const Vec3d&);
double length2(const Vec3d&);
Vec3d normalize(const Vec3d&);
```

### Quatd (SU(2) unit quaternion, double)
Convention: `(x, y, z, w)`.

```cpp
struct Quatd {
  double x, y, z, w;
};
Quatd q_normalize(const Quatd&);
Quatd q_mul(const Quatd& a, const Quatd& b);
Quatd q_from_omega_world(const Vec3d& omega); // (omega.x, omega.y, omega.z, 0)
```

### Cast helpers
- `Vec3d <-> godot::Vector3`
- `Quatd <-> godot::Quaternion`

Never store Godot float vectors as authoritative state.

---

## Global Constants (tunable defaults)
- `FIXED_DT = 1.0 / 120.0`
- `ACTIVE_RADIUS_A = 25'000.0` meters
- `DESPAWN_RADIUS = 30'000.0` meters
- `ORIGIN_SHIFT_THRESHOLD = 5'000.0` meters

---

## Simulation Frames

### Map Space
- Double-precision meters.
- All bodies have `map_pos`, `map_vel`.

### Local Space
- Float transform for rendering/interaction.
- Derived each frame:
- `local_pos = (map_pos - origin_map_pos)` cast to float.

### Floating Origin Contract
Maintain:
- `origin_map_pos` (`Vec3d`), typically player ship position in map space.

Update rule:
- If `|player_local_pos| > ORIGIN_SHIFT_THRESHOLD`, set `origin_map_pos = player.map_pos`.

Never incrementally shift nodes; always derive local from map.

---

## Math Contracts

### Gravity (point mass baseline)
Input: `mu` (`m^3/s^2`), `p = (ship_pos - source_pos)`

```cpp
Vec3d accel_point_mass(double mu, const Vec3d& p);
```

Definition:
- `a = -mu * p / |p|^3`

Safety:
- If `|p| < r_min`, return 0 or clamp externally (horizon handles termination).

### Horizon rule
- If `r <= r_horizon`: trigger fail/transition; do not integrate deeper.

### Tidal math
```cpp
double tidal_gradient(double mu, double r);          // T(r) = 2*mu / r^3
double tidal_load(double mu, double r, double L);    // delta_a = T * L
double tidal_orient_factor(double cos_theta);        // 0.3 + 0.7*cos^2(theta)
```

Composite tidal load from multiple bodies:
- `T_total = sum(2*mu_i / r_i^3)`
- `delta_a_total = T_total * L`

Strain ratio:
- `strain = delta_a_eff / Smax`

---

## Integrators

### Translation (semi-implicit Euler)
```cpp
void integrate_translation(Vec3d& x, Vec3d& v, const Vec3d& a, double dt);
```
- `v += a * dt`
- `x += v * dt`

### Rotation (SU(2), omega in world frame)
```cpp
void integrate_su2_world(Quatd& q, const Vec3d& omega_world, double dt);
```

Discrete:
- `q = normalize(q + 0.5 * dt * (Omega(omega) * q))`
- Where `Omega(omega) = (omega.x, omega.y, omega.z, 0)` and multiplication is quaternion multiply.

If later storing omega in body frame, switch to right-multiply:
- `q_dot = 0.5 * q * Omega(omega_body)`

---

## Node Classes (Godot-facing)

### BlackHoleNode : Node3D
Purpose: provides `mu` and hazard radii; optionally visual hooks.

Properties:
- `mu: double`
- `horizon_radius: double`
- `max_accel: double` (early stability only; can be removed later)

Methods:
```cpp
Vec3d accel_at_map(const Vec3d& ship_map_pos) const;
bool is_inside_horizon(const Vec3d& ship_map_pos) const;
```

### ShipSimNode : Node3D
Purpose: authoritative ship state + fixed-step integration + controls + transform writeback.

Properties (Godot):
- `black_hole_path: NodePath` (or list of sources later)
- `length_L: double`
- `strain_budget_Smax: double`
- `thrust_accel: double` (optional)
- `turn_rate: double` (optional)
- `fixed_dt: double` (default `FIXED_DT`)
- `enable_debug: bool`

Authoritative state (C++ only):
- `Vec3d map_pos, map_vel`
- `Quatd q`
- `Vec3d omega_world`
- resource state (fuel/heat/integrity/sensor health, later)

Internal:
- `double accumulator`

Lifecycle methods:
```cpp
void _ready() override;
void _physics_process(double delta) override;
```

Core step:
```cpp
void step_fixed(double dt);
```

Update order inside `step_fixed(dt)`:
1. Resolve sources (`BlackHoleNode` pointer or list).
2. Compute `a_grav` (sum of gravity accelerations).
3. Apply thrust/controls to compute `a_ctrl` (optional).
4. Integrate translation (`map_vel`, `map_pos`).
5. Integrate rotation (`q` from `omega_world`).
6. Horizon check (terminate if crossed).
7. Compute tidal strain (for damage/noise scaling).
8. Write back local transform (map-to-local using origin).

### SimWorldNode : Node
Purpose: manages origin, active set streaming, optional timewarp gating.

Properties:
- `active_radius: double` (default 25 km)
- `despawn_radius: double`
- `origin_shift_threshold: double`

Responsibilities:
- maintain `origin_map_pos` (`Vec3d`)
- supply origin to `ShipSimNode` and other active nodes
- spawn/despawn nodes based on map distance (later)

---

## Transform Writeback Contract
`ShipSimNode` writes its own transform each fixed step (or once per frame after stepping):

- `local_pos = map_pos - origin_map_pos`
- `Basis basis = Basis(Quaternion(q_float))`
- `Transform3D t; t.basis = basis; t.origin = local_pos_float;`
- `set_global_transform(t)`

No other system is allowed to move the ship node directly.

---

## Debug Outputs (required early)
Expose (HUD print or debug overlay):
- `r` to primary well
- `|a_grav|`
- `tidal delta_a_eff`
- `strain ratio`
- `r_horizon`

Also draw:
- gravity vector at ship
- forward axis
- sensor rays (when enabled)

---

## Minimal Playtest Scene Contract
Scene contents:
- `SimWorldNode` (optional early)
- `BlackHoleNode` at origin:
- `mu` tuned for visible spiral within seconds
- `horizon_radius` set to safe termination
- `ShipSimNode`:
- `map_pos = (0, 0, 30)`
- `map_vel = (6, 0, 0)` for spiral approach
- `q = identity`
- `omega = 0`

Validation checklist:
- Spiral is smooth and repeatable
- Horizon triggers reliably (no numeric explosion)
- Tidal strain ramps ~ `1/r^3` and feels predictable
- Floating origin does not jitter EVA-scale structures when present

---

## Registration Contract
In `initialize_*` at `MODULE_INITIALIZATION_LEVEL_SCENE`:

```cpp
ClassDB::register_class<BlackHoleNode>();
ClassDB::register_class<ShipSimNode>();
ClassDB::register_class<SimWorldNode>(); // if used
```

---

## Future Extensions (do not implement yet)
- Multi-well chain: list of gravity sources and switching primary
- Spin effects: pseudo frame-drag term (gameplay-driven, still consistent)
- Sensor reconstruction: ray bundles -> probabilistic curvature volume
- Resource decay coupling: strain and time increasing noise/latency/damage
- Streaming system: map bodies into active nodes as player approaches
