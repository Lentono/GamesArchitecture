using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace GameEngine.Components
{
    // <C.L>
    /// <summary>
    /// Component for the velocity of an object
    /// Note - Could I add mutators for changing individual x, y, z? Would make code elsewhere cleaner than new vector3
    /// </summary>
    public class ComponentVelocity : IComponent
    {
        private Vector3 _velocity;

        /// <summary>
        /// Constructor - Create an empty velocity component
        /// </summary>
        public ComponentVelocity()
        {
            _velocity = new Vector3();
        }

        /// <summary>
        /// Constructor - Create a custom velocity component with float values
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public ComponentVelocity(float x, float y, float z)
        {
            _velocity = new Vector3(x, y, z);
        }

        /// <summary>
        /// Constructor - Create a custom velocity component with a Vector3
        /// </summary>
        /// <param name="velocity"></param>
        public ComponentVelocity(Vector3 velocity)
        {
            _velocity = velocity;
        }

        /// <summary>
        /// Vector3 Mutator and Accessor for velocity
        /// </summary>
        public Vector3 Velocity
        {
            get
            {
                return _velocity;
            }
            set
            {
                _velocity = value;
            }
        }

        public ComponentTypes ComponentType
        {
            get
            {
                return ComponentTypes.COMPONENT_VELOCITY;
            }
        }
    }
}
