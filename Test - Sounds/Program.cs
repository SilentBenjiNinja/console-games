using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sounds
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 500; i < 20000; i += 500)
            {
                Console.WriteLine(i + " Hz");
                Console.Beep(i, 1000);
            }
        }
    }
}
