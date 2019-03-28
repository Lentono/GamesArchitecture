#version 330
#extension GL_NV_shadow_samplers_cube : enable

// MJB 
uniform samplerCube Skybox;
out vec4 Color;

in vec3 v_SkyboxCoord;
in vec2 v_TexCoord;
in vec3 v_Normal;

void main()
{
	vec4 cube = textureCube(Skybox, v_SkyboxCoord);
	cube.xyz /= 3f;
	Color = cube;
}