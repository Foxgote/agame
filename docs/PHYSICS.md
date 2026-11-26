# 2D Physics Prototype

The playable slice uses Godot's built-in 2D physics with the default gravity setting. Physics behavior is kept in scene scripts and physics nodes; the C++ extension is not part of gameplay.

## Player And Weapons

- The player is a `CharacterBody2D` moved in `_physics_process`.
- Horizontal movement sets velocity directly; jump applies an upward velocity; gravity accumulates while airborne.
- Shots are ray queries, so hitscan does not spawn fast-moving projectile bodies or require continuous-collision handling.
- The query uses collision layer 1 and calls `take_damage(amount, direction)` on compatible hit bodies.

## Damageable Props

Breakable crates accumulate health damage. Damage stages are drawn on the crate; at zero health its collision is disabled and it spawns six `RigidBody2D` fragments. Each fragment is cleaned up after six seconds to keep the test scene bounded.

## Ragdoll

The prototype creates a torso, head, upper/lower arms, and upper/lower legs as individual `RigidBody2D` nodes. Nine `PinJoint2D` nodes constrain their anchors. A defeated target or player is replaced by this dynamic rig.

The rig intentionally starts with simple geometry, default gravity, and no active pose control or joint motors. Tune mass, collision filtering, joint limits, and self-collision only after verifying stable motion in the Godot runtime. Avoid simulating terrain as one rigid body per tile; use static terrain and reserve dynamic physics for the limited debris shown here.
