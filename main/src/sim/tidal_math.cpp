#include "tidal_math.hpp"

#include <algorithm>
#include <cmath>

namespace agame::sim {

TidalResult computeNewtonianTidalResult(const Vec3d &pPositionM, const std::vector<BodyState> &pBodies, const GravityConfig &pConfig) {
	TidalResult result;
	const double guard = std::max(1.0e-6, pConfig.nearCollisionGuardM);
	for (const BodyState &body : pBodies) {
		if (!body.active || body.massKg <= 0.0) {
			continue;
		}
		const Vec3d offset = pPositionM - body.positionM;
		const double radius = std::max(offset.length(), guard);
		const Vec3d n = offset / radius;
		const double scale = gravitationalConstant * body.massKg / (radius * radius * radius);
		const double components[3] = {n.x, n.y, n.z};
		for (int row = 0; row < 3; ++row) {
			for (int column = 0; column < 3; ++column) {
				const double identity = row == column ? 1.0 : 0.0;
				result.tensorPerS2[row * 3 + column] += scale * (3.0 * components[row] * components[column] - identity);
			}
		}
	}

	double frobeniusSquared = 0.0;
	for (double component : result.tensorPerS2) {
		frobeniusSquared += component * component;
	}
	result.severityPerS2 = std::sqrt(frobeniusSquared);

	Vec3d direction{1.0, 0.3, 0.1};
	for (int iteration = 0; iteration < 8; ++iteration) {
		const auto &t = result.tensorPerS2;
		direction = Vec3d{
			t[0] * direction.x + t[1] * direction.y + t[2] * direction.z,
			t[3] * direction.x + t[4] * direction.y + t[5] * direction.z,
			t[6] * direction.x + t[7] * direction.y + t[8] * direction.z,
		}.normalized();
		if (direction.lengthSquared() == 0.0) {
			direction = {1.0, 0.0, 0.0};
			break;
		}
	}
	result.principalStretchDirection = direction;
	return result;
}

} // namespace agame::sim
