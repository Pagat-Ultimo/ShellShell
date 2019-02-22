namespace ShellShell.Core.Constants
{
    /// <summary>
    /// Exception codes thrown from Commands
    /// </summary>
    public enum CommandExceptionCode
    {
        /// <summary>
        /// Unknown error
        /// </summary>
        Unknown,

        /// <summary>
        /// Switch was entered but not configured
        /// </summary>
        UnknownSwitch,

        /// <summary>
        /// The Command you tried to configure was already configured on the ShellShellExecutor
        /// </summary>
        CommandAlreadyConfigured
    }
}