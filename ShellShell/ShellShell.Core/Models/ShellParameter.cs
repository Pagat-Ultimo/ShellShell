using System;
using System.Collections.Generic;
using System.Text;

namespace ShellShell.Core.Models
{
    public class ShellParameter
    {
        public bool Mandatory { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
