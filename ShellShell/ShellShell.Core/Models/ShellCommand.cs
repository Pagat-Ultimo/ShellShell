using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShellShell.Core.Exceptions;

namespace ShellShell.Core.Models
{
    public class ShellCommand
    {
        public string Name { get; }
        public Action CommandAction { get; }
        private List<ShellParameter> _parameters;
        private Dictionary<string, bool> _switches;

        public string SwitchChar { get; set; } = "/";
        public ShellCommand(string name, Action action)
        {
            Name = name;
            CommandAction = action;
            _switches = new Dictionary<string, bool>();
            _parameters = new List<ShellParameter>();
        }

        public void ConfigureParameter(string name, bool isMandatory = false, string defaultValue = "")
        {
            if(_parameters.Exists(x => x.Name == name))
                throw new Exception($"There is already a parameter with the name {name} for the command {Name} registered");
            var parameter = new ShellParameter()
            {
                Name = name,
                Mandatory = isMandatory,
                Value = defaultValue
            };
            _parameters.Add(parameter);
        }

        public bool ConfigureSwitch(string text, bool defaultValue = false)
        {
            if (_switches.ContainsKey(SwitchChar + text))
                return false;
            _switches.Add(SwitchChar + text, defaultValue);
            return true;
        }

        public bool GetSwitchValue(string switchName)
        {
            if (!_switches.ContainsKey(SwitchChar + switchName))
                return false;
            return _switches[SwitchChar + switchName];
        }

        public void SetSwitch(string name, bool value)
        {
            if (!_switches.ContainsKey(name))
                throw new CommandArgumentException($"Switch {name} is not known!");
            _switches[name] = value;
        }

        public void SetParameter(string name, string value)
        {
            if (!_parameters.Exists(x => x.Name == name))
                throw new Exception($"Parameter {name} not recognized");
            _parameters.FirstOrDefault(x => x.Name == name).Value = value;
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
            var resultList = new List<ShellParameter>();
            var result = _parameters.Where(x => x.Mandatory == true);
            resultList.AddRange(result);
            return resultList;
        }
    }
}
