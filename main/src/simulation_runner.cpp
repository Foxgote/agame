#include "simulation_runner.h"

#include <godot_cpp/core/class_db.hpp>

#include <algorithm>
#include <chrono>
#include <cmath>

using namespace godot;
using namespace agame::sim;

SimulationRunner::SimulationRunner() {
	config_.fixedStepS = 2.0;
	config_.gravity.nearCollisionGuardM = 50000.0;
	config_.gravity.debugMaxAccelerationMps2 = 500000.0;
	workerKernel_ = std::make_unique<SimulationKernel>(config_);
}

SimulationRunner::~SimulationRunner() {
	stopSimulation();
}

void SimulationRunner::_bind_methods() {
	ClassDB::bind_method(D_METHOD("startSimulation"), &SimulationRunner::startSimulation);
	ClassDB::bind_method(D_METHOD("stopSimulation"), &SimulationRunner::stopSimulation);
	ClassDB::bind_method(D_METHOD("setEstimatedMassScale", "scale"), &SimulationRunner::setEstimatedMassScale);
	ClassDB::bind_method(D_METHOD("getEstimatedMassScale"), &SimulationRunner::getEstimatedMassScale);
	ClassDB::bind_method(D_METHOD("setGravityModel", "model"), &SimulationRunner::setGravityModel);
	ClassDB::bind_method(D_METHOD("getGravityModel"), &SimulationRunner::getGravityModel);
	ClassDB::bind_method(D_METHOD("queueProgradeBurn", "deltaVMps"), &SimulationRunner::queueProgradeBurn);
	ClassDB::bind_method(D_METHOD("queueReactionMassDump", "massKg"), &SimulationRunner::queueReactionMassDump);
	ClassDB::bind_method(D_METHOD("getTrueVersion"), &SimulationRunner::getTrueVersion);
	ClassDB::bind_method(D_METHOD("getSimulationTimeS"), &SimulationRunner::getSimulationTimeS);
	ClassDB::bind_method(D_METHOD("getProperTimeS"), &SimulationRunner::getProperTimeS);
	ClassDB::bind_method(D_METHOD("getTruePrimaryMassKg"), &SimulationRunner::getTruePrimaryMassKg);
	ClassDB::bind_method(D_METHOD("getSensorPrimaryMassKg"), &SimulationRunner::getSensorPrimaryMassKg);
	ClassDB::bind_method(D_METHOD("getEstimatedPrimaryMassKg"), &SimulationRunner::getEstimatedPrimaryMassKg);
	ClassDB::bind_method(D_METHOD("getPredictionEndpointErrorM"), &SimulationRunner::getPredictionEndpointErrorM);
	ClassDB::bind_method(D_METHOD("getTidalSeverity"), &SimulationRunner::getTidalSeverity);
	ClassDB::bind_method(D_METHOD("getShipTotalMassKg"), &SimulationRunner::getShipTotalMassKg);
	ClassDB::bind_method(D_METHOD("getShipPropellantMassKg"), &SimulationRunner::getShipPropellantMassKg);
	ClassDB::bind_method(D_METHOD("getShipReactionMassKg"), &SimulationRunner::getShipReactionMassKg);
	ClassDB::bind_method(D_METHOD("getShipRelativeSpeedMps"), &SimulationRunner::getShipRelativeSpeedMps);
	ClassDB::bind_method(D_METHOD("getPreviewOberthEnergyGain", "deltaVMps"), &SimulationRunner::getPreviewOberthEnergyGain);
	ClassDB::bind_method(D_METHOD("getManeuverCount"), &SimulationRunner::getManeuverCount);
	ClassDB::bind_method(D_METHOD("getLastManeuverDeltaVMps"), &SimulationRunner::getLastManeuverDeltaVMps);
	ClassDB::bind_method(D_METHOD("getLastSpecificEnergyGainJPerKg"), &SimulationRunner::getLastSpecificEnergyGainJPerKg);
	ClassDB::bind_method(D_METHOD("getSnapshotAgeMs"), &SimulationRunner::getSnapshotAgeMs);
	ClassDB::bind_method(D_METHOD("getProfiling"), &SimulationRunner::getProfiling);
	ClassDB::bind_method(D_METHOD("getStateSummary"), &SimulationRunner::getStateSummary);
	ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "estimatedMassScale", PROPERTY_HINT_RANGE, "0.5,1.5,0.01"), "setEstimatedMassScale", "getEstimatedMassScale");
	ADD_PROPERTY(PropertyInfo(Variant::INT, "gravityModel", PROPERTY_HINT_ENUM, "Newtonian,Newtonian + dominant Schwarzschild 1PN"), "setGravityModel", "getGravityModel");
	ADD_SIGNAL(MethodInfo("snapshotUpdated", PropertyInfo(Variant::INT, "version")));
	ADD_SIGNAL(MethodInfo("tidalStress", PropertyInfo(Variant::FLOAT, "severityPerS2")));
	ADD_SIGNAL(MethodInfo("maneuverExecuted", PropertyInfo(Variant::STRING_NAME, "kind"), PropertyInfo(Variant::FLOAT, "deltaVMps"), PropertyInfo(Variant::FLOAT, "expendedMassKg"), PropertyInfo(Variant::FLOAT, "specificEnergyGainJPerKg")));
}

