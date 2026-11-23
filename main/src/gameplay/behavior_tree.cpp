#include "behavior_tree.hpp"

#include <utility>

namespace agame::gameplay {

void BehaviorNode::cancel(ActionBlackboard &pBlackboard) {
	trace(pBlackboard);
}

void BehaviorNode::reset() {}

SequenceNode::SequenceNode(std::string pName) : BehaviorNode(std::move(pName)) {}

void SequenceNode::add(std::unique_ptr<BehaviorNode> pChild) {
	children_.push_back(std::move(pChild));
}

BehaviorStatus SequenceNode::tick(TickContext &pContext) {
	trace(pContext.blackboard);
	while (currentIndex_ < children_.size()) {
		const BehaviorStatus status = children_[currentIndex_]->tick(pContext);
		if (status == BehaviorStatus::Success) {
			++currentIndex_;
			continue;
		}
		return status;
	}
	return BehaviorStatus::Success;
}

void SequenceNode::cancel(ActionBlackboard &pBlackboard) {
	if (currentIndex_ < children_.size()) {
		children_[currentIndex_]->cancel(pBlackboard);
	}
	BehaviorNode::cancel(pBlackboard);
}

void SequenceNode::reset() {
	currentIndex_ = 0;
	for (auto &child : children_) {
		child->reset();
	}
}

SelectorNode::SelectorNode(std::string pName) : BehaviorNode(std::move(pName)) {}

void SelectorNode::add(std::unique_ptr<BehaviorNode> pChild) {
	children_.push_back(std::move(pChild));
}

BehaviorStatus SelectorNode::tick(TickContext &pContext) {
	trace(pContext.blackboard);
	while (currentIndex_ < children_.size()) {
		const BehaviorStatus status = children_[currentIndex_]->tick(pContext);
		if (status == BehaviorStatus::Failure) {
			++currentIndex_;
			continue;
		}
		return status;
	}
	return BehaviorStatus::Failure;
}

void SelectorNode::cancel(ActionBlackboard &pBlackboard) {
	if (currentIndex_ < children_.size()) {
		children_[currentIndex_]->cancel(pBlackboard);
	}
	BehaviorNode::cancel(pBlackboard);
}

void SelectorNode::reset() {
	currentIndex_ = 0;
	for (auto &child : children_) {
		child->reset();
	}
}

ConditionNode::ConditionNode(std::string pName, Predicate pPredicate) : BehaviorNode(std::move(pName)), predicate_(std::move(pPredicate)) {}

BehaviorStatus ConditionNode::tick(TickContext &pContext) {
	trace(pContext.blackboard);
	return predicate_ && predicate_(pContext.blackboard) ? BehaviorStatus::Success : BehaviorStatus::Failure;
}

TaskNode::TaskNode(std::string pName, Task pTask, Cleanup pCleanup) : BehaviorNode(std::move(pName)), task_(std::move(pTask)), cleanup_(std::move(pCleanup)) {}

BehaviorStatus TaskNode::tick(TickContext &pContext) {
	trace(pContext.blackboard);
	active_ = true;
	const BehaviorStatus status = task_ ? task_(pContext) : BehaviorStatus::Failure;
	if (status != BehaviorStatus::Running) {
		active_ = false;
	}
	return status;
}

void TaskNode::cancel(ActionBlackboard &pBlackboard) {
	if (active_ && cleanup_) {
		cleanup_(pBlackboard);
	}
	active_ = false;
	BehaviorNode::cancel(pBlackboard);
}

void TaskNode::reset() {
	active_ = false;
}

WaitNode::WaitNode(double pDurationS, std::string pName) : BehaviorNode(std::move(pName)), durationS_(pDurationS) {}

BehaviorStatus WaitNode::tick(TickContext &pContext) {
	trace(pContext.blackboard);
	elapsedS_ += pContext.deltaS;
	pContext.blackboard.actionProgress = durationS_ > 0.0 ? elapsedS_ / durationS_ : 1.0;
	return elapsedS_ >= durationS_ ? BehaviorStatus::Success : BehaviorStatus::Running;
}

void WaitNode::reset() {
	elapsedS_ = 0.0;
}

BehaviorTree::BehaviorTree(std::unique_ptr<BehaviorNode> pRoot) : root_(std::move(pRoot)) {}

BehaviorStatus BehaviorTree::tick(double pDeltaS, ActionBlackboard &pBlackboard) {
	if (!root_ || status_ == BehaviorStatus::Cancelled) {
		return status_ = root_ ? BehaviorStatus::Cancelled : BehaviorStatus::Failure;
	}
	TickContext context{pDeltaS, pBlackboard};
	return status_ = root_->tick(context);
}

void BehaviorTree::cancel(ActionBlackboard &pBlackboard) {
	if (root_ && status_ == BehaviorStatus::Running) {
		root_->cancel(pBlackboard);
	}
	status_ = BehaviorStatus::Cancelled;
}

void BehaviorTree::reset() {
	if (root_) {
		root_->reset();
	}
	status_ = BehaviorStatus::Running;
}

const char *actionIdName(ActionId pAction) {
	switch (pAction) {
		case ActionId::Interact: return "interact";
		case ActionId::Inspect: return "inspect";
		case ActionId::Sit: return "sit";
		case ActionId::Stand: return "stand";
		case ActionId::Eat: return "eat";
		case ActionId::Press: return "press";
		case ActionId::Turn: return "turn";
		case ActionId::Adjust: return "adjust";
		case ActionId::Open: return "open";
		case ActionId::Repair: return "repair";
	}
	return "interact";
}

} // namespace agame::gameplay
