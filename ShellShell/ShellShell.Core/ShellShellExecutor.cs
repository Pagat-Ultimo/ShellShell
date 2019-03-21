using System;
using System.Collections.Generic;
using System.Linq;
using ShellShell.Core.Constants;
using ShellShell.Core.Exceptions;
using ShellShell.Core.Models;

namespace ShellShell.Core
{
    public class ShellShellExecutor
    {
        #region Fields

        private readonly List<ShellCommand> _commandList;
        private readonly List<string> _globalMandatoryExceptions;
        private readonly List<ShellParameter> _globalShellParameters;
        private List<string> _mandatoryParameters;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of the ShellShellExecutor class
        /// </summary>
        public ShellShellExecutor()
        {
            _commandList = new List<ShellCommand>();
            _globalShellParameters = new List<ShellParameter>();
            _globalMandatoryExceptions = new List<string> {"help"};

            var helpCmd = new ShellCommand("help", HelpCommand);
            helpCmd.ConfigureParameter("cmd");
            ConfigureCommand(helpCmd);
            DefaultCommand = helpCmd;
        }

        #endregion

        #region  Properties

        /// <summary>
        /// Gets or sets the character to identify a switch
        /// </summary>
        public string SwitchChar { get; set; } = "/";
        /// <summary>
        /// Gets or sets the character to identify a parameter
        /// </summary>
        public string ParamChar { get; set; } = "-";

        /// <summary>
        /// Gets the current command that will be executed
        /// </summary>
        public ShellCommand CurrentCommand { get; private set; }

        /// <summary>
        /// Gets or sets if a default command should be used. If true the first registered command will be executed if the user omits the command. Default is false.
        /// </summary>
        public bool UseDefaultCommand { get; set; } = false;

