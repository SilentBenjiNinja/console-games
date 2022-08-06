using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.TicTacToe
{
    class Program
    {
        public static Program instance = new Program();
        bool isRunning;
        int margin = 3;

        readonly int[] winPermutations = { 7, 56, 73, 84, 146, 273, 292, 448 };
        int[,] state;
        bool playerCrossTurn;
        int lastMove;
        int gameOver;   // -1 = not game over; 0 = O won; 1 = X won; 2 = Draw
        int turns;

        static void Main(string[] args)
        {
            Console.Title = "TIC TAC TOE";
            Console.SetWindowSize(instance.margin * 4 + 11, instance.margin * 2 + 8);

            instance.PlayGame();
        }

        void PlayGame()
        {
            isRunning = true;

            while (isRunning)
                GameLoop();

            while (true)
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                    return;
        }

        void GameLoop()
        {
            NewBoard();
            DrawBoard();

            while (gameOver == -1)
            {
                ReadInput();
                Update();
                Draw();
            }

            while (true)
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                    return;
        }

        void Update()
        {
            turns++;
            CheckWinCondition();
            playerCrossTurn = !playerCrossTurn;
        }

        void Draw()
        {
            int x = lastMove % 3,
                y = lastMove / 3;

            Console.SetCursorPosition(margin * 2 + 1 + x * 4, margin + y * 2);

            bool isCross = Convert.ToBoolean(state[y, x]);
            SetPlayerColor(isCross);
            Console.Write(PlayerString(isCross));

            Console.SetCursorPosition(0, margin + 7);

            if (gameOver == -1)
                DrawTurnMessage();
            else if (gameOver == 2)
                DrawDrawMessage();
            else
                DrawWinMessage();
        }

        void ReadInput()
        {
            if (gameOver == -1)
            {
                lastMove = -1;
                while (lastMove < 0 || lastMove > 8 || state[lastMove / 3, lastMove % 3] != -1)
                {
                    Console.SetCursorPosition(19, margin + 7);
                    Console.Write(new string(' ', Console.WindowWidth) + '\n');
                    Console.SetCursorPosition(19, margin + 7);

                    if (int.TryParse(Console.ReadLine(), out int result))
                        lastMove = result - 1;
                }
                state[lastMove / 3, lastMove % 3] = Convert.ToInt32(playerCrossTurn);
            }
        }

        void NewBoard()
        {
            gameOver = -1;
            state = new int[3, 3];
            turns = 0;

            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                    state[y, x] = -1;
        }

        void DrawBoard()
        {
            Console.Clear();
            Console.CursorVisible = true;
            Console.ResetColor();

            for (int i = 0; i < margin; i++)
                Console.WriteLine();

            Console.Write("".PadLeft(margin * 2));
            Console.WriteLine("   |   |   ");
            Console.Write("".PadLeft(margin * 2));
            Console.WriteLine("---+---+---");
            Console.Write("".PadLeft(margin * 2));
            Console.WriteLine("   |   |   ");
            Console.Write("".PadLeft(margin * 2));
            Console.WriteLine("---+---+---");
            Console.Write("".PadLeft(margin * 2));
            Console.WriteLine("   |   |   ");

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    Console.SetCursorPosition(margin * 2 + 1 + x * 4, margin + y * 2);

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(y * 3 + x + 1);
                }
            }

            Console.SetCursorPosition(0, margin + 7);
            DrawTurnMessage();
        }

        void CheckWinCondition()
        {
            if (turns >= 9)
                gameOver = 2;

            foreach (var p in winPermutations)
                if ((p & PlayerState()) == p)
                    gameOver = Convert.ToInt32(playerCrossTurn);
        }

        int PlayerState()
        {
            int ret = 0;
            for (int i = 0; i < 9; i++)
            {
                int fieldContent = state[i / 3, i % 3];
                if (fieldContent > -1 && Convert.ToBoolean(fieldContent) == playerCrossTurn)
                    ret += 1 << i;
            }
            return ret;
        }

        string PlayerString(bool isCross)
        {
            return isCross ? "X" : "O";
        }

        void SetPlayerColor(bool isCross)
        {
            Console.ForegroundColor = isCross ? ConsoleColor.Cyan : ConsoleColor.Magenta;
        }

        void DrawTurnMessage()
        {
            Console.Beep(100, 100);

            Console.Write("".PadLeft((Console.WindowWidth - 16) / 2));
            SetPlayerColor(playerCrossTurn);
            Console.Write($"{PlayerString(playerCrossTurn)}'s turn [1-9]: ");
        }

        void DrawWinMessage()
        {
            Console.CursorVisible = false;
            Console.Write("".PadLeft((Console.WindowWidth - 6) / 2));
            SetPlayerColor(!playerCrossTurn);
            Console.WriteLine($"{PlayerString(!playerCrossTurn)} WON!      ");

            Console.Beep(880, 200);
            Console.Beep(1320, 300);
            Console.Beep(1760, 400);

            Console.Write("".PadLeft((Console.WindowWidth - 18) / 2));
            Console.Write("[ENTER] Try again?");
        }

        void DrawDrawMessage()
        {
            Console.CursorVisible = false;
            Console.Write("".PadLeft((Console.WindowWidth - 5) / 2));
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Draw!      ");

            Console.Beep(293, 500);
            Console.Beep(277, 500);
            Console.Beep(261, 500);
            Console.Beep(247, 500);

            Console.Write("".PadLeft((Console.WindowWidth - 18) / 2));
            Console.Write("[ENTER] Try again?");
        }
    }
}
