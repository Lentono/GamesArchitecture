using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GameEngine.Components
{
    //Ben Mullenger
    public class Component2DCollider : IComponent
    {
        public ComponentTypes ComponentType
        {
            get
            {
                return ComponentTypes.COMPONENT_2D_COLLIDER;
            }
        }

        /// <summary>
        /// Hitbox for the collider
        /// </summary>
        public Rectangle rectangle;

        public Component2DCollider(int x, int y, int width, int height)
            : this(new Rectangle(x, y, width, height))
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect">Hitbox for the collider</param>
        public Component2DCollider(Rectangle rect)
        {
            rectangle = rect;
        }
    }
}
