using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Objects
{
    //Ben Mullenger
    public class FrameBuffer
    {
        public int Location;

        public string Name;

        public Matrix4 Perspective;
        public Matrix4 Orthographic;
        public Matrix4 View;

        public Vector4 ClearColour;

        public FrameBuffer(string name, Texture colourTexture, Texture depthTexture, Matrix4 perspective, Matrix4 orthographic, Matrix4 view, Vector4 clearColour)
        {
            Perspective = perspective;
            Orthographic = orthographic;
            View = view;
            ClearColour = clearColour;

            Name = name;
            Location = GL.GenFramebuffer();
            Bind();

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, colourTexture.TextureLocation, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, depthTexture.TextureLocation, 0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception("Frame buffer failed to be created :: " + GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer).ToString());
            }

            Unbind();
        }

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Location);
        }

        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void CleanUp()
        {
            Unbind();
            GL.DeleteFramebuffer(Location);
        }
    }
}
