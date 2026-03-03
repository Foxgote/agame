#include "blackhole.h"

#include <godot_cpp/classes/mesh_instance3d.hpp>
#include <godot_cpp/classes/sphere_mesh.hpp>
#include <godot_cpp/classes/standard_material3d.hpp>

void BlackHole::_bind_methods() {
	// Expose setters/getters so they are editable in Godot and callable from script.
	ClassDB::bind_method(D_METHOD("set_mu", "mu"), &BlackHole::set_mu);
	ClassDB::bind_method(D_METHOD("get_mu"), &BlackHole::get_mu);
	ClassDB::bind_method(D_METHOD("set_horizon_radius", "radius"), &BlackHole::set_horizon_radius);
	ClassDB::bind_method(D_METHOD("get_horizon_radius"), &BlackHole::get_horizon_radius);
	ClassDB::bind_method(D_METHOD("set_max_accel", "max_accel"), &BlackHole::set_max_accel);
	ClassDB::bind_method(D_METHOD("get_max_accel"), &BlackHole::get_max_accel);
	ClassDB::bind_method(D_METHOD("set_visual_radius_scale", "scale"), &BlackHole::set_visual_radius_scale);
	ClassDB::bind_method(D_METHOD("get_visual_radius_scale"), &BlackHole::get_visual_radius_scale);
	ClassDB::bind_method(D_METHOD("accel_at", "ship_pos"), &BlackHole::accel_at);
	ClassDB::bind_method(D_METHOD("is_inside_horizon", "ship_pos"), &BlackHole::is_inside_horizon);

	// Inspector-visible properties.
	ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "mu"), "set_mu", "get_mu");
	ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "horizon_radius"), "set_horizon_radius", "get_horizon_radius");
	ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "max_accel"), "set_max_accel", "get_max_accel");
	ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "visual_radius_scale"), "set_visual_radius_scale", "get_visual_radius_scale");
}

double BlackHole::compute_visual_radius() const {
	// Radius grows with cbrt(mu): smooth growth without exploding size at high mu.
	double r = Math::pow(mu, 1.0 / 3.0) * visual_radius_scale;
	// Ensure a minimum visible size.
	return Math::max(r, 0.1);
}

void BlackHole::update_visual() {
	// Safe guard if _ready hasn't built the visual yet.
	if (!visual || visual_mesh.is_null()) {
		return;
	}
	// Keep visual radius synced with current mu/scale.
	const float radius = (float)compute_visual_radius();
	visual_mesh->set_radius(radius);
	visual_mesh->set_height(radius * 2.0f);
}

void BlackHole::_ready() {
	// Create a child MeshInstance3D as the black-hole body.
	visual = memnew(MeshInstance3D);
	visual->set_name("BlackHoleVisual");

	// Use a sphere mesh; from most camera angles this reads as a black circle.
	visual_mesh.instantiate();
	visual->set_mesh(visual_mesh);

	// Very dark material so the object looks like a black hole silhouette.
	Ref<StandardMaterial3D> mat;
	mat.instantiate();
	mat->set_albedo(Color(0.0f, 0.0f, 0.0f));
	mat->set_shading_mode(BaseMaterial3D::SHADING_MODE_PER_PIXEL);
	mat->set_specular_mode(BaseMaterial3D::SPECULAR_DISABLED);
	mat->set_specular(0.0f);
	mat->set_metallic(0.0f);
	mat->set_roughness(1.0f);
	visual_mesh->set_material(mat);

	add_child(visual);
	// Initial size based on starting mu.
	update_visual();
}

void BlackHole::set_mu(double p_mu) {
	mu = p_mu;
	// Re-size visual immediately when mass changes in inspector/runtime.
	update_visual();
}

double BlackHole::get_mu() const {
	return mu;
}

void BlackHole::set_horizon_radius(double p_radius) {
	horizon_radius = p_radius;
}

double BlackHole::get_horizon_radius() const {
	return horizon_radius;
}

void BlackHole::set_max_accel(double p_value) {
	max_accel = p_value;
}

double BlackHole::get_max_accel() const {
	return max_accel;
}

void BlackHole::set_visual_radius_scale(double p_value) {
	visual_radius_scale = p_value;
	// Re-size visual when scale changes.
	update_visual();
}

double BlackHole::get_visual_radius_scale() const {
	return visual_radius_scale;
}

Vector3 BlackHole::accel_at(const Vector3 &p_ship_pos) const {
	// Relative vector from black hole center to ship.
	Vector3 p = p_ship_pos - get_global_position();
	double r2 = (double)p.length_squared();
	// Avoid singularity at exact center.
	if (r2 < 1e-6) {
		return Vector3();
	}

	// Point-mass gravity: a = -mu * p / |p|^3
	double inv_r = 1.0 / Math::sqrt(r2);
	double inv_r3 = inv_r * inv_r * inv_r;
	Vector3 accel = -p * (real_t)(mu * inv_r3);

	// Clamp acceleration to keep gameplay numerically stable.
	double a_len = (double)accel.length();
	if (a_len > max_accel) {
		accel = accel.normalized() * (real_t)max_accel;
	}
	return accel;
}

bool BlackHole::is_inside_horizon(const Vector3 &p_ship_pos) const {
	// Horizon check used for fail/transition rules.
	return (p_ship_pos - get_global_position()).length() <= (real_t)horizon_radius;
}
