/* godot-cpp integration testing project.
 *
 * This is free and unencumbered software released into the public domain.
 */

#pragma once

#include <godot_cpp/classes/node.hpp>
#include <godot_cpp/core/class_db.hpp>
#include <godot_cpp/variant/string.hpp>

using namespace godot;

class HelloWorld : public Node {
	GDCLASS(HelloWorld, Node);

protected:
	static void _bind_methods();

public:
	String get_message() const;
	void say_hello() const;
};
