#version 330
 
in vec2 v_TexCoord;

uniform sampler2D s_texture;
uniform vec2 offset;

out vec4 Color;
 
void main()
{
	Color = texture2D(s_texture, v_TexCoord + offset);
}