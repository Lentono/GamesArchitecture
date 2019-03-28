#version 330
 
in vec2 v_TexCoord;

uniform sampler2D s_texture;
uniform vec2 delta;

out vec4 Color;


// https://github.com/spite/Wagner/blob/master/fragment-shaders/rgb-split-fs.glsl
void main()
{
	vec2 resolution = vec2(0.1,0.1);
	vec2 dir = v_TexCoord - vec2( .5 );
	float d = .7 * length( dir );
	normalize( dir );
	vec2 value = d * dir * delta;

	vec4 c1 = texture2D( s_texture, v_TexCoord - value / resolution.x );
	vec4 c2 = texture2D( s_texture, v_TexCoord );
	vec4 c3 = texture2D( s_texture, v_TexCoord + value / resolution.y );
	
	Color = mix(vec4( c1.r, c2.g, c3.b, c1.a + c2.a + c3.b ), vec4(0.933, 0.24, 1, 1), 0.3);
}