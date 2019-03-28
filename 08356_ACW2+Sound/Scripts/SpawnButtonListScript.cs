using GameEngine.Components;
using GameEngine.Managers;
using GameEngine.Objects;
using GameEngine.Scripts;
using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGL_Game.Scripts
{
    //Ben Mullenger
    class SpawnButtonListScript : Script
    {
        public SpawnButtonListScript(SceneManager sceneManager, string sceneName, Dictionary<string, Key> controls, Texture font) :base()
        {
            int count = 0;
            foreach(var k in controls)
            {
                var e = new Entity("Control Guide " + k.Key);
                e.AddComponent(new ComponentScale(8));
                e.AddComponent(new ComponentGeometry("Geometry/SquareGeometry.txt"));
                e.AddComponent(new ComponentRotation());
                e.AddComponent(new ComponentUI());
                e.AddComponent(new ComponentPosition(new Vector3(400, 20 + 30 * count, 0)));
                e.AddComponent(new ComponentText(k.Key + " : " + k.Value.ToString(), font));
                sceneManager.AddToScene(e, sceneName);
                count++;
            }
        }
    }
}
