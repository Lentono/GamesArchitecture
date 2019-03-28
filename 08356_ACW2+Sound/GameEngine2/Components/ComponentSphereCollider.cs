using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Components
{
    //Ben Mullenger, Callum Lenton
    public class ComponentSphereCollider : IComponent
    {
        public ComponentTypes ComponentType
        {
            get => ComponentTypes.COMPONENT_SPHERE_COLLIDER;
        }
        public Vector3 Offset;
        public float Radius;

        public ComponentSphereCollider(float radius)
            : this (radius, Vector3.Zero)
        {

        }

        public ComponentSphereCollider(float radius, Vector3 offset)
        {
            Radius = radius;
            Offset = offset;
        }

    }
}
