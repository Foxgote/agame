# Vertical Slice Handoff

## Build And Run

The tested path is CMake on Windows with the repository's vendored `godot-cpp`:

```powershell
cmake -S main -B main\build -DCMAKE_BUILD_TYPE=Debug
cmake --build main\build --target agame_ext --parallel 4
ctest --test-dir main\build --output-on-failure
```

MinGW runtime DLLs are copied beside `libagame_ext.dll` automatically. The SCons source globs and output name were aligned, but SCons was not the verified path this session.

Open `main/project/project.godot` in Godot 4.6, allow the first import scan, and run the main scene. A portable official Godot 4.6 stable runtime was used for verification because the locally installed editor was 4.2.1.

The runtime action test is:

```powershell
Godot_v4.6-stable_win64_console.exe --headless --path main\project --script res://tests/runtime_smoke.gd
```

That script is the only new GDScript. It is test glue, not runtime architecture.

## Controls

- `W A S D`: move.
- Mouse: look.
- `E`: use the advertised primary action; while seated, stand.
- `Space` / `Ctrl`: rise/lower in zero gravity.
- `Q`: cancel an interruptible action.
- `Esc`: release or recapture the mouse.

The expanded forward console adds four manual-maneuver controls:

- `DELTA-V`: selects requested prograde engine impulse.
- `EXECUTE`: queues the selected burn at the next simulation step.
- `EJECT MASS`: selects dedicated reaction mass to expel.
- `DUMP`: releases that mass retrograde for an emergency prograde impulse.

The HUD previews energy gain at the current encounter speed. Waiting until a faster periapsis passage increases the payoff from the same selected delta-v, while fuel, reaction mass, tidal exposure, and correction time remain the tradeoffs.

## Playable Path

Move to clear the opening tutorial step, inspect the forward placard, press the red gravity breaker, move in zero gravity, restore gravity, change the mass estimate, adjust shield power, sit/stand, and consume the ration. A fade/reposition beat demonstrates staged tutorial transitions. The final narrative card offers two resource-changing choices.

The top-left telemetry intentionally shows truth, sensor, estimate, prediction, and render versions separately. The mass knob changes only the player estimate and predictor input. Tidal severity damages the shield over time, producing integrity warnings, generated placeholder tones, and camera feedback without pausing the worker simulation.

## Known Limits

- Geometry and tones are purpose-built placeholders; no final art, animation clips, character hands, or authored audio are present.
- The action runtime has semantic timing and hooks but uses transform snapping for alignment and no animation-tree adapter yet.
- Prediction is synchronous and short. Its API/versioning is ready for a cancellable job worker later.
- The worker snapshot buffer is two slots. Add a larger ring only if replay, network history, or delayed sensors require it.
- Sensor noise is deterministic synthetic noise; calibration, bias, stale timestamps, dropout, and propagation delay are future models.
- Tidal damage thresholds are gameplay-tuned for the smoke system, not structural-engineering values.
- No autonomous burn planner, finite-duration thrust model, failure screen, save/load, EVA, crew actor, procedural chapter generation, or exact strong-field solver is implemented.
- Manual impulses are implemented, but autonomous scheduled burns and manual takeover of an executing burn timeline are still pending.
- The current camera is first-person for testing; the controller/action boundary does not require that final choice.

## Best Next Session

1. Replace transform snapping with authored interaction anchors and animation/contact events.
2. Add a navigation console view that edits mass, distance, velocity, uncertainty, and burn plan as explicit player-believed data.
3. Render predicted versus authoritative exterior trajectories from snapshots in a dedicated floating-origin viewport.
4. Add module consequences to movement and sensor quality, then author one understandable navigation failure path.
5. Replace placeholder primitives and generated tones incrementally without moving simulation logic into scene nodes.

Do not encode an upgrade tree until the next concept pass. Natural progression hooks now exist in sensor uncertainty, prediction horizon, engine exhaust velocity, maximum queued delta-v, reaction-mass capacity/ejecta velocity, shield mitigation, and module integrity, but their unlock order and economy remain deliberately unresolved.
