using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTF16_Farben
{
    class Program
    {
        static void Main(string[] args)
        {
            VomitColors();
            VomitUnicode();
        }

        static void VomitColors()
        {
            ConsoleColor[] colors = {
                ConsoleColor.Black,
                ConsoleColor.DarkRed,
                ConsoleColor.DarkYellow,
                ConsoleColor.DarkGreen,
                ConsoleColor.DarkCyan,
                ConsoleColor.DarkBlue,
                ConsoleColor.DarkMagenta,
                ConsoleColor.Gray,

                ConsoleColor.DarkGray,
                ConsoleColor.Red,
                ConsoleColor.Yellow,
                ConsoleColor.Green,
                ConsoleColor.Cyan,
                ConsoleColor.Blue,
                ConsoleColor.Magenta,
                ConsoleColor.White,
            };
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            Console.Title = "Colors";
            Console.WriteLine("Colors");
            Console.OutputEncoding = Encoding.Unicode;

            ConsoleColor col1;
            ConsoleColor col2;
            for (int i = 0; i < 16; i++)
            {
                col1 = colors[i];
                for (int j = i + 1; j < 16; j++)
                {
                    col2 = colors[j];

                    Console.ForegroundColor = col2;
                    Console.BackgroundColor = col1;
                    Console.Write("  ");
                    Console.Write("\u2591\u2591");

                    Console.Write("\u2592\u2592");

                    Console.ForegroundColor = col1;
                    Console.BackgroundColor = col2;
                    Console.Write("\u2593\u2593");

                    Console.ForegroundColor = col2;
                    Console.BackgroundColor = col1;
                    Console.Write("\u2593\u2593");

                    Console.ForegroundColor = col1;
                    Console.BackgroundColor = col2;
                    Console.Write("\u2592\u2592");

                    Console.Write("\u2591\u2591");
                    Console.Write("  ");

                    Console.ResetColor();
                }

                Console.WriteLine();
            }


            Console.ReadLine();
        }

        static void VomitUnicode()
        {
            uint[] nonDisplayable = { 0, 7, 8, 9, 10, 13 };

            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            Console.Title = "UTF-8";
            Console.WriteLine("UTF-8");
            Console.OutputEncoding = Encoding.Unicode;

            for (uint i = 0; i < 1 << 14; i++)
            {
                char character = (char)i;

                if (nonDisplayable.Contains(i)) character = (char)0x258C;

                Console.ForegroundColor = (ConsoleColor)(i % 16);
                Console.BackgroundColor = i % 16 < 2 ? ConsoleColor.White : ConsoleColor.Black;

                if (i % 16 == 0) Console.WriteLine();

                Console.Write(($"{Convert.ToString(i, 16)} {character}").PadLeft(9));
            }
            Console.ReadLine();
        }
    }
}