using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sounds
{
    class Program
    {
        static Program instance = new Program();

        int[] healingFrequencies = {
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

        Dictionary<string, decimal> noteFrequencies = new Dictionary<string, decimal>{
            {"" , -1.00m},
            {"C0" , 16.35m},
            {"C#0", 17.32m},
            {"Db0", 17.32m},
            {"D0" , 18.35m},
            {"D#0", 19.45m},
            {"Eb0", 19.45m},
            {"E0" , 20.60m},
            {"F0" , 21.83m},
            {"F#0", 23.12m},
            {"Gb0", 23.12m},
            {"G0" , 24.50m},
            {"G#0", 25.96m},
            {"Ab0", 25.96m},
            {"A0" , 27.50m},
            {"A#0", 29.14m},
            {"Bb0", 29.14m},
            {"B0" , 30.87m},
            {"C1" , 32.70m},
            {"C#1", 34.65m},
            {"Db1", 34.65m},
            {"D1" , 36.71m},
            {"D#1", 38.89m},
            {"Eb1", 38.89m},
            {"E1" , 41.20m},
            {"F1" , 43.65m},
            {"F#1", 46.25m},
            {"Gb1", 46.25m},
            {"G1" , 49.00m},
            {"G#1", 51.91m},
            {"Ab1", 51.91m},
            {"A1" , 55.00m},
            {"A#1", 58.27m},
            {"Bb1", 58.27m},
            {"B1" , 61.74m},
            {"C2" , 65.41m},
            {"C#2", 69.30m},
            {"Db2", 69.30m},
            {"D2" , 73.42m},
            {"D#2", 77.78m},
            {"Eb2", 77.78m},
            {"E2" , 82.41m},
            {"F2" , 87.31m},
            {"F#2", 92.50m},
            {"Gb2", 92.50m},
            {"G2" , 98.00m},
            {"G#2", 103.83m},
            {"Ab2", 103.83m},
            {"A2" , 110.00m},
            {"A#2", 116.54m},
            {"Bb2", 116.54m},
            {"B2" , 123.47m},
            {"C3" , 130.81m},
            {"C#3", 138.59m},
            {"Db3", 138.59m},
            {"D3" , 146.83m},
            {"D#3", 155.56m},
            {"Eb3", 155.56m},
            {"E3" , 164.81m},
            {"F3" , 174.61m},
            {"F#3", 185.00m},
            {"Gb3", 185.00m},
            {"G3" , 196.00m},
            {"G#3", 207.65m},
            {"Ab3", 207.65m},
            {"A3" , 220.00m},
            {"A#3", 233.08m},
            {"Bb3", 233.08m},
            {"B3" , 246.94m},
            {"C4" , 261.63m},
            {"C#4", 277.18m},
            {"Db4", 277.18m},
            {"D4" , 293.66m},
            {"D#4", 311.13m},
            {"Eb4", 311.13m},
            {"E4" , 329.63m},
            {"F4" , 349.23m},
            {"F#4", 369.99m},
            {"Gb4", 369.99m},
            {"G4" , 392.00m},
            {"G#4", 415.30m},
            {"Ab4", 415.30m},
            {"A4" , 440.00m},
            {"A#4", 466.16m},
            {"Bb4", 466.16m},
            {"B4" , 493.88m},
            {"C5" , 523.25m},
            {"C#5", 554.37m},
            {"Db5", 554.37m},
            {"D5" , 587.33m},
            {"D#5", 622.25m},
            {"Eb5", 622.25m},
            {"E5" , 659.25m},
            {"F5" , 698.46m},
            {"F#5", 739.99m},
            {"Gb5", 739.99m},
            {"G5" , 783.99m},
            {"G#5", 830.61m},
            {"Ab5", 830.61m},
            {"A5" , 880.00m},
            {"A#5", 932.33m},
            {"Bb5", 932.33m},
            {"B5" , 987.77m},
            {"C6" , 1046.50m},
            {"C#6", 1108.73m},
            {"Db6", 1108.73m},
            {"D6" , 1174.66m},
            {"D#6", 1244.51m},
            {"Eb6", 1244.51m},
            {"E6" , 1318.51m},
            {"F6" , 1396.91m},
            {"F#6", 1479.98m},
            {"Gb6", 1479.98m},
            {"G6" , 1567.98m},
            {"G#6", 1661.22m},
            {"Ab6", 1661.22m},
            {"A6" , 1760.00m},
            {"A#6", 1864.66m},
            {"Bb6", 1864.66m},
            {"B6" , 1975.53m},
            {"C7" , 2093.00m},
            {"C#7", 2217.46m},
            {"Db7", 2217.46m},
            {"D7" , 2349.32m},
            {"D#7", 2489.02m},
            {"Eb7", 2489.02m},
            {"E7" , 2637.02m},
            {"F7" , 2793.83m},
            {"F#7", 2959.96m},
            {"Gb7", 2959.96m},
            {"G7" , 3135.96m},
            {"G#7", 3322.44m},
            {"Ab7", 3322.44m},
            {"A7" , 3520.00m},
            {"A#7", 3729.31m},
            {"Bb7", 3729.31m},
            {"B7" , 3951.07m},
            {"C8" , 4186.01m},
            {"C#8", 4434.92m},
            {"Db8", 4434.92m},
            {"D8" , 4698.63m},
            {"D#8", 4978.03m},
            {"Eb8", 4978.03m},
            {"E8" , 5274.04m},
            {"F8" , 5587.65m},
            {"F#8", 5919.91m},
            {"Gb8", 5919.91m},
            {"G8" , 6271.93m},
            {"G#8", 6644.88m},
            {"Ab8", 6644.88m},
            {"A8" , 7040.00m},
            {"A#8", 7458.62m},
            {"Bb8", 7458.62m},
            {"B8" , 7902.13m},
        };

        int bpm = 120;

        static void Main(string[] args)
        {
            //instance.PlayFrequencies(500, 20000, 500, 1000);
            //instance.PlayHealingFrequencies();
            //instance.PlayHealFreq(4, 100000);


            // TODO: test for multithreading
            // https://stackoverflow.com/questions/2751686/how-can-i-execute-a-non-blocking-system-beep

            //new Thread(() => instance.PlayNote("A4", 0.5f)).Start();

            //instance.PlayNote("A3", 0.5f);

            instance.PlaySMBTheme();
        }

        // https://musescore.com/user/27687306/scores/4913846
        private void PlaySMBTheme()
        {
            bpm = 180;

            Note("E5", .125f);
            Note("G5", .125f);
            Note("E6", .125f);
            Note("C6", .125f);
            Note("D6", .125f);
            Note("G6", .125f);

            Note("", 1f);

            while (true)
            {
                SMBSeq1();

                for (int i = 0; i < 2; i++)
                    SMBSeq2();

                for (int i = 0; i < 2; i++)
                {
                    SMBSeq3A();
                    SMBSeq3B();
                    SMBSeq3A();
                    SMBSeq3C();
                }

                SMBSeq4A();
                SMBSeq4B();
                SMBSeq4A();

                SMBSeq1();

                for (int i = 0; i < 2; i++)
                    SMBSeq2();

                for (int i = 0; i < 2; i++)
                {
                    SMBSeq5A();
                    SMBSeq5B();
                    SMBSeq5A();
                    SMBSeq5C();
                }

                SMBSeq4A();
                SMBSeq4B();
                SMBSeq4A();
            }
        }

        private void SMBSeq1()
        {
            Note("E5", .125f);
            Note("E5", .125f);
            Note("", .125f);
            Note("E5", .125f);
            Note("", .125f);
            Note("C5", .125f);
            Note("E5", .25f);

            Note("G5", .25f);
            Note("", .25f);
            Note("G4", .25f);
            Note("", .25f);
        }

        private void SMBSeq2()
        {
            Note("C5", .25f);
            Note("", .125f);
            Note("G4", .25f);
            Note("", .125f);
            Note("E4", .25f);

            Note("", .125f);
            Note("A4", .25f);
            Note("B4", .25f);
            Note("Bb4", .125f);
            Note("A4", .25f);

            Note("G4", .17f);
            Note("E5", .17f);
            Note("G5", .16f);
            Note("A5", .25f);
            Note("F5", .125f);
            Note("G5", .125f);

            Note("", .125f);
            Note("E5", .25f);
            Note("C5", .125f);
            Note("D5", .125f);
            Note("B4", .25f);
            Note("", .125f);
        }

        private void SMBSeq3A()
        {
            Note("C3", .25f);
            Note("G5", .125f);
            Note("F#5", .125f);
            Note("F5", .125f);
            Note("D#5", .125f);
            Note("C4", .125f);
            Note("E5", .125f);

            Note("F3", .125f);
            Note("G#4", .125f);
            Note("A4", .125f);
            Note("C5", .125f);
            Note("C4", .125f);
            Note("A4", .125f);
            Note("C5", .125f);
            Note("D5", .125f);
        }

        private void SMBSeq3B()
        {
            Note("C3", .25f);
            Note("G5", .125f);
            Note("F#5", .125f);
            Note("F5", .125f);
            Note("D#5", .125f);
            Note("G3", .125f);
            Note("E5", .125f);

            Note("", .125f);
            Note("C6", .25f);
            Note("C6", .125f);
            Note("C6", .25f);
            Note("G3", .25f);
        }

        private void SMBSeq3C()
        {
            Note("C3", .25f);
            Note("Eb5", .25f);
            Note("", .125f);
            Note("D5", .25f);
            Note("", .125f);

            Note("C5", .25f);
            Note("", .125f);
            Note("G3", .125f);
            Note("G3", .25f);
            Note("C3", .25f);
        }

        private void SMBSeq4A()
        {
            Note("C5", .125f);
            Note("C5", .125f);
            Note("", .125f);
            Note("C5", .125f);
            Note("", .125f);
            Note("C5", .125f);
            Note("D5", .25f);

            Note("E5", .125f);
            Note("C5", .125f);
            Note("", .125f);
            Note("A4", .125f);
            Note("G4", .25f);
            Note("G2", .25f);
        }

        private void SMBSeq4B()
        {
            Note("C5", .125f);
            Note("C5", .125f);
            Note("", .125f);
            Note("C5", .125f);
            Note("", .125f);
            Note("C5", .125f);
            Note("D5", .125f);
            Note("E5", .125f);

            Note("G3", .25f);
            Note("", .125f);
            Note("C3", .125f);
            Note("", .25f);
            Note("G2", .25f);
        }

        private void SMBSeq5A()
        {
            Note("E5", .125f);
            Note("C5", .25f);
            Note("G4", .125f);
            Note("G3", .25f);
            Note("G#4", .25f);

            Note("A4", .125f);
            Note("F5", .125f);
            Note("F3", .125f);
            Note("F5", .125f);
            Note("A4", .125f);
            Note("C4", .125f);
            Note("F3", .25f);
        }

        private void SMBSeq5B()
        {
            Note("B4", .17f);
            Note("A5", .17f);
            Note("A5", .16f);
            Note("A5", .17f);
            Note("G5", .17f);
            Note("F5", .16f);

            Note("E5", .125f);
            Note("C5", .25f);
            Note("A4", .125f);
            Note("G4", .125f);
            Note("C4", .125f);
            Note("F3", .25f);
        }

        private void SMBSeq5C()
        {
            Note("B4", .125f);
            Note("F5", .25f);
            Note("F5", .125f);
            Note("F5", .17f);
            Note("E5", .17f);
            Note("D5", .16f);

            Note("G4", .125f);
            Note("E4", .25f);
            Note("E4", .125f);
            Note("C4", .25f);
            Note("", .25f);
        }

        private void Note(string note, float measures)
        {
            float bps = bpm / 60f;
            int duration = (int)Math.Round((measures * 4) / bps * 1000f);
            int frequency = (int)Math.Round(noteFrequencies[note]);

            if (frequency > -1)
                Console.Beep(frequency, duration);
            else
                Thread.Sleep(duration);
        }

        void PlayFrequencies(int start = 500, int max = 20000, int stepSize = 500, int duration = 1000)
        {
            for (int i = start; i < max; i += stepSize)
            {
                Console.WriteLine(i + " Hz");
                Console.Beep(i, duration);
            }
        }

        void PlayHealingFrequencies()
        {
            for (int i = 0; i < healingFrequencies.Length; i++)
            {
                PlayHealFreq(i, 1000);
            }
        }

        void PlayHealFreq(int index, int duration)
        {
            Console.WriteLine(healingFrequencies[index] + " Hz");
            Console.Beep(healingFrequencies[index], duration);
        }
    }
}
