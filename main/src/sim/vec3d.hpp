#pragma once

#include <cmath>

namespace agame::sim {

struct Vec3d {
	double x = 0.0;
	double y = 0.0;
	double z = 0.0;

	constexpr Vec3d() = default;
	constexpr Vec3d(double pX, double pY, double pZ) : x(pX), y(pY), z(pZ) {}

	constexpr Vec3d operator+(const Vec3d &pOther) const { return {x + pOther.x, y + pOther.y, z + pOther.z}; }
	constexpr Vec3d operator-(const Vec3d &pOther) const { return {x - pOther.x, y - pOther.y, z - pOther.z}; }
	constexpr Vec3d operator-() const { return {-x, -y, -z}; }
	constexpr Vec3d operator*(double pScale) const { return {x * pScale, y * pScale, z * pScale}; }
	constexpr Vec3d operator/(double pScale) const { return {x / pScale, y / pScale, z / pScale}; }

	Vec3d &operator+=(const Vec3d &pOther) {
		x += pOther.x;
		y += pOther.y;
		z += pOther.z;
		return *this;
	}

	Vec3d &operator-=(const Vec3d &pOther) {
		x -= pOther.x;
		y -= pOther.y;
		z -= pOther.z;
		return *this;
	}

	Vec3d &operator*=(double pScale) {
		x *= pScale;
		y *= pScale;
		z *= pScale;
		return *this;
	}

	constexpr double lengthSquared() const { return x * x + y * y + z * z; }
	double length() const { return std::sqrt(lengthSquared()); }
	bool isFinite() const { return std::isfinite(x) && std::isfinite(y) && std::isfinite(z); }

	Vec3d normalized(double pEpsilon = 1.0e-12) const {
		const double magnitude = length();
		return magnitude > pEpsilon ? *this / magnitude : Vec3d{};
	}
};

constexpr Vec3d operator*(double pScale, const Vec3d &pVector) { return pVector * pScale; }
constexpr double dot(const Vec3d &pA, const Vec3d &pB) { return pA.x * pB.x + pA.y * pB.y + pA.z * pB.z; }
constexpr Vec3d cross(const Vec3d &pA, const Vec3d &pB) {
	return {pA.y * pB.z - pA.z * pB.y, pA.z * pB.x - pA.x * pB.z, pA.x * pB.y - pA.y * pB.x};
}

inline Vec3d lerp(const Vec3d &pFrom, const Vec3d &pTo, double pWeight) {
	return pFrom + (pTo - pFrom) * pWeight;
}

} // namespace agame::sim
