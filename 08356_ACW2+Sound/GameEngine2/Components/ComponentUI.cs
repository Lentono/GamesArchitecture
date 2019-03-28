

namespace GameEngine.Components
{
    //Ben Mullenger
    public class ComponentUI : IComponent
    {
        public ComponentTypes ComponentType
        {
            get
            {
                return ComponentTypes.COMPONENT_UI;
            }
        }
    }
}