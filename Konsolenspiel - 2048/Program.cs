using System;
using System.Collections.Generic;

// TODO: undo functionality

namespace Games._2048
{
    enum Direction { None, Right, Down, Left, Up };

    struct TileType
    {
        public string name;
        public ConsoleColor color;

        public TileType(string name, ConsoleColor color)
        {
            this.name = name;
            this.color = color;
        }
    }

    class Program
    {
        static Program instance = new Program();

        bool isRunning = false;
        bool gameOver = false;

        int[] state;
        int[] prev;
        Direction dir;
        int moves;
        int score;

        Random randy = new Random();
        readonly Dictionary<int, TileType> tileStrings = new Dictionary<int, TileType> {
            {0,  new TileType("       ",  ConsoleColor.Black)},
            {1,  new TileType("   2   ",  ConsoleColor.DarkGray)},
            {2,  new TileType("   4   ",  ConsoleColor.Gray)},
            {3,  new TileType("   8   ",  ConsoleColor.White)},
            {4,  new TileType("  1 6  ",  ConsoleColor.DarkYellow)},
            {5,  new TileType("  3 2  ",  ConsoleColor.Yellow)},
            {6,  new TileType("  6 4  ",  ConsoleColor.DarkGreen)},
            {7,  new TileType(" 1 2 8 ",  ConsoleColor.Green)},
            {8,  new TileType(" 2 5 6 ",  ConsoleColor.DarkCyan)},
            {9,  new TileType(" 5 1 2 ",  ConsoleColor.Cyan)},
            {10, new TileType("1 0 2 4",  ConsoleColor.DarkMagenta)},
            {11, new TileType("2 0 4 8",  ConsoleColor.Magenta)},
            {12, new TileType("4 0 9 6",  ConsoleColor.DarkRed)},
            {13, new TileType("8 1 9 2",  ConsoleColor.Red)},
            {14, new TileType(" 16384 ",  ConsoleColor.Red)},
            {15, new TileType(" 32768 ",  ConsoleColor.Red)},
            {16, new TileType(" 65536 ",  ConsoleColor.Red)},
            {17, new TileType("131072 ",  ConsoleColor.Red)},
        };

        static void Main(string[] args)
        {
            Console.Title = "2048";
            Console.SetWindowSize(37, 22);
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
            DrawTiles();
            DrawStats();
        }

        private void ReadInput()
        {
            if (!gameOver)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                dir = Direction.None;
                switch (keyInfo.Key)
                {
                    case ConsoleKey.RightArrow:
                        dir = Direction.Right;
                        break;
                    case ConsoleKey.LeftArrow:
                        dir = Direction.Left;
                        break;
                    case ConsoleKey.UpArrow:
                        dir = Direction.Up;
                        break;
                    case ConsoleKey.DownArrow:
                        dir = Direction.Down;
                        break;
                    case ConsoleKey.R:
                        SetupGame();
                        return;
                    default:
                        return;
                }
            }
        }

        private void Update()
        {
            if (dir != Direction.None)
            {
                ShiftBoard();
                AddRandomTile();
                CheckWinCondition();
            }
        }

        private void SetupGame()
        {
            state = new int[16];
            prev = new int[16];
            gameOver = false;
            dir = Direction.None;
            moves = 0;
            score = 0;

            DrawBoard();
            AddRandomTile();
            AddRandomTile();
        }

        private void DrawBoard()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.Write(@"
  ╔═══════╦═══════╦═══════╦═══════╗
  ║       ║       ║       ║       ║
  ║       ║       ║       ║       ║
  ║       ║       ║       ║       ║
  ╠═══════╬═══════╬═══════╬═══════╣
  ║       ║       ║       ║       ║
  ║       ║       ║       ║       ║
  ║       ║       ║       ║       ║
  ╠═══════╬═══════╬═══════╬═══════╣
  ║       ║       ║       ║       ║
  ║       ║       ║       ║       ║
  ║       ║       ║       ║       ║
  ╠═══════╬═══════╬═══════╬═══════╣
  ║       ║       ║       ║       ║
  ║       ║       ║       ║       ║
  ║       ║       ║       ║       ║
  ╚═══════╩═══════╩═══════╩═══════╝
");
        }

