using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Managers
{
    //Ben Mullenger
    public class ViewProjectionManager
    {
        public Matrix4 View, Perspective, Orthographic;

        public ViewProjectionManager(Matrix4 view, Matrix4 perspective, Matrix4 orthographic)
        {
            View = view;
            Perspective = perspective;
            Orthographic = orthographic;
        }
    }
}
