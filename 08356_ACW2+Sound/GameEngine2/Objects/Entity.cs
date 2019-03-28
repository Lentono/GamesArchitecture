using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GameEngine.Components;
using GameEngine.Scripts;
using GameEngine.Managers;

namespace GameEngine.Objects
{
    //Altered by Ben Mullenger
    public class Entity
    {
        string name;
        List<IComponent> componentList = new List<IComponent>();
        ComponentTypes mask;
        
        public static List<Entity> ToDestroy = new List<Entity>();


        public Entity(string name)
        {
            this.name = name;
        }

        /// <summary>Adds a single component</summary>
        public void AddComponent(IComponent component)
        {
            Debug.Assert(component != null, "Component cannot be null");

            componentList.Add(component);
            mask |= component.ComponentType;

            if(component.ComponentType == ComponentTypes.COMPONENT_SCRIPT)
            {
                (component as ComponentScript).script.SetEntity(this);
                (component as ComponentScript).script.OnAddToEntity();
            }
        }

        public IComponent GetComponent(ComponentTypes pType)
        {
            return Components.Find(delegate (IComponent component)
            {
                return component.ComponentType == pType;
            });
        }

        public List<IComponent> GetComponents(ComponentTypes pType)
        {
            return Components.FindAll(delegate (IComponent component)
            {
                return component.ComponentType == pType;
            });
        }

        public void RemoveComponent(ComponentTypes type)
        {
            mask -= type;

            Components.Remove(Components.Find(delegate (IComponent component)
            {
                return component.ComponentType == type;
            }));
        }

        public bool HasMask(ComponentTypes pMask)
        {
            return (mask & pMask) == pMask;
        }

        public String Name
        {
            get { return name; }
        }

        public ComponentTypes Mask
        {
            get { return mask; }
        }

        public List<IComponent> Components
        {
            get { return componentList; }
        }

        public void Destroy()
        {
            if(HasMask(ComponentTypes.COMPONENT_SCRIPT))
            {
                foreach (ComponentScript s in GetComponents(ComponentTypes.COMPONENT_SCRIPT))
                {
                    s.script.OnDestroy();
                }
            }
            ToDestroy.Add(this);
        }

        public void OnDestroyed()
        {
            componentList.Clear();
        }
    }
}
