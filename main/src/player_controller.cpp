#include "player_controller.h"

#include <godot_cpp/classes/capsule_shape3d.hpp>
#include <godot_cpp/classes/collision_shape3d.hpp>
#include <godot_cpp/classes/input.hpp>
#include <godot_cpp/classes/input_event_mouse_motion.hpp>
#include <godot_cpp/classes/object.hpp>
#include <godot_cpp/core/class_db.hpp>
#include <godot_cpp/core/object.hpp>

#include <algorithm>
#include <cmath>

using namespace godot;
using namespace agame::gameplay;

PlayerController::PlayerController() = default;
PlayerController::~PlayerController() = default;

void PlayerController::_bind_methods() {
	ClassDB::bind_method(D_METHOD("setLocomotionMode", "mode"), &PlayerController::setLocomotionMode);
	ClassDB::bind_method(D_METHOD("getLocomotionMode"), &PlayerController::getLocomotionMode);
	ClassDB::bind_method(D_METHOD("getLocomotionModeName"), &PlayerController::getLocomotionModeName);
	ClassDB::bind_method(D_METHOD("setArtificialGravityEnabled", "enabled"), &PlayerController::setArtificialGravityEnabled);
	ClassDB::bind_method(D_METHOD("isArtificialGravityEnabled"), &PlayerController::isArtificialGravityEnabled);
	ClassDB::bind_method(D_METHOD("requestInteraction"), &PlayerController::requestInteraction);
	ClassDB::bind_method(D_METHOD("requestAction", "target", "action"), &PlayerController::requestAction);
	ClassDB::bind_method(D_METHOD("cancelAction", "reason"), &PlayerController::cancelAction, DEFVAL("Cancelled"));
	ClassDB::bind_method(D_METHOD("addCameraTrauma", "amount"), &PlayerController::addCameraTrauma);
	ClassDB::bind_method(D_METHOD("getFocusPrompt"), &PlayerController::getFocusPrompt);
	ClassDB::bind_method(D_METHOD("getCurrentBehaviorNode"), &PlayerController::getCurrentBehaviorNode);
	ClassDB::bind_method(D_METHOD("setMoveSpeed", "speed"), &PlayerController::setMoveSpeed);
	ClassDB::bind_method(D_METHOD("getMoveSpeed"), &PlayerController::getMoveSpeed);
	ClassDB::bind_method(D_METHOD("setMouseSensitivity", "sensitivity"), &PlayerController::setMouseSensitivity);
	ClassDB::bind_method(D_METHOD("getMouseSensitivity"), &PlayerController::getMouseSensitivity);
	ADD_PROPERTY(PropertyInfo(Variant::INT, "locomotionMode", PROPERTY_HINT_ENUM, "Grounded,Zero gravity,Seated,Action locked,Cinematic,Disabled"), "setLocomotionMode", "getLocomotionMode");
	ADD_PROPERTY(PropertyInfo(Variant::BOOL, "artificialGravityEnabled"), "setArtificialGravityEnabled", "isArtificialGravityEnabled");
	ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "moveSpeed", PROPERTY_HINT_RANGE, "1,12,0.1"), "setMoveSpeed", "getMoveSpeed");
	ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "mouseSensitivity", PROPERTY_HINT_RANGE, "0.0005,0.01,0.0001"), "setMouseSensitivity", "getMouseSensitivity");
	ADD_SIGNAL(MethodInfo("locomotionModeChanged", PropertyInfo(Variant::INT, "mode"), PropertyInfo(Variant::STRING, "modeName")));
	ADD_SIGNAL(MethodInfo("focusChanged", PropertyInfo(Variant::STRING, "prompt")));
	ADD_SIGNAL(MethodInfo("actionStarted", PropertyInfo(Variant::STRING_NAME, "action"), PropertyInfo(Variant::OBJECT, "target")));
	ADD_SIGNAL(MethodInfo("actionContact", PropertyInfo(Variant::STRING_NAME, "action"), PropertyInfo(Variant::OBJECT, "target")));
	ADD_SIGNAL(MethodInfo("actionCompleted", PropertyInfo(Variant::STRING_NAME, "action"), PropertyInfo(Variant::OBJECT, "target")));
	ADD_SIGNAL(MethodInfo("actionFailed", PropertyInfo(Variant::STRING_NAME, "action"), PropertyInfo(Variant::STRING, "reason")));

	BIND_ENUM_CONSTANT(Grounded);
	BIND_ENUM_CONSTANT(ZeroGravity);
	BIND_ENUM_CONSTANT(Seated);
	BIND_ENUM_CONSTANT(ActionLocked);
	BIND_ENUM_CONSTANT(Cinematic);
	BIND_ENUM_CONSTANT(Disabled);
}

