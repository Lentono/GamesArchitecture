#version 330

//MJB 
layout (location = 0) in vec3 a_Position;
layout (location = 1) in vec3 a_Normal;
layout (location = 2) in vec2 a_TexCoord;

uniform mat4 _ViewProjection;

out vec3 v_SkyboxCoord;
out vec2 v_TexCoord;
out vec3 v_Normal;

void main()
{
	v_SkyboxCoord = a_Position;
	v_TexCoord = a_TexCoord;
	v_Normal = a_Normal;

	vec4 viewPosition = _ViewProjection * vec4(a_Position, 1.0);

	gl_Position = viewPosition.xyzw;
}