using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocationHandler;

namespace ConsoleHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            string line;
            Location Paris = new Location();
            do
            {
                line = Console.ReadLine();
                Console.WriteLine(line);
            } while (line != "exit");
        }
    }
}
