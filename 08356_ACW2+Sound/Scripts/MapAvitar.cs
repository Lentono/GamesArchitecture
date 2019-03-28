using GameEngine.Components;
using GameEngine.Objects;
using GameEngine.Scripts;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Scripts
{
    //Ben Mullenger
    class MapAvitar : Script
    {
        private Entity _toRep;
        private Vector3 _rotOffset;
        private bool HasCamera;

        public MapAvitar(Entity toRepresent, Vector3 rotOffset)
        {
            _toRep = toRepresent;
            _rotOffset = rotOffset;
            HasCamera = _toRep.HasMask(ComponentTypes.COMPONENT_CAMERA);
        }

        public override void OnUpdate(float pDelta)
        {
            var rot = (entity.GetComponent(ComponentTypes.COMPONENT_ROTATION) as ComponentRotation);
            var playerRot = (_toRep.GetComponent(ComponentTypes.COMPONENT_ROTATION) as ComponentRotation).Rotation;
            rot.Rotation = new OpenTK.Vector3(0, (HasCamera? -playerRot.X : playerRot.Y), 0) + _rotOffset;
            (entity.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition).Position = (_toRep.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition).Position;
            base.OnUpdate(pDelta);
        }
    }
}
