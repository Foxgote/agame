#include "interactable_3d.h"

#include <godot_cpp/classes/box_mesh.hpp>
#include <godot_cpp/classes/box_shape3d.hpp>
#include <godot_cpp/classes/standard_material3d.hpp>
#include <godot_cpp/core/class_db.hpp>

#include <algorithm>

using namespace godot;

Interactable3D::Interactable3D() {
	setOptions({InteractionOption{StringName("inspect"), "Inspect", false, 0.45, false, true, StringName("inspect"), StringName("inspectSoft")}});
}

void Interactable3D::_bind_methods() {
	ClassDB::bind_method(D_METHOD("getAvailableActions"), &Interactable3D::getAvailableActions);
	ClassDB::bind_method(D_METHOD("supportsAction", "action"), &Interactable3D::supportsAction);
	ClassDB::bind_method(D_METHOD("getPrimaryAction"), &Interactable3D::getPrimaryAction);
	ClassDB::bind_method(D_METHOD("getInteractionAnchor"), &Interactable3D::getInteractionAnchor);
	ClassDB::bind_method(D_METHOD("performAction", "action", "amount"), &Interactable3D::performAction, DEFVAL(1.0));
	ClassDB::bind_method(D_METHOD("reserve", "actorId"), &Interactable3D::reserve);
	ClassDB::bind_method(D_METHOD("release", "actorId"), &Interactable3D::release);
	ClassDB::bind_method(D_METHOD("isReserved"), &Interactable3D::isReserved);
	ClassDB::bind_method(D_METHOD("getReservationOwner"), &Interactable3D::getReservationOwner);
	ClassDB::bind_method(D_METHOD("beginInteraction", "action", "actorId"), &Interactable3D::beginInteraction);
	ClassDB::bind_method(D_METHOD("completeInteraction", "action", "actorId"), &Interactable3D::completeInteraction);
	ClassDB::bind_method(D_METHOD("getActionDuration", "action"), &Interactable3D::getActionDuration);
	ClassDB::bind_method(D_METHOD("actionRequiresAlignment", "action"), &Interactable3D::actionRequiresAlignment);
	ClassDB::bind_method(D_METHOD("actionIsInterruptible", "action"), &Interactable3D::actionIsInterruptible);
	ClassDB::bind_method(D_METHOD("setDisplayName", "name"), &Interactable3D::setDisplayName);
	ClassDB::bind_method(D_METHOD("getDisplayName"), &Interactable3D::getDisplayName);
	ClassDB::bind_method(D_METHOD("getPrompt"), &Interactable3D::getPrompt);
	ADD_PROPERTY(PropertyInfo(Variant::STRING, "displayName"), "setDisplayName", "getDisplayName");
	ADD_SIGNAL(MethodInfo("interactionStarted", PropertyInfo(Variant::STRING_NAME, "action"), PropertyInfo(Variant::INT, "actorId")));
	ADD_SIGNAL(MethodInfo("activated", PropertyInfo(Variant::STRING_NAME, "action"), PropertyInfo(Variant::FLOAT, "value")));
	ADD_SIGNAL(MethodInfo("interactionCompleted", PropertyInfo(Variant::STRING_NAME, "action"), PropertyInfo(Variant::INT, "actorId")));
	ADD_SIGNAL(MethodInfo("valueChanged", PropertyInfo(Variant::FLOAT, "normalizedValue"), PropertyInfo(Variant::FLOAT, "gameplayValue")));
	ADD_SIGNAL(MethodInfo("feedbackRequested", PropertyInfo(Variant::STRING_NAME, "kind"), PropertyInfo(Variant::FLOAT, "strength")));
}

void Interactable3D::_ready() {
	if (!visual_) {
		createBoxGeometry(Vector3(0.35f, 0.35f, 0.35f), Color(0.35f, 0.42f, 0.45f));
	}
}

void Interactable3D::setOptions(std::vector<InteractionOption> pOptions) { options_ = std::move(pOptions); }

