using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.ConsoleInterface
{
    public class CommandSet {
        public string command;
        public string argument1;
        public string argument2;

        public CommandSet(string command, string argument1, string argument2)
        {
            this.command = command;
            this.argument1 = argument1;
            this.argument2 = argument2;
        }

        public CommandSet()
        {
        }
    }
}
