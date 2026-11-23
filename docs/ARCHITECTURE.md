# Architecture

## Naming

Game-owned identifiers use lower camelCase, while classes, structs, and enum values use PascalCase. Filenames and required Godot callbacks such as `_ready`, `_process`, and `_bind_methods` retain their framework-compatible names.

## Runtime Layers

1. `main/project` owns the Godot scene, rendering, input, and the small runtime smoke-test script.
2. Godot-facing C++ classes in `main/src` own scene-tree objects, interaction adapters, UI, audio, and camera feedback.
3. `main/src/gameplay` owns the plain-C++ behavior-tree runtime and action blackboard.
4. `main/src/sim` owns deterministic state, gravity, integration, sensor observations, trajectory prediction, and snapshots.
5. `Vec3d`, state records, constants, and conversion functions are the math/unit primitives shared inside the lower layers.

The simulation directory includes no Godot headers and has no `Node`, rendering, physics-server, input, UI, audio, or narrative dependency.

## Five State Channels

`NavigationStateBundle` owns five independent values:

- `trueState`: authoritative astronomical bodies and ship trajectory.
- `sensorState`: deterministic noisy observations stamped with the source truth version.
- `playerEstimate`: values entered or believed by the player, with its own estimate version.
- `predictionState`: samples produced by advancing a copied player estimate.
- `renderState`: interpolated, floating-origin positions derived from consecutive immutable snapshots.

The test suite mutates an estimated mass and verifies that the true mass does not change. Prediction advances a copied kernel and cannot mutate the worker-owned simulation.

## Thread Ownership

`SimulationRunner` creates one worker thread. The worker alone owns and advances `SimulationKernel`; it touches no Godot object or API. It publishes `shared_ptr<const SimulationSnapshot>` values through a two-slot buffer protected only during pointer swaps.

The main thread copies the newest snapshot into the five state channels, calculates rendering interpolation, updates UI, emits tidal gameplay signals, and runs synchronous short-horizon prediction. The runner limits catch-up to four steps and records average/max step duration, catch-up count, skipped work, body count, and snapshot age.

## Coordinates And Units

- Astronomical position: double-precision SI metres.
- Astronomical velocity: metres per second.
- Mass: kilograms.
- Simulation and proper time: seconds.
- Gravity and tidal calculations: SI.
- Exterior render position: `astronomicalPosition - renderOrigin`, converted only at the Godot boundary.
- Ship interior: local Godot metres near scene origin.
- Player walking and zero-g movement affect only the interior `CharacterBody3D`; they never change astronomical ship coordinates.
- Thrust is reserved for later burn-plan work and will use newtons plus explicit ship mass.

## Player And Actions

`PlayerController` uses mutually exclusive `Grounded`, `ZeroGravity`, `Seated`, `ActionLocked`, `Cinematic`, and `Disabled` modes. Physical movement remains outside the behavior tree.

The behavior tree supplies sequence, selector, condition, task, and wait nodes with `Running`, `Success`, `Failure`, and `Cancelled` statuses. Its blackboard supports actor/target, action ID, anchors, hand/look/camera targets, item, progress, interruptibility, failure reason, reservation token, arbitrary context, and current-node tracing.

Interactables advertise options. The player never branches on concrete chair/knob/button types. The demonstrated pipeline validates, reserves, optionally aligns, begins, waits for the semantic contact point, applies the target effect, waits for exit, completes, releases, and restores control. Cancellation explicitly releases the target.

## Presentation Boundary

The simulation emits tidal severity only. `ShipModuleSystem` converts it into module integrity changes and emits warning, audio, and camera-feedback signals. `VerticalSlice` routes those signals to generated placeholder tones, HUD warnings, and camera trauma. Interactables similarly emit generic hooks rather than naming the systems they control.

## Manual Maneuvers

Manual burn and reaction-mass requests cross into `SimulationRunner` through a bounded command queue. The worker drains that queue immediately before a fixed simulation step and is the only owner allowed to change authoritative ship velocity or mass. The main thread receives actual delta-v, expelled mass, and energy gain later through snapshot telemetry and a main-thread signal.

This makes manual override asynchronous and honest: the console requests an amount, while available propellant/reaction mass determines what the ship actually achieves. Maneuver count in the authoritative state lets UI and tests detect completion without sharing mutable state.
