using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using GameEngine.Objects;

namespace GameEngine.Components
{
    //<C.L>
    /// <summary>
    /// Component for the target position of a drone
    /// </summary>
    public class ComponentTargetNode : IComponent
    {
        private Entity _targetNode;

        /// <summary>
        /// Constructor - Create a custom target node component with a given entity reference>
        /// </summary>
        /// <param name="node">node target</param>
        public ComponentTargetNode(Entity node)
        {
            _targetNode = node;
        }

        /// <summary>
        /// Vector3 Target Position Mutator and Accessor
        /// </summary>
        public Entity TargetNode
        {
            get
            {
                return _targetNode;
            }
            set
            {
                _targetNode = value;
            }
        }

        public ComponentTypes ComponentType
        {
            get
            {
                return ComponentTypes.COMPONENT_TARGET;
            }
        }
    }
}
