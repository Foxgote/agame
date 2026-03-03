#pragma once

#include <godot_cpp/variant/vector3.hpp>

namespace shipcalc {
// Baseline point-mass gravity acceleration: a = -mu * p / |p|^3
godot::Vector3 accel_point_mass(double p_mu, const godot::Vector3 &p_relative_pos);
} // namespace shipcalc
