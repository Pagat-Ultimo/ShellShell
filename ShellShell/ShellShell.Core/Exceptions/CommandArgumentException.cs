using System;
using System.Collections.Generic;
using System.Text;
using ShellShell.Core.Constants;

namespace ShellShell.Core.Exceptions
{
    public class CommandArgumentException : Exception
    {
        public CommandExceptionCode ExceptionCode { get; }
        public CommandArgumentException(string msg, CommandExceptionCode code = CommandExceptionCode.Unknown) : base(msg)
        {
            ExceptionCode = code;
        }
    }
}
