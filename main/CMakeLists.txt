cmake_minimum_required(VERSION 3.20)
project(agame_ext LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Path to godot-cpp repo root
set(GODOT_CPP_DIR "${CMAKE_CURRENT_SOURCE_DIR}/.." CACHE PATH "Path to godot-cpp repo root")
get_filename_component(GODOT_CPP_DIR "${GODOT_CPP_DIR}" ABSOLUTE)

add_subdirectory(${GODOT_CPP_DIR} ${CMAKE_BINARY_DIR}/godot-cpp)

add_library(agame_ext SHARED
  src/register_types.cpp
  src/example.cpp
  src/hello_world.cpp
)

target_include_directories(agame_ext PRIVATE ${CMAKE_CURRENT_SOURCE_DIR}/src)
target_link_libraries(agame_ext PRIVATE godot-cpp)

# Drop DLL into the Godot project bin folder
set(OUTPUT_DIR "${CMAKE_CURRENT_SOURCE_DIR}/project/bin")

set_target_properties(agame_ext PROPERTIES
  RUNTIME_OUTPUT_DIRECTORY "$<1:${OUTPUT_DIR}>"
  LIBRARY_OUTPUT_DIRECTORY "$<1:${OUTPUT_DIR}>"
  PDB_OUTPUT_DIRECTORY     "$<1:${OUTPUT_DIR}>"
)

if (WIN32)
  target_compile_definitions(agame_ext PRIVATE NOMINMAX)
endif()

# MinGW-built extensions need their C++ runtime beside the DLL because Godot does
# not inherit the developer shell's compiler PATH when launched normally.
if (MINGW)
  get_filename_component(MINGW_RUNTIME_DIR "${CMAKE_CXX_COMPILER}" DIRECTORY)
  foreach(RUNTIME_DLL libgcc_s_seh-1.dll libstdc++-6.dll libwinpthread-1.dll)
    add_custom_command(TARGET agame_ext POST_BUILD
      COMMAND ${CMAKE_COMMAND} -E copy_if_different
        "${MINGW_RUNTIME_DIR}/${RUNTIME_DLL}"
        "${OUTPUT_DIR}/${RUNTIME_DLL}"
    )
  endforeach()
endif()
