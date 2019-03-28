using GameEngine.Components;
using GameEngine.Managers;
using GameEngine.Objects;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Systems
{
    //Ben Mullenger
    public class SystemCamera : ISystem
    {
        private const ComponentTypes CAMERA_MASK = (ComponentTypes.COMPONENT_CAMERA | ComponentTypes.COMPONENT_POSITION | ComponentTypes.COMPONENT_ROTATION);

        private List<Entity> _entities = new List<Entity>();
        public string Name
        {
            get
            {
                return "SystemCamera";
            }
        }
        

        public SystemCamera(ViewProjectionManager viewProjectionManager)
        {
            _viewProjectionManager = viewProjectionManager;
        }

        public void OnNewEntity(Entity entity)
        {
            if ((entity.Mask & CAMERA_MASK) == CAMERA_MASK)
            {
                _entities.Add(entity);
            }
        }

        public void OnRemoveEntity(Entity entity)
        {
            _entities.Remove(entity);
        }

        public int CameraNumber;
        private ViewProjectionManager _viewProjectionManager;

        public void OnUpdate()
        {
            for (int i = 0; i < _entities.Count; i++)
            {
                var entity = _entities[i];
                if (!((entity.Mask & CAMERA_MASK) == CAMERA_MASK))
                {
                    //not a camera
                    return;
                }

                var camComp = (ComponentCamera)entity.GetComponent(ComponentTypes.COMPONENT_CAMERA);
                var camPos = ((ComponentPosition)entity.GetComponent(ComponentTypes.COMPONENT_POSITION)).Position;
                var camRot = ((ComponentRotation)entity.GetComponent(ComponentTypes.COMPONENT_ROTATION)).Rotation;

                if (MathHelper.RadiansToDegrees(camRot.Y) > 89)
                    camRot.Y = MathHelper.DegreesToRadians(89);
                else if (MathHelper.RadiansToDegrees(camRot.Y) < -89)
                    camRot.Y = MathHelper.DegreesToRadians(-89);
                
                camComp.Forward = Vector3.Normalize(new Vector3((float)(Math.Cos(camRot.Y) * Math.Cos(camRot.X)), (float)(Math.Sin(camRot.Y)), (float)(Math.Cos(camRot.Y) * Math.Sin(camRot.X))));

                camComp.Up = new Vector3(0, 1, 0);
                camComp.Right = Vector3.Normalize(Vector3.Cross(camComp.Up, camComp.Forward));
                camComp.Up = Vector3.Normalize(Vector3.Cross(camComp.Forward, camComp.Right));

                if (i == CameraNumber)
                {
                    _viewProjectionManager.View = Matrix4.LookAt(camPos + camComp.Offset, camPos + camComp.Offset + camComp.Forward, camComp.Up);
                    _viewProjectionManager.Perspective = camComp.Projection;
                }
            }
        }

        public void OnRender()
        {

        }
    }
}
