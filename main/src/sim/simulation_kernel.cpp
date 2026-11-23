#include "simulation_kernel.hpp"

#include <algorithm>
#include <cmath>
#include <utility>

namespace agame::sim {
namespace {

double deterministicNoise(std::uint64_t pSeed) {
	pSeed ^= pSeed >> 33;
	pSeed *= 0xff51afd7ed558ccdULL;
	pSeed ^= pSeed >> 33;
	pSeed *= 0xc4ceb9fe1a85ec53ULL;
	pSeed ^= pSeed >> 33;
	return (static_cast<double>(pSeed & 0xffffULL) / 32767.5) - 1.0;
}

} // namespace

SimulationKernel::SimulationKernel(SimulationConfig pConfig) : config_(pConfig) {
	setState(makeAuthoredSmokeTestState());
}

void SimulationKernel::setState(AstronomicalState pState) {
	std::stable_sort(pState.bodies.begin(), pState.bodies.end(), [](const BodyState &pA, const BodyState &pB) {
		return pA.id < pB.id;
	});
	state_ = std::move(pState);
}

bool SimulationKernel::step() {
	if (config_.fixedStepS <= 0.0 || !isFinite(state_)) {
		return false;
	}
	velocityVerletStepBodies(state_, config_.fixedStepS, gravitySolver_, config_.gravity);
	rk4StepShip(state_, config_.fixedStepS, gravitySolver_, config_.gravity);
	state_.simulationTimeS += config_.fixedStepS;
	++state_.version;
	return isFinite(state_);
}

const BodyState *SimulationKernel::dominantBody() const {
	for (const BodyState &body : state_.bodies) {
		if (body.id == state_.dominantBodyId) {
			return &body;
		}
	}
	return nullptr;
}

ManeuverResult SimulationKernel::applyProgradeImpulse(ManeuverKind pKind, double pRequestedAmount, double pAvailableMassKg, double pExhaustVelocityMps) {
	ManeuverResult result;
	result.kind = pKind;
	result.requestedAmount = pRequestedAmount;
	const BodyState *dominant = dominantBody();
	const double initialMass = state_.ship.totalMassKg();
	if (!dominant || pRequestedAmount <= 0.0 || pAvailableMassKg <= 0.0 || pExhaustVelocityMps <= 0.0 || initialMass <= pAvailableMassKg) {
		return result;
	}

	const Vec3d relativeVelocity = state_.ship.velocityMps - dominant->velocityMps;
	const Vec3d prograde = relativeVelocity.normalized();
	if (prograde.lengthSquared() == 0.0) {
		return result;
	}

	double expendedMass = 0.0;
	double appliedDeltaV = 0.0;
	if (pKind == ManeuverKind::ProgradeEngineBurn) {
		expendedMass = initialMass * (1.0 - std::exp(-pRequestedAmount / pExhaustVelocityMps));
		expendedMass = std::min(expendedMass, pAvailableMassKg);
		appliedDeltaV = pExhaustVelocityMps * std::log(initialMass / (initialMass - expendedMass));
	} else {
		expendedMass = std::min(pRequestedAmount, pAvailableMassKg);
		appliedDeltaV = pExhaustVelocityMps * std::log(initialMass / (initialMass - expendedMass));
	}

	if (expendedMass <= 0.0 || !std::isfinite(appliedDeltaV)) {
		return result;
	}
	const double speedBefore = relativeVelocity.length();
	state_.ship.velocityMps += prograde * appliedDeltaV;
	const double energyGain = speedBefore * appliedDeltaV + 0.5 * appliedDeltaV * appliedDeltaV;
	if (pKind == ManeuverKind::ProgradeEngineBurn) {
		state_.ship.propellantMassKg -= expendedMass;
	} else {
		state_.ship.reactionMassKg -= expendedMass;
	}

	++state_.ship.maneuverCount;
	state_.ship.lastManeuver = pKind;
	state_.ship.lastRequestedAmount = pRequestedAmount;
	state_.ship.lastAppliedDeltaVMps = appliedDeltaV;
	state_.ship.lastExpendedMassKg = expendedMass;
	state_.ship.lastSpecificEnergyGainJPerKg = energyGain;
	state_.ship.lastManeuverTimeS = state_.simulationTimeS;

	result.applied = true;
	result.appliedDeltaVMps = appliedDeltaV;
	result.expendedMassKg = expendedMass;
	result.specificEnergyGainJPerKg = energyGain;
	return result;
}

ManeuverResult SimulationKernel::applyProgradeBurn(double pRequestedDeltaVMps) {
	return applyProgradeImpulse(ManeuverKind::ProgradeEngineBurn, pRequestedDeltaVMps, state_.ship.propellantMassKg, config_.engineExhaustVelocityMps);
}

ManeuverResult SimulationKernel::applyReactionMassDump(double pRequestedMassKg) {
	return applyProgradeImpulse(ManeuverKind::ReactionMassDump, pRequestedMassKg, state_.ship.reactionMassKg, config_.reactionDumpExhaustVelocityMps);
}

std::shared_ptr<const SimulationSnapshot> SimulationKernel::makeSnapshot(const SimulationDiagnostics &pDiagnostics) const {
	auto snapshot = std::make_shared<SimulationSnapshot>();
	snapshot->trueState = state_;
	snapshot->sensorState = observe(state_);
	snapshot->tidal = computeNewtonianTidalResult(state_.ship.positionM, state_.bodies, config_.gravity);
	snapshot->diagnostics = pDiagnostics;
	return snapshot;
}

AstronomicalState SimulationKernel::makeAuthoredSmokeTestState() {
	AstronomicalState state;
	state.gravityModel = GravityModel::NewtonianDominantSchwarzschild1PN;
	state.dominantBodyId = 1;
	state.bodies = {
		BodyState{1, "Compact primary", 1.20e30, 4.0e4, Vec3d{0.0, 0.0, 0.0}, Vec3d{0.0, 0.0, 0.0}, true},
		BodyState{2, "Survey remnant", 6.0e24, 6.0e6, Vec3d{0.0, 0.0, 1.8e9}, Vec3d{205000.0, 0.0, 0.0}, true},
		BodyState{3, "Route marker body", 8.0e22, 2.0e6, Vec3d{-2.2e9, 0.0, -8.0e8}, Vec3d{40000.0, 0.0, -120000.0}, true},
	};
	state.ship.positionM = {1.15e9, 0.0, 2.0e8};
	state.ship.velocityMps = {-25000.0, 0.0, 245000.0};
	return state;
}

SensorState SimulationKernel::observe(const AstronomicalState &pTrueState) {
	SensorState sensor;
	sensor.sourceTrueVersion = pTrueState.version;
	sensor.timestampS = pTrueState.simulationTimeS;
	const double shipScale = std::max(1.0, pTrueState.ship.positionM.length() * 2.0e-5);
	sensor.reportedShipPositionM = pTrueState.ship.positionM + Vec3d{
		deterministicNoise(pTrueState.version + 11) * shipScale,
		deterministicNoise(pTrueState.version + 12) * shipScale,
		deterministicNoise(pTrueState.version + 13) * shipScale,
	};
	for (const BodyState &body : pTrueState.bodies) {
		const double positionSigma = std::max(1.0, (body.positionM - pTrueState.ship.positionM).length() * 2.0e-4);
		const double massSigma = body.massKg * 5.0e-3;
		const std::uint64_t seed = pTrueState.version * 131ULL + body.id * 17ULL;
		sensor.bodies.push_back(SensorBodyObservation{
			body.id,
			body.positionM + Vec3d{deterministicNoise(seed + 1) * positionSigma, deterministicNoise(seed + 2) * positionSigma, deterministicNoise(seed + 3) * positionSigma},
			body.velocityMps + Vec3d{deterministicNoise(seed + 4) * 5.0, deterministicNoise(seed + 5) * 5.0, deterministicNoise(seed + 6) * 5.0},
			body.massKg + deterministicNoise(seed + 7) * massSigma,
			positionSigma,
			massSigma,
		});
	}
	return sensor;
}

bool SimulationKernel::isFinite(const AstronomicalState &pState) {
	if (!std::isfinite(pState.simulationTimeS) || !pState.ship.positionM.isFinite() || !pState.ship.velocityMps.isFinite() || !std::isfinite(pState.ship.properTimeS) ||
		!std::isfinite(pState.ship.dryMassKg) || !std::isfinite(pState.ship.propellantMassKg) || !std::isfinite(pState.ship.reactionMassKg) ||
		pState.ship.dryMassKg <= 0.0 || pState.ship.propellantMassKg < 0.0 || pState.ship.reactionMassKg < 0.0) {
		return false;
	}
	for (const BodyState &body : pState.bodies) {
		if (!std::isfinite(body.massKg) || body.massKg < 0.0 || !body.positionM.isFinite() || !body.velocityMps.isFinite()) {
			return false;
		}
	}
	return true;
}

} // namespace agame::sim
