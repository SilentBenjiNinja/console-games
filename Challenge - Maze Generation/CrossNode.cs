namespace Challenges.MazeGeneration
{
    class CrossNode
    {
        Int2 position;
        public Int2 Position => position;
        public Int2 AbsolutePosition => new Int2 { X = position.X * 2, Y = position.Y * 2 };
        public bool Closed { get; set; }

        public CrossNode(int x, int y)
        {
            this.position = new Int2 { X = x, Y = y };
            this.Closed = false;
        }

        public CrossNode(Int2 position)
        {
            this.position = new Int2 { X = position.X, Y = position.Y };
            this.Closed = false;
        }
    }
}
