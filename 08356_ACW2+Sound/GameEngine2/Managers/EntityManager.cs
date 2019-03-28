using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GameEngine.Objects;
using GameEngine.Scripts;
using GameEngine.Components;

namespace GameEngine.Managers
{
    //Altered by Ben Mullenger
    public class EntityManager
    {
        List<Entity> entityList;
        List<Entity> newEntityList;
        List<Entity> toRemoveEntityList;

        public EntityManager()
        {
            entityList = new List<Entity>();
            newEntityList = new List<Entity>();
            toRemoveEntityList = new List<Entity>();
        }

        public void AddEntity(Entity entity)
        {
            Entity result = FindEntity(entity.Name);
            Debug.Assert(result == null, "Entity '" + entity.Name + "' already exists");
            entityList.Add(entity);
            newEntityList.Add(entity);
        }

        private Entity FindEntity(string name)
        {
            return entityList.Find(delegate (Entity e)
            {
                return e.Name == name;
            }
            );
        }

        public  void ClearNew()
        {
            newEntityList.Clear();
        }

        public  void ClearToRemove()
        {
            toRemoveEntityList.Clear();
        }

        public List<Entity> Entities()
        {
            return entityList;
        }

        public List<Entity> NewEntities()
        {
            return newEntityList;
        }
        public List<Entity> ToRemoveEntities()
        {
            return toRemoveEntityList;
        }

        public  void RemoveEntity(Entity e)
        {
            entityList.Remove(e);
            newEntityList.Remove(e);
            toRemoveEntityList.Add(e);
        }

        public void Update(float pDelta)
        {
            //foreach (var e in entityList)
            //{
            //    if (e.HasMask(Components.ComponentTypes.COMPONENT_SCRIPT))
            //    {
            //        foreach (ComponentScript s in e.GetComponents(Components.ComponentTypes.COMPONENT_SCRIPT))
            //        {
            //            s.script.OnUpdate(pDelta);
            //        }
            //    }
            //}

            for (int i = 0; i < entityList.Count; i++)
            {
                if (entityList[i].HasMask(Components.ComponentTypes.COMPONENT_SCRIPT))
                {
                    foreach (ComponentScript s in entityList[i].GetComponents(Components.ComponentTypes.COMPONENT_SCRIPT))
                    {
                        s.script.OnUpdate(pDelta);
                    }
                }
            }
        }

        public  void Render()
        {
            foreach (var e in entityList)
            {
                if (e.HasMask(Components.ComponentTypes.COMPONENT_SCRIPT))
                {
                    foreach (ComponentScript s in e.GetComponents(Components.ComponentTypes.COMPONENT_SCRIPT))
                    {
                        s.script.OnRender();
                    }
                }
            }
        }
    }
}
