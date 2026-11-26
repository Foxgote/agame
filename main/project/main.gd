extends Node2D

const PLAYER_SCENE: PackedScene = preload("res://scenes/player.tscn")
const TARGET_SCENE: PackedScene = preload("res://scenes/target.tscn")
const BREAKABLE_SCENE: PackedScene = preload("res://scenes/breakable.tscn")

var world: Node2D
var actors: Node2D
var status_label: Label
var player: CharacterBody2D


func _ready() -> void:
	_configure_input()
	_create_world()
	_create_interface()
	queue_redraw()


func _process(_delta: float) -> void:
	if is_instance_valid(player) and is_instance_valid(status_label):
		status_label.text = "MOVE  A/D     JUMP  SPACE     SHOOT  LMB     RESTART  R\n" \
			+ "HEALTH  %d     AMMO  INFINITE\n" % player.get("health") \
			+ "Break the crates; shoot the training dummy to test the ragdoll."


func _unhandled_input(event: InputEvent) -> void:
	if event.is_action_pressed(&"restart"):
		restart()


func _draw() -> void:
	draw_rect(Rect2(-1000, -1000, 4000, 3000), Color("#101a27"))
	for x in range(-800, 2401, 64):
		draw_line(Vector2(x, -1000), Vector2(x, 2000), Color("#172536"), 1.0)
	for y in range(-800, 2001, 64):
		draw_line(Vector2(-1000, y), Vector2(3000, y), Color("#172536"), 1.0)


func _configure_input() -> void:
	_add_key_action(&"move_left", KEY_A)
	_add_key_action(&"move_right", KEY_D)
	_add_key_action(&"jump", KEY_SPACE)
	_add_key_action(&"restart", KEY_R)
	if not InputMap.has_action(&"shoot"):
		InputMap.add_action(&"shoot")
		var shoot_event := InputEventMouseButton.new()
		shoot_event.button_index = MOUSE_BUTTON_LEFT
		InputMap.action_add_event(&"shoot", shoot_event)


func _add_key_action(action: StringName, key: int) -> void:
	if InputMap.has_action(action):
		return
	InputMap.add_action(action)
	var event := InputEventKey.new()
	event.physical_keycode = key
	InputMap.action_add_event(action, event)


func _create_world() -> void:
	world = Node2D.new()
	world.name = "World"
	add_child(world)

	_add_platform(Vector2(640, 660), Vector2(1600, 64))
	_add_platform(Vector2(450, 520), Vector2(240, 24))
	_add_platform(Vector2(790, 430), Vector2(220, 24))
	_add_platform(Vector2(1080, 340), Vector2(200, 24))

	actors = Node2D.new()
	actors.name = "Actors"
	world.add_child(actors)

	var crate_positions := [
		Vector2(470, 475), Vector2(506, 475), Vector2(542, 475),
		Vector2(812, 385), Vector2(848, 385),
	]
	for index in range(crate_positions.size()):
		var crate := BREAKABLE_SCENE.instantiate() as Node2D
		crate.name = "Crate%d" % index
		crate.position = crate_positions[index]
		world.add_child(crate)

	player = PLAYER_SCENE.instantiate() as CharacterBody2D
	player.name = "Player"
	player.position = Vector2(180, 600)
	actors.add_child(player)

	var target := TARGET_SCENE.instantiate() as Node2D
	target.name = "TrainingTarget"
	target.position = Vector2(1080, 296)
	actors.add_child(target)


func _add_platform(position: Vector2, size: Vector2) -> void:
	var platform := StaticBody2D.new()
	platform.position = position
	platform.collision_layer = 1
	platform.collision_mask = 0
	world.add_child(platform)

	var shape := RectangleShape2D.new()
	shape.size = size
	var collision := CollisionShape2D.new()
	collision.shape = shape
	platform.add_child(collision)

	var polygon := Polygon2D.new()
	polygon.polygon = PackedVector2Array([
		Vector2(-size.x * 0.5, -size.y * 0.5),
		Vector2(size.x * 0.5, -size.y * 0.5),
		Vector2(size.x * 0.5, size.y * 0.5),
		Vector2(-size.x * 0.5, size.y * 0.5),
	])
	polygon.color = Color("#34485a")
	platform.add_child(polygon)


func _create_interface() -> void:
	var layer := CanvasLayer.new()
	layer.name = "HUD"
	add_child(layer)

	status_label = Label.new()
	status_label.position = Vector2(20, 18)
	status_label.add_theme_color_override("font_color", Color("#e6edf5"))
	status_label.add_theme_font_size_override("font_size", 17)
	layer.add_child(status_label)


func show_tracer(from: Vector2, to: Vector2) -> void:
	var tracer := Line2D.new()
	tracer.width = 2.0
	tracer.default_color = Color("#f6d27a")
	tracer.points = PackedVector2Array([from, to])
	add_child(tracer)
	get_tree().create_timer(0.055).timeout.connect(tracer.queue_free)


func restart() -> void:
	get_tree().reload_current_scene()


func on_player_died() -> void:
	if is_instance_valid(status_label):
		status_label.text = "DOWN — press R to restart"
