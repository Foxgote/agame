extends SceneTree

var failures: Array[String] = []


func _initialize() -> void:
	call_deferred("runSmokeTest")


func check(pCondition: bool, pMessage: String) -> void:
	if not pCondition:
		failures.append(pMessage)
		printerr("FAIL: ", pMessage)


func runAction(player: PlayerController, target: Interactable3D, action: StringName, simulation: SimulationRunner) -> void:
	var versionBefore := simulation.getTrueVersion()
	player.requestAction(target, action)
	var frames := 0
	while player.getLocomotionMode() == PlayerController.ActionLocked and frames < 180:
		await physics_frame
		frames += 1
	check(frames < 180, "action %s completed before timeout" % action)
	check(simulation.getTrueVersion() > versionBefore, "simulation advanced during %s" % action)


func runSmokeTest() -> void:
	var scene := load("res://main.tscn") as PackedScene
	var slice := scene.instantiate()
	root.add_child(slice)
	await process_frame
	await create_timer(0.2).timeout

	var simulation := slice.get_node("SimulationRunner") as SimulationRunner
	var player := slice.get_node("IzuPlayer") as PlayerController
	var modules := slice.get_node("ShipModules") as ShipModuleSystem
	var placard := slice.get_node("MaintenancePlacard") as Inspectable3D
	var button := slice.get_node("GravityBreaker") as PushButton3D
	var knob := slice.get_node("MassEstimateKnob") as RotaryKnob3D
	var slider := slice.get_node("ShieldPowerSlider") as LinearSlider3D
	var burnKnob := slice.get_node("BurnDeltaVKnob") as RotaryKnob3D
	var burnButton := slice.get_node("ExecuteBurnButton") as PushButton3D
	var dumpSlider := slice.get_node("ReactionMassSlider") as LinearSlider3D
	var dumpButton := slice.get_node("ExecuteMassDumpButton") as PushButton3D
	var seat := slice.get_node("NavigationSeat") as Seat3D
	var ration := slice.get_node("PeachRation") as EdibleItem3D

	check(simulation.getTrueVersion() > 0, "worker published an authoritative snapshot")
	check(button.supportsAction(&"press"), "button advertises press")
	check(knob.supportsAction(&"turn"), "knob advertises turn")
	check(slider.supportsAction(&"adjust"), "slider advertises adjust")
	check(burnButton.supportsAction(&"press"), "manual burn control advertises press")
	check(dumpButton.supportsAction(&"press"), "reaction-mass control advertises press")
	check(seat.supportsAction(&"sit"), "seat advertises sit")
	check(ration.supportsAction(&"eat"), "ration advertises eat")

	await runAction(player, placard, &"inspect", simulation)
	await runAction(player, button, &"press", simulation)
	check(not modules.isArtificialGravityEnabled(), "gravity breaker disables artificial gravity")
	var knobBefore := knob.getNormalizedValue()
	await runAction(player, knob, &"turn", simulation)
	check(knob.getNormalizedValue() > knobBefore, "knob changes the player-entered mass estimate")
	var sliderBefore := slider.getNormalizedValue()
	await runAction(player, slider, &"adjust", simulation)
	check(slider.getNormalizedValue() > sliderBefore, "slider changes shield power")
	await runAction(player, burnKnob, &"turn", simulation)
	var maneuversBefore := simulation.getManeuverCount()
	var propellantBefore := simulation.getShipPropellantMassKg()
	var speedBefore := simulation.getShipRelativeSpeedMps()
	await runAction(player, burnButton, &"press", simulation)
	check(simulation.getManeuverCount() == maneuversBefore + 1, "manual prograde burn reaches the authoritative worker")
	check(simulation.getShipPropellantMassKg() < propellantBefore, "manual burn consumes propellant")
	check(simulation.getShipRelativeSpeedMps() > speedBefore, "manual burn increases prograde encounter speed")
	check(simulation.getLastSpecificEnergyGainJPerKg() > 0.0, "manual burn reports positive Oberth energy gain")
	await runAction(player, dumpSlider, &"adjust", simulation)
	var reactionMassBefore := simulation.getShipReactionMassKg()
	await runAction(player, dumpButton, &"press", simulation)
	check(simulation.getManeuverCount() == maneuversBefore + 2, "reaction-mass dump reaches the authoritative worker")
	check(simulation.getShipReactionMassKg() < reactionMassBefore, "directed matter dump expels reaction mass")
	check(simulation.getLastManeuverDeltaVMps() > 0.0, "directed matter dump produces a measured impulse")
	await runAction(player, seat, &"sit", simulation)
	check(seat.isOccupied(), "seat becomes occupied")
	await runAction(player, seat, &"stand", simulation)
	check(not seat.isOccupied(), "stand action clears seat occupancy")
	await runAction(player, ration, &"eat", simulation)
	check(ration.isConsumed(), "eat action consumes the ration")
	check(simulation.getStateSummary().contains("TRUE") and simulation.getStateSummary().contains("PREDICTION"), "five state channels remain visible")

	slice.queue_free()
	await process_frame
	if failures.is_empty():
		print("All Godot runtime smoke checks passed")
		quit(0)
	else:
		quit(1)
