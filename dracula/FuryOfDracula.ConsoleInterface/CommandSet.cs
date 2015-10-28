namespace FuryOfDracula.ConsoleInterface
{
    public class CommandSet
    {
        public string argument1;
        public string argument2;
        public string command;

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