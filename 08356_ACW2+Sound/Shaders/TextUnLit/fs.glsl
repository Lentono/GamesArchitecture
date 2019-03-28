#version 330
in vec2 v_TexCoord;
in vec3 v_fragPos;
in vec3 v_Normal;

uniform sampler2D s_texture;
uniform vec4 uColour;

out vec4 FragColour;

void main()
{
	vec4 objCol = texture2D(s_texture, v_TexCoord);

	if (objCol.r < 0.5)
	{
		discard;
	}
	objCol.a = objCol.r * 1.25;

	
	if(uColour != vec4(1,1,1,1))
	{
		objCol = vec4 (uColour.xyz, objCol.a * uColour.a);
	}

	FragColour = objCol;
}