using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Input;

namespace POS3K
{
    public static class Vars
    {
        public static string cashierId, cashierName;
        public static bool cashierStatus;

        public static int mainMenuSelected;
        public static bool equipmentIsOk;

        public static int regime;

        //public static bool isFree = true;
        public static bool goodsIsBusy = false;
        public static bool exchangeIsActive = false;

        public static bool fiscalMode;

        public static bool autonomode = false;

        public static byte brightness = 0xD3;

        public static string lastZDateTime;
    }
}
