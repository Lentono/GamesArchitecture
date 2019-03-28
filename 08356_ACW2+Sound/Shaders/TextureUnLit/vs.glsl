#version 330
 
layout (location = 0) in vec3 a_Position;
layout (location = 1) in vec3 a_Normal;
layout (location = 2) in vec2 a_TexCoord;

uniform mat4 View;
uniform mat4 Proj;
uniform mat4 uModel;

out vec2 v_TexCoord;
out vec3 v_fragPos;
out vec3 v_Normal;


void main()
{
	v_fragPos = vec3(uModel * vec4(a_Position,1.0));
	v_Normal = mat3(transpose(inverse(uModel))) * a_Normal;

    v_TexCoord = a_TexCoord;

	gl_Position = Proj * View * vec4(v_fragPos, 1.0);
}