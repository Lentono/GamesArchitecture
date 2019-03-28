#version 330
 
in vec2 v_TexCoord;

uniform sampler2D s_texture;
uniform sampler2D s_texture2;
uniform vec4 uChroma;

out vec4 Color;
 
void main()
{
	vec4 back = texture2D(s_texture, v_TexCoord);
	vec4 front = texture2D(s_texture2, v_TexCoord);
	
	Color = mix(front, back, 0.5);
}