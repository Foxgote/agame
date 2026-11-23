#pragma once

#include "vec3d.hpp"

#include <array>
#include <cstdint>
#include <string>
#include <vector>

namespace agame::sim {

using BodyId = std::uint64_t;

enum class GravityModel : std::uint8_t {
	Newtonian = 0,
	NewtonianDominantSchwarzschild1PN = 1,
	KerrReserved = 2,
	LenseThirringReserved = 3,
	MovingStrongFieldReserved = 4,
};

enum class ManeuverKind : std::uint8_t {
	None = 0,
	ProgradeEngineBurn = 1,
	ReactionMassDump = 2,
};

struct BodyState {
	BodyId id = 0;
	std::string name;
	double massKg = 0.0;
	double radiusM = 0.0;
	Vec3d positionM;
	Vec3d velocityMps;
	bool active = true;
};

struct ShipState {
	Vec3d positionM;
	Vec3d velocityMps;
	double properTimeS = 0.0;
	double dryMassKg = 780000.0;
	double propellantMassKg = 180000.0;
	double reactionMassKg = 120000.0;
	std::uint64_t maneuverCount = 0;
	ManeuverKind lastManeuver = ManeuverKind::None;
	double lastRequestedAmount = 0.0;
	double lastAppliedDeltaVMps = 0.0;
	double lastExpendedMassKg = 0.0;
	double lastSpecificEnergyGainJPerKg = 0.0;
	double lastManeuverTimeS = 0.0;

	double totalMassKg() const { return dryMassKg + propellantMassKg + reactionMassKg; }
};

struct AstronomicalState {
	std::uint64_t version = 0;
	double simulationTimeS = 0.0;
	std::vector<BodyState> bodies;
	ShipState ship;
	GravityModel gravityModel = GravityModel::NewtonianDominantSchwarzschild1PN;
	BodyId dominantBodyId = 0;
};

struct SensorBodyObservation {
	BodyId id = 0;
	Vec3d reportedPositionM;
	Vec3d reportedVelocityMps;
	double reportedMassKg = 0.0;
	double oneSigmaPositionM = 0.0;
	double oneSigmaMassKg = 0.0;
};

struct SensorState {
	std::uint64_t sourceTrueVersion = 0;
	double timestampS = 0.0;
	std::vector<SensorBodyObservation> bodies;
	Vec3d reportedShipPositionM;
};

struct PlayerEstimateState {
	std::uint64_t estimateVersion = 0;
	double enteredAtS = 0.0;
	std::vector<BodyState> believedBodies;
	ShipState believedShip;
	GravityModel selectedModel = GravityModel::Newtonian;
	BodyId selectedDominantBodyId = 0;
};

struct TrajectorySample {
	double timeS = 0.0;
	Vec3d positionM;
	Vec3d velocityMps;
};

struct TrajectoryPredictionState {
	std::uint64_t estimateVersion = 0;
	bool complete = false;
	std::vector<TrajectorySample> samples;
};

struct RenderBodyState {
	BodyId id = 0;
	Vec3d renderPositionM;
};

struct InterpolatedRenderState {
	std::uint64_t sourceVersion = 0;
	double renderTimeS = 0.0;
	Vec3d renderOriginM;
	Vec3d shipRenderPositionM;
	std::vector<RenderBodyState> bodies;
};

struct TidalResult {
	std::array<double, 9> tensorPerS2{};
	double severityPerS2 = 0.0;
	Vec3d principalStretchDirection{1.0, 0.0, 0.0};
};

struct SimulationDiagnostics {
	double lastStepMs = 0.0;
	double averageStepMs = 0.0;
	double maxRecentStepMs = 0.0;
	std::uint64_t completedSteps = 0;
	std::uint64_t skippedCatchUpSteps = 0;
	std::uint32_t lastCatchUpSteps = 0;
	std::uint32_t activeBodies = 0;
};

struct SimulationSnapshot {
	AstronomicalState trueState;
	SensorState sensorState;
	TidalResult tidal;
	SimulationDiagnostics diagnostics;
};

// This aggregate deliberately owns five separate values. None is a reference or pointer
// to another state, so an incorrect player estimate cannot alter the true simulation.
struct NavigationStateBundle {
	AstronomicalState trueState;
	SensorState sensorState;
	PlayerEstimateState playerEstimate;
	TrajectoryPredictionState predictionState;
	InterpolatedRenderState renderState;
};

} // namespace agame::sim
