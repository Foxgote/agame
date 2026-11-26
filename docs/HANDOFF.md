# 2D Prototype Handoff

## Run

Open `main/project/project.godot` with Godot 4.6 and run `main.tscn`. The project currently needs no imported artwork and does not load the C++ extension.

## Controls

- `A` / `D`: move
- `Space`: jump
- Mouse: aim
- Left mouse button: fire
- `R`: restart

The first stage is a graybox with platforms, breakable crates, and one training target. Shooting crates advances their damage state and eventually creates temporary physics debris. Defeating the target triggers a connected-body ragdoll.

## Validation

Run:

```powershell
Godot_v4.6-stable_win64_console.exe --headless --path main\project --script res://tests/runtime_smoke.gd
```

The smoke test checks project startup, prop damage/destruction, and ragdoll body/joint creation. Replace the placeholder visuals and tune the physics in small steps; keep world terrain static and put hard limits on dynamic debris.

The decompiled folders are preserved as references only. The new project contains no copied game assets.
