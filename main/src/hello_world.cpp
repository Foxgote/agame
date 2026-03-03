/* godot-cpp integration testing project.
 *
 * This is free and unencumbered software released into the public domain.
 */

#include "hello_world.h"

#include <godot_cpp/variant/utility_functions.hpp>

void HelloWorld::_bind_methods() {
	ClassDB::bind_method(D_METHOD("get_message"), &HelloWorld::get_message);
	ClassDB::bind_method(D_METHOD("say_hello"), &HelloWorld::say_hello);
}

String HelloWorld::get_message() const {
	return "Hello World";
}

void HelloWorld::say_hello() const {
	UtilityFunctions::print(get_message());
}
