extends SceneTree

func _initialize() -> void:
    call_deferred("run")

func run() -> void:
    var args := OS.get_cmdline_user_args()
    var role := args[0]
    var duel := preload("res://LatencyDuel.tscn").instantiate()
    root.add_child(duel)
    duel.find_child("DuelPort", true, false).value = int(args[1])
    button(duel, "DuelHost" if role == "host" else "DuelJoin").pressed.emit()
    var status := label(duel, "DuelConnection")
    var expected := "Hosting" if role == "host" else "Joined"
    if not await until(func(): return status.text == expected):
        fail(role, "Connection failed: " + status.text)
        return

    var your_damage := label(duel, "DuelYourDamage")
    var their_damage := label(duel, "DuelTheirDamage")
    var clock := label(duel, "DuelClock")
    if role == "host":
        button(duel, "DuelStart").pressed.emit()
        if not await until(func(): return clock.text.ends_with("LIVE") and clock.text.to_float() >= 0.08):
            fail(role, "Host did not start the clock")
            return
        tap(KEY_O)
        if not await until(func(): return your_damage.text.begins_with("DAMAGE 24  ")):
            fail(role, "Guest did not land their earned special: " + your_damage.text)
            return
        tap(KEY_O)
        if not await until(func(): return status.text == "Guest left - waiting"):
            fail(role, "Guest did not observe the host's follow-up and disconnect")
            return
    else:
        if not await until(func(): return clock.text.ends_with("LIVE") and clock.text.to_float() >= 0.28):
            fail(role, "Guest clock did not reach the defense beat")
            return
        tap(KEY_P)
        tap(KEY_O)
        if not await until(func(): return their_damage.text.begins_with("DAMAGE 24  ")):
            fail(role, "Guest did not see their own special land: " + their_damage.text)
            return
        if not await until(func(): return your_damage.text.begins_with("DAMAGE 12  ")):
            fail(role, "Guest did not see the host's attack land: " + your_damage.text)
            return
        if not await until(func(): return label(duel, "DuelNetwork").text.contains(" ms")):
            fail(role, "Guest did not see RTT")
            return
        button(duel, "DuelLeave").pressed.emit()
        await create_timer(0.25).timeout
    print("Latency duel ", role, " smoke passed")
    quit(0)

func until(condition: Callable) -> bool:
    var deadline := Time.get_ticks_msec() + 8000
    while Time.get_ticks_msec() < deadline:
        if condition.call():
            return true
        await process_frame
    return false

func label(duel: Node, id: String) -> Label:
    return duel.find_child(id, true, false) as Label

func button(duel: Node, id: String) -> Button:
    return duel.find_child(id, true, false) as Button

func tap(key: Key) -> void:
    for pressed in [true, false]:
        var event := InputEventKey.new()
        event.keycode = key
        event.pressed = pressed
        root.push_input(event)

func fail(role: String, message: String) -> void:
    push_error(role + ": " + message)
    quit(1)
