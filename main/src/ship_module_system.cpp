#include "ship_module_system.h"

#include <godot_cpp/core/class_db.hpp>

#include <algorithm>

using namespace godot;

ShipModuleSystem::ShipModuleSystem() {
	modules_ = {{
		{StringName("Reactor"), 1.0},
		{StringName("Power Bus"), 1.0},
		{StringName("Propulsion"), 1.0},
		{StringName("Attitude Control"), 1.0},
		{StringName("Vacuum Shield"), 1.0},
		{StringName("Artificial Gravity"), 1.0},
		{StringName("Navigation Computer"), 1.0},
		{StringName("Sensor Array"), 1.0},
		{StringName("Communications"), 1.0},
		{StringName("Life Support"), 1.0},
		{StringName("Hydroponics"), 1.0},
	}};
}

void ShipModuleSystem::_bind_methods() {
	ClassDB::bind_method(D_METHOD("applyTidalStress", "severityPerS2"), &ShipModuleSystem::applyTidalStress);
	ClassDB::bind_method(D_METHOD("applyModuleDamage", "module", "amount", "cause"), &ShipModuleSystem::applyModuleDamage);
	ClassDB::bind_method(D_METHOD("getModuleIntegrity", "module"), &ShipModuleSystem::getModuleIntegrity);
	ClassDB::bind_method(D_METHOD("getModuleStatus"), &ShipModuleSystem::getModuleStatus);
	ClassDB::bind_method(D_METHOD("setArtificialGravityEnabled", "enabled"), &ShipModuleSystem::setArtificialGravityEnabled);
	ClassDB::bind_method(D_METHOD("isArtificialGravityEnabled"), &ShipModuleSystem::isArtificialGravityEnabled);
	ClassDB::bind_method(D_METHOD("setShieldPower", "normalized"), &ShipModuleSystem::setShieldPower);
	ClassDB::bind_method(D_METHOD("getShieldPower"), &ShipModuleSystem::getShieldPower);
	ClassDB::bind_method(D_METHOD("getFood"), &ShipModuleSystem::getFood);
	ClassDB::bind_method(D_METHOD("getCapacitorPower"), &ShipModuleSystem::getCapacitorPower);
	ClassDB::bind_method(D_METHOD("applyNarrativeChoice", "choice"), &ShipModuleSystem::applyNarrativeChoice);
	ADD_PROPERTY(PropertyInfo(Variant::BOOL, "artificialGravityEnabled"), "setArtificialGravityEnabled", "isArtificialGravityEnabled");
	ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "shieldPower", PROPERTY_HINT_RANGE, "0,1,0.01"), "setShieldPower", "getShieldPower");
	ADD_SIGNAL(MethodInfo("moduleDamaged", PropertyInfo(Variant::STRING_NAME, "module"), PropertyInfo(Variant::FLOAT, "integrity"), PropertyInfo(Variant::STRING, "cause")));
	ADD_SIGNAL(MethodInfo("warning", PropertyInfo(Variant::STRING, "message"), PropertyInfo(Variant::FLOAT, "severity")));
	ADD_SIGNAL(MethodInfo("cameraFeedback", PropertyInfo(Variant::FLOAT, "trauma")));
	ADD_SIGNAL(MethodInfo("audioFeedback", PropertyInfo(Variant::STRING_NAME, "cue")));
	ADD_SIGNAL(MethodInfo("gravityChanged", PropertyInfo(Variant::BOOL, "enabled")));
	ADD_SIGNAL(MethodInfo("resourcesChanged", PropertyInfo(Variant::FLOAT, "food"), PropertyInfo(Variant::FLOAT, "capacitorPower")));
}

ShipModuleSystem::ModuleEntry *ShipModuleSystem::findModule(const StringName &pModule) {
	for (ModuleEntry &module : modules_) if (module.name == pModule) return &module;
	return nullptr;
}

const ShipModuleSystem::ModuleEntry *ShipModuleSystem::findModule(const StringName &pModule) const {
	for (const ModuleEntry &module : modules_) if (module.name == pModule) return &module;
	return nullptr;
}

void ShipModuleSystem::applyTidalStress(double pSeverityPerS2) {
	if (pSeverityPerS2 <= tidalThresholdPerS2_) return;
	const double shieldMitigation = 0.25 + shieldPower_ * 0.65;
	const double overload = pSeverityPerS2 / tidalThresholdPerS2_ - 1.0;
	const double damage = std::clamp(overload * (1.0 - shieldMitigation) * 0.003, 0.0, 0.012);
	applyModuleDamage(StringName("Vacuum Shield"), damage, "Tidal gradient overload");
	if (getModuleIntegrity(StringName("Vacuum Shield")) < 0.82) {
		applyModuleDamage(StringName("Power Bus"), damage * 0.35, "Shield bus oscillation");
	}
}

void ShipModuleSystem::applyModuleDamage(const StringName &pModule, double pAmount, const String &pCause) {
	ModuleEntry *module = findModule(pModule);
	if (!module || pAmount <= 0.0) return;
	const double before = module->integrity;
	module->integrity = std::clamp(module->integrity - pAmount, 0.0, 1.0);
	if (module->integrity == before) return;
	emit_signal("moduleDamaged", module->name, module->integrity, pCause);
	emit_signal("cameraFeedback", std::clamp(pAmount * 30.0, 0.04, 0.38));
	if (module->integrity < 0.75 && before >= 0.75) {
		emit_signal("warning", String(module->name) + " integrity below 75%", 0.65);
		emit_signal("audioFeedback", StringName("moduleWarning"));
	}
	if (module->integrity < 0.4 && before >= 0.4) {
		emit_signal("warning", String(module->name) + " CRITICAL", 1.0);
		emit_signal("audioFeedback", StringName("criticalAlarm"));
	}
}

double ShipModuleSystem::getModuleIntegrity(const StringName &pModule) const { const ModuleEntry *module = findModule(pModule); return module ? module->integrity : 0.0; }

Dictionary ShipModuleSystem::getModuleStatus() const {
	Dictionary result;
	for (const ModuleEntry &module : modules_) result[module.name] = module.integrity;
	return result;
}

void ShipModuleSystem::setArtificialGravityEnabled(bool pEnabled) {
	if (artificialGravityEnabled_ == pEnabled) return;
	artificialGravityEnabled_ = pEnabled;
	emit_signal("gravityChanged", pEnabled);
	emit_signal("audioFeedback", pEnabled ? StringName("gravityRestore") : StringName("gravityFailure"));
	if (!pEnabled) emit_signal("warning", "ARTIFICIAL GRAVITY OFFLINE", 0.8);
}

void ShipModuleSystem::setShieldPower(double pNormalized) {
	shieldPower_ = std::clamp(pNormalized, 0.0, 1.0);
	emit_signal("resourcesChanged", food_, capacitorPower_);
}

void ShipModuleSystem::applyNarrativeChoice(int pChoice) {
	if (pChoice == 0) {
		food_ = std::max(0.0, food_ - 0.10);
		applyModuleDamage(StringName("Hydroponics"), 0.04, "Emergency cultivation draw");
	} else {
		capacitorPower_ = std::max(0.0, capacitorPower_ - 0.16);
		applyModuleDamage(StringName("Sensor Array"), 0.03, "Unfiltered beacon sweep");
	}
	emit_signal("resourcesChanged", food_, capacitorPower_);
}
