using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Components
{
    //Ben Mullenger
    public class ComponentColour : IComponent
    {
        public ComponentTypes ComponentType
        {
            get
            {
                return ComponentTypes.COMPONENT_COLOUR;
            }
        }

        public Vector4 Colour;
        /// <summary>
        /// Default Colour
        /// </summary>
        public  Vector4 BaseColour;

        public ComponentColour(float r, float g, float b, float a = 1.0f)
            : this (new Vector4(r,g,b,a))
        {

        }

        public ComponentColour(Vector4 pColour)
        {
            BaseColour = Colour = pColour;
        }
    }
}
