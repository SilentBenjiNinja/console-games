using Games.Sokoban.Properties;
using System;
using System.Collections.Generic;
using System.Text;

// TODO: auto-save & load
// TODO: helper function for aligning stuff in center
// TODO: WASD-support & continue with SPACE
// TODO: Player class with input or A* pathfinding
// TODO: main menu
// TODO: partition program in classes / regions
// TODO: make state array rectangular?
// TODO: level validation -> only one player, walls around level, goals = boxes, (boxes & goals > 0 -> not already solved)
namespace Games.Sokoban
{
    enum Direction { N, R, D, L, U };

    struct SokobanLevel
    {
        public readonly string title;
        public readonly string level;
        //public readonly string comment;

        public SokobanLevel(string rawLevel)
        {
            string[] split1 = rawLevel.Split(new string[] { "Title: " }, StringSplitOptions.None);
            level = split1[0];
            string[] split2 = split1[1].Split(new string[] { "\r\n" }, StringSplitOptions.None);
            title = split2[0];
        }
    }

    struct SokobanLevelPack
    {
        public readonly string title;
        public readonly string author;
        public readonly string description;

        public SokobanLevel[] Levels { get; private set; }
        public int LevelCount => Levels.Length;

        public SokobanLevelPack(string rawLevelPack)
        {
            string[] split1 = rawLevelPack.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
            string[] splitHeader = split1[0].Split(new string[] { "\r\n" }, StringSplitOptions.None);
            title = splitHeader[0].Split(new string[] { ": " }, StringSplitOptions.None)[1];
            author = splitHeader[1].Split(new string[] { ": " }, StringSplitOptions.None)[1];

            string tempDesc = split1[1];
            int index;

            for (index = 2; index < split1.Length; index++)
                if (!split1[index].Contains("###"))
                    tempDesc += "\n\n" + split1[index];
                else
                    break;

            description = tempDesc;

            int levelAmount = split1.Length - index;
            Levels = new SokobanLevel[levelAmount];
            for (int i = 0; i < levelAmount; i++)
                Levels[i] = new SokobanLevel(split1[i + index]);

            //Console.WriteLine("Level pack loaded!\n");
            //Console.WriteLine($"Name: {title}");
            //Console.WriteLine($"Author: {author}");
            //Console.WriteLine($"Description:\n\n{description}");
            //Console.WriteLine($"\n\nLevels: {LevelCount}\n");
            //Console.ReadLine();
            //for (int i = 0; i < levelAmount; i++)
            //{
            //    Console.WriteLine($"[{i + 1}] {Levels[i].title}\n------------");
            //    Console.WriteLine($"{Levels[i].level}\n\n");
            //}
            //Console.ReadLine();
        }
    }

    class Program
    {
        public static Program instance = new Program();

        const char floor = '-';
        const char wall = '#';
        const char goal = '.';
        const char box = '$';
        const char boxOnGoal = '*';
        const char player = '@';
        const char playerOnGoal = '+';

        #region Levels
        SokobanLevel[] levels;

        List<SokobanLevelPack> levelPacks = new List<SokobanLevelPack>();

        private void ImportLevels()
        {
            levelPacks.Add(new SokobanLevelPack(Resources.DrFogh_Archanfel_and_Joseph_L_Traub_Original_Extra_Sharpen));
            levelPacks.Add(new SokobanLevelPack(Resources.DrFogh_It_Is_All_Greek_Publish));
            levelPacks.Add(new SokobanLevelPack(Resources.DrFogh_Numbers));
            levelPacks.Add(new SokobanLevelPack(Resources.DrFogh_Original01));
            levelPacks.Add(new SokobanLevelPack(Resources.DrFogh_Original_02));
            levelPacks.Add(new SokobanLevelPack(Resources.DrFogh_Original_3));
            levelPacks.Add(new SokobanLevelPack(Resources.DrFogh_Sokobet_1));
            levelPacks.Add(new SokobanLevelPack(Resources.Howard_Abed_Set_4));
            levelPacks.Add(new SokobanLevelPack(Resources.Thinking_Rabbit_Original_Plus_Extra));
        }
        #endregion

