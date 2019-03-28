using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GameEngine.Objects;
using GameEngine.Components;

using OpenTK;
using GameEngine.Managers;
using GameEngine.Scripts;

namespace OpenGL_Game.Scripts
{
    //Callum Lenton
    class DroneHealthScript : Script
    {
        private SceneManager _sceneManager;

        private Entity _droneTracker;

        private int _health;

        public DroneHealthScript(SceneManager sceneManager, int health) : base()
        {
            _sceneManager = sceneManager;

            //Tell the drone tracker there is one less drone
            _droneTracker = _sceneManager.Scenes["Main"].Entities.Find(delegate (Entity e)
            {
                return (e.Name == "DroneTracker");
            });

            _health = health;
        }

        public int Health
        {
            get
            {
                return _health;
            }
            set
            {
                _health = value;

                if (_health == 0)
                {
                    DroneDeath();
                }
            }
        }

        private void DroneDeath()
        {
            ComponentRotation rotation = entity.GetComponent(ComponentTypes.COMPONENT_ROTATION) as ComponentRotation;
            ComponentPosition position = entity.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition;
            ComponentAudio audio = entity.GetComponent(ComponentTypes.COMPONENT_AUDIO) as ComponentAudio;

            DroneMovementScript movement = null;

            List<IComponent> scripts = entity.GetComponents(ComponentTypes.COMPONENT_SCRIPT);

            foreach (ComponentScript script in scripts)
            {
                if (script.script is DroneMovementScript)
                {
                    movement = script.script as DroneMovementScript;
                }
            }

            rotation.Rotation = LookAt(position.Position, new Vector3(position.Position.X, -10.0f, position.Position.Z));
            position.Position = new Vector3(position.Position.X, 0.0f, position.Position.Z);
            audio.SetAudioBuffer("drone-disable", false);

            movement.droneState = DroneMovementScript.DroneStateTypes.Dead;

            ResourceManager.GetPostProccessEffects()["AttackShake"].Active = false;

            entity.RemoveComponent(ComponentTypes.COMPONENT_RIGIDBODY);

            scripts = _droneTracker.GetComponents(ComponentTypes.COMPONENT_SCRIPT);

            foreach (ComponentScript script in scripts)
            {
                if (script.script is DroneTrackerScript)
                {
                    (script.script as DroneTrackerScript).DeadDrones++;
                }
            }
        }

        /// <summary>
        /// Creates a rotation vector that is looking at the target
        /// </summary>
        /// <param name="position">current object position</param>
        /// <param name="targetPosition">targets position</param>
        /// <returns>rotation vector towards target</returns>
        private Vector3 LookAt(Vector3 position, Vector3 targetPosition)
        {
            Vector3 forward = Vector3.Normalize(targetPosition - position);

            float dot = Vector3.Dot(Vector3.UnitZ, forward);

            if (Math.Abs(dot - (-1.0f)) < 0.000001f)
            {
                return Vector3.UnitY * (float)Math.PI;
            }

            if (Math.Abs(dot - (1.0f)) < 0.000001f)
            {
                return new Vector3(0.0f, 0.0f, 0.0f) * 1.0f;
            }

            float angle = (float)Math.Acos(dot);
            Vector3 axis = Vector3.Cross(Vector3.UnitZ, forward);
            axis = Vector3.Normalize(axis);

            return axis * angle;
        }
    }
}
