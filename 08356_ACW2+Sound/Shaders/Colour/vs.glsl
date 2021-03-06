﻿#version 330
 
layout (location = 0) in vec3 a_Position;
layout (location = 1) in vec3 a_Normal;
layout (location = 2) in vec2 a_TexCoord;

out vec2 v_TexCoord;


void main()
{
    v_TexCoord = vec2(a_TexCoord.x, -a_TexCoord.y);

	gl_Position = vec4(a_Position, 1.0);
}