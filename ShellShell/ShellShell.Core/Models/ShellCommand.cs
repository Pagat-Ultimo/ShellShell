using System;
using System.Collections.Generic;
using System.Linq;
using ShellShell.Core.Constants;
using ShellShell.Core.Exceptions;

namespace ShellShell.Core.Models
{
    /// <summary>
    /// Class that represents a executable command for the ShellExecutor
    /// </summary>
    public class ShellCommand
    {
        #region Fields

        private readonly Dictionary<string, bool> _switches;
        public readonly List<string> Aliases;

        #endregion

        #region  Properties

        /// <summary>
        /// Gets the name of the command
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the description of the command
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// Gets or sets if an exception should be thrown if an invalid switch was passed to the command. Default is TRUE
        /// </summary>
        public bool ThrowOnInvalidSwitch { get; set; } = true;
        /// <summary>
        /// Gets or sets if an exception should be thrown if an not configured parameter was passed to the command. Default is TRUE
        /// </summary>
        public bool ThrowOnInvalidParameter { get; set; } = true;
        /// <summary>
        /// The Action that will be invoked if the command should be executed
        /// </summary>
        public Action<ShellShellExecutor> CommandAction { get; }
        /// <summary>
        /// Gets the parameters that are configured for this command
        /// </summary>
        public List<ShellParameter> Parameters { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of the ShellCommand class
        /// </summary>
        /// <param name="name">The name of the command</param>
        /// <param name="action">The action that should be invoked</param>
        /// <param name="description">The description of the command. Will be used for the build in help command</param>
        public ShellCommand(string name, Action<ShellShellExecutor> action, string description = "")
        {
            Name = name;
            CommandAction = action;
            Description = description;
            Aliases = new List<string>();
            _switches = new Dictionary<string, bool>();
            Parameters = new List<ShellParameter>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Configures a parameter for the command
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        /// <param name="isMandatory">If the parameter should be mandatory</param>
        /// <param name="defaultValue">The default value if the parameter was not set by the user</param>
        /// <param name="description">The description of the parameter will be used by the build in help command</param>
        public void ConfigureParameter(string name, bool isMandatory = false, string defaultValue = "",
            string description = "")
        {
            if (Parameters.Exists(x => x.Name == name))
                throw new Exception(
                    $"There is already a parameter with the name {name} for the command {Name} registered");
            var parameter = new ShellParameter()
            {
                Name = name,
                Mandatory = isMandatory,
                Value = defaultValue,
                Description = description
            };
            Parameters.Add(parameter);
        }

        /// <summary>
        /// Configures a switch to be available for the command
        /// </summary>
        /// <param name="name">The name of the switch</param>
        /// <param name="defaultValue">The description of the switch. Will be used by the build in help command</param>
        public void ConfigureSwitch(string name, bool defaultValue = false)
        {
            if (_switches.ContainsKey(name))
                throw new Exception(
                    $"There is already a switch with the name {name} for the command {Name} registered");
            _switches.Add(name, defaultValue);
        }

        /// <summary>
        /// Gets the value for a specific switch
        /// </summary>
        /// <param name="name">The name of the switch to look for</param>
        /// <returns>The switch value</returns>
        public bool GetSwitchValue(string name)
        {
            if (!_switches.ContainsKey(name))
                throw new Exception($"Switch with the name {name} for the command {Name} is not registered");
            return _switches[name];
        }

        /// <summary>
        /// Sets the value for a specific switch
        /// </summary>
        /// <param name="name">The name of the switch</param>
        /// <param name="value">The value to be set</param>
        public void SetSwitch(string name, bool value)
        {
            if (_switches.ContainsKey(name))
                _switches[name] = value;

            if (ThrowOnInvalidSwitch)
                throw new CommandArgumentException($"Switch {name} is not known!", CommandExceptionCode.UnknownSwitch);
        }

        /// <summary>
        /// Sets the value for a specific parameter
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        /// <param name="value">The value to be set</param>
        public void SetParameter(string name, string value)
        {
            var parameter = Parameters.FirstOrDefault(x => x.Name == name);
            if (parameter != null)
                parameter.Value = value;

            if (ThrowOnInvalidParameter)
                throw new Exception($"Parameter {name} not recognized");
        }

        /// <summary>
        /// Gets the value for a specific parameter as a String
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        /// <returns>Parameter value as String</returns>
        public string GetParameterAsString(string name)
        {
            if (!Parameters.Exists(x => x.Name == name))
                throw new Exception($"Parameter {name} not recognized");
            var par = Parameters.FirstOrDefault(x => x.Name == name)?.Value;
            return par;
        }

        /// <summary>
        /// Gets the value for specific parameter as Int
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        /// <returns>Parameter value as Int</returns>
        public int GetParameterAsInt(string name)
        {
            if (!Parameters.Exists(x => x.Name == name))
                throw new Exception($"Parameter {name} not recognized");
            var par = Parameters.FirstOrDefault(x => x.Name == name)?.Value;
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
        /// Gets a list of mandatory parameters
        /// </summary>
        /// <returns></returns>
        public List<ShellParameter> GetMandatoryParameters()
        {
            return Parameters.Where(x => x.Mandatory).ToList();
        }

        #endregion
    }
}