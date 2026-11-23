#pragma once

#include "gravity_math.hpp"

namespace agame::sim {

void velocityVerletStepBodies(AstronomicalState &pState, double pDeltaS, const GravitySolver &pSolver, const GravityConfig &pConfig);
void rk4StepShip(AstronomicalState &pState, double pDeltaS, const GravitySolver &pSolver, const GravityConfig &pConfig);

} // namespace agame::sim
