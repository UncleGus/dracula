using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleHandler;

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

        internal static void ClearLogs(UserInterface ui)
        {
            try
            {
                System.IO.File.Delete(@"debuglog.txt");
            }
            catch (System.IO.IOException)
            {
                ui.TellUser("Couldn't delete the old debug log file");
            }

            try
            {
                System.IO.File.Delete(@"gamelog.txt");
            }
            catch (System.IO.IOException)
            {
                ui.TellUser("Couldn't delete the old game log file");
            }
        }
    }
}
