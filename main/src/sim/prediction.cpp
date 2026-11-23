#include "prediction.hpp"

#include <algorithm>
#include <cmath>

namespace agame::sim {

TrajectoryPredictionState TrajectoryPredictor::predict(const PlayerEstimateState &pEstimate, const SimulationConfig &pConfig, double pHorizonS, std::uint32_t pSampleStride, const std::atomic_bool *pCancel) const {
	TrajectoryPredictionState prediction;
	prediction.estimateVersion = pEstimate.estimateVersion;
	if (pConfig.fixedStepS <= 0.0 || pHorizonS <= 0.0) {
		return prediction;
	}

	AstronomicalState copiedState;
	copiedState.bodies = pEstimate.believedBodies;
	copiedState.ship = pEstimate.believedShip;
	copiedState.gravityModel = pEstimate.selectedModel;
	copiedState.dominantBodyId = pEstimate.selectedDominantBodyId;
	SimulationKernel copiedKernel(pConfig);
	copiedKernel.setState(std::move(copiedState));

	const std::uint64_t stepCount = static_cast<std::uint64_t>(std::ceil(pHorizonS / pConfig.fixedStepS));
	const std::uint32_t stride = std::max<std::uint32_t>(1, pSampleStride);
	prediction.samples.push_back({0.0, copiedKernel.state().ship.positionM, copiedKernel.state().ship.velocityMps});
	for (std::uint64_t stepIndex = 0; stepIndex < stepCount; ++stepIndex) {
		if (pCancel && pCancel->load()) {
			return prediction;
		}
		if (!copiedKernel.step()) {
			return prediction;
		}
		if ((stepIndex + 1) % stride == 0 || stepIndex + 1 == stepCount) {
			const AstronomicalState &state = copiedKernel.state();
			prediction.samples.push_back({state.simulationTimeS, state.ship.positionM, state.ship.velocityMps});
		}
	}
	prediction.complete = true;
	return prediction;
}

PlayerEstimateState estimateFromSensor(const SensorState &pSensor, const AstronomicalState &pReference, std::uint64_t pEstimateVersion) {
	PlayerEstimateState estimate;
	estimate.estimateVersion = pEstimateVersion;
	estimate.enteredAtS = pSensor.timestampS;
	estimate.believedShip = pReference.ship;
	estimate.believedShip.positionM = pSensor.reportedShipPositionM;
	estimate.selectedModel = GravityModel::Newtonian;
	estimate.selectedDominantBodyId = pReference.dominantBodyId;
	for (const SensorBodyObservation &observation : pSensor.bodies) {
		auto trueBody = std::find_if(pReference.bodies.begin(), pReference.bodies.end(), [&](const BodyState &pBody) { return pBody.id == observation.id; });
		BodyState body;
		body.id = observation.id;
		body.name = trueBody != pReference.bodies.end() ? trueBody->name : "Unclassified contact";
		body.radiusM = trueBody != pReference.bodies.end() ? trueBody->radiusM : 0.0;
		body.positionM = observation.reportedPositionM;
		body.velocityMps = observation.reportedVelocityMps;
		body.massKg = observation.reportedMassKg;
		estimate.believedBodies.push_back(std::move(body));
	}
	return estimate;
}

InterpolatedRenderState interpolateRenderState(const SimulationSnapshot &pPrevious, const SimulationSnapshot &pCurrent, double pWeight, const Vec3d &pRenderOriginM) {
	InterpolatedRenderState render;
	const double weight = std::clamp(pWeight, 0.0, 1.0);
	render.sourceVersion = pCurrent.trueState.version;
	render.renderTimeS = pPrevious.trueState.simulationTimeS + (pCurrent.trueState.simulationTimeS - pPrevious.trueState.simulationTimeS) * weight;
	render.renderOriginM = pRenderOriginM;
	render.shipRenderPositionM = lerp(pPrevious.trueState.ship.positionM, pCurrent.trueState.ship.positionM, weight) - pRenderOriginM;
	for (const BodyState &currentBody : pCurrent.trueState.bodies) {
		auto previousBody = std::find_if(pPrevious.trueState.bodies.begin(), pPrevious.trueState.bodies.end(), [&](const BodyState &pBody) { return pBody.id == currentBody.id; });
		const Vec3d previousPosition = previousBody != pPrevious.trueState.bodies.end() ? previousBody->positionM : currentBody.positionM;
		render.bodies.push_back({currentBody.id, lerp(previousPosition, currentBody.positionM, weight) - pRenderOriginM});
	}
	return render;
}

} // namespace agame::sim
