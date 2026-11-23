#pragma once

#include "sim/prediction.hpp"
#include "sim/snapshot_buffer.hpp"

#include <godot_cpp/classes/node.hpp>
#include <godot_cpp/variant/dictionary.hpp>

#include <atomic>
#include <chrono>
#include <deque>
#include <memory>
#include <mutex>
#include <thread>

class SimulationRunner : public godot::Node {
	GDCLASS(SimulationRunner, godot::Node);

public:
	SimulationRunner();
	~SimulationRunner() override;

	void _ready() override;
	void _process(double pDelta) override;
	void _exit_tree() override;

	void startSimulation();
	void stopSimulation();
	void setEstimatedMassScale(double pScale);
	double getEstimatedMassScale() const { return estimatedMassScale_; }
	void setGravityModel(int pModel);
	int getGravityModel() const;
	bool queueProgradeBurn(double pDeltaVMps);
	bool queueReactionMassDump(double pMassKg);

	std::int64_t getTrueVersion() const;
	double getSimulationTimeS() const;
	double getProperTimeS() const;
	double getTruePrimaryMassKg() const;
	double getSensorPrimaryMassKg() const;
	double getEstimatedPrimaryMassKg() const;
	double getPredictionEndpointErrorM() const;
	double getTidalSeverity() const;
	double getShipTotalMassKg() const;
	double getShipPropellantMassKg() const;
	double getShipReactionMassKg() const;
	double getShipRelativeSpeedMps() const;
	double getPreviewOberthEnergyGain(double pDeltaVMps) const;
	std::int64_t getManeuverCount() const;
	double getLastManeuverDeltaVMps() const;
	double getLastSpecificEnergyGainJPerKg() const;
	double getSnapshotAgeMs() const;
	godot::Dictionary getProfiling() const;
	godot::String getStateSummary() const;

protected:
	static void _bind_methods();

private:
	void workerLoop();
	void drainManeuverCommands();
	void consumeSnapshots(double pDelta);
	void rebuildPrediction();

	agame::sim::SimulationConfig config_;
	std::unique_ptr<agame::sim::SimulationKernel> workerKernel_;
	agame::sim::SnapshotBuffer<agame::sim::SimulationSnapshot> snapshots_;
	std::atomic_bool running_{false};
	std::thread worker_;
	struct ManeuverCommand {
		agame::sim::ManeuverKind kind = agame::sim::ManeuverKind::None;
		double amount = 0.0;
	};
	std::mutex commandMutex_;
	std::deque<ManeuverCommand> maneuverCommands_;

	agame::sim::NavigationStateBundle navigation_;
	bool navigationInitialized_ = false;
	std::uint64_t consumedVersion_ = 0;
	std::uint64_t consumedManeuverCount_ = 0;
	double interpolationElapsedS_ = 0.0;
	double estimatedMassScale_ = 0.88;
	std::chrono::steady_clock::time_point latestArrival_;
	std::shared_ptr<const agame::sim::SimulationSnapshot> latestSnapshot_;
};
