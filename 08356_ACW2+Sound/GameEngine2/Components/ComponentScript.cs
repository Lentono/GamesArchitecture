using GameEngine.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Components
{
    //Ben Mullenger
    public class ComponentScript : IComponent
    {
        public ComponentTypes ComponentType
        {
            get
            {
                return ComponentTypes.COMPONENT_SCRIPT;
            }
        }

        public Script script;

        public ComponentScript(Script pScript)
        {
            script = pScript;
        }
    }
}
