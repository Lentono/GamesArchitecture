using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Objects
{
    //Ben Mullenger
    public class PostProcessEffect
    {
        public delegate void TextureShaderMethod(Texture t, Shader s);
        public Shader Shader;
        private TextureShaderMethod _prepRender;
        public bool Active = true;

        public PostProcessEffect(Shader shader, TextureShaderMethod prepRender)
        {
            Shader = shader;
            _prepRender = prepRender;
        }

        public void PrepRender(Texture t)
        {
            Shader.Use();
            _prepRender?.Invoke(t, Shader);
        }
    }
}
