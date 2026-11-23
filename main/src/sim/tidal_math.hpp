#pragma once

#include "gravity_math.hpp"

namespace agame::sim {

TidalResult computeNewtonianTidalResult(const Vec3d &pPositionM, const std::vector<BodyState> &pBodies, const GravityConfig &pConfig);

} // namespace agame::sim
