using System;
using System.IO;
using ShellShell.Core.Models;

namespace ShellShell.Core.Commands
{
    /// <summary>
    /// Command to add path of the current groxy exe to the users PATH variable
    /// </summary>
    public class AddToPathCommand : ShellCommand
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the AddToPathCommand class
        /// </summary>
        /// <param name="name">Name of the command</param>
        /// <param name="description">Description of the command</param>
        public AddToPathCommand(string name, string description = "") : base(name, description)
        {
            CommandAction = Execute;
        }

        #endregion

        #region Private Methods

        private void Execute(ShellShellExecutor executor)
        {
            string pathToGroxy = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(pathToGroxy))
            {
                Console.WriteLine("Cant get path to groxy.exe");
                return;
            }

            Console.WriteLine("Setting PATH...");
            const string name = "PATH";
            string currentPathContent = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User);
            string value = currentPathContent + ";" + pathToGroxy;
            Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.User);
            Console.WriteLine("PATH Set!");
            Console.WriteLine("Please reopen your console session to use changes");
        }

        #endregion
    }
}