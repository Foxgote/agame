#pragma once

#include "state.hpp"

#include <vector>

namespace agame::sim {

constexpr double gravitationalConstant = 6.67430e-11;
constexpr double speedOfLightMps = 299792458.0;

struct GravityConfig {
	double nearCollisionGuardM = 1000.0;
	double debugMaxAccelerationMps2 = 0.0;
};

class GravitySolver {
public:
	virtual ~GravitySolver() = default;
	virtual std::vector<Vec3d> bodyAccelerations(const std::vector<BodyState> &pBodies, const GravityConfig &pConfig) const = 0;
	virtual Vec3d accelerationAt(const Vec3d &pPositionM, const std::vector<BodyState> &pBodies, const GravityConfig &pConfig) const = 0;
};

class DirectNewtonianGravity final : public GravitySolver {
public:
	std::vector<Vec3d> bodyAccelerations(const std::vector<BodyState> &pBodies, const GravityConfig &pConfig) const override;
	Vec3d accelerationAt(const Vec3d &pPositionM, const std::vector<BodyState> &pBodies, const GravityConfig &pConfig) const override;
};

Vec3d newtonianAccelerationFromBody(const Vec3d &pPositionM, const BodyState &pBody, const GravityConfig &pConfig);
Vec3d schwarzschild1pnCorrection(const ShipState &pShip, const BodyState &pDominantBody, const GravityConfig &pConfig);
Vec3d shipAcceleration(const ShipState &pShip, const AstronomicalState &pState, const GravitySolver &pSolver, const GravityConfig &pConfig);
double gravitationalPotential(const Vec3d &pPositionM, const std::vector<BodyState> &pBodies, const GravityConfig &pConfig);
double weakFieldProperTimeRate(const ShipState &pShip, const std::vector<BodyState> &pBodies, const GravityConfig &pConfig);

} // namespace agame::sim
