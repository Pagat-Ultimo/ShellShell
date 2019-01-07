using System;
using System.Collections.Generic;
using System.Text;

namespace ShellShell.Core.Exceptions
{
    public class CommandArgumentException : Exception
    {
        public CommandArgumentException(string msg) : base(msg)
        {

        }
    }
}
