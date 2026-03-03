#include "debug_grid.h"

#include <godot_cpp/classes/immediate_mesh.hpp>
#include <godot_cpp/classes/mesh.hpp>
#include <godot_cpp/classes/standard_material3d.hpp>

void DebugGrid::_bind_methods() {
	ClassDB::bind_method(D_METHOD("set_half_cells", "half_cells"), &DebugGrid::set_half_cells);
	ClassDB::bind_method(D_METHOD("get_half_cells"), &DebugGrid::get_half_cells);
	ClassDB::bind_method(D_METHOD("set_half_cells_y", "half_cells_y"), &DebugGrid::set_half_cells_y);
	ClassDB::bind_method(D_METHOD("get_half_cells_y"), &DebugGrid::get_half_cells_y);
	ClassDB::bind_method(D_METHOD("set_cell_size", "cell_size"), &DebugGrid::set_cell_size);
	ClassDB::bind_method(D_METHOD("get_cell_size"), &DebugGrid::get_cell_size);
	ClassDB::bind_method(D_METHOD("set_grid_color", "grid_color"), &DebugGrid::set_grid_color);
	ClassDB::bind_method(D_METHOD("get_grid_color"), &DebugGrid::get_grid_color);

	ADD_PROPERTY(PropertyInfo(Variant::INT, "half_cells"), "set_half_cells", "get_half_cells");
	ADD_PROPERTY(PropertyInfo(Variant::INT, "half_cells_y"), "set_half_cells_y", "get_half_cells_y");
	ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "cell_size"), "set_cell_size", "get_cell_size");
	ADD_PROPERTY(PropertyInfo(Variant::COLOR, "grid_color"), "set_grid_color", "get_grid_color");
}

void DebugGrid::_ready() {
	if (!grid_visual) {
		grid_visual = memnew(MeshInstance3D);
		grid_visual->set_name("GridVisual");
		add_child(grid_visual);
	}
	rebuild_grid();
}

void DebugGrid::rebuild_grid() {
	if (!grid_visual) {
		return;
	}

	const int cells = half_cells < 1 ? 1 : half_cells;
	const int cells_y = half_cells_y < 1 ? 1 : half_cells_y;
	const real_t step = cell_size <= 0.001 ? (real_t)0.001 : (real_t)cell_size;
	const real_t extent_xz = (real_t)cells * step;
	const real_t extent_y = (real_t)cells_y * step;

	Ref<ImmediateMesh> grid_mesh;
	grid_mesh.instantiate();
	grid_mesh->surface_begin(Mesh::PRIMITIVE_LINES);

	// X-axis lines for each (y, z) in the lattice.
	for (int yi = -cells_y; yi <= cells_y; yi++) {
		const real_t y = (real_t)yi * step;
		for (int zi = -cells; zi <= cells; zi++) {
			const real_t z = (real_t)zi * step;
			grid_mesh->surface_add_vertex(Vector3(-extent_xz, y, z));
			grid_mesh->surface_add_vertex(Vector3(extent_xz, y, z));
		}
	}

	// Z-axis lines for each (x, y) in the lattice.
	for (int yi = -cells_y; yi <= cells_y; yi++) {
		const real_t y = (real_t)yi * step;
		for (int xi = -cells; xi <= cells; xi++) {
			const real_t x = (real_t)xi * step;
			grid_mesh->surface_add_vertex(Vector3(x, y, -extent_xz));
			grid_mesh->surface_add_vertex(Vector3(x, y, extent_xz));
		}
	}

	// Y-axis lines for each (x, z) in the lattice.
	for (int xi = -cells; xi <= cells; xi++) {
		const real_t x = (real_t)xi * step;
		for (int zi = -cells; zi <= cells; zi++) {
			const real_t z = (real_t)zi * step;
			grid_mesh->surface_add_vertex(Vector3(x, -extent_y, z));
			grid_mesh->surface_add_vertex(Vector3(x, extent_y, z));
		}
	}
	grid_mesh->surface_end();

	Ref<StandardMaterial3D> grid_material;
	grid_material.instantiate();
	grid_material->set_shading_mode(BaseMaterial3D::SHADING_MODE_UNSHADED);
	grid_material->set_transparency(BaseMaterial3D::TRANSPARENCY_ALPHA);
	grid_material->set_albedo(grid_color);
	grid_mesh->surface_set_material(0, grid_material);

	grid_visual->set_mesh(grid_mesh);
}

void DebugGrid::set_half_cells(int p_half_cells) {
	half_cells = p_half_cells;
	rebuild_grid();
}

int DebugGrid::get_half_cells() const {
	return half_cells;
}

void DebugGrid::set_half_cells_y(int p_half_cells_y) {
	half_cells_y = p_half_cells_y;
	rebuild_grid();
}

int DebugGrid::get_half_cells_y() const {
	return half_cells_y;
}

void DebugGrid::set_cell_size(double p_cell_size) {
	cell_size = p_cell_size;
	rebuild_grid();
}

double DebugGrid::get_cell_size() const {
	return cell_size;
}

void DebugGrid::set_grid_color(const Color &p_color) {
	grid_color = p_color;
	rebuild_grid();
}

Color DebugGrid::get_grid_color() const {
	return grid_color;
}
