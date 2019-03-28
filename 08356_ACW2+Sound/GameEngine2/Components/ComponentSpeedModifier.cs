using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace GameEngine.Components
{
    //<C.L>
    /// <summary>
    /// Component for the speed of a drone
    /// </summary>
    public class ComponentSpeedModifier : IComponent
    {
        private float _speedMod;

        /// <summary>
        /// Constructor - Create an empty speed component
        /// </summary>
        public ComponentSpeedModifier()
        {
            _speedMod = 0.0f;
        }

        /// <summary>
        /// Constructor - Create a custom speed modifier component with a float value
        /// </summary>
        /// <param name="speedMode">x speed</param>
        public ComponentSpeedModifier(float speedMod)
        {
            _speedMod = speedMod;
        }

        /// <summary>
        /// Vector3 Speed Mutator and Accessor
        /// </summary>
        public float SpeedMod
        {
            get
            {
                return _speedMod;
            }
            set
            {
                _speedMod = value;
            }
        }

        public ComponentTypes ComponentType
        {
            get
            {
                return ComponentTypes.COMPONENT_SPEEDMOD;
            }
        }
    }
}
