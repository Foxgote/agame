# 2D Shooter Prototype

This Godot 4.6 project is a small, original 2D platform-shooter starting point. It is separate in gameplay from the godot-cpp sample extension, which remains available for binding/build experiments.

## Run

Open `main/project/project.godot` in Godot 4.6 and run the main scene.

Controls:

- `A` / `D`: move
- `Space`: jump
- Mouse: aim
- Left mouse button: fire
- `R`: restart

## Current Slice

- `scripts/player.gd` implements platform movement and ray-based hitscan.
- `scripts/breakable.gd` tracks health, shows damage, and spawns short-lived rigid debris when destroyed.
- `scripts/target.gd` turns a defeated target into a multi-body physics ragdoll.
- `scripts/ragdoll.gd` builds the ragdoll from `RigidBody2D` segments joined by `PinJoint2D`.
- `tests/runtime_smoke.gd` checks scene startup, damage progression, debris breakage, and ragdoll joints.

The art is drawn from simple shapes and is only placeholder presentation. The Unity and PB2 decompilation directories are retained as local reference material; they are not loaded by the game.

Run the headless smoke test with:

```powershell
Godot_v4.6-stable_win64_console.exe --headless --path main\project --script res://tests/runtime_smoke.gd
```

The `main/CMakeLists.txt` and `main/SConstruct` targets build only the generic godot-cpp example classes. The 2D game itself uses Godot/GDScript and does not require the extension.
