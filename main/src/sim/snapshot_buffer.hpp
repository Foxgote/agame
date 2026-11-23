#pragma once

#include <array>
#include <memory>
#include <mutex>

namespace agame::sim {

template <typename T>
class SnapshotBuffer {
public:
	struct Pair {
		std::shared_ptr<const T> previous;
		std::shared_ptr<const T> current;
	};

	void publish(std::shared_ptr<const T> pSnapshot) {
		std::lock_guard<std::mutex> lock(mutex_);
		slots_[0] = slots_[1];
		slots_[1] = std::move(pSnapshot);
	}

	Pair latestPair() const {
		std::lock_guard<std::mutex> lock(mutex_);
		return {slots_[0], slots_[1]};
	}

private:
	mutable std::mutex mutex_;
	std::array<std::shared_ptr<const T>, 2> slots_;
};

} // namespace agame::sim
