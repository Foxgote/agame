#include "sim/prediction.hpp"
#include "sim/snapshot_buffer.hpp"

#include <algorithm>
#include <cmath>
#include <cstdlib>
#include <iostream>
#include <memory>

using namespace agame::sim;

namespace {

int failures = 0;

void check(bool pCondition, const char *pMessage) {
	if (!pCondition) {
		std::cerr << "FAIL: " << pMessage << '\n';
		++failures;
	}
}

bool nearlyEqual(double pA, double pB, double pRelativeTolerance = 1.0e-10) {
	const double scale = std::max({1.0, std::abs(pA), std::abs(pB)});
	return std::abs(pA - pB) <= pRelativeTolerance * scale;
}

void testNewtonianPair() {
	DirectNewtonianGravity solver;
	GravityConfig config;
	config.nearCollisionGuardM = 1.0;
	std::vector<BodyState> bodies = {
		BodyState{1, "A", 2.0e20, 1.0, {-1000.0, 0.0, 0.0}, {}, true},
		BodyState{2, "B", 4.0e20, 1.0, {1000.0, 0.0, 0.0}, {}, true},
	};
	const auto accelerations = solver.bodyAccelerations(bodies, config);
	check(accelerations[0].x > 0.0 && accelerations[1].x < 0.0, "pair gravity points each body toward the other");
	check(nearlyEqual(bodies[0].massKg * accelerations[0].x, -bodies[1].massKg * accelerations[1].x), "pair gravity conserves equal and opposite force");
}

void testDeterminismAndVersions() {
	SimulationConfig config;
	config.fixedStepS = 0.5;
	config.gravity.nearCollisionGuardM = 1000.0;
	SimulationKernel first(config);
	SimulationKernel second(config);
	for (int i = 0; i < 100; ++i) {
		check(first.step() && second.step(), "kernel step remains finite");
	}
	check(first.state().version == 100 && first.state().simulationTimeS == 50.0, "fixed steps advance version and coordinate time");
	check(first.state().ship.positionM.x == second.state().ship.positionM.x, "same authored state is bitwise deterministic");
	check(first.state().ship.properTimeS > 0.0 && first.state().ship.properTimeS < first.state().simulationTimeS, "weak-field proper time accumulates below coordinate time");
}

void testFiveStatesAndPredictionCopy() {
	SimulationKernel kernel;
	const auto snapshot = kernel.makeSnapshot();
	NavigationStateBundle states;
	states.trueState = snapshot->trueState;
	states.sensorState = snapshot->sensorState;
	states.playerEstimate = estimateFromSensor(states.sensorState, states.trueState, 7);
	const double trueMass = states.trueState.bodies.front().massKg;
	states.playerEstimate.believedBodies.front().massKg *= 0.5;
	check(states.trueState.bodies.front().massKg == trueMass, "player estimate does not alias true state");

	TrajectoryPredictor predictor;
	states.predictionState = predictor.predict(states.playerEstimate, kernel.config(), 12.0, 2);
	check(states.predictionState.complete && states.predictionState.estimateVersion == 7, "predictor labels copied estimate result");
	check(states.trueState.version == snapshot->trueState.version, "trajectory prediction does not advance authoritative state");

	states.renderState = interpolateRenderState(*snapshot, *snapshot, 0.5, states.trueState.ship.positionM);
	check(states.renderState.shipRenderPositionM.length() < 1.0e-9, "floating origin keeps ship exterior rendering near zero");
	check(states.renderState.sourceVersion == states.trueState.version, "render state records but does not alias source version");
}

void testSensorTidalAndSnapshotBuffer() {
	SimulationKernel kernel;
	SimulationDiagnostics diagnostics;
	diagnostics.activeBodies = 3;
	auto first = kernel.makeSnapshot(diagnostics);
	check(first->sensorState.bodies.front().reportedMassKg != first->trueState.bodies.front().massKg, "sensor observation is noisy and separate from truth");
	check(std::isfinite(first->tidal.severityPerS2) && first->tidal.severityPerS2 > 0.0, "tidal severity is finite and nonzero");

	SnapshotBuffer<SimulationSnapshot> buffer;
	buffer.publish(first);
	kernel.step();
	auto second = kernel.makeSnapshot(diagnostics);
	buffer.publish(second);
	const auto pair = buffer.latestPair();
	check(pair.previous && pair.current, "snapshot buffer exposes two immutable frames");
	check(pair.current->trueState.version > pair.previous->trueState.version, "snapshot versions increase monotonically");
}

void testManualManeuversAndOberthGain() {
	SimulationKernel slow;
	AstronomicalState slowState = slow.copyState();
	slowState.ship.velocityMps = {1000.0, 0.0, 0.0};
	slow.setState(slowState);
	const double slowPropellant = slow.state().ship.propellantMassKg;
	const ManeuverResult slowBurn = slow.applyProgradeBurn(500.0);
	check(slowBurn.applied && slowBurn.appliedDeltaVMps > 499.0, "manual prograde burn applies requested delta-v when fuel permits");
	check(slow.state().ship.propellantMassKg < slowPropellant, "engine burn consumes propellant");

	SimulationKernel fast;
	AstronomicalState fastState = fast.copyState();
	fastState.ship.velocityMps = {10000.0, 0.0, 0.0};
	fast.setState(fastState);
	const ManeuverResult fastBurn = fast.applyProgradeBurn(500.0);
	check(fastBurn.specificEnergyGainJPerKg > slowBurn.specificEnergyGainJPerKg, "same delta-v gains more orbital energy at higher encounter speed");

	const double reactionMass = fast.state().ship.reactionMassKg;
	const double propellantAfterBurn = fast.state().ship.propellantMassKg;
	const ManeuverResult dump = fast.applyReactionMassDump(12000.0);
	check(dump.applied && dump.appliedDeltaVMps > 0.0, "directed reaction-mass dump produces a finite prograde impulse");
	check(nearlyEqual(fast.state().ship.reactionMassKg, reactionMass - 12000.0), "reaction-mass dump expels the requested available mass");
	check(fast.state().ship.propellantMassKg == propellantAfterBurn, "reaction-mass dump does not consume engine propellant");
	check(fast.state().ship.maneuverCount == 2, "ship records manual maneuver count and telemetry");
}

} // namespace

int main() {
	testNewtonianPair();
	testDeterminismAndVersions();
	testFiveStatesAndPredictionCopy();
	testSensorTidalAndSnapshotBuffer();
	testManualManeuversAndOberthGain();
	if (failures != 0) {
		std::cerr << failures << " simulation test(s) failed\n";
		return EXIT_FAILURE;
	}
	std::cout << "All agame simulation tests passed\n";
	return EXIT_SUCCESS;
}
