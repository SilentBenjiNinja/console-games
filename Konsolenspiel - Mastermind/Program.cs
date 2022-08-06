using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Mastermind
{
    enum Colors
    {
        Red,
        Orange,
        Yellow,
        Green,
        Blue,
        Violet,
        Size,
    }

    class Program
    {
        public static Program instance = new Program();

        private bool isRunning;
        private bool debug = false;

        private int gameOver;
        private int attempt;
        private Colors[] goalColors;
        private Colors[,] guesses;
        private Colors[] currentGuess;

        private const int MAXATTEMPTS = 10;
        private const int PLACES = 4;
        private const bool REUSECOLORS = false;

        static void Main(string[] args)
        {
            Console.Title = "MASTERMIND";
            Console.SetWindowSize(70, 60);

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

            while (gameOver == 0)
            {
                Update();
                Draw();
                ReadInput();
            }

            while (true)
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                    return;
        }

        void Update()
        {
            if (attempt > 0)
            {
                for (int i = 0; i < PLACES; i++)
                {
                    guesses[attempt - 1, i] = currentGuess[i];
                }
                CheckWinCondition();
            }

            attempt++;
        }

        void Draw()
        {
            Console.Clear();
            Console.ResetColor();
            Console.WriteLine();

            DrawGameRules();
            Console.WriteLine();

            DrawCurrentBoard();

            DrawInputPrompts();
        }

        void ReadInput()
        {
            if (gameOver == 0)
            {
                string input = Console.ReadLine();
                if (ValidInput(input))
                {
                    currentGuess = new Colors[PLACES];
                    for (int i = 0; i < input.Length; i++)
                        currentGuess[i] = ColorFromInput(input.ToCharArray()[i]);
                }
                else
                {
                    Console.WriteLine($"Invalid input: \"{input}\"\nOnly use the letters shown above and make it exactly {PLACES}!");
                    ReadInput();
                }
            }
        }

        void NewBoard()
        {
            gameOver = 0;
            attempt = 0;

            guesses = new Colors[MAXATTEMPTS, PLACES];

            FillGoalsArray();
        }

        void CheckWinCondition()
        {
            if (attempt < MAXATTEMPTS)
            {
                gameOver = 1;
                for (int i = 0; i < PLACES; i++)
                    if (currentGuess[i] != goalColors[i])
                        gameOver = 0;
            }
            else
            {
                gameOver = 2;
            }

        }

        private void DrawGameRules()
        {
            Console.Write("  [0] ".PadRight(6));

            if (debug || gameOver > 0)
                foreach (var c in goalColors)
                    Console.Write(" (" + c.ToString().ToCharArray()[0].ToString() + ") ", Console.ForegroundColor = FromColor(c));
            else
                foreach (var c in goalColors)
                    Console.Write(" (?) ");

            Console.WriteLine();
            Console.WriteLine();

            Console.ResetColor();
            Console.WriteLine($"  Find the sequence of {PLACES} colors!");
            if (REUSECOLORS)
                Console.WriteLine("  HINT: A color can occur multiple times.");
            else
                Console.WriteLine("  HINT: Colors can only occur once within the sequence.");
        }

        void DrawCurrentBoard()
        {
            for (int i = 0; i < attempt - 1; i++)
            {
                Console.ResetColor();
                Console.Write($"  [{i + 1}] ".PadRight(6));
                for (int c = 0; c < PLACES; c++)
                {
                    Colors color = guesses[i, c];

                    Console.Write(" (" + color.ToString().ToCharArray()[0].ToString() + ") ", Console.ForegroundColor = FromColor(color));
                }

                Console.ResetColor();
                Console.Write(" | ");

                List<int> truePos = new List<int>();
                List<int> trueCol = new List<int>();
                Colors[] cols = goalColors.Clone() as Colors[];

                for (int c = 0; c < PLACES; c++)
                {
                    if (guesses[i, c] == cols[c])
                    {
                        truePos.Add(c);
                        cols[c] = Colors.Size;
                    }
                }

                for (int c = 0; c < PLACES; c++)
                {
                    if (truePos.Contains(c)) continue;
                    if (cols.Contains(guesses[i, c]))
                    {
                        trueCol.Add(c);
                        for (int j = 0; j < PLACES; j++)
                        {
                            if (cols[j] == guesses[i, c])
                            {
                                cols[j] = Colors.Size;
                                break;
                            }
                        }
                    }
                }

                foreach (var c in truePos)
                    Console.Write("X ");
                foreach (var c in trueCol)
                    Console.Write("* ");
                for (int c = 0; c < PLACES - (trueCol.Count + truePos.Count); c++)
                    Console.Write(". ");

                Console.WriteLine();
                Console.WriteLine();
            }
        }

        void DrawInputPrompts()
        {
            switch (gameOver)
            {
                case 0:
                    for (int i = 0; i < (int)Colors.Size; i++)
                    {
                        Colors color = (Colors)i;
                        string colorString = color.ToString();
                        string prompt = $"  [{colorString[0]}] {colorString}";
                        Console.Write(prompt, Console.ForegroundColor = FromColor(color));
                        Console.ResetColor();
                        Console.WriteLine();
                    }

                    Console.WriteLine();
                    Console.Write($"  Attempt {attempt}/{MAXATTEMPTS}: ");

                    break;
                case 1:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("  CONGRATULATIONS!");
                    Console.WriteLine($"  You found the sequence within {attempt - 1}/{MAXATTEMPTS} attempts!");
                    break;
                case 2:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("  TOO BAD!");
                    Console.WriteLine($"  You didn't find the sequence within {MAXATTEMPTS} attempts!");
                    break;
            }

            if (gameOver > 0)
                Console.WriteLine("\n  Press [ENTER] to start another game...");
        }

        ConsoleColor FromColor(Colors color)
        {
            switch (color)
            {
                case Colors.Red: return ConsoleColor.Red;
                case Colors.Orange: return ConsoleColor.DarkYellow;
                case Colors.Yellow: return ConsoleColor.Yellow;
                case Colors.Green: return ConsoleColor.Green;
                case Colors.Blue: return ConsoleColor.DarkCyan;
                case Colors.Violet: return ConsoleColor.DarkMagenta;
                default: return ConsoleColor.DarkGray;
            }
        }

        Colors ColorFromInput(char input)
        {
            switch (input)
            {
                case 'r': return Colors.Red;
                case 'o': return Colors.Orange;
                case 'y': return Colors.Yellow;
                case 'g': return Colors.Green;
                case 'b': return Colors.Blue;
                case 'v': return Colors.Violet;
                default: return Colors.Size;
            }
        }

        void FillGoalsArray()
        {
            goalColors = new Colors[PLACES];
            List<Colors> availableColors = new List<Colors>();
            for (int i = 0; i < (int)Colors.Size; i++)
                availableColors.Add((Colors)i);

            Random r = new Random();
            for (int i = 0; i < PLACES; i++)
            {
                Colors color;
                bool newColor = false;

                do
                {
                    color = (Colors)r.Next((int)Colors.Size);
                    if (!REUSECOLORS && availableColors.Contains(color))
                    {
                        newColor = true;
                        availableColors.Remove(color);
                    }
                }
                while (!REUSECOLORS && !newColor);

                goalColors[i] = color;
            }
        }

        bool ValidInput(string input)
        {
            bool ret = false;
            if (input.Length == PLACES)
            {
                char[] colors = input.ToLower().ToCharArray();
                ret = true;

                foreach (var c in colors)
                {
                    if (ColorFromInput(c) == Colors.Size)
                    {
                        ret = false;
                        break;
                    }

                }
            }
            return ret;
        }
    }
}