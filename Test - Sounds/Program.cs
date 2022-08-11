using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO: test for multithreading

namespace Sounds
{
    class Program
    {
        static int[] healingFrequencies = {
            174,
            285,
            396,
            417,
            432,
            440,
            528,
            555,
            639,
            741,
            852,
            963,
            1111,
        };

        static void Main(string[] args)
        {
            //PlayFrequencies(500, 20000, 500, 1000);
            //PlayHealingFrequencies();
            PlayHealFreq(4, 100000);
        }

        static void PlayFrequencies(int start = 500, int max = 20000, int stepSize = 500, int duration = 1000)
        {
            for (int i = start; i < max; i += stepSize)
            {
                Console.WriteLine(i + " Hz");
                Console.Beep(i, duration);
            }
        }

        static void PlayHealingFrequencies()
        {
            for (int i = 0; i < healingFrequencies.Length; i++)
            {
                PlayHealFreq(i, 1000);
            }
        }

        static void PlayHealFreq(int index, int duration)
        {
            Console.WriteLine(healingFrequencies[index] + " Hz");
            Console.Beep(healingFrequencies[index], duration);
        }
    }
}