        private void DrawTiles()
        {
            Console.ResetColor();

            for (int i = 0; i < state.Length; i++)
            {
                if (state[i] != prev[i])
                {
                    Console.ForegroundColor = tileStrings[state[i]].color;
                    Console.SetCursorPosition(3 + (i % 4) * 8, 3 + (i / 4) * 4);
                    Console.Write(tileStrings[state[i]].name);
                }
            }
        }

        private void DrawStats()
        {
            Console.ResetColor();
            Console.SetCursorPosition(2, 19);

            Console.Write("Score: " + score);
            Console.Write(("Moves: " + moves).PadLeft(26 - score.ToString().Length));

            if (gameOver)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\n    GAME OVER! [ENTER] to restart");
            }
        }

        private void ShiftBoard()
        {
            IterationParams(out int st, out int it1, out int it2);
            ApplyMoveToTempArray(st, it1, it2, out int[,] temp, out bool validMove);

            if (validMove)
            {
                moves += 1;
                prev = state.Clone() as int[];

                for (int i = 0; i < 16; i++)
                    state[st + (i % 4) * it1 + (i / 4) * it2] = temp[i / 4, i % 4];
            }
            else
            {
                dir = Direction.None;
            }
        }

        private void ApplyMoveToTempArray(int st, int it1, int it2, out int[,] temp, out bool validMove)
        {
            validMove = false;
            temp = new int[4, 4];
            
            for (int i = 0; i < 4; i++)
            {
                // fill temp array with values from columns / rows in order of shift direction
                for (int j = 0; j < 4; j++)
                    temp[i, j] = state[st + j * it1 + i * it2];

                // shift arrays together
                for (int x = 0; x < 3; x++)
                {
                    // if empty, pull value from next filled tile
                    if (temp[i, x] == 0)
                    {
                        for (int scout1 = x + 1; scout1 < 4; scout1++)
                        {
                            if (temp[i, scout1] == 0) continue;

                            temp[i, x] = temp[i, scout1];
                            temp[i, scout1] = 0;
                            validMove = true;
                            break;
                        }
                    }

                    // then, if not empty, find next filled tile and compare, blend if equal
                    if (temp[i, x] > 0)
                    {
                        for (int scout2 = x + 1; scout2 < 4; scout2++)
                        {
                            if (temp[i, scout2] == 0) continue;

                            if (temp[i, scout2] == temp[i, x])
                            {
                                temp[i, x] += 1;
                                temp[i, scout2] = 0;
                                score += 1 << temp[i, x];
                                validMove = true;
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void IterationParams(out int st, out int it1, out int it2)
        {
            switch (dir)
            {
                case Direction.Right:
                    st = 15;
                    it1 = -1;
                    it2 = -4;
                    break;
                case Direction.Down:
                    st = 15;
                    it1 = -4;
                    it2 = -1;
                    break;
                case Direction.Left:
                    st = 0;
                    it1 = 1;
                    it2 = 4;
                    break;
                case Direction.Up:
                    st = 0;
                    it1 = 4;
                    it2 = 1;
                    break;
                default:
                    st = 0;
                    it1 = 0;
                    it2 = 0;
                    break;
            }
        }

        private void CheckWinCondition()
        {
            gameOver = true;

            for (int i = 0; i < state.Length; i++)
            {
                if ((state[i] == 0) ||                          // check for empty fields
                    (i < 12 && state[i] == state[i + 4]) ||     // check for vertical shifts
                    (i % 4 < 3 && state[i] == state[i + 1]))    // check for horizontal shifts
                {
                    gameOver = false;
                    return;
                }
            }
        }

        private void AddRandomTile()
        {
            if (moves == 0 || (!gameOver && dir != Direction.None))
            {
                List<int> emptyTiles = new List<int>();

                for (int i = 0; i < state.Length; i++)
                    if (state[i] == 0)
                        emptyTiles.Add(i);

                int randomTile = emptyTiles[randy.Next(emptyTiles.Count)];
                state[randomTile] = randy.Next(2) + 1;
            }
        }
    }
}
