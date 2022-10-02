using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// https://tildesites.bowdoin.edu/~echown/courses/210/javalab9/TetrisAssignment.pdf
// https://tetris.fandom.com/wiki/Tetris_(NES,_Nintendo)
namespace Games.Tetris
{
    /*
     * TODO: scoring system
     * TODO: statistics
     * TODO: refactor inputs for Delayed Auto Shift
     * and Button Up / Down Recognition (Rotation)
     */

    struct Piece
    {
        public bool large;
        public ushort[] blockMasks;
    }

    class Program
    {
        #region Program Window / Bootstrap

        const string GAME_TITLE = "TETRIS";

        const int BOARD_WIDTH = 10;
        const int BOARD_HEIGHT = 20;
        const int BORDER_WIDTH = 1;
        const int BORDER_HEIGHT = 1;
        const int SIDE_PANEL_WIDTH = 10;
        const int TITLE_PANEL_HEIGHT = 4;
        const int BOTTOM_PANEL_HEIGHT = 3;

        int WindowWidth => 2 * BOARD_WIDTH + 2 * BORDER_WIDTH + 4 * SIDE_PANEL_WIDTH;
        int WindowHeight => BOARD_HEIGHT + 2 * BORDER_HEIGHT + TITLE_PANEL_HEIGHT + BOTTOM_PANEL_HEIGHT;

        static Program instance = new Program();

        bool isRunning = false;
        bool gameOver = false;

        static void Main(string[] args)
        {
            Console.Title = GAME_TITLE;

            Console.SetWindowSize(instance.WindowWidth, instance.WindowHeight);
            Console.OutputEncoding = Encoding.Unicode;
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

        #endregion

        Thread gameTickThread;
        readonly object cursorLock = new object();

        Random randy = new Random();

        const int FRAME_RATE = 60;

        int currentFrame;

        enum Inputs { None, Up, Left, Down, Right }
        Inputs currentInput;

        int lastTickFrame;

        int currentLines;
        int CurrentLines
        {
            get => currentLines;
            set
            {
                if (value >= (currentLevel + 1) * 10)
                    CurrentLevel++;

                currentLines = value;

                lock (cursorLock)
                    WriteTextToPos(CurrentLines.ToString().PadLeft(3, '0'), SIDE_PANEL_WIDTH * 2 + BORDER_WIDTH + BOARD_WIDTH * 2 - 4, TITLE_PANEL_HEIGHT - 2);
            }
        }

        int currentLevel;
        int CurrentLevel
        {
            get => currentLevel;
            set
            {
                currentLevel = value;

                lock (cursorLock)
                {
                    DrawGameBoard();
                    WriteTextToPos(CurrentLevel.ToString().PadLeft(2, '0'), SIDE_PANEL_WIDTH * 2 + BORDER_WIDTH * 2 + BOARD_WIDTH * 2 + 8, TITLE_PANEL_HEIGHT + BORDER_HEIGHT + 15);
                }
            }
        }

        int[] LevelDropSpeeds = {
            48, 43, 38, 33, 28, 23, 18, 13, 8, 6,
            5,  5,  5,  4,  4,  4,  3,  3,  3, 2,
            2,  2,  2,  2,  2,  2,  2,  2,  2, 1,
        };

        int[] EntryDelaysByLastLockedY =
        {
            18,18,18,18,18,18,
            16,16,16,16,
            14,14,14,14,
            12,12,12,12,
            10,10,
        };

        int SoftDropSpeed => Math.Min(2, DropSpeed);
        int DropSpeed => LevelDropSpeeds[currentLevel < 30 ? currentLevel : 29];

        int CurrentDropSpeed => currentInput != Inputs.Down ? DropSpeed : SoftDropSpeed;
        int FrameTime => (1000 / FRAME_RATE);

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
            gameOver = false;

            currentFrame = 0;

            CurrentLevel = 0;
            CurrentLines = 0;

            gameBoard = new int[BOARD_HEIGHT, BOARD_WIDTH];
            gameTickThread = new Thread(UpdateLoopThread);

            DrawStats();

            DrawBorders(SIDE_PANEL_WIDTH * 2 + BORDER_WIDTH, TITLE_PANEL_HEIGHT + BORDER_HEIGHT, BOARD_WIDTH * 2, BOARD_HEIGHT);
            DrawGameBoard();

            NextPieceId = randy.Next(0, 7);
            NewPiece();
        }

