#pragma once

#include "simulation_kernel.hpp"

#include <atomic>

namespace agame::sim {

class TrajectoryPredictor {
public:
	TrajectoryPredictionState predict(const PlayerEstimateState &pEstimate, const SimulationConfig &pConfig, double pHorizonS, std::uint32_t pSampleStride = 8, const std::atomic_bool *pCancel = nullptr) const;
};

PlayerEstimateState estimateFromSensor(const SensorState &pSensor, const AstronomicalState &pReference, std::uint64_t pEstimateVersion);
InterpolatedRenderState interpolateRenderState(const SimulationSnapshot &pPrevious, const SimulationSnapshot &pCurrent, double pWeight, const Vec3d &pRenderOriginM);

} // namespace agame::sim