        byte[][] state;
        Direction dir = Direction.N;
        byte[][] nextState;

        int playerX, playerY;
        int levelPadding;
        int levelWidth;

        int currentLevel;
        bool levelFinished;

        DateTime startTime;
        int totalMoves;
        int totalAttempts;

        DateTime levelStartTime;
        int levelMoves;
        int levelAttempts;

        static void Main(string[] args)
        {
            Console.Title = "SOKOBAN";
            Console.OutputEncoding = Encoding.Unicode;

            instance.PlayGame();
        }

        void PlayGame()
        {
            // TODO: main menu, level selection, loop back
            ImportLevels();
            PickLevelPack();
            PlayLevelPack();
        }

        void PlayLevelPack()
        {
            Console.CursorVisible = false;

            startTime = DateTime.Now;
            currentLevel = 0;

            while (currentLevel < levels.Length)
            {
                GameLoop();
                currentLevel++;
                totalAttempts += levelAttempts;
                levelAttempts = 0;
                totalMoves += levelMoves;
            }

            DrawWinMessage();

            while (true)
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                    return;
        }

        void GameLoop()
        {
            SetupGame();

            while (!levelFinished)
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
            if (!levelFinished && CalculateNextState(dir, out byte[][] nextState))
            {
                state = nextState;
                CheckWinCondition();
            }
        }

        void Draw()
        {
            DrawLevelTile(playerX, playerY);
            DrawLevelTile(playerX - 1, playerY);
            DrawLevelTile(playerX + 1, playerY);
            DrawLevelTile(playerX, playerY - 1);
            DrawLevelTile(playerX, playerY + 1);

            if (levelFinished)
                DrawLevelFinishMessage();
        }

        void ReadInput()
        {
            if (!levelFinished)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                dir = Direction.N;
                switch (keyInfo.Key)
                {
                    case ConsoleKey.RightArrow:
                        dir = Direction.R;
                        break;
                    case ConsoleKey.LeftArrow:
                        dir = Direction.L;
                        break;
                    case ConsoleKey.UpArrow:
                        dir = Direction.U;
                        break;
                    case ConsoleKey.DownArrow:
                        dir = Direction.D;
                        break;
                    case ConsoleKey.R:
                        SetupGame();
                        return;
                    case ConsoleKey.End:        // skip level cheat :)
                        levelFinished = true;
                        currentLevel++;
                        SetupGame();
                        return;
                    default:
                        return;
                }
            }
        }

        void SetupGame()
        {
            if (currentLevel < levels.Length)
            {
                LoadLevel();
                DrawLevel();
            }
        }

        void LoadLevel()
        {
            string level = levels[currentLevel].level;
            levelFinished = false;

            string[] rows = level.Split('\n');
            char[] tiles;

            levelWidth = 0;

            nextState = new byte[rows.Length][];

            for (byte i = 0; i < rows.Length; i++)
            {
                rows[i] = rows[i].Trim();
                tiles = rows[i].ToCharArray();

                nextState[i] = new byte[tiles.Length];

                for (byte j = 0; j < tiles.Length; j++)
                {
                    nextState[i][j] = Convert.ToByte(tiles[j]);
                    if (nextState[i][j] == (byte)player || nextState[i][j] == (byte)playerOnGoal)
                    {
                        playerX = j;
                        playerY = i;
                    }
                }

                levelWidth = Math.Max(levelWidth, tiles.Length);
            }

            state = nextState;

            Console.SetWindowSize(Math.Max(levelWidth * 2 + 5, 34), rows.Length + 16);
            levelPadding = (Console.WindowWidth - levelWidth * 2) / 2;

            levelStartTime = DateTime.Now;
            levelAttempts++;
            levelMoves = 0;
        }

