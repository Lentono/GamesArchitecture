using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace GameEngine.Components
{
    //Not Used!
    //Ben Mullenger
    /// <summary>
    /// Add component if you want to inspect an object
    /// </summary>
    public class ComponentUserInspectControl : IComponent
    {
        public ComponentTypes ComponentType
        {
            get
            {
                return ComponentTypes.COMPONENT_USER_INSPECT;
            }
        }

        /// <summary>
        /// Speed to rotate at
        /// </summary>
        public float RotSpeed
        {
            get;
            private set;
        }

        /// <summary>
        /// speed to scale at
        /// </summary>
        public float ScaleSpeed
        {
            get;
            private set;
        }

        /// <summary>
        /// Component will allow the inpsection of the entity
        /// </summary>
        /// <param name="pRotSpeed">speed of rotating the entity</param>
        /// <param name="pScaleSpeed">speed of scaling the entity</param>
        public ComponentUserInspectControl(float pRotSpeed, float pScaleSpeed)
        {
            RotSpeed = pRotSpeed;
            ScaleSpeed = pScaleSpeed;
        }
        /// <summary>
        /// Component will allow the inpsection of the entity
        /// </summary>
        public ComponentUserInspectControl()
            : this(0.1f, 0.1f)
        {            
        }
    }
}
