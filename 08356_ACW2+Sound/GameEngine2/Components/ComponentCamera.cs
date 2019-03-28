using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Components
{

    public enum CameraTypes
    {
        FPS,
        Stationary,
        Flying
    }
    //Ben Mullenger
    public class ComponentCamera : IComponent
    {
        public ComponentTypes ComponentType
        {
            get
            {
                return ComponentTypes.COMPONENT_CAMERA;
            }
        }

        private static int NUM_CAMS = 0;

        /// <summary>
        /// The index number of the camera
        /// </summary>
        public int Num
        {
            get;
            private set;
        }
        /// <summary>
        /// Positional offset from the entity
        /// </summary>
        public Vector3 Offset
        {
            get;
            private set;
        }
        public Vector3 Forward
        {
            get;
            set;
        }

        public Vector3 Up
        {
            get;
            set;
        }

        public Vector3 Right
        {
            get;
            set;
        }
        public CameraTypes CameraType
        {
            get;
            set;
        }
        public bool Changed
        {
            get;
            set;
        }

        public Matrix4 Projection
        {
            get;
            private set;
        }

        public ComponentCamera(CameraTypes pCameraType, float pFov, float pRenderLong, float pRenderShort, float pWidth, float pHeight, Vector3 pOffset)
        {
            CameraType = pCameraType;
            Changed = true;
            Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(pFov), pWidth / pHeight, pRenderShort, pRenderLong);
            Num = NUM_CAMS;
            Offset = pOffset;
            NUM_CAMS++;
        }
    }
}