void SimulationRunner::_ready() {
	set_process(true);
	startSimulation();
}

void SimulationRunner::_process(double pDelta) {
	consumeSnapshots(pDelta);
}

void SimulationRunner::_exit_tree() {
	stopSimulation();
}

void SimulationRunner::startSimulation() {
	if (running_.exchange(true)) {
		return;
	}
	worker_ = std::thread(&SimulationRunner::workerLoop, this);
}

void SimulationRunner::stopSimulation() {
	running_.store(false);
	if (worker_.joinable()) {
		worker_.join();
	}
}

void SimulationRunner::workerLoop() {
	// This thread touches only the plain-C++ kernel, diagnostics, atomics, and snapshot buffer.
	using Clock = std::chrono::steady_clock;
	constexpr double realStepS = 1.0 / 30.0;
	constexpr std::uint32_t maxCatchUpSteps = 4;
	auto nextStep = Clock::now();
	SimulationDiagnostics diagnostics;

	while (running_.load()) {
		const auto now = Clock::now();
		if (now < nextStep) {
			std::this_thread::sleep_for(std::chrono::milliseconds(1));
			continue;
		}

		std::uint32_t catchUpSteps = 0;
		while (Clock::now() >= nextStep && catchUpSteps < maxCatchUpSteps && running_.load()) {
			const auto started = Clock::now();
			drainManeuverCommands();
			if (!workerKernel_->step()) {
				running_.store(false);
				break;
			}
			const double stepMs = std::chrono::duration<double, std::milli>(Clock::now() - started).count();
			diagnostics.lastStepMs = stepMs;
			diagnostics.averageStepMs = diagnostics.completedSteps == 0 ? stepMs : diagnostics.averageStepMs * 0.95 + stepMs * 0.05;
			diagnostics.maxRecentStepMs = std::max(stepMs, diagnostics.maxRecentStepMs * 0.995);
			++diagnostics.completedSteps;
			++catchUpSteps;
			nextStep += std::chrono::duration_cast<Clock::duration>(std::chrono::duration<double>(realStepS));
		}

		diagnostics.lastCatchUpSteps = catchUpSteps;
		diagnostics.activeBodies = static_cast<std::uint32_t>(workerKernel_->state().bodies.size());
		const auto afterSteps = Clock::now();
		if (afterSteps >= nextStep) {
			const auto lag = afterSteps - nextStep;
			const auto skipped = static_cast<std::uint64_t>(std::chrono::duration<double>(lag).count() / realStepS) + 1;
			diagnostics.skippedCatchUpSteps += skipped;
			nextStep += std::chrono::duration_cast<Clock::duration>(std::chrono::duration<double>(realStepS * static_cast<double>(skipped)));
		}
		snapshots_.publish(workerKernel_->makeSnapshot(diagnostics));
	}
}

void SimulationRunner::drainManeuverCommands() {
	std::deque<ManeuverCommand> commands;
	{
		std::lock_guard<std::mutex> lock(commandMutex_);
		commands.swap(maneuverCommands_);
	}
	for (const ManeuverCommand &command : commands) {
		if (command.kind == ManeuverKind::ProgradeEngineBurn) {
			workerKernel_->applyProgradeBurn(command.amount);
		} else if (command.kind == ManeuverKind::ReactionMassDump) {
			workerKernel_->applyReactionMassDump(command.amount);
		}
	}
}

