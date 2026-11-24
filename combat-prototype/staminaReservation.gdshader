shader_type canvas_item;

void fragment() {
    float stripe = step(0.5, fract((FRAGCOORD.x + FRAGCOORD.y) / 8.0));
    COLOR.rgb *= mix(vec3(0.32, 0.45, 0.53), vec3(0.71, 0.83, 0.86), stripe);
}
