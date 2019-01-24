using System;
using System.Collections.Generic;
using System.Linq;
using ShellShell.Core.Constants;
using ShellShell.Core.Exceptions;

namespace ShellShell.Core.Models
{
    public class ShellCommand
    {
        public readonly List<string> Aliases;

        public string Name { get; }
        public string Description { get; }
        public bool ThrowOnInvalidSwitch { get; set; } = true;
        public bool ThrowOnInvalidParameter { get; set; } = true;
        public Action<ShellShellExecutor> CommandAction { get; }


        private readonly List<ShellParameter> _parameters;
        private readonly Dictionary<string, bool> _switches;

        public ShellCommand(string name, Action<ShellShellExecutor> action, string description = "")
        {
            Name = name;
            CommandAction = action;
            Description = description;
            Aliases = new List<string>();
            _switches = new Dictionary<string, bool>();
            _parameters = new List<ShellParameter>();
        }

        public void ConfigureParameter(string name, bool isMandatory = false, string defaultValue = "", string description = "")
        {
            if(_parameters.Exists(x => x.Name == name))
                throw new Exception($"There is already a parameter with the name {name} for the command {Name} registered");
            var parameter = new ShellParameter()
            {
                Name = name,
                Mandatory = isMandatory,
                Value = defaultValue,
                Description = description
            };
            _parameters.Add(parameter);
        }

        public void ConfigureSwitch(string name, bool defaultValue = false)
        {
            if (_switches.ContainsKey(name))
                throw new Exception($"There is already a switch with the name {name} for the command {Name} registered");
            _switches.Add(name, defaultValue);
        }

        public bool GetSwitchValue(string name)
        {
            if (!_switches.ContainsKey(name))
                throw new Exception($"Switch with the name {name} for the command {Name} is not registered");
            return _switches[name];
        }

        public void SetSwitch(string name, bool value)
        {
           
            if (_switches.ContainsKey(name))
                _switches[name] = value;

            if (ThrowOnInvalidSwitch)
                throw new CommandArgumentException($"Switch {name} is not known!", CommandExceptionCode.UnknownSwitch);
        }

        public void SetParameter(string name, string value)
        {
            var parameter = _parameters.FirstOrDefault(x => x.Name == name);
            if (parameter != null)
                parameter.Value = value;

            if (ThrowOnInvalidParameter)
                throw new Exception($"Parameter {name} not recognized");
        }

        public string GetParameterAsString(string name) 
        {
            if (!_parameters.Exists(x => x.Name == name))
                throw new Exception($"Parameter {name} not recognized");
            var par = _parameters.FirstOrDefault(x => x.Name == name)?.Value;
            return par;
        }

        public int GetParameterAsInt(string name) 
        {
            if (!_parameters.Exists(x => x.Name == name))
                throw new Exception($"Parameter {name} not recognized");
            var par = _parameters.FirstOrDefault(x => x.Name == name)?.Value;
            if (int.TryParse(par, out var result))
            {
                return result;
            }
            else
            {
                throw new Exception($"Value for Parameter {name}is not valid");
            }
        }

        public List<ShellParameter> GetMandatoryParameters()
        {
            return _parameters.Where(x => x.Mandatory).ToList();
        }
    }
}
