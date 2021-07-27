using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using POS.Devices;

namespace POS3K
{
    public static class POSKeyboard
    {
        public static bool Initialize()
        {
            MainWindow.keyboard = new OPOSPOSKeyboardClass();
            MainWindow.keyboard.Open("Keyboard");
            MainWindow.keyboard.ClaimDevice(1000);
            if (MainWindow.keyboard.CheckHealth(1) == 105)
            {
                MainWindow.keyboard.DeviceEnabled = true;

                MainWindow.keyboard.DataEventEnabled = true;

                MainWindow.keyboard.DataEvent += Keyboard_DataEvent1;
                MainWindow.keyboard.DirectIOEvent += Keyboard_DirectIOEvent;
                MainWindow.keyboard.ErrorEvent += Keyboard_ErrorEvent;

                return true;
            }
            else
            {
                MessageBox.Show("Ошибка инициализации клавиатуры");
                return false;
            }
        }

        public static bool Close()
        {
            //MainWindow.keyboard.Close();
            return true;
        }

        private static void Keyboard_ErrorEvent(int ResultCode, int ResultCodeExtended, int ErrorLocus, ref int pErrorResponse)
        {
            MessageBox.Show("Keyboard_ErrorEvent");
        }

        private static void Keyboard_DirectIOEvent(int EventNumber, ref int pData, ref string pString)
        {
            MessageBox.Show("Keyboard_DirectIOEvent");
        }

        private static void Keyboard_DataEvent1(int Status)
        {
            MessageBox.Show("Status = " + Status.ToString() + " POSKeyData = " + MainWindow.keyboard.POSKeyData.ToString() + " POSKeyEventType = " + MainWindow.keyboard.POSKeyEventType.ToString());
            MainWindow.keyboard.DataEventEnabled = true;
        }
    }
}
