#include "spaceship.h"
#include "blackhole.h"

#include <godot_cpp/classes/collision_shape3d.hpp>
#include <godot_cpp/classes/mesh_instance3d.hpp>
#include <godot_cpp/classes/node.hpp>
#include <godot_cpp/classes/sphere_mesh.hpp>
#include <godot_cpp/classes/sphere_shape3d.hpp>

static BlackHole *find_black_hole_recursive(Node *p_node) {
	if (!p_node) {
		return nullptr;
	}

	if (BlackHole *bh = Object::cast_to<BlackHole>(p_node)) {
		return bh;
	}

	const int child_count = p_node->get_child_count();
	for (int i = 0; i < child_count; i++) {
		if (BlackHole *found = find_black_hole_recursive(p_node->get_child(i))) {
			return found;
		}
	}

	return nullptr;
}

void Spaceship::_bind_methods() {
	ClassDB::bind_method(D_METHOD("set_black_hole_path", "path"), &Spaceship::set_black_hole_path);
	ClassDB::bind_method(D_METHOD("get_black_hole_path"), &Spaceship::get_black_hole_path);
	ADD_PROPERTY(PropertyInfo(Variant::NODE_PATH, "black_hole_path", PROPERTY_HINT_NODE_PATH_VALID_TYPES, "BlackHole"), "set_black_hole_path", "get_black_hole_path");
}

void Spaceship::_ready() {
	MeshInstance3D *hull = memnew(MeshInstance3D);
	hull->set_name("Hull");
	Ref<SphereMesh> sphere_mesh;
	sphere_mesh.instantiate();
	sphere_mesh->set_radius(0.6f);
	hull->set_mesh(sphere_mesh);
	add_child(hull);
	
	CollisionShape3D *collision = memnew(CollisionShape3D);
	collision->set_name("Collision");
	Ref<SphereShape3D> sphere_shape;
	sphere_shape.instantiate();
	sphere_shape->set_radius(0.6f);
	collision->set_shape(sphere_shape);
	add_child(collision);

	set_physics_process(true);
}

void Spaceship::_physics_process(double delta) {
	step_accumulator += delta;
	while(step_accumulator>=fixed_dt){
		step_accumulator -= fixed_dt;
		BlackHole *bh = nullptr;
		if (!black_hole_path.is_empty()) {
			bh = Object::cast_to<BlackHole>(get_node_or_null(black_hole_path));
		}
		if (!bh) {
			bh = find_black_hole_recursive(get_parent());
		}
		if(!bh) return;
		Vector3 a = bh->accel_at(get_global_position());
		velocity += a * (real_t)fixed_dt;
		set_global_position(get_global_position() + velocity * (real_t)fixed_dt);
	}
}

void Spaceship::set_black_hole_path(const NodePath &p_path) {
	black_hole_path = p_path;
}

NodePath Spaceship::get_black_hole_path() const {
	return black_hole_path;
}
