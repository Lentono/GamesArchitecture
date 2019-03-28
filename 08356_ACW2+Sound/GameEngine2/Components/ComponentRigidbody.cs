using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Components
{
    //Ben Mullenger, Callum Lenton
    public class ComponentRigidbody : IComponent
    {
        public bool IsKinematic;

        public ComponentRigidbody(bool isKinematic = false)
        {
            IsKinematic = isKinematic;
        }

        public ComponentTypes ComponentType => ComponentTypes.COMPONENT_RIGIDBODY;
    }
}
