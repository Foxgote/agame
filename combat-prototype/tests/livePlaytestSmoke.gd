extends SceneTree

var failed := false

func _initialize() -> void:
    call_deferred("run")

func check(ok: bool, message: String) -> void:
    if not ok:
        push_error(message)
        failed = true

func run() -> void:
    var dir := OS.get_environment("COMBAT_LIVE_DIR")
    if dir.is_empty():
        push_error("Set COMBAT_LIVE_DIR to a temporary test directory")
        quit(1)
        return
    DirAccess.make_dir_recursive_absolute(dir)
    write_settings(dir, 1, 0.88, 6)
    var lab := preload("res://Main.tscn").instantiate()
    root.add_child(lab)
    await create_timer(1.15).timeout
    var first := read_status(dir)
    check(first.get("appliedRevision") == 1 and first.get("paused") == true,
        "Opt-in lab should apply the first profile and start paused")
    check(first.get("player", {}).get("stamina") == 100, "The live starting reserve should apply before the first frame")
    var pause: BaseButton = null
    for button in lab.find_children("*", "BaseButton", true, false):
        if button.text.begins_with("Pause"):
            pause = button
            break
    if pause == null:
        quit(1)
        return
    pause.button_pressed = false
    write_settings(dir, 2, 0.61, -1)
    await create_timer(1.30).timeout
    var invalid := read_status(dir)
    check(invalid.get("appliedRevision") == 1 and invalid.get("error") != null,
        "Out-of-range settings must not replace the active profile")
    write_settings(dir, 3, 0.36, 0, 0.20)
    await create_timer(2.30).timeout
    var updated := read_status(dir)
    check(updated.get("appliedRevision") == 3 and updated.get("error") == null,
        "The running lab should acknowledge a new valid profile")
    check(is_equal_approx(float(updated.get("settings", {}).get("cadence", 0)), 0.36),
        "The applied profile must report the new cadence")
    check(is_equal_approx(float(updated.get("opponentLightStartup", 0)), 0.27) and
        updated.get("interruptHeavyStartup") == false,
        "The applied profile must report the faster bot Light and disabled interruptions")
    check(is_equal_approx(float(updated.get("sharedAttackWindup", 0)), 0.28) and
        is_equal_approx(float(updated.get("mixups", {}).get("comboChance", 0)), 0.45),
        "The live profile must acknowledge matched attacks and the trained mixup settings")
    check(updated.get("round") == first.get("round") and
        float(updated.get("combatTime", 0)) > float(invalid.get("combatTime", 0)),
        "Live tuning should not reset or pause the fight")
    check(not updated.get("recent", []).is_empty(), "The status should contain actual exchanges")
    pause.button_pressed = true
    for tap in range(64):
        tap_light()
    await create_timer(1.25).timeout
    var bought := read_status(dir)
    var player: Dictionary = bought.get("player", {})
    var input: Dictionary = bought.get("attackInput", {})
    check(input.get("taps") == 64 and input.get("accepted") == 64 and input.get("lastTapBlock") == null,
        "Every O tap must be accepted while paused, even when all taps arrive in one frame")
    check(player.get("queuedAttacks", 0) + (1 if player.get("attack") == "Light" else 0) == 64,
        "All 64 Lights should be in the backlog or winding up")
    var ai_queue: Label = lab.find_child("AiQueuedCount", true, false)
    check(ai_queue != null and ai_queue.is_visible_in_tree() and
        ai_queue.text == "YOUR QUEUE  %d" % player.get("queuedAttacks", 0),
        "The AI chart must show the queued Lights without waiting for the clock")
    var player_queue: Label = lab.find_child("PlayerQueuedCount", true, false)
    var player_next: Label = lab.find_child("PlayerNextAttack", true, false)
    var ai_pending: int = bought.get("opponent", {}).get("queuedAttacks", 0)
    check(player_queue != null and player_queue.is_visible_in_tree() and player_queue.size.x >= 130 and
        player_queue.text == "AI QUEUE  %d/8" % ai_pending,
        "The player's queue row must remain readable even when the AI has nothing waiting")
    check(bought.get("economy", {}).get("lightCost") == 0 and player.get("stamina") == 100,
        "A zero-cost live Light must not reserve stamina")
    check(is_equal_approx(float(bought.get("playerLightStartup", 0)), 0.20),
        "The live status should report the applied player windup")
    pause.button_pressed = false
    await create_timer(2.4).timeout
    var chain := read_status(dir)
    check(outcomes(chain) >= 3 and chain.get("player", {}).get("stamina") == 100,
        "The queued Lights must keep resolving without spending stamina")
    var burst_notes: Control = visible_lane(lab, "AiChart").get_node("BurstNotes")
    check(is_equal_approx(float(chain.get("lightLaunchInterval", 0)), 0.04) and
        chain.get("player", {}).get("attacksInFlight", 0) > 1 and burst_notes.is_visible_in_tree(),
        "The AI chart should show multiple launched Lights in the 40 ms burst mode")
    check(ai_queue != null and ai_queue.text != "YOUR QUEUE  %d" % player.get("queuedAttacks", 0),
        "The AI preview must drain as queued attacks launch")
    pause.button_pressed = true
    write_settings(dir, 4, 0.36, 0, 0.20, true)
    await create_timer(1.25).timeout
    var reset: BaseButton = null
    for button in lab.find_children("*", "BaseButton", true, false):
        if button.text.begins_with("Reset"):
            reset = button
            break
    if reset != null:
        reset.pressed.emit()
    pause.button_pressed = true
    for tap in range(10):
        tap_light()
    await create_timer(1.25).timeout
    var capped := read_status(dir)
    check(capped.get("appliedRevision") == 4 and capped.get("queueLimit") == 4 and
        is_equal_approx(float(capped.get("specialCounterRecovery", 0)), 0.12) and
        capped.get("clearQueueOnSpecialDefense") == true,
        "The live profile must acknowledge four slots, 120 ms special recovery, and queue clears")
    check(capped.get("player", {}).get("queuedAttacks") == 4 and capped.get("player", {}).get("attack") == "Light" and
        capped.get("attackInput", {}).get("accepted") == 69,
        "Four Lights may wait behind the current swing; extra taps must not expand the backlog")
    var step: BaseButton = null
    for button in lab.find_children("*", "BaseButton", true, false):
        if button.text.begins_with("Step"):
            step = button
            break
    check(reset != null and step != null and is_equal_approx(float(capped.get("opponentLookahead", 0)), 0.24),
        "The capped session should enable the bot's planning window")
    if reset != null and step != null:
        reset.pressed.emit()
        pause.button_pressed = true
        for frame in range(16):
            step.pressed.emit()
        var incoming_note: Control = visible_lane(lab, "PlayerChart").get_node("IncomingNote")
        check(player_queue.is_visible_in_tree() and player_queue.text == "AI QUEUE  1/4" and
            player_next.text == "NEXT: LIGHT" and not incoming_note.is_visible_in_tree(),
            "The committed bot Light should be named in the queue before launch")
        for frame in range(12):
            step.pressed.emit()
        check(incoming_note.is_visible_in_tree(), "The queued bot attack should become a falling note on launch")
    write_settings(dir, 5, 0.36, 0, 0.20, true, true)
    await create_timer(1.25).timeout
    var no_recovery := read_status(dir)
    check(no_recovery.get("appliedRevision") == 5 and no_recovery.get("specialCounterRecovery") == 0 and
        no_recovery.get("specialAttackerRecovery") == 0 and no_recovery.get("specialDefenderRecovery") == 0,
        "The no-recovery profile must disable all three special recovery values")
    write_settings(dir, 6, 0.36, 0, 0.20, true, false,
        {"windup": 0.32, "contactRecovery": 0.08, "avoidedRecovery": 0.12}, 0.18, 0.0585)
    await create_timer(1.25).timeout
    var commitment := read_status(dir)
    check(commitment.get("appliedRevision") == 6 and is_equal_approx(float(commitment.get("specialWindup", 0)), 0.32) and
        is_equal_approx(float(commitment.get("specialAttackerRecovery", 0)), 0.08) and
        is_equal_approx(float(commitment.get("specialCounterRecovery", 0)), 0.12) and
        commitment.get("specialDefenderRecovery") == 0,
        "The commitment profile must report its effective windup and attacker-only recovery")
    check(is_equal_approx(float(commitment.get("specialLaunchInterval", 0)), 0.18) and
        is_equal_approx(float(commitment.get("lightLaunchInterval", 0)), 0.0585) and
        is_equal_approx(float(commitment.get("speed", 0)), 0.605),
        "The live profile should report the distinct special gap and the new default speed")
    tap_light()
    lab.queue_free()
    await process_frame
    var trail := FileAccess.get_file_as_string(dir.path_join("exchange.jsonl"))
    var kept_older_round := false
    var kept_last_attack := false
    for line in trail.split("\n", false):
        var entry: Variant = JSON.parse_string(line)
        if not entry is Dictionary:
            check(false, "Exchange history must contain valid JSON records")
            continue
        kept_older_round = kept_older_round or (entry.get("round") == 1 and entry.get("kind") == "AttackResolved")
        kept_last_attack = kept_last_attack or (entry.get("round") == 3 and
            str(entry.get("message", "")).begins_with("Player queued Mid Light"))
    check(kept_older_round and kept_last_attack,
        "Exchange history must retain earlier rounds and flush the last attack on close")
    if not failed:
        print("Live playtest smoke passed: uncapped and four-attack profiles, persistent Exchange history")
    quit(1 if failed else 0)

