using System;
using System.Collections.Generic;
using OpenTK;
using GameEngine.Scripts;
using GameEngine.Components;

namespace GameEngine.Objects
{
    public delegate void Method();
    //Ben Mullenger
    /// <summary>
    /// Object containing all the entities of a scene, as well as information about the camera and projections of the scene
    /// </summary>
    public class Scene
    {

        public Matrix4 View;
        public Matrix4 Perspective;
        public Matrix4 Orthographic;
        public int CameraIndex;

        public List<Entity> Entities = new List<Entity>();

        public Method OnSet;//Called when this scene is set as the current scene
        public Method OnRemove;//Called when this is is removed from being the current scene

        public Scene(Matrix4 pPerspective, Matrix4 pOrthographic, Method pOnSet, Method pOnRemove)
        {
            Perspective = pPerspective;
            Orthographic = pOrthographic;
            View = Matrix4.Identity;
            OnSet += pOnSet;
            OnRemove = pOnRemove;
        }

        public void AddEntity(Entity pEntity)
        {
            Entities.Add(pEntity);
            if(pEntity.HasMask(Components.ComponentTypes.COMPONENT_SCRIPT))
            {
                foreach (ComponentScript s in pEntity.GetComponents(Components.ComponentTypes.COMPONENT_SCRIPT))
                {
                    s.script.OnAddToScene();
                }
            }
        }

        public void RemoveEntity(Entity pEntity)
        {
            Entities.Remove(pEntity);
            if (pEntity.HasMask(Components.ComponentTypes.COMPONENT_SCRIPT))
            {
                foreach (ComponentScript s in pEntity.GetComponents(Components.ComponentTypes.COMPONENT_SCRIPT))
                {
                    s.script.OnRemovedFromScene();
                }
            }
        }
    }
}
