#include "integrators.hpp"

namespace agame::sim {

void velocityVerletStepBodies(AstronomicalState &pState, double pDeltaS, const GravitySolver &pSolver, const GravityConfig &pConfig) {
	const std::vector<Vec3d> initialAccelerations = pSolver.bodyAccelerations(pState.bodies, pConfig);
	const double halfDtSquared = 0.5 * pDeltaS * pDeltaS;
	for (std::size_t i = 0; i < pState.bodies.size(); ++i) {
		if (!pState.bodies[i].active) {
			continue;
		}
		pState.bodies[i].positionM += pState.bodies[i].velocityMps * pDeltaS + initialAccelerations[i] * halfDtSquared;
	}

	const std::vector<Vec3d> finalAccelerations = pSolver.bodyAccelerations(pState.bodies, pConfig);
	for (std::size_t i = 0; i < pState.bodies.size(); ++i) {
		if (!pState.bodies[i].active) {
			continue;
		}
		pState.bodies[i].velocityMps += (initialAccelerations[i] + finalAccelerations[i]) * (0.5 * pDeltaS);
	}
}

void rk4StepShip(AstronomicalState &pState, double pDeltaS, const GravitySolver &pSolver, const GravityConfig &pConfig) {
	const ShipState initial = pState.ship;

	const Vec3d k1Position = initial.velocityMps;
	const Vec3d k1Velocity = shipAcceleration(initial, pState, pSolver, pConfig);

	ShipState stage = initial;
	stage.positionM = initial.positionM + k1Position * (0.5 * pDeltaS);
	stage.velocityMps = initial.velocityMps + k1Velocity * (0.5 * pDeltaS);
	const Vec3d k2Position = stage.velocityMps;
	const Vec3d k2Velocity = shipAcceleration(stage, pState, pSolver, pConfig);

	stage.positionM = initial.positionM + k2Position * (0.5 * pDeltaS);
	stage.velocityMps = initial.velocityMps + k2Velocity * (0.5 * pDeltaS);
	const Vec3d k3Position = stage.velocityMps;
	const Vec3d k3Velocity = shipAcceleration(stage, pState, pSolver, pConfig);

	stage.positionM = initial.positionM + k3Position * pDeltaS;
	stage.velocityMps = initial.velocityMps + k3Velocity * pDeltaS;
	const Vec3d k4Position = stage.velocityMps;
	const Vec3d k4Velocity = shipAcceleration(stage, pState, pSolver, pConfig);

	pState.ship.positionM = initial.positionM + (k1Position + k2Position * 2.0 + k3Position * 2.0 + k4Position) * (pDeltaS / 6.0);
	pState.ship.velocityMps = initial.velocityMps + (k1Velocity + k2Velocity * 2.0 + k3Velocity * 2.0 + k4Velocity) * (pDeltaS / 6.0);
	pState.ship.properTimeS += weakFieldProperTimeRate(initial, pState.bodies, pConfig) * pDeltaS;
}

} // namespace agame::sim