func visible_lane(lab: Node, chart_name: String) -> Control:
    for lane in lab.find_child(chart_name, true, false).find_children("LaneSurface", "Control", true, false):
        if lane.is_visible_in_tree():
            return lane
    return null

func tap_light() -> void:
    var attack := InputEventKey.new()
    attack.keycode = KEY_O
    attack.pressed = true
    root.push_input(attack)
    var release := InputEventKey.new()
    release.keycode = KEY_O
    root.push_input(release)

func outcomes(snapshot: Dictionary) -> int:
    var defender: Dictionary = snapshot.get("opponent", {})
    return int(defender.get("hitsTaken", 0) + defender.get("blocks", 0) +
        defender.get("parries", 0) + defender.get("dodges", 0) + defender.get("incomingWhiffs", 0))

func write_settings(dir: String, revision: int, cadence: float, light_cost: float, startup: float = 0.32,
        capped: bool = false, no_special_recovery: bool = false, special_timing: Variant = null,
        special_launch_interval: Variant = null, light_launch_interval: float = 0.04) -> void:
    var path := dir.path_join("opponent.json")
    var file := FileAccess.open(path, FileAccess.WRITE)
    file.store_string(JSON.stringify({
        "revision": revision, "name": "Smoke profile", "playerLightStartup": startup, "burstLights": light_cost == 0,
        "opponentLightStartup": (0.27 if light_cost == 0 else 0.32), "interruptHeavyStartup": light_cost != 0,
        "sharedAttackWindup": 0.28, "lightLaunchInterval": light_launch_interval, "mixups": {"comboChance": 0.45, "comboGap": 0.12,
            "counterUseChance": 0.85, "parryBias": 0.55},
        "queueLimit": (4 if capped else null), "specialCounterRecovery": (0.12 if capped else null),
        "opponentLookahead": (0.24 if capped else 0),
        "clearQueueOnSpecialDefense": capped,
        "disableSpecialRecovery": no_special_recovery,
        "specialTiming": special_timing,
        "specialLaunchInterval": special_launch_interval,
        "settings": {"cadence": cadence, "cadenceSpread": 0.27, "reactionDelay": 0.23,
            "readChance": 0.72, "timingSpread": 0.09},
        "economy": {"startingStamina": 100, "lightCost": light_cost, "heavyCost": (0 if light_cost == 0 else 12),
            "regeneration": 15, "defenseReward": 10}
    }))
    file.close()

func read_status(dir: String) -> Dictionary:
    var parsed: Variant = JSON.parse_string(FileAccess.get_file_as_string(dir.path_join("status.json")))
    return parsed if parsed is Dictionary else {}
