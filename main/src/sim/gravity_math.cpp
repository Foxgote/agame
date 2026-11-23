#include "gravity_math.hpp"

#include <algorithm>
#include <cmath>

namespace agame::sim {
namespace {

Vec3d clampAcceleration(const Vec3d &pAcceleration, double pMaximum) {
	if (pMaximum <= 0.0) {
		return pAcceleration;
	}
	const double magnitude = pAcceleration.length();
	return magnitude > pMaximum ? pAcceleration * (pMaximum / magnitude) : pAcceleration;
}

const BodyState *findBody(const std::vector<BodyState> &pBodies, BodyId pId) {
	for (const BodyState &body : pBodies) {
		if (body.id == pId) {
			return &body;
		}
	}
	return nullptr;
}

} // namespace

Vec3d newtonianAccelerationFromBody(const Vec3d &pPositionM, const BodyState &pBody, const GravityConfig &pConfig) {
	if (!pBody.active || pBody.massKg <= 0.0) {
		return {};
	}
	const Vec3d offset = pBody.positionM - pPositionM;
	const double guard = std::max(1.0e-6, pConfig.nearCollisionGuardM);
	const double distance = std::max(offset.length(), guard);
	return offset * (gravitationalConstant * pBody.massKg / (distance * distance * distance));
}

std::vector<Vec3d> DirectNewtonianGravity::bodyAccelerations(const std::vector<BodyState> &pBodies, const GravityConfig &pConfig) const {
	std::vector<Vec3d> accelerations(pBodies.size());
	for (std::size_t i = 0; i < pBodies.size(); ++i) {
		if (!pBodies[i].active) {
			continue;
		}
		for (std::size_t j = 0; j < pBodies.size(); ++j) {
			if (i == j) {
				continue;
			}
			accelerations[i] += newtonianAccelerationFromBody(pBodies[i].positionM, pBodies[j], pConfig);
		}
		accelerations[i] = clampAcceleration(accelerations[i], pConfig.debugMaxAccelerationMps2);
	}
	return accelerations;
}

Vec3d DirectNewtonianGravity::accelerationAt(const Vec3d &pPositionM, const std::vector<BodyState> &pBodies, const GravityConfig &pConfig) const {
	Vec3d acceleration;
	for (const BodyState &body : pBodies) {
		acceleration += newtonianAccelerationFromBody(pPositionM, body, pConfig);
	}
	return clampAcceleration(acceleration, pConfig.debugMaxAccelerationMps2);
}

Vec3d schwarzschild1pnCorrection(const ShipState &pShip, const BodyState &pDominantBody, const GravityConfig &pConfig) {
	const Vec3d relativePosition = pShip.positionM - pDominantBody.positionM;
	const double guard = std::max(1.0e-6, pConfig.nearCollisionGuardM);
	const double radius = std::max(relativePosition.length(), guard);
	const Vec3d radialUnit = relativePosition / radius;
	const Vec3d relativeVelocity = pShip.velocityMps - pDominantBody.velocityMps;
	const double velocitySquared = relativeVelocity.lengthSquared();
	const double radialVelocity = dot(relativeVelocity, radialUnit);
	const double mu = gravitationalConstant * pDominantBody.massKg;
	const double factor = mu / (speedOfLightMps * speedOfLightMps * radius * radius);
	return (radialUnit * (4.0 * mu / radius - velocitySquared) + relativeVelocity * (4.0 * radialVelocity)) * factor;
}

Vec3d shipAcceleration(const ShipState &pShip, const AstronomicalState &pState, const GravitySolver &pSolver, const GravityConfig &pConfig) {
	Vec3d acceleration = pSolver.accelerationAt(pShip.positionM, pState.bodies, pConfig);
	if (pState.gravityModel == GravityModel::NewtonianDominantSchwarzschild1PN) {
		if (const BodyState *dominant = findBody(pState.bodies, pState.dominantBodyId)) {
			acceleration += schwarzschild1pnCorrection(pShip, *dominant, pConfig);
		}
	}
	return clampAcceleration(acceleration, pConfig.debugMaxAccelerationMps2);
}

double gravitationalPotential(const Vec3d &pPositionM, const std::vector<BodyState> &pBodies, const GravityConfig &pConfig) {
	double potential = 0.0;
	const double guard = std::max(1.0e-6, pConfig.nearCollisionGuardM);
	for (const BodyState &body : pBodies) {
		if (!body.active || body.massKg <= 0.0) {
			continue;
		}
		potential -= gravitationalConstant * body.massKg / std::max((body.positionM - pPositionM).length(), guard);
	}
	return potential;
}

double weakFieldProperTimeRate(const ShipState &pShip, const std::vector<BodyState> &pBodies, const GravityConfig &pConfig) {
	const double cSquared = speedOfLightMps * speedOfLightMps;
	const double rate = 1.0 + gravitationalPotential(pShip.positionM, pBodies, pConfig) / cSquared - pShip.velocityMps.lengthSquared() / (2.0 * cSquared);
	return std::max(0.0, rate);
}

} // namespace agame::sim
