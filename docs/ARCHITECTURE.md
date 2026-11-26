# 2D Prototype Architecture

The playable project is `main/project`, a Godot 4.6 2D project using GDScript and built-in physics nodes. The game has no dependency on the C++ extension.

## Runtime Ownership

- `main.gd` creates the placeholder test stage, input actions, player, target, breakables, and HUD.
- `scripts/player.gd` owns player movement, aiming, hitscan queries, health, and the transition to a ragdoll.
- `scripts/breakable.gd` owns prop health, damage visuals, collision removal, and temporary debris.
- `scripts/target.gd` owns target health and swaps the static target body for a ragdoll on defeat.
- `scripts/ragdoll.gd` creates dynamic body segments and connects them with 2D pin joints.

Damage is sent to a hit object's `take_damage(amount, direction)` method. This keeps weapon collision queries independent from target and prop behavior.

## Physics Bounds

Player and target shapes use collision layer 1. Breakable debris shares that layer and is removed after six seconds. Destroyed props are replaced with a bounded set of six small rigid bodies. The ragdoll prototype has ten body segments connected by nine pin joints; it is a learning scaffold, not a production character rig.

## Repository Boundary

`main/src/example.cpp`, `main/src/hello_world.cpp`, and `main/src/register_types.cpp` remain as generic godot-cpp sample code. The PB2/Unity reference exports remain separate from the playable project and are not imported as game assets.
