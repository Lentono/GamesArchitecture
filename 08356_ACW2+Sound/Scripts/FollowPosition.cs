using GameEngine.Components;
using GameEngine.Objects;
using GameEngine.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Scripts
{
    //Ben Mullenger
    class FollowPosition : Script
    {
        private Entity _entity;

        public FollowPosition(Entity entity)
        {
            _entity = entity;
        }

        public override void OnUpdate(float pDelta)
        {
            var pos = (entity.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition);
            pos.Position = (_entity.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition).Position;
            base.OnUpdate(pDelta);
        }
    }
}
