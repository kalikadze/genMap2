using System;

namespace genMap
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (testGen game = new testGen())
            {
                game.Run();
            }
        }
    }
#endif
}

