#pragma once

#include "integrators.hpp"
#include "tidal_math.hpp"

#include <memory>

namespace agame::sim {

struct SimulationConfig {
	double fixedStepS = 1.0;
	double engineExhaustVelocityMps = 45000.0;
	double reactionDumpExhaustVelocityMps = 2500.0;
	GravityConfig gravity;
};

struct ManeuverResult {
	bool applied = false;
	ManeuverKind kind = ManeuverKind::None;
	double requestedAmount = 0.0;
	double appliedDeltaVMps = 0.0;
	double expendedMassKg = 0.0;
	double specificEnergyGainJPerKg = 0.0;
};

class SimulationKernel {
public:
	explicit SimulationKernel(SimulationConfig pConfig = {});

	void setState(AstronomicalState pState);
	const AstronomicalState &state() const { return state_; }
	AstronomicalState copyState() const { return state_; }
	const SimulationConfig &config() const { return config_; }

	bool step();
	ManeuverResult applyProgradeBurn(double pRequestedDeltaVMps);
	ManeuverResult applyReactionMassDump(double pRequestedMassKg);
	std::shared_ptr<const SimulationSnapshot> makeSnapshot(const SimulationDiagnostics &pDiagnostics = {}) const;

	static AstronomicalState makeAuthoredSmokeTestState();
	static SensorState observe(const AstronomicalState &pTrueState);
	static bool isFinite(const AstronomicalState &pState);

private:
	const BodyState *dominantBody() const;
	ManeuverResult applyProgradeImpulse(ManeuverKind pKind, double pRequestedAmount, double pAvailableMassKg, double pExhaustVelocityMps);
	SimulationConfig config_;
	AstronomicalState state_;
	DirectNewtonianGravity gravitySolver_;
};

} // namespace agame::sim
