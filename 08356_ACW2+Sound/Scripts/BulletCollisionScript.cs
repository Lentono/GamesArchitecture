using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GameEngine.Objects;
using GameEngine.Components;
using GameEngine.Scripts;

namespace OpenGL_Game.Scripts
{
    //Callum Lenton
    class BulletCollisionScript : Script
    {
        public BulletCollisionScript() : base()
        {

        }

        public override void OnCollision(Entity other)
        {
            if (other.Name.Contains("Drone"))
            {
                List<IComponent> scripts = other.GetComponents(ComponentTypes.COMPONENT_SCRIPT);

                foreach (ComponentScript script in scripts)
                {
                    if (script.script is DroneHealthScript)
                    {
                        (script.script as DroneHealthScript).Health--;
                    }
                }
            }


            if (!other.Name.Contains("Camera") && !other.Name.Contains("Node"))
            {
                entity.Destroy();
            }

            base.OnCollision(other);
        }
    }
}
