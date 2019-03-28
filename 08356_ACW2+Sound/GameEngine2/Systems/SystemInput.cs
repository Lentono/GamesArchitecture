using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OpenTK;
using OpenTK.Input;
using GameEngine.Components;
using GameEngine.Objects;
using GameEngine.Managers;

namespace GameEngine.Systems
{
    //Mn. Prod. <B.M>
    //Co. Prod. <C.L> - Scale Component Contribution
    //MJB (contributed 'Collisions' key)
    /// <summary>
    /// System for hadling user input
    /// </summary>
    public class SystemInput : ISystem
    {
        //I think it makes sense to keep all user input in one system public class, but different masks for different component interactions?
        const ComponentTypes INSPECT_MASK = (ComponentTypes.COMPONENT_USER_INSPECT | ComponentTypes.COMPONENT_POSITION | ComponentTypes.COMPONENT_ROTATION | ComponentTypes.COMPONENT_SCALE);
        const ComponentTypes CAMERA_MASK = (ComponentTypes.COMPONENT_CAMERA | ComponentTypes.COMPONENT_POSITION | ComponentTypes.COMPONENT_ROTATION);

        private List<Entity> _entities = new List<Entity>();

        private float _rotSpeed = 0.001f;
        private float _sclSpeed = 0.1f;
        private float _trnSpeed = 0.1f;

        private Dictionary<string, Key> _controls = new Dictionary<string, Key>();

        private Dictionary<string, Key> _defaultControls = new Dictionary<string, OpenTK.Input.Key>()
        {
            { "Forward", Key.Up },        { "Backward", Key.Down },           { "Right", Key.Right },           {"Collisions", Key.C},
            { "Left", Key.Left },          { "RotateRight", Key.Keypad6 }, { "RotateLeft", Key.Keypad4},
            { "RotateUp", Key.Keypad8 },{ "RotateDown", Key.Keypad2 },  { "RollRight", Key.Keypad9},
            { "RollLeft", Key.Keypad7 },{ "ScaleUpX", Key.Number1 },    { "ScaleUpY", Key.Number7},
            { "ScaleUpZ", Key.Number9 },{ "ScaleDownX", Key.Number6},   { "ScaleDownY", Key.Number8},
            { "ScaleDownZ", Key.Number0 }, { "Up", Key.Space },         { "Down", Key.ShiftLeft}
        };

        private Dictionary<Key, bool> _isPressed = new Dictionary<Key, bool>();

        private bool _mouseMoved;
        private Vector2 _mouseChange = Vector2.Zero;

        public string Name
        {
            get
            {
                return "SystemInput";
            }
        }

        public Dictionary<string, Key> Controls { get => _controls; }

        public SystemInput()
        {
            if(!File.Exists("Controls"))
            {
                StreamWriter sw = new StreamWriter("Controls");
                foreach(var c in _defaultControls)
                {
                    sw.WriteLine(c.Key + " | " + (int)c.Value);
                }
                sw.Close();
            }
            StreamReader sr = new StreamReader("Controls");

            var lines = sr.ReadToEnd().Split('\r');
            foreach(var l in lines)
            {
                if(!l.Contains("|"))
                {
                    continue;
                }
                _controls.Add(l.Substring(0, l.IndexOf('|') - 1).Trim(), (Key)int.Parse(l.Substring(l.IndexOf('|') + 1)));
            }
            sr.Close();

            foreach (var c in _controls)
            {
                _isPressed.Add(c.Value, false);
                InputManager.AddKeyboardTrigger(c.Value, ButtonPosition.Held, () => _isPressed[c.Value] = true);
                InputManager.AddKeyboardTrigger(c.Value, ButtonPosition.Released, () => _isPressed[c.Value] = false);
            }
            InputManager.AddMouseMoveTrigger(() =>
            {
                _mouseMoved = true;
                _mouseChange = InputManager.MouseChange;
            });
            InputManager.AddMouseStillTrigger(() =>
            {
                _mouseMoved = false;
                _mouseChange = Vector2.Zero;
            });
        }

        public void OnNewEntity(Entity entity)
        {
            if ((entity.Mask & INSPECT_MASK) == INSPECT_MASK || (entity.Mask & CAMERA_MASK) == CAMERA_MASK)
            {
                _entities.Add(entity);
            }
        }

        public void OnRemoveEntity(Entity entity)
        {
            _entities.Remove(entity);
        }

        public void OnUpdate()
        {
            foreach (var entity in _entities)
            {
                if ((entity.Mask & INSPECT_MASK) == INSPECT_MASK)
                {
                    UserInspect(entity);
                }
                var cam = (ComponentCamera)entity.GetComponent(ComponentTypes.COMPONENT_CAMERA);
                if ((entity.Mask & CAMERA_MASK) == CAMERA_MASK)
                {
                    
                    if (cam.CameraType == CameraTypes.FPS)
                    {
                        UserFirstPerson(entity);
                    }
                    else if(cam.CameraType == CameraTypes.Flying)
                    {
                        UserFlight(entity);
                    }
                    else
                    {
                        //Stationary
                        return;
                    }
                }                
            }
        }

        public void OnRender()
        {

        }

