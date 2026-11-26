extends SceneTree

var failures: Array[String] = []


func _initialize() -> void:
	call_deferred("_run_smoke_test")


func _check(condition: bool, message: String) -> void:
	if not condition:
		failures.append(message)
		printerr("FAIL: ", message)


func _run_smoke_test() -> void:
	var game_scene := load("res://main.tscn") as PackedScene
	_check(game_scene != null, "main scene loads")
	if game_scene == null:
		quit(1)
		return

	var game := game_scene.instantiate()
	root.add_child(game)
	await process_frame
	await physics_frame

	var player := game.get_node_or_null("World/Actors/Player")
	_check(player != null, "2D player is created")
	_check(game.get_node_or_null("World/Actors/TrainingTarget") != null, "training target is created")

	var crate := game.get_node("World/Crate0") as StaticBody2D
	crate.call("take_damage", 34.0, Vector2.RIGHT)
	_check(is_equal_approx(crate.get("health"), 66.0), "breakable tracks cumulative damage")
	_check(is_instance_valid(crate), "breakable remains until its health is exhausted")
	crate.call("take_damage", 34.0, Vector2.RIGHT)
	crate.call("take_damage", 34.0, Vector2.RIGHT)
	await process_frame
	_check(not is_instance_valid(crate), "breakable is removed after its health reaches zero")

	var target := game.get_node("World/Actors/TrainingTarget") as StaticBody2D
	target.call("take_damage", 100.0, Vector2.RIGHT)
	await process_frame
	var ragdoll := game.get_node_or_null("World/Actors/Ragdoll") as Node2D
	_check(game.get_node_or_null("World/Actors/TrainingTarget") == null, "defeated target is removed")
	_check(ragdoll != null, "defeated target is replaced by a ragdoll")
	if ragdoll == null:
		game.queue_free()
		quit(1)
		return
	await physics_frame
	_check(ragdoll.get_node_or_null("Torso") is RigidBody2D, "ragdoll creates a rigid torso")

	var body_count := 0
	var joint_count := 0
	for child in ragdoll.get_children():
		if child is RigidBody2D:
			body_count += 1
		elif child is PinJoint2D:
			joint_count += 1
			_check(not child.node_a.is_empty() and not child.node_b.is_empty(), "ragdoll joints reference both bodies")
	_check(body_count == 10, "ragdoll has ten connected body parts")
	_check(joint_count == 9, "ragdoll has a joint for each body connection")

	game.queue_free()
	await process_frame
	if failures.is_empty():
		print("2D platformer runtime smoke checks passed")
		quit(0)
	else:
		quit(1)
