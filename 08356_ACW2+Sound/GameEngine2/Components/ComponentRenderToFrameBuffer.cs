using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Components
{
    //Ben Mullenger
    public class ComponentRenderToFrameBuffer : IComponent 
    {
        public ComponentTypes ComponentType
        {
            get => ComponentTypes.COMPONENT_RENDER_TO_FRAME_BUFFER;
        }

        public string BufferName;

        public bool RenderToMainBuffer;

        /// <summary>
        /// Component to tell the renderer to render this object to a frame buffer
        /// </summary>
        /// <param name="bufferName">Name of the buffer to render to</param>
        /// <param name="renderToMainBuffer"> Whether this should also be rendered to the main buffer </param>
        public ComponentRenderToFrameBuffer(string bufferName, bool renderToMainBuffer)
        {
            BufferName = bufferName;
            RenderToMainBuffer = renderToMainBuffer;
        }
    }
}
