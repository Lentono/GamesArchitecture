#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace OpenGL_Game
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class MainEntry
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Logger.Initialise();
            using (var game = new MyGame())
                game.Run();
            Logger.Close();
        }
    }
#endif
}
