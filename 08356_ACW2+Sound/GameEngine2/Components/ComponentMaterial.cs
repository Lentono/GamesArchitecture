using GameEngine.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Components
{
    //Ben Mullenger
    public class ComponentMaterial : IComponent
    {
        public ComponentTypes ComponentType
        {
            get => ComponentTypes.COMPONENT_MATERIAL;
        }

        public Material material;

        public ComponentMaterial(Material pMaterial)
        {
            material = pMaterial;
        }
    }
}
