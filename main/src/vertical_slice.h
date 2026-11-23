#pragma once

#include "interactable_3d.h"
#include "player_controller.h"
#include "ship_module_system.h"
#include "simulation_runner.h"

#include <godot_cpp/classes/audio_stream_player.hpp>
#include <godot_cpp/classes/color_rect.hpp>
#include <godot_cpp/classes/label.hpp>
#include <godot_cpp/classes/panel_container.hpp>

class VerticalSlice : public godot::Node3D {
	GDCLASS(VerticalSlice, godot::Node3D);

public:
	void _ready() override;
	void _process(double pDelta) override;

protected:
	static void _bind_methods();

private:
	void configureInput();
	void buildShipInterior();
	void buildSystemsAndPlayer();
	void buildInteractables();
	void buildHud();
	void updateHud();
	void advanceTutorial();
	void beginTutorialTransition();
	void showNarrativeEvent();
	void playGeneratedTone(double pFrequencyHz, double pDurationS);

	void onPlayerActionCompleted(const godot::StringName &pAction, godot::Object *pTarget);
	void onGravityButton(const godot::StringName &pAction, double pValue);
	void onKnobChanged(double pNormalized, double pGameplayValue);
	void onSliderChanged(double pNormalized, double pGameplayValue);
	void onBurnSettingChanged(double pNormalized, double pGameplayValue);
	void onDumpSettingChanged(double pNormalized, double pGameplayValue);
	void onBurnButton(const godot::StringName &pAction, double pValue);
	void onDumpButton(const godot::StringName &pAction, double pValue);
	void onManeuverExecuted(const godot::StringName &pKind, double pDeltaVMps, double pExpendedMassKg, double pEnergyGainJPerKg);
	void onInspected(const godot::String &pText);
	void onWarning(const godot::String &pMessage, double pSeverity);
	void onAudioFeedback(const godot::StringName &pCue);
	void onInteractionFeedback(const godot::StringName &pKind, double pStrength);
	void onEventChoice(int pChoice);

	SimulationRunner *simulation_ = nullptr;
	ShipModuleSystem *modules_ = nullptr;
	PlayerController *player_ = nullptr;
	PushButton3D *gravityButton_ = nullptr;
	RotaryKnob3D *estimateKnob_ = nullptr;
	LinearSlider3D *shieldSlider_ = nullptr;
	RotaryKnob3D *burnSettingKnob_ = nullptr;
	PushButton3D *burnButton_ = nullptr;
	LinearSlider3D *dumpSettingSlider_ = nullptr;
	PushButton3D *dumpButton_ = nullptr;
	Seat3D *seat_ = nullptr;
	Inspectable3D *inspectable_ = nullptr;
	EdibleItem3D *ration_ = nullptr;

	godot::Label *tutorialLabel_ = nullptr;
	godot::Label *promptLabel_ = nullptr;
	godot::Label *telemetryLabel_ = nullptr;
	godot::Label *resourcesLabel_ = nullptr;
	godot::Label *maneuverLabel_ = nullptr;
	godot::Label *warningLabel_ = nullptr;
	godot::ColorRect *fadeOverlay_ = nullptr;
	godot::PanelContainer *eventPanel_ = nullptr;
	godot::Label *eventResultLabel_ = nullptr;
	godot::AudioStreamPlayer *audioPlayer_ = nullptr;

	int tutorialStage_ = 0;
	godot::Vector3 tutorialStartPosition_;
	double warningRemainingS_ = 0.0;
	double eventTimerS_ = 0.0;
	double selectedBurnDeltaVMps_ = 680.0;
	double selectedDumpMassKg_ = 15000.0;
	bool eventShown_ = false;
	int fadeDirection_ = 0;
	double fadeAlpha_ = 1.0;
};