void PlayerController::_ready() {
	CollisionShape3D *collision = memnew(CollisionShape3D);
	collision->set_name("PlayerCollision");
	Ref<CapsuleShape3D> capsule;
	capsule.instantiate();
	capsule->set_radius(0.34f);
	capsule->set_height(1.7f);
	collision->set_shape(capsule);
	collision->set_position(Vector3(0.0f, 0.85f, 0.0f));
	add_child(collision);

	camera_ = memnew(Camera3D);
	camera_->set_name("Camera");
	camera_->set_position(Vector3(0.0f, 1.55f, 0.0f));
	camera_->set_current(true);
	add_child(camera_);

	interactionRay_ = memnew(RayCast3D);
	interactionRay_->set_name("InteractionRay");
	interactionRay_->set_target_position(Vector3(0.0f, 0.0f, -3.2f));
	interactionRay_->set_collision_mask(1);
	interactionRay_->set_enabled(true);
	camera_->add_child(interactionRay_);

	Input::get_singleton()->set_mouse_mode(Input::MOUSE_MODE_CAPTURED);
	set_physics_process(true);
	set_process(true);
}

void PlayerController::_process(double pDelta) {
	updateFocus();
	shakePhase_ += pDelta * 31.0;
	cameraTrauma_ = std::max(0.0, cameraTrauma_ - pDelta * 0.9);
	if (camera_) {
		const double shake = cameraTrauma_ * cameraTrauma_;
		camera_->set_rotation(Vector3(static_cast<float>(pitch_ + std::sin(shakePhase_ * 1.7) * shake * 0.018), 0.0f, static_cast<float>(std::sin(shakePhase_ * 2.3) * shake * 0.014)));
	}
}

void PlayerController::_physics_process(double pDelta) {
	Input *input = Input::get_singleton();
	if (input->is_action_just_pressed("interact")) {
		requestInteraction();
	}
	if (input->is_action_just_pressed("cancelAction") && mode_ == ActionLocked) {
		cancelAction("Action cancelled");
	}

	switch (mode_) {
		case Grounded: updateGrounded(pDelta); break;
		case ZeroGravity: updateZeroGravity(pDelta); break;
		case ActionLocked: updateAction(pDelta); break;
		case Seated:
		case Cinematic:
		case Disabled:
			set_velocity(Vector3());
			break;
	}
}

void PlayerController::_unhandled_input(const Ref<InputEvent> &pEvent) {
	const Ref<InputEventMouseMotion> mouseMotion = pEvent;
	if (mouseMotion.is_valid()) {
		if (Input::get_singleton()->get_mouse_mode() == Input::MOUSE_MODE_CAPTURED && mode_ != Disabled) {
			rotate_y(static_cast<float>(-mouseMotion->get_relative().x * mouseSensitivity_));
			pitch_ = std::clamp(pitch_ - mouseMotion->get_relative().y * mouseSensitivity_, -1.45, 1.45);
		}
	}
	if (pEvent->is_action_pressed("toggleMouse")) {
		const Input::MouseMode current = Input::get_singleton()->get_mouse_mode();
		Input::get_singleton()->set_mouse_mode(current == Input::MOUSE_MODE_CAPTURED ? Input::MOUSE_MODE_VISIBLE : Input::MOUSE_MODE_CAPTURED);
	}
}

void PlayerController::updateGrounded(double pDelta) {
	Input *input = Input::get_singleton();
	const Vector2 movement = input->get_vector("moveLeft", "moveRight", "moveForward", "moveBack");
	Vector3 direction = get_global_transform().basis.xform(Vector3(movement.x, 0.0f, movement.y));
	direction.y = 0.0f;
	direction = direction.normalized();
	Vector3 velocity = get_velocity();
	velocity.x = static_cast<float>(direction.x * moveSpeed_);
	velocity.z = static_cast<float>(direction.z * moveSpeed_);
	if (!is_on_floor()) {
		velocity.y -= static_cast<float>(9.81 * pDelta);
	} else {
		velocity.y = -0.2f;
	}
	set_velocity(velocity);
	move_and_slide();
}