void SimulationRunner::consumeSnapshots(double pDelta) {
	const auto pair = snapshots_.latestPair();
	if (!pair.current) {
		return;
	}
	interpolationElapsedS_ += pDelta;
	if (pair.current->trueState.version != consumedVersion_) {
		consumedVersion_ = pair.current->trueState.version;
		latestSnapshot_ = pair.current;
		latestArrival_ = std::chrono::steady_clock::now();
		interpolationElapsedS_ = 0.0;
		navigation_.trueState = pair.current->trueState;
		navigation_.sensorState = pair.current->sensorState;
		if (!navigationInitialized_) {
			navigation_.playerEstimate = estimateFromSensor(navigation_.sensorState, navigation_.trueState, 1);
			if (!navigation_.playerEstimate.believedBodies.empty()) {
				navigation_.playerEstimate.believedBodies.front().massKg *= estimatedMassScale_;
			}
			navigationInitialized_ = true;
			rebuildPrediction();
		}
		emit_signal("snapshotUpdated", static_cast<std::int64_t>(consumedVersion_));
		emit_signal("tidalStress", pair.current->tidal.severityPerS2);
		const ShipState &ship = pair.current->trueState.ship;
		if (ship.maneuverCount != consumedManeuverCount_) {
			consumedManeuverCount_ = ship.maneuverCount;
			const StringName kind = ship.lastManeuver == ManeuverKind::ProgradeEngineBurn ? StringName("progradeBurn") : StringName("reactionMassDump");
			emit_signal("maneuverExecuted", kind, ship.lastAppliedDeltaVMps, ship.lastExpendedMassKg, ship.lastSpecificEnergyGainJPerKg);
		}
	}

	const SimulationSnapshot &previous = pair.previous ? *pair.previous : *pair.current;
	const double interpolationWindowS = 1.0 / 30.0;
	const double weight = interpolationWindowS > 0.0 ? interpolationElapsedS_ / interpolationWindowS : 1.0;
	navigation_.renderState = interpolateRenderState(previous, *pair.current, weight, pair.current->trueState.ship.positionM);
}

void SimulationRunner::setEstimatedMassScale(double pScale) {
	estimatedMassScale_ = std::clamp(pScale, 0.1, 3.0);
	if (!navigationInitialized_ || navigation_.sensorState.bodies.empty() || navigation_.playerEstimate.believedBodies.empty()) {
		return;
	}
	navigation_.playerEstimate.believedBodies.front().massKg = navigation_.sensorState.bodies.front().reportedMassKg * estimatedMassScale_;
	++navigation_.playerEstimate.estimateVersion;
	rebuildPrediction();
}

void SimulationRunner::setGravityModel(int pModel) {
	if (!navigationInitialized_) {
		return;
	}
	navigation_.playerEstimate.selectedModel = pModel == 0 ? GravityModel::Newtonian : GravityModel::NewtonianDominantSchwarzschild1PN;
	++navigation_.playerEstimate.estimateVersion;
	rebuildPrediction();
}

int SimulationRunner::getGravityModel() const {
	return navigation_.playerEstimate.selectedModel == GravityModel::Newtonian ? 0 : 1;
}

bool SimulationRunner::queueProgradeBurn(double pDeltaVMps) {
	if (!running_.load() || !std::isfinite(pDeltaVMps) || pDeltaVMps <= 0.0) {
		return false;
	}
	std::lock_guard<std::mutex> lock(commandMutex_);
	if (maneuverCommands_.size() >= 16) return false;
	maneuverCommands_.push_back({ManeuverKind::ProgradeEngineBurn, std::clamp(pDeltaVMps, 1.0, 5000.0)});
	return true;
}

bool SimulationRunner::queueReactionMassDump(double pMassKg) {
	if (!running_.load() || !std::isfinite(pMassKg) || pMassKg <= 0.0) {
		return false;
	}
	std::lock_guard<std::mutex> lock(commandMutex_);
	if (maneuverCommands_.size() >= 16) return false;
	maneuverCommands_.push_back({ManeuverKind::ReactionMassDump, std::clamp(pMassKg, 1.0, 50000.0)});
	return true;
}

void SimulationRunner::rebuildPrediction() {
	TrajectoryPredictor predictor;
	navigation_.predictionState = predictor.predict(navigation_.playerEstimate, config_, 240.0, 4);
}

