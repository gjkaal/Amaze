using System;

namespace MonoMazeOne.Desktop
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new GameOne())
                game.Run();
        }
    }
}
