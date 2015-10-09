using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogHandler
{
    public class Logger
    {
        public static void WriteToDebugLog(string lines)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"debuglog.txt", true))
            {
                file.WriteLine(lines);
            }

        }

        public static void WriteToGameLog(string lines)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"gamelog.txt", true))
            {
                file.WriteLine(lines);
            }

        }

    }
}
