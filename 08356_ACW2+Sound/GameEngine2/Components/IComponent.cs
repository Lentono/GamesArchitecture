using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Components
{
    [FlagsAttribute]
    public enum ComponentTypes {
        COMPONENT_NONE = 0,
	    COMPONENT_POSITION = 1 << 0,
        COMPONENT_ROTATION = 1 << 1,
        COMPONENT_SCALE    = 1 << 2,
        COMPONENT_VELOCITY = 1 << 3,
        COMPONENT_GEOMETRY = 1 << 4,
        COMPONENT_TEXTURE  = 1 << 5,
        COMPONENT_AUDIO    = 1 << 6,
        COMPONENT_USER_INSPECT = 1 << 7,
        COMPONENT_CAMERA = 1 << 8,
        COMPONENT_TEXT = 1 << 9,
        COMPONENT_UI = 1 << 10,
        COMPONENT_SCRIPT = 1 << 11,
        COMPONENT_2D_COLLIDER = 1 << 12,
        COMPONENT_COLOUR = 1 << 13,
        COMPONENT_NODE = 1 << 14,
        COMPONENT_SPEEDMOD = 1 << 15,
        COMPONENT_TARGET = 1 << 16,
        COMPONENT_RENDER_TO_FRAME_BUFFER = 1 << 17,
        COMPONENT_MATERIAL = 1 << 18,
        COMPONENT_SPHERE_COLLIDER = 1 << 19,
        COMPONENT_LINE_COLLIDER = 1 << 20,
        COMPONENT_RIGIDBODY = 1 << 21,
    }

    public interface IComponent
    {
        ComponentTypes ComponentType
        {
            get;
        }
    }
}
