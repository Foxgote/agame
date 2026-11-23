#include "vertical_slice.h"

#include <godot_cpp/classes/audio_stream_wav.hpp>
#include <godot_cpp/classes/box_mesh.hpp>
#include <godot_cpp/classes/box_shape3d.hpp>
#include <godot_cpp/classes/button.hpp>
#include <godot_cpp/classes/canvas_layer.hpp>
#include <godot_cpp/classes/collision_shape3d.hpp>
#include <godot_cpp/classes/directional_light3d.hpp>
#include <godot_cpp/classes/environment.hpp>
#include <godot_cpp/classes/global_constants.hpp>
#include <godot_cpp/classes/input_event_key.hpp>
#include <godot_cpp/classes/input.hpp>
#include <godot_cpp/classes/input_map.hpp>
#include <godot_cpp/classes/label3d.hpp>
#include <godot_cpp/classes/margin_container.hpp>
#include <godot_cpp/classes/mesh_instance3d.hpp>
#include <godot_cpp/classes/omni_light3d.hpp>
#include <godot_cpp/classes/panel_container.hpp>
#include <godot_cpp/classes/standard_material3d.hpp>
#include <godot_cpp/classes/static_body3d.hpp>
#include <godot_cpp/classes/style_box_flat.hpp>
#include <godot_cpp/classes/v_box_container.hpp>
#include <godot_cpp/classes/world_environment.hpp>
#include <godot_cpp/core/class_db.hpp>

#include <algorithm>
#include <cmath>

using namespace godot;

namespace {

StaticBody3D *createStaticBox(Node3D *pParent, const String &pName, const Vector3 &pPosition, const Vector3 &pSize, const Color &pColor) {
	StaticBody3D *body = memnew(StaticBody3D);
	body->set_name(pName);
	body->set_position(pPosition);
	pParent->add_child(body);

	MeshInstance3D *visual = memnew(MeshInstance3D);
	Ref<BoxMesh> mesh;
	mesh.instantiate();
	mesh->set_size(pSize);
	Ref<StandardMaterial3D> material;
	material.instantiate();
	material->set_albedo(pColor);
	material->set_metallic(0.22f);
	material->set_roughness(0.78f);
	mesh->set_material(material);
	visual->set_mesh(mesh);
	body->add_child(visual);

	CollisionShape3D *collision = memnew(CollisionShape3D);
	Ref<BoxShape3D> shape;
	shape.instantiate();
	shape->set_size(pSize);
	collision->set_shape(shape);
	body->add_child(collision);
	return body;
}

Label3D *createEquipmentLabel(Node3D *pParent, const String &pText, const Vector3 &pPosition) {
	Label3D *label = memnew(Label3D);
	label->set_text(pText);
	label->set_position(pPosition);
	label->set_font_size(24);
	label->set_modulate(Color(0.72f, 0.86f, 0.82f));
	label->set_outline_size(4);
	pParent->add_child(label);
	return label;
}

void addKeyAction(const StringName &pAction, Key pKey) {
	InputMap *map = InputMap::get_singleton();
	if (!map->has_action(pAction)) map->add_action(pAction);
	Ref<InputEventKey> key;
	key.instantiate();
	key->set_physical_keycode(pKey);
	map->action_add_event(pAction, key);
}

Ref<StyleBoxFlat> makePanelStyle(const Color &pColor, const Color &pBorder) {
	Ref<StyleBoxFlat> style;
	style.instantiate();
	style->set_bg_color(pColor);
	style->set_border_color(pBorder);
	style->set_border_width_all(1);
	style->set_corner_radius_all(4);
	style->set_content_margin_all(14.0f);
	return style;
}

} // namespace

