using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JointPPT
{
    public class ErrorInfo
    {
        public ErrorInfo(string name, string message)
        {
            Name = name;
            Message = message;
        }
        public string Name { get; set; }
        public string Message { get; set; }
    }
}
