#pragma once

#include <godot_cpp/classes/mesh_instance3d.hpp>
#include <godot_cpp/classes/node3d.hpp>
#include <godot_cpp/core/class_db.hpp>
#include <godot_cpp/variant/color.hpp>

using namespace godot;

class DebugGrid : public Node3D {
	GDCLASS(DebugGrid, Node3D);

protected:
	static void _bind_methods();

private:
	MeshInstance3D *grid_visual = nullptr;
	int half_cells = 80;
	int half_cells_y = 20;
	double cell_size = 2.0;
	Color grid_color = Color(0.5f, 0.7f, 0.9f, 0.55f);

	void rebuild_grid();

public:
	void _ready() override;

	void set_half_cells(int p_half_cells);
	int get_half_cells() const;

	void set_half_cells_y(int p_half_cells_y);
	int get_half_cells_y() const;

	void set_cell_size(double p_cell_size);
	double get_cell_size() const;

	void set_grid_color(const Color &p_color);
	Color get_grid_color() const;
};
