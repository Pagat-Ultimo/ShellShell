using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ShellShell.Core.Constants;
using ShellShell.Core.Exceptions;
using ShellShell.Core.Models;

namespace ShellShell.Core
{
    public class ShellShellExecutor
    {
        private readonly List<ShellCommand> _commandList;
        private readonly List<ShellParameter> _globalShellParameters;
        private readonly List<string> _globalMandatoryExceptions;

        private List<string> _mandatoryParameters;

        public ShellCommand CurrentCommand { get; private set; }
        private string _switchChar = "/";

        public string SwitchChar
        {
            get => _switchChar;
            set
            {
                _switchChar = value;
                foreach (var cmd in _commandList)
                {
                    cmd.SwitchChar = SwitchChar;
                }
            }
        }

        public string ParamChar { get; set; } = "-";

        public bool UseDefaultCommand { get; set; } = false;
        public ShellShellExecutor()
        {
            _commandList = new List<ShellCommand>();
            _globalShellParameters = new List<ShellParameter>();
            _globalMandatoryExceptions = new List<string>();
            _globalMandatoryExceptions.Add("help");

            var helpCmd = new ShellCommand("help", HelpCommand);
            helpCmd.ConfigureParameter("cmd");
            ConfigureCommand(helpCmd);
        }

        private void HelpCommand(ShellShellExecutor shell)
        {
            var cmdParam = GetParameterAsString("cmd");
            if (cmdParam != "")
            {
                if(!_commandList.Exists(x=> x.Name == cmdParam))
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

        public void DisableHelpCommand()
        {
            if (_commandList.Exists(x => x.Name == "help"))
                _commandList.RemoveAll(x => x.Name == "help");
        }

        public void SetArguments(string[] args)
        {
            CheckArgumentForCommand(args);
            if(CurrentCommand == null)
                throw new Exception("No suitable command found");

            _mandatoryParameters = new List<string>();
            if (!_globalMandatoryExceptions.Contains(CurrentCommand.Name))
                _mandatoryParameters.AddRange(_globalShellParameters.Where(x => x.Mandatory).Select(x => x.Name));
            _mandatoryParameters.AddRange(_globalShellParameters.Where(x => x.Mandatory).Select(x => x.Name));

            CheckArgumentForSwitches(args);
            CheckArgumentForParameters(args);
        }

        public void Execute()
        {
            if (CurrentCommand == null)
                throw new Exception("No suitable CurrentCommand found");
            CurrentCommand.CommandAction.Invoke(this);
        }

        private void CheckArgumentForSwitches(string[] args)
        {
            foreach (var arg in args)
            {
                if (!arg.StartsWith(SwitchChar))
                    continue;
                CurrentCommand.SetSwitch(StripCommandChar(arg,SwitchChar), true);
            }
        }

        private void CheckArgumentForParameters(string[] args)
        {
            for (int i = 1; i < args.Length; i++)
            {
                var currentArg = args[i];
                if (!currentArg.StartsWith(ParamChar))
                {
                    if (currentArg.StartsWith(SwitchChar))
                        continue;
                    else
                    {

                        if (_globalShellParameters.Count >= i-1 && !SetGlobalParameter(_globalShellParameters[i-1].Name, currentArg))
                            CurrentCommand.SetParameter(_globalShellParameters[i - 1].Name, currentArg);
                        if (_mandatoryParameters.Contains(_globalShellParameters[i - 1].Name))
                            _mandatoryParameters.Remove(_globalShellParameters[i - 1].Name);
                        continue;
                    }
                }

                if (i == args.Length - 1 || args[i+1].StartsWith(SwitchChar) || args[i + 1].StartsWith(ParamChar))
                    throw new Exception($"Missing value for parameter {args[i]}");
                if(!SetGlobalParameter(args[i].Substring(1, args[i].Length - 1), args[i + 1]))
                    CurrentCommand.SetParameter(args[i].Substring(1,args[i].Length-1), args[i + 1]);
                if(_mandatoryParameters.Contains(args[i].Substring(1, args[i].Length - 1)))
                    _mandatoryParameters.Remove(args[i].Substring(1, args[i].Length - 1));
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
            var defaultCommand = _commandList.First();
            if (args.Length == 0)
            {
                if(UseDefaultCommand)
                    CurrentCommand = defaultCommand;
                else
                    throw new Exception("No suitable CurrentCommand found");
            }
            else
            {
                if (args[0].StartsWith(defaultCommand.SwitchChar))
                {
                    if (UseDefaultCommand)
                        CurrentCommand = defaultCommand;
                    else
                        throw new Exception("No suitable CurrentCommand found");
                }
                else
                {
                    CurrentCommand = _commandList.FirstOrDefault(x => x.Name == args[0]);
                    if (CurrentCommand == null)
                        throw new Exception("No suitable CurrentCommand found");
                }
            }
        }

        public void ConfigureCommand(ShellCommand command)
        {
            if (_commandList.Exists(x => x.Name == command.Name))
                throw new CommandArgumentException($"The command {command.Name} is already configured", CommandExceptionCode.CommandAlreadyConfigured);
            _commandList.Add(command);
        }

        public void ConfigureGlobalParameter(string name, bool isMandatory = false, string defaultValue = "")
        {
            if (_globalShellParameters.Exists(x => x.Name == name))
                throw new Exception($"There is already a global parameter with the name {name}");
            var parameter = new ShellParameter()
            {
                Name = name,
                Mandatory = isMandatory,
                Value = ""
            };
            _globalShellParameters.Add(parameter);
        }

        private bool SetGlobalParameter(string name, string value)
        {
            if (!_globalShellParameters.Exists(x => x.Name == name))
                return false;
            _globalShellParameters.FirstOrDefault(x => x.Name == name).Value = value;
            return true;
        }

        public string GetParameterAsString(string name)
        {
            if (!_globalShellParameters.Exists(x => x.Name == name))
                return CurrentCommand.GetParameterAsString(name);
            var par = _globalShellParameters.FirstOrDefault(x => x.Name == name)?.Value;
            return par;
        }

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

        public bool GetSwitch(string name)
        {
            return CurrentCommand.GetSwitchValue(name);
        }

        private string StripCommandChar(string arg, string commandChar)
        {
            return arg.Substring(arg.IndexOf(commandChar, StringComparison.Ordinal) + commandChar.Length);
        }
    }
}
