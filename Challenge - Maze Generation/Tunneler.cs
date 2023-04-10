namespace Challenges.MazeGeneration
{
    struct Tunneler
    {
        public Int2 startNodePosition;
        public Direction direction;

        public Int2 GetTunnel()
        {
            Int2 tunnelPosition = new Int2 { X = startNodePosition.X * 2, Y = startNodePosition.Y * 2 };

            switch (direction)
            {
                case Direction.Up:
                    tunnelPosition.Y -= 1;
                    break;
                case Direction.Right:
                    tunnelPosition.X += 1;
                    break;
                case Direction.Down:
                    tunnelPosition.Y += 1;
                    break;
                case Direction.Left:
                    tunnelPosition.X -= 1;
                    break;
            }

            return tunnelPosition;
        }
    }
}
