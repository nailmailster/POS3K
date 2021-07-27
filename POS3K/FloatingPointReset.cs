using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;

namespace POS3K
{
    class FloatingPointReset
    {
        //[DllImport("msvcrt120.dll", CallingConvention = CallingConvention.Cdecl)]
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int _fpreset();

        public static void Action()
        {
            _fpreset();
        }
    }
}
