using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GameEngine.Managers;
using GameEngine.Scripts;

namespace OpenGL_Game.Scripts
{
    //Author: <C.L>
    /// <summary>
    /// 
    /// </summary>
    class DroneTrackerScript : Script
    {
        private SceneManager _sceneManager;

        private int _totalDrones;
        private int _deadDrones;

        public DroneTrackerScript(SceneManager sceneManager, int totalDrones) : base()
        {
            _sceneManager = sceneManager;
            _totalDrones = totalDrones;
            _deadDrones = 0;
        }

        public int DeadDrones
        {
            get
            {
                return _deadDrones;
            }
            set
            {
                _deadDrones = value;
            }
        }

        public override void OnUpdate(float pDelta)
        {
            if (_deadDrones == _totalDrones)
            {
                _sceneManager.SetScene("LevelComplete");
            }

            base.OnUpdate(pDelta);
        }
    }
}
