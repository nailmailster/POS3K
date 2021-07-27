using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using POS.Devices;

namespace POS3K
{
    public static class Scanner
    {
        public static bool Initialize()
        {
            MainWindow.scanner = new OPOSScanner();
            MainWindow.scanner.Open("Scanner");
            MainWindow.scanner.ClaimDevice(1000);

            if (MainWindow.scanner.CheckHealth(1) == 105)
            {
                MainWindow.scanner.DeviceEnabled = true;
                MainWindow.scanner.DecodeData = true;
                MainWindow.scanner.DataEvent += Scanner_DataEvent;
                MainWindow.scanner.DataEventEnabled = true;
                return true;
            }
            else
            {
                MessageBox.Show("Ошибка инициализации сканера");
                return false;
            }

            //Window2.scanner = new OPOSScanner();
            //Window2.scanner.Open("Scanner");
            //Window2.scanner.ClaimDevice(1000);

            //if (Window2.scanner.CheckHealth(1) == 105)
            //{
            //    Window2.scanner.DeviceEnabled = true;
            //    Window2.scanner.DecodeData = true;
            //    Window2.scanner.DataEvent += Scanner_DataEvent;
            //    Window2.scanner.DataEventEnabled = true;
            //    return true;
            //}
            //else
            //{
            //    MessageBox.Show("Ошибка инициализации сканера");
            //    return false;
            //}
        }

        private static void Scanner_DataEvent(int Status)
        {
            string scanData = Window2.scanner.ScanData;
            scanData = scanData.Substring(4, scanData.Length - 4);
            MessageBox.Show(scanData);
            //MessageBox.Show(MainWindow.scanner.ScanDataType.ToString());
            Window2.scanner.DataEventEnabled = true;
        }
    }
}
