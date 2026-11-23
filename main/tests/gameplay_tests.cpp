#include "gameplay/behavior_tree.hpp"

#include <cstdlib>
#include <iostream>
#include <memory>

using namespace agame::gameplay;

namespace {

int failures = 0;

void check(bool pCondition, const char *pMessage) {
	if (!pCondition) {
		std::cerr << "FAIL: " << pMessage << '\n';
		++failures;
	}
}

void testSequenceWaitAndTrace() {
	ActionBlackboard blackboard;
	bool effectApplied = false;
	auto sequence = std::make_unique<SequenceNode>("Action pipeline");
	sequence->add(std::make_unique<ConditionNode>("Validate", [](ActionBlackboard &) { return true; }));
	sequence->add(std::make_unique<WaitNode>(0.2, "Contact wait"));
	sequence->add(std::make_unique<TaskNode>("Apply", [&](TickContext &) { effectApplied = true; return BehaviorStatus::Success; }));
	BehaviorTree tree(std::move(sequence));
	check(tree.tick(0.1, blackboard) == BehaviorStatus::Running, "wait returns Running before its duration");
	check(blackboard.currentNode == "Contact wait", "debug trace reports the current leaf");
	check(tree.tick(0.1, blackboard) == BehaviorStatus::Success && effectApplied, "sequence applies effect after semantic wait");
}

void testSelectorAndCancellationCleanup() {
	ActionBlackboard blackboard;
	auto selector = std::make_unique<SelectorNode>("Fallback");
	selector->add(std::make_unique<ConditionNode>("Unavailable", [](ActionBlackboard &) { return false; }));
	selector->add(std::make_unique<ConditionNode>("Available", [](ActionBlackboard &) { return true; }));
	BehaviorTree selectorTree(std::move(selector));
	check(selectorTree.tick(0.0, blackboard) == BehaviorStatus::Success, "selector advances past a failed condition");

	bool cleanedUp = false;
	auto task = std::make_unique<TaskNode>("Reserved action", [](TickContext &) { return BehaviorStatus::Running; }, [&](ActionBlackboard &) { cleanedUp = true; });
	BehaviorTree cancellable(std::move(task));
	check(cancellable.tick(0.01, blackboard) == BehaviorStatus::Running, "long task enters Running state");
	cancellable.cancel(blackboard);
	check(cancellable.status() == BehaviorStatus::Cancelled && cleanedUp, "cancellation invokes active-task cleanup");
}

} // namespace

int main() {
	testSequenceWaitAndTrace();
	testSelectorAndCancellationCleanup();
	if (failures != 0) return EXIT_FAILURE;
	std::cout << "All agame gameplay tests passed\n";
	return EXIT_SUCCESS;
}
