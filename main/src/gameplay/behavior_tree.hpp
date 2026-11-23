#pragma once

#include "sim/vec3d.hpp"

#include <any>
#include <cstdint>
#include <functional>
#include <memory>
#include <string>
#include <unordered_map>
#include <utility>
#include <vector>

namespace agame::gameplay {

enum class BehaviorStatus : std::uint8_t {
	Running,
	Success,
	Failure,
	Cancelled,
};

enum class ActionId : std::uint8_t {
	Interact,
	Inspect,
	Sit,
	Stand,
	Eat,
	Press,
	Turn,
	Adjust,
	Open,
	Repair,
};

struct ActionBlackboard {
	void *actor = nullptr;
	void *target = nullptr;
	ActionId requestedAction = ActionId::Interact;
	sim::Vec3d interactionAnchor;
	void *leftHandTarget = nullptr;
	void *rightHandTarget = nullptr;
	void *lookTarget = nullptr;
	void *cameraTarget = nullptr;
	void *currentItem = nullptr;
	double actionProgress = 0.0;
	bool interruptible = true;
	std::string failureReason;
	std::uint64_t reservationToken = 0;
	std::unordered_map<std::string, std::any> actionContext;
	std::string currentNode;
};

struct TickContext {
	double deltaS = 0.0;
	ActionBlackboard &blackboard;
};

class BehaviorNode {
public:
	explicit BehaviorNode(std::string pName) : name_(std::move(pName)) {}
	virtual ~BehaviorNode() = default;

	virtual BehaviorStatus tick(TickContext &pContext) = 0;
	virtual void cancel(ActionBlackboard &pBlackboard);
	virtual void reset();
	const std::string &name() const { return name_; }

protected:
	void trace(ActionBlackboard &pBlackboard) const { pBlackboard.currentNode = name_; }

private:
	std::string name_;
};

class SequenceNode final : public BehaviorNode {
public:
	explicit SequenceNode(std::string pName = "Sequence");
	void add(std::unique_ptr<BehaviorNode> pChild);
	BehaviorStatus tick(TickContext &pContext) override;
	void cancel(ActionBlackboard &pBlackboard) override;
	void reset() override;

private:
	std::vector<std::unique_ptr<BehaviorNode>> children_;
	std::size_t currentIndex_ = 0;
};

class SelectorNode final : public BehaviorNode {
public:
	explicit SelectorNode(std::string pName = "Selector");
	void add(std::unique_ptr<BehaviorNode> pChild);
	BehaviorStatus tick(TickContext &pContext) override;
	void cancel(ActionBlackboard &pBlackboard) override;
	void reset() override;

private:
	std::vector<std::unique_ptr<BehaviorNode>> children_;
	std::size_t currentIndex_ = 0;
};

class ConditionNode final : public BehaviorNode {
public:
	using Predicate = std::function<bool(ActionBlackboard &)>;
	ConditionNode(std::string pName, Predicate pPredicate);
	BehaviorStatus tick(TickContext &pContext) override;

private:
	Predicate predicate_;
};

class TaskNode final : public BehaviorNode {
public:
	using Task = std::function<BehaviorStatus(TickContext &)>;
	using Cleanup = std::function<void(ActionBlackboard &)>;
	TaskNode(std::string pName, Task pTask, Cleanup pCleanup = {});
	BehaviorStatus tick(TickContext &pContext) override;
	void cancel(ActionBlackboard &pBlackboard) override;
	void reset() override;

private:
	Task task_;
	Cleanup cleanup_;
	bool active_ = false;
};

class WaitNode final : public BehaviorNode {
public:
	WaitNode(double pDurationS, std::string pName = "Wait");
	BehaviorStatus tick(TickContext &pContext) override;
	void reset() override;

private:
	double durationS_ = 0.0;
	double elapsedS_ = 0.0;
};

class BehaviorTree {
public:
	explicit BehaviorTree(std::unique_ptr<BehaviorNode> pRoot);
	BehaviorStatus tick(double pDeltaS, ActionBlackboard &pBlackboard);
	void cancel(ActionBlackboard &pBlackboard);
	void reset();
	BehaviorStatus status() const { return status_; }

private:
	std::unique_ptr<BehaviorNode> root_;
	BehaviorStatus status_ = BehaviorStatus::Running;
};

const char *actionIdName(ActionId pAction);

} // namespace agame::gameplay