        private void UserFirstPerson(Entity entity)
        {
            var rot = (ComponentRotation)entity.GetComponent(ComponentTypes.COMPONENT_ROTATION);
            var pos = (ComponentPosition)entity.GetComponent(ComponentTypes.COMPONENT_POSITION);
            var cam = (ComponentCamera)entity.GetComponent(ComponentTypes.COMPONENT_CAMERA);

            float rx = 0;
            float ry = 0;
            if (_mouseMoved)
            {
                rx = _mouseChange.X * _rotSpeed;
                ry = _mouseChange.Y* -_rotSpeed;
            }


            float px = BoolTrans("Right");
            float pz = BoolTrans("Backward");
            px -= BoolTrans("Left");
            pz -= BoolTrans("Forward");

           var forward = new Vector3(cam.Forward.X, 0, cam.Forward.Z);

            rot.Rotation = rot.Rotation + new Vector3(rx, ry, 0);
            pos.Position = pos.Position + forward * -pz + cam.Right * -px;
        }

        /// <summary>
        /// Controls for user inspecting an object
        /// </summary>
        /// <param name="entity">Entity to inspect</param>
        private void UserInspect(Entity entity)
        {
            var rot = (ComponentRotation)entity.GetComponent(ComponentTypes.COMPONENT_ROTATION);
            var scl = (ComponentScale)entity.GetComponent(ComponentTypes.COMPONENT_SCALE);
            var pos = (ComponentPosition)entity.GetComponent(ComponentTypes.COMPONENT_POSITION);
            _rotSpeed = ((ComponentUserInspectControl)entity.GetComponent(ComponentTypes.COMPONENT_USER_INSPECT)).RotSpeed;
            _sclSpeed = ((ComponentUserInspectControl)entity.GetComponent(ComponentTypes.COMPONENT_USER_INSPECT)).ScaleSpeed;
            var cam = (ComponentCamera)entity.GetComponent(ComponentTypes.COMPONENT_CAMERA);

            //works out whether to rotate from numpad keys
            float rx = BoolRot("RotateRight");
            float ry = BoolRot("RotateUp");
            float rz = BoolRot("RollRight");
            rx -= BoolRot("RotateLeft");
            ry -= BoolRot("RotateDown");
            rz -= BoolRot("RollLegy");

            float sx = BoolScl("ScaleUpX");
            float sy = BoolScl("ScaleUpY");
            float sz = BoolScl("ScaleUpZ");
            sx -= BoolScl("ScaleDownX");
            sy -= BoolScl("ScaleDownY");
            sz -= BoolScl("ScaleDownZ");

            //Dunno whether we ever want to move in user inspect but will leave for now
            float px = 0, py = 0, pz = 0;

            rot.Rotation = rot.Rotation + new Vector3(rx, ry, rz);
            scl.Scale = scl.Scale + new Vector3(sx, sy, sz);
            pos.Position = pos.Position + new Vector3(px, py, pz);
        }

        /// <summary>
        /// Controls for flying the entity about
        /// </summary>
        /// <param name="entity">entity to fly</param>
        private void UserFlight(Entity entity)
        {
            var rot = (ComponentRotation)entity.GetComponent(ComponentTypes.COMPONENT_ROTATION);
            var pos = (ComponentPosition)entity.GetComponent(ComponentTypes.COMPONENT_POSITION);
            //_rotSpeed = ((ComponentFlightControl)entity.GetComponent(ComponentTypes.COMPONENT_FLIGHT_CONTROL)).RotSpeed;
            //_trnSpeed = ((ComponentFlightControl)entity.GetComponent(ComponentTypes.COMPONENT_FLIGHT_CONTROL)).TrnSpeed;
            var cam = (ComponentCamera)entity.GetComponent(ComponentTypes.COMPONENT_CAMERA);

            float rx = 0;
            float ry = 0;
            if (_mouseMoved)
            {
                rx = _mouseChange.X * _rotSpeed;
                ry = _mouseChange.Y * -_rotSpeed;
            }


            float px = BoolTrans("Right");
            float py = BoolTrans("Up");
            float pz = BoolTrans("Backward");
            px -= BoolTrans("Left");
            py -= BoolTrans("Down");
            pz -= BoolTrans("Forward");

            rot.Rotation = rot.Rotation + new Vector3(rx, ry, 0);
            pos.Position = pos.Position + cam.Forward * -pz + cam.Right * -px + new Vector3(0, py, 0);
        }

        

        /// <summary>
        /// If key pressed returns the rot speed member var
        /// </summary>
        /// <param name="k">key to check</param>
        /// <returns>rot speed if key pressed, 0 otherwise</returns>
        private float BoolRot(string inputName)
        {
            return _isPressed[_controls[inputName]] ? _rotSpeed : 0;
        }



        /// <summary>
        /// Same as above method, just returns the scale speed
        /// </summary>
        /// <param name="key">key to check</param>
        /// <returns>scale speed or 0 if no key is pressed</returns>
        private float BoolScl(string inputName)
        {
            return _isPressed[_controls[inputName]] ? _sclSpeed : 0;
        }

        /// <summary>
        /// Same as above method, just returns the translate speed
        /// </summary>
        /// <param name="key">key to check</param>
        /// <returns>Translate speed or 0 if no key is pressed</returns>
        private float BoolTrans(string inputName)
        {
            return _isPressed[_controls[inputName]] ? _trnSpeed : 0;
        }
    }
}
