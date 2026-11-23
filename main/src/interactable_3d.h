#pragma once

#include <godot_cpp/classes/collision_shape3d.hpp>
#include <godot_cpp/classes/mesh_instance3d.hpp>
#include <godot_cpp/classes/static_body3d.hpp>
#include <godot_cpp/variant/packed_string_array.hpp>
#include <godot_cpp/variant/string_name.hpp>

#include <vector>

struct InteractionOption {
	godot::StringName action;
	godot::String displayName;
	bool holdRequired = false;
	double durationS = 0.35;
	bool alignmentRequired = false;
	bool interruptible = true;
	godot::StringName animationId;
	godot::StringName soundCue;
};

class Interactable3D : public godot::StaticBody3D {
	GDCLASS(Interactable3D, godot::StaticBody3D);

public:
	Interactable3D();
	void _ready() override;

	godot::PackedStringArray getAvailableActions() const;
	bool supportsAction(const godot::StringName &pAction) const;
	godot::StringName getPrimaryAction() const;
	virtual godot::Vector3 getInteractionAnchor() const;
	virtual bool performAction(const godot::StringName &pAction, double pAmount = 1.0);

	bool reserve(std::int64_t pActorId);
	void release(std::int64_t pActorId);
	bool isReserved() const { return reservationOwner_ != 0; }
	std::int64_t getReservationOwner() const { return reservationOwner_; }

	void beginInteraction(const godot::StringName &pAction, std::int64_t pActorId);
	void completeInteraction(const godot::StringName &pAction, std::int64_t pActorId);
	double getActionDuration(const godot::StringName &pAction) const;
	bool actionRequiresAlignment(const godot::StringName &pAction) const;
	bool actionIsInterruptible(const godot::StringName &pAction) const;

	void setDisplayName(const godot::String &pName) { displayName_ = pName; }
	godot::String getDisplayName() const { return displayName_; }
	godot::String getPrompt() const;

protected:
	static void _bind_methods();
	void setOptions(std::vector<InteractionOption> pOptions);
	void createBoxGeometry(const godot::Vector3 &pSize, const godot::Color &pColor);
	const InteractionOption *findOption(const godot::StringName &pAction) const;

	std::vector<InteractionOption> options_;
	godot::MeshInstance3D *visual_ = nullptr;
	godot::CollisionShape3D *collision_ = nullptr;

private:
	godot::String displayName_ = "Interactable";
	std::int64_t reservationOwner_ = 0;
};

class PushButton3D : public Interactable3D {
	GDCLASS(PushButton3D, Interactable3D);

public:
	PushButton3D();
	void _ready() override;
	void _process(double pDelta) override;
	bool performAction(const godot::StringName &pAction, double pAmount = 1.0) override;

	void setToggleMode(bool pEnabled) { toggleMode_ = pEnabled; }
	bool isToggleMode() const { return toggleMode_; }
	void setPressed(bool pPressed);
	bool isPressed() const { return pressed_; }
	void setTravelDistance(double pDistance) { travelDistance_ = pDistance; }
	double getTravelDistance() const { return travelDistance_; }
	void setResetTime(double pTime) { resetTimeS_ = pTime; }
	double getResetTime() const { return resetTimeS_; }

protected:
	static void _bind_methods();

private:
	bool toggleMode_ = false;
	bool pressed_ = false;
	double travelDistance_ = 0.06;
	double resetTimeS_ = 0.25;
	double resetRemainingS_ = 0.0;
};

class RotaryKnob3D : public Interactable3D {
	GDCLASS(RotaryKnob3D, Interactable3D);

public:
	RotaryKnob3D();
	void _ready() override;
	bool performAction(const godot::StringName &pAction, double pAmount = 1.0) override;
	void setNormalizedValue(double pValue);
	double getNormalizedValue() const { return normalizedValue_; }
	void setAngleRange(double pDegrees) { angleRangeDegrees_ = pDegrees; }
	double getAngleRange() const { return angleRangeDegrees_; }
	void setGameplayMin(double pValue) { gameplayMin_ = pValue; }
	double getGameplayMin() const { return gameplayMin_; }
	void setGameplayMax(double pValue) { gameplayMax_ = pValue; }
	double getGameplayMax() const { return gameplayMax_; }
	void setSensitivity(double pValue) { sensitivity_ = pValue; }
	double getSensitivity() const { return sensitivity_; }
	void setInverted(bool pValue) { inverted_ = pValue; }
	bool isInverted() const { return inverted_; }
	void setDetents(int pValue) { detents_ = pValue; }
	int getDetents() const { return detents_; }

protected:
	static void _bind_methods();

private:
	double normalizedValue_ = 0.5;
	double angleRangeDegrees_ = 270.0;
	double gameplayMin_ = 0.0;
	double gameplayMax_ = 1.0;
	double sensitivity_ = 0.1;
	bool inverted_ = false;
	int detents_ = 11;
};

class LinearSlider3D : public Interactable3D {
	GDCLASS(LinearSlider3D, Interactable3D);

public:
	LinearSlider3D();
	void _ready() override;
	bool performAction(const godot::StringName &pAction, double pAmount = 1.0) override;
	void setNormalizedValue(double pValue);
	double getNormalizedValue() const { return normalizedValue_; }
	void setTravelDistance(double pValue) { travelDistance_ = pValue; }
	double getTravelDistance() const { return travelDistance_; }
	void setSensitivity(double pValue) { sensitivity_ = pValue; }
	double getSensitivity() const { return sensitivity_; }
	void setInverted(bool pValue) { inverted_ = pValue; }
	bool isInverted() const { return inverted_; }
	void setSteps(int pValue) { steps_ = pValue; }
	int getSteps() const { return steps_; }

protected:
	static void _bind_methods();

private:
	double normalizedValue_ = 0.5;
	double travelDistance_ = 0.7;
	double sensitivity_ = 0.1;
	bool inverted_ = false;
	int steps_ = 11;
};

class Seat3D : public Interactable3D {
	GDCLASS(Seat3D, Interactable3D);

public:
	Seat3D();
	void _ready() override;
	godot::Vector3 getInteractionAnchor() const override;
	bool performAction(const godot::StringName &pAction, double pAmount = 1.0) override;
	bool isOccupied() const { return occupied_; }

protected:
	static void _bind_methods();

private:
	bool occupied_ = false;
};

class Inspectable3D : public Interactable3D {
	GDCLASS(Inspectable3D, Interactable3D);

public:
	Inspectable3D();
	void _ready() override;
	bool performAction(const godot::StringName &pAction, double pAmount = 1.0) override;
	void setInspectionText(const godot::String &pText) { inspectionText_ = pText; }
	godot::String getInspectionText() const { return inspectionText_; }

protected:
	static void _bind_methods();

private:
	godot::String inspectionText_ = "A worn component with a careful repair history.";
};

class EdibleItem3D : public Interactable3D {
	GDCLASS(EdibleItem3D, Interactable3D);

public:
	EdibleItem3D();
	void _ready() override;
	bool performAction(const godot::StringName &pAction, double pAmount = 1.0) override;
	bool isConsumed() const { return consumed_; }
	double getNutrition() const { return nutrition_; }
	void setNutrition(double pValue) { nutrition_ = pValue; }

protected:
	static void _bind_methods();

private:
	bool consumed_ = false;
	double nutrition_ = 0.15;
};
