#pragma once

#include "gameplay/behavior_tree.hpp"
#include "interactable_3d.h"

#include <godot_cpp/classes/camera3d.hpp>
#include <godot_cpp/classes/character_body3d.hpp>
#include <godot_cpp/classes/input_event.hpp>
#include <godot_cpp/classes/ray_cast3d.hpp>

#include <memory>

class PlayerController : public godot::CharacterBody3D {
	GDCLASS(PlayerController, godot::CharacterBody3D);

public:
	enum LocomotionMode {
		Grounded = 0,
		ZeroGravity = 1,
		Seated = 2,
		ActionLocked = 3,
		Cinematic = 4,
		Disabled = 5,
	};

	PlayerController();
	~PlayerController() override;
	void _ready() override;
	void _process(double pDelta) override;
	void _physics_process(double pDelta) override;
	void _unhandled_input(const godot::Ref<godot::InputEvent> &pEvent) override;

	void setLocomotionMode(int pMode);
	int getLocomotionMode() const { return static_cast<int>(mode_); }
	godot::String getLocomotionModeName() const;
	void setArtificialGravityEnabled(bool pEnabled);
	bool isArtificialGravityEnabled() const { return artificialGravityEnabled_; }
	void requestInteraction();
	void requestAction(Interactable3D *pTarget, const godot::StringName &pAction);
	void cancelAction(const godot::String &pReason = "Cancelled");
	void addCameraTrauma(double pAmount);
	godot::String getFocusPrompt() const { return focusPrompt_; }
	godot::String getCurrentBehaviorNode() const;
	godot::Camera3D *getCamera() const { return camera_; }

	void setMoveSpeed(double pSpeed) { moveSpeed_ = pSpeed; }
	double getMoveSpeed() const { return moveSpeed_; }
	void setMouseSensitivity(double pSensitivity) { mouseSensitivity_ = pSensitivity; }
	double getMouseSensitivity() const { return mouseSensitivity_; }

protected:
	static void _bind_methods();

private:
	void updateGrounded(double pDelta);
	void updateZeroGravity(double pDelta);
	void updateAction(double pDelta);
	void updateFocus();
	void startAction(Interactable3D *pTarget, const godot::StringName &pAction);
	Interactable3D *resolveActionTarget() const;
	static agame::gameplay::ActionId parseAction(const godot::StringName &pAction);

	LocomotionMode mode_ = Grounded;
	LocomotionMode modeBeforeAction_ = Grounded;
	bool artificialGravityEnabled_ = true;
	double moveSpeed_ = 4.2;
	double zeroGSpeed_ = 3.5;
	double mouseSensitivity_ = 0.0022;
	double pitch_ = 0.0;
	double cameraTrauma_ = 0.0;
	double shakePhase_ = 0.0;

	godot::Camera3D *camera_ = nullptr;
	godot::RayCast3D *interactionRay_ = nullptr;
	godot::String focusPrompt_;
	godot::ObjectID actionTargetId_;
	godot::ObjectID seatedTargetId_;
	godot::StringName currentAction_;
	std::unique_ptr<agame::gameplay::BehaviorTree> actionTree_;
	agame::gameplay::ActionBlackboard blackboard_;
};

VARIANT_ENUM_CAST(PlayerController::LocomotionMode);
