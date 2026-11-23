# Vertical Scaffold Work Log

Date: 2026-08-05
Branch: `codex/agame-vertical-scaffold`

## Baseline

- Repository was clean before work began.
- Project targets Godot 4.6 and uses GDExtension through `example_library_init`.
- Both CMake and SCons build paths exist; CMake is used for this session.
- Existing `BlackHole`, `Spaceship`, and `DebugGrid` classes were inspected and retained.
- Godot was not available on `PATH`; only 4.2.1 was installed locally. The official portable 4.6 stable runtime was used from a temporary directory for version-matched checks.
- The first build command timed out after 124 seconds while compiling `godot-cpp`; it emitted no compiler error.
- Resuming the untouched build completed successfully in 488.9 seconds and produced `main/project/bin/libagame_ext.dll`.

## Session Progress

- [x] Audit repository, versions, build paths, registration, scenes, and existing gameplay code.
- [x] Establish a clean scaffold branch and record the baseline build.
- [x] Add the scene-independent deterministic simulation kernel and tests.
- [x] Add Godot adapters, player action runtime, interactables, and ship modules.
- [x] Build the playable smoke-test scene and tutorial/event UI.
- [x] Run all available build, unit, launch, action-path, and rendered-frame checks.

## Verification

- `cmake --build main/build --target agame_ext --parallel 2`: passed.
- `ctest --test-dir main/build --output-on-failure`: 2/2 passed.
- Godot 4.6 headless editor import: passed after repairing the extension DLL mapping and MinGW runtime deployment.
- Godot 4.6 main-scene headless run: passed with a clean log.
- `res://tests/runtime_smoke.gd`: all action and continuous-simulation checks passed.
- OpenGL compatibility movie capture: passed; final frame confirmed a visible interior and HUD after the opening fade.
- `git diff --check`: passed before the documentation pass.

## Manual Maneuver Extension

- Added fuel-limited prograde burns whose specific-energy payoff scales with encounter speed.
- Added directed reaction-mass dumping; passive mass loss is intentionally not treated as propulsion.
- Added a bounded main-to-worker command queue and snapshot-delivered maneuver telemetry.
- Added physical delta-v, execute, eject-mass, and dump controls to the forward console.
- Extended native and Godot runtime tests to verify consumption, impulse, Oberth gain, and continued simulation.

## Naming Pass

- Converted game-owned C++ and GDScript identifiers, bound methods, properties, signals, input actions, and test names to camelCase.
- Retained PascalCase for types and enum values, and retained required Godot callback and framework API names.
- Rebuilt the extension and reran both native suites, the Godot runtime smoke test, and a headless main-scene launch successfully after the rename.

This log is updated as work completes. Design limitations and next-session work live in `docs/HANDOFF.md`.
