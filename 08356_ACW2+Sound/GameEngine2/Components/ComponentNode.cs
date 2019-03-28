using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using GameEngine.Objects;

namespace GameEngine.Components
{
    //Author: <C.L>
    /// <summary>
    /// Component for storing node references (links)
    /// </summary>
    public class ComponentNode : IComponent
    {
        private List<Entity> _nodeLinks;

        /// <summary>
        /// Constructor - Empty constructor, initialises list
        /// </summary>
        public ComponentNode()
        {
            _nodeLinks = new List<Entity>();
        }

        /// <summary>
        /// Constructor - Add a node location component with a Vector3
        /// </summary>
        /// <param name="entity">Entity reference</param>
        public ComponentNode(Entity entity)
        {
            _nodeLinks = new List<Entity>();
            _nodeLinks.Add(entity);
        }

        /// <summary>
        /// Constructor - Add a range of custom node location components with a List of Vector3s>
        /// </summary>
        /// <param name="entities">List of entity references</param>
        public ComponentNode(List<Entity> entities)
        {
            _nodeLinks = new List<Entity>();
            _nodeLinks.AddRange(entities);
        }

        /// <summary>
        /// Accessor for List of Node Locations
        /// </summary>
        public List<Entity> NodeLinks
        {
            get
            {
                return _nodeLinks;
            }
        }

        public ComponentTypes ComponentType
        {
            get
            {
                return ComponentTypes.COMPONENT_NODE;
            }
        }
    }
}