void Interactable3D::createBoxGeometry(const Vector3 &pSize, const Color &pColor) {
	visual_ = memnew(MeshInstance3D);
	visual_->set_name("Visual");
	Ref<BoxMesh> mesh;
	mesh.instantiate();
	mesh->set_size(pSize);
	Ref<StandardMaterial3D> material;
	material.instantiate();
	material->set_albedo(pColor);
	material->set_metallic(0.35f);
	material->set_roughness(0.62f);
	mesh->set_material(material);
	visual_->set_mesh(mesh);
	add_child(visual_);

	collision_ = memnew(CollisionShape3D);
	collision_->set_name("Collision");
	Ref<BoxShape3D> shape;
	shape.instantiate();
	shape->set_size(pSize);
	collision_->set_shape(shape);
	add_child(collision_);
}

const InteractionOption *Interactable3D::findOption(const StringName &pAction) const {
	for (const InteractionOption &option : options_) {
		if (option.action == pAction) {
			return &option;
		}
	}
	return nullptr;
}

PackedStringArray Interactable3D::getAvailableActions() const {
	PackedStringArray actions;
	for (const InteractionOption &option : options_) {
		actions.push_back(String(option.action));
	}
	return actions;
}

bool Interactable3D::supportsAction(const StringName &pAction) const { return findOption(pAction) != nullptr; }
StringName Interactable3D::getPrimaryAction() const { return options_.empty() ? StringName("interact") : options_.front().action; }
Vector3 Interactable3D::getInteractionAnchor() const { return get_global_position(); }
bool Interactable3D::performAction(const StringName &pAction, double) { return supportsAction(pAction); }

bool Interactable3D::reserve(std::int64_t pActorId) {
	if (pActorId == 0 || (reservationOwner_ != 0 && reservationOwner_ != pActorId)) {
		return false;
	}
	reservationOwner_ = pActorId;
	return true;
}

void Interactable3D::release(std::int64_t pActorId) {
	if (reservationOwner_ == pActorId) {
		reservationOwner_ = 0;
	}
}

void Interactable3D::beginInteraction(const StringName &pAction, std::int64_t pActorId) { emit_signal("interactionStarted", pAction, pActorId); }
void Interactable3D::completeInteraction(const StringName &pAction, std::int64_t pActorId) { emit_signal("interactionCompleted", pAction, pActorId); }
double Interactable3D::getActionDuration(const StringName &pAction) const { const auto *option = findOption(pAction); return option ? option->durationS : 0.35; }
bool Interactable3D::actionRequiresAlignment(const StringName &pAction) const { const auto *option = findOption(pAction); return option && option->alignmentRequired; }
bool Interactable3D::actionIsInterruptible(const StringName &pAction) const { const auto *option = findOption(pAction); return !option || option->interruptible; }
String Interactable3D::getPrompt() const { return options_.empty() ? displayName_ : String(options_.front().displayName) + " " + displayName_; }

PushButton3D::PushButton3D() {
	setDisplayName("gravity breaker");
	setOptions({InteractionOption{StringName("press"), "Press", false, 0.3, false, true, StringName("press"), StringName("buttonClick")}});
}

void PushButton3D::_bind_methods() {
	ClassDB::bind_method(D_METHOD("setToggleMode", "enabled"), &PushButton3D::setToggleMode);
	ClassDB::bind_method(D_METHOD("isToggleMode"), &PushButton3D::isToggleMode);
	ClassDB::bind_method(D_METHOD("setPressed", "pressed"), &PushButton3D::setPressed);
	ClassDB::bind_method(D_METHOD("isPressed"), &PushButton3D::isPressed);
	ClassDB::bind_method(D_METHOD("setTravelDistance", "distance"), &PushButton3D::setTravelDistance);
	ClassDB::bind_method(D_METHOD("getTravelDistance"), &PushButton3D::getTravelDistance);
	ClassDB::bind_method(D_METHOD("setResetTime", "timeS"), &PushButton3D::setResetTime);
	ClassDB::bind_method(D_METHOD("getResetTime"), &PushButton3D::getResetTime);
	ADD_PROPERTY(PropertyInfo(Variant::BOOL, "toggleMode"), "setToggleMode", "isToggleMode");
	ADD_PROPERTY(PropertyInfo(Variant::BOOL, "pressed"), "setPressed", "isPressed");
	ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "travelDistance", PROPERTY_HINT_RANGE, "0.01,0.2,0.01"), "setTravelDistance", "getTravelDistance");
	ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "resetTimeS", PROPERTY_HINT_RANGE, "0.05,2.0,0.05"), "setResetTime", "getResetTime");
}