        void DrawLevel()
        {
            Console.ResetColor();
            Console.Clear();
            DrawLevelText();

            for (int i = 0; i < state.GetLength(0); i++)
            {
                Console.ResetColor();
                Console.Write("".PadLeft(levelPadding));

                for (int j = 0; j < state[i].GetLength(0); j++)
                    DrawLevelTile(j, i);

                Console.ResetColor();
                Console.WriteLine();
            }

            DrawControlsMessage();
        }

        void CheckWinCondition()
        {
            bool finished = true;
            foreach (var tileRow in state)
                foreach (var tile in tileRow)
                    if (tile == (byte)goal || tile == (byte)playerOnGoal)
                        finished = false;

            levelFinished = finished;
        }

        // TODO: calculate all next states in state tree! -> let it solve itself (A* pathfinding)
        bool CalculateNextState(Direction dir, out byte[][] next)
        {
            int y = 0;
            int x = 0;
            next = state;

            switch (dir)
            {
                case Direction.R:
                    x = 1;
                    break;
                case Direction.D:
                    y = 1;
                    break;
                case Direction.L:
                    x = -1;
                    break;
                case Direction.U:
                    y = -1;
                    break;
                default:
                    return false;
            }

            nextState = state.Clone() as byte[][];

            if (playerX + x < levelWidth && playerX + x > -1 &&
                playerY + y < state.GetLength(0) && playerY + y > -1)
            {
                byte adjacentTile = state[playerY + y][playerX + x];
                bool move = (adjacentTile == (byte)floor || adjacentTile == (byte)goal);

                if (playerX + 2 * x < levelWidth && playerX + 2 * x > -1 &&
                    playerY + 2 * y < state.GetLength(0) && playerY + 2 * y > -1)
                {
                    if (adjacentTile == (byte)box || adjacentTile == (byte)boxOnGoal)
                    {
                        byte subAdjacentTile = state[playerY + 2 * y][playerX + 2 * x];
                        bool moveBox = (subAdjacentTile == (byte)floor || subAdjacentTile == (byte)goal);
                        move = moveBox;

                        if (moveBox)
                            if (subAdjacentTile == (byte)floor)
                                nextState[playerY + 2 * y][playerX + 2 * x] = (byte)box;
                            else if (subAdjacentTile == (byte)goal)
                                nextState[playerY + 2 * y][playerX + 2 * x] = (byte)boxOnGoal;
                    }
                }

                byte playerTile = state[playerY][playerX];
                if (move)
                {
                    if (playerTile == (byte)player)
                        nextState[playerY][playerX] = (byte)floor;
                    else if (playerTile == (byte)playerOnGoal)
                        nextState[playerY][playerX] = (byte)goal;

                    if (adjacentTile == (byte)floor || adjacentTile == (byte)box)
                        nextState[playerY + y][playerX + x] = (byte)player;
                    else if (adjacentTile == (byte)goal || adjacentTile == (byte)boxOnGoal)
                        nextState[playerY + y][playerX + x] = (byte)playerOnGoal;

                    playerX += x;
                    playerY += y;

                    levelMoves++;
                    next = nextState;

                    return true;
                }
            }
            return false;
        }

        private void PickLevelPack()
        {
            Console.CursorVisible = true;

            for (int i = 0; i < levelPacks.Count; i++)
            {
                Console.WriteLine($"[{i + 1}] {levelPacks[i].title}");
            }
            Console.Write($"\nPick level pack [1-{levelPacks.Count}]: ");

            int input = 0;
            do
            {
                if (int.TryParse(Console.ReadLine(), out int result))
                {
                    input = result;
                    if (input < 1 || input > levelPacks.Count)
                        Console.Write($"Must be a number between 1 and {levelPacks.Count}! ");
                }
                else
                {
                    Console.Write($"Must be a number! ");
                    continue;
                }

            } while (input < 1 || input > levelPacks.Count);
            levels = levelPacks[input - 1].Levels;
        }

