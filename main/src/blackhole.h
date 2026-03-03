#pragma once

// Black hole node: gameplay physics source + simple visual representation.
#include <godot_cpp/classes/mesh_instance3d.hpp>
#include <godot_cpp/classes/node3d.hpp>
#include <godot_cpp/classes/sphere_mesh.hpp>
#include <godot_cpp/core/class_db.hpp>
#include <godot_cpp/variant/vector3.hpp>

using namespace godot;

class BlackHole : public Node3D {
	GDCLASS(BlackHole, Node3D);

protected:
	// Registers methods/properties so they appear in Godot inspector/scripting.
	static void _bind_methods();

private:
	// mu is our mass proxy (larger mu => stronger gravity and larger visual size).
	double mu = 2000.0;
	// Radius where we consider the ship "inside the event horizon".
	double horizon_radius = 2.0;
	// Safety clamp to avoid runaway acceleration values at tiny distances.
	double max_accel = 5000.0;
	// Tuning factor that maps cbrt(mu) to scene radius.
	double visual_radius_scale = 0.08;

	// Runtime-created visual child node (black sphere).
	MeshInstance3D *visual = nullptr;
	// Cached mesh reference so we can update radius quickly.
	Ref<SphereMesh> visual_mesh;

	// Converts mass proxy (mu) to a drawable radius.
	double compute_visual_radius() const;
	// Applies current radius to the visual mesh.
	void update_visual();

public:
	// Creates visual mesh/material when the node enters the scene.
	void _ready() override;

	// Inspector/script property accessors.
	void set_mu(double p_mu);
	double get_mu() const;
	void set_horizon_radius(double p_radius);
	double get_horizon_radius() const;
	void set_max_accel(double p_value);
	double get_max_accel() const;
	void set_visual_radius_scale(double p_value);
	double get_visual_radius_scale() const;

	// Returns gravity acceleration at ship position in world space.
	Vector3 accel_at(const Vector3 &p_ship_pos) const;
	// True if ship is within horizon radius.
	bool is_inside_horizon(const Vector3 &p_ship_pos) const;
};
