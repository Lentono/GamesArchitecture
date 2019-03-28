using GameEngine.Objects;
using System;
using System.Collections.Generic;
using OpenTK;
using GameEngine.Scripts;
using GameEngine.Components;

namespace GameEngine.Managers
{
    //Ben Mullenger
    /// <summary>
    /// Manager of scenes within the game
    /// </summary>
    public class SceneManager
    {
        private EntityManager _entityManager;
        private Scene _current;
        private Dictionary<string, Scene> _scenes = new Dictionary<string, Scene>();

        public SceneManager(EntityManager pEntityManager)
        {
            _entityManager = pEntityManager;
            _current = null;
        }
        /// <summary>
        /// Add a new scene
        /// </summary>
        /// <param name="pName">Name of the new scene, used to reference in the future</param>
        /// <param name="pScene">New Scene</param>
        public void AddScene(string pName, Scene pScene)
        {
            _scenes.Add(pName,pScene);
        }
        /// <summary>
        /// Set the current scene
        /// </summary>
        /// <param name="pName">Name of the scene to set to current</param>
        public void SetScene(string pName)
        {
            if(_current != null)
            {
                foreach(var e in _current.Entities)
                {
                    _entityManager.RemoveEntity(e);
                    var scripts = e.GetComponents(Components.ComponentTypes.COMPONENT_SCRIPT);
                    foreach (ComponentScript s in scripts)
                    {
                        s.script.OnSceneRemoved();
                    }
                }
                _current.OnRemove?.Invoke();
            }

            _current = _scenes[pName];
            _current.OnSet?.Invoke();

            foreach (var e in _current.Entities)
            {
                _entityManager.AddEntity(e);
                var scripts = e.GetComponents(Components.ComponentTypes.COMPONENT_SCRIPT);
                foreach (ComponentScript s in scripts)
                {
                    s.script.OnSceneAdded();
                }
            }
        }

        public  void SetOrthographic(Matrix4 value)
        {
            _current.Orthographic = value;
        }

        public  void SetPerspective(Matrix4 value)
        {
            _current.Perspective = value;
        }

        public  Matrix4 GetPerspective()
        {
            return _current.Perspective;
        }

        public  void SetView(Matrix4 value)
        {
            _current.View = value;
        }

        public  Matrix4 GetView()
        {
            return _current.View;
        }

        public  Matrix4 GetOrthographic()
        {
            return _current.Orthographic;
        }

        public  int GetCameraNum()
        {
            return _current.CameraIndex;
        }

        public  void SetCameraNum(int value)
        {
            _current.CameraIndex = value;
        }
        /// <summary>
        /// Add entity to scene
        /// </summary>
        /// <param name="pEntity">Entity to add</param>
        /// <param name="pName">Name of the scene to add to, if blank will add entity to the current scene</param>
        public void AddToScene(Entity pEntity, string pName = null)
        {
            if(pName == null)
            {
                _current.AddEntity(pEntity);
                _entityManager.AddEntity(pEntity);
            }
            else
            {
                _scenes[pName].AddEntity(pEntity);
                if(_scenes[pName] == _current)
                {
                    _entityManager.AddEntity(pEntity);
                }
            }
        }
        /// <summary>
        /// Remove and entity from the scene
        /// </summary>
        /// <param name="pEntity">Entity to remove</param>
        /// <param name="pName">Name of the scene to remove from, if blank will add entity to the current scene</param>
        public void RemoveFromScene(Entity pEntity, string pName = null)
        {
            if (pName == null)
            {
                _current.RemoveEntity(pEntity);
            }
            else
            {
                _scenes[pName].RemoveEntity(pEntity);
            }
        }

        //TemporaryTestMethod
        public Dictionary<string, Scene> Scenes
        {
            get
            {
                return _scenes;
            }
        }

        public void CleanUpDestoyed()
        {
            foreach (var e in Entity.ToDestroy)
            {
                foreach(var s in _scenes)
                {
                    s.Value.RemoveEntity(e);
                }
                _entityManager.RemoveEntity(e);
                
                e.OnDestroyed();
            }
            Entity.ToDestroy.Clear();
        }
    }
}