        public ShellCommand DefaultCommand { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Disabled the build in help command
        /// </summary>
        public void DisableHelpCommand()
        {
            if (_commandList.Exists(x => x.Name == "help"))
                _commandList.RemoveAll(x => x.Name == "help");
        }

        /// <summary>
        /// Parses the command line to evaluate which command should be executed and fills the parameters/switches with values
        /// </summary>
        /// <param name="args">The command line text the user entered</param>
        public void SetArguments(string[] args)
        {
            CheckArgumentForCommand(args);
            if (CurrentCommand == null)
                throw new Exception("No suitable command found");

            _mandatoryParameters = new List<string>();
            if (!_globalMandatoryExceptions.Contains(CurrentCommand.Name))
                _mandatoryParameters.AddRange(_globalShellParameters.Where(x => x.Mandatory).Select(x => x.Name));
            _mandatoryParameters.AddRange(CurrentCommand.Parameters.Where(x => x.Mandatory).Select(x => x.Name));


            CheckArgumentForSwitches(args);
            CheckArgumentForParameters(args);
        }

        /// <summary>
        /// Executes the CurrentCommand if its set. SetArguments has to be called first.
        /// </summary>
        public void Execute()
        {
            if (CurrentCommand == null)
                throw new Exception("No suitable CurrentCommand found");
            CurrentCommand.CommandAction.Invoke(this);
        }

        /// <summary>
        /// Configures a command to be available for the user
        /// </summary>
        /// <param name="command"></param>
        public void ConfigureCommand(ShellCommand command)
        {
            if (_commandList.Exists(x => x.Name == command.Name))
                throw new CommandArgumentException($"The command {command.Name} is already configured",
                    CommandExceptionCode.CommandAlreadyConfigured);
            _commandList.Add(command);
        }

        /// <summary>
        /// Configures a global parameters. Global parameters will be available for all registered commands
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        /// <param name="isMandatory">If the parameter should be mandatory</param>
        /// <param name="defaultValue">The default value if the parameter was not set by the user</param>
        public void ConfigureGlobalParameter(string name, bool isMandatory = false, string defaultValue = "")
        {
            if (_globalShellParameters.Exists(x => x.Name == name))
                throw new Exception($"There is already a global parameter with the name {name}");
            var parameter = new ShellParameter()
            {
                Name = name,
                Mandatory = isMandatory,
                Value = defaultValue
            };
            _globalShellParameters.Add(parameter);
        }

        /// <summary>
        /// Gets the value for a specific parameter as a String
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        /// <returns>Parameter value as String</returns>
        public string GetParameterAsString(string name)
        {
            if (!_globalShellParameters.Exists(x => x.Name == name))
                return CurrentCommand.GetParameterAsString(name);
            var par = _globalShellParameters.FirstOrDefault(x => x.Name == name)?.Value;
            return par;
        }

        /// <summary>
        /// Gets the value for specific parameter as Int
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        /// <returns>Parameter value as Int</returns>
        public int GetParameterAsInt(string name)
        {
            if (!_globalShellParameters.Exists(x => x.Name == name))
                return CurrentCommand.GetParameterAsInt(name);
            var par = _globalShellParameters.FirstOrDefault(x => x.Name == name)?.Value;
            if (int.TryParse(par, out var result))
            {
                return result;
            }
            else
            {
                throw new Exception($"Value for Parameter {name}is not valid");
            }
        }

        /// <summary>
        /// Gets the value for a specific switch
        /// </summary>
        /// <param name="name">The name of the switch to look for</param>
        /// <returns>The switch value</returns>
        public bool GetSwitch(string name)
        {
            return CurrentCommand.GetSwitchValue(name);
        }

        #endregion

        #region Private Methods

        private void HelpCommand(ShellShellExecutor shell)
        {
            var cmdParam = GetParameterAsString("cmd");
            if (cmdParam != "")
            {
                if (!_commandList.Exists(x => x.Name == cmdParam))
                    Console.WriteLine($"Command {cmdParam} not recognized");
                else
                {
                    Console.WriteLine($"Available Parameters for cmd {cmdParam}:");
                }
            }
            else
            {
                Console.WriteLine("Available Commands:");
                foreach (var command in _commandList)
                {
                    Console.WriteLine(command.Name);
                }
            }
        }

        private void CheckArgumentForSwitches(string[] args)
        {
            foreach (var arg in args)
            {
                if (!arg.StartsWith(SwitchChar))
                    continue;
                CurrentCommand.SetSwitch(StripCommandChar(arg, SwitchChar), true);
            }
        }

        private void CheckArgumentForParameters(string[] args)
        {
            var parameterCount = 0;
            int i = 0;
            if (args.Length > 0 && args[0] == CurrentCommand.Name)
                i = 1;
            for (; i < args.Length; i++)
            {
                var currentArg = args[i];

                if (currentArg.StartsWith(SwitchChar))
                    continue;

                if (!currentArg.StartsWith(ParamChar) && CurrentCommand.Parameters.Count >= parameterCount)
                {
                    var positionalParameter = CurrentCommand.Parameters[parameterCount];
                    CurrentCommand.SetParameter(positionalParameter.Name, currentArg);
                    if (_mandatoryParameters.Contains(positionalParameter.Name))
                        _mandatoryParameters.Remove(positionalParameter.Name);
                    parameterCount++;
                    continue;
                }

                if (i == args.Length - 1 || args[i + 1].StartsWith(SwitchChar) || args[i + 1].StartsWith(ParamChar))
                    throw new Exception($"Missing value for parameter {args[i]}");
                var parameterName = args[i].Substring(1, args[i].Length - 1);
                if (!SetGlobalParameter(parameterName, args[i + 1]))
                {
                    CurrentCommand.SetParameter(parameterName, args[i + 1]);
                    parameterCount++;
                }
                if (_mandatoryParameters.Contains(parameterName))
                    _mandatoryParameters.Remove(parameterName);
                i++;
            }

            if (_mandatoryParameters.Count > 0)
            {
                var error = "";
                foreach (var item in _mandatoryParameters)
                {
                    error += $"{item}, ";
                }

                throw new Exception($"Following parameters are mandatory and were not set: {error}");
            }
        }

        private void CheckArgumentForCommand(string[] args)
        {
            if (_commandList.Count == 0)
                throw new Exception("No commands defined in this application");
            if (args.Length == 0)
            {
                if (UseDefaultCommand)
                    CurrentCommand = DefaultCommand;
                else
                    throw new Exception("No suitable CurrentCommand found");
            }
            else
            {
                if (args[0].StartsWith(SwitchChar))
                {
                    if (UseDefaultCommand)
                        CurrentCommand = DefaultCommand;
                    else
                        throw new Exception("No suitable CurrentCommand found");
                }
                else
                {
                    CurrentCommand = _commandList.FirstOrDefault(x => x.Name == args[0]);
                    if (CurrentCommand == null)
                    {
                        if (UseDefaultCommand)
                        {
                            CurrentCommand = DefaultCommand;
                        }
                        else
                        {
                            throw new Exception("No suitable CurrentCommand found");
                        }
                    }
                }
            }
        }

        private bool SetGlobalParameter(string name, string value)
        {
            var parameterToSet = _globalShellParameters.FirstOrDefault(x => x.Name == name);
            if (parameterToSet == null)
                return false;
            parameterToSet.Value = value;
            return true;
        }

        private string StripCommandChar(string arg, string commandChar)
        {
            return arg.Substring(arg.IndexOf(commandChar, StringComparison.Ordinal) + commandChar.Length);
        }

        #endregion
    }
}