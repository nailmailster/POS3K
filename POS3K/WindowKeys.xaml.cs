using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Windows.Forms;

using POS.Devices;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Media.Animation;

namespace POS3K
{
    /// <summary>
    /// Логика взаимодействия для WindowKeys.xaml
    /// </summary>
    public partial class WindowKeys : Window
    {
        public static OPOSScanner scanner;
        System.Windows.Controls.Label activeLabel, activeLabelDesc;

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public Keys key;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr extra;
        }

        //System level functions to be used for hook and unhook keyboard input
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyboardProc callback, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hook);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wp, IntPtr lp);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string name);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern short GetAsyncKeyState(Keys key);

        //Declaring Global objects
        private IntPtr ptrHook;
        private LowLevelKeyboardProc objKeyboardProcess;

        public WindowKeys()
        {
            ProcessModule objCurrentModule = Process.GetCurrentProcess().MainModule;
            objKeyboardProcess = new LowLevelKeyboardProc(captureKey);
            ptrHook = SetWindowsHookEx(13, objKeyboardProcess, GetModuleHandle(objCurrentModule.ModuleName), 0);

            InitializeComponent();

            btn1Label.Content = "СБРОС";
            btn2Label.Content = Properties.Settings.Default.btn2Barcode;
            btn3Label.Content = Properties.Settings.Default.btn3Barcode;
            btn4Label.Content = Properties.Settings.Default.btn4Barcode;
            btn5Label.Content = Properties.Settings.Default.btn5Barcode;
            btn6Label.Content = Properties.Settings.Default.btn6Barcode;
            btn7Label.Content = Properties.Settings.Default.btn7Barcode;
            btn8Label.Content = Properties.Settings.Default.btn8Barcode;
            btn9Label.Content = Properties.Settings.Default.btn9Barcode;
            btn10Label.Content = Properties.Settings.Default.btn10Barcode;
            btn11Label.Content = Properties.Settings.Default.btn11Barcode;
            btn12Label.Content = Properties.Settings.Default.btn12Barcode;
            btn13Label.Content = Properties.Settings.Default.btn13Barcode;
            btn14Label.Content = Properties.Settings.Default.btn14Barcode;
            btn15Label.Content = Properties.Settings.Default.btn15Barcode;
            btn16Label.Content = Properties.Settings.Default.btn16Barcode;
            btn17Label.Content = Properties.Settings.Default.btn17Barcode;
            btn18Label.Content = Properties.Settings.Default.btn18Barcode;
            btn19Label.Content = "WINDOWS";
            btn20Label.Content = Properties.Settings.Default.btn20Barcode;

            btn1LabelDesc.Content = "MUMKIN EMAS";
            btn2LabelDesc.Content = GoodsDBF.getDescriptionByBarcode(btn2Label.Content.ToString());
            btn3LabelDesc.Content = GoodsDBF.getDescriptionByBarcode(btn3Label.Content.ToString());
            btn4LabelDesc.Content = GoodsDBF.getDescriptionByBarcode(btn4Label.Content.ToString());
            btn5LabelDesc.Content = GoodsDBF.getDescriptionByBarcode(btn5Label.Content.ToString());
            btn6LabelDesc.Content = GoodsDBF.getDescriptionByBarcode(btn6Label.Content.ToString());
            btn7LabelDesc.Content = GoodsDBF.getDescriptionByBarcode(btn7Label.Content.ToString());
            btn8LabelDesc.Content = GoodsDBF.getDescriptionByBarcode(btn8Label.Content.ToString());
            btn9LabelDesc.Content = GoodsDBF.getDescriptionByBarcode(btn9Label.Content.ToString());
            btn10LabelDesc.Content = GoodsDBF.getDescriptionByBarcode(btn10Label.Content.ToString());
            btn11LabelDesc.Content = GoodsDBF.getDescriptionByBarcode(btn11Label.Content.ToString());
            btn12LabelDesc.Content = GoodsDBF.getDescriptionByBarcode(btn12Label.Content.ToString());
            btn13LabelDesc.Content = GoodsDBF.getDescriptionByBarcode(btn13Label.Content.ToString());
            btn14LabelDesc.Content = GoodsDBF.getDescriptionByBarcode(btn14Label.Content.ToString());
            btn15LabelDesc.Content = GoodsDBF.getDescriptionByBarcode(btn15Label.Content.ToString());
            btn16LabelDesc.Content = GoodsDBF.getDescriptionByBarcode(btn16Label.Content.ToString());
            btn17LabelDesc.Content = GoodsDBF.getDescriptionByBarcode(btn17Label.Content.ToString());
            btn18LabelDesc.Content = GoodsDBF.getDescriptionByBarcode(btn18Label.Content.ToString());
            btn19LabelDesc.Content = "MUMKIN EMAS";
            btn20LabelDesc.Content = GoodsDBF.getDescriptionByBarcode(btn20Label.Content.ToString());

            if (Properties.Settings.Default.UseOPOSScanner)
                InitializeScanner();
        }

        private IntPtr captureKey(int nCode, IntPtr wp, IntPtr lp)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));

                if (objKeyInfo.key == Keys.RWin || objKeyInfo.key == Keys.LWin) // Disabling Windows keys
                {
                    return (IntPtr)1;
                }
            }
            return CallNextHookEx(ptrHook, nCode, wp, lp);
        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            Topmost = true;
        }

        private void ButtonSaveKeys_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.btn1Barcode = btn1Label.Content.ToString();
            Properties.Settings.Default.btn2Barcode = btn2Label.Content.ToString();
            Properties.Settings.Default.btn3Barcode = btn3Label.Content.ToString();
            Properties.Settings.Default.btn4Barcode = btn4Label.Content.ToString();
            Properties.Settings.Default.btn5Barcode = btn5Label.Content.ToString();
            Properties.Settings.Default.btn6Barcode = btn6Label.Content.ToString();
            Properties.Settings.Default.btn7Barcode = btn7Label.Content.ToString();
            Properties.Settings.Default.btn8Barcode = btn8Label.Content.ToString();
            Properties.Settings.Default.btn9Barcode = btn9Label.Content.ToString();
            Properties.Settings.Default.btn10Barcode = btn10Label.Content.ToString();
            Properties.Settings.Default.btn11Barcode = btn11Label.Content.ToString();
            Properties.Settings.Default.btn12Barcode = btn12Label.Content.ToString();
            Properties.Settings.Default.btn13Barcode = btn13Label.Content.ToString();
            Properties.Settings.Default.btn14Barcode = btn14Label.Content.ToString();
            Properties.Settings.Default.btn15Barcode = btn15Label.Content.ToString();
            Properties.Settings.Default.btn16Barcode = btn16Label.Content.ToString();
            Properties.Settings.Default.btn17Barcode = btn17Label.Content.ToString();
            Properties.Settings.Default.btn18Barcode = btn18Label.Content.ToString();
            Properties.Settings.Default.btn19Barcode = btn19Label.Content.ToString();
            Properties.Settings.Default.btn20Barcode = btn20Label.Content.ToString();

            Properties.Settings.Default.Save();

            //Close();
            //Owner.Show();
            UnhookWindowsHookEx(ptrHook);
            Owner.Show();
            //  сделаем окно-владелец (окно меню) непрозрачным (видимым)
            DoubleAnimation doubleAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
            //  стартуем анимацию непрозрачности
            Owner.BeginAnimation(OpacityProperty, doubleAnimation);
            Close();
        }

        private void ButtonCloseKeys_Click(object sender, RoutedEventArgs e)
        {
            UnhookWindowsHookEx(ptrHook);
            Owner.Show();
            //  сделаем окно-владелец (окно меню) непрозрачным (видимым)
            DoubleAnimation doubleAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
            //  стартуем анимацию непрозрачности
            Owner.BeginAnimation(OpacityProperty, doubleAnimation);
            Close();
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.A)    //  кнопка 50
            {
                ClearBackgrounds();
                btn5Stack.Background = Brushes.Yellow;
                activeLabel = btn5Label;
                activeLabelDesc = btn5LabelDesc;
                e.Handled = true;
            }
            else if (e.Key == Key.B)    //  кнопка под 50
            {
                ClearBackgrounds();
                btn9Stack.Background = Brushes.Yellow;
                activeLabel = btn9Label;
                activeLabelDesc = btn9LabelDesc;
                e.Handled = true;
            }
            else if (e.Key == Key.C)    //  кнопка 100
            {
                ClearBackgrounds();
                btn13Stack.Background = Brushes.Yellow;
                activeLabel = btn13Label;
                activeLabelDesc = btn13LabelDesc;
                e.Handled = true;
            }
            else if (e.Key == Key.D)    //  кнопка под 100
            {
                ClearBackgrounds();
                btn17Stack.Background = Brushes.Yellow;
                activeLabel = btn17Label;
                activeLabelDesc = btn17LabelDesc;
                e.Handled = true;
            }
            else if (e.Key == Key.F1)    //  кнопка МЫЛО ХОЗ
            {
                ClearBackgrounds();
                btn2Stack.Background = Brushes.Yellow;
                activeLabel = btn2Label;
                activeLabelDesc = btn2LabelDesc;
                e.Handled = true;
            }
            else if (e.Key == Key.F2)    //  кнопка ЯЙЦО
            {
                ClearBackgrounds();
                btn3Stack.Background = Brushes.Yellow;
                activeLabel = btn3Label;
                activeLabelDesc = btn3LabelDesc;
                e.Handled = true;
            }
            else if (e.Key == Key.F3)    //  кнопка ЯЙЦО D2
            {
                ClearBackgrounds();
                btn4Stack.Background = Brushes.Yellow;
                activeLabel = btn4Label;
                activeLabelDesc = btn4LabelDesc;
                e.Handled = true;
            }
            else if (e.Key == Key.F4)    //  кнопка ТЕСТО МУЗА
            {
                ClearBackgrounds();
                btn6Stack.Background = Brushes.Yellow;
                activeLabel = btn6Label;
                activeLabelDesc = btn6LabelDesc;
                e.Handled = true;
            }
            else if (e.Key == Key.F5)    //  кнопка ЖВАЧКА
            {
                ClearBackgrounds();
                btn7Stack.Background = Brushes.Yellow;
                activeLabel = btn7Label;
                activeLabelDesc = btn7LabelDesc;
                e.Handled = true;
            }
            else if (e.Key == Key.F6)    //  кнопка САЛАТ
            {
                ClearBackgrounds();
                btn8Stack.Background = Brushes.Yellow;
                activeLabel = btn8Label;
                activeLabelDesc = btn8LabelDesc;
                e.Handled = true;
            }
            else if (e.Key == Key.F7)    //  кнопка ЗЕЛЕНЬ
            {
                ClearBackgrounds();
                btn10Stack.Background = Brushes.Yellow;
                activeLabel = btn10Label;
                activeLabelDesc = btn10LabelDesc;
                e.Handled = true;
            }
            else if (e.Key == Key.F8)    //  кнопка СЫРОК
            {
                ClearBackgrounds();
                btn11Stack.Background = Brushes.Yellow;
                activeLabel = btn11Label;
                activeLabelDesc = btn11LabelDesc;
                e.Handled = true;
            }
            else if (e.Key == Key.F9)    //  кнопка СВЕЧИ
            {
                ClearBackgrounds();
                btn12Stack.Background = Brushes.Yellow;
                activeLabel = btn12Label;
                activeLabelDesc = btn12LabelDesc;
                e.Handled = true;
            }
            else if (e.SystemKey == Key.F10)    //  кнопка ЧУПА-ЧУПС
            {
                ClearBackgrounds();
                btn14Stack.Background = Brushes.Yellow;
                activeLabel = btn14Label;
                activeLabelDesc = btn14LabelDesc;
                e.Handled = true;
            }
            else if (e.Key == Key.F11)    //  кнопка ПЕР. ЯЙЦО
            {
                ClearBackgrounds();
                btn15Stack.Background = Brushes.Yellow;
                activeLabel = btn15Label;
                activeLabelDesc = btn15LabelDesc;
                e.Handled = true;
            }
            else if (e.Key == Key.F12)    //  кнопка ПЕРЕЦ
            {
                ClearBackgrounds();
                btn16Stack.Background = Brushes.Yellow;
                activeLabel = btn16Label;
                activeLabelDesc = btn16LabelDesc;
                e.Handled = true;
            }
            else if (e.Key == Key.Tab)    //  кнопка СПИЧКИ
            {
                ClearBackgrounds();
                btn18Stack.Background = Brushes.Yellow;
                activeLabel = btn18Label;
                activeLabelDesc = btn18LabelDesc;
                e.Handled = true;
            }
            else if (e.Key == Key.LWin)    //  кнопка ШАРИК С РИС
            {
                ClearBackgrounds();
                btn19Stack.Background = Brushes.Yellow;
                activeLabel = btn19Label;
                activeLabelDesc = btn19LabelDesc;
                e.Handled = true;
            }
            else if (e.SystemKey == Key.LeftAlt)    //  кнопка ЧЕСНОК
            {
                ClearBackgrounds();
                btn20Stack.Background = Brushes.Yellow;
                activeLabel = btn20Label;
                activeLabelDesc = btn20LabelDesc;
                e.Handled = true;
            }
        }

        private void ClearBackgrounds()
        {
            btn1Stack.Background = Brushes.Gray;
            btn2Stack.Background = Brushes.Gray;
            btn3Stack.Background = Brushes.Gray;
            btn4Stack.Background = Brushes.Gray;
            btn5Stack.Background = Brushes.Gray;
            btn6Stack.Background = Brushes.Gray;
            btn7Stack.Background = Brushes.Gray;
            btn8Stack.Background = Brushes.Gray;
            btn9Stack.Background = Brushes.Gray;
            btn10Stack.Background = Brushes.Gray;
            btn11Stack.Background = Brushes.Gray;
            btn12Stack.Background = Brushes.Gray;
            btn13Stack.Background = Brushes.Gray;
            btn14Stack.Background = Brushes.Gray;
            btn15Stack.Background = Brushes.Gray;
            btn16Stack.Background = Brushes.Gray;
            btn17Stack.Background = Brushes.Gray;
            btn18Stack.Background = Brushes.Gray;
            btn19Stack.Background = Brushes.Gray;
            btn20Stack.Background = Brushes.Gray;
        }

        public bool InitializeScanner()
        {
            scanner = new OPOSScanner();
            scanner.Open("Scanner");
            scanner.ClaimDevice(1000);

            if (scanner.CheckHealth(1) == 105)
            {
                scanner.DeviceEnabled = true;
                scanner.DecodeData = true;
                scanner.DataEvent += Scanner_DataEvent;
                scanner.DataEventEnabled = true;
                return true;
            }
            else
            {
                if (Properties.Settings.Default.UseOPOSScanner)
                    System.Windows.MessageBox.Show("Ошибка инициализации сканера");
                return false;
            }
        }

        private void Scanner_DataEvent(int Status)
        {
            int scanDataType = scanner.ScanDataType;

            string scanData = scanner.ScanData;
            scanData = scanData.Substring(4, scanData.Length - 5);

            if (activeLabel != null)
            {
                activeLabel.Content = scanData;
                activeLabelDesc.Content = GoodsDBF.getDescriptionByBarcode(activeLabel.Content.ToString());
            }

            scanner.DataEventEnabled = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            scanner.ReleaseDevice();
            scanner.Close();
        }
    }
}
