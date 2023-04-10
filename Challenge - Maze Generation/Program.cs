using System;

namespace Challenges.MazeGeneration
{
    class Program
    {
        public static Program instance = new Program();

        public Maze maze = new Maze();

        static void Main(string[] args)
        {
            Helper.SetupWindow();
            
            instance.Loop();
        }

        private void Loop()
        {
            while (true)
            {
                maze.Setup();

                maze.Generate();

                maze.Solve();

                Console.Clear();
            }
        }
    }
}
