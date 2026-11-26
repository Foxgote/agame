extends SceneTree

var failed := false

func _initialize() -> void:
    call_deferred("run")

func check(ok: bool, message: String) -> void:
    if not ok:
        push_error(message)
        failed = true

func run() -> void:
    var lab := preload("res://Main.tscn").instantiate()
    root.add_child(lab)
    for frame in range(5):
        await process_frame
    var views := lab.find_child("Views", true, false) as TabContainer
    if views == null:
        push_error("The C# lab did not load")
        quit(1)
        return
    var mode := views.find_child("TrialMode", true, false) as OptionButton
    var light_startup := views.find_child("LightStartup", true, false) as HSlider
    check(is_equal_approx(light_startup.value, 0.320), "Counter trial slider should open at 320 ms")
    root.size = Vector2i(800, 500)
    await process_frame
    var controls := views.find_child("CounterControls", true, false)
    check(controls.is_visible_in_tree(), "Counter trial should open by default")
    var spacing := views.find_child("Spacing", true, false) as Control
    var range_status := spacing.find_child("RangeStatus", true, false) as Label
    var opponent_scan := spacing.find_child("OpponentScan", true, false) as Label
    for frame in range(5):
        await process_frame
    var lanes_visible := 0
    for lane in views.find_children("LaneSurface", "Control", true, false):
        if lane.is_visible_in_tree():
            lanes_visible += 1
            check(root.get_visible_rect().grow(1).encloses(lane.get_global_rect()),
                "Single lane is clipped: %s within %s" % [lane.get_global_rect(), root.get_visible_rect()])
    check(lanes_visible == 2 and spacing.is_visible_in_tree(), "Expected one visible lane per fighter and the spacing strip")
    var pause := find_button(lab, "Pause [Esc]")
    var step := find_button(lab, "Step")
    var speed := lab.find_child("PlaySpeed", true, false) as Button
    var clock := lab.find_child("CombatClock", true, false) as Label
    var exchange := views.get_tab_control(2).find_children("*", "RichTextLabel", true, false)[0] as RichTextLabel
    check(speed.text == "0.605x [T]", "Counter trial should open at 60.5% speed")
    send_key(KEY_T, true)
    send_key(KEY_T, false)
    check(speed.text == "1x [T]", "T did not restore full speed")
    speed.pressed.emit()
    check(speed.text == "0.605x [T]", "Speed button did not restore slow speed")
    find_button(lab, "Reset [R]").pressed.emit()
    var elapsed := 0.0
    while elapsed < 0.30:
        await process_frame
        elapsed += lab.get_process_delta_time()
    var combat_time := clock.text.get_slice("  ", 1).trim_suffix("s").to_float()
    check(absf(combat_time - elapsed * 0.605) < 0.06, "The live combat clock did not advance at 60.5% speed")
    find_button(lab, "Reset [R]").pressed.emit()
    pause.button_pressed = true
    # First Light: 450 ms AI delay + 320 ms startup.
    for tick in range(46):
        step.pressed.emit()
    check(opponent_scan.text.ends_with("Light"), "Windup should show the incoming Light scan")
    send_key(KEY_P, true)
    send_key(KEY_P, false)
    step.pressed.emit()
    check(opponent_scan.text.ends_with("IN REACH"), "Impact scan should show reach before defense resolves")
    for tick in range(6):
        step.pressed.emit()
    check(controls.get_child(0).text.begins_with("Unparryable [O]"), "P did not parry into a counter cue: " + read_exchange(views, exchange))
    check(opponent_scan.text.ends_with("PARRIED"), "Confirmed parry should show on the scan")
    send_key(KEY_O, true)
    send_key(KEY_O, false)
    step.pressed.emit()
    check(read_exchange(views, exchange).contains("Player queued Mid Unparryable"), "O did not queue the single-lane parry counter")

    find_button(lab, "Reset [R]").pressed.emit()
    pause.button_pressed = true
    send_key(KEY_P, true)
    for tick in range(53):
        step.pressed.emit()
    check(find_button(lab, "Block [P]") != null, "Held P should show block")
    check(opponent_scan.text.ends_with("BLOCKED"), "Holding P early should block the incoming Light")
    check(controls.get_child(0).text == "Light [O]", "Blocking should not open a counter")
    send_key(KEY_P, false)
    step.pressed.emit()
    check(find_button(lab, "Parry [P]") != null, "Releasing P should drop block")

    find_button(lab, "Reset [R]").pressed.emit()
    pause.button_pressed = true
    for tick in range(46):
        step.pressed.emit()
    send_key(KEY_SHIFT, true)
    send_key(KEY_SHIFT, false)
    check(not read_exchange(views, exchange).contains("Player activated Dodge"), "Shift should no longer dodge")
    check(find_button(lab, "Dodge [I]") != null, "The dodge button should advertise I")
    send_key(KEY_I, true)
    send_key(KEY_I, false)
    send_key(KEY_O, true)
    send_key(KEY_O, false)
    step.pressed.emit()
    check(controls.get_child(0).text == "Follow-up buffered", "An early O should wait for the dodge result")
    for tick in range(6):
        step.pressed.emit()
    check(read_exchange(views, exchange).contains("Player queued Mid Undodgeable"),
        "Early O did not queue the single-lane dodge counter")

    var cadence := views.find_child("AiCadence", true, false) as HSlider
    cadence.value = 0.2
    find_button(lab, "Reset [R]").pressed.emit()
    pause.button_pressed = true
    for tick in range(46):
        step.pressed.emit()
    send_key(KEY_P, true)
    send_key(KEY_P, false)
    for tick in range(7):
        step.pressed.emit()
    send_key(KEY_O, true)
    send_key(KEY_O, false)
    var ai_chart := views.find_child("AiChart", true, false)
    var ghost: Control = null
    for tick in range(32):
        step.pressed.emit()
        ghost = ai_chart.find_child("InterruptedNote", true, false) as Control
        if ghost != null:
            break
    check(ghost != null and ghost.get_child(0).text == "INTERRUPTED",
        "An interrupted Heavy should leave a labeled visual ghost")
    if ghost != null:
        var before_fade := ghost.modulate.a
        await create_timer(0.10).timeout
        check(is_instance_valid(ghost) and ghost.modulate.a < before_fade, "The interrupted note should fade")
        await create_timer(0.25).timeout
        check(not is_instance_valid(ghost), "The interrupted note should clear")
    cadence.value = 0.72

    find_button(lab, "Reset [R]").pressed.emit()
    pause.button_pressed = true
    for tick in range(28):
        step.pressed.emit()
    send_key(KEY_A, true)
    for tick in range(25):
        step.pressed.emit()
    send_key(KEY_A, false)
    check(read_exchange(views, exchange).contains("WHIFFED"), "Backing away during startup should make the Light miss")
    check(opponent_scan.text.ends_with("MISS"), "A whiff should show on the scan")
    check(range_status.text == "HEAVY RANGE", "Retreat should reach the Heavy-only band")
    send_key(KEY_D, true)
    for tick in range(12):
        step.pressed.emit()
    send_key(KEY_D, false)
    check(range_status.text == "LIGHT RANGE", "Advancing should take you back into Light range")
    check(opponent_scan.text.is_empty(), "The impact flash should clear")
    check(not read_exchange(views, exchange).contains("Player activated"), "Movement must not trigger a defense input")

    var damage_labels := views.find_children("DamageTaken", "Label", true, false)
    check(damage_labels.size() == 2, "Both fighters need their own damage counter")
    find_button(lab, "Reset [R]").pressed.emit()
    pause.button_pressed = true
    for tick in range(53):
        step.pressed.emit()
    check(damage_labels[0].text == "DAMAGE TAKEN  12", "The first unguarded Light should add 12")
    check(damage_labels[1].text == "DAMAGE TAKEN  0", "The untouched opponent should still show zero")
    find_button(lab, "Reset [R]").pressed.emit()
    check(damage_labels[0].text == "DAMAGE TAKEN  0", "Reset did not clear damage")
    pause.button_pressed = true
    send_key(KEY_O, true)
    send_key(KEY_O, false)
    step.pressed.emit()
    check(read_exchange(views, exchange).contains("Player queued Mid Light"), "O should buy a Light in neutral")

    find_button(lab, "Reset [R]").pressed.emit()
    pause.button_pressed = true
    for tick in range(3200):
        step.pressed.emit()
    check(exchange.get_parsed_text().is_empty(), "Hidden Exchange should not format combat events")
    views.current_tab = 2
    var log_lines := exchange.get_parsed_text().split("\n", false)
    check(log_lines.size() == 280 and not log_lines[0].contains("Round ready"),
        "Opening Exchange should load only the newest 280 events")
    for tick in range(110):
        step.pressed.emit()
    var advanced_log := exchange.get_parsed_text().split("\n", false)
    check(advanced_log.size() == 280 and advanced_log[-1] != log_lines[-1] and advanced_log[0] != log_lines[0],
        "The visible log should append recent events and trim the oldest ones")
    views.current_tab = 0

    mode.select(1)
    mode.item_selected.emit(1)
    check(not speed.visible, "Classic charts should not show Counter trial speed controls")
    check(not spacing.visible, "Classic charts should keep their original layout")
    check(is_equal_approx(light_startup.value, 0.180), "Classic Light startup should remain 180 ms")
    for window_size in [Vector2i(800, 500), Vector2i(1152, 680), Vector2i(1390, 900)]:
        root.size = window_size
        for frame in range(5):
            await process_frame
        var player := views.find_child("PlayerChart", true, false) as Control
        var opponent := views.find_child("AiChart", true, false) as Control
        var a := player.get_global_rect()
        var b := opponent.get_global_rect()
        check(a.end.x <= b.position.x and abs(a.position.y - b.position.y) < 1, "Charts must sit side by side")
        check(abs(a.size.x - b.size.x) <= 2, "Chart widths must match")
        var screen := root.get_visible_rect().grow(1)
        var lanes := views.find_children("LaneSurface", "Control", true, false)
        check(lanes.size() == 6, "Expected six lanes")
        for lane in lanes:
            var r: Rect2 = lane.get_global_rect()
            check(screen.encloses(r) and r.size.y > window_size.y * 0.45, "Lane clipped or too short: %s at %s" % [r, window_size])
        for button in views.find_child("CombatControls", true, false).find_children("*", "BaseButton", true, false):
            check(screen.encloses(button.get_global_rect()), "Offscreen combat button: " + button.text)
        print("Charts at ", window_size, ": lane ", lanes[0].size)
    for tab in range(1, views.get_tab_count()):
        views.current_tab = tab
        await process_frame
        check(views.get_current_tab_control().is_visible_in_tree(), "Lab tab inaccessible")
    views.current_tab = 0
    var buttons := lab.find_children("*", "BaseButton", true, false)
    var high: Button
    var block: BaseButton
    for button in buttons:
        if button.text == "Reset [R]":
            button.pressed.emit()
        if button.text == "Block":
            block = button
        if button.text == "High":
            high = button
    pause.button_pressed = true
    block.button_pressed = true
    var player_high := views.find_child("PlayerChart", true, false).find_child("LaneSurface", true, false) as Control
    var idle_count := player_high.get_child_count()
    high.pressed.emit()
    check(player_high.get_child_count() == idle_count + 1, "Buffered input did not flash on the owner's high lane")
    var flash := player_high.get_child(-1) as ColorRect
    if flash != null:
        var start_y := flash.position.y
        check(flash.modulate.a > 0 and flash.modulate.a <= 0.501, "Flash must start translucent")
        for frame in range(2):
            await process_frame
        check(is_instance_valid(flash) and flash.position.y < start_y, "Flash did not rise while the fight was paused")
        var ai_notes := views.find_child("AiChart", true, false).find_children("IncomingNote", "Control", true, false)
        check(not ai_notes[0].visible, "Held purchase must not display a solid attack on the defender")
        await create_timer(0.25).timeout
        check(not is_instance_valid(flash), "Flash outlived its confirmation interval")
        for tick in range(28):
            step.pressed.emit()
        block.button_pressed = false
        high.pressed.emit()
        await process_frame
        await process_frame
        check(ai_notes[0].visible, "Launched High attack missing from AI chart")
    if OS.get_cmdline_user_args().has("--preview") and DisplayServer.get_name() != "headless":
        mode.select(0)
        mode.item_selected.emit(0)
        pause.button_pressed = true
        for tick in range(53):
            step.pressed.emit()
        await RenderingServer.frame_post_draw
        var path := OS.get_cache_dir().path_join("combat-chart-preview.png")
        root.get_texture().get_image().save_png(path)
        print("Preview: ", path)
    lab.queue_free()
    await process_frame
    quit(1 if failed else 0)

func send_key(key: Key, pressed: bool) -> void:
    var event := InputEventKey.new()
    event.keycode = key
    event.pressed = pressed
    root.push_input(event)

func read_exchange(views: TabContainer, exchange: RichTextLabel) -> String:
    var tab := views.current_tab
    views.current_tab = 2
    var log_text := exchange.get_parsed_text()
    views.current_tab = tab
    return log_text

func find_button(parent: Node, prefix: String) -> BaseButton:
    for button in parent.find_children("*", "BaseButton", true, false):
        if button.text.begins_with(prefix):
            return button
    return null
