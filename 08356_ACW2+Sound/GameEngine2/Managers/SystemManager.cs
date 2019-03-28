using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GameEngine.Systems;
using GameEngine.Objects;

namespace GameEngine.Managers
{
    //Altered by Ben Mullenger
    public class SystemManager
    {
        List<ISystem> systemList = new List<ISystem>();

        public SystemManager()
        {
        }

        public void ManagerEntityChanges(EntityManager entityManager)
        {
            List<Entity> entityList = entityManager.NewEntities();
            foreach(ISystem system in systemList)
            {
                foreach(Entity entity in entityList)
                {
                    system.OnNewEntity(entity);
                }
            }
            entityManager.ClearNew();

            entityList = entityManager.ToRemoveEntities();
            foreach (ISystem system in systemList)
            {
                foreach (Entity entity in entityList)
                {
                    system.OnRemoveEntity(entity);
                }
            }
            entityManager.ClearToRemove();
        }

        public void Update()
        {
            foreach(var s in systemList)
            {
                s.OnUpdate();
            }
        }

        public void Render()
        {
            foreach (var s in systemList)
            {
                s.OnRender();
            }
        }

        public void AddSystem(ISystem system)
        {
            ISystem result = FindSystem(system.Name);
            //Debug.Assert(result != null, "System '" + system.Name + "' already exists");
            systemList.Add(system);
        }

        public ISystem FindSystem(string name)
        {
            return systemList.Find(delegate(ISystem system)
            {
                return system.Name == name;
            }
            );
        }
    }
}