std::int64_t SimulationRunner::getTrueVersion() const { return static_cast<std::int64_t>(navigation_.trueState.version); }
double SimulationRunner::getSimulationTimeS() const { return navigation_.trueState.simulationTimeS; }
double SimulationRunner::getProperTimeS() const { return navigation_.trueState.ship.properTimeS; }
double SimulationRunner::getTruePrimaryMassKg() const { return navigation_.trueState.bodies.empty() ? 0.0 : navigation_.trueState.bodies.front().massKg; }
double SimulationRunner::getSensorPrimaryMassKg() const { return navigation_.sensorState.bodies.empty() ? 0.0 : navigation_.sensorState.bodies.front().reportedMassKg; }
double SimulationRunner::getEstimatedPrimaryMassKg() const { return navigation_.playerEstimate.believedBodies.empty() ? 0.0 : navigation_.playerEstimate.believedBodies.front().massKg; }
double SimulationRunner::getTidalSeverity() const { return latestSnapshot_ ? latestSnapshot_->tidal.severityPerS2 : 0.0; }
double SimulationRunner::getShipTotalMassKg() const { return navigation_.trueState.ship.totalMassKg(); }
double SimulationRunner::getShipPropellantMassKg() const { return navigation_.trueState.ship.propellantMassKg; }
double SimulationRunner::getShipReactionMassKg() const { return navigation_.trueState.ship.reactionMassKg; }

double SimulationRunner::getShipRelativeSpeedMps() const {
	for (const BodyState &body : navigation_.trueState.bodies) {
		if (body.id == navigation_.trueState.dominantBodyId) {
			return (navigation_.trueState.ship.velocityMps - body.velocityMps).length();
		}
	}
	return navigation_.trueState.ship.velocityMps.length();
}

double SimulationRunner::getPreviewOberthEnergyGain(double pDeltaVMps) const {
	const double deltaV = std::max(0.0, pDeltaVMps);
	return getShipRelativeSpeedMps() * deltaV + 0.5 * deltaV * deltaV;
}

std::int64_t SimulationRunner::getManeuverCount() const { return static_cast<std::int64_t>(navigation_.trueState.ship.maneuverCount); }
double SimulationRunner::getLastManeuverDeltaVMps() const { return navigation_.trueState.ship.lastAppliedDeltaVMps; }
double SimulationRunner::getLastSpecificEnergyGainJPerKg() const { return navigation_.trueState.ship.lastSpecificEnergyGainJPerKg; }

double SimulationRunner::getPredictionEndpointErrorM() const {
	if (navigation_.predictionState.samples.empty() || navigation_.trueState.version == 0) {
		return 0.0;
	}
	return (navigation_.predictionState.samples.back().positionM - navigation_.trueState.ship.positionM).length();
}

double SimulationRunner::getSnapshotAgeMs() const {
	if (!latestSnapshot_) {
		return 0.0;
	}
	return std::chrono::duration<double, std::milli>(std::chrono::steady_clock::now() - latestArrival_).count();
}

Dictionary SimulationRunner::getProfiling() const {
	Dictionary values;
	if (!latestSnapshot_) {
		return values;
	}
	const SimulationDiagnostics &diagnostics = latestSnapshot_->diagnostics;
	values["averageStepMs"] = diagnostics.averageStepMs;
	values["maxRecentStepMs"] = diagnostics.maxRecentStepMs;
	values["snapshotAgeMs"] = getSnapshotAgeMs();
	values["catchUpSteps"] = diagnostics.lastCatchUpSteps;
	values["skippedCatchUpSteps"] = static_cast<std::int64_t>(diagnostics.skippedCatchUpSteps);
	values["activeBodies"] = diagnostics.activeBodies;
	return values;
}

String SimulationRunner::getStateSummary() const {
	return "TRUE v" + String::num_uint64(navigation_.trueState.version) + "  SENSOR v" + String::num_uint64(navigation_.sensorState.sourceTrueVersion) + "  ESTIMATE v" + String::num_uint64(navigation_.playerEstimate.estimateVersion) + "  PREDICTION v" + String::num_uint64(navigation_.predictionState.estimateVersion) + "  RENDER v" + String::num_uint64(navigation_.renderState.sourceVersion);
}
