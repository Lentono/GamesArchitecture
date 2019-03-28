using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Objects
{
    //Ben Mullenger
    public class Material
    {
        public delegate void EntityShaderMethod(Entity e, Shader s);
        public Shader Shader;
        private EntityShaderMethod _prepRender;

        public Material(Shader shader, EntityShaderMethod prepRender)
        {
            Shader = shader;
            _prepRender = prepRender;
        }

        public void PrepRender(Entity e)
        {
            Shader.Use();
            _prepRender?.Invoke(e, Shader);
        }
    }
}
