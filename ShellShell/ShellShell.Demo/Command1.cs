using ShellShell.Core;
using ShellShell.Core.Models;

namespace ShellShell.Demo
{
    class Command1 : ShellCommand
    {
        public Command1(string name, string description = "") : base(name, description)
        {
            CommandAction = Execute;
        }

        private void Execute(ShellShellExecutor executor)
        {

        }
    }
}
