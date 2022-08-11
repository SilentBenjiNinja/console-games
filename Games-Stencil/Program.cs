using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.UnnamedConsoleGame
{
    class Program
    {
        const int WINDOW_WIDTH = 128;
        const int WINDOW_HEIGHT = 36;
        const string GAME_TITLE = "UNNAMED CONSOLE GAME WINDOW";

        static Program instance = new Program();

        bool isRunning = false;
        bool gameOver = false;

        // for this demo
        Random randy = new Random();

        static void Main(string[] args)
        {
            Console.Title = GAME_TITLE;
            Console.SetWindowSize(WINDOW_WIDTH, WINDOW_HEIGHT);
            Console.CursorVisible = false;

            instance.StartProgram();
        }

        private void StartProgram()
        {
            isRunning = true;

            while (isRunning)
                GameLoop();

            Console.WriteLine("end of program! press [ENTER] to close...");

            while (true)
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                    return;
        }

        private void GameLoop()
        {
            SetupGame();

            while (!gameOver)
            {
                Update();
                Draw();
                ReadInput();
            }

            Console.WriteLine("game over! press [ENTER] to restart...");

            while (true)
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                    return;
        }

        private void Draw()
        {
            // draw what is being simulated for this frame
            Console.WriteLine("drawing...");
        }

        private void ReadInput()
        {
            if (!gameOver)
            {
                Console.WriteLine("waiting for player input...");

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    // process key input for this frame
                    default:
                        Console.WriteLine($"processing key {keyInfo.Key}...");
                        return;
                }
            }
        }

        private void Update()
        {
            // update simulation for this frame
            Console.WriteLine("updating simulation...");

            if (randy.NextDouble() < 0.1)
            {
                gameOver = true;

                Console.WriteLine("game over condition!");

                if (randy.NextDouble() < 0.5)
                {
                    isRunning = false;

                    Console.WriteLine("program end condition!");
                }
            }
        }

        private void SetupGame()
        {
            gameOver = false;

            // whatever happens before the game can be played
            Console.WriteLine("setting up game...");
        }

    }
}