void PushButton3D::_ready() { createBoxGeometry(Vector3(0.42f, 0.16f, 0.42f), Color(0.75f, 0.12f, 0.08f)); set_process(false); }
void PushButton3D::setPressed(bool pPressed) { pressed_ = pPressed; if (visual_) visual_->set_position(Vector3(0.0f, pressed_ ? static_cast<float>(-travelDistance_) : 0.0f, 0.0f)); }
bool PushButton3D::performAction(const StringName &pAction, double) {
	if (!supportsAction(pAction)) return false;
	setPressed(toggleMode_ ? !pressed_ : true);
	resetRemainingS_ = resetTimeS_;
	set_process(!toggleMode_ && pressed_);
	emit_signal("activated", pAction, pressed_ ? 1.0 : 0.0);
	emit_signal("feedbackRequested", StringName("buttonTactile"), 0.18);
	return true;
}
void PushButton3D::_process(double pDelta) { resetRemainingS_ -= pDelta; if (resetRemainingS_ <= 0.0) { setPressed(false); set_process(false); emit_signal("activated", StringName("released"), 0.0); } }

RotaryKnob3D::RotaryKnob3D() {
	setDisplayName("sensor mass estimate");
	setOptions({InteractionOption{StringName("turn"), "Turn", false, 0.4, false, true, StringName("turn"), StringName("knobDetent")}, InteractionOption{StringName("adjust"), "Adjust", true, 0.4, false, true, StringName("adjust"), StringName("knobDetent")}});
}
void RotaryKnob3D::_bind_methods() {
	ClassDB::bind_method(D_METHOD("setNormalizedValue", "value"), &RotaryKnob3D::setNormalizedValue);
	ClassDB::bind_method(D_METHOD("getNormalizedValue"), &RotaryKnob3D::getNormalizedValue);
	ClassDB::bind_method(D_METHOD("setAngleRange", "degrees"), &RotaryKnob3D::setAngleRange);
	ClassDB::bind_method(D_METHOD("getAngleRange"), &RotaryKnob3D::getAngleRange);
	ClassDB::bind_method(D_METHOD("setGameplayMin", "value"), &RotaryKnob3D::setGameplayMin);
	ClassDB::bind_method(D_METHOD("getGameplayMin"), &RotaryKnob3D::getGameplayMin);
	ClassDB::bind_method(D_METHOD("setGameplayMax", "value"), &RotaryKnob3D::setGameplayMax);
	ClassDB::bind_method(D_METHOD("getGameplayMax"), &RotaryKnob3D::getGameplayMax);
	ClassDB::bind_method(D_METHOD("setSensitivity", "value"), &RotaryKnob3D::setSensitivity);
	ClassDB::bind_method(D_METHOD("getSensitivity"), &RotaryKnob3D::getSensitivity);
	ClassDB::bind_method(D_METHOD("setInverted", "enabled"), &RotaryKnob3D::setInverted);
	ClassDB::bind_method(D_METHOD("isInverted"), &RotaryKnob3D::isInverted);
	ClassDB::bind_method(D_METHOD("setDetents", "count"), &RotaryKnob3D::setDetents);
	ClassDB::bind_method(D_METHOD("getDetents"), &RotaryKnob3D::getDetents);
	ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "normalizedValue", PROPERTY_HINT_RANGE, "0,1,0.01"), "setNormalizedValue", "getNormalizedValue");
	ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "angleRangeDegrees"), "setAngleRange", "getAngleRange");
	ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "gameplayMin"), "setGameplayMin", "getGameplayMin");
	ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "gameplayMax"), "setGameplayMax", "getGameplayMax");
	ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "sensitivity"), "setSensitivity", "getSensitivity");
	ADD_PROPERTY(PropertyInfo(Variant::BOOL, "inverted"), "setInverted", "isInverted");
	ADD_PROPERTY(PropertyInfo(Variant::INT, "detents", PROPERTY_HINT_RANGE, "0,64,1"), "setDetents", "getDetents");
}
void RotaryKnob3D::_ready() { createBoxGeometry(Vector3(0.34f, 0.22f, 0.34f), Color(0.78f, 0.65f, 0.16f)); setNormalizedValue(normalizedValue_); }
void RotaryKnob3D::setNormalizedValue(double pValue) {
	normalizedValue_ = std::clamp(pValue, 0.0, 1.0);
	if (detents_ > 1) normalizedValue_ = std::round(normalizedValue_ * (detents_ - 1)) / static_cast<double>(detents_ - 1);
	if (visual_) visual_->set_rotation(Vector3(0.0f, static_cast<float>((normalizedValue_ - 0.5) * Math_TAU * angleRangeDegrees_ / 360.0), 0.0f));
	const double gameplayValue = gameplayMin_ + (gameplayMax_ - gameplayMin_) * normalizedValue_;
	emit_signal("valueChanged", normalizedValue_, gameplayValue);
}
bool RotaryKnob3D::performAction(const StringName &pAction, double pAmount) { if (!supportsAction(pAction)) return false; setNormalizedValue(normalizedValue_ + pAmount * sensitivity_ * (inverted_ ? -1.0 : 1.0)); emit_signal("activated", pAction, normalizedValue_); emit_signal("feedbackRequested", StringName("knobDetent"), 0.08); return true; }

LinearSlider3D::LinearSlider3D() {
	setDisplayName("shield power slider");
	setOptions({InteractionOption{StringName("adjust"), "Adjust", true, 0.45, false, true, StringName("slide"), StringName("sliderMove")}});
}
void LinearSlider3D::_bind_methods() {
	ClassDB::bind_method(D_METHOD("setNormalizedValue", "value"), &LinearSlider3D::setNormalizedValue);
	ClassDB::bind_method(D_METHOD("getNormalizedValue"), &LinearSlider3D::getNormalizedValue);
	ClassDB::bind_method(D_METHOD("setTravelDistance", "distance"), &LinearSlider3D::setTravelDistance);
	ClassDB::bind_method(D_METHOD("getTravelDistance"), &LinearSlider3D::getTravelDistance);
	ClassDB::bind_method(D_METHOD("setSensitivity", "value"), &LinearSlider3D::setSensitivity);
	ClassDB::bind_method(D_METHOD("getSensitivity"), &LinearSlider3D::getSensitivity);
	ClassDB::bind_method(D_METHOD("setInverted", "enabled"), &LinearSlider3D::setInverted);
	ClassDB::bind_method(D_METHOD("isInverted"), &LinearSlider3D::isInverted);
	ClassDB::bind_method(D_METHOD("setSteps", "count"), &LinearSlider3D::setSteps);
	ClassDB::bind_method(D_METHOD("getSteps"), &LinearSlider3D::getSteps);
	ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "normalizedValue", PROPERTY_HINT_RANGE, "0,1,0.01"), "setNormalizedValue", "getNormalizedValue");
	ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "travelDistance"), "setTravelDistance", "getTravelDistance");
	ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "sensitivity"), "setSensitivity", "getSensitivity");
	ADD_PROPERTY(PropertyInfo(Variant::BOOL, "inverted"), "setInverted", "isInverted");
	ADD_PROPERTY(PropertyInfo(Variant::INT, "steps", PROPERTY_HINT_RANGE, "0,64,1"), "setSteps", "getSteps");
}
void LinearSlider3D::_ready() { createBoxGeometry(Vector3(0.22f, 0.16f, 0.32f), Color(0.22f, 0.72f, 0.68f)); setNormalizedValue(normalizedValue_); }
void LinearSlider3D::setNormalizedValue(double pValue) { normalizedValue_ = std::clamp(pValue, 0.0, 1.0); if (steps_ > 1) normalizedValue_ = std::round(normalizedValue_ * (steps_ - 1)) / static_cast<double>(steps_ - 1); if (visual_) visual_->set_position(Vector3(static_cast<float>((normalizedValue_ - 0.5) * travelDistance_), 0.0f, 0.0f)); emit_signal("valueChanged", normalizedValue_, normalizedValue_); }
bool LinearSlider3D::performAction(const StringName &pAction, double pAmount) { if (!supportsAction(pAction)) return false; setNormalizedValue(normalizedValue_ + pAmount * sensitivity_ * (inverted_ ? -1.0 : 1.0)); emit_signal("activated", pAction, normalizedValue_); emit_signal("feedbackRequested", StringName("sliderMove"), 0.06); return true; }

Seat3D::Seat3D() { setDisplayName("navigation chair"); setOptions({InteractionOption{StringName("sit"), "Sit", false, 0.7, true, true, StringName("sit"), StringName("seatCreak")}, InteractionOption{StringName("stand"), "Stand", false, 0.45, true, true, StringName("stand"), StringName("seatCreak")}}); }
void Seat3D::_bind_methods() { ClassDB::bind_method(D_METHOD("isOccupied"), &Seat3D::isOccupied); ADD_SIGNAL(MethodInfo("occupancyChanged", PropertyInfo(Variant::BOOL, "occupied"))); }
void Seat3D::_ready() { createBoxGeometry(Vector3(0.85f, 0.9f, 0.85f), Color(0.21f, 0.27f, 0.31f)); }
Vector3 Seat3D::getInteractionAnchor() const { return get_global_position() + Vector3(0.0f, 0.95f, 0.12f); }
bool Seat3D::performAction(const StringName &pAction, double) { if (pAction == StringName("sit")) occupied_ = true; else if (pAction == StringName("stand")) occupied_ = false; else return false; emit_signal("activated", pAction, occupied_ ? 1.0 : 0.0); emit_signal("occupancyChanged", occupied_); return true; }

Inspectable3D::Inspectable3D() { setDisplayName("maintenance placard"); setOptions({InteractionOption{StringName("inspect"), "Inspect", false, 0.55, false, true, StringName("inspect"), StringName("inspectSoft")}}); }
void Inspectable3D::_bind_methods() { ClassDB::bind_method(D_METHOD("setInspectionText", "text"), &Inspectable3D::setInspectionText); ClassDB::bind_method(D_METHOD("getInspectionText"), &Inspectable3D::getInspectionText); ADD_PROPERTY(PropertyInfo(Variant::STRING, "inspectionText", PROPERTY_HINT_MULTILINE_TEXT), "setInspectionText", "getInspectionText"); ADD_SIGNAL(MethodInfo("inspected", PropertyInfo(Variant::STRING, "text"))); }
void Inspectable3D::_ready() { createBoxGeometry(Vector3(0.9f, 0.55f, 0.08f), Color(0.63f, 0.58f, 0.44f)); }
bool Inspectable3D::performAction(const StringName &pAction, double) { if (!supportsAction(pAction)) return false; emit_signal("activated", pAction, 1.0); emit_signal("inspected", inspectionText_); return true; }

EdibleItem3D::EdibleItem3D() { setDisplayName("sealed peach ration"); setOptions({InteractionOption{StringName("eat"), "Eat", false, 0.9, false, true, StringName("eat"), StringName("rationOpen")}, InteractionOption{StringName("inspect"), "Inspect", false, 0.4, false, true, StringName("inspect"), StringName("inspectSoft")}}); }
void EdibleItem3D::_bind_methods() { ClassDB::bind_method(D_METHOD("isConsumed"), &EdibleItem3D::isConsumed); ClassDB::bind_method(D_METHOD("getNutrition"), &EdibleItem3D::getNutrition); ClassDB::bind_method(D_METHOD("setNutrition", "nutrition"), &EdibleItem3D::setNutrition); ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "nutrition", PROPERTY_HINT_RANGE, "0,1,0.01"), "setNutrition", "getNutrition"); ADD_SIGNAL(MethodInfo("consumed", PropertyInfo(Variant::FLOAT, "nutrition"))); }
void EdibleItem3D::_ready() { createBoxGeometry(Vector3(0.26f, 0.18f, 0.42f), Color(0.83f, 0.44f, 0.20f)); }
bool EdibleItem3D::performAction(const StringName &pAction, double) { if (pAction == StringName("inspect")) { emit_signal("activated", pAction, 1.0); return true; } if (pAction != StringName("eat") || consumed_) return false; consumed_ = true; if (visual_) visual_->set_visible(false); if (collision_) collision_->set_disabled(true); emit_signal("activated", pAction, nutrition_); emit_signal("consumed", nutrition_); return true; }
