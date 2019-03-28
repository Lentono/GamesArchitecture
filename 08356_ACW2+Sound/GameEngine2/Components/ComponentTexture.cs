using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;
using GameEngine.Managers;
using GameEngine.Objects;

namespace GameEngine.Components
{
    public enum TextureType { CubeMap, Standard2D }
    //Ben Mullenger
    public class ComponentTexture : IComponent
    {
        Texture texture;

        public ComponentTexture(string textureName)
        {
            texture = ResourceManager.LoadTexture(textureName);
          //  type = TextureType.Standard2D;
        }

        public ComponentTexture(List<string> nameOfTextures)
        {
            //texture = ResourceManager.LoadCubeMap(nameOfTextures);
            //type = TextureType.CubeMap;
        }

        public Texture Texture
        {
            get { return texture; }
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_TEXTURE; }
        }
    }
}
