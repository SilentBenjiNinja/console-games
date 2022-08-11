using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// https://tildesites.bowdoin.edu/~echown/courses/210/javalab9/TetrisAssignment.pdf
namespace Games.UnnamedConsoleGame
{
    class Program
    {
        const string GAME_TITLE = "UNNAMED CONSOLE GAME WINDOW";

        const int BOARD_WIDTH = 10;
        const int BOARD_HEIGHT = 20;
        const int BORDER_WIDTH = 1;
        const int BORDER_HEIGHT = 1;
        const int SIDE_PANEL_WIDTH = 10;
        const int TOP_PANEL_HEIGHT = 5;

        int WindowWidth => 2 * BOARD_WIDTH + 2 * BORDER_WIDTH + 4 * SIDE_PANEL_WIDTH;
        int WindowHeight => BOARD_HEIGHT + 2 * BORDER_HEIGHT + 2 * TOP_PANEL_HEIGHT;
        int TickTime => 100;

        static Program instance = new Program();

        bool isRunning = false;
        bool gameOver = false;

        Thread gameTickThread;
        readonly object cursorLock = new object();
        int currentTick;

        static void Main(string[] args)
        {
            Console.Title = GAME_TITLE;

            Console.SetWindowSize(instance.WindowWidth, instance.WindowHeight);
            Console.CursorVisible = false;

            instance.StartProgram();
        }

        private void StartProgram()
        {
            isRunning = true;
            gameTickThread = new Thread(GameTick);

            while (isRunning)
                GameLoop();

            while (true)
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                    return;
        }

        private void GameLoop()
        {
            SetupGame();

            gameTickThread.Start();

            InputLoop();

            gameTickThread.Abort();

            while (true)
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                    return;
        }

        private void SetupGame()
        {
            currentTick = 0;
            gameOver = false;

            DrawBordersAndDebug();
            DrawCheckerboard();
        }

        private void GameTick()
        {
            while (!gameOver)
            {
                currentTick++;
                Thread.Sleep(TickTime);

                lock (cursorLock)
                {
                    Console.SetCursorPosition(6, WindowHeight - 2);
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(currentTick);
                }
            }
        }

        private void InputLoop()
        {
            while (!gameOver)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    default:
                        lock (cursorLock)
                        {
                            Console.SetCursorPosition(8, WindowHeight - 3);
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Write(keyInfo.Key.ToString().PadRight(12));
                        }
                        break;
                }
            }
        }

        private void DrawCheckerboard()
        {
            for (int i = 0; i < BOARD_WIDTH; i++)
            {
                for (int j = 0; j < BOARD_HEIGHT; j++)
                {
                    if ((i + j) % 2 == 0)
                    {
                        DrawPixelAt(ConsoleColor.DarkRed, i, j);
                    }
                }
            }
        }

        private void DrawBordersAndDebug()
        {
            Console.SetCursorPosition(SIDE_PANEL_WIDTH * 2, TOP_PANEL_HEIGHT);
            Console.Write('╔' + new string('═', BOARD_WIDTH * 2) + '╗');
            for (int i = 0; i < BOARD_HEIGHT; i++)
            {
                Console.SetCursorPosition(SIDE_PANEL_WIDTH * 2, TOP_PANEL_HEIGHT + i + BORDER_HEIGHT);
                Console.Write('║' + new string(' ', BOARD_WIDTH * 2) + '║');
            }
            Console.SetCursorPosition(SIDE_PANEL_WIDTH * 2, TOP_PANEL_HEIGHT + BOARD_HEIGHT + BORDER_HEIGHT);
            Console.Write('╚' + new string('═', BOARD_WIDTH * 2) + '╝');

            Console.SetCursorPosition(1, WindowHeight - 4);
            Console.Write("DEBUG:");
            Console.SetCursorPosition(1, WindowHeight - 3);
            Console.Write("Input: N/A");
            Console.SetCursorPosition(1, WindowHeight - 2);
            Console.Write("Tick 0");
        }

        private void DrawPixelAt(ConsoleColor color, int x, int y)
        {
            Console.SetCursorPosition(SIDE_PANEL_WIDTH * 2 + BORDER_WIDTH + x * 2, TOP_PANEL_HEIGHT + BORDER_HEIGHT + y);
            Console.BackgroundColor = color;
            Console.Write("  ");
        }
    }
}
