extends StaticBody2D

@export var max_health := 100.0
@export var size := Vector2(32, 32)

var health := 100.0
var _broken := false


func _ready() -> void:
	health = max_health
	queue_redraw()


func _draw() -> void:
	var rect := Rect2(-size * 0.5, size)
	draw_rect(rect, Color("#a96e45"))
	draw_rect(rect, Color("#e2af6c"), false, 2.0)
	draw_line(Vector2(-size.x * 0.5, 0), Vector2(size.x * 0.5, 0), Color("#70482f"), 2.0)
	draw_line(Vector2(0, -size.y * 0.5), Vector2(0, size.y * 0.5), Color("#70482f"), 2.0)
	var damage_ratio := 1.0 - health / max_health
	if damage_ratio > 0.25:
		draw_line(Vector2(-7, -7), Vector2(1, 1), Color("#392f2a"), 2.0)
	if damage_ratio > 0.55:
		draw_line(Vector2(1, 1), Vector2(8, -8), Color("#392f2a"), 2.0)
	if damage_ratio > 0.8:
		draw_line(Vector2(1, 1), Vector2(-4, 10), Color("#392f2a"), 2.0)


func take_damage(amount: float, direction: Vector2) -> void:
	if _broken:
		return
	health = maxf(0.0, health - amount)
	queue_redraw()
	if health <= 0.0:
		_break(direction)


func _break(direction: Vector2) -> void:
	_broken = true
	collision_layer = 0
	collision_mask = 0
	for _index in range(6):
		var debris := RigidBody2D.new()
		debris.name = "CrateDebris"
		debris.collision_layer = 1
		debris.collision_mask = 1
		debris.mass = 0.35

		var shape := RectangleShape2D.new()
		shape.size = Vector2(9, 9)
		var collision := CollisionShape2D.new()
		collision.shape = shape
		debris.add_child(collision)

		var polygon := Polygon2D.new()
		polygon.polygon = PackedVector2Array([
			Vector2(-4, -4), Vector2(4, -4), Vector2(4, 4), Vector2(-4, 4),
		])
		polygon.color = Color("#d99a5c")
		debris.add_child(polygon)
		get_parent().add_child(debris)
		debris.global_position = global_position + Vector2(randf_range(-8, 8), randf_range(-8, 8))
		debris.rotation = randf_range(-PI, PI)
		debris.linear_velocity = direction.normalized() * 100.0 \
			+ Vector2(randf_range(-100, 100), randf_range(-180, -40))
		debris.angular_velocity = randf_range(-8.0, 8.0)
		get_tree().create_timer(6.0).timeout.connect(debris.queue_free)
	queue_free()
