namespace Challenges.MazeGeneration
{
    class MazeConfig
    {
        public const int DELTATIME_GENERATOR = 50;
        public const int DELTATIME_SOLVER = 5;
        public const int DELTATIME_TRACER = 10;
        public const int PAUSETIME = 2500;

        // amount of possible crossroads per row/column
        public const int WIDTH = 11;
        public const int HEIGHT = 11;

        public static float[] BranchingChances => new float[]{
            1f,
            //0.9f,
            0.05f,
            //0.2f,
            0f,
            //0.04f,
        };

        public const bool FILL_ALL_NODES = true;

        // default values, might be overridden in setup if set to random
        public static Int2 GenesisPosition => RANDOM_GENESIS ?
            Helper.RandomCrossNodePosition :
            new Int2 { X = WIDTH / 2, Y = HEIGHT / 2 };

        public static Int2 PathfindStartPosition => RANDOM_START ?
            Helper.RandomCrossNodePosition :
            new Int2 { X = 0, Y = HEIGHT - 1 };

        public static Int2 PathfindTargetPosition => RANDOM_TARGET ?
            Helper.RandomCrossNodePosition :
            new Int2 { X = WIDTH - 1, Y = 0 };

        const bool RANDOM_GENESIS = false;
        const bool RANDOM_START = false;
        const bool RANDOM_TARGET = false;

        public const int HEURISTICWEIGHT = 10;
    }
}
