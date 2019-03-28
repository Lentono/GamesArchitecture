using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace GameEngine.Components
{
    public class ComponentPosition : IComponent
    {
        Vector3 position;
        Vector3 oldPos;

        public ComponentPosition(float x, float y, float z)
        {
            position = new Vector3(x, y, z);
        }

        public ComponentPosition(Vector3 pos)
        {
            position = pos;
        }

        public Vector3 OldPosition
        {
            get { return oldPos; }
        }

        public Vector3 Position
        {
            get { return position; }
            set
            {
                oldPos = position;
                position = value;
            }
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_POSITION; }
        }
    }
}
