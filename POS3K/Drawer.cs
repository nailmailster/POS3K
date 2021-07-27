using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using POS.Devices;

namespace POS3K
{
    public static class Drawer
    {
        public static bool Initialize()
        {
            MainWindow.drawer = new OPOSCashDrawerClass();
            MainWindow.drawer.Open("Drawer");
            MainWindow.drawer.ClaimDevice(1000);
            if (MainWindow.drawer.CheckHealth(1) == 105)
            {
                MainWindow.drawer.DeviceEnabled = true;
                return true;
            }
            else
                return false;
        }

        public static void OpenDrawer()
        {
            if (MainWindow.drawer.DeviceEnabled)
                MainWindow.drawer.OpenDrawer();
        }

        public static bool IsOpened()
        {
            return MainWindow.drawer.DrawerOpened;
        }
    }
}
