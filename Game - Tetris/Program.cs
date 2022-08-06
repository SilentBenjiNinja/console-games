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
     * group blocks in 7 shapes: L, periscope, T, dog left, dog right, I, box
     * define difficulty levels with different tick speeds
     * 
     */

    class Program
    {
        const int WINDOW_WIDTH = 22;
        const int WINDOW_HEIGHT = 22;
        const string GAME_TITLE= "TETRIS";

        static Program instance = new Program();

        bool isRunning = false;
        bool gameOver = false;

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
            // whatever happens before the game can be played
        }
    }
}
