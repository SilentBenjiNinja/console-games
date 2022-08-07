using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://tildesites.bowdoin.edu/~echown/courses/210/javalab9/TetrisAssignment.pdf
namespace Games.Tetris
{
    /*
     * TODO:
     * draw blocks in different colors
     * group blocks in 7 pieces: L, periscope, T, dog left, dog right, I, box
     * pieces have pivot points
     * define difficulty levels with different tick speeds
     * gameplay loop:
     * - piece comes down at tick speed
     * - can be moved, rotated
     * - on tick where piece overlaps with stack, add to stack at current position instead
     * - when piece tries to add to stack but stack has already reached max height, then game over
     * - when a row has BOARD_WIDTH blocks in it after stacking new piece, clear row and restack rows above
     * - show next piece at side of 
     */

    struct Piece
    {
        public int[][,] permutations;
    }

    class Program
    {
        const string GAME_TITLE = "TETRIS";

        const int BOARD_WIDTH = 10;
        const int BOARD_HEIGHT = 20;
        const int BORDER_WIDTH = 1;
        const int BORDER_HEIGHT = 1;
        const int SIDE_PANEL_WIDTH = 10;
        const int TITLE_PANEL_HEIGHT = 5;

        int WindowWidth => 2 * BOARD_WIDTH + 2 * BORDER_WIDTH + 4 * SIDE_PANEL_WIDTH;
        int WindowHeight => BOARD_HEIGHT + 2 * BORDER_HEIGHT + TITLE_PANEL_HEIGHT;

        Piece[] pieces = {
            // L
            // L mirrored
            // dog left
            // dog right
            // T
            // I
            // square
        };

        static Program instance = new Program();

        bool isRunning = false;
        bool gameOver = false;

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

            while (isRunning)
                GameLoop();

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

            while (true)
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                    return;
        }

        private void Draw()
        {
            // draw what is being simulated for this frame
            DrawStats();
            DrawGameBoard();
        }

        private void DrawGameBoard()
        {
            for (int i = 0; i < BOARD_WIDTH; i++)
            {
                for (int j = 0; j < BOARD_HEIGHT; j++)
                {
                    DrawPixelAt(i * j % 2 == 0 ? ConsoleColor.Yellow : ConsoleColor.DarkGray, i, j);
                }
            }
        }

        private void DrawStats()
        {
            Console.SetCursorPosition(0, TITLE_PANEL_HEIGHT - 1);
            for (int i = 0; i < WindowWidth; i++)
            {
                Console.Write(i % 2 == 0 ? '_' : ' ');
            }
        }

        private void DrawBorders()
        {
            Console.SetCursorPosition(SIDE_PANEL_WIDTH * 2, TITLE_PANEL_HEIGHT);
            Console.Write('╔' + new string('═', BOARD_WIDTH * 2) + '╗');
            for (int i = 0; i < BOARD_HEIGHT; i++)
            {
                Console.SetCursorPosition(SIDE_PANEL_WIDTH * 2, TITLE_PANEL_HEIGHT + i + BORDER_HEIGHT);
                Console.Write('║' + new string(' ', BOARD_WIDTH * 2) + '║');
            }
            Console.SetCursorPosition(SIDE_PANEL_WIDTH * 2, TITLE_PANEL_HEIGHT + BOARD_HEIGHT + BORDER_HEIGHT);
            Console.Write('╚' + new string('═', BOARD_WIDTH * 2) + '╝');
        }

        private void DrawPixelAt(ConsoleColor color, int x, int y)
        {
            Console.SetCursorPosition(SIDE_PANEL_WIDTH * 2 + BORDER_WIDTH + x * 2, TITLE_PANEL_HEIGHT + BORDER_HEIGHT + y);
            Console.BackgroundColor = color;
            Console.Write("  ");
        }

        private void ReadInput()
        {
            if (!gameOver)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    // process key input for this frame
                    default:
                        return;
                }
            }
        }

        private void Update()
        {
            // update simulation for this frame
        }

        private void SetupGame()
        {
            DrawBorders();
        }
    }
}