void PlayerController::updateZeroGravity(double pDelta) {
	Input *input = Input::get_singleton();
	const Vector2 movement = input->get_vector("moveLeft", "moveRight", "moveForward", "moveBack");
	const double vertical = input->get_action_strength("moveUp") - input->get_action_strength("moveDown");
	Vector3 localDirection(movement.x, static_cast<float>(vertical), movement.y);
	if (localDirection.length_squared() > 1.0f) localDirection = localDirection.normalized();
	const Vector3 desired = get_global_transform().basis.xform(localDirection) * static_cast<float>(zeroGSpeed_);
	set_velocity(get_velocity().lerp(desired, static_cast<float>(std::clamp(pDelta * 3.0, 0.0, 1.0))));
	move_and_slide();
}

void PlayerController::setLocomotionMode(int pMode) {
	const int safeMode = std::clamp(pMode, static_cast<int>(Grounded), static_cast<int>(Disabled));
	const LocomotionMode next = static_cast<LocomotionMode>(safeMode);
	if (next == mode_) return;
	mode_ = next;
	emit_signal("locomotionModeChanged", static_cast<int>(mode_), getLocomotionModeName());
}

String PlayerController::getLocomotionModeName() const {
	switch (mode_) {
		case Grounded: return "Grounded";
		case ZeroGravity: return "Zero gravity";
		case Seated: return "Seated";
		case ActionLocked: return "Action locked";
		case Cinematic: return "Cinematic";
		case Disabled: return "Disabled";
	}
	return "Unknown";
}

void PlayerController::setArtificialGravityEnabled(bool pEnabled) {
	artificialGravityEnabled_ = pEnabled;
	if (mode_ == Grounded && !pEnabled) setLocomotionMode(ZeroGravity);
	else if (mode_ == ZeroGravity && pEnabled) setLocomotionMode(Grounded);
}

void PlayerController::updateFocus() {
	String prompt;
	if (mode_ == Seated) {
		prompt = "Stand from navigation chair";
	} else if (interactionRay_ && interactionRay_->is_colliding()) {
		if (Interactable3D *target = Object::cast_to<Interactable3D>(interactionRay_->get_collider())) {
			prompt = target->getPrompt();
		}
	}
	if (prompt != focusPrompt_) {
		focusPrompt_ = prompt;
		emit_signal("focusChanged", focusPrompt_);
	}
}

void PlayerController::requestInteraction() {
	if (mode_ == ActionLocked || mode_ == Cinematic || mode_ == Disabled) return;
	if (mode_ == Seated) {
		if (Interactable3D *seat = Object::cast_to<Interactable3D>(ObjectDB::get_instance(seatedTargetId_))) startAction(seat, StringName("stand"));
		return;
	}
	if (!interactionRay_) return;
	interactionRay_->force_raycast_update();
	if (Interactable3D *target = Object::cast_to<Interactable3D>(interactionRay_->get_collider())) {
		startAction(target, target->getPrimaryAction());
	}
}

void PlayerController::requestAction(Interactable3D *pTarget, const StringName &pAction) {
	if (!pTarget || mode_ == ActionLocked || mode_ == Cinematic || mode_ == Disabled) return;
	startAction(pTarget, pAction);
}

void PlayerController::startAction(Interactable3D *pTarget, const StringName &pAction) {
	if (!pTarget) return;
	cancelAction("Replaced by another action");
	actionTargetId_ = pTarget->get_instance_id();
	currentAction_ = pAction;
	modeBeforeAction_ = mode_;
	blackboard_ = ActionBlackboard{};
	blackboard_.actor = this;
	blackboard_.target = pTarget;
	blackboard_.requestedAction = parseAction(pAction);
	blackboard_.interactionAnchor = {pTarget->getInteractionAnchor().x, pTarget->getInteractionAnchor().y, pTarget->getInteractionAnchor().z};
	blackboard_.interruptible = pTarget->actionIsInterruptible(pAction);
	blackboard_.reservationToken = static_cast<std::uint64_t>(get_instance_id());

	auto sequence = std::make_unique<SequenceNode>("Player action");
	sequence->add(std::make_unique<ConditionNode>("Validate target and verb", [this](ActionBlackboard &) {
		Interactable3D *target = resolveActionTarget();
		if (!target) { blackboard_.failureReason = "Target no longer exists"; return false; }
		if (!target->supportsAction(currentAction_)) { blackboard_.failureReason = "Action is unavailable"; return false; }
		return true;
	}));
	sequence->add(std::make_unique<TaskNode>("Reserve target", [this](TickContext &) {
		Interactable3D *target = resolveActionTarget();
		if (!target || !target->reserve(static_cast<std::int64_t>(get_instance_id()))) { blackboard_.failureReason = "Target is occupied"; return BehaviorStatus::Failure; }
		return BehaviorStatus::Success;
	}));
	sequence->add(std::make_unique<TaskNode>("Align actor and begin", [this](TickContext &) {
		Interactable3D *target = resolveActionTarget();
		if (!target) return BehaviorStatus::Failure;
		if (target->actionRequiresAlignment(currentAction_)) set_global_position(target->getInteractionAnchor());
		target->beginInteraction(currentAction_, static_cast<std::int64_t>(get_instance_id()));
		emit_signal("actionStarted", currentAction_, target);
		return BehaviorStatus::Success;
	}));
	const double halfDuration = std::max(0.05, pTarget->getActionDuration(pAction) * 0.5);
	sequence->add(std::make_unique<WaitNode>(halfDuration, "Wait for contact marker"));
	sequence->add(std::make_unique<TaskNode>("Apply gameplay effect", [this](TickContext &) {
		Interactable3D *target = resolveActionTarget();
		if (!target || !target->performAction(currentAction_, 1.0)) { blackboard_.failureReason = "Gameplay effect rejected"; return BehaviorStatus::Failure; }
		emit_signal("actionContact", currentAction_, target);
		return BehaviorStatus::Success;
	}));
	sequence->add(std::make_unique<WaitNode>(halfDuration, "Play exit animation"));
	sequence->add(std::make_unique<TaskNode>("Release target and return control", [this](TickContext &) {
		Interactable3D *target = resolveActionTarget();
		if (!target) return BehaviorStatus::Failure;
		target->completeInteraction(currentAction_, static_cast<std::int64_t>(get_instance_id()));
		target->release(static_cast<std::int64_t>(get_instance_id()));
		return BehaviorStatus::Success;
	}));
	actionTree_ = std::make_unique<BehaviorTree>(std::move(sequence));
	setLocomotionMode(ActionLocked);
}

