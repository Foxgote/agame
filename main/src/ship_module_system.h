#pragma once

#include <godot_cpp/classes/node.hpp>
#include <godot_cpp/variant/dictionary.hpp>

#include <array>

class ShipModuleSystem : public godot::Node {
	GDCLASS(ShipModuleSystem, godot::Node);

public:
	ShipModuleSystem();

	void applyTidalStress(double pSeverityPerS2);
	void applyModuleDamage(const godot::StringName &pModule, double pAmount, const godot::String &pCause);
	double getModuleIntegrity(const godot::StringName &pModule) const;
	godot::Dictionary getModuleStatus() const;
	void setArtificialGravityEnabled(bool pEnabled);
	bool isArtificialGravityEnabled() const { return artificialGravityEnabled_; }
	void setShieldPower(double pNormalized);
	double getShieldPower() const { return shieldPower_; }
	double getFood() const { return food_; }
	double getCapacitorPower() const { return capacitorPower_; }
	void applyNarrativeChoice(int pChoice);

protected:
	static void _bind_methods();

private:
	struct ModuleEntry {
		godot::StringName name;
		double integrity = 1.0;
	};

	ModuleEntry *findModule(const godot::StringName &pModule);
	const ModuleEntry *findModule(const godot::StringName &pModule) const;

	std::array<ModuleEntry, 11> modules_;
	bool artificialGravityEnabled_ = true;
	double shieldPower_ = 0.65;
	double food_ = 0.72;
	double capacitorPower_ = 0.84;
	double tidalThresholdPerS2_ = 8.0e-8;
	double warningCooldown_ = 0.0;
};
