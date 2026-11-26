extends CharacterBody2D

const RAGDOLL_SCENE: PackedScene = preload("res://scenes/ragdoll.tscn")

@export var run_speed := 270.0
@export var jump_speed := 430.0
@export var gravity := 1200.0
@export var health := 100
@export var shot_damage := 34.0
@export var shot_cooldown := 0.16

var _aim_angle := 0.0
var _shot_timer := 0.0
var _alive := true


func _physics_process(delta: float) -> void:
	if not _alive:
		return

	_shot_timer = maxf(0.0, _shot_timer - delta)
	if not is_on_floor():
		velocity.y += gravity * delta
	elif Input.is_action_just_pressed(&"jump"):
		velocity.y = -jump_speed

	var direction := Input.get_axis(&"move_left", &"move_right")
	velocity.x = direction * run_speed
	move_and_slide()

	_aim_angle = (get_global_mouse_position() - global_position).angle()
	queue_redraw()
	if Input.is_action_pressed(&"shoot") and _shot_timer <= 0.0:
		_shoot()


func _draw() -> void:
	draw_circle(Vector2(0, -16), 9.0, Color("#e6b18b"))
	draw_rect(Rect2(-10, -8, 20, 25), Color("#58a6a6"))
	draw_rect(Rect2(-9, 15, 7, 13), Color("#304b60"))
	draw_rect(Rect2(2, 15, 7, 13), Color("#304b60"))
	draw_set_transform(Vector2(2, -3), _aim_angle)
	draw_rect(Rect2(0, -3, 29, 6), Color("#d6dde5"))
	draw_rect(Rect2(13, -2, 17, 4), Color("#6d7f91"))
	draw_set_transform(Vector2.ZERO, 0.0)


func _shoot() -> void:
	_shot_timer = shot_cooldown
	var direction := Vector2.RIGHT.rotated(_aim_angle)
	var origin := global_position + Vector2(2, -3) + direction * 31.0
	var destination := origin + direction * 1100.0
	var query := PhysicsRayQueryParameters2D.create(origin, destination, 1, [get_rid()])
	var hit := get_world_2d().direct_space_state.intersect_ray(query)
	if not hit.is_empty():
		destination = hit.position
		var collider: Object = hit.collider
		if collider.has_method("take_damage"):
			collider.take_damage(shot_damage, direction)
	get_tree().current_scene.call("show_tracer", origin, destination)


func take_damage(amount: float, direction: Vector2) -> void:
	if not _alive:
		return
	health = maxi(0, health - roundi(amount))
	if health == 0:
		_die(direction)


func _die(direction: Vector2) -> void:
	_alive = false
	set_physics_process(false)
	collision_layer = 0
	collision_mask = 0
	var ragdoll := RAGDOLL_SCENE.instantiate() as Node2D
	ragdoll.position = global_position
	get_parent().add_child(ragdoll)
	ragdoll.call("setup", velocity + direction * 100.0)
	get_tree().current_scene.call("on_player_died")
	queue_free()
