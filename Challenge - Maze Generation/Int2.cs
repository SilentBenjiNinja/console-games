using System;

namespace Challenges.MazeGeneration
{
    struct Int2
    {
        public int X
        {
            get;
            set;
        }

        public int Y
        {
            get;
            set;
        }

        public void SetCursor() => Console.SetCursorPosition(2 + (X * 2), 1 + Y);

        public double Dist(Int2 other) =>
            Math.Sqrt(((X - other.X) * (X - other.X)) + ((Y - other.Y) * (Y - other.Y)));

        public bool Equals(Int2 other) => X == other.X && Y == other.Y;
        public override string ToString() => $"({X}, {Y})";

        public static Int2 operator *(int operand1, Int2 operand2) => new Int2 { X = operand1 * operand2.X, Y = operand1 * operand2.Y };
        public static Int2 operator *(Int2 operand1, int operand2) => new Int2 { X = operand2 * operand1.X, Y = operand2 * operand1.Y };
        public static Int2 operator +(Int2 operand1, Int2 operand2) => new Int2 { X = operand1.X + operand2.X, Y = operand1.Y + operand2.Y };
        public static Int2 operator -(Int2 operand1, Int2 operand2) => new Int2 { X = operand1.X - operand2.X, Y = operand1.Y - operand2.Y };
    }
}
