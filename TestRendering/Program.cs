using System;

namespace TestRendering
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new TrivialGame())
                game.Run();
        }
    }
}