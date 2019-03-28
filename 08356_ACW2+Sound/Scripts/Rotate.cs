using GameEngine.Components;
using GameEngine.Scripts;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Scripts
{
    //Ben Mullenger
    class Rotate : Script
    {
        private Vector3 _speed;

        public Rotate(Vector3 speed)
        {
            _speed = speed;
        }

        public override void OnUpdate(float pDelta)
        {
            var rot = entity.GetComponent(ComponentTypes.COMPONENT_ROTATION) as ComponentRotation;
            rot.Rotation = new Vector3(rot.Rotation.X + _speed.X * pDelta, rot.Rotation.Y + _speed.Y * pDelta, rot.Rotation.Z + _speed.Z * pDelta);
            //(entity.GetComponent(ComponentTypes.COMPONENT_SCALE) as ComponentScale).Scale *= new Vector3(-0.97f, -0.97f, -0.97f);
            base.OnUpdate(pDelta);
        }
    }
}
