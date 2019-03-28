using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Components
{
    public class ComponentLineCollider : IComponent
    {

        //Ben Mullenger , Callum Lenton
        public ComponentTypes ComponentType
        {
            get => ComponentTypes.COMPONENT_LINE_COLLIDER;
        }
        
        public Vector3 Start;
        public Vector3 End;
        public Vector3 Offset;
        public float Radius;

        public ComponentLineCollider(float radius, Vector3 start, Vector3 end)
            : this (radius, start, end, Vector3.Zero)
        {

        }

        public ComponentLineCollider(float radius, Vector3 start, Vector3 end, Vector3 offset)
        {
            Radius = radius;
            Start = start;
            End = end;
            Offset = offset;
        }
    }
}
