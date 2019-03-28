#version 330
in vec2 v_TexCoord;
in vec3 v_fragPos;
in vec3 v_Normal;

uniform sampler2D s_texture;
uniform vec3 EyePos;
uniform vec3 LightPos;
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

	float ambStr = 0.1f;
	vec3 lightCol = vec3(1, 1, 1);

	vec3 amb = ambStr * lightCol;

	vec3 norm = normalize(v_Normal);
	vec3 lightDir = normalize(LightPos - v_fragPos);
	float diff = max(dot(norm, lightDir), 0.0);
	vec3 diffuse = diff * lightCol;

	float specStr = 0.5f;
	vec3 viewDir = normalize(EyePos - v_fragPos);
	vec3 refDir = reflect(-lightDir, norm);
	float spec = pow(max(dot(viewDir, refDir), 0.0), 32);
	vec3 specular = specStr * spec * lightCol;

	vec3 result = (amb + diffuse + specular) * objCol.xyz;

	FragColour = vec4(result, objCol.a);
}