using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;

namespace POS3K
{
    class DLLClass
    {
        [DllImport("MyMax2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double myfun();

        [DllImport("MyMax2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int myfun2();

        [DllImport("MyMax2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int myfun3();

        [DllImport("MyMax2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool myfun7();

        [DllImport("MyMax2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool myfun10();

        [DllImport("MyMax2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool myfun11();

        [DllImport("MyMax2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool myfun12();

        [DllImport("MyMax2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool myfun13();

        public static double ActivateDLL()
        {
            return myfun();
        }
        public static int ActivateDLL2()
        {
            return myfun2();
        }
        public static int ActivateDLL3()
        {
            return myfun3();
        }
        public static bool ActivateDLL7()
        {
            return myfun7();
        }
        public static bool ActivateDLLRDS()
        {
            return myfun11();
        }
        public static bool ActivateDLLRDD()
        {
            return myfun10();
        }
        public static bool ActivateDLLRNS()
        {
            return myfun13();
        }
        public static bool ActivateDLLRND()
        {
            return myfun12();
        }
    }
}