void VerticalSlice::_bind_methods() {
	ClassDB::bind_method(D_METHOD("onPlayerActionCompleted", "action", "target"), &VerticalSlice::onPlayerActionCompleted);
	ClassDB::bind_method(D_METHOD("onGravityButton", "action", "value"), &VerticalSlice::onGravityButton);
	ClassDB::bind_method(D_METHOD("onKnobChanged", "normalized", "gameplayValue"), &VerticalSlice::onKnobChanged);
	ClassDB::bind_method(D_METHOD("onSliderChanged", "normalized", "gameplayValue"), &VerticalSlice::onSliderChanged);
	ClassDB::bind_method(D_METHOD("onBurnSettingChanged", "normalized", "gameplayValue"), &VerticalSlice::onBurnSettingChanged);
	ClassDB::bind_method(D_METHOD("onDumpSettingChanged", "normalized", "gameplayValue"), &VerticalSlice::onDumpSettingChanged);
	ClassDB::bind_method(D_METHOD("onBurnButton", "action", "value"), &VerticalSlice::onBurnButton);
	ClassDB::bind_method(D_METHOD("onDumpButton", "action", "value"), &VerticalSlice::onDumpButton);
	ClassDB::bind_method(D_METHOD("onManeuverExecuted", "kind", "deltaVMps", "expendedMassKg", "energyGainJPerKg"), &VerticalSlice::onManeuverExecuted);
	ClassDB::bind_method(D_METHOD("onInspected", "text"), &VerticalSlice::onInspected);
	ClassDB::bind_method(D_METHOD("onWarning", "message", "severity"), &VerticalSlice::onWarning);
	ClassDB::bind_method(D_METHOD("onAudioFeedback", "cue"), &VerticalSlice::onAudioFeedback);
	ClassDB::bind_method(D_METHOD("onInteractionFeedback", "kind", "strength"), &VerticalSlice::onInteractionFeedback);
	ClassDB::bind_method(D_METHOD("onEventChoice", "choice"), &VerticalSlice::onEventChoice);
}

void VerticalSlice::_ready() {
	configureInput();
	buildShipInterior();
	buildSystemsAndPlayer();
	buildInteractables();
	buildHud();
	tutorialStartPosition_ = player_->get_global_position();
	fadeDirection_ = -1;
	set_process(true);
}

void VerticalSlice::configureInput() {
	addKeyAction("moveForward", KEY_W);
	addKeyAction("moveBack", KEY_S);
	addKeyAction("moveLeft", KEY_A);
	addKeyAction("moveRight", KEY_D);
	addKeyAction("moveUp", KEY_SPACE);
	addKeyAction("moveDown", KEY_CTRL);
	addKeyAction("interact", KEY_E);
	addKeyAction("cancelAction", KEY_Q);
	addKeyAction("toggleMouse", KEY_ESCAPE);
}

void VerticalSlice::buildShipInterior() {
	WorldEnvironment *worldEnvironment = memnew(WorldEnvironment);
	Ref<Environment> environment;
	environment.instantiate();
	environment->set_background(Environment::BG_COLOR);
	environment->set_bg_color(Color(0.012f, 0.018f, 0.021f));
	environment->set_ambient_source(Environment::AMBIENT_SOURCE_COLOR);
	environment->set_ambient_light_color(Color(0.44f, 0.57f, 0.58f));
	environment->set_ambient_light_energy(0.72f);
	worldEnvironment->set_environment(environment);
	add_child(worldEnvironment);

	const Color hull(0.11f, 0.14f, 0.15f);
	const Color bulkhead(0.16f, 0.20f, 0.20f);
	createStaticBox(this, "Deck", Vector3(0.0f, -0.15f, 0.0f), Vector3(11.5f, 0.3f, 15.0f), Color(0.13f, 0.16f, 0.16f));
	createStaticBox(this, "Ceiling", Vector3(0.0f, 3.65f, 0.0f), Vector3(11.5f, 0.25f, 15.0f), hull);
	createStaticBox(this, "PortBulkhead", Vector3(-5.65f, 1.75f, 0.0f), Vector3(0.25f, 3.8f, 15.0f), bulkhead);
	createStaticBox(this, "StarboardBulkhead", Vector3(5.65f, 1.75f, 0.0f), Vector3(0.25f, 3.8f, 15.0f), bulkhead);
	createStaticBox(this, "ForwardBulkhead", Vector3(0.0f, 1.75f, -7.4f), Vector3(11.5f, 3.8f, 0.25f), bulkhead);
	createStaticBox(this, "AftBulkhead", Vector3(0.0f, 1.75f, 7.4f), Vector3(11.5f, 3.8f, 0.25f), bulkhead);
	createStaticBox(this, "NavigationConsole", Vector3(0.0f, 0.55f, -5.4f), Vector3(9.2f, 1.1f, 1.1f), Color(0.20f, 0.24f, 0.24f));
	createStaticBox(this, "KitchenBench", Vector3(3.8f, 0.48f, 3.4f), Vector3(2.7f, 0.95f, 1.2f), Color(0.22f, 0.23f, 0.21f));
	createStaticBox(this, "CenterRib", Vector3(-3.9f, 1.0f, 1.0f), Vector3(1.2f, 2.0f, 0.7f), Color(0.18f, 0.21f, 0.20f));

	DirectionalLight3D *fill = memnew(DirectionalLight3D);
	fill->set_rotation_degrees(Vector3(-58.0f, -24.0f, 0.0f));
	fill->set_color(Color(0.72f, 0.79f, 0.82f));
	fill->set_param(Light3D::PARAM_ENERGY, 1.05f);
	add_child(fill);
	for (int i = -1; i <= 1; ++i) {
		OmniLight3D *light = memnew(OmniLight3D);
		light->set_position(Vector3(static_cast<float>(i * 3), 3.1f, static_cast<float>(i * 4)));
		light->set_param(Light3D::PARAM_RANGE, 8.5f);
		light->set_param(Light3D::PARAM_ENERGY, 3.2f);
		light->set_color(i == 0 ? Color(0.72f, 0.88f, 0.84f) : Color(0.94f, 0.74f, 0.53f));
		add_child(light);
	}
}

void VerticalSlice::buildSystemsAndPlayer() {
	simulation_ = memnew(SimulationRunner);
	simulation_->set_name("SimulationRunner");
	add_child(simulation_);
	modules_ = memnew(ShipModuleSystem);
	modules_->set_name("ShipModules");
	add_child(modules_);
	player_ = memnew(PlayerController);
	player_->set_name("IzuPlayer");
	player_->set_position(Vector3(0.0f, 0.05f, 5.6f));
	add_child(player_);

	simulation_->connect("tidalStress", Callable(modules_, "applyTidalStress"));
	simulation_->connect("maneuverExecuted", Callable(this, "onManeuverExecuted"));
	modules_->connect("gravityChanged", Callable(player_, "setArtificialGravityEnabled"));
	modules_->connect("cameraFeedback", Callable(player_, "addCameraTrauma"));
	modules_->connect("warning", Callable(this, "onWarning"));
	modules_->connect("audioFeedback", Callable(this, "onAudioFeedback"));
	player_->connect("actionCompleted", Callable(this, "onPlayerActionCompleted"));

	audioPlayer_ = memnew(AudioStreamPlayer);
	audioPlayer_->set_name("GeneratedFeedbackAudio");
	add_child(audioPlayer_);
}

void VerticalSlice::buildInteractables() {
	gravityButton_ = memnew(PushButton3D);
	gravityButton_->set_name("GravityBreaker");
	gravityButton_->setToggleMode(true);
	gravityButton_->set_position(Vector3(-3.8f, 1.25f, -4.75f));
	add_child(gravityButton_);
	gravityButton_->connect("activated", Callable(this, "onGravityButton"));
	gravityButton_->connect("feedbackRequested", Callable(this, "onInteractionFeedback"));
	createEquipmentLabel(this, "GRAVITY", Vector3(-3.8f, 1.7f, -4.72f));

	estimateKnob_ = memnew(RotaryKnob3D);
	estimateKnob_->set_name("MassEstimateKnob");
	estimateKnob_->setGameplayMin(0.5);
	estimateKnob_->setGameplayMax(1.5);
	estimateKnob_->setNormalizedValue(0.38);
	estimateKnob_->set_position(Vector3(-2.55f, 1.25f, -4.75f));
	add_child(estimateKnob_);
	estimateKnob_->connect("valueChanged", Callable(this, "onKnobChanged"));
	estimateKnob_->connect("feedbackRequested", Callable(this, "onInteractionFeedback"));
	createEquipmentLabel(this, "MASS MODEL", Vector3(-2.55f, 1.7f, -4.72f));

	shieldSlider_ = memnew(LinearSlider3D);
	shieldSlider_->set_name("ShieldPowerSlider");
	shieldSlider_->setNormalizedValue(0.65);
	shieldSlider_->set_position(Vector3(-1.25f, 1.25f, -4.75f));
	add_child(shieldSlider_);
	shieldSlider_->connect("valueChanged", Callable(this, "onSliderChanged"));
	shieldSlider_->connect("feedbackRequested", Callable(this, "onInteractionFeedback"));
	createEquipmentLabel(this, "SHIELD BUS", Vector3(-1.25f, 1.7f, -4.72f));

	burnSettingKnob_ = memnew(RotaryKnob3D);
	burnSettingKnob_->set_name("BurnDeltaVKnob");
	burnSettingKnob_->setDisplayName("burn delta-v selector");
	burnSettingKnob_->setGameplayMin(100.0);
	burnSettingKnob_->setGameplayMax(3000.0);
	burnSettingKnob_->setNormalizedValue(0.2);
	burnSettingKnob_->set_position(Vector3(0.25f, 1.25f, -4.75f));
	add_child(burnSettingKnob_);
	burnSettingKnob_->connect("valueChanged", Callable(this, "onBurnSettingChanged"));
	burnSettingKnob_->connect("feedbackRequested", Callable(this, "onInteractionFeedback"));
	createEquipmentLabel(this, "DELTA-V", Vector3(0.25f, 1.7f, -4.72f));

	burnButton_ = memnew(PushButton3D);
	burnButton_->set_name("ExecuteBurnButton");
	burnButton_->setDisplayName("manual prograde burn");
	burnButton_->set_position(Vector3(1.25f, 1.25f, -4.75f));
	add_child(burnButton_);
	burnButton_->connect("activated", Callable(this, "onBurnButton"));
	burnButton_->connect("feedbackRequested", Callable(this, "onInteractionFeedback"));
	createEquipmentLabel(this, "EXECUTE", Vector3(1.25f, 1.7f, -4.72f));

	dumpSettingSlider_ = memnew(LinearSlider3D);
	dumpSettingSlider_->set_name("ReactionMassSlider");
	dumpSettingSlider_->setDisplayName("reaction-mass amount");
	dumpSettingSlider_->setNormalizedValue(0.3);
	dumpSettingSlider_->set_position(Vector3(2.45f, 1.25f, -4.75f));
	add_child(dumpSettingSlider_);
	dumpSettingSlider_->connect("valueChanged", Callable(this, "onDumpSettingChanged"));
	dumpSettingSlider_->connect("feedbackRequested", Callable(this, "onInteractionFeedback"));
	createEquipmentLabel(this, "EJECT MASS", Vector3(2.45f, 1.7f, -4.72f));

	dumpButton_ = memnew(PushButton3D);
	dumpButton_->set_name("ExecuteMassDumpButton");
	dumpButton_->setDisplayName("directed matter dump");
	dumpButton_->set_position(Vector3(3.65f, 1.25f, -4.75f));
	add_child(dumpButton_);
	dumpButton_->connect("activated", Callable(this, "onDumpButton"));
	dumpButton_->connect("feedbackRequested", Callable(this, "onInteractionFeedback"));
	createEquipmentLabel(this, "DUMP", Vector3(3.65f, 1.7f, -4.72f));

	seat_ = memnew(Seat3D);
	seat_->set_name("NavigationSeat");
	seat_->set_position(Vector3(0.0f, 0.45f, -2.75f));
	add_child(seat_);
	seat_->connect("feedbackRequested", Callable(this, "onInteractionFeedback"));

	inspectable_ = memnew(Inspectable3D);
	inspectable_->set_name("MaintenancePlacard");
	inspectable_->setInspectionText("Cirrus-9 navigation bus. Three repair dates are scratched out; the newest handwriting is much smaller.");
	inspectable_->set_position(Vector3(3.0f, 1.45f, -7.15f));
	add_child(inspectable_);
	inspectable_->connect("inspected", Callable(this, "onInspected"));

	ration_ = memnew(EdibleItem3D);
	ration_->set_name("PeachRation");
	ration_->set_position(Vector3(3.8f, 1.08f, 3.25f));
	add_child(ration_);
	createEquipmentLabel(this, "RATION 04", Vector3(3.8f, 1.48f, 3.22f));
}

