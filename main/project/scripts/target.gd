extends StaticBody2D

const RAGDOLL_SCENE: PackedScene = preload("res://scenes/ragdoll.tscn")

@export var health := 100.0

var _defeated := false


func _draw() -> void:
	draw_circle(Vector2(0, -14), 9.0, Color("#d9ac8b"))
	draw_rect(Rect2(-10, -6, 20, 26), Color("#cf6b62"))
	draw_rect(Rect2(-9, 19, 7, 13), Color("#76566d"))
	draw_rect(Rect2(2, 19, 7, 13), Color("#76566d"))
	draw_line(Vector2(-10, 4), Vector2(10, 4), Color("#722f39"), 2.0)


func take_damage(amount: float, direction: Vector2) -> void:
	if _defeated:
		return
	health = maxf(0.0, health - amount)
	if health <= 0.0:
		_die(direction)


func _die(direction: Vector2) -> void:
	_defeated = true
	collision_layer = 0
	collision_mask = 0
	var ragdoll := RAGDOLL_SCENE.instantiate() as Node2D
	ragdoll.position = global_position
	get_parent().add_child(ragdoll)
	ragdoll.call("setup", direction * 150.0)
	queue_free()
