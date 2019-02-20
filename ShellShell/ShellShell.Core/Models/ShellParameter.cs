namespace ShellShell.Core.Models
{
    /// <summary>
    /// Class that describes a parameter that can be passed to a command
    /// </summary>
    public class ShellParameter
    {
        /// <summary>
        /// Gets or sets if the parameter is mandatory
        /// </summary>
        public bool Mandatory { get; set; }
        /// <summary>
        /// Gets or sets the name of the parameter
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the value of the parameter
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Gets or sets a description of the parameter
        /// </summary>
        public string Description { get; set; }
    }
}
