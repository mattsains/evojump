using System;

namespace WindowsGame
{
#if WINDOWS || XBOX
    static class Program
    {
        public static Game1 Game1;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Game1 = new Game1();
            Game1.Run();
        }
    }
#endif
}

