using ASCII_Art_Renderer.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCII_Art_Renderer
{
    class Program
    {
        const string splitString = @"

---
";

        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            // fill strings array with stuff from text file
            string[] strings = Resources.Ascii_Images.Split(new string[] { splitString }, StringSplitOptions.None);

            int index = 0;

            while (true)
            {
                string currentString = strings[index];
                string[] rows = currentString.Split('\n');

                int consoleWidth = 10;
                foreach (var row in rows)
                {
                    int rowLength = row.Length;

                    if (row.Contains('\t'))
                        foreach (var c in row)
                            if (c == '\t')
                                rowLength += 7;

                    consoleWidth = Math.Max(consoleWidth, rowLength);
                }
                consoleWidth = Math.Min(consoleWidth, Console.LargestWindowWidth);
                int consoleHeight = Math.Min(rows.Length, Console.LargestWindowHeight);

                Console.Clear();
                Console.SetWindowSize(consoleWidth, consoleHeight);
                Console.Write(currentString);

                //Console.Write("\n" + consoleWidth);
                //Console.ReadLine();

                index += 1;
                if (index == strings.Length) index -= strings.Length;

                Console.ReadKey(true);
            }
        }
    }
}
