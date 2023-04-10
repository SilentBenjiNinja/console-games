using System;
using System.Threading;

namespace Challenges.MazeGeneration
{
    static class Helper
    {
        #region Randomness
        static Random randy = new Random();
        public static Random Randy => randy;

        public static Int2 RandomCrossNodePosition => new Int2
        {
            X = Randy.Next(MazeConfig.WIDTH),
            Y = Randy.Next(MazeConfig.HEIGHT)
        };
        #endregion

        #region Time
        static DateTime stopwatchStartTime;

        public static void DeltaTimeGenerator() => Sleep(MazeConfig.DELTATIME_GENERATOR);
        public static void DeltaTimeSolver() => Sleep(MazeConfig.DELTATIME_SOLVER);
        public static void DeltaTimeTracer() => Sleep(MazeConfig.DELTATIME_TRACER);
        public static void Pause() => Sleep(MazeConfig.PAUSETIME);

        static void Sleep(int sleepTimeInMs) => Thread.Sleep(sleepTimeInMs);
        #endregion

        #region Drawing
        const string PIXEL = "\u2588\u2588";

        public static void DrawTile(Int2 position, NodeColorKey colorKey)
        {
            Console.ForegroundColor = ColorConfig.colorDictionary[colorKey];
            position.SetCursor();
            Console.Write(PIXEL);
        }
        #endregion

        #region Debug
        const int debugRows = 10;
        const ConsoleColor debugColor = ConsoleColor.Green;
        const int statNameX = 2;
        const int statValueX = 45;
        static int[] statRowLenghts = new int[debugRows];

        public static void DebugStat(int row, string statName, string statValue)
        {
            if (row < 1 || row > debugRows)
                return;

            int statRowY = MazeConfig.HEIGHT * 2 + row;

            Console.ForegroundColor = debugColor;

            Console.SetCursorPosition(statNameX, statRowY);
            Console.Write(GetTextWithTrailingWhiteSpace(statName, statValueX - statNameX));

            Console.SetCursorPosition(statValueX, statRowY);
            Console.Write(GetTextWithTrailingWhiteSpace(statValue, Math.Max(statRowLenghts[row - 1], statValue.Length)));
            statRowLenghts[row - 1] = statValue.Length;
        }

        static string GetTextWithTrailingWhiteSpace(string text, int maxLength) =>
            $"{text}{new string(' ', maxLength - text.Length)}";

        public static string GetPercentageString(float value) =>
            string.Format("{0:P2}", value);

        public static string GetDecimalString(double value) =>
            string.Format("{0:0.00}", value);

        public static string GetGenerationSettings()
        {
            string dimensionSettings = $"D: {MazeConfig.WIDTH}x{MazeConfig.HEIGHT}";
            string branchingSettings = $"BC: [{MazeConfig.BranchingChances[0]}, {MazeConfig.BranchingChances[1]}, {MazeConfig.BranchingChances[2]}]";
            string additionalSettings = $"Fill all: {(MazeConfig.FILL_ALL_NODES ? "On" : "Off")}";
            return $"{dimensionSettings}; {branchingSettings}; {additionalSettings}";
        }

        public static void ResetStopwatch() => stopwatchStartTime = DateTime.Now;
        public static TimeSpan StopwatchTime => DateTime.Now - stopwatchStartTime;
        public static string StopwatchTimeString => $"{GetDecimalString(StopwatchTime.TotalMilliseconds)}ms";
        #endregion

        public static void SetupWindow()
        {
            int windowWidth = (MazeConfig.WIDTH * 4) + 2;
            int windowHeight = (MazeConfig.HEIGHT * 2) + 2 + debugRows;

            Console.SetWindowSize(windowWidth, windowHeight);

            Console.Title = "Maze Generator";
            Console.CursorVisible = false;
        }
    }
}
