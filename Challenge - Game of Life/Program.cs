using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Game_Of_Life
{
    class Program
    {
        static Program instance = new Program();

        int W = 318;
        int H = 167;
        const int DT = 500;
        const bool wrap = false;

        const string DEAD = "  ";
        const string ALIVE = "\u2588\u2588";
        const ConsoleColor ALIVE_COLOR = ConsoleColor.Cyan;
        const ConsoleColor DEAD_COLOR = ConsoleColor.Black;

        int[,] state;
        int[,] prev;

        static void Main(string[] args)
        {
            instance.Setup();
            instance.GameLoop();
        }

        private void Setup()
        {
            state = new int[(W - 1) / 32 + 1, H];
            prev = new int[(W - 1) / 32 + 1, H];

            FillRandomly();
            //FillLwssArmada();
            //FillLwss();

            Console.Title = "Game of Life";
            Console.ForegroundColor = ALIVE_COLOR;
            Console.BackgroundColor = DEAD_COLOR;
            Console.CursorVisible = false;
            Console.SetWindowSize(W * 2, H);
            Console.SetWindowPosition(0, 0);
        }

        private void GameLoop()
        {
            while (true)
            {
                Draw();
                Update();
            }
        }

        void Draw()
        {
            for (int i = 0; i < H; i++)
            {
                for (int j = 0; j < W; j++)
                {
                    if ((prev[j / 32, i] >> j & 1) != (state[j / 32, i] >> j & 1))
                    {
                        Console.SetCursorPosition(j * 2, i);
                        Console.Write((state[j / 32, i] >> j & 1) == 1 ? ALIVE : DEAD);
                    }
                }
            }

            Thread.Sleep(DT);
        }

        void Update()
        {
            prev = state.Clone() as int[,];

            for (int i = 0; i < H; i++)
            {
                for (int j = 0; j < W; j++)
                {
                    bool isAlive = (prev[j / 32, i] >> j & 1) == 1;
                    int liveNb = LiveNB(j, i);

                    // Any live cell with fewer than two live neighbours dies, as if by underpopulation.
                    if (isAlive && liveNb < 2)
                        state[j / 32, i] -= 1 << j;

                    // Any live cell with more than three live neighbours dies, as if by overpopulation.
                    else
                    if (isAlive && liveNb > 3)
                        state[j / 32, i] -= 1 << j;

                    // Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
                    else
                    if (!isAlive && liveNb == 3)
                        state[j / 32, i] += 1 << j;

                    // Any live cell with two or three live neighbours lives on to the next generation.
                    // There is no code here...
                }
            }
        }

        int LiveNB(int x, int y)
        {
            int amount = 0;

            if (wrap)
            {
                amount += (prev[((x - 1 + W) % W) / 32, (y - 1 + H) % H] >> ((x - 1 + W) % W)) & 1;
                amount += (prev[((x) % W) / 32, (y - 1 + H) % H] >> ((x) % W)) & 1;
                amount += (prev[((x + 1) % W) / 32, (y - 1 + H) % H] >> ((x + 1) % W)) & 1;
                amount += (prev[((x - 1 + W) % W) / 32, (y) % H] >> ((x - 1 + W) % W)) & 1;
                amount += (prev[((x + 1) % W) / 32, (y) % H] >> ((x + 1) % W)) & 1;
                amount += (prev[((x - 1 + W) % W) / 32, (y + 1) % H] >> ((x - 1 + W) % W)) & 1;
                amount += (prev[((x) % W) / 32, (y + 1) % H] >> ((x) % W)) & 1;
                amount += (prev[((x + 1) % W) / 32, (y + 1) % H] >> ((x + 1) % W)) & 1;
            }
            else
            {
                if (x > 0 && y > 0)
                    amount += (prev[(x - 1) / 32, y - 1] >> x - 1) & 1;
                if (y > 0)
                    amount += (prev[x / 32, y - 1] >> x) & 1;
                if (x < W - 1 && y > 0)
                    amount += (prev[(x + 1) / 32, y - 1] >> x + 1) & 1;
                if (x > 0)
                    amount += (prev[(x - 1) / 32, y] >> x - 1) & 1;
                if (x < W - 1)
                    amount += (prev[(x + 1) / 32, y] >> x + 1) & 1;
                if (x > 0 && y < H - 1)
                    amount += (prev[(x - 1) / 32, y + 1] >> x - 1) & 1;
                if (y < H - 1)
                    amount += (prev[x / 32, y + 1] >> x) & 1;
                if (x < W - 1 && y < H - 1)
                    amount += (prev[(x + 1) / 32, y + 1] >> x + 1) & 1;
            }


            return amount;
        }

        private void FillLwssArmada()
        {
            state = new int[,]{
                {
                    0b_00000000_00000000_00000000_00000000,
                    0b_00000000_00000000_00000000_00000000,
                    0b_00001111_00001111_00001111_00001111,
                    0b_00010001_00010001_00010001_00010001,
                    0b_00000001_00000001_00000001_00000001,
                    0b_00010010_00010010_00010010_00010010,
                    0b_00000000_00000000_00000000_00000000,
                    0b_00000000_00000000_00000000_00000000,
                    0b_00001111_00001111_00001111_00001111,
                    0b_00010001_00010001_00010001_00010001,
                    0b_00000001_00000001_00000001_00000001,
                    0b_00010010_00010010_00010010_00010010,
                    0b_00000000_00000000_00000000_00000000,
                    0b_00000000_00000000_00000000_00000000,
                    0b_00000000_00000000_00000000_00000000,
                    0b_00001111_00001111_00001111_00001111,
                    0b_00010001_00010001_00010001_00010001,
                    0b_00000001_00000001_00000001_00000001,
                    0b_00010010_00010010_00010010_00010010,
                    0b_00000000_00000000_00000000_00000000,
                    0b_00000000_00000000_00000000_00000000,
                    0b_00001111_00001111_00001111_00001111,
                    0b_00010001_00010001_00010001_00010001,
                    0b_00000001_00000001_00000001_00000001,
                    0b_00010010_00010010_00010010_00010010,
                    0b_00000000_00000000_00000000_00000000,
                },
            };

            W = state.GetLength(0) * 32;
            H = state.GetLength(1);
            prev = new int[(W - 1) / 32 + 1, H];
        }

        private void FillRandomly()
        {
            Random randy = new Random();

            for (int i = 0; i < H; i++)
            {
                for (int j = 0; j < W; j++)
                {
                    state[j / 32, i] = randy.Next(int.MinValue, int.MaxValue);
                }
            }
        }

        private void FillLwss()
        {
            state = new int[,]{
                {
                    0b_00000000_00000000_00000000_00000000,
                    0b_00000000_00000000_00000000_00001100,
                    0b_00000000_00000000_00000000_00011011,
                    0b_00000000_00000000_00000000_00001111,
                    0b_00000000_00000000_00000000_00000110,
                },
            };

            W = 20;
            H = 5;
            prev = new int[(W - 1) / 32 + 1, H];
        }
    }
}