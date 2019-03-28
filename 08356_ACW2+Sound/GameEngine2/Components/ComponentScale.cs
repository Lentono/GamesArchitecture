using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace GameEngine.Components
{
    //<C.L>
    /// <summary>
    /// Component for the scale of an object
    /// Note - Might be needed for the maze environment so I'm creating it now
    /// Another Note - Inspired by Ben's component rotation, very good public class, popped a boner, couldn't not be inspired to use it
    /// </summary>
    public class ComponentScale : IComponent
    {
        private Vector3 _scale;

        /// <summary>
        /// Constructor - Create an empty scale component
        /// </summary>
        public ComponentScale()
        {
            _scale = new Vector3();
        }

        /// <summary>
        /// Constructor - Create an equal scale component (Cube)
        /// </summary>
        /// <param name="xyz">scale value</param>
        public ComponentScale(float xyz)
        {
            _scale = new Vector3(xyz);
        }

        /// <summary>
        /// Constructor - Create a custom scale component with float values
        /// </summary>
        /// <param name="x">scale value in x</param>
        /// <param name="y">scale value in y</param>
        /// <param name="z">scale value in z</param>
        public ComponentScale(float x, float y, float z)
        {
            _scale = new Vector3(x, y, z);
        }

        /// <summary>
        /// Constructor - Create a custom scale component with a Vector3
        /// </summary>
        /// <param name="scale">scale value</param>
        public ComponentScale(Vector3 scale)
        {
            _scale = scale;
        }

        /// <summary>
        /// Vector3 Mutator and Accessor for scale
        /// </summary>
        public Vector3 Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
            }
        }

        public ComponentTypes ComponentType
        {
            get
            {
                return ComponentTypes.COMPONENT_SCALE;
            }
        }
    }
}