        #region Drawing
        void DrawLevelTile(int x, int y)
        {
            // level outer bounds
            if (x < 0 || x > levelWidth || y < 0 || y > state.GetLength(0))
                return;

            Console.SetCursorPosition(levelPadding + x * 2, 5 + y);
            switch (state[y][x])
            {
                case (byte)floor:
                    DrawFloor();
                    break;
                case (byte)wall:
                    DrawWall();
                    break;
                case (byte)goal:
                    DrawGoal();
                    break;
                case (byte)box:
                    DrawBox();
                    break;
                case (byte)boxOnGoal:
                    DrawBoxOnGoal();
                    break;
                case (byte)player:
                case (byte)playerOnGoal:
                    DrawPlayer();
                    break;
                default:
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("??");
                    break;
            }
        }

        void DrawLevelText()
        {
            string levelString = $"{levels[currentLevel].title} ({currentLevel + 1}/{levels.Length})";
            if (levelString.Length + 4 > Console.WindowWidth)
            {
                levelPadding += (levelString.Length - Console.WindowWidth + 4) / 2;
                Console.WindowWidth = levelString.Length + 4;
            }

            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("  ".PadRight(Console.WindowWidth - 2, '='));
            Console.Write("".PadLeft((Console.WindowWidth - levelString.Length) / 2));
            Console.WriteLine(levelString);
            Console.WriteLine("  ".PadRight(Console.WindowWidth - 2, '='));
            Console.WriteLine();
        }

        void DrawControlsMessage()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine();
            Console.WriteLine("  ".PadRight(Console.WindowWidth - 2, '-'));

            string controlsString = "CONTROLS:";
            Console.Write("".PadLeft((Console.WindowWidth - controlsString.Length) / 2));
            Console.WriteLine(controlsString);
            Console.WriteLine("  ".PadRight(Console.WindowWidth - 2, '-'));
            Console.WriteLine();

            string prompt = $"[{(char)24}{(char)27}{(char)25}{(char)26}]";
            controlsString = " Move";
            Console.Write("".PadLeft((Console.WindowWidth - (prompt.Length + controlsString.Length)) / 2));
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(prompt);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(controlsString);

