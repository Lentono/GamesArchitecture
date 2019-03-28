using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using GameEngine.Objects;
using GameEngine.Managers;
using GameEngine.Components;
using GameEngine.Scripts;

namespace OpenGL_Game.Scripts
{
    //Callum Lenton
    class PlayerNodeCollisionScript : Script
    {
        private SceneManager _sceneManager;
        private Entity _previous;

        public PlayerNodeCollisionScript(SceneManager sceneManager) : base()
        {
            _sceneManager = sceneManager;
            _previous = null;
        }

        public override void OnCollision(Entity other)
        {
            if (other != _previous && other.Name.Contains("Node"))
            {
                Console.WriteLine("Collided with node: " + other.Name);

                List<Entity> drones = new List<Entity>();

                drones.AddRange(_sceneManager.Scenes["Main"].Entities.FindAll(delegate (Entity e)
                {
                    return e.Name.Contains("Drone");
                }));

                foreach (var drone in drones)
                {
                    List<IComponent> scripts = drone.GetComponents(ComponentTypes.COMPONENT_SCRIPT);

                    foreach (ComponentScript script in scripts)
                    {
                        if (script.script is DroneMovementScript)
                        {
                            if ((script.script as DroneMovementScript).droneState == DroneMovementScript.DroneStateTypes.Aggressive)
                            {
                                (script.script as DroneMovementScript).AddEntityToPath(other);
                            }
                        }
                    }
                }

                _previous = other;
            }

            base.OnCollision(other);
        }
    }
}
