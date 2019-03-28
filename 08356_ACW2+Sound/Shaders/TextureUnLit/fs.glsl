#version 330
 
in vec2 v_TexCoord;
in vec3 v_fragPos;
in vec3 v_Normal;

uniform sampler2D s_texture;
uniform vec4 uColour;

out vec4 Color;
 
void main()
{
	vec4 objCol = texture2D(s_texture, v_TexCoord);
	if (objCol.a == 0)
	{
		discard;
	}
	if(uColour != vec4(1,1,1,1))
	{
		objCol = mix(objCol, vec4(uColour.xyz,1), 0.5f);
	}

	Color = vec4(objCol);
}