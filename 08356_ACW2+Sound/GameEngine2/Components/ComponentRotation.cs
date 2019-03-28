using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace GameEngine.Components
{
    //Ben Mullenger
    /// <summary>
    /// Component for rotating an object
    /// </summary>
    public class ComponentRotation : IComponent
    {
        Vector3 _rotation;

        /// <summary>
        /// Create rotation component with zero in all positions
        /// </summary>
        public ComponentRotation()
        {
            _rotation = Vector3.Zero;
        }

        public ComponentRotation(float x, float y, float z)
        {
            _rotation = new Vector3(x, y, z);
        }

        public ComponentRotation(Vector3 rot)
        {
            _rotation = rot;
        }

        public Vector3 Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
            }
        }

        public ComponentTypes ComponentType
        {
            get
            {
                return ComponentTypes.COMPONENT_ROTATION;
            }
        }
    }
}
