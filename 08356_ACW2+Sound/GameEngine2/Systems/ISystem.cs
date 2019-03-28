using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine;
using GameEngine.Objects;
using GameEngine.Components;

namespace GameEngine.Systems
{
    public interface ISystem
    {
        void OnNewEntity(Entity entity);
        void OnUpdate();
        void OnRender();
        //void OnAction(Entity entity);

        // Property signatures: 
        string Name
        {
            get;
        }

        void OnRemoveEntity(Entity entity);
    }
}