void PlayerController::updateAction(double pDelta) {
	set_velocity(Vector3());
	if (!actionTree_) { setLocomotionMode(artificialGravityEnabled_ ? Grounded : ZeroGravity); return; }
	const BehaviorStatus status = actionTree_->tick(pDelta, blackboard_);
	if (status == BehaviorStatus::Running) return;
	Interactable3D *target = resolveActionTarget();
	if (status == BehaviorStatus::Success) {
		if (currentAction_ == StringName("sit")) { seatedTargetId_ = actionTargetId_; setLocomotionMode(Seated); }
		else if (currentAction_ == StringName("stand")) { seatedTargetId_ = ObjectID(); setLocomotionMode(artificialGravityEnabled_ ? Grounded : ZeroGravity); }
		else setLocomotionMode(modeBeforeAction_ == ZeroGravity || !artificialGravityEnabled_ ? ZeroGravity : Grounded);
		emit_signal("actionCompleted", currentAction_, target);
	} else {
		if (target) target->release(static_cast<std::int64_t>(get_instance_id()));
		setLocomotionMode(modeBeforeAction_);
		emit_signal("actionFailed", currentAction_, String(blackboard_.failureReason.c_str()));
	}
	actionTree_.reset();
	actionTargetId_ = ObjectID();
}

void PlayerController::cancelAction(const String &pReason) {
	if (!actionTree_) return;
	if (!blackboard_.interruptible) return;
	actionTree_->cancel(blackboard_);
	if (Interactable3D *target = resolveActionTarget()) target->release(static_cast<std::int64_t>(get_instance_id()));
	emit_signal("actionFailed", currentAction_, pReason);
	actionTree_.reset();
	actionTargetId_ = ObjectID();
	setLocomotionMode(modeBeforeAction_);
}

void PlayerController::addCameraTrauma(double pAmount) { cameraTrauma_ = std::clamp(cameraTrauma_ + pAmount, 0.0, 1.0); }
String PlayerController::getCurrentBehaviorNode() const { return String(blackboard_.currentNode.c_str()); }
Interactable3D *PlayerController::resolveActionTarget() const { return Object::cast_to<Interactable3D>(ObjectDB::get_instance(actionTargetId_)); }

ActionId PlayerController::parseAction(const StringName &pAction) {
	if (pAction == StringName("inspect")) return ActionId::Inspect;
	if (pAction == StringName("sit")) return ActionId::Sit;
	if (pAction == StringName("stand")) return ActionId::Stand;
	if (pAction == StringName("eat")) return ActionId::Eat;
	if (pAction == StringName("press")) return ActionId::Press;
	if (pAction == StringName("turn")) return ActionId::Turn;
	if (pAction == StringName("adjust")) return ActionId::Adjust;
	if (pAction == StringName("open")) return ActionId::Open;
	if (pAction == StringName("repair")) return ActionId::Repair;
	return ActionId::Interact;
}