            prompt = "[R]";
            controlsString = " Retry";
            Console.Write("".PadLeft((Console.WindowWidth - (prompt.Length + controlsString.Length)) / 2));
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(prompt);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(controlsString);
        }

        void DrawLevelFinishMessage()
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(0, 5 + state.GetLength(0));
            Console.WriteLine();
            Console.WriteLine("  ".PadRight(Console.WindowWidth - 2, '='));
            Console.Write("".PadLeft((Console.WindowWidth - 14) / 2));
            Console.WriteLine($"LEVEL CLEARED!");
            Console.WriteLine("  ".PadRight(Console.WindowWidth - 2, '='));
            Console.ResetColor();

            string time = (DateTime.Now - levelStartTime).ToString(@"m\:ss\.fff");
            Console.WriteLine("\n  Time:" + time.PadLeft(Console.WindowWidth - 9, '.'));
            Console.WriteLine("  Moves:" + levelMoves.ToString().PadLeft(Console.WindowWidth - 10, '.'));
            Console.WriteLine("  Attempts:" + levelAttempts.ToString().PadLeft(Console.WindowWidth - 13, '.'));
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("\n  Press ");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("[ENTER]");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(" to continue...");
        }

        void DrawWinMessage()
        {
            Console.Clear();
            Console.SetWindowSize(45, 11);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("  ".PadRight(Console.WindowWidth - 2, '+'));
            Console.WriteLine("  CONGRATULATIONS on completing all levels!  ");
            Console.WriteLine("  ".PadRight(Console.WindowWidth - 2, '+'));
            Console.ResetColor();

            string totalTime = (DateTime.Now - startTime).ToString(@"m\:ss\.fff");
            Console.WriteLine("\n  Total time:" + totalTime.ToString().PadLeft(Console.WindowWidth - 15, '.'));
            Console.WriteLine("  Total moves:" + totalMoves.ToString().PadLeft(Console.WindowWidth - 16, '.'));
            Console.WriteLine("  Failed attempts:" + (totalAttempts - levels.Length).ToString().PadLeft(Console.WindowWidth - 20, '.'));
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("\n  Press ");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("[ENTER]");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(" to exit...");
        }

        void DrawWall()
        {
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("\u2592\u2592");
        }
        void DrawBox()
        {
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("\u256a\u256a");
        }
        void DrawGoal()
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("\u25c4\u25ba");
        }
        void DrawBoxOnGoal()
        {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\u256a\u256a");
        }
        void DrawPlayer()
        {
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            //Console.Write("\u0352\u0352");
            Console.Write("\u0298\u0298");
            //Console.Write("\u2c7a\u2c7a");
            //Console.Write("\u00d3\u00d2");
            //Console.Write("@@");
            //Console.Write("\u03b8\u03b8");
            //Console.Write("\u00f3\u00f2");
        }
        void DrawFloor()
        {
            Console.ResetColor();
            Console.Write("  ");
        }
        #endregion

        string[] _oldLevels = {
            @"#####
              #.$@#
              #####",
            @"#######
              #.$@$.#
              #######",
            @"--###
              --#.#
              ###$###
              #.$@$.#
              #######",
            @"--###
              --#.#
              ###$###
              #.$@$.#
              ###$###
              --#.#
              --###",
            @"-###
              -#.###
              ##$#.#
              #.$@$##
              ###$$.#
              --#.###
              --###",
            @"#####
              #@$.##
              ##$$.##
              -#.$$.#
              -##.$##
              --##.#
              ---###",
            @"###
              #.##
              #$.###
              #-$#.####
              #.$-$-$.#
              ####@#$-#
              ---###.$#
              -----##.#
              ------###",
            @"----###
              ---##@#
              --##-$###
              -##-$.#.#
              ##-$.#.$#
              #-$.#.$-#
              #-.#.$-##
              #-##$-##
              #----##
              ######",
            @"-------###
              ------##@#
              #######-$#
              #----#-$.#
              #-##--$.####
              #$-###.###.#
              #.$-#####.$#
              ##.$---#.$-#
              -##.##-#$-##
              --####-#-##
              -----#---#
              -----#####",
            @"---------###
              ---------#.##
              ---------#$.##
              ---------#-$.#
              ---------##-$#
              ----------##-#
              --##########-#
              -##----------#--#####
              ##--##########-##-$.#
              #--##---#-----##-$.##
              #-##-$#-#-----#-$.##
              #-#-$.#-#--####-###
              #--$.##-####.##-#
              ###.###---------#
              --###-######$####
              -----------#@#
              -----------###",

            @"#######
              #.@-#-#
              #$*-$-#
              #---$-#
              #-..--#
              #--*--#
              #######",
            @"--#####
              ###---#
              #.@$--#
              ###-$.#
              #.##$-#
              #-#-.-##
              #$-*$$.#
              #---.--#
              ########",

            @"###########
              #---------#
              #-$-$@$-$-#
              #--$-$-$--#
              #-$-$-$-$-#
              #--$-$-$--#
              #####$##$##
              -#.....#-#
              -#....*#-#
              -#...*---#
              -#....-###
              -########",
            @"-------######
              -####--#----#
              -#-@####-##-##
              ##-$#--#...--#
              #--$-$-#.#.#-#
              #-$-$--$...#-#
              ##-$--$-##-#-#
              -##-###----#-#
              --#---######-#
              --###--------#
              ----##########",
        };
    }
}
