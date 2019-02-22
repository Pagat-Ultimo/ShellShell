using System;
using ShellShell.Core.Constants;

namespace ShellShell.Core.Exceptions
{
    /// <summary>
    /// Exception thrown from Command actions
    /// </summary>
    public class CommandArgumentException : Exception
    {
        #region  Properties

        /// <summary>
        /// The exception code representing the occured error
        /// </summary>
        public CommandExceptionCode ExceptionCode { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of the CommandArgumentException class
        /// </summary>
        /// <param name="msg">The error message</param>
        /// <param name="code">The error code representing the occured error</param>
        public CommandArgumentException(string msg, CommandExceptionCode code = CommandExceptionCode.Unknown) :
            base(msg)
        {
            ExceptionCode = code;
        }

        #endregion
    }
}