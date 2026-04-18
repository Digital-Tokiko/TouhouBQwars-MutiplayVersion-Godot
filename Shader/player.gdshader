shader_type canvas_item;

uniform float FlashState:hint_range(0,1);
void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec4 color = texture(TEXTURE,UV);
	COLOR = mix(color,vec4(1,1,1,1),FlashState);
	COLOR.a = color.a;
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
