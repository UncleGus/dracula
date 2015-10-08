using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleHandler
{
    class LogHandler
    {
        public static void WriteToLog(string lines)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"gamelog.txt", true))
            {
                file.WriteLine(lines);
            }

        }
    }
}
