using System;
using ShellShell.Core.Models;

namespace ShellShell.Demo
{
    class Program
    {
        public static Core.ShellShell Shell = null;
        static void Main(string[] args)
        {
            Shell = new Core.ShellShell();
            var cmd = new ShellCommand(CommandNames.Command2, Command1);
            cmd.ConfigureSwitch(SwitchesNames.Switch1);
            cmd.ConfigureParameter(ParameterNames.Parameter1, false);
            cmd.ConfigureParameter(ParameterNames.Parameter2, false, "default1");
            Shell.ConfigureCommand(cmd);

            var cmd2 = new ShellCommand(CommandNames.Command1, Command1);
            cmd2.ConfigureSwitch(SwitchesNames.Switch2);
            cmd2.ConfigureParameter(ParameterNames.Parameter1, true);
            Shell.ConfigureCommand(cmd2);

            Shell.ConfigureGlobalParameter(ParameterNames.Parameter3, true);

            //shell.UseDefaultCommand = true;
            try
            {
                Shell.SetArguments(args);
                Shell.Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();

        }

        static void Command1()
        {
            Console.Write($"GlobalParameter Value {Shell.GetParameterAsString(ParameterNames.Parameter3)} ");
            if(Shell.GetSwitch(SwitchesNames.Switch1))
                Console.WriteLine($"Switch1 shows parameter 2 {Shell.GetParameterAsString(ParameterNames.Parameter2)}");
            else
                Console.WriteLine($"Switch1 shows parameter 2 {Shell.GetParameterAsString(ParameterNames.Parameter1)}");
        }
    }

    public class CommandNames
    {
        public const string Command1 = "cmd1";
        public const string Command2 = "cmd1";
    }

    public class ParameterNames
    {
        public const string Parameter1 = "p1";
        public const string Parameter2 = "p2";
        public const string Parameter3 = "p3";
    }

    public class SwitchesNames
    {
        public const string Switch2 = "s1";
        public const string Switch1 = "s2";
    }
}
