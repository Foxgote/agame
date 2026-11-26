extends Node2D

const BODY_COLOR := Color("#d47e69")


func setup(inherited_velocity: Vector2) -> void:
	var torso := _add_segment("Torso", Vector2(0, 0), Vector2(18, 30), BODY_COLOR, inherited_velocity)
	var head := _add_circle("Head", Vector2(0, -24), 10.0, Color("#d9ac8b"), inherited_velocity)
	var left_upper_arm := _add_segment("LeftUpperArm", Vector2(-17, -4), Vector2(18, 8), BODY_COLOR, inherited_velocity)
	var left_lower_arm := _add_segment("LeftLowerArm", Vector2(-31, 4), Vector2(17, 7), BODY_COLOR, inherited_velocity)
	var right_upper_arm := _add_segment("RightUpperArm", Vector2(17, -4), Vector2(18, 8), BODY_COLOR, inherited_velocity)
	var right_lower_arm := _add_segment("RightLowerArm", Vector2(31, 4), Vector2(17, 7), BODY_COLOR, inherited_velocity)
	var left_upper_leg := _add_segment("LeftUpperLeg", Vector2(-6, 22), Vector2(9, 21), Color("#76607a"), inherited_velocity)
	var left_lower_leg := _add_segment("LeftLowerLeg", Vector2(-7, 42), Vector2(8, 20), Color("#76607a"), inherited_velocity)
	var right_upper_leg := _add_segment("RightUpperLeg", Vector2(6, 22), Vector2(9, 21), Color("#76607a"), inherited_velocity)
	var right_lower_leg := _add_segment("RightLowerLeg", Vector2(7, 42), Vector2(8, 20), Color("#76607a"), inherited_velocity)

	_connect_bodies(torso, head, Vector2(0, -13))
	_connect_bodies(torso, left_upper_arm, Vector2(-9, -7))
	_connect_bodies(left_upper_arm, left_lower_arm, Vector2(-24, 0))
	_connect_bodies(torso, right_upper_arm, Vector2(9, -7))
	_connect_bodies(right_upper_arm, right_lower_arm, Vector2(24, 0))
	_connect_bodies(torso, left_upper_leg, Vector2(-5, 14))
	_connect_bodies(left_upper_leg, left_lower_leg, Vector2(-7, 32))
	_connect_bodies(torso, right_upper_leg, Vector2(5, 14))
	_connect_bodies(right_upper_leg, right_lower_leg, Vector2(7, 32))


func _add_segment(
	part_name: String,
	part_position: Vector2,
	part_size: Vector2,
	color: Color,
	inherited_velocity: Vector2
) -> RigidBody2D:
	var body := RigidBody2D.new()
	body.name = part_name
	body.position = part_position
	body.mass = 0.8
	body.linear_velocity = inherited_velocity + Vector2(randf_range(-25, 25), -25)
	body.angular_velocity = randf_range(-4.0, 4.0)
	add_child(body)

	var shape := RectangleShape2D.new()
	shape.size = part_size
	var collision := CollisionShape2D.new()
	collision.shape = shape
	body.add_child(collision)

	var polygon := Polygon2D.new()
	polygon.polygon = PackedVector2Array([
		Vector2(-part_size.x * 0.5, -part_size.y * 0.5),
		Vector2(part_size.x * 0.5, -part_size.y * 0.5),
		Vector2(part_size.x * 0.5, part_size.y * 0.5),
		Vector2(-part_size.x * 0.5, part_size.y * 0.5),
	])
	polygon.color = color
	body.add_child(polygon)
	return body


func _add_circle(
	part_name: String,
	part_position: Vector2,
	radius: float,
	color: Color,
	inherited_velocity: Vector2
) -> RigidBody2D:
	var body := RigidBody2D.new()
	body.name = part_name
	body.position = part_position
	body.mass = 0.5
	body.linear_velocity = inherited_velocity + Vector2(randf_range(-20, 20), -25)
	body.angular_velocity = randf_range(-3.0, 3.0)
	add_child(body)

	var shape := CircleShape2D.new()
	shape.radius = radius
	var collision := CollisionShape2D.new()
	collision.shape = shape
	body.add_child(collision)

	var points := PackedVector2Array()
	for index in range(12):
		var angle := TAU * float(index) / 12.0
		points.append(Vector2(cos(angle), sin(angle)) * radius)
	var polygon := Polygon2D.new()
	polygon.polygon = points
	polygon.color = color
	body.add_child(polygon)
	return body


func _connect_bodies(body_a: RigidBody2D, body_b: RigidBody2D, anchor: Vector2) -> void:
	var joint := PinJoint2D.new()
	joint.name = "%s_%s_Joint" % [body_a.name, body_b.name]
	joint.position = anchor
	joint.disable_collision = true
	add_child(joint)
	joint.node_a = joint.get_path_to(body_a)
	joint.node_b = joint.get_path_to(body_b)
