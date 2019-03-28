using GameEngine.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Scripts
{
    //Ben Mullenger
    public abstract class Script
    {
        protected Entity entity;

        public void SetEntity(Entity pEntity)
        {
            entity = pEntity;
        }

        public virtual void OnAddToEntity()
        {

        }

        public virtual void OnSceneAdded()
        {

        }

        public virtual void OnSceneRemoved()
        {

        }

        public virtual void OnAddToScene()
        {

        }

        public virtual void OnRemovedFromScene()
        {

        }

        public virtual void OnLeftClicked()
        {

        }

        public virtual void OnLeftClickHeld()
        {

        }

        public virtual void OnMouseColide()
        {

        }

        public virtual void OnRightClicked()
        {

        }

        public virtual void OnRightClickHeld()
        {

        }

        public virtual void OnDestroy()
        {
        }

        public virtual void OnUpdate(float pDelta)
        {

        }

        public virtual void OnRender()
        {

        }

        public virtual void OnCollision(Entity other)
        {

        }
    }
}
