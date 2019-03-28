#version 330
 
in vec2 v_TexCoord;

uniform sampler2D s_texture;

out vec4 Color;
 
void main()
{
	Color = vec4(0.5,0.5,0.5,1) * texture2D(s_texture, v_TexCoord);
}