void VerticalSlice::buildHud() {
	CanvasLayer *canvas = memnew(CanvasLayer);
	canvas->set_name("Interface");
	add_child(canvas);

	PanelContainer *hud = memnew(PanelContainer);
	hud->set_anchors_preset(Control::PRESET_TOP_LEFT);
	hud->set_h_grow_direction(Control::GROW_DIRECTION_END);
	hud->set_v_grow_direction(Control::GROW_DIRECTION_END);
	hud->set_position(Vector2(18.0f, 18.0f));
	hud->set_custom_minimum_size(Vector2(670.0f, 0.0f));
	hud->add_theme_stylebox_override("panel", makePanelStyle(Color(0.025f, 0.04f, 0.045f, 0.92f), Color(0.25f, 0.52f, 0.48f, 0.8f)));
	canvas->add_child(hud);
	VBoxContainer *stack = memnew(VBoxContainer);
	stack->add_theme_constant_override("separation", 7);
	hud->add_child(stack);
	Label *title = memnew(Label);
	title->set_text("CIRRUS-9  /  NAVIGATION MAINTENANCE");
	title->add_theme_color_override("font_color", Color(0.57f, 0.93f, 0.84f));
	title->add_theme_font_size_override("font_size", 16);
	stack->add_child(title);
	tutorialLabel_ = memnew(Label);
	tutorialLabel_->set_text("Move through the compartment.");
	tutorialLabel_->set_autowrap_mode(TextServer::AUTOWRAP_WORD_SMART);
	tutorialLabel_->add_theme_font_size_override("font_size", 18);
	stack->add_child(tutorialLabel_);
	telemetryLabel_ = memnew(Label);
	telemetryLabel_->add_theme_color_override("font_color", Color(0.68f, 0.75f, 0.76f));
	stack->add_child(telemetryLabel_);
	resourcesLabel_ = memnew(Label);
	resourcesLabel_->add_theme_color_override("font_color", Color(0.92f, 0.78f, 0.45f));
	stack->add_child(resourcesLabel_);
	maneuverLabel_ = memnew(Label);
	maneuverLabel_->add_theme_color_override("font_color", Color(0.45f, 0.84f, 0.90f));
	stack->add_child(maneuverLabel_);

	promptLabel_ = memnew(Label);
	promptLabel_->set_anchors_preset(Control::PRESET_CENTER_BOTTOM);
	promptLabel_->set_position(Vector2(-180.0f, -86.0f));
	promptLabel_->set_size(Vector2(360.0f, 44.0f));
	promptLabel_->set_horizontal_alignment(HORIZONTAL_ALIGNMENT_CENTER);
	promptLabel_->add_theme_font_size_override("font_size", 18);
	canvas->add_child(promptLabel_);
	Label *crosshair = memnew(Label);
	crosshair->set_text("+");
	crosshair->set_anchors_preset(Control::PRESET_CENTER);
	crosshair->set_position(Vector2(-8.0f, -14.0f));
	crosshair->add_theme_font_size_override("font_size", 22);
	crosshair->add_theme_color_override("font_color", Color(0.74f, 0.86f, 0.82f, 0.8f));
	canvas->add_child(crosshair);

	warningLabel_ = memnew(Label);
	warningLabel_->set_anchors_preset(Control::PRESET_CENTER_TOP);
	warningLabel_->set_position(Vector2(-260.0f, 26.0f));
	warningLabel_->set_size(Vector2(520.0f, 52.0f));
	warningLabel_->set_horizontal_alignment(HORIZONTAL_ALIGNMENT_CENTER);
	warningLabel_->add_theme_font_size_override("font_size", 22);
	warningLabel_->add_theme_color_override("font_color", Color(1.0f, 0.34f, 0.25f));
	canvas->add_child(warningLabel_);

	eventPanel_ = memnew(PanelContainer);
	eventPanel_->set_anchors_preset(Control::PRESET_CENTER);
	eventPanel_->set_position(Vector2(-300.0f, -210.0f));
	eventPanel_->set_size(Vector2(600.0f, 420.0f));
	eventPanel_->add_theme_stylebox_override("panel", makePanelStyle(Color(0.055f, 0.06f, 0.058f, 0.98f), Color(0.76f, 0.55f, 0.25f, 0.9f)));
	eventPanel_->set_visible(false);
	canvas->add_child(eventPanel_);
	VBoxContainer *eventStack = memnew(VBoxContainer);
	eventStack->add_theme_constant_override("separation", 14);
	eventPanel_->add_child(eventStack);
	Label *eventTitle = memnew(Label);
	eventTitle->set_text("A SIGNAL BETWEEN WELLS");
	eventTitle->add_theme_font_size_override("font_size", 25);
	eventTitle->add_theme_color_override("font_color", Color(0.95f, 0.76f, 0.44f));
	eventStack->add_child(eventTitle);
	Label *eventBody = memnew(Label);
	eventBody->set_text("The damaged array resolves a narrow multispecies beacon. Its route is older than Cirrus-9's last refit. Following it may reveal a quiet pocket, but the scan will draw power from either food preservation or the capacitor bank.");
	eventBody->set_autowrap_mode(TextServer::AUTOWRAP_WORD_SMART);
	eventBody->set_custom_minimum_size(Vector2(540.0f, 145.0f));
	eventBody->add_theme_font_size_override("font_size", 17);
	eventStack->add_child(eventBody);
	Button *foodChoice = memnew(Button);
	foodChoice->set_text("Preserve the scan. Divert cultivation power.");
	foodChoice->connect("pressed", Callable(this, "onEventChoice").bind(0));
	eventStack->add_child(foodChoice);
	Button *powerChoice = memnew(Button);
	powerChoice->set_text("Protect the food. Drain the capacitor bank.");
	powerChoice->connect("pressed", Callable(this, "onEventChoice").bind(1));
	eventStack->add_child(powerChoice);
	eventResultLabel_ = memnew(Label);
	eventResultLabel_->set_autowrap_mode(TextServer::AUTOWRAP_WORD_SMART);
	eventResultLabel_->add_theme_color_override("font_color", Color(0.73f, 0.84f, 0.76f));
	eventStack->add_child(eventResultLabel_);

	fadeOverlay_ = memnew(ColorRect);
	fadeOverlay_->set_anchors_and_offsets_preset(Control::PRESET_FULL_RECT);
	fadeOverlay_->set_mouse_filter(Control::MOUSE_FILTER_IGNORE);
	fadeOverlay_->set_color(Color(0.0f, 0.0f, 0.0f, 1.0f));
	canvas->add_child(fadeOverlay_);
}

void VerticalSlice::_process(double pDelta) {
	if (tutorialStage_ == 0 && player_->get_global_position().distance_to(tutorialStartPosition_) > 1.25f) advanceTutorial();
	if (warningRemainingS_ > 0.0) {
		warningRemainingS_ -= pDelta;
		if (warningRemainingS_ <= 0.0) warningLabel_->set_text("");
	}

	if (fadeDirection_ != 0) {
		fadeAlpha_ = std::clamp(fadeAlpha_ + fadeDirection_ * pDelta * 1.8, 0.0, 1.0);
		fadeOverlay_->set_color(Color(0.0f, 0.0f, 0.0f, static_cast<float>(fadeAlpha_)));
		if (fadeDirection_ > 0 && fadeAlpha_ >= 1.0) {
			player_->set_global_position(Vector3(2.1f, 0.1f, 5.0f));
			player_->set_rotation(Vector3(0.0f, 3.14f, 0.0f));
			fadeDirection_ = -1;
		} else if (fadeDirection_ < 0 && fadeAlpha_ <= 0.0) {
			fadeDirection_ = 0;
			fadeOverlay_->set_visible(false);
			if (player_->getLocomotionMode() == PlayerController::Cinematic) player_->setLocomotionMode(modules_->isArtificialGravityEnabled() ? PlayerController::Grounded : PlayerController::ZeroGravity);
		}
	}

	if (tutorialStage_ >= 8 && !eventShown_) {
		eventTimerS_ += pDelta;
		if (eventTimerS_ > 2.0) showNarrativeEvent();
	}
	updateHud();
}

void VerticalSlice::updateHud() {
	if (!simulation_ || !player_) return;
	promptLabel_->set_text(player_->getFocusPrompt().is_empty() ? "" : "[E]  " + player_->getFocusPrompt());
	const Dictionary profile = simulation_->getProfiling();
	const double stepMs = profile.has("averageStepMs") ? static_cast<double>(profile["averageStepMs"]) : 0.0;
	telemetryLabel_->set_text(simulation_->getStateSummary() + "\nMODE  " + player_->getLocomotionModeName() + "  /  BT  " + player_->getCurrentBehaviorNode() + "\nSIM  " + String::num(simulation_->getSimulationTimeS(), 0) + " s  SHIP CLOCK  " + String::num(simulation_->getProperTimeS(), 3) + " s  STEP  " + String::num(stepMs, 3) + " ms\nTIDAL  " + String::num_scientific(simulation_->getTidalSeverity()) + " s^-2  SNAPSHOT AGE  " + String::num(simulation_->getSnapshotAgeMs(), 1) + " ms");
	resourcesLabel_->set_text("SHIELD " + String::num(modules_->getShieldPower() * 100.0, 0) + "%   INTEGRITY " + String::num(modules_->getModuleIntegrity(StringName("Vacuum Shield")) * 100.0, 0) + "%   FOOD " + String::num(modules_->getFood() * 100.0, 0) + "%   CAP " + String::num(modules_->getCapacitorPower() * 100.0, 0) + "%\nSENSOR MASS " + String::num_scientific(simulation_->getSensorPrimaryMassKg()) + " kg   ENTERED " + String::num_scientific(simulation_->getEstimatedPrimaryMassKg()) + " kg");
	maneuverLabel_->set_text("ENCOUNTER SPEED " + String::num(simulation_->getShipRelativeSpeedMps() / 1000.0, 1) + " km/s   SHIP MASS " + String::num(simulation_->getShipTotalMassKg() / 1000.0, 1) + " t\nPROPELLANT " + String::num(simulation_->getShipPropellantMassKg() / 1000.0, 1) + " t   REACTION MASS " + String::num(simulation_->getShipReactionMassKg() / 1000.0, 1) + " t\nSELECTED BURN " + String::num(selectedBurnDeltaVMps_, 0) + " m/s   PREVIEW ENERGY +" + String::num(simulation_->getPreviewOberthEnergyGain(selectedBurnDeltaVMps_) / 1.0e6, 1) + " MJ/kg");
}

void VerticalSlice::advanceTutorial() {
	++tutorialStage_;
	switch (tutorialStage_) {
		case 1: tutorialLabel_->set_text("Inspect the maintenance placard on the forward bulkhead."); break;
		case 2: tutorialLabel_->set_text("Press the red gravity breaker at the navigation console."); break;
		case 3: tutorialLabel_->set_text("Gravity is gone. Use Space and Ctrl to move vertically, then press the breaker again."); break;
		case 4: tutorialLabel_->set_text("Turn the brass mass-model knob. The entered estimate changes; truth does not."); break;
		case 5: tutorialLabel_->set_text("Time passes. Adjust the shield-bus slider in the next maintenance beat."); beginTutorialTransition(); break;
		case 6: tutorialLabel_->set_text("Sit in the navigation chair."); break;
		case 7: tutorialLabel_->set_text("Press E to stand, then consume the peach ration on the kitchen bench."); break;
		case 8: tutorialLabel_->set_text("Training beat complete. Celestial simulation continued throughout every action."); break;
		default: break;
	}
}

void VerticalSlice::beginTutorialTransition() {
	fadeOverlay_->set_visible(true);
	fadeDirection_ = 1;
	player_->setLocomotionMode(PlayerController::Cinematic);
}

void VerticalSlice::onPlayerActionCompleted(const StringName &pAction, Object *pTarget) {
	if (tutorialStage_ == 1 && pAction == StringName("inspect") && pTarget == inspectable_) advanceTutorial();
	else if (tutorialStage_ == 2 && pAction == StringName("press") && pTarget == gravityButton_) advanceTutorial();
	else if (tutorialStage_ == 3 && pAction == StringName("press") && pTarget == gravityButton_ && modules_->isArtificialGravityEnabled()) advanceTutorial();
	else if (tutorialStage_ == 4 && (pAction == StringName("turn") || pAction == StringName("adjust")) && pTarget == estimateKnob_) advanceTutorial();
	else if (tutorialStage_ == 5 && pAction == StringName("adjust") && pTarget == shieldSlider_) advanceTutorial();
	else if (tutorialStage_ == 6 && pAction == StringName("sit") && pTarget == seat_) advanceTutorial();
	else if (tutorialStage_ == 7 && pAction == StringName("eat") && pTarget == ration_) advanceTutorial();
}

void VerticalSlice::onGravityButton(const StringName &pAction, double) {
	if (pAction != StringName("press")) return;
	modules_->setArtificialGravityEnabled(!modules_->isArtificialGravityEnabled());
}

void VerticalSlice::onKnobChanged(double, double pGameplayValue) { if (simulation_) simulation_->setEstimatedMassScale(pGameplayValue); }
void VerticalSlice::onSliderChanged(double pNormalized, double) { if (modules_) modules_->setShieldPower(pNormalized); }
void VerticalSlice::onBurnSettingChanged(double, double pGameplayValue) { selectedBurnDeltaVMps_ = pGameplayValue; }
void VerticalSlice::onDumpSettingChanged(double pNormalized, double) { selectedDumpMassKg_ = std::max(1000.0, pNormalized * 50000.0); }

void VerticalSlice::onBurnButton(const StringName &pAction, double) {
	if (pAction != StringName("press")) return;
	if (simulation_->queueProgradeBurn(selectedBurnDeltaVMps_)) {
		warningLabel_->set_text("MANUAL PROGRADE BURN QUEUED");
		warningRemainingS_ = 1.2;
	}
}

void VerticalSlice::onDumpButton(const StringName &pAction, double) {
	if (pAction != StringName("press")) return;
	if (simulation_->queueReactionMassDump(selectedDumpMassKg_)) {
		warningLabel_->set_text("RETROGRADE EJECTA RELEASE QUEUED");
		warningRemainingS_ = 1.2;
	}
}

void VerticalSlice::onManeuverExecuted(const StringName &pKind, double pDeltaVMps, double pExpendedMassKg, double pEnergyGainJPerKg) {
	const String label = pKind == StringName("progradeBurn") ? "OBERTH BURN" : "REACTION-MASS DUMP";
	warningLabel_->set_text(label + String("  +") + String::num(pDeltaVMps, 1) + " m/s  /  " + String::num(pExpendedMassKg / 1000.0, 2) + " t  /  ENERGY +" + String::num(pEnergyGainJPerKg / 1.0e6, 1) + " MJ/kg");
	warningRemainingS_ = 3.0;
	player_->addCameraTrauma(pKind == StringName("progradeBurn") ? 0.28 : 0.16);
	playGeneratedTone(pKind == StringName("progradeBurn") ? 145.0 : 230.0, 0.28);
}
void VerticalSlice::onInspected(const String &pText) { tutorialLabel_->set_text(pText); }

void VerticalSlice::onWarning(const String &pMessage, double pSeverity) {
	warningLabel_->set_text(pMessage);
	warningRemainingS_ = 3.5;
	player_->addCameraTrauma(0.08 + pSeverity * 0.12);
}

void VerticalSlice::onAudioFeedback(const StringName &pCue) {
	playGeneratedTone(pCue == StringName("criticalAlarm") ? 180.0 : 420.0, pCue == StringName("criticalAlarm") ? 0.3 : 0.12);
}

void VerticalSlice::onInteractionFeedback(const StringName &pKind, double pStrength) {
	player_->addCameraTrauma(pStrength);
	playGeneratedTone(pKind == StringName("knobDetent") ? 720.0 : 510.0, 0.045);
}

void VerticalSlice::playGeneratedTone(double pFrequencyHz, double pDurationS) {
	constexpr int mixRate = 22050;
	const int sampleCount = std::max(1, static_cast<int>(pDurationS * mixRate));
	PackedByteArray data;
	data.resize(sampleCount * 2);
	for (int i = 0; i < sampleCount; ++i) {
		const double envelope = 1.0 - static_cast<double>(i) / sampleCount;
		const std::int16_t sample = static_cast<std::int16_t>(std::sin(Math_TAU * pFrequencyHz * i / mixRate) * envelope * 5200.0);
		data.set(i * 2, static_cast<std::uint8_t>(sample & 0xff));
		data.set(i * 2 + 1, static_cast<std::uint8_t>((sample >> 8) & 0xff));
	}
	Ref<AudioStreamWAV> stream;
	stream.instantiate();
	stream->set_format(AudioStreamWAV::FORMAT_16_BITS);
	stream->set_mix_rate(mixRate);
	stream->set_stereo(false);
	stream->set_data(data);
	audioPlayer_->set_stream(stream);
	audioPlayer_->play();
}

void VerticalSlice::showNarrativeEvent() {
	eventShown_ = true;
	eventPanel_->set_visible(true);
	player_->setLocomotionMode(PlayerController::Cinematic);
	Input::get_singleton()->set_mouse_mode(Input::MOUSE_MODE_VISIBLE);
}

void VerticalSlice::onEventChoice(int pChoice) {
	modules_->applyNarrativeChoice(pChoice);
	eventResultLabel_->set_text(pChoice == 0 ? "Cultivation lamps dim. The route remains legible, but breakfast becomes arithmetic." : "The capacitor bank falls quiet by one more bar. Cold storage remains stable.");
	eventPanel_->set_visible(false);
	player_->setLocomotionMode(modules_->isArtificialGravityEnabled() ? PlayerController::Grounded : PlayerController::ZeroGravity);
	Input::get_singleton()->set_mouse_mode(Input::MOUSE_MODE_CAPTURED);
}
