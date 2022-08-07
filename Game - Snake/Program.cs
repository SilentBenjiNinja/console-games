using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Games.Snake
{
    class Program
    {
        static Program instance = new Program();

        int mx, my, ax, ay, len;
        List<int> x, y;

        const int W = 32;
        const int H = 24;
        const int FRAMERATE = 15;
        const int DT = 1000 / FRAMERATE;

        bool inputForFrame = false;
        Random randy = new Random();

        static void Main(string[] args)
        {
            instance.Start();
        }

        void Start()
        {
            Console.SetWindowSize(W * 2, H);
            Console.CursorVisible = false;

            Setup();

            Thread t = new Thread(InputLoop);
            t.Start();

            GameLoop();
        }

        void Setup()
        {
            Console.Clear();

            len = 5;
            mx = 1;
            my = 0;
            x = new List<int> { W / 2 - 4, W / 2 - 3, W / 2 - 2, W / 2 - 1, W / 2 };
            y = new List<int> { H / 2, H / 2, H / 2, H / 2, H / 2 };

            NewApple();

            for (int i = 0; i < len; i++)
            {
                Console.SetCursorPosition(x[i] * 2, y[i]);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\u2588\u2588");
            }
        }

        private void GameLoop()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;

                Thread.Sleep(DT);

                // check for collision
                int nx = (x[len - 1] + W + mx) % W,
                       ny = (y[len - 1] + H + my) % H;
                bool valid = true;
                for (int i = 0; i < len; i++)
                    if (x[i] == nx && y[i] == ny)
                        valid = false;

                if (valid)
                {
                    // push head
                    x.Add((x[len - 1] + W + mx) % W);
                    y.Add((y[len - 1] + H + my) % H);

                    Console.SetCursorPosition(x[len] * 2, y[len]);
                    Console.Write("\u2588\u2588");

                    // check for apple
                    if (x[len] == ax && y[len] == ay)
                    {
                        NewApple();
                        len += 1;
                    }
                    else
                    {
                        // pull end of tail
                        if (x[0] != ax || y[0] != ay)
                        {
                            Console.SetCursorPosition(x[0] * 2, y[0]);
                            Console.Write("  ");
                        }

                        x.RemoveAt(0);
                        y.RemoveAt(0);
                    }
                }
                else
                {
                    // restart when failed
                    Setup();
                }

                inputForFrame = false;
            }
        }

        void InputLoop()
        {
            while (true)
            {
                if (!inputForFrame)
                {
                    switch (Console.ReadKey(true).Key)
                    {
                        case ConsoleKey.LeftArrow:
                            if (mx != 1)
                            {
                                mx = -1;
                                my = 0;
                            }
                            break;
                        case ConsoleKey.UpArrow:
                            if (my != 1)
                            {
                                my = -1;
                                mx = 0;
                            }
                            break;
                        case ConsoleKey.RightArrow:
                            if (mx != -1)
                            {
                                mx = 1;
                                my = 0;
                            }
                            break;
                        case ConsoleKey.DownArrow:
                            if (my != -1)
                            {
                                my = 1;
                                mx = 0;
                            }
                            break;
                    }

                    inputForFrame = true;
                }
            }
        }

        private void NewApple()
        {
            ax = randy.Next(W);
            ay = randy.Next(H);

            Console.SetCursorPosition(ax * 2, ay);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\u2588\u2588");
        }
    }
}
