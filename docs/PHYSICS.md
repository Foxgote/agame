# Physics Model And Limits

This scaffold is not exact general relativity and does not simulate dynamic spacetime, black-hole interiors, or exact horizon crossing.

## Celestial Bodies

Authored bodies use direct pairwise Newtonian gravity in deterministic body-ID order:

`a_i = sum(G * m_j * (r_j - r_i) / |r_j - r_i|^3)`

The body integrator is fixed-step velocity-Verlet. Work is `O(N^2)`, appropriate for a few to a few dozen authored bodies. `GravitySolver` is replaceable if later maps justify a hierarchy. There is no Barnes-Hut, FMM, GPU solver, or hidden softening.

The configurable near-collision guard clamps the distance denominator. This avoids singular numerical values but is not a physical collision model. A configurable acceleration clamp exists for debug containment and is disabled by setting it to zero.

## Ship

The ship is a test particle and does not perturb celestial bodies. It receives summed Newtonian acceleration from every active body. The selected composite mode additionally applies one Schwarzschild first post-Newtonian correction relative to one explicitly selected dominant compact body:

`a_1PN = mu / (c^2 r^2) * [n * (4 mu / r - v^2) + v * 4 v_r]`

The sign convention uses `r = ship - body`, so Newtonian acceleration is inward and the correction matches the documented project formula. Multiple independent one-body corrections are not summed or presented as a rigorous relativistic many-body result.

Because the correction depends on velocity, the ship uses fixed-step RK4 rather than position-only Verlet. The approximation is valid only in weak-to-moderate fields where post-Newtonian expansion remains meaningful. It is not accurate near the singularity or for horizon crossing.

Reserved gravity model IDs exist for Kerr, Lense-Thirring, and moving-source strong-field work. They are not implemented.

## Tides

The Newtonian tidal tensor sums:

`T_ab = G M / r^3 * (3 n_a n_b - delta_ab)`

The result includes the tensor, its Frobenius-norm severity, and a power-iterated principal direction. It drives gameplay damage and sensor/UI feedback through main-thread services; it never calls camera or audio APIs.

## Manual Burns And Matter Dump

The manual engine control requests a prograde delta-v relative to the selected dominant body. Available propellant limits the achieved impulse through the ideal rocket equation:

`delta_v = v_e * ln(m_initial / m_final)`

The displayed instantaneous specific orbital-energy increase is:

`delta_epsilon = v * delta_v + 0.5 * delta_v^2`

This is the playable Oberth relationship: the same delta-v produces a larger energy change when the ship is moving faster near periapsis. The burn is not a free gravity boost; it consumes propellant.

The matter-dump control models a deliberately directed retrograde ejecta stream. It uses a lower effective exhaust velocity and consumes dedicated reaction mass, producing a smaller prograde impulse through the same momentum principle. Merely discarding stationary mass would not change the trajectory, so that nonphysical behavior is intentionally absent.

The current maneuver is instantaneous at a fixed-step boundary. Finite burn duration, thrust limits, engine integrity, attitude alignment, ejecta-body simulation, and a committed autonomous burn schedule remain future work.

## Proper Time

The non-authoritative ship clock accumulates the weak-field, low-speed approximation:

`d_tau/dt ~= 1 + Phi/c^2 - v^2/(2c^2)`

with `Phi = -sum(G M_i / r_i)`. It is useful for clock presentation, future timestamps, and narrative timing, not evidence of strong-field accuracy.

## Future Work

Reserved directions include Kerr-inspired dynamics, Lense-Thirring frame dragging, improved timelike geodesics, null geodesics, lensing, redshift/frequency calculations, sensor propagation delay, and improved proper-time integration. None is claimed complete in this pass.
