using System;
namespace Challenges.TicTacToe_Minimal
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] state = { 0, 0 }, winPermutations = { 7, 56, 73, 84, 146, 273, 292, 448 };
            int turns = 0;                              // 1-9 = current turn; 0 = X won; -1 = O won; 10 = Draw
            while (true)                                // GAME LOOP
            {
                turns++;                                // NEXT PLAYER'S TURN
                foreach (var p in winPermutations)      // CHECK WIN CONDITION
                {
                    if ((p & state[(turns % 2) * (turns % 2)]) == p)
                    {
                        turns = (turns % 2) - 2;
                    }
                }
                for (int i = 0; i < 9; i++)             // DRAW BOARD
                {
                    Console.Write((i % 3 == 0 ? "\n" : " ") + (((1 << i & (state[0] + state[1])) == 1 << i) ? ((1 << i & state[0]) == 1 << i ? "X" : "O") : (i + 1).ToString()));
                }
                if (turns > 9 || turns < 1)             // GAME OVER
                {
                    Console.Write((turns > 9 ? "\n\nDraw!" : $"\n\n=== WINNER! [{(turns % 2 == 0 ? "X" : "O")}] ===") + "\nAny key to quit...");
                    break;
                }
                int input = -1;                         // INPUT PROMPT & INPUT VALIDATION
                while (input < 0 || input > 8 || ((1 << input) & (state[0] + state[1])) == (1 << input))
                {
                    Console.Write($"\n\n{(turns % 2 == 1 ? "X" : "O")}'s turn [1-9]: ");
                    input = int.Parse(Console.ReadLine()) - 1;
                }
                state[(turns - 1) % 2] += 1 << input;
            }
            Console.ReadLine();                         // END OF PROGRAM
        }
    }
}