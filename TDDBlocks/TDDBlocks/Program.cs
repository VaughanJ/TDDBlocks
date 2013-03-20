using System;
using TDDBlocks.Logging;

namespace TDDBlocks
{
#if WINDOWS || XBOX
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        {
            try
            {
                using (var game = new Game1())
                {
                    game.Run();
                }
            }
            catch (Exception ex)
            {
                // Log untrapped error
                Log.Instance.Fatal("An unhandled error has stopped the game dead in it's tracks. Error : " + ex.ToString() + ".");
            }
        }
    }
#endif
}