        private void UpdateLoopThread()
        {
            while (!gameOver)
            {
                currentFrame++;
                TickFrame();

                lock (cursorLock)
                {
                    WriteTextToPos(currentFrame.ToString().PadRight(4), 8, WindowHeight - 3);

                    // TODO: rotating only once per button press
                    if (currentInput == Inputs.Up)
                        if (!Overlapping(currentPieceId, CurrentX, CurrentY, CurrentRotation + 1))
                            CurrentRotation++;

                    // TODO: delayed auto shift (16+6)
                    if (currentInput == Inputs.Left)
                        if (!Overlapping(currentPieceId, CurrentX - 1, CurrentY, CurrentRotation))
                            CurrentX--;
                    if (currentInput == Inputs.Right)
                        if (!Overlapping(currentPieceId, CurrentX + 1, CurrentY, CurrentRotation))
                            CurrentX++;

                    if (/*currentInput == Inputs.Down || */currentFrame - lastTickFrame >= CurrentDropSpeed)
                    {
                        lastTickFrame = currentFrame;

                        if (!Overlapping(currentPieceId, CurrentX, CurrentY + 1, CurrentRotation))
                            CurrentY++;
                        else
                        {
                            // lock piece in place
                            int pieceSize = pieces[currentPieceId].large ? 16 : 9;
                            int lastLockedY = 0;

                            for (int i = 0; i < pieceSize; i++)
                            {
                                if ((pieces[currentPieceId].blockMasks[CurrentRotation] >> (pieceSize - (i + 1)) & 1) == 1)     // only draw if not empty space
                                {
                                    int px = CurrentX + (int)(i % Math.Sqrt(pieceSize));
                                    int py = CurrentY + (int)(i / Math.Sqrt(pieceSize));

                                    gameBoard[py, px] = currentPieceId + 1;

                                    lastLockedY = Math.Max(py, lastLockedY);
                                }
                            }

                            List<int> clearableLines = new List<int>();

                            // check only for lines where piece was placed
                            for (int prevYId = 0; prevYId < 4; prevYId++)
                            {
                                int lineY = prevY[prevYId];

                                if (!clearableLines.Contains(lineY))
                                {
                                    bool isLineClearable = true;

                                    for (int x = 0; x < BOARD_WIDTH; x++)
                                        isLineClearable &= gameBoard[lineY, x] > 0;

                                    if (isLineClearable)
                                        clearableLines.Add(lineY);
                                }
                            }

                            // clear line
                            if (clearableLines.Count > 0)
                            {
                                int lowestRow = 0;
                                foreach (var item in clearableLines)
                                    lowestRow = Math.Max(lowestRow, item);

                                for (int clearY = lowestRow; clearY >= clearableLines.Count; clearY--)
                                {
                                    int nextY = clearY - clearableLines.Count;
                                    while (clearableLines.Contains(nextY))
                                    {
                                        nextY++;
                                    }

                                    for (int x = 0; x < BOARD_WIDTH; x++)
                                    {
                                        gameBoard[clearY, x] = gameBoard[nextY, x];
                                        gameBoard[nextY, x] = 0;
                                    }
                                }

                                CurrentLines += clearableLines.Count;

                                DrawGameBoard(lowestRow + 1);
                            }

                            TickFrame(EntryDelaysByLastLockedY[lastLockedY]);

                            NewPiece();
                        }
                    }

                    currentInput = Inputs.None;
                }
            }
        }

        private void InputLoop()
        {
            while (!gameOver)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                lock (cursorLock)
                {
                    WriteTextToPos(keyInfo.Key.ToString().PadRight(11), 9, WindowHeight - 4);

                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.W:
                            currentInput = Inputs.Up;
                            break;
                        case ConsoleKey.LeftArrow:
                        case ConsoleKey.A:
                            currentInput = Inputs.Left;
                            break;
                        case ConsoleKey.DownArrow:
                        case ConsoleKey.S:
                            currentInput = Inputs.Down;
                            break;
                        case ConsoleKey.RightArrow:
                        case ConsoleKey.D:
                            currentInput = Inputs.Right;
                            break;
                        default:
                            currentInput = Inputs.None;
                            break;
                    }
                }
            }
        }

        #region Piece Logic

        Piece[] pieces = {
            // T
            new Piece(){
                blockMasks = new ushort[]{
                    0b000111010,
                    0b010110010,
                    0b010111000,
                    0b010011010,
                }
            },
            // L mirrored
            new Piece(){
                blockMasks = new ushort[]{
                    0b000111001,
                    0b010010110,
                    0b100111000,
                    0b011010010,
                }
            },
            // dog left
            new Piece(){
                blockMasks = new ushort[]{
                    0b000110011,
                    0b001011010,
                }
            },
            // square
            new Piece(){
                large = true,
                blockMasks = new ushort[]{
                    0b0000011001100000,
                }
            },
            // dog right
            new Piece(){
                blockMasks = new ushort[]{
                    0b000011110,
                    0b010011001,
                }
            },
            // L
            new Piece(){
                blockMasks = new ushort[]{
                    0b000111100,
                    0b110010010,
                    0b001111000,
                    0b010010011,
                }
            },
            // I
            new Piece(){
                large = true,
                blockMasks = new ushort[]{
                    0b0000000011110000,
                    0b0010001000100010,
                }
            },
        };

        int[,] gameBoard;

        int[] prevX = new int[4];
        int[] prevY = new int[4];

        int currentPieceId;
        int nextPieceId;
        int NextPieceId
        {
            get => nextPieceId;
            set
            {
                nextPieceId = value;
                DrawPieceToUi(nextPieceId, SIDE_PANEL_WIDTH * 2 + BORDER_WIDTH * 2 + BOARD_WIDTH * 2 + 5, TITLE_PANEL_HEIGHT + BORDER_HEIGHT + 9);


            }
        }

        int currentX;
        int CurrentX
        {
            get => currentX;
            set
            {
                currentX = value;
                DrawPiece();
            }
        }

        int currentY;
        int CurrentY
        {
            get => currentY;
            set
            {
                currentY = value;
                DrawPiece();
            }
        }

        int currentRotation;
        int CurrentRotation
        {
            get => currentRotation;
            set
            {
                currentRotation = value % pieces[currentPieceId].blockMasks.Length;
                DrawPiece();
            }
        }

        private void TickFrame(int frames = 1) => Thread.Sleep(FrameTime * frames);

        private bool Overlapping(int id, int x, int y, int rot)
        {
            int iterations = pieces[id].large ? 16 : 9;
            rot %= pieces[id].blockMasks.Length;

            for (int i = 0; i < iterations; i++)
            {
                int px = x + (int)(i % Math.Sqrt(iterations));
                int py = y + (int)(i / Math.Sqrt(iterations));

                if ((pieces[id].blockMasks[rot] >> (iterations - (i + 1)) & 1) == 1)
                    // check if pixel is out of game board bounds (ignore roof)
                    if (px < 0 || px >= BOARD_WIDTH || py >= BOARD_HEIGHT)
                        return true;

                    // check for overlapping of board blocks and current piece
                    else if (py >= 0 && gameBoard[py, px] > 0)
                        return true;
            }
            return false;
        }

        private void NewPiece()
        {
            for (int i = 0; i < 4; i++)
            {
                prevX[i] = -1;
                prevY[i] = -1;
            }

            currentPieceId = NextPieceId;
            currentRotation = 0;
            currentX = pieces[currentPieceId].large ? 3 : 4;
            currentY = currentPieceId == 6 ? -2 : -1;
            DrawPiece();

            NextPieceId = randy.Next(0, 7);

            if (Overlapping(currentPieceId, CurrentX, CurrentY, 0))
                gameOver = true;
        }

        #endregion

        #region Rendering

        private void DrawPiece()
        {
            for (int i = 0; i < 4; i++)         // overdraw previous frame to avoid trailing
            {
                DrawPixelToBoard(0, prevX[i], prevY[i]);

                prevX[i] = -1;
                prevY[i] = -1;
            }

            int prevIt = 0;

            int iterations = pieces[currentPieceId].large ? 16 : 9;

            for (int i = 0; i < iterations; i++)
            {
                if ((pieces[currentPieceId].blockMasks[CurrentRotation] >> (iterations - (i + 1)) & 1) == 1)     // only draw if not empty space
                {
                    prevX[prevIt] = CurrentX + (int)(i % Math.Sqrt(iterations));
                    prevY[prevIt] = CurrentY + (int)(i / Math.Sqrt(iterations));

                    DrawPixelToBoard(currentPieceId + 1, prevX[prevIt], prevY[prevIt]);

                    prevIt++;
                }
            }
        }

        private void DrawGameBoard(int maxRow = BOARD_HEIGHT)
        {
            if (gameBoard != null)
                for (int y = 0; y < maxRow; y++)
                    for (int x = 0; x < BOARD_WIDTH; x++)
                        DrawPixelToBoard(gameBoard[y, x], x, y);
        }

        private void DrawStats()
        {
            DrawBorders(SIDE_PANEL_WIDTH * 2 + BORDER_WIDTH * 2 + BOARD_WIDTH * 2 + 5, TITLE_PANEL_HEIGHT + BORDER_HEIGHT + 7, 8, 5);
            WriteTextToPos("NEXT", SIDE_PANEL_WIDTH * 2 + BORDER_WIDTH * 2 + BOARD_WIDTH * 2 + 7, TITLE_PANEL_HEIGHT + BORDER_HEIGHT + 7);

            DrawBorders(SIDE_PANEL_WIDTH * 2 + BORDER_WIDTH * 2 + BOARD_WIDTH * 2 + 5, TITLE_PANEL_HEIGHT + BORDER_HEIGHT + 14, 7, 2);
            WriteTextToPos("LEVEL", SIDE_PANEL_WIDTH * 2 + BORDER_WIDTH * 2 + BOARD_WIDTH * 2 + 6, TITLE_PANEL_HEIGHT + BORDER_HEIGHT + 14);
            WriteTextToPos("00", SIDE_PANEL_WIDTH * 2 + BORDER_WIDTH * 2 + BOARD_WIDTH * 2 + 8, TITLE_PANEL_HEIGHT + BORDER_HEIGHT + 15);

            DrawBorders(SIDE_PANEL_WIDTH * 2 + BORDER_WIDTH, TITLE_PANEL_HEIGHT - 2, BOARD_WIDTH * 2, 1);
            WriteTextToPos("LINES", SIDE_PANEL_WIDTH * 2 + BORDER_WIDTH + 1, TITLE_PANEL_HEIGHT - 2);
            WriteTextToPos("000", SIDE_PANEL_WIDTH * 2 + BORDER_WIDTH + BOARD_WIDTH * 2 - 4, TITLE_PANEL_HEIGHT - 2);

            DrawBorders(2, TITLE_PANEL_HEIGHT - 2, (SIDE_PANEL_WIDTH - 2) * 2, BOARD_HEIGHT + TITLE_PANEL_HEIGHT - 1);
            WriteTextToPos("STATISTICS", 5, TITLE_PANEL_HEIGHT - 2);
            WriteTextToPos("----------------", 2, TITLE_PANEL_HEIGHT - 1);
            for (int i = 0; i < 7; i++)
            {
                DrawPieceToUi(i, 3, TITLE_PANEL_HEIGHT + i * 3 + 1);
                WriteTextToPos("000", 13, TITLE_PANEL_HEIGHT + i * 3 + 1);
            }

            // DEBUG
            WriteTextToPos("DEBUG:", 2, WindowHeight - 5, ConsoleColor.White, ConsoleColor.Black);
            WriteTextToPos("Input: N/A", 2, WindowHeight - 4);
            WriteTextToPos("Frame 0", 2, WindowHeight - 3);
            WriteTextToPos("Tick 0", 2, WindowHeight - 2);
        }

        // pieces in UI display:
        string[,] uiPieces = new string[7, 2] {
            { " \u2588\u2588\u2588\u2588\u2588\u2588 ", "   \u2588\u2588   " }, // T
            { " \u2588\u2588\u2588\u2588\u2588\u2588 ", "     \u2588\u2588 " }, // reverse L
            { " \u2588\u2588\u2588\u2588   ", "   \u2588\u2588\u2588\u2588 " }, // left dog
            { "  \u2588\u2588\u2588\u2588  ", "  \u2588\u2588\u2588\u2588  " }, // square
            { "   \u2588\u2588\u2588\u2588 ", " \u2588\u2588\u2588\u2588   " }, // right dog
            { " \u2588\u2588\u2588\u2588\u2588\u2588 ", " \u2588\u2588     " }, // L
            { "\u2584\u2584\u2584\u2584\u2584\u2584\u2584\u2584", "\u2580\u2580\u2580\u2580\u2580\u2580\u2580\u2580" }, // I
        };

        ConsoleColor[,] colorSchemes = new ConsoleColor[10, 2] {
            { ConsoleColor.DarkBlue, ConsoleColor.DarkCyan },
            { ConsoleColor.DarkGreen, ConsoleColor.Yellow },        // this yellow is actually lush yellow-green
            { ConsoleColor.DarkMagenta, ConsoleColor.Magenta },     // the dark magenta is actually purple, and the magenta is pink
            { ConsoleColor.DarkBlue, ConsoleColor.Green },
            { ConsoleColor.DarkRed, ConsoleColor.Cyan },
            { ConsoleColor.Cyan, ConsoleColor.Blue },
            { ConsoleColor.Red, ConsoleColor.Gray },
            { ConsoleColor.DarkMagenta, ConsoleColor.DarkRed },     // this dark red is actually dark red, not normal looking red
            { ConsoleColor.DarkBlue, ConsoleColor.DarkRed },
            { ConsoleColor.Red, ConsoleColor.DarkYellow },
        };

        private void DrawPieceToUi(int id, int x, int y)
        {
            ConsoleColor color = ConsoleColor.White;
            if (id % 3 > 0)
                color = colorSchemes[CurrentLevel % 10, id % 3 - 1];

            WriteTextToPos(uiPieces[id, 0], x, y, ConsoleColor.Black, color);
            WriteTextToPos(uiPieces[id, 1], x, y + 1, ConsoleColor.Black, color);
        }

        private void DrawPixelToBoard(int blockIndex, int x, int y)
        {
            if (x < 0 || x >= BOARD_WIDTH || y < 0 || y >= BOARD_HEIGHT)
                return;

            int cursorX = SIDE_PANEL_WIDTH * 2 + BORDER_WIDTH + x * 2;
            int cursorY = TITLE_PANEL_HEIGHT + BORDER_HEIGHT + y;

            // blockIndex 0 = black
            string text = "  ";

            // blockIndex 1, 4, 7 = white center
            // blockIndex 2, 5 = color 1 from scheme
            // blockIndex 3, 6 = color 2 from scheme
            if (blockIndex > 0)
                text = blockIndex % 3 == 1 ? "\u02ea\u02e5" : "\u02f9 ";

            int colorFromPalette = blockIndex % 3 != 0 ? 0 : 1;
            ConsoleColor color = blockIndex > 0 ? colorSchemes[CurrentLevel % 10, colorFromPalette] : ConsoleColor.Black;
            if (blockIndex % 3 != 1)
                WriteTextToPos(text, cursorX, cursorY, color, ConsoleColor.White);
            else
                WriteTextToPos(text, cursorX, cursorY, ConsoleColor.White, color);
        }

        #endregion

        #region Helpers

        // draws a border around a specified rectangle (in cursor coordinates)
        private void DrawBorders(int x, int y, int w, int h)
        {
            WriteTextToPos('╔' + new string('═', w) + '╗', x - BORDER_WIDTH, y - BORDER_HEIGHT);

            for (int i = 0; i < h; i++)
                WriteTextToPos('║' + new string(' ', w) + '║', x - BORDER_WIDTH, y + i);

            WriteTextToPos('╚' + new string('═', w) + '╝', x - BORDER_WIDTH, y + h);
        }

        // writes the text to the specified position with the specified colors
        private void WriteTextToPos(string text, int x, int y, ConsoleColor bgColor = ConsoleColor.Black, ConsoleColor fgColor = ConsoleColor.Gray)
        {
            Console.SetCursorPosition(x, y);
            Console.BackgroundColor = bgColor;
            Console.ForegroundColor = fgColor;
            Console.Write(text);
        }

        #endregion
    }
}
