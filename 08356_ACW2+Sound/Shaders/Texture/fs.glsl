#version 330
 
in vec2 v_TexCoord;
in vec3 v_fragPos;
in vec3 v_Normal;

uniform sampler2D s_texture;
uniform vec3 EyePos;
uniform vec3 LightPos;
uniform vec4 uColour;

out vec4 Color;
 
void main()
{
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

	vec3 objCol = texture2D(s_texture, v_TexCoord).xyz;
	
	if(uColour != vec4(1,1,1,1))
	{
		objCol = mix(vec4(objCol,1), vec4(uColour.xyz,1), 0.5f).xyz;
	}

	vec3 result = (amb + diffuse + specular) * objCol;

	
	Color = vec4(result, 1.0);
}