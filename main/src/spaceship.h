#pragma once

#include <godot_cpp/classes/character_body3d.hpp>
#include <godot_cpp/core/class_db.hpp>
#include <godot_cpp/variant/node_path.hpp>

using namespace godot;

class Spaceship : public CharacterBody3D {
	GDCLASS(Spaceship, CharacterBody3D);

protected:
	static void _bind_methods();
private:
	// Ship velocity in world space.
	Vector3 velocity = Vector3(0,0,0);
	NodePath black_hole_path;
	double fixed_dt = 1.0/120.0;
	double step_accumulator = 0.0;


public:
	void _ready() override;
	void _physics_process(double delta) override;

	void set_black_hole_path(const NodePath &p_path);
	NodePath get_black_hole_path() const;
};
