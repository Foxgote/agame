#include "shipcalc.h"

#include <godot_cpp/core/math.hpp>

namespace shipcalc {
godot::Vector3 accel_point_mass(double p_mu, const godot::Vector3 &p_relative_pos) {
	double r2 = (double)p_relative_pos.length_squared();
	if (r2 < 1e-9) {
		return godot::Vector3();
	}

	double inv_r = 1.0 / godot::Math::sqrt(r2);
	double inv_r3 = inv_r * inv_r * inv_r;
	return -p_relative_pos * (godot::real_t)(p_mu * inv_r3);
}
} // namespace shipcalc
