#version 330
 
in vec2 v_TexCoord;

uniform sampler2D s_texture;
uniform float pixelSize;

out vec4 Color;
 
void main()
{
	vec4 smoothedCol=vec4(0.0);
	for (int i=-2; i<=2; i++)
	{
		for (int j=-2; j<=2; j++)
		{
			smoothedCol += texture2D(s_texture, v_TexCoord + vec2(i, j)*pixelSize );
		}
	}
	Color = smoothedCol/25.0;
}