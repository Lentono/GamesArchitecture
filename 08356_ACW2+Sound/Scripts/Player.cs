using GameEngine.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using GameEngine.Objects;
using GameEngine.Components;
using GameEngine.Scripts;
using OpenTK;

namespace OpenGL_Game.Scripts
{
    //Ben Mullenger
    //Contributions by Callum Lenton
    class Player : Script
    {
        private int _health = 3;
        private int _bulletSpeed;
        private int _bulletsFired;
        private Vector3 _startPos;
        private SceneManager _sceneManager;

        private Entity _droneCollision;

        public int Health
        {
            get
            {
                return _health;
            }
            set
            {
                _health = value;
                HealthDisplay = "Health : ";

                for (int i = 0; i < _health; i++)
                {
                    HealthDisplay += " | ";
                }
            }
        }
        public string HealthDisplay
        {
            get;
            private set;
        } = "Health :  |  |  |";

        private int _ammo = 3;
        public int Ammo
        {
            get
            {
                return _ammo;
            }
            set
            {
                _ammo = value;
                AmmoDisplay = "Ammo :";

                for (int i = 0; i < _ammo; i++)
                {
                    AmmoDisplay += "|";
                }
            }
        }
        public string AmmoDisplay
        {
            get;
            private set;
        } = "Ammo :|||";

        public Player(int startHealth, int startAmmo, int bulletSpeed, Vector3 startPos, SceneManager sceneManager)
        {
            Health = startHealth;
            Ammo = startAmmo;
            _bulletSpeed = bulletSpeed;
            _bulletsFired = 0;

            _startPos = startPos;
            _sceneManager = sceneManager;

            _droneCollision = sceneManager.Scenes["Main"].Entities.Find(delegate (Entity e)
            {
                return e.Name == "MusicObject2";
            });
        }

        private void ToggleGodMode()
        {
            (entity.GetComponent(ComponentTypes.COMPONENT_RIGIDBODY) as ComponentRigidbody).IsKinematic = !(entity.GetComponent(ComponentTypes.COMPONENT_RIGIDBODY) as ComponentRigidbody).IsKinematic;
            if(!(entity.GetComponent(ComponentTypes.COMPONENT_RIGIDBODY) as ComponentRigidbody).IsKinematic)
            {
                var pos = (entity.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition);
                pos.Position = new Vector3(pos.Position.X, 1.25f, pos.Position.Z);
                var cam = (entity.GetComponent(ComponentTypes.COMPONENT_CAMERA)) as ComponentCamera;
                if(cam != null)
                {
                    cam.CameraType = CameraTypes.FPS;
                }
            }
            else
            {
                var cam = (entity.GetComponent(ComponentTypes.COMPONENT_CAMERA)) as ComponentCamera;
                if (cam != null)
                {
                    cam.CameraType = CameraTypes.Flying;
                }
            }
        }

        public override void OnSceneAdded()
        {
            InputManager.AddKeyboardTrigger(OpenTK.Input.Key.C, ButtonPosition.PressedOnce, ToggleGodMode);
            InputManager.AddMouseTrigger(OpenTK.Input.MouseButton.Left, ButtonPosition.PressedOnce, ShootBullet);
            base.OnSceneAdded();
        }
        public override void OnSceneRemoved()
        {
            InputManager.RemoveKeyboardTrigger(OpenTK.Input.Key.C, ButtonPosition.PressedOnce, ToggleGodMode);
            InputManager.RemoveMouseTrigger(OpenTK.Input.MouseButton.Left, ButtonPosition.PressedOnce, ShootBullet);
            base.OnSceneRemoved();
        }

        public override void OnAddToScene()
        {
            base.OnAddToScene();
        }

        public override void OnRemovedFromScene()
        {
            base.OnRemovedFromScene();
        }

        public override void OnCollision(Entity other)
        {
            if(other.HasMask(ComponentTypes.COMPONENT_SCRIPT))
            {
                var scripts = other.GetComponents(ComponentTypes.COMPONENT_SCRIPT);
                bool isPickUp = false;
                foreach(ComponentScript p in scripts)
                {
                    if(p.script is PickUp)
                    {
                        ComponentAudio audio = other.GetComponent(ComponentTypes.COMPONENT_AUDIO) as ComponentAudio;
                        audio.SetAudioBuffer("collectable-pickup", false);

                        isPickUp = true;
                        UsePickUp(p.script as PickUp);
                    }
                    if(p.script is DroneMovementScript)
                    {
                        if ((p.script as DroneMovementScript).droneState != DroneMovementScript.DroneStateTypes.Dead && (p.script as DroneMovementScript).droneState != DroneMovementScript.DroneStateTypes.Disabled)
                        {
                            Health--;

                            ComponentAudio audio = _droneCollision.GetComponent(ComponentTypes.COMPONENT_AUDIO) as ComponentAudio;
                            audio.SetAudioBuffer("drone-collide", false);

                            (entity.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition).Position = _startPos;

                            if (Health <= 0)
                            {
                                _sceneManager.SetScene("GameOver");
                            }
                        }
                    }
                }
                if(isPickUp)
                {
                    other.Destroy();
                }
            }
            base.OnCollision(other);
        }

        private void UsePickUp(PickUp p)
        {
            switch (p.type)
            {
                case PickUp.Type.Health:
                    Health += p.Amount;
                    break;
                case PickUp.Type.Ammo:
                    Ammo += p.Amount;
                    break;
                default:
                    break;
            }
        }

        public override void OnUpdate(float pDelta)
        {
            List<Entity> bullets = (_sceneManager.Scenes["Main"].Entities.FindAll(delegate (Entity e)
            {
                return e.Name.Contains("Bullet") == true;
            }));

            foreach (var bullet in bullets)
            {
                if (bullet != null)
                {
                    ComponentPosition position = bullet.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition;
                    ComponentVelocity velocity = bullet.GetComponent(ComponentTypes.COMPONENT_VELOCITY) as ComponentVelocity;

                    position.Position = position.Position + (velocity.Velocity * _bulletSpeed) * pDelta;
                }
            }

            base.OnUpdate(pDelta);
        }

        private void ShootBullet()
        {
            if (_ammo != 0)
            {
                Vector3 forward = (entity.GetComponent(ComponentTypes.COMPONENT_CAMERA) as ComponentCamera).Forward;
                Vector3 position = (entity.GetComponent(ComponentTypes.COMPONENT_POSITION) as ComponentPosition).Position;

                Entity newEntity = new Entity("Bullet" + _bulletsFired);

                newEntity.AddComponent(new ComponentPosition(new Vector3(position.X, 1.7f, position.Z)));
                newEntity.AddComponent(new ComponentRotation());
                newEntity.AddComponent(new ComponentScale(0.0001f, 0.0001f, 0.0001f));
                newEntity.AddComponent(new ComponentVelocity(forward.X, 0.0f, forward.Z));
                newEntity.AddComponent(new ComponentAudio("shoot", false));
                newEntity.AddComponent(new ComponentTexture("spaceship.png"));
                newEntity.AddComponent(new ComponentGeometry("Geometry/Sphere.obj"));
                newEntity.AddComponent(new ComponentSphereCollider(0.1f));
                newEntity.AddComponent(new ComponentScript(new BulletCollisionScript()));

                _sceneManager.AddToScene(newEntity, "Main");

                Ammo--;
                _bulletsFired++;
            }
        }
    }
}
