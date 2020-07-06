using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JointPPT
{
    public class NativeMethods
    {
        [DllImport("kernel32.dll", EntryPoint = "_lopen")]
        public static extern IntPtr Lopen(string lpPathName, int iReadWrite);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        public const int OF_READWRITE = 2;
        public const int OF_SHARE_DENY_NONE = 0x40;
        public readonly static IntPtr HFILE_ERROR = new IntPtr(-1);
    }
}
