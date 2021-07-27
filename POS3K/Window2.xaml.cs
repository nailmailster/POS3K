using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
//using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using POS.Devices;

namespace POS3K
{
    public partial class Window2 : Window
    {
        DoubleAnimation doubleAnimationLabelGotDiscount;

        public ActivePosition activePosition = new ActivePosition();
        public ActiveCheck activeCheck = new ActiveCheck();

        public static OPOSScanner scanner;
        public static OPOSMSR msr;
        //public static OPOSToneIndicator tone;

        public int width = 44;

        //public System.Timers.Timer timer;
        public DispatcherTimer timer;

        //DateTime startTime, endTime, eTime;

        Fiscal fiscal = new Fiscal();

        PrintDialog printDialog = new System.Windows.Controls.PrintDialog();
        DrawingVisual visual;
        DrawingContext dc;
        Effect monochromeEffect;
        Effect transition_Ripple_modified_with_MonochromeEffect;
        ColorAnimation colorAnimationlabelAutonomode;
        DoubleAnimation doubleAnimationCanvasMainParameter;
        Effect transition_Ripple_modifiedEffect;
        DoubleAnimation doubleAnimationLabelTotalFontSize;
        DoubleAnimation doubleAnimationLabelChangeFontSize;
        DoubleAnimation doubleAnimationLabelDiscountSumFontSize;
        DoubleAnimation doubleAnimationLabelLoyaltyFontSize;
        //DoubleAnimation doubleAnimationLabelLoyaltyFontSize;
        BlurEffect blurEffect;

        double printedHeight = 0;

        int escapes = 0;

        SolidColorBrush animatedBrushCancel = new SolidColorBrush()
        {
            Color = (Color)ColorConverter.ConvertFromString("#252525")
        };

        Process currentProcess = Process.GetCurrentProcess();

        WindowAutorization windowAuthorization;
        int keysAfterAuthorization = 100;

        //[StructLayout(LayoutKind.Sequential)]
        //private struct KBDLLHOOKSTRUCT
        //{
        //    public Keys key;
        //    public int scanCode;
        //    public int flags;
        //    public int time;
        //    public IntPtr extra;
        //}

        ////System level functions to be used for hook and unhook keyboard input
        //private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyboardProc callback, IntPtr hMod, uint dwThreadId);
        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //private static extern bool UnhookWindowsHookEx(IntPtr hook);
        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wp, IntPtr lp);
        //[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //private static extern IntPtr GetModuleHandle(string name);
        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //private static extern short GetAsyncKeyState(Keys key);

        ////Declaring Global objects
        //private IntPtr ptrHook;
        //private LowLevelKeyboardProc objKeyboardProcess;

        //------------------------------------------------- hook start
        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hHook);

        private static readonly InterceptKeys.LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        //------------------------------------------------- hook finish

        SolidColorBrush animatedBrush = new SolidColorBrush()
        {
            Color = (Color)ColorConverter.ConvertFromString("#252525")
        };

        OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Properties.Settings.Default.DBF + ";Extended Properties=dBase IV;User ID=;Password=;Mode=Read");

        public Window2()
        {
            //ProcessModule objCurrentModule = Process.GetCurrentProcess().MainModule;
            //objKeyboardProcess = new LowLevelKeyboardProc(captureKey);
            //ptrHook = SetWindowsHookEx(13, objKeyboardProcess, GetModuleHandle(objCurrentModule.ModuleName), 0);

            InitializeComponent();

            try
            {
                width = Convert.ToInt32(Properties.Settings.Default.SymbolsPerLine);
            }
            catch { }

            labelTotal.DataContext = activeCheck;
            labelPositionBarcodeName.DataContext = activePosition;

            labelQuantity.DataContext = activePosition;

            labelCouponSum.DataContext = activeCheck;
            labelDiscountSum.DataContext = activeCheck;
            labelTopaySum.DataContext = activeCheck;

            labelAcceptedInCash.DataContext = activeCheck;
            labelAcceptedByCard.DataContext = activeCheck;
            labelChange.DataContext = activeCheck;

            labelLoyalty.DataContext = activeCheck;

            activePosition.RegimeChanged += ActivePosition_RegimeChanged;

            activeCheck.TotalSumChanged += ActiveCheck_TotalSumChanged;
            activeCheck.CouponSumChanged += ActiveCheck_CouponSumChanged;
            activeCheck.DiscountSumChanged += ActiveCheck_DiscountSumChanged;
            activeCheck.TopaySumChanged += ActiveCheck_TopaySumChanged;

            activeCheck.CashSumChanged += ActiveCheck_CashSumChanged;
            activeCheck.CardSumChanged += ActiveCheck_CardSumChanged;
            activeCheck.ChangeSumChanged += ActiveCheck_ChangeSumChanged;

            activeCheck.LoyaltySumChanged += ActiveCheck_LoyaltySumChanged;

            if (Properties.Settings.Default.UseOPOSScanner)
                InitializeScanner();
            InitializeMSR();
            //InitializeTone();

            this.Cursor = System.Windows.Input.Cursors.None;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += Timer_Tick;
            timer.Start();

            colorAnimationlabelAutonomode = new ColorAnimation(System.Windows.Media.Colors.Orange, new Duration(TimeSpan.FromSeconds(0.25)))
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            monochromeEffect = new MonochromeEffect()
            {
                FilterColor = Color.FromRgb(255, 255, 0)
            };

            transition_Ripple_modified_with_MonochromeEffect = new Transition_Ripple_modified_with_MonochromeEffect()
            {
                FilterColor = Color.FromRgb(255, 255, 0)
            };

            doubleAnimationCanvasMainParameter = new DoubleAnimation(80, 95, new Duration(TimeSpan.FromSeconds(1.0)))
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            doubleAnimationLabelTotalFontSize = new DoubleAnimation(42, 60, new Duration(TimeSpan.FromSeconds(0.25)))
            {
                AutoReverse = true
            };

            blurEffect = new BlurEffect() { Radius = 5 };

            doubleAnimationLabelChangeFontSize = new DoubleAnimation(32, 50, new Duration(TimeSpan.FromSeconds(0.25)))
            {
                AutoReverse = true
            };

            doubleAnimationLabelChangeFontSize = new DoubleAnimation(32, 36, new Duration(TimeSpan.FromSeconds(0.35)))
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            doubleAnimationLabelGotDiscount = new DoubleAnimation(150, 0, new Duration(TimeSpan.FromSeconds(1)));

            doubleAnimationLabelDiscountSumFontSize = new DoubleAnimation(42, 60, new Duration(TimeSpan.FromSeconds(0.25)))
            {
                AutoReverse = true
            };

            doubleAnimationLabelLoyaltyFontSize = new DoubleAnimation(32, 50, new Duration(TimeSpan.FromSeconds(0.25)))
            {
                AutoReverse = true
            };

            //doubleAnimationLabelLoyaltyFontSize = new DoubleAnimation(32, 36, new Duration(TimeSpan.FromSeconds(0.35)))
            //{
            //    AutoReverse = true,
            //    RepeatBehavior = RepeatBehavior.Forever
            //};

            escapes = 0;
        }

        //------------------------------------------------- hook start
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            _hookID = InterceptKeys.SetHook(_proc);
        }

        public static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                System.Windows.Forms.Keys key = (System.Windows.Forms.Keys)vkCode;

                if (key == System.Windows.Forms.Keys.LWin)
                    return (IntPtr)1; // Handled.
                if (key == System.Windows.Forms.Keys.RWin)
                    return (IntPtr)1; // Handled.
            }

            return InterceptKeys.CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
        //------------------------------------------------- hook finish

        private void Timer_Tick(object sender, EventArgs e)
        {
#region Autonomode on/off
            //labelAutonoMode.Foreground = animatedBrush;
            if (GC.GetTotalMemory(false) > 46000)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            if (activeCheck.autonoMode && !Vars.autonomode)
            {
                if (activeCheck.returnMode)
                {
                    canvasMain.Effect = monochromeEffect;
                }
                else
                    canvasMain.Effect = null;
                labelAutonoMode.Style = Resources["InactivePanelLabel"] as Style;
                animatedBrush.BeginAnimation(SolidColorBrush.ColorProperty, null);
                labelAutonoMode.Foreground = new SolidColorBrush(Color.FromRgb(0x25, 0x25, 0x25));

                activeCheck.autonoMode = !activeCheck.autonoMode;
            }
            else if (!activeCheck.autonoMode && Vars.autonomode)
            {
                labelAutonoMode.Foreground = animatedBrush;
                labelAutonoMode.Style = Resources["ActiveAutonomodePanelLabel"] as Style;
                animatedBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimationlabelAutonomode, HandoffBehavior.SnapshotAndReplace);

                //canvasMain.Effect = new InvertColorEffect();
                //Effect sine1Effect = new Sine1Effect();
                //canvasMain.Effect = sine1Effect;
                //DoubleAnimation doubleAnimationCanvasMainParameter = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(1.0)))
                //{
                //    AutoReverse = true,
                //    RepeatBehavior = RepeatBehavior.Forever
                //};
                //sine1Effect.BeginAnimation(Sine1Effect.ParameterProperty, doubleAnimationCanvasMainParameter);
                if (activeCheck.returnMode)
                {
                    if (Properties.Settings.Default.UseShaderInAutonomode)
                    {
                        canvasMain.Effect = transition_Ripple_modified_with_MonochromeEffect;
                        transition_Ripple_modified_with_MonochromeEffect.BeginAnimation(Transition_Ripple_modified_with_MonochromeEffect.ProgressProperty, doubleAnimationCanvasMainParameter);
                    }
                }
                else
                {
                    if (Properties.Settings.Default.UseShaderInAutonomode)
                    {
                        transition_Ripple_modifiedEffect = new Transition_Ripple_modifiedEffect();
                        canvasMain.Effect = transition_Ripple_modifiedEffect;
                        transition_Ripple_modifiedEffect.BeginAnimation(Transition_Ripple_modifiedEffect.ProgressProperty, doubleAnimationCanvasMainParameter);
                    }
                }

                activeCheck.autonoMode = !activeCheck.autonoMode;
            }
#endregion Autonomode on/off
        }

        //private IntPtr captureKey(int nCode, IntPtr wp, IntPtr lp)
        //{
        //    if (nCode >= 0)
        //    {
        //        KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));

        //        if (objKeyInfo.key == Keys.RWin || objKeyInfo.key == Keys.LWin) // Disabling Windows keys
        //        {
        //            return (IntPtr)1;
        //        }
        //    }
        //    try
        //    {
        //        return CallNextHookEx(ptrHook, nCode, wp, lp);
        //    }
        //    catch
        //    {
        //        return (IntPtr)1;
        //    }
        //}

        //public bool InitializeTone()
        //{
        //    tone = new OPOSToneIndicator();
        //    tone.Open("Tone");
        //    tone.ClaimDevice(1000);

        //    if (tone.CheckHealth(1) == 105)
        //    {
        //        tone.DeviceEnabled = true;
        //        return true;
        //    }
        //    else
        //    {
        //        System.Windows.MessageBox.Show("Ошибка инициализации тонового индикатора");
        //        return false;
        //    }
        //}

        public bool InitializeMSR()
        {
            msr = new OPOSMSR();
            msr.Open("MSR");
            msr.ClaimDevice(1000);

            if (msr.CheckHealth(1) == 105)
            {
                msr.DeviceEnabled = true;
                msr.DataEvent += Msr_DataEvent;
                msr.DataEventEnabled = true;
                return true;
            }
            else
            {
                System.Windows.MessageBox.Show("Ошибка инициализации магнитного считывателя");
                return false;
            }
        }

        private void Msr_DataEvent(int Status)
        {
            //MessageBox.Show(msr.Track1Data);
            msr.DataEventEnabled = true;
            if (activeCheck.couponMode)
            {
                textBoxCouponCode.Text = msr.Track1Data;

                if (Keyboard.PrimaryDevice != null)
                {
                    var e = new System.Windows.Input.KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Return)
                    {
                        RoutedEvent = Keyboard.KeyUpEvent
                    };

                    InputManager.Current.ProcessInput(e);
                }
            }
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
            //if (scanDataType != 103 && scanDataType != 104)
            //{
            //    scanner.DataEventEnabled = true;
            //    return;
            //}

            string scanData = scanner.ScanData;
            scanData = scanData.Substring(4, scanData.Length - 5);

            if (!activeCheck.finished && activeCheck.preFinished)
            {
                FinishCheck();

                if (!fiscal.CompareDateTime())
                {
                    fiscal.Close();
                    System.Windows.MessageBox.Show("Не сделан суточный отчет!");
                    this.Owner.Show();
                    this.Owner.Activate();
                    this.Owner.Opacity = 1;
                    Close();
                }
            }

            if (activeCheck.returnMode && activeCheck.specification.Count > 0 && !activeCheck.preFinished && !activeCheck.finished)
            {
                scanner.DataEventEnabled = true;
                return;
            }

            if (activeCheck.totalMode || activeCheck.paymentMode)
            {
                scanner.DataEventEnabled = true;
                return;
            }

            if (activePosition.Quantity == 0)
            {
                panelQuantity.Visibility = Visibility.Collapsed;
                labelQuantity.Content = 0;
                panelBarcode.Visibility = Visibility.Collapsed;
                labelBarcode.Content = "";
                panelDescription.Visibility = Visibility.Collapsed;
                labelDescriptionWin2.Content = "";
            }
            //  Режим готовности к добавлению позиций
            if (activePosition.Regime == ActivePositionModes.codeReady)
            {
                //activeCheck.finished = false;
                //  Если это не первая позиция документа, очистим Info от содержимого, оставшегося после ввода предыдущей позиции
                if (activePosition.Info.Length > 0)
                    activePosition.Info = "";
                //  Переключаем в режим "позиция добавляется"
                activePosition.Regime = ActivePositionModes.codeInput;
            }

            if (activeCheck.couponMode)
            {
                textBoxCouponCode.Text = scanData;

                if (Keyboard.PrimaryDevice != null)
                {
                    //var e = new System.Windows.Input.KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Return)
                    //{
                    //    RoutedEvent = Keyboard.KeyUpEvent
                    //};
                    activePosition.Regime = ActivePositionModes.codeReady;
                    var e = new System.Windows.Input.KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Return)
                    {
                        RoutedEvent = Keyboard.KeyUpEvent
                    };
                    InputManager.Current.ProcessInput(e);
                }
            }
            //  Режим "позиция добавляется"
            else if (activePosition.Regime == ActivePositionModes.codeInput)
            {
                //  Добавляем символ в Info и Barcode
                activePosition.Info += scanData;
                activePosition.Barcode = scanData;

                //MessageBox.Show(scanData);
                //MessageBox.Show(scanner.ScanDataType.ToString());

                if (Keyboard.PrimaryDevice != null)
                {
                    //var e = new System.Windows.Input.KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Return)
                    //{
                    //    RoutedEvent = Keyboard.KeyUpEvent
                    //};
                    var e = new System.Windows.Input.KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Return)
                    {
                        RoutedEvent = Keyboard.KeyUpEvent
                    };
                    InputManager.Current.ProcessInput(e);
                }
            }
            scanner.DataEventEnabled = true;
        }

        private void SendBarcode(string scanData)
        {
            if (!activeCheck.finished && activeCheck.preFinished)
            {
                FinishCheck();

                if (!fiscal.CompareDateTime())
                {
                    fiscal.Close();
                    System.Windows.MessageBox.Show("Не сделан суточный отчет!");
                    this.Owner.Show();
                    this.Owner.Activate();
                    this.Owner.Opacity = 1;
                    Close();
                }
            }
            if (activePosition.Quantity == 0)
            {
                panelQuantity.Visibility = Visibility.Collapsed;
                labelQuantity.Content = 0;
                panelBarcode.Visibility = Visibility.Collapsed;
                labelBarcode.Content = "";
                panelDescription.Visibility = Visibility.Collapsed;
                labelDescriptionWin2.Content = "";
            }
            //  Режим готовности к добавлению позиций
            if (activePosition.Regime == ActivePositionModes.codeReady)
            {
                //activeCheck.finished = false;
                //  Если это не первая позиция документа, очистим Info от содержимого, оставшегося после ввода предыдущей позиции
                if (activePosition.Info.Length > 0)
                    activePosition.Info = "";
                //  Переключаем в режим "позиция добавляется"
                activePosition.Regime = ActivePositionModes.codeInput;
            }

            if (activeCheck.couponMode)
            {
                textBoxCouponCode.Text = scanData;

                if (Keyboard.PrimaryDevice != null)
                {
                    var e = new System.Windows.Input.KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Return)
                    {
                        RoutedEvent = Keyboard.KeyUpEvent
                    };
                    InputManager.Current.ProcessInput(e);
                }
            }
            //  Режим "позиция добавляется"
            else if (activePosition.Regime == ActivePositionModes.codeInput)
            {
                //  Добавляем символ в Info и Barcode
                activePosition.Info += scanData;
                activePosition.Barcode = scanData;

                //MessageBox.Show(scanData);
                //MessageBox.Show(scanner.ScanDataType.ToString());

                if (Keyboard.PrimaryDevice != null)
                {
                    //var e = new System.Windows.Input.KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Return)
                    //{
                    //    RoutedEvent = Keyboard.KeyUpEvent
                    //};
                    var e = new System.Windows.Input.KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Return)
                    {
                        RoutedEvent = Keyboard.KeyUpEvent
                    };
                    InputManager.Current.ProcessInput(e);
                }
            }
        }

        private void ShowErrorCanvas()
        {
            return;
            //  Анимируем canvasBackground
            DoubleAnimation doubleAnimationCanvasBackground = new DoubleAnimation(0.7, new Duration(TimeSpan.FromSeconds(0.10)))
            {
                AutoReverse = true,
            };
            doubleAnimationCanvasBackground.Completed += DoubleAnimationCanvasBackground_Completed;
            System.Windows.Controls.Panel.SetZIndex(canvasBackground, 10);
            canvasBackground.BeginAnimation(OpacityProperty, doubleAnimationCanvasBackground);
        }

        private void DoubleAnimationLabelGotDiscount_Completed(object sender, EventArgs e)
        {
            doubleAnimationLabelGotDiscount.Completed -= DoubleAnimationLabelGotDiscount_Completed;
            if (labelGotDiscount.FontSize < 20)
                labelGotDiscount.Visibility = Visibility.Hidden;
        }

        private void ShowErrorMessage(string errorMessage, int fontSize)
        {
            escapes = 0;

            //labelGotError.Content = "";
            //labelGotError.UpdateLayout();
            labelGotError.Visibility = Visibility.Visible;
            labelGotError.Content = errorMessage;
            labelGotError.FontSize = fontSize;

            System.Windows.Controls.Panel.SetZIndex(canvasBackground, 10);
            canvasBackground.Opacity = 0.3;
            ////  Анимируем labelGotError
            //DoubleAnimation doubleAnimationLabelGotError = new DoubleAnimation(0, fontSize, new Duration(TimeSpan.FromSeconds(0.5)))
            //{
            //    AutoReverse = false,
            //    //BeginTime = new TimeSpan(500)
            //};
            ////doubleAnimationLabelGotError.Completed += DoubleAnimationLabelGotError_Completed;
            //labelGotError.BeginAnimation(FontSizeProperty, doubleAnimationLabelGotError);

            //BlurEffect blurEffect = new BlurEffect() { Radius = 3 };
            //dockPanelMain.Effect = blurEffect;
        }

        private void DoubleAnimationLabelGotError_Completed(object sender, EventArgs e)
        {
            dockPanelMain.Effect = blurEffect;
        }

        private void HideErrorMessage()
        {
            labelGotError.Visibility = Visibility.Hidden;
            labelGotError.Content = "";

            //Panel.SetZIndex(canvasBackground, 10);
            canvasBackground.Opacity = 0;
            //labelGotError.UpdateLayout();
            ////  Анимируем labelGotError в нулевой размер шрифта
            //DoubleAnimation doubleAnimationLabelGotError = new DoubleAnimation(labelGotError.FontSize, 0, new Duration(TimeSpan.FromMilliseconds(100)))
            //{
            //    AutoReverse = false
            //};
            //labelGotError.FontSize = 1;
            ////doubleAnimationLabelGotError.Completed += DoubleAnimationLabelHideError_Completed;
            //labelGotError.BeginAnimation(FontSizeProperty, doubleAnimationLabelGotError);
        }

        private void DoubleAnimationLabelHideError_Completed(object sender, EventArgs e)
        {
            labelGotError.Visibility = Visibility.Hidden;
            labelGotError.Content = "";
        }

        private void ActiveCheck_ChangeSumChanged(object sender, ChangeSumChangedEventArgs e)
        {
            if (e.NewChangeSum != 0)
            {
                doubleAnimationLabelChangeFontSize.Completed += DoubleAnimationLabelChangeFontSize_Completed;
                labelChange.BeginAnimation(FontSizeProperty, doubleAnimationLabelChangeFontSize);
            }
        }

        private void ActiveCheck_LoyaltySumChanged(object sender, LoyaltySumChangedEventArgs e)
        {
            if (e.NewLoyaltySum != 0)
            {
                //DoubleAnimation doubleAnimationLabelLoyaltyFontSize = new DoubleAnimation(32, 50, new Duration(TimeSpan.FromSeconds(0.25)))
                //{
                //    AutoReverse = true
                //};
                doubleAnimationLabelLoyaltyFontSize.Completed += DoubleAnimationLabelLoyaltyFontSize_Completed;
                labelLoyalty.BeginAnimation(FontSizeProperty, doubleAnimationLabelLoyaltyFontSize);
            }
        }

        private void DoubleAnimationLabelLoyaltyFontSize_Completed(object sender, EventArgs e)
        {
            //  Анимируем labelChange
            //DoubleAnimation doubleAnimationLabelLoyaltyFontSize = new DoubleAnimation(32, 36, new Duration(TimeSpan.FromSeconds(0.35)))
            //{
            //    AutoReverse = true,
            //    RepeatBehavior = RepeatBehavior.Forever
            //};
            labelLoyalty.BeginAnimation(FontSizeProperty, doubleAnimationLabelLoyaltyFontSize);
        }

        private void DoubleAnimationLabelChangeFontSize_Completed(object sender, EventArgs e)
        {
            doubleAnimationLabelChangeFontSize.Completed -= DoubleAnimationLabelChangeFontSize_Completed;
            //  Анимируем labelChange
            labelChange.BeginAnimation(FontSizeProperty, doubleAnimationLabelChangeFontSize);
        }

        private void ActiveCheck_CardSumChanged(object sender, CardSumChangedEventArgs e)
        {
            if (e.NewCardSum != 0)
            {
                //  Анимируем labelAcceptedByCard
                DoubleAnimation doubleAnimationLabelAcceptedByCardFontSize = new DoubleAnimation(32, 50, new Duration(TimeSpan.FromSeconds(0.25)))
                {
                    AutoReverse = true
                };
                labelAcceptedByCard.BeginAnimation(FontSizeProperty, doubleAnimationLabelAcceptedByCardFontSize);
            }
        }

        private void ActiveCheck_CashSumChanged(object sender, CashSumChangedEventArgs e)
        {
            if (e.NewCashSum != 0)
            {
                //  Анимируем labelAcceptedByCash
                DoubleAnimation doubleAnimationLabelAcceptedInCashFontSize = new DoubleAnimation(32, 50, new Duration(TimeSpan.FromSeconds(0.25)))
                {
                    AutoReverse = true
                };
                labelAcceptedInCash.BeginAnimation(FontSizeProperty, doubleAnimationLabelAcceptedInCashFontSize);
            }
        }

        private void ActiveCheck_TopaySumChanged(object sender, TopaySumChangedEventArgs e)
        {
            if (e.NewTopaySum != 0)
            {
                //  Анимируем labelTopay
                DoubleAnimation doubleAnimationLabelTopaySumFontSize = new DoubleAnimation(42, 60, new Duration(TimeSpan.FromSeconds(0.25)))
                {
                    AutoReverse = true,
                    //RepeatBehavior = RepeatBehavior.Forever
                };
                labelTopaySum.BeginAnimation(FontSizeProperty, doubleAnimationLabelTopaySumFontSize);
            }
        }

        private void ActiveCheck_DiscountSumChanged(object sender, DiscountSumChangedEventArgs e)
        {
            if (e.NewDiscountSum != 0)
            {
                panelDiscount.Visibility = Visibility.Visible;
                labelGotDiscount.Visibility = Visibility.Visible;

                //  Анимируем labelGotDiscount
                doubleAnimationLabelGotDiscount.Completed += DoubleAnimationLabelGotDiscount_Completed;
                labelGotDiscount.BeginAnimation(FontSizeProperty, doubleAnimationLabelGotDiscount);

                //  Анимируем labelDiscountSumHeader и labelDiscountSum
                labelDiscountSum.BeginAnimation(FontSizeProperty, doubleAnimationLabelDiscountSumFontSize);
            }
            else
            {
                panelDiscount.Visibility = Visibility.Hidden;
            }
        }

        private void DoubleAnimationCanvasBackground_Completed(object sender, EventArgs e)
        {
            DoubleAnimation doubleAnimationCanvasBackgroundCompleted = new DoubleAnimation(0.3, new Duration(TimeSpan.FromSeconds(0.10)))
            {
                AutoReverse = true
            };
            doubleAnimationCanvasBackgroundCompleted.Completed += DoubleAnimationCanvasBackgroundCompleted_Completed;
            canvasBackground.BeginAnimation(OpacityProperty, doubleAnimationCanvasBackgroundCompleted);
        }

        private void DoubleAnimationCanvasBackgroundCompleted_Completed(object sender, EventArgs e)
        {
            System.Windows.Controls.Panel.SetZIndex(canvasBackground, 0);
        }

        private void ActiveCheck_CouponSumChanged(object sender, CouponSumChangedEventArgs e)
        {
            //  Анимируем labelCouponSumHeader и labelCouponSum
            DoubleAnimation doubleAnimationLabelCouponSumFontSize = new DoubleAnimation(42, 60, new Duration(TimeSpan.FromSeconds(0.25)))
            {
                AutoReverse = true
            };
            DoubleAnimation doubleAnimationLabelCuponSumHeaderFontSize = new DoubleAnimation(16, 24, new Duration(TimeSpan.FromSeconds(0.25)))
            {
                AutoReverse = true
            };

            //ElasticEase elasticEasingFunction = new ElasticEase();
            //elasticEasingFunction.EasingMode = EasingMode.EaseOut;
            //elasticEasingFunction.Springiness = 3;
            //elasticEasingFunction.Oscillations = 1;
            //doubleAnimationLabelCouponSumFontSize.EasingFunction = elasticEasingFunction;

            labelCouponSumHeader.BeginAnimation(FontSizeProperty, doubleAnimationLabelCuponSumHeaderFontSize);
            labelCouponSum.BeginAnimation(FontSizeProperty, doubleAnimationLabelCouponSumFontSize);
        }

        private void ActiveCheck_TotalSumChanged(object sender, TotalSumChangedEventArgs e)
        {
            if (e.NewTotalSum != 0)
            {
                //  Анимируем labelTotalHeader и labelTotal
                labelTotal.BeginAnimation(FontSizeProperty, doubleAnimationLabelTotalFontSize);

                //DoubleAnimation doubleAnimationLabelTotalHeaderFontSize = new DoubleAnimation(20, 30, new Duration(TimeSpan.FromSeconds(0.25)))
                //{
                //    AutoReverse = true
                //};
                //labelTotalHeader.BeginAnimation(FontSizeProperty, doubleAnimationLabelTotalHeaderFontSize);
            }
        }

        private void ActivePosition_RegimeChanged(object sender, RegimeChangedEventArgs e)
        {
            if (e.LastRegime == ActivePositionModes.codeReady && e.NewRegime == ActivePositionModes.codeInput)
            {
                //  Из режима СВОБОДНА программа переходит в режим ПРОДАЖА
                labelReadyMode.Style = Resources["InactivePanelLabel"] as Style;
                if (!activeCheck.returnMode)
                    labelSaleMode.Style = Resources["ActivePanelLabel"] as Style;
            }
            else if (e.LastRegime == ActivePositionModes.codeDone && e.NewRegime == ActivePositionModes.codeReady)
            {
                if (activeCheck.finished)
                {
                    //  Из режима ПРОДАЖА программа переходит в режим СВОБОДНА
                    //labelSaleMode.Style = Resources["InactivePanelLabel"] as Style;
                    //labelReadyMode.Style = Resources["ActivePanelLabel"] as Style;
                }
            }
            else if (e.LastRegime == ActivePositionModes.codeInput && e.NewRegime == ActivePositionModes.codeDone)
            {
                //  Из режима ПРОДАЖА программа переходит в режим СВОБОДНА
                //labelSaleMode.Style = Resources["InactivePanelLabel"] as Style;
                //labelReadyMode.Style = Resources["ActivePanelLabel"] as Style;
            }
            else if (e.LastRegime == ActivePositionModes.codeInput && e.NewRegime == ActivePositionModes.codeReady)
            {
                //  Из режима ПРОДАЖА программа переходит в режим СВОБОДНА
                //labelSaleMode.Style = Resources["InactivePanelLabel"] as Style;
                //labelReadyMode.Style = Resources["ActivePanelLabel"] as Style;
            }
            else
                System.Windows.MessageBox.Show(e.LastRegime.ToString() + " -> " + e.NewRegime.ToString());
        }

        private void PlayKeyboardTone()
        {
            //if (Properties.Settings.Default.UseSystemTone)
            //{
            //    tone.InterToneWait = 5;

            //    tone.Tone1Pitch = 300;
            //    tone.Tone1Duration = 15;
            //    tone.Tone1Volume = 100;

            //    tone.Tone2Pitch = 150;
            //    tone.Tone2Duration = 15;
            //    tone.Tone2Volume = 100;

            //    tone.Sound(1, 15);
            //}
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != Key.Escape && e.Key != Key.E)
                if (labelGotError.Visibility == Visibility.Visible)
                {
                    dockPanelMain.Effect = null;
                    HideErrorMessage();
                }

#region Цифры
            //if ((e.Key == Key.D0 || e.Key == Key.D1 || e.Key == Key.D2 || e.Key == Key.D3 || e.Key == Key.D4 || e.Key == Key.D5 || e.Key == Key.D6 || e.Key == Key.D7 || e.Key == Key.D8 || e.Key == Key.D9) && !activeCheck.couponMode)
            //if ((e.Key == Key.D0 || e.Key == Key.D1 || e.Key == Key.D2 || e.Key == Key.D3 || e.Key == Key.D4 || e.Key == Key.D5 || e.Key == Key.D6 || e.Key == Key.D7 || e.Key == Key.D8 || e.Key == Key.D9 || e.Key == Key.A || e.Key == Key.B || e.Key == Key.C || e.Key == Key.D || e.Key == Key.E || e.Key == Key.F || e.Key == Key.G || e.Key == Key.H || e.Key == Key.I || e.Key == Key.J || e.Key == Key.K || e.Key == Key.L || e.Key == Key.M || e.Key == Key.N || e.Key == Key.O || e.Key == Key.P || e.Key == Key.Q || e.Key == Key.R || e.Key == Key.S || e.Key == Key.T || e.Key == Key.U || e.Key == Key.V || e.Key == Key.W || e.Key == Key.X || e.Key == Key.Y || e.Key == Key.Z
            if ((e.Key == Key.D0 || e.Key == Key.D1 || e.Key == Key.D2 || e.Key == Key.D3 || e.Key == Key.D4 || e.Key == Key.D5 || e.Key == Key.D6 || e.Key == Key.D7 || e.Key == Key.D8 || e.Key == Key.D9
                || e.Key == Key.End || e.Key == Key.Down || e.Key == Key.Next || e.Key == Key.Left || e.Key == Key.Clear || e.Key == Key.Right || e.Key == Key.Home || e.Key == Key.Up || e.Key == Key.PageUp || e.Key == Key.Insert) && !activeCheck.couponMode)
            {
                //PlayKeyboardTone();

                if (activeCheck.totalMode && !activeCheck.preFinished && !activeCheck.paymentMode)
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);

                    e.Handled = true;
                    return;
                }

                if (!activeCheck.finished && activeCheck.preFinished)
                {
                    FinishCheck();

                    if (!fiscal.CompareDateTime())
                    {
                        fiscal.Close();
                        System.Windows.MessageBox.Show("Не сделан суточный отчет!");
                        this.Owner.Show();
                        this.Owner.Activate();
                        this.Owner.Opacity = 1;
                        Close();
                    }
                }
                if (activePosition.Quantity == 0)
                {
                    panelQuantity.Visibility = Visibility.Collapsed;
                    labelQuantity.Content = 0;
                    panelBarcode.Visibility = Visibility.Collapsed;
                    labelBarcode.Content = "";
                    panelDescription.Visibility = Visibility.Collapsed;
                    labelDescriptionWin2.Content = "";
                }
                //  Режим готовности к добавлению позиций
                if (activePosition.Regime == ActivePositionModes.codeReady)
                {
                    if (activeCheck.returnMode && activeCheck.specification.Count > 0 && !activeCheck.preFinished && !activeCheck.finished)
                    {
                        ShowErrorCanvas();
                        ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);

                        e.Handled = true;
                        return;
                    }
                    //activeCheck.finished = false;
                    //  Если это не первая позиция документа, очистим Info от содержимого, оставшегося после ввода предыдущей позиции
                    if (activePosition.Info.Length > 0)
                        activePosition.Info = "";
                    //  Переключаем в режим "позиция добавляется"
                    activePosition.Regime = ActivePositionModes.codeInput;
                }

                //  Режим "позиция добавляется"
                if (activePosition.Regime == ActivePositionModes.codeInput)
                {
                    //  Добавляем символ в Info и Barcode
                    if (e.Key == Key.D0)
                    {
                        activePosition.Info += e.Key.ToString().Substring(1);
                        activePosition.Barcode = activePosition.Info;
                        //if (activePosition.Barcode == "4780067681270")
                        //{
                        //    endTime = DateTime.Now;
                        //    MessageBox.Show((endTime - startTime).ToString());
                        //    MessageBox.Show((endTime - startTime).ToString());
                        //}
                        return;
                    }
                    else if (e.Key == Key.D1)
                    {
                        activePosition.Info += e.Key.ToString().Substring(1);
                        activePosition.Barcode = activePosition.Info;
                        return;
                    }
                    else if (e.Key == Key.D2)
                    {
                        activePosition.Info += e.Key.ToString().Substring(1);
                        activePosition.Barcode = activePosition.Info;
                        return;
                    }
                    else if (e.Key == Key.D3)
                    {
                        activePosition.Info += e.Key.ToString().Substring(1);
                        activePosition.Barcode = activePosition.Info;
                        return;
                    }
                    else if (e.Key == Key.D4)
                    {
                        activePosition.Info += e.Key.ToString().Substring(1);
                        activePosition.Barcode = activePosition.Info;
                        //startTime = DateTime.Now;
                        return;
                    }
                    else if (e.Key == Key.D5)
                    {
                        activePosition.Info += e.Key.ToString().Substring(1);
                        activePosition.Barcode = activePosition.Info;
                        return;
                    }
                    else if (e.Key == Key.D6)
                    {
                        activePosition.Info += e.Key.ToString().Substring(1);
                        activePosition.Barcode = activePosition.Info;
                        return;
                    }
                    else if (e.Key == Key.D7)
                    {
                        activePosition.Info += e.Key.ToString().Substring(1);
                        activePosition.Barcode = activePosition.Info;
                        return;
                    }
                    else if (e.Key == Key.D8)
                    {
                        activePosition.Info += e.Key.ToString().Substring(1);
                        activePosition.Barcode = activePosition.Info;
                        return;
                    }
                    else if (e.Key == Key.D9)
                    {
                        activePosition.Info += e.Key.ToString().Substring(1);
                        activePosition.Barcode = activePosition.Info;
                        return;
                    }
                    else if (e.Key == Key.End)
                    {
                        activePosition.Info += "1";
                        activePosition.Barcode = activePosition.Info;
                        return;
                    }
                    else if (e.Key == Key.Down)
                    {
                        activePosition.Info += "2";
                        activePosition.Barcode = activePosition.Info;
                        return;
                    }
                    else if (e.Key == Key.Next)
                    {
                        activePosition.Info += "3";
                        activePosition.Barcode = activePosition.Info;
                        return;
                    }
                    else if (e.Key == Key.Left)
                    {
                        activePosition.Info += "4";
                        activePosition.Barcode = activePosition.Info;
                        return;
                    }
                    else if (e.Key == Key.Clear)
                    {
                        activePosition.Info += "5";
                        activePosition.Barcode = activePosition.Info;
                        return;
                    }
                    else if (e.Key == Key.Right)
                    {
                        activePosition.Info += "6";
                        activePosition.Barcode = activePosition.Info;
                        return;
                    }
                    else if (e.Key == Key.Home)
                    {
                        activePosition.Info += "7";
                        activePosition.Barcode = activePosition.Info;
                        return;
                    }
                    else if (e.Key == Key.Up)
                    {
                        activePosition.Info += "8";
                        activePosition.Barcode = activePosition.Info;
                        return;
                    }
                    else if (e.Key == Key.PageUp)
                    {
                        activePosition.Info += "9";
                        activePosition.Barcode = activePosition.Info;
                        return;
                    }
                    else if (e.Key == Key.Insert)
                    {
                        activePosition.Info += "0";
                        activePosition.Barcode = activePosition.Info;
                        return;
                    }
                    else
                    {
                        activePosition.Info += e.Key.ToString().Substring(0);
                        activePosition.Barcode = activePosition.Info;
                        return;
                    }
                }
            }
#endregion Цифры

#region Escape
            else if (e.Key == Key.Escape || e.Key == Key.E)
            {
                keysAfterAuthorization++;   //  20201215
                if (!activeCheck.finished && activeCheck.preFinished)
                {
                    //PlayKeyboardTone();

                    FinishCheck();
                }
                if (!activeCheck.annulationMode)
                {
                    //PlayKeyboardTone();

                    //  В режиме "позиция добавляется" Escape очищает Info и Barcode
                    if (activePosition.Regime == ActivePositionModes.codeInput && !activeCheck.couponMode)
                    {
                        activePosition.Info = "";
                        activePosition.Barcode = "";

                        activePosition.Regime = ActivePositionModes.codeReady;
                    }
                    else if (activePosition.Regime == ActivePositionModes.codeReady && !activeCheck.paymentMode && Math.Abs(activePosition.Quantity) > 1)
                    {
                        //if (activePosition.Quantity < -1)
                        //    activePosition.Quantity = -1;
                        //else if (activePosition.Quantity > 1)
                        //    activePosition.Quantity = 1;
                        //activePosition.Barcode = "";
                        //activePosition.Info = "";
                        //activePosition.InputType = ActivePositionTypes.barcodeType;
                        //activePosition.Regime = ActivePositionModes.codeReady;
                        //panelQuantity.Visibility = Visibility.Visible;
                        //labelQuantity.Content = activePosition.Quantity;

                        activePosition.Clear();
                        activePosition.Info = "";

                        panelQuantity.Visibility = Visibility.Collapsed;
                        labelQuantity.Content = 0;
                        panelBarcode.Visibility = Visibility.Collapsed;
                        labelBarcode.Content = "";
                        panelDescription.Visibility = Visibility.Collapsed;
                        labelDescriptionWin2.Content = "";
                    }
                    else if (activePosition.Regime == ActivePositionModes.codeReady && activeCheck.paymentMode)
                    {
                        activePosition.Info = "";
                        activePosition.Barcode = "";
                    }
                    else if (activePosition.Regime == ActivePositionModes.codeReady && activeCheck.finished && !activeCheck.couponMode)
                    {
                        //Window1 window1 = new Window1();
                        //window1.Show();
                        //Close();
                        if (labelGotError.Visibility == Visibility.Visible)
                        {
                            HideErrorMessage();
                            escapes++;
                        }
                        else
                            escapes += 2;
                        //if (escapes > 1)
                        if (escapes > 1)
                        //  если на кассе СБРОС вызывается тоже по Escape, то закомментированной строкой заместим верхнее условие   20201215
                        //if (escapes > 1 && keysAfterAuthorization > 1)
                        {
                            UnhookWindowsHookEx(_hookID);
                            timer.Stop();

                            //long memoryUsed = currentProcess.PrivateMemorySize64;
                            activeCheck.specification.Clear();
                            //memoryUsed = currentProcess.PrivateMemorySize64;

                            Owner.Show();
                            //  сделаем окно-владелец (окно меню) непрозрачным (видимым)
                            DoubleAnimation doubleAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
                            //  стартуем анимацию непрозрачности
                            Owner.BeginAnimation(OpacityProperty, doubleAnimation);

                            Close();
                        }
                    }
                    else if (activeCheck.couponMode)
                    {
                        if (textBoxCouponCode.Text.Length == 0)
                        {
                            labelCouponMode.Style = Resources["InactivePanelLabel"] as Style;

                            //labelInvitation.Visibility = System.Windows.Visibility.Collapsed;

                            activeCheck.CouponSum = 0;

                            dockPanelMain.Effect = null;
                            borderCoupon.Visibility = Visibility.Hidden;

                            activeCheck.couponMode = false;
                        }
                        else
                            textBoxCouponCode.Text = "";
                    }
                }
            }
#endregion Escape

#region временно для управления панелью
            //  Return
            //else if (e.Key == Key.F8 || e.Key == Key.Oem1)  //  кнопка СКИДКА
            else if (e.Key == Key.Snapshot)  //  кнопка ВОЗВРАТ
            {
#region ReturnMode
                if (!Vars.cashierStatus)
                    return;

                //if (activeCheck.specification.Count > 0 && !activeCheck.finished && !activeCheck.preFinished)
                //{
                //    ShowErrorCanvas();
                //    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                //    return;
                //}

                BlurEffect blurEffect = new BlurEffect() { Radius = 6 };
                dockPanelMain.Effect = blurEffect;

                //  если окно авторизации еще не создано (или было уничтожено)
                if (windowAuthorization == null)
                    //  создадим его
                    windowAuthorization = new WindowAutorization();
                windowAuthorization.Mode = "super";
                windowAuthorization.labelContent.Content = "!!! ВОЗВРАТ !!!";
                keysAfterAuthorization = 0;

                //  покажем окно авторизации в модальном режиме
                windowAuthorization.ShowDialog();
                bool authorizationResult = windowAuthorization.Result;
                //  после завершения диалога закроем окно авторизации
                windowAuthorization.Close();
                dockPanelMain.Effect = null;
                //  и обнулим объект
                windowAuthorization = null;
                if (!authorizationResult)
                {
                    return;
                }
                if (activeCheck.returnMode)
                {
                    if (activeCheck.finished)
                    {
                        activeCheck.returnMode = false;
                        labelReturnMode.Style = Resources["InactivePanelLabel"] as Style;
                        canvasMain.Effect = null;

                        labelReadyMode.Style = Resources["ActivePanelLabel"] as Style;
                        activeCheck.saleMode = false;
                    }
                }
                else
                {
                    if (!activeCheck.finished && activeCheck.preFinished)
                        FinishCheck();
                    if (!activeCheck.saleMode && activeCheck.finished)
                    {
                        //activeCheck.finished = false;
                        labelReadyMode.Style = Resources["InactivePanelLabel"] as Style;
                        activeCheck.saleMode = false;
                        labelSaleMode.Style = Resources["InactivePanelLabel"] as Style;
                        activeCheck.returnMode = true;
                        labelReturnMode.Style = Resources["ActivePanelLabel"] as Style;
                        //canvasMain.Effect = new InvertColorEffect();
                        Effect monochromeEffect = new MonochromeEffect()
                        {
                            FilterColor = Color.FromRgb(255, 255, 0)
                        };
                        canvasMain.Effect = monochromeEffect;
                        //DoubleAnimation doubleAnimationCanvasMainParameter = new DoubleAnimation(80, 95, new Duration(TimeSpan.FromSeconds(1.0)))
                        //{
                        //    AutoReverse = true,
                        //    RepeatBehavior = RepeatBehavior.Forever
                        //};
                        //monochromeEffect.BeginAnimation(Transition_Ripple_modifiedEffect.ProgressProperty, doubleAnimationCanvasMainParameter);
                    }
                }
                //activeCheck.returnMode = !activeCheck.returnMode;
#endregion ReturnMode
            }

            //  Cancel
            else if (e.Key == Key.OemPeriod)
            {
#region CancelMode
                labelCancelMode.Foreground = animatedBrushCancel;
                if (activeCheck.cancelMode)
                {
                    labelCancelMode.Style = Resources["InactivePanelLabel"] as Style;
                    animatedBrushCancel.BeginAnimation(SolidColorBrush.ColorProperty, null);
                    labelCancelMode.Foreground = new SolidColorBrush(Color.FromRgb(0x25, 0x25, 0x25));
                }
                else
                {
                    labelCancelMode.Style = Resources["ActivePanelLabel"] as Style;
                    ColorAnimation colorAnimationlabelAutonomode = new ColorAnimation(System.Windows.Media.Colors.White, new Duration(TimeSpan.FromSeconds(0.25)))
                    {
                        AutoReverse = true,
                        RepeatBehavior = RepeatBehavior.Forever
                    };
                    animatedBrushCancel.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimationlabelAutonomode, HandoffBehavior.SnapshotAndReplace);
                }
                activeCheck.cancelMode = !activeCheck.cancelMode;
#endregion CancelMode
            }

            //  Coupon
            else if (e.Key == Key.F14 || e.Key == Key.V)
            {
#region CouponMode on/off
                if (activeCheck.totalMode || activeCheck.paymentMode)
                    return;
                if (activeCheck.couponMode)
                {
                    labelCouponMode.Style = Resources["InactivePanelLabel"] as Style;

                    activeCheck.CouponSum = 0;

                    dockPanelMain.Effect = null;
                    borderCoupon.Visibility = Visibility.Hidden;
                }
                else
                {
                    labelCouponMode.Style = Resources["ActivePanelLabel"] as Style;

                    BlurEffect blurEffect = new BlurEffect() { Radius = 8 };
                    dockPanelMain.Effect = blurEffect;

                    borderCoupon.Visibility = Visibility.Visible;
                    textBoxCouponCode.Text = "";
                    textBoxCouponCode.Focus();
                }
                activeCheck.couponMode = !activeCheck.couponMode;
#endregion CouponMode on/off
            }

            //  Voucher
            //else if (e.Key == Key.F9)
            //{
            //    #region VoucherMode on/off
            //    if (activeCheck.voucherMode)
            //    {
            //        labelVoucherMode.Style = Resources["InactivePanelLabel"] as Style;
            //    }
            //    else
            //    {
            //        labelVoucherMode.Style = Resources["ActivePanelLabel"] as Style;
            //    }
            //    activeCheck.voucherMode = !activeCheck.voucherMode;
            //    #endregion VoucherMode on/off
            //}

            //  Autonomode
            //else if (e.Key == Key.F4)
            //{
            //    #region Autonomode on/off
            //    SolidColorBrush animatedBrush = new SolidColorBrush()
            //    {
            //        Color = (Color)ColorConverter.ConvertFromString("#252525")
            //    };
            //    labelAutonoMode.Foreground = animatedBrush;
            //    if (activeCheck.autonoMode)
            //    {
            //        if (activeCheck.returnMode)
            //        {
            //            Effect monochromeEffect = new MonochromeEffect()
            //            {
            //                FilterColor = Color.FromRgb(255, 255, 0)
            //            };
            //            canvasMain.Effect = monochromeEffect;
            //        }
            //        else
            //            canvasMain.Effect = null;
            //        labelAutonoMode.Style = Resources["InactivePanelLabel"] as Style;
            //        animatedBrush.BeginAnimation(SolidColorBrush.ColorProperty, null);
            //        labelAutonoMode.Foreground = new SolidColorBrush(Color.FromRgb(0x25, 0x25, 0x25));
            //    }
            //    else
            //    {
            //        labelAutonoMode.Style = Resources["ActiveAutonomodePanelLabel"] as Style;
            //        ColorAnimation colorAnimationlabelAutonomode = new ColorAnimation(System.Windows.Media.Colors.Orange, new Duration(TimeSpan.FromSeconds(0.25)))
            //        {
            //            AutoReverse = true,
            //            RepeatBehavior = RepeatBehavior.Forever
            //        };
            //        animatedBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimationlabelAutonomode, HandoffBehavior.SnapshotAndReplace);

            //        //canvasMain.Effect = new InvertColorEffect();
            //        //Effect sine1Effect = new Sine1Effect();
            //        //canvasMain.Effect = sine1Effect;
            //        //DoubleAnimation doubleAnimationCanvasMainParameter = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(1.0)))
            //        //{
            //        //    AutoReverse = true,
            //        //    RepeatBehavior = RepeatBehavior.Forever
            //        //};
            //        //sine1Effect.BeginAnimation(Sine1Effect.ParameterProperty, doubleAnimationCanvasMainParameter);
            //        if (activeCheck.returnMode)
            //        {
            //            Effect transition_Ripple_modified_with_MonochromeEffect = new Transition_Ripple_modified_with_MonochromeEffect()
            //            {
            //                FilterColor = Color.FromRgb(255, 255, 0)
            //            };
            //            canvasMain.Effect = transition_Ripple_modified_with_MonochromeEffect;
            //            DoubleAnimation doubleAnimationCanvasMainParameter = new DoubleAnimation(80, 95, new Duration(TimeSpan.FromSeconds(1.0)))
            //            {
            //                AutoReverse = true,
            //                RepeatBehavior = RepeatBehavior.Forever
            //            };
            //            transition_Ripple_modified_with_MonochromeEffect.BeginAnimation(Transition_Ripple_modified_with_MonochromeEffect.ProgressProperty, doubleAnimationCanvasMainParameter);
            //        }
            //        else
            //        {
            //            Effect transition_Ripple_modifiedEffect = new Transition_Ripple_modifiedEffect();
            //            canvasMain.Effect = transition_Ripple_modifiedEffect;
            //            DoubleAnimation doubleAnimationCanvasMainParameter = new DoubleAnimation(80, 95, new Duration(TimeSpan.FromSeconds(1.0)))
            //            {
            //                AutoReverse = true,
            //                RepeatBehavior = RepeatBehavior.Forever
            //            };
            //            transition_Ripple_modifiedEffect.BeginAnimation(Transition_Ripple_modifiedEffect.ProgressProperty, doubleAnimationCanvasMainParameter);
            //        }
            //    }
            //    activeCheck.autonoMode = !activeCheck.autonoMode;
            //    #endregion Autonomode on/off
            //}
#endregion временно для управления панелью

            //  Quantity
            else if (e.Key == Key.OemComma)
            {
#region Q(uantity)
                if (activeCheck.preFinished)
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                    return;
                }
                if (activePosition.InputType == ActivePositionTypes.barcodeType)
                {
#region ввод количества завершен
                    //PlayKeyboardTone();

                    if (activePosition.Barcode.Length > 0)
                    {
                        try
                        {
                            activePosition.Quantity = Convert.ToInt32(activePosition.Barcode);
                            activePosition.Barcode = "";
                            activePosition.Info = "";
                            activePosition.InputType = ActivePositionTypes.barcodeType;
                            activePosition.Regime = ActivePositionModes.codeReady;
                            panelQuantity.Visibility = Visibility.Visible;
                            labelQuantity.Content = activePosition.Quantity;
                        }
                        catch
                        {
                            ShowErrorCanvas();
                            ShowErrorMessage("НЕВЕРНЫЙ ФОРМАТ", 70);
                        }
                    }
#endregion ввод количества завершен
                }
#endregion Q(uantity)
            }

            else if (e.Key == Key.Return)
            {
                keysAfterAuthorization++;
#region Enter
                if (activeCheck.preFinished)
                {
                    if (keysAfterAuthorization > 1) //  20201215
                    {
                        ShowErrorCanvas();
                        ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                    }
                    return;
                }
                if (activeCheck.annulationMode)
                {
                }
                else
                {
#region Завершен ввод кода товара
                    if (activePosition.Regime == ActivePositionModes.codeInput && !activeCheck.paymentMode && !activeCheck.couponMode)
                    {
#region ввод кода завершен
                        if (activePosition.InputType == ActivePositionTypes.barcodeType)
                        {
                            //PlayKeyboardTone();

#region ввод баркода завершен
                            //  Если Barcode заполнен
                            if (activePosition.Barcode.Length > 0)
                            {
                                //  И позиция с введенным кодом найдена в таблице GOODS.DBF
                                if (GoodsDBF.getRecordByBarcode(activePosition.Barcode, ref activePosition, ref connection))
                                {
                                    //eTime = DateTime.Now;
                                    //MessageBox.Show((eTime - startTime).ToString());
                                    //  Пока количество по умолчанию равно 1
                                    if (activePosition.Quantity == 0)
                                    {
                                        activePosition.Quantity = 1;
                                    }

                                    //  При отмене нужно проверить количество
                                    if (activeCheck.cancelMode)
                                    {
                                        //  Если количество отмены превышено, помигаем ошибкой, и надо бы добавить уведомление
                                        if (!activeCheck.QualifyCancel(activePosition))
                                        {
                                            ShowErrorCanvas();
                                            ShowErrorMessage("ПРЕВЫШЕНИЕ КОЛИЧЕСТВА", 50);
                                            activePosition.Clear();
                                            return;
                                        }
                                        activePosition.Quantity *= -1;
                                    }

                                    //  Рассчитаем сумму позиции Sum и увеличим на получившийся результат общую сумму по всем позициям TotalSum
                                    activePosition.Calculate(activeCheck);

                                    panelBarcode.Visibility = Visibility.Visible;
                                    labelBarcode.Content = activePosition.Barcode;
                                    panelDescription.Visibility = Visibility.Visible;
                                    labelDescriptionWin2.Content = activePosition.Description;

                                    //  Добавляем в gridReceipt новую строку
                                    gridReceipt.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                                    //  В первую колонку добавленной строки помещаем значение Index
                                    System.Windows.Controls.Label labelIndex = new System.Windows.Controls.Label
                                    {
                                        Content = activeCheck.specification.Count + 1,
                                        Style = Resources["GridLabelCenteredText"] as Style,
                                        Margin = new Thickness(0, 1, 0, 1)
                                    };
                                    //  Если количество данной позиции отрицательное, то это отмена - выделим стилем для отмены
                                    if (activePosition.Quantity < 0)
                                        labelIndex.Style = Resources["GridLabelCenteredTextCancelled"] as Style;
                                    else if (activePosition.Price > 999999)
                                        labelIndex.Style = Resources["GridLabelCenteredTextHuge"] as Style;
                                    Grid.SetRow(labelIndex, gridReceipt.RowDefinitions.Count - 1);
                                    Grid.SetColumn(labelIndex, 0);
                                    gridReceipt.Children.Add(labelIndex);

                                    //  Во вторую колонку добавленной строки помещаем значение Info
                                    System.Windows.Controls.Label labelDescription = new System.Windows.Controls.Label
                                    {
                                        Content = activePosition.Description,
                                        Style = Resources["GridLabelText"] as Style,
                                        Margin = new Thickness(0, 1, 0, 1)
                                    };
                                    //  Если количество данной позиции отрицательное, то это отмена - выделим стилем для отмены
                                    if (activePosition.Quantity < 0)
                                        labelDescription.Style = Resources["GridLabelTextCancelled"] as Style;
                                    else if (activePosition.Price > 999999)
                                        labelDescription.Style = Resources["GridLabelTextHuge"] as Style;
                                    Grid.SetRow(labelDescription, gridReceipt.RowDefinitions.Count - 1);
                                    Grid.SetColumn(labelDescription, 1);
                                    gridReceipt.Children.Add(labelDescription);

                                    //  Теперь, когда Info помещен в gridReceipt, можем сформировать подробное описание позиции, предварив его баркодом и дополнив стоимостью позиции Price и суммой по позиции
                                    //activePosition.Info = activePosition.Barcode + "   " + activePosition.Info + "   " + activePosition.Price.ToString() + " * " + activePosition.Quantity.ToString() + " = " + activePosition.Sum.ToString();
                                    activePosition.Info = activePosition.Price.ToString() + " * " + activePosition.Quantity.ToString() + " = " + activePosition.Sum.ToString();

                                    //  В третью колонку добавленной строки помещаем значение Quantity
                                    System.Windows.Controls.Label labelQuantity = new System.Windows.Controls.Label
                                    {
                                        Content = activePosition.Quantity,
                                        //Style = Resources["GridLabelNumber"] as Style,
                                        Style = Resources["GridLabelCenteredText"] as Style,
                                        Margin = new Thickness(0, 1, 0, 1)
                                    };
                                    //  Если количество больше 1, выделим стилем для количества
                                    if (Math.Abs(activePosition.Quantity) > 1)
                                        labelQuantity.Background = new SolidColorBrush(Color.FromRgb(0x31, 0x31, 0x31));
                                    //  Если количество отрицательное, то это отмена - выделим стилем для отмены
                                    if (activePosition.Quantity < 0)
                                    {
                                        if (Math.Abs(activePosition.Quantity) > 1)
                                        {
                                            labelQuantity.Background = new SolidColorBrush(Color.FromRgb(0x31, 0x31, 0x00));
                                            labelQuantity.Style = Resources["GridLabelCenteredTextCancelBigQty"] as Style;
                                        }
                                        else
                                        {
                                            //labelQuantity.Background = new SolidColorBrush(Color.FromRgb(0x21, 0x21, 0x00));
                                            labelQuantity.Style = Resources["GridLabelCenteredTextCancelQty"] as Style;
                                        }
                                        //labelQuantity.Foreground = System.Windows.Media.Brushes.Yellow;
                                    }
                                    else if (activePosition.Price > 999999)
                                        labelQuantity.Style = Resources["GridLabelCenteredTextHuge"] as Style;
                                    Grid.SetRow(labelQuantity, gridReceipt.RowDefinitions.Count - 1);
                                    Grid.SetColumn(labelQuantity, 2);
                                    gridReceipt.Children.Add(labelQuantity);

                                    //  В четвертую колонку добавленной строки помещаем значение Price
                                    System.Windows.Controls.Label labelPrice = new System.Windows.Controls.Label
                                    {
                                        Content = activePosition.Price,
                                        Style = Resources["GridLabelCenteredText"] as Style,
                                        Margin = new Thickness(0, 1, 0, 1)
                                    };
                                    //  Если количество данной позиции отрицательное, то это отмена - выделим стилем для отмены
                                    if (activePosition.Quantity < 0)
                                        labelPrice.Style = Resources["GridLabelCenteredTextCancelled"] as Style;
                                    else if (activePosition.Price > 999999)
                                        labelPrice.Style = Resources["GridLabelCenteredTextHuge"] as Style;
                                    Grid.SetRow(labelPrice, gridReceipt.RowDefinitions.Count - 1);
                                    Grid.SetColumn(labelPrice, 3);
                                    gridReceipt.Children.Add(labelPrice);

                                    //  В пятую колонку добавленной строки помещаем значение Sum
                                    System.Windows.Controls.Label labelSum = new System.Windows.Controls.Label
                                    {
                                        Content = Math.Round(activePosition.Sum, 2),
                                        Style = Resources["GridLabelCenteredText"] as Style,
                                        Margin = new Thickness(0, 1, 0, 1)
                                    };
                                    //  Если количество данной позиции отрицательное, то это отмена - выделим стилем для отмены
                                    if (activePosition.Quantity < 0)
                                        labelSum.Style = Resources["GridLabelCenteredTextCancelled"] as Style;
                                    else if (activePosition.Price > 999999)
                                        labelSum.Style = Resources["GridLabelCenteredTextHuge"] as Style;
                                    Grid.SetRow(labelSum, gridReceipt.RowDefinitions.Count - 1);
                                    Grid.SetColumn(labelSum, 4);
                                    gridReceipt.Children.Add(labelSum);

                                    //  В шестую колонку добавленной строки помещаем значение Discount
                                    if (activePosition.PercentDiscount > 0)
                                    {
                                        activePosition.Discount = Math.Round(activePosition.Sum * activePosition.PercentDiscount / 100, 2);
                                        activeCheck.DiscountSum += activePosition.Discount;
                                    }
                                    activeCheck.TopaySum = Math.Round(activeCheck.TotalSum - activeCheck.DiscountSum, 2);
                                    if (activeCheck.TopaySum != activeCheck.TotalSum)
                                        panelTopay.Visibility = Visibility.Visible;
                                    else
                                        panelTopay.Visibility = Visibility.Hidden;
                                    System.Windows.Controls.Label labelDiscount = new System.Windows.Controls.Label
                                    {
                                        Style = Resources["GridLabelCenteredText"] as Style,
                                        Margin = new Thickness(0, 1, 0, 1)
                                    };
                                    if (activePosition.Quantity < 0)
                                        labelDiscount.Style = Resources["GridLabelCenteredTextCancelled"] as Style;
                                    else if (activePosition.Price > 999999)
                                        labelDiscount.Style = Resources["GridLabelCenteredTextHuge"] as Style;
                                    if (activePosition.PercentDiscount > 0)
                                    {
                                        labelDiscount.Foreground = Brushes.Sienna;
                                        //labelDiscount.FontWeight = FontWeights.Bold;
                                        labelDiscount.Content = activePosition.Discount;
                                        labelDiscount.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                                        labelDiscount.VerticalContentAlignment = VerticalAlignment.Center;

                                        //ElasticEase elasticEase = new ElasticEase();
                                        //elasticEase.EasingMode = EasingMode.EaseInOut;
                                        //elasticEase.Springiness = 7;
                                        //elasticEase.Oscillations = 5;

                                        //SolidColorBrush animatedBrush = new SolidColorBrush();

                                        //ColorAnimation colorAnimationlabelDiscount = new ColorAnimation(System.Windows.Media.Colors.Sienna, new Duration(TimeSpan.FromSeconds(0.5)))
                                        //{
                                        //    AutoReverse = false,
                                        //    //EasingFunction = elasticEase
                                        //};
                                        ////colorAnimationlabelDiscount.EasingFunction = elasticEase;
                                        //labelDiscount.Background = animatedBrush;
                                        //animatedBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimationlabelDiscount);
                                    }

                                    Grid.SetRow(labelDiscount, gridReceipt.RowDefinitions.Count - 1);
                                    Grid.SetColumn(labelDiscount, 5);
                                    gridReceipt.Children.Add(labelDiscount);

                                    scrollViewerGrid.ScrollToEnd();

                                    activeCheck.AddToSpecification(activePosition);

                                    Display.ClearText();
                                    Display.DisplayText(Display.FormatTwoStrings(activePosition.Description, (activePosition.Sum - activePosition.Discount).ToString()));
                                    Display.DisplayTextAt(0, 1, Display.FormatTwoStrings("ИТОГ: ", activeCheck.TopaySum.ToString()));

                                    activePosition.Clear();
                                    activeCheck.finished = false;

                                    if (activeCheck.DiscountSum > 0)
                                    {
                                        //  Анимируем labelTopay
                                        DoubleAnimation doubleAnimationLabelTopaySumFontSize = new DoubleAnimation(42, 46, new Duration(TimeSpan.FromSeconds(0.35)))
                                        {
                                            AutoReverse = true,
                                            RepeatBehavior = RepeatBehavior.Forever
                                        };
                                        labelTopaySum.BeginAnimation(FontSizeProperty, doubleAnimationLabelTopaySumFontSize);
                                    }

                                    if (activeCheck.cancelMode)
                                    {
                                        activeCheck.cancelMode = false;
                                        labelCancelMode.Style = Resources["InactivePanelLabel"] as Style;
                                        animatedBrushCancel.BeginAnimation(SolidColorBrush.ColorProperty, null);
                                    }
                                }
                                //  Позиция не найдена в GOODS.DBF
                                else
                                {
                                    ShowErrorCanvas();
                                    ShowErrorMessage("ПОЗИЦИЯ НЕ НАЙДЕНА", 60);
                                }
                            }
                            //  Баркод пуст
                            else
                            {
                                ShowErrorCanvas();
                                //ShowErrorMessage("ВВЕДИТЕ КОД ТОВАРА", 60);
                            }
#endregion ввод баркода завершен
                        }
                        else
                        {
                            ShowErrorCanvas();
                            //ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                        }
#endregion ввод кода завершен
                    }
#endregion Завершен ввод кода товара
#region Завершен ввод кода карты лояльности
                    else if (activeCheck.couponMode)
                    {
                        //if (textBoxCouponCode.Text.Length != 15)
                        if (textBoxCouponCode.Text.Length == 0)
                        {
                            ShowErrorCanvas();
                            //ShowErrorMessage("ВВЕДИТЕ КОД КУПОНА", 60);
                        }
                        else
                        {
                            //  проверим купон и добавим его
                            if (activeCheck.VerifyLoyalty(textBoxCouponCode.Text, out string result))
                            {
                                dockPanelMain.Effect = null;
                                borderCoupon.Visibility = Visibility.Hidden;

                                RedrawSpecification();
                                activeCheck.RecalculateTotals();

                                //panelTopay.Visibility = Visibility.Visible;

                                ////  Анимируем labelTopay
                                //DoubleAnimation doubleAnimationLabelTopaySumFontSize = new DoubleAnimation(42, 46, new Duration(TimeSpan.FromSeconds(0.35)))
                                //{
                                //    AutoReverse = true,
                                //    RepeatBehavior = RepeatBehavior.Forever
                                //};
                                //labelTopaySum.BeginAnimation(FontSizeProperty, doubleAnimationLabelTopaySumFontSize);

                                activeCheck.couponMode = false;

                                activeCheck.LoyaltySum = activeCheck.loyalty.Sums;

                                panelLoyalty.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                ShowErrorCanvas();
                                if (result == "КАРТА ЛОЯЛЬНОСТИ НЕ НАЙДЕНА")
                                    ShowErrorMessage(result, 42);
                                else if (result == "КАРТА ЛОЯЛЬНОСТИ УЖЕ ПРИМЕНЕНА")
                                    ShowErrorMessage(result, 38);
                                else if (result == "КАРТА ЛОЯЛЬНОСТИ ЗАБЛОКИРОВАНА")
                                    ShowErrorMessage(result, 38);
                                else if (result == "КАРТА ЛОЯЛЬНОСТИ НЕ АКТИВИРОВАНА")
                                    ShowErrorMessage(result, 36);
                                else
                                    ShowErrorMessage(result, 50);
                            }
                        }
                    }
#endregion Завершен ввод кода карты лояльности
                    else if (activePosition.InputType == ActivePositionTypes.barcodeType)
                    {
#region ввод суммы наличными завершен
                        if (activeCheck.ChangeSum >= 0 && labelChangeHeader.Content.ToString() != "СДАЧА")
                        {
                            if (activePosition.Barcode.Length > 0 && activeCheck.paymentModeCash)
                            {
                                //PlayKeyboardTone();

                                activeCheck.CashSum += Convert.ToDouble(activePosition.Barcode);
                                activeCheck.ChangeSum = Math.Round(activeCheck.TopaySum - (activeCheck.CashSum + activeCheck.CardSum), 2);
                                if (activeCheck.ChangeSum < 0)
                                {
                                    labelChangeHeader.Content = "СДАЧА";
                                    labelChangeHeader.Foreground = new SolidColorBrush(Colors.Sienna);
                                    labelChange.Foreground = new SolidColorBrush(Colors.Sienna);
                                    activeCheck.ChangeSum *= -1;
                                }
                                else if (activeCheck.ChangeSum > 0)
                                {
                                    labelChangeHeader.Content = "НЕДОПЛАТА";
                                    labelChangeHeader.Foreground = new SolidColorBrush(Colors.YellowGreen);
                                    labelChange.Foreground = new SolidColorBrush(Colors.YellowGreen);
                                }
                                else
                                {
                                    labelChangeHeader.Content = "РОВНО";
                                    labelChangeHeader.Foreground = new SolidColorBrush(Colors.YellowGreen);
                                    labelChange.Foreground = new SolidColorBrush(Colors.YellowGreen);
                                }
                                if (activeCheck.ChangeSum > 0 && labelChangeHeader.Content.ToString() != "СДАЧА")
                                {
                                    activePosition.Info = activeCheck.ChangeSum.ToString();
                                    activePosition.Barcode = activePosition.Info;
                                }
                                else
                                {
                                    activePosition.Barcode = "";
                                    activePosition.Info = "";
                                    labelPaymentType.Visibility = Visibility.Collapsed;

                                    activeCheck.preFinished = true;

                                    //  Из режима ПРОДАЖА программа переходит в режим СВОБОДНА
                                    labelSaleMode.Style = Resources["InactivePanelLabel"] as Style;
                                    labelReturnMode.Style = Resources["InactivePanelLabel"] as Style;
                                    labelTotalMode.Style = Resources["InactivePanelLabel"] as Style;
                                    labelPaymentMode.Style = Resources["InactivePanelLabel"] as Style;
                                    labelReadyMode.Style = Resources["ActivePanelLabel"] as Style;
                                    labelCouponMode.Style = Resources["InactivePanelLabel"] as Style;

                                    activeCheck.loyalty.SumSub = activeCheck.CardSum;
                                    if (activeCheck.CardSum == 0)
                                        activeCheck.loyalty.SumAdd = Math.Round(activeCheck.TopaySum / 100 * activeCheck.loyalty.Percent, 2);
                                    else
                                        activeCheck.loyalty.SumAdd = Math.Round((activeCheck.TopaySum - activeCheck.CardSum) / 100 * activeCheck.loyalty.Percent, 2);
                                    activeCheck.loyalty.SumNew = Math.Round(activeCheck.loyalty.Sums - activeCheck.loyalty.SumSub + activeCheck.loyalty.SumAdd, 2);
                                    activeCheck.LoyaltySum = activeCheck.loyalty.SumNew;

                                    if (activeCheck.loyalty.Code != null)
                                        UploadLoyalty();
                                }
                                activePosition.InputType = ActivePositionTypes.barcodeType;
                                activePosition.Regime = ActivePositionModes.codeReady;
                            }
#endregion ввод суммы наличными завершен
#region ввод суммы лояльностью завершен
                            else if (activePosition.Barcode.Length > 0 && activeCheck.paymentModeCard)
                            {
                                //PlayKeyboardTone();

                                double cardSum = Convert.ToInt32(activePosition.Barcode);
                                if (cardSum > activeCheck.LoyaltySum)
                                {
                                    System.Windows.MessageBox.Show("Превышение суммы карты лояльности!");
                                    return;
                                }
                                if (activeCheck.TopaySum - (activeCheck.CashSum + activeCheck.CardSum) < cardSum)
                                {
                                    cardSum = activeCheck.TopaySum - (activeCheck.CashSum + activeCheck.CardSum);
                                }
                                activeCheck.CardSum += cardSum;
                                activeCheck.LoyaltySum = Math.Round(activeCheck.LoyaltySum - cardSum, 2);
                                activeCheck.ChangeSum = Math.Round(activeCheck.TopaySum - (activeCheck.CashSum + activeCheck.CardSum), 2);
                                if (activeCheck.ChangeSum < 0)
                                {
                                    labelChangeHeader.Content = "СДАЧА";
                                    labelChangeHeader.Foreground = new SolidColorBrush(Colors.Sienna);
                                    labelChange.Foreground = new SolidColorBrush(Colors.Sienna);
                                    activeCheck.ChangeSum *= -1;
                                }
                                else if (activeCheck.ChangeSum > 0)
                                {
                                    labelChangeHeader.Content = "НЕДОПЛАТА";
                                    labelChangeHeader.Foreground = new SolidColorBrush(Colors.YellowGreen);
                                    labelChange.Foreground = new SolidColorBrush(Colors.YellowGreen);
                                    activeCheck.paymentModeCard = false;
                                    activeCheck.paymentModeCash = true;
                                    labelPaymentType.Content = "СУММА НАЛИЧНЫМИ:";
                                }
                                else
                                {
                                    labelChangeHeader.Content = "РОВНО";
                                    labelChangeHeader.Foreground = new SolidColorBrush(Colors.YellowGreen);
                                    labelChange.Foreground = new SolidColorBrush(Colors.YellowGreen);
                                }
                                if (activeCheck.ChangeSum > 0 && labelChangeHeader.Content.ToString() != "СДАЧА")
                                {
                                    activePosition.Info = activeCheck.ChangeSum.ToString();
                                    activePosition.Barcode = activePosition.Info;
                                }
                                else
                                {
                                    activePosition.Barcode = "";
                                    activePosition.Info = "";
                                    labelPaymentType.Visibility = Visibility.Collapsed;

                                    activeCheck.preFinished = true;

                                    //  Из режима ПРОДАЖА программа переходит в режим СВОБОДНА
                                    labelSaleMode.Style = Resources["InactivePanelLabel"] as Style;
                                    labelReturnMode.Style = Resources["InactivePanelLabel"] as Style;
                                    labelTotalMode.Style = Resources["InactivePanelLabel"] as Style;
                                    labelPaymentMode.Style = Resources["InactivePanelLabel"] as Style;
                                    labelReadyMode.Style = Resources["ActivePanelLabel"] as Style;
                                    labelCouponMode.Style = Resources["InactivePanelLabel"] as Style;

                                    activeCheck.loyalty.SumSub = activeCheck.CardSum;
                                    if (activeCheck.CardSum == 0)
                                        activeCheck.loyalty.SumAdd = Math.Round(activeCheck.TopaySum / 100 * activeCheck.loyalty.Percent, 2);
                                    else
                                        activeCheck.loyalty.SumAdd = Math.Round((activeCheck.TopaySum - activeCheck.CardSum) / 100 * activeCheck.loyalty.Percent, 2);
                                    activeCheck.loyalty.SumNew = Math.Round(activeCheck.loyalty.Sums - activeCheck.loyalty.SumSub + activeCheck.loyalty.SumAdd, 2);
                                    activeCheck.LoyaltySum = activeCheck.loyalty.SumNew;

                                    if (activeCheck.loyalty.Code == null)
                                        UploadLoyalty();
                                }
                                activePosition.InputType = ActivePositionTypes.barcodeType;
                                activePosition.Regime = ActivePositionModes.codeReady;
                            }
                            else
                            {
                                if (keysAfterAuthorization > 1) //  20201215
                                {
                                    ShowErrorCanvas();
                                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                                }
                            }
#endregion ввод суммы лояльностью завершен
                        }
                        else
                        {
                            activePosition.Barcode = "";
                            activePosition.Info = "";
                            ShowErrorCanvas();
                            ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                        }

                        if (activePosition.Regime == ActivePositionModes.codeReady && activeCheck.paymentMode && (labelChangeHeader.Content.ToString() == "СДАЧА" || labelChangeHeader.Content.ToString() == "РОВНО"))
                        {
#region завершение чека
                            //PlayKeyboardTone();

                            activeCheck.type = "+";
                            //if (activeCheck.saleMode)
                            //    activeCheck.type = "+";
                            //else if (activeCheck.returnMode)
                            //    activeCheck.type = "-";
                            if (activeCheck.returnMode)
                                activeCheck.type = "-";
                            else
                                Drawer.OpenDrawer();

                            activeCheck.AddCheckToDBF();

                            Display.ClearText();
                            Display.DisplayText(Display.FormatTwoStrings("ИТОГО:", activeCheck.TopaySum.ToString()));
                            Display.DisplayTextAt(0, 1, Display.FormatTwoStrings("СДАЧА: ", activeCheck.ChangeSum.ToString()));

                            if (!Properties.Settings.Default.UseWindowsDriverForPrinting)
                                PrintFooterAndHeaderOPOS();
                            else
                                PrintFooterAndHeaderWindows();
                            //FinishCheck();
#endregion завершение чека
                        }
                    }

                    else
                    {
                        if (keysAfterAuthorization > 1) //  20201215
                        {
                            ShowErrorCanvas();
                            ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                        }
                    }
                }
#endregion Enter
            }

            //  Card
            else if (e.Key == Key.F15 || e.Key == Key.C)
            {
#region B(езналичные)
                if (activePosition.InputType == ActivePositionTypes.barcodeType)
                {
#region ввод суммы картой завершен
                    if (activeCheck.ChangeSum > 0 && labelChangeHeader.Content.ToString() != "СДАЧА")
                    {
                        activeCheck.paymentModeCash = false;
                        activeCheck.paymentModeCard = true;
                        labelPaymentType.Content = "СУММА ЛОЯЛЬНОСТИ:";
                        //activeCheck.LoyaltySum
                        //if (activePosition.Barcode.Length > 0)
                        //{
                        //    double cardSum = Convert.ToInt32(activePosition.Barcode);
                        //    if (activeCheck.TopaySum - (activeCheck.CashSum + activeCheck.CardSum) < cardSum)
                        //    {
                        //        cardSum = activeCheck.TopaySum - (activeCheck.CashSum + activeCheck.CardSum);
                        //    }
                        //    activeCheck.CardSum += cardSum;
                        //    activeCheck.ChangeSum = activeCheck.TopaySum - (activeCheck.CashSum + activeCheck.CardSum);
                        //    if (activeCheck.ChangeSum < 0)
                        //    {
                        //        labelChangeHeader.Content = "СДАЧА";
                        //        labelChangeHeader.Foreground = new SolidColorBrush(Colors.Sienna);
                        //        labelChange.Foreground = new SolidColorBrush(Colors.Sienna);
                        //        activeCheck.ChangeSum *= -1;
                        //    }
                        //    else if (activeCheck.ChangeSum > 0)
                        //    {
                        //        labelChangeHeader.Content = "НЕДОПЛАТА";
                        //        labelChangeHeader.Foreground = new SolidColorBrush(Colors.YellowGreen);
                        //        labelChange.Foreground = new SolidColorBrush(Colors.YellowGreen);
                        //    }
                        //    else
                        //    {
                        //        labelChangeHeader.Content = "РОВНО";
                        //        labelChangeHeader.Foreground = new SolidColorBrush(Colors.YellowGreen);
                        //        labelChange.Foreground = new SolidColorBrush(Colors.YellowGreen);
                        //    }
                        //    if (activeCheck.ChangeSum > 0 && labelChangeHeader.Content.ToString() != "СДАЧА")
                        //    {
                        //        activePosition.Info = activeCheck.ChangeSum.ToString();
                        //        activePosition.Barcode = activePosition.Info;
                        //    }
                        //    else
                        //    {
                        //        activePosition.Barcode = "";
                        //        activePosition.Info = "";
                        //    }
                        //    activePosition.InputType = ActivePositionTypes.barcodeType;
                        //    activePosition.Regime = ActivePositionModes.codeReady;
                        //}
                    }
                    else
                    {
                        activePosition.Barcode = "";
                        activePosition.Info = "";
                        ShowErrorCanvas();
                        ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                    }
#endregion ввод суммы картой завершен
                }
#endregion B(езналичные)
            }

            else if (e.Key == Key.LeftCtrl)
            {
#region ИТОГ (LeftCtrl)
                if (activeCheck.preFinished || activeCheck.totalMode || activeCheck.paymentMode || activePosition.Regime == ActivePositionModes.codeInput)
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                    return;
                }

                //PlayKeyboardTone();

                activeCheck.totalMode = true;
                labelTotalMode.Style = Resources["ActivePanelLabel"] as Style;

                if (!Properties.Settings.Default.UseWindowsDriverForPrinting)
                    PrintSubHeaderWithSpecificationOPOS();
                else
                    PrintSubHeaderWithSpecificationWindows();
#endregion ИТОГ (LeftCtrl)
            }

            //else if (e.Key == Key.OemMinus || e.Key == Key.Space)
            else if (e.Key == Key.Space)
            {
#region ОПЛАТА (OemMinus || Space)
                //  Программа готова к добавлению позиции, но был нажат "-" (OemMinus) - это означает, что позиций больше не будет - переход к следующему этапу транзакции
                if (activeCheck.preFinished || !activeCheck.totalMode)
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                    return;
                }
                if (activePosition.Regime == ActivePositionModes.codeReady && activeCheck.specification.Count > 0)
                {
                    //PlayKeyboardTone();

                    if (activeCheck.returnMode)
                        Drawer.OpenDrawer();
#region переход к оплате
                    activePosition.Clear();
                    //  activePosition.Clear() не очищает Info, поэтому сделаем это вручную
                    activePosition.Info = "";

                    activeCheck.paymentMode = true;
                    activeCheck.paymentModeCash = true;
                    activeCheck.paymentModeCard = false;
                    labelPaymentType.Content = "СУММА НАЛИЧНЫМИ:";
                    labelPaymentType.Visibility = Visibility.Visible;

                    //  Из режима ПРОДАЖА программа переходит в режим ПРОДАЖА + ОПЛАТА
                    //labelSaleMode.Style = Resources["InactivePanelLabel"] as Style;
                    labelPaymentMode.Style = Resources["ActivePanelLabel"] as Style;

                    panelQuantity.Visibility = Visibility.Collapsed;
                    labelQuantity.Content = 0;
                    panelBarcode.Visibility = Visibility.Collapsed;
                    labelBarcode.Content = "";
                    panelDescription.Visibility = Visibility.Collapsed;
                    labelDescriptionWin2.Content = "";

                    panelTopay.Visibility = Visibility.Visible;
                    panelAcceptedInCash.Visibility = Visibility.Visible;
                    panelAcceptedByCard.Visibility = Visibility.Visible;
                    panelChange.Visibility = Visibility.Visible;

                    activeCheck.ChangeSum = Math.Round(activeCheck.TopaySum - (activeCheck.CashSum + activeCheck.CardSum), 2);

                    //  Анимируем labelTopay
                    DoubleAnimation doubleAnimationLabelTopaySumFontSize = new DoubleAnimation(42, 46, new Duration(TimeSpan.FromSeconds(0.35)))
                    {
                        AutoReverse = true,
                        RepeatBehavior = RepeatBehavior.Forever
                    };
                    labelTopaySum.BeginAnimation(FontSizeProperty, doubleAnimationLabelTopaySumFontSize);

                    //  Анимируем labelChange
                    DoubleAnimation doubleAnimationLabelChangeFontSize = new DoubleAnimation(32, 36, new Duration(TimeSpan.FromSeconds(0.35)))
                    {
                        AutoReverse = true,
                        RepeatBehavior = RepeatBehavior.Forever
                    };
                    labelChange.BeginAnimation(FontSizeProperty, doubleAnimationLabelChangeFontSize);

                    activePosition.Info = activeCheck.TopaySum.ToString();
                    activePosition.Barcode = activePosition.Info;
#endregion переход к оплате
                }
                else
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                    //System.Windows.MessageBox.Show(activePosition.Regime.ToString());
                }
#endregion ОПЛАТА (OemMinus || Space)
            }

            //  Annulation
            else if (e.Key == Key.Oem3) //  для отладки || e.Key == Key.RightCtrl)
            {
#region вызов диалога "Аннуляция"
                if (activeCheck.preFinished)
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                    return;
                }
                if (!activeCheck.annulationMode)
                {
                    activeCheck.annulationMode = true;

                    BlurEffect blurEffect = new BlurEffect() { Radius = 6 };
                    dockPanelMain.Effect = blurEffect;

                    //borderAnnulation.Visibility = Visibility.Visible;
                    //buttonAnnulateYes.Focus();

                    if (windowAuthorization == null)
                        windowAuthorization = new WindowAutorization();
                    windowAuthorization.Mode = "super";
                    windowAuthorization.labelContent.Content = "!!! АННУЛЯЦИЯ !!!";
                    keysAfterAuthorization = 0;

                    windowAuthorization.ShowDialog();
                    bool authorizationResult = windowAuthorization.Result;
                    windowAuthorization.Close();
                    windowAuthorization = null;
                    dockPanelMain.Effect = null;
                    if (!authorizationResult)
                    {
                        activeCheck.annulationMode = false;
                        return;
                    }
                    else
                    {
                        if (activeCheck.preFinished)
                        {
                            activeCheck.annulationMode = false;
                            if (keysAfterAuthorization > 1) //  20201215
                            {
                                ShowErrorCanvas();
                                ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                            }
                            return;
                        }
                        activeCheck.annulationMode = false;

                        #region Получено подтверждение аннуляции - завершение аннулированного чека
                        activeCheck.type = "@";

                        activeCheck.PrepareCheck();
                        activeCheck.AddCheckToDBF();

                        Display.ClearText();
                        Display.DisplayText(Display.FormatTwoStrings("АННУЛЯЦИЯ:", activeCheck.TopaySum.ToString()));

                        if (!Properties.Settings.Default.UseWindowsDriverForPrinting)
                        {
                            if (!activeCheck.totalMode && !activeCheck.paymentMode)
                            {
                                Printer.SetRegularFont();
                                Printer.BeginTransaction();
                                if (!Properties.Settings.Default.FontB)
                                    Printer.PrintString(Printer.FormatTwoStrings("KASSIR: " + Vars.cashierName, " POS № " + Properties.Settings.Default.POSNumber));
                                else
                                    Printer.FormatAndPrintTwoStringsB("KASSIR: " + Vars.cashierName, " POS № " + Properties.Settings.Default.POSNumber);
                                string date = activeCheck.date.Substring(6, 2) + "/" + activeCheck.date.Substring(4, 2) + "/" + activeCheck.date.Substring(0, 4);
                                string time = activeCheck.time.Substring(0, 2) + ":" + activeCheck.time.Substring(2, 2) + ":" + activeCheck.time.Substring(4, 2);
                                if (activeCheck.returnMode)
                                {
                                    if (!Properties.Settings.Default.FontB)
                                        Printer.PrintString(Printer.FormatTwoStrings("QAYTAR CHEKI № " + activeCheck.number, date + " " + time));
                                    else
                                        Printer.FormatAndPrintTwoStringsB("QAYTAR CHEKI № " + activeCheck.number, date + " " + time);
                                }
                                else
                                {
                                    if (!Properties.Settings.Default.FontB)
                                        Printer.PrintString(Printer.FormatTwoStrings("SAVDO CHEKI № " + activeCheck.number, date + " " + time));
                                    else
                                        Printer.FormatAndPrintTwoStringsB("SAVDO CHEKI № " + activeCheck.number, date + " " + time);
                                }
                                if (!Properties.Settings.Default.FontB)
                                {
                                    if (width == 38)
                                        Printer.PrintString("--------------------------------------");
                                    else if (width == 44)
                                        Printer.PrintString("--------------------------------------------");
                                }
                                else
                                    Printer.CenterAndPrintTextB("----------------------------------------------------------------");
                                for (int i = 0; i < activeCheck.specification.Count; i++)
                                {
                                    if (!Properties.Settings.Default.FontB)
                                    {
                                        string itemToPrint = Printer.FormatString(activeCheck.specification[i]);
                                        Printer.PrintString(itemToPrint);
                                    }
                                    else
                                        Printer.FormatAndPrintStringB(activeCheck.specification[i]);
                                }
                            }
                            Printer.EndTransaction();
                        }
                        else
                            PrintSubHeaderWithSpecificationWindows();

                        if (!Properties.Settings.Default.UseWindowsDriverForPrinting)
                        {
                            Printer.BeginTransaction();
                            if (!Properties.Settings.Default.FontB)
                            {
                                if (width == 38)
                                {
                                    if (!activeCheck.totalMode && !activeCheck.paymentMode)
                                        Printer.PrintString("--------------------------------------");
                                    Printer.SetBoldFont();
                                    Printer.PrintString("            ЧЕК АННУЛИРОВАН           ");
                                    Printer.PrintString("");
                                    if (Vars.fiscalMode)
                                        Printer.PrintString("                 -FR-                 ");
                                    else
                                        Printer.PrintString("                -NFR-                 ");
                                    //Printer.PrintRegistanBitmap();
                                    //Printer.PrintString("--------------------------------------");
                                    //Printer.CutPaper();
                                    Printer.PrintString(Printer.CenterText(Properties.Settings.Default.CompanyName));
                                    Printer.PrintString(Printer.CenterText(Properties.Settings.Default.CompanyAddress));
                                    Printer.CutPaper();
                                    Printer.PrintString(Printer.FormatTwoStrings("INN: " + Properties.Settings.Default.INN, "S/N " + Properties.Settings.Default.SerialNumber));
                                }
                                else if (width == 44)
                                {
                                    if (!activeCheck.totalMode && !activeCheck.paymentMode)
                                        Printer.PrintString("--------------------------------------------");
                                    Printer.PrintString("\x1b|bC               ЧЕК АННУЛИРОВАН              ");
                                    Printer.PrintString("");
                                    if (Vars.fiscalMode)
                                        Printer.PrintString("\x1b|bC                    -FR-                    ");
                                    else
                                        Printer.PrintString("\x1b|bC                   -NFR-                    ");
                                    Printer.PrintString(Printer.CenterText("\x1b|bC" + Properties.Settings.Default.CompanyName));
                                    Printer.PrintString(Printer.CenterText("\x1b|bC" + Properties.Settings.Default.CompanyAddress));
                                    Printer.CutPaper();
                                    Printer.PrintString(Printer.FormatTwoStrings("\x1b|bCINN: " + Properties.Settings.Default.INN, "S/N " + Properties.Settings.Default.SerialNumber));
                                }
                            }
                            else
                            {
                                if (!activeCheck.totalMode && !activeCheck.paymentMode)
                                    Printer.CenterAndPrintTextB("----------------------------------------------------------------");
                                Printer.CenterAndPrintTextB("\x1b|bCЧЕК АННУЛИРОВАН");
                                Printer.CenterAndPrintTextB("");
                                if (Vars.fiscalMode)
                                    Printer.CenterAndPrintTextB("\x1b|bC-FR-");
                                else
                                    Printer.CenterAndPrintTextB("\x1b|bC-NFR-");
                                Printer.CenterAndPrintTextB("\x1b|bC" + Properties.Settings.Default.CompanyName);
                                Printer.CenterAndPrintTextB("\x1b|bC" + Properties.Settings.Default.CompanyAddress);
                                Printer.CutPaper();
                                Printer.FormatAndPrintTwoStringsB("\x1b|bCINN: " + Properties.Settings.Default.INN, "S/N " + Properties.Settings.Default.SerialNumber);
                            }
                            Printer.EndTransaction();

                            //  Это добавлено чтобы после аннуляции в режиме "ОПЛАТА" исчезла надпись "СУММА НАЛИЧНЫМИ" нач
                            activePosition.Barcode = "";
                            activePosition.Info = "";
                            labelPaymentType.Visibility = Visibility.Collapsed;

                            activeCheck.preFinished = true;
                            //  Это добавлено чтобы после аннуляции в режиме "ОПЛАТА" исчезла надпись "СУММА НАЛИЧНЫМИ" кон
                        }
                        else
                            PrintFooterAndHeaderAnnulationWindows();

                        FinishCheck();
                        #endregion Получено подтверждение аннуляции - завершение аннулированного чека
                    }
                }
                #endregion вызов диалога (Анну)L(яция)
            }

            //else if (e.Key == Key.Space)
            else if (e.Key == Key.OemPlus)
            {
#region Завершение чека (Space)
                if (activeCheck.preFinished)
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                    return;
                }
                //  Программа готова к внесению оплаты, но расчет уже выполнен и был нажат " " (Key.Space, Пробел) - это означает, что чек завершен
                if (activePosition.Regime == ActivePositionModes.codeReady && activeCheck.paymentMode && (labelChangeHeader.Content.ToString() == "СДАЧА" || labelChangeHeader.Content.ToString() == "РОВНО"))
                {
#region завершение чека
                    activeCheck.type = "+";
                    //if (activeCheck.saleMode)
                    //    activeCheck.type = "+";
                    //else if (activeCheck.returnMode)
                    //    activeCheck.type = "-";
                    if (activeCheck.returnMode)
                        activeCheck.type = "-";

                    activeCheck.AddCheckToDBF();

                    Display.ClearText();
                    Display.DisplayText(Display.FormatTwoStrings("ИТОГО:", activeCheck.TopaySum.ToString()));
                    Display.DisplayTextAt(0, 1, Display.FormatTwoStrings("СДАЧА: ", activeCheck.ChangeSum.ToString()));

                    //Printer.SetRegularFont();
                    //Printer.PrintString(Printer.FormatTwoStrings("KASSIR: " + Vars.cashierName, " POS № 7"));
                    //string date = activeCheck.date.Substring(6, 2) + "/" + activeCheck.date.Substring(4, 2) + "/" + activeCheck.date.Substring(0, 4);
                    //string time = activeCheck.time.Substring(0, 2) + ":" + activeCheck.time.Substring(2, 2) + ":" + activeCheck.time.Substring(4, 2);
                    //if (activeCheck.returnMode)
                    //    Printer.PrintString(Printer.FormatTwoStrings("QAYTAR CHEKI № " + activeCheck.number, date + " " + time));
                    //else
                    //    Printer.PrintString(Printer.FormatTwoStrings("SOTMOQ CHEKI № " + activeCheck.number, date + " " + time));
                    //Printer.PrintString("--------------------------------------");
                    //for (int i = 0; i < activeCheck.specification.Count; i++)
                    //{
                    //    string itemToPrint = Printer.FormatString(activeCheck.specification[i]);
                    //    Printer.PrintString(itemToPrint);
                    //}

                    //Printer.SetBoldFont();
                    //Printer.PrintString("--------------------------------------");
                    Printer.BeginTransaction();
                    if (activeCheck.returnMode)
                    {
                        //Printer.PrintString("\x1b|1C\x1b|3C" + Printer.FormatTwoStrings("XAMMASI (К ВЫПЛАТЕ):", activeCheck.TopaySum.ToString() + " So'm"));
                        Printer.PrintString("\x1b|1C" + Printer.FormatTwoStrings("XAMMASI (К ВЫПЛАТЕ):", activeCheck.TopaySum.ToString() + " So'm"));
                        Printer.PrintString(Printer.FormatTwoStrings("TO'LANDI (ВЫДАНО):", activeCheck.CashSum.ToString() + " So'm"));
                    }
                    else
                    {
                        //Printer.PrintString("\x1b|bC\x1b|3C" + Printer.FormatTwoStrings("XAMMASI (К ОПЛАТЕ):", activeCheck.TopaySum.ToString() + " So'm"));
                        Printer.PrintString("\x1b|1C" + Printer.FormatTwoStrings("XAMMASI (К ОПЛАТЕ):", activeCheck.TopaySum.ToString() + " So'm"));

                        if (activeCheck.VAT7 != 0)
                            Printer.PrintString(Printer.FormatTwoStrings("S.J. QQS (В Т.Ч. НДС) 7%:", activeCheck.VAT7.ToString() + " So'm"));
                        if (activeCheck.VAT10 != 0)
                            Printer.PrintString(Printer.FormatTwoStrings("S.J. QQS (В Т.Ч. НДС) 10%:", activeCheck.VAT10.ToString() + " So'm"));
                        Printer.PrintString(Printer.FormatTwoStrings("S.J. QQS (В Т.Ч. НДС) 15%:", activeCheck.VAT15.ToString() + " So'm"));
                        if (activeCheck.VAT20 != 0)
                            Printer.PrintString(Printer.FormatTwoStrings("S.J. QQS (В Т.Ч. НДС) 20%:", activeCheck.VAT20.ToString() + " So'm"));
                        Printer.PrintString(Printer.FormatTwoStrings("TO'LANDI (ОПЛАЧЕНО):", activeCheck.CashSum.ToString() + " So'm"));
                        if (activeCheck.ChangeSum != 0)
                            Printer.PrintString(Printer.FormatTwoStrings("QAYTIM (СДАЧА):", activeCheck.ChangeSum.ToString() + " So'm"));
                        if (activeCheck.loyalty.Code != null)
                        {
                            if (activeCheck.CardSum > 0)
                                Printer.PrintString(Printer.FormatTwoStrings("LOYAL (ЛОЯЛЬНОСТЬЮ):", activeCheck.LoyaltySum.ToString() + " So'm"));
                        }
                    }
                    Printer.PrintString("--------------------------------------");
                    Printer.PrintString("          RAXMAT! - СПАСИБО!          ");
                    Printer.PrintString("");
                    if (Vars.fiscalMode)
                        Printer.PrintString("                 -FR-                 ");
                    else
                        Printer.PrintString("                -NFR-                 ");
                    //Printer.PrintRegistanBitmap();
                    //Printer.PrintString("--------------------------------------");
                    //Printer.CutPaper();
                    Printer.PrintString(Printer.CenterText(Properties.Settings.Default.CompanyName));
                    Printer.PrintString(Printer.CenterText(Properties.Settings.Default.CompanyAddress));
                    Printer.CutPaper();
                    Printer.PrintString(Printer.FormatTwoStrings("INN: " + Properties.Settings.Default.INN, "S/N " + Properties.Settings.Default.SerialNumber));
                    Printer.EndTransaction();

                    //FinishCheck();
#endregion завершение чека
                }
                else
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                }
#endregion Завершение чека (Space)
            }
            else if (e.Key == Key.A)    //  кнопка 50
            {
                if (activeCheck.returnMode && activeCheck.specification.Count > 0 && !activeCheck.preFinished && !activeCheck.finished)
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);

                    e.Handled = true;
                    return;
                }

                //PlayKeyboardTone();

                //SendBarcode("47800922");
                if (!activeCheck.totalMode && !activeCheck.paymentMode || activeCheck.preFinished)
                    SendBarcode(Properties.Settings.Default.btn5Barcode);
                else
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.B)    //  кнопка под 50
            {
                if (activeCheck.returnMode && activeCheck.specification.Count > 0 && !activeCheck.preFinished && !activeCheck.finished)
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);

                    e.Handled = true;
                    return;
                }

                //PlayKeyboardTone();

                if (!activeCheck.totalMode && !activeCheck.paymentMode)
                    SendBarcode(Properties.Settings.Default.btn9Barcode);
                else
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.C)    //  кнопка 100
            {
                if (activeCheck.returnMode && activeCheck.specification.Count > 0 && !activeCheck.preFinished && !activeCheck.finished)
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);

                    e.Handled = true;
                    return;
                }

                //PlayKeyboardTone();

                //SendBarcode("47801073");
                if (!activeCheck.totalMode && !activeCheck.paymentMode || activeCheck.preFinished)
                    SendBarcode(Properties.Settings.Default.btn13Barcode);
                else
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.D)    //  кнопка под 100
            {
                if (activeCheck.returnMode && activeCheck.specification.Count > 0 && !activeCheck.preFinished && !activeCheck.finished)
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);

                    e.Handled = true;
                    return;
                }

                //PlayKeyboardTone();

                if (!activeCheck.totalMode && !activeCheck.paymentMode)
                    SendBarcode(Properties.Settings.Default.btn17Barcode);
                else
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.F1)    //  кнопка МЫЛО ХОЗ
            {
                if (activeCheck.returnMode && activeCheck.specification.Count > 0 && !activeCheck.preFinished && !activeCheck.finished)
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);

                    e.Handled = true;
                    return;
                }

                //PlayKeyboardTone();

                //SendBarcode("?");
                if (!activeCheck.totalMode && !activeCheck.paymentMode || activeCheck.preFinished)
                    SendBarcode(Properties.Settings.Default.btn2Barcode);
                else
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.F2)    //  кнопка ЯЙЦО
            {
                if (activeCheck.returnMode && activeCheck.specification.Count > 0 && !activeCheck.preFinished && !activeCheck.finished)
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);

                    e.Handled = true;
                    return;
                }

                //PlayKeyboardTone();

                //SendBarcode("?");
                if (!activeCheck.totalMode && !activeCheck.paymentMode || activeCheck.preFinished)
                    SendBarcode(Properties.Settings.Default.btn3Barcode);
                else
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.F3)    //  кнопка ЯЙЦО D2
            {
                if (activeCheck.returnMode && activeCheck.specification.Count > 0 && !activeCheck.preFinished && !activeCheck.finished)
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);

                    e.Handled = true;
                    return;
                }

                //PlayKeyboardTone();

                //SendBarcode("21080951");
                if (!activeCheck.totalMode && !activeCheck.paymentMode || activeCheck.preFinished)
                    SendBarcode(Properties.Settings.Default.btn4Barcode);
                else
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.F4)    //  кнопка ТЕСТО МУЗА
            {
                if (activeCheck.returnMode && activeCheck.specification.Count > 0 && !activeCheck.preFinished && !activeCheck.finished)
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);

                    e.Handled = true;
                    return;
                }

                //PlayKeyboardTone();

                //SendBarcode("25002270");
                if (!activeCheck.totalMode && !activeCheck.paymentMode || activeCheck.preFinished)
                    SendBarcode(Properties.Settings.Default.btn6Barcode);
                else
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.F5)    //  кнопка ЖВАЧКА
            {
                if (activeCheck.returnMode && activeCheck.specification.Count > 0 && !activeCheck.preFinished && !activeCheck.finished)
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);

                    e.Handled = true;
                    return;
                }

                //PlayKeyboardTone();

                //SendBarcode("4780019650033");
                if (!activeCheck.totalMode && !activeCheck.paymentMode || activeCheck.preFinished)
                    SendBarcode(Properties.Settings.Default.btn7Barcode);
                else
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.F6)    //  кнопка САЛАТ
            {
                if (activeCheck.returnMode && activeCheck.specification.Count > 0 && !activeCheck.preFinished && !activeCheck.finished)
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);

                    e.Handled = true;
                    return;
                }

                //PlayKeyboardTone();

                //SendBarcode("28005513");
                if (!activeCheck.totalMode && !activeCheck.paymentMode || activeCheck.preFinished)
                    SendBarcode(Properties.Settings.Default.btn8Barcode);
                else
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.F7)    //  кнопка ЗЕЛЕНЬ
            {
                if (activeCheck.returnMode && activeCheck.specification.Count > 0 && !activeCheck.preFinished && !activeCheck.finished)
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);

                    e.Handled = true;
                    return;
                }

                //PlayKeyboardTone();

                //SendBarcode("28007562");
                if (!activeCheck.totalMode && !activeCheck.paymentMode || activeCheck.preFinished)
                    SendBarcode(Properties.Settings.Default.btn10Barcode);
                else
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.F8)    //  кнопка СЫРОК
            {
                if (activeCheck.returnMode && activeCheck.specification.Count > 0 && !activeCheck.preFinished && !activeCheck.finished)
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);

                    e.Handled = true;
                    return;
                }

                //PlayKeyboardTone();

                //SendBarcode("28008888");
                if (!activeCheck.totalMode && !activeCheck.paymentMode || activeCheck.preFinished)
                    SendBarcode(Properties.Settings.Default.btn11Barcode);
                else
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.F9)    //  кнопка СВЕЧИ
            {
                if (activeCheck.returnMode && activeCheck.specification.Count > 0 && !activeCheck.preFinished && !activeCheck.finished)
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);

                    e.Handled = true;
                    return;
                }

                //PlayKeyboardTone();

                //SendBarcode("27001912");
                if (!activeCheck.totalMode && !activeCheck.paymentMode || activeCheck.preFinished)
                    SendBarcode(Properties.Settings.Default.btn12Barcode);
                else
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                }
                e.Handled = true;
            }
            //else if (e.Key == Key.System)    //  кнопка ЧУПА-ЧУПС
            //else if (e.Key == Key.F10)    //  кнопка ЧУПА-ЧУПС
            else if (e.SystemKey == Key.F10)    //  кнопка ЧУПА-ЧУПС
            {
                if (activeCheck.returnMode && activeCheck.specification.Count > 0 && !activeCheck.preFinished && !activeCheck.finished)
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);

                    e.Handled = true;
                    return;
                }

                //PlayKeyboardTone();

                //SendBarcode("22355080");
                if (!activeCheck.totalMode && !activeCheck.paymentMode || activeCheck.preFinished)
                    SendBarcode(Properties.Settings.Default.btn14Barcode);
                else
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.F11)    //  кнопка ПЕР. ЯЙЦО
            {
                if (activeCheck.returnMode && activeCheck.specification.Count > 0 && !activeCheck.preFinished && !activeCheck.finished)
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);

                    e.Handled = true;
                    return;
                }

                //PlayKeyboardTone();

                //SendBarcode("21004834");
                if (!activeCheck.totalMode && !activeCheck.paymentMode || activeCheck.preFinished)
                    SendBarcode(Properties.Settings.Default.btn15Barcode);
                else
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.F12)    //  кнопка ПЕРЕЦ
            {
                if (activeCheck.returnMode && activeCheck.specification.Count > 0 && !activeCheck.preFinished && !activeCheck.finished)
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);

                    e.Handled = true;
                    return;
                }

                //PlayKeyboardTone();

                //SendBarcode("28000952");
                if (!activeCheck.totalMode && !activeCheck.paymentMode || activeCheck.preFinished)
                    SendBarcode(Properties.Settings.Default.btn16Barcode);
                else
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Tab)    //  кнопка СПИЧКИ
            {
                if (activeCheck.returnMode && activeCheck.specification.Count > 0 && !activeCheck.preFinished && !activeCheck.finished)
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);

                    e.Handled = true;
                    return;
                }

                //PlayKeyboardTone();

                //SendBarcode("21003936");
                if (!activeCheck.totalMode && !activeCheck.paymentMode || activeCheck.preFinished)
                    SendBarcode(Properties.Settings.Default.btn18Barcode);
                else
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.LWin)    //  кнопка ШАРИК С РИС
            {
                if (activeCheck.returnMode && activeCheck.specification.Count > 0 && !activeCheck.preFinished && !activeCheck.finished)
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);

                    e.Handled = true;
                    return;
                }

                //PlayKeyboardTone();

                //SendBarcode("21011641");
                if (!activeCheck.totalMode && !activeCheck.paymentMode || activeCheck.preFinished)
                    SendBarcode(Properties.Settings.Default.btn19Barcode);
                else
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                }
                e.Handled = true;
            }
            else if (e.SystemKey == Key.LeftAlt)    //  кнопка ЧЕСНОК
            {
                if (activeCheck.returnMode && activeCheck.specification.Count > 0 && !activeCheck.preFinished && !activeCheck.finished)
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);

                    e.Handled = true;
                    return;
                }

                //PlayKeyboardTone();

                //SendBarcode("21011979");
                if (!activeCheck.totalMode && !activeCheck.paymentMode || activeCheck.preFinished)
                    SendBarcode(Properties.Settings.Default.btn20Barcode);
                else
                {
                    ShowErrorCanvas();
                    ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.LeftShift)
            {
                //PlayKeyboardTone();

                Drawer.OpenDrawer();
            }
            else if (e.Key == Key.Scroll)   //  клавиша КУПОН
            {
                //PlayKeyboardTone();

                //System.Windows.MessageBox.Show("Scroll");
                ShowErrorCanvas();
                ShowErrorMessage("КУПОНЫ НЕ АКТИВИРОВАНЫ", 45);
            }
            else if (e.Key == Key.NumLock)   //  клавиша ВАУЧЕР
            {
                //PlayKeyboardTone();

                //System.Windows.MessageBox.Show("NumLock");
                ShowErrorCanvas();
                ShowErrorMessage("ВАУЧЕРЫ НЕ АКТИВИРОВАНЫ", 45);
            }
            else if (e.Key == Key.Pause)    //  клавиша ВВЕРХ
            {
                //PlayKeyboardTone();

                scrollViewerGrid.LineUp();
            }
            else if (e.Key == Key.OemQuotes)    //  клавиша ВНИЗ
            {
                //PlayKeyboardTone();

                scrollViewerGrid.LineDown();
            }
            else if (e.Key == Key.Back) //  клавиша TAB
            {
                //PlayKeyboardTone();

                //System.Windows.MessageBox.Show("Back");
                ShowErrorCanvas();
                ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
            }
            else if (e.Key == Key.Oem5) //  клавиша ПРОБЕЛ
            {
                //PlayKeyboardTone();

                //System.Windows.MessageBox.Show("Oem5");
                ShowErrorCanvas();
                ShowErrorMessage("НЕПРАВИЛЬНАЯ КОМАНДА", 50);
            }
            else if (e.Key != Key.F1 && e.Key != Key.F2 && e.Key != Key.F13 && e.Key != Key.Delete && e.Key != Key.Back && e.Key != Key.Oem5 && e.Key != Key.OemQuestion && e.Key != Key.Oem1 && e.Key != Key.F16 && e.Key != Key.F17)
            {
                //System.Windows.MessageBox.Show(e.Key.ToString());
                e.Handled = true;
                if (e.Key == Key.K)
                {
                    if (Vars.brightness + 5 <= 0xFF)
                        Vars.brightness += 5;
                    else
                        Vars.brightness = 0xFF;
                }
                else if (e.Key == Key.J)
                {
                    if (Vars.brightness - 5 >= 0xD3)
                        Vars.brightness -= 5;
                    else
                        Vars.brightness = 0xD3;
                }

                var labels = gridReceipt.Children.OfType<Label>();
                //var bc = new BrushConverter();
                foreach (Label label in labels)
                    //label.Foreground = (Brush)bc.ConvertFrom("#" + Vars.brightness.ToString("X2") + Vars.brightness.ToString("X2") + Vars.brightness.ToString("X2"));
                    label.Foreground = new SolidColorBrush(Color.FromRgb(Vars.brightness, Vars.brightness, Vars.brightness));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            labelCashierName.Content = Vars.cashierName;
            activePosition.Clear();
            activeCheck.finished = true;
            escapes = 0;

            if (fiscal.OpenRead())
            {
                if (fiscal.GetHeader())
                {
                    if (fiscal.fiscalHeader.fiscalMode[0] != '1')
                    {
                        fiscal.Close();
                        System.Windows.MessageBox.Show("Касса не фискализирована!");
                        Vars.fiscalMode = false;
                        this.Owner.Show();
                        this.Owner.Activate();
                        this.Owner.Opacity = 1;
                        Close();
                    }
                    else
                    {
                        Vars.fiscalMode = true;
                    }
                    if (fiscal.GetUsers())
                    {
                        if (fiscal.GetLatestZ())
                        {
                            if (!fiscal.CompareDateTime())
                            {
                                fiscal.Close();
                                System.Windows.MessageBox.Show("Не сделан суточный отчет!");
                                this.Owner.Show();
                                this.Owner.Activate();
                                this.Owner.Opacity = 1;
                                Close();
                            }
                        }
                        else
                        {
                            fiscal.Close();
                            System.Windows.MessageBox.Show("Ошибка чтения закрытия смены!");
                            this.Owner.Show();
                            this.Owner.Activate();
                            this.Owner.Opacity = 1;
                            Close();
                        }
                    }
                    else
                    {
                        fiscal.Close();
                        System.Windows.MessageBox.Show("Ошибка чтения журнала регистраций!");
                        this.Owner.Show();
                        this.Owner.Activate();
                        this.Owner.Opacity = 1;
                        Close();
                    }
                }
                fiscal.Close();
            }
            else
            {
                System.Windows.MessageBox.Show("Ошибка фискального модуля!");
                this.Owner.Show();
                this.Owner.Activate();
                this.Owner.Opacity = 1;
                Close();
            }
        }

        //  Возвращаем окно на поверхность после потери им фокуса
        private void ActiveCheckWindow_LostFocus(object sender, RoutedEventArgs e)
        {
            Topmost = true;
        }

        //  Контроль Regex при вводе номера купона
        private void TextBoxCouponCode_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex r = new Regex("^[0-9]*$");
            e.Handled = !r.IsMatch(e.Text);
        }

        //  Запрет пробела при вводе номера купона
        private void TextBoxCouponCode_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = e.Key == Key.Space;
        }

        //  Перерисовка GripReceipt (после ввода действительного купона и последовавшего пересчета)
        private void RedrawSpecification()
        {
            int numberOfPositions = gridReceipt.Children.Count / 6;
            for (int i = 0; i < numberOfPositions; i++)
            {
                if (activeCheck.specification[i].Discount != 0)
                {
                    System.Windows.Controls.Label label = (System.Windows.Controls.Label)(gridReceipt.Children[(i + 1) * 6 - 1]);
                    label.Content = activeCheck.specification[i].Discount;
                    label.Foreground = Brushes.Sienna;
                    gridReceipt.UpdateLayout();
                }
                if (activeCheck.specification[i].Quantity < 0)
                {
                    System.Windows.Controls.Label label = (System.Windows.Controls.Label)(gridReceipt.Children[(i + 1) * 6 - 4]);
                    //label.Background = new SolidColorBrush(Color.FromRgb(0x75, 0x00, 0x00));
                    //label.Background = System.Windows.Media.Brushes.Red;
                    label.Foreground = System.Windows.Media.Brushes.Yellow;
                    gridReceipt.UpdateLayout();
                }
            }
        }

        //  Аннуляция чека после ее подтверждения пользователем
        private void ButtonAnnulateYes_Click(object sender, RoutedEventArgs e)
        {
#region аннуляция чека
            dockPanelMain.Effect = null;
            borderAnnulation.Visibility = Visibility.Hidden;
            activeCheck.type = "@";
            //FinishCheck();    закомментировал давно
#endregion аннуляция чека
        }

        //  Пользователь отказался от аннуляции
        private void ButtonAnnulateNo_Click(object sender, RoutedEventArgs e)
        {
            dockPanelMain.Effect = null;
            borderAnnulation.Visibility = Visibility.Hidden;
            activeCheck.annulationMode = false;
        }

        //  Очистка gridReceipt от всех (кроме первой - шапки) строк, предварительно очистив содержимое удаляемых ячеек
        private void ClearGridReceipt()
        {
            while (gridReceipt.RowDefinitions.Count > 1)
            {
                gridReceipt.Children.RemoveRange(0, 6 * (gridReceipt.RowDefinitions.Count - 1));
                gridReceipt.RowDefinitions.RemoveAt(gridReceipt.RowDefinitions.Count - 1);
            }
        }

        //  Запись чека в DBF, его завершение и подготовка к новому чеку
        private void FinishCheck()
        {
            activePosition.Clear();
            //  activePosition.Clear() не очищает Info, поэтому сделаем это вручную
            activePosition.Info = "";

            //  Очистим gridReceipt
            ClearGridReceipt();

            //activeCheck.AddCheckToDBF();
            activeCheck.Clear();

            activeCheck.returnMode = false;
            if (activeCheck.autonoMode)
            {
                //Effect transition_Ripple_modifiedEffect = new Transition_Ripple_modifiedEffect();
                //canvasMain.Effect = transition_Ripple_modifiedEffect;
                //DoubleAnimation doubleAnimationCanvasMainParameter = new DoubleAnimation(80, 95, new Duration(TimeSpan.FromSeconds(1.0)))
                //{
                //    AutoReverse = true,
                //    RepeatBehavior = RepeatBehavior.Forever
                //};
                //transition_Ripple_modifiedEffect.BeginAnimation(Transition_Ripple_modifiedEffect.ProgressProperty, doubleAnimationCanvasMainParameter);
            }
            else
                canvasMain.Effect = null;
            canvasMain.Effect = null;

            //  Из режима ПРОДАЖА программа переходит в режим СВОБОДНА
            labelSaleMode.Style = Resources["InactivePanelLabel"] as Style;
            labelReturnMode.Style = Resources["InactivePanelLabel"] as Style;
            labelTotalMode.Style = Resources["InactivePanelLabel"] as Style;
            labelPaymentMode.Style = Resources["InactivePanelLabel"] as Style;
            labelReadyMode.Style = Resources["ActivePanelLabel"] as Style;
            labelCouponMode.Style = Resources["InactivePanelLabel"] as Style;

            activeCheck.couponMode = false;

            activeCheck.finished = true;
            panelTopay.Visibility = Visibility.Hidden;

            if (activePosition.Quantity == 0)
            {
                panelQuantity.Visibility = Visibility.Collapsed;
                labelQuantity.Content = 0;
            }
            panelBarcode.Visibility = Visibility.Collapsed;
            labelBarcode.Content = "";
            panelDescription.Visibility = Visibility.Collapsed;
            labelDescriptionWin2.Content = "";

            activeCheck.paymentMode = false;
            panelTopay.Visibility = Visibility.Hidden;
            panelAcceptedInCash.Visibility = Visibility.Hidden;
            panelAcceptedByCard.Visibility = Visibility.Hidden;
            panelChange.Visibility = Visibility.Hidden;
            labelChangeHeader.Content = "НЕДОПЛАТА";
            labelChangeHeader.Foreground = new SolidColorBrush(Colors.YellowGreen);
            labelChange.Foreground = new SolidColorBrush(Colors.YellowGreen);

            panelLoyalty.Visibility = Visibility.Hidden;

            activeCheck.paymentMode = false;
            activeCheck.totalMode = false;
        }

        private void ActiveCheckWindow_Closed(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.UseOPOSScanner)
            {
                scanner.DataEvent -= Scanner_DataEvent;
                scanner.ReleaseDevice();
                scanner.Close();
            }

            msr.DataEvent -= Msr_DataEvent;
            msr.ReleaseDevice();
            msr.Close();
        }

        //private void ActiveCheckWindow_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        //{
        //    if (e.Key == Key.LWin)
        //        e.Handled = true;
        //}

        //private void ActiveCheckWindow_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        //{
        //    if (e.Key == Key.LWin)
        //        e.Handled = true;
        //}

        private bool UploadLoyalty()
        {
            string footer1, footer2;
            string pathOut = Properties.Settings.Default.LoyaltyPath + @"\Out\" + Properties.Settings.Default.POSName + "-" + activeCheck.date + "-" + activeCheck.time + "-" + activeCheck.number + ".loy";
            using (StreamWriter outputFile = new StreamWriter(pathOut))
            {
                string header = Properties.Settings.Default.POSName + "-" + activeCheck.date + "-" + activeCheck.time + "-" + activeCheck.number + ";" + activeCheck.loyalty.Code + ";" + activeCheck.date + activeCheck.time + ";" + ";" + activeCheck.TopaySum.ToString() + ";" + activeCheck.CardSum.ToString() + ";" + Math.Round((activeCheck.TopaySum - activeCheck.CardSum) / 100 * activeCheck.loyalty.Percent, 2) + ";" + Vars.cashierName;
                outputFile.WriteLine(header);

                int i = 0;
                foreach (ActivePosition position in activeCheck.specification)
                {
                    i++;
                    string pos = i.ToString() + ";" + position.Barcode + ";" + position.Description + ";" + position.Quantity.ToString() + ";" + position.Price.ToString() + ";" + position.Sum;
                    outputFile.WriteLine(pos);
                }

                if (activeCheck.CardSum != 0)
                {
                    footer1 = "0;loyalpayment;loyal payment;1;-" + activeCheck.CardSum.ToString() + ";-" + activeCheck.CardSum.ToString();
                    outputFile.WriteLine(footer1);
                }

                footer2 = "0;endofcheque;endofcheque;0;0;0";
                outputFile.WriteLine(footer2);
            }
            return true;
        }

        private void TextBoxCouponCode_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (activeCheck.couponMode)
            {
                if (e.Key == Key.End)
                {
                    textBoxCouponCode.Text += "1";
                    return;
                }
                else if (e.Key == Key.Down)
                {
                    textBoxCouponCode.Text += "2";
                    return;
                }
                else if (e.Key == Key.Next)
                {
                    textBoxCouponCode.Text += "3";
                    return;
                }
                else if (e.Key == Key.Left)
                {
                    textBoxCouponCode.Text += "4";
                    return;
                }
                else if (e.Key == Key.Clear)
                {
                    textBoxCouponCode.Text += "5";
                    return;
                }
                else if (e.Key == Key.Right)
                {
                    textBoxCouponCode.Text += "6";
                    return;
                }
                else if (e.Key == Key.Home)
                {
                    textBoxCouponCode.Text += "7";
                    return;
                }
                else if (e.Key == Key.Up)
                {
                    textBoxCouponCode.Text += "8";
                    return;
                }
                else if (e.Key == Key.PageUp)
                {
                    textBoxCouponCode.Text += "9";
                    return;
                }
                else if (e.Key == Key.Insert)
                {
                    textBoxCouponCode.Text += "0";
                    return;
                }

                textBoxCouponCode.CaretIndex = textBoxCouponCode.Text.Length;
            }
        }

        public void PrintSubHeaderWithSpecificationOPOS()
        {
            Printer.SetRegularFont();
            Printer.BeginTransaction();
            if (!Properties.Settings.Default.FontB)
            {
                Printer.PrintString(Printer.FormatTwoStrings("KASSIR: " + Vars.cashierName, " POS № " + Properties.Settings.Default.POSNumber));
                activeCheck.PrepareCheck();
                string date = activeCheck.date.Substring(6, 2) + "/" + activeCheck.date.Substring(4, 2) + "/" + activeCheck.date.Substring(0, 4);
                string time = activeCheck.time.Substring(0, 2) + ":" + activeCheck.time.Substring(2, 2) + ":" + activeCheck.time.Substring(4, 2);

                if (activeCheck.returnMode)
                    Printer.PrintString(Printer.FormatTwoStrings("QAYTAR CHEKI № " + activeCheck.number, date + " " + time));
                else
                    Printer.PrintString(Printer.FormatTwoStrings("SAVDO CHEKI № " + activeCheck.number, date + " " + time));

                if (width == 38)
                    Printer.PrintString("--------------------------------------");
                else if (width == 44)
                    Printer.PrintString("--------------------------------------------");

                for (int i = 0; i < activeCheck.specification.Count; i++)
                {
                    string itemToPrint = Printer.FormatString(activeCheck.specification[i]);
                    Printer.PrintString(itemToPrint);
                }

                if (width == 38)
                {
                    Printer.SetBoldFont();
                    Printer.PrintString("--------------------------------------");
                }
                else if (width == 44)
                    Printer.PrintString("--------------------------------------------");
            }
            else
            {
                Printer.FormatAndPrintTwoStringsB("KASSIR: " + Vars.cashierName, " POS № " + Properties.Settings.Default.POSNumber);
                activeCheck.PrepareCheck();
                string date = activeCheck.date.Substring(6, 2) + "/" + activeCheck.date.Substring(4, 2) + "/" + activeCheck.date.Substring(0, 4);
                string time = activeCheck.time.Substring(0, 2) + ":" + activeCheck.time.Substring(2, 2) + ":" + activeCheck.time.Substring(4, 2);

                if (activeCheck.returnMode)
                    Printer.FormatAndPrintTwoStringsB("QAYTAR CHEKI № " + activeCheck.number, date + " " + time);
                else
                    Printer.FormatAndPrintTwoStringsB("SAVDO CHEKI № " + activeCheck.number, date + " " + time);

                Printer.CenterAndPrintTextB("----------------------------------------------------------------");

                for (int i = 0; i < activeCheck.specification.Count; i++)
                {
                    Printer.FormatAndPrintStringB(activeCheck.specification[i]);
                }

                Printer.CenterAndPrintTextB("----------------------------------------------------------------");
            }
            Printer.EndTransaction();
        }

        public void PrintSubHeaderWithSpecificationWindows()
        {
            string description;
            double qty;
            double price;
            double sum;
            int VATRate;
            double VATSum;

            //MessageBox.Show(printDialog.PrintableAreaWidth.ToString());               //  == 301.9
            visual = new DrawingVisual();

            double leftMargin = Properties.Settings.Default.LeftMargin;
            double rightMargin = Properties.Settings.Default.RightMargin;

            double printableAreaWidth = printDialog.PrintableAreaWidth;

            FormattedText descriptionText;
            FormattedText sumText;
            Point point;

            printedHeight = 0;

            // Получить контекст рисования
            dc = visual.RenderOpen();

            descriptionText = new FormattedText("INN: " + Properties.Settings.Default.INN,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"), 10, Brushes.Black);
            descriptionText.MaxTextWidth = Properties.Settings.Default.DescriptionWidth;
            descriptionText.LineHeight = 10;
            descriptionText.TextAlignment = TextAlignment.Left;
            descriptionText.SetFontWeight(FontWeights.Bold);
            point = new Point(leftMargin, printedHeight);
            dc.DrawText(descriptionText, point);

            sumText = new FormattedText("S/N " + Properties.Settings.Default.SerialNumber,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"), 10, Brushes.Black);
            sumText.MaxTextWidth = Properties.Settings.Default.SumWidth;
            sumText.LineHeight = 10;
            sumText.TextAlignment = TextAlignment.Right;
            sumText.SetFontWeight(FontWeights.Bold);
            point = new Point(printableAreaWidth - rightMargin - Properties.Settings.Default.SumWidth, printedHeight);
            dc.DrawText(sumText, point);

            printedHeight += descriptionText.Height;

            descriptionText = new FormattedText("KASSIR: " + Vars.cashierName,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"), 10, Brushes.Black);
            descriptionText.MaxTextWidth = Properties.Settings.Default.DescriptionWidth;
            descriptionText.LineHeight = 10;
            descriptionText.TextAlignment = TextAlignment.Left;
            Size textSize = new Size(descriptionText.Width, descriptionText.Height);
            point = new Point(leftMargin, printedHeight);
            dc.DrawText(descriptionText, point);

            sumText = new FormattedText(" POS № " + Properties.Settings.Default.POSNumber,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"), 10, Brushes.Black);
            sumText.MaxTextWidth = Properties.Settings.Default.SumWidth;
            sumText.LineHeight = 10;
            sumText.TextAlignment = TextAlignment.Right;
            point = new Point(printableAreaWidth - rightMargin - Properties.Settings.Default.SumWidth, printedHeight);
            dc.DrawText(sumText, point);

            printedHeight += descriptionText.Height;

            activeCheck.PrepareCheck();
            string date = activeCheck.date.Substring(6, 2) + "/" + activeCheck.date.Substring(4, 2) + "/" + activeCheck.date.Substring(0, 4);
            string time = activeCheck.time.Substring(0, 2) + ":" + activeCheck.time.Substring(2, 2) + ":" + activeCheck.time.Substring(4, 2);

            if (activeCheck.returnMode)
            {
                descriptionText = new FormattedText("QAYTAR CHEKI № " + activeCheck.number,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"), 10, Brushes.Black);
            }
            else
            {
                descriptionText = new FormattedText("SAVDO CHEKI № " + activeCheck.number,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"), 10, Brushes.Black);
            }
            descriptionText.MaxTextWidth = Properties.Settings.Default.DescriptionWidth;
            descriptionText.LineHeight = 10;
            descriptionText.TextAlignment = TextAlignment.Left;
            point = new Point(leftMargin, printedHeight);
            dc.DrawText(descriptionText, point);

            sumText = new FormattedText(date + " " + time,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"), 10, Brushes.Black);
            sumText.MaxTextWidth = Properties.Settings.Default.SumWidth;
            sumText.LineHeight = 10;
            sumText.TextAlignment = TextAlignment.Right;
            point = new Point(printableAreaWidth - rightMargin - Properties.Settings.Default.SumWidth - 1, printedHeight);
            dc.DrawText(sumText, point);

            printedHeight += descriptionText.Height;

            Point point1 = new Point(leftMargin, printedHeight);
            Point point2 = new Point(printableAreaWidth - rightMargin, printedHeight);
            dc.DrawLine(new Pen(Brushes.Black, 1), point1, point2);

            printedHeight += 1;

            for (int i = 0; i < activeCheck.specification.Count; i++)
            {
                description = activeCheck.specification[i].Description;
                qty = activeCheck.specification[i].Quantity;
                price = activeCheck.specification[i].Price;
                sum = activeCheck.specification[i].Sum;
                VATRate = activeCheck.specification[i].VATRate;
                VATSum = activeCheck.specification[i].VATSum;

                descriptionText = new FormattedText(description,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"), 10, Brushes.Black);
                descriptionText.MaxTextWidth = Properties.Settings.Default.DescriptionWidth;
                descriptionText.LineHeight = 10;
                descriptionText.TextAlignment = TextAlignment.Left;
                if (sum < 0)
                    descriptionText.SetFontWeight(FontWeights.Bold);
                point = new Point(leftMargin, printedHeight);
                dc.DrawText(descriptionText, point);

                if (Math.Abs(qty) == 1)
                {
                    sumText = new FormattedText(sum.ToString() + " So'm",
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Arial"), 10, Brushes.Black);

                    sumText.SetFontWeight(FontWeights.Bold, 0, sum.ToString().Length);
                    sumText.MaxTextWidth = Properties.Settings.Default.SumWidth;
                    sumText.LineHeight = 10;
                    sumText.TextAlignment = TextAlignment.Right;
                    point = new Point(printableAreaWidth - rightMargin - Properties.Settings.Default.SumWidth, printedHeight);
                    dc.DrawText(sumText, point);
                }
                else
                {
                    sumText = new FormattedText(qty.ToString() + "*" + price.ToString() + "= " + sum.ToString() + " So'm",
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Arial"), 10, Brushes.Black);

                    sumText.SetFontWeight(FontWeights.Bold, 0, qty.ToString().Length);
                    sumText.SetFontWeight(FontWeights.Bold, (qty.ToString() + "*" + price.ToString() + "= ").Length, sum.ToString().Length);
                    sumText.MaxTextWidth = Properties.Settings.Default.SumWidth;
                    sumText.LineHeight = 10;
                    sumText.TextAlignment = TextAlignment.Right;
                    point = new Point(printableAreaWidth - rightMargin - Properties.Settings.Default.SumWidth, printedHeight);
                    dc.DrawText(sumText, point);
                }
                printedHeight += descriptionText.Height;

                descriptionText = new FormattedText("   в т.ч. НДС " + VATRate.ToString() + "% = " + VATSum.ToString(),
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"), 10, Brushes.Black);
                descriptionText.MaxTextWidth = Properties.Settings.Default.DescriptionWidth;
                descriptionText.LineHeight = 10;
                descriptionText.TextAlignment = TextAlignment.Left;
                descriptionText.SetFontStyle(FontStyles.Italic);
                point = new Point(leftMargin, printedHeight);
                dc.DrawText(descriptionText, point);
                printedHeight += descriptionText.Height;
            }
            point1 = new Point(leftMargin, printedHeight);
            point2 = new Point(printableAreaWidth - rightMargin, printedHeight);
            dc.DrawLine(new Pen(Brushes.Black, 1), point1, point2);

            printedHeight += 2;
        }

        public void PrintFooterAndHeaderOPOS()
        {
            //Printer.SetRegularFont();
            //Printer.PrintString(Printer.FormatTwoStrings("KASSIR: " + Vars.cashierName, " POS № 7"));
            //string date = activeCheck.date.Substring(6, 2) + "/" + activeCheck.date.Substring(4, 2) + "/" + activeCheck.date.Substring(0, 4);
            //string time = activeCheck.time.Substring(0, 2) + ":" + activeCheck.time.Substring(2, 2) + ":" + activeCheck.time.Substring(4, 2);
            //if (activeCheck.returnMode)
            //    Printer.PrintString(Printer.FormatTwoStrings("QAYTAR CHEKI № " + activeCheck.number, date + " " + time));
            //else
            //    Printer.PrintString(Printer.FormatTwoStrings("SOTMOQ CHEKI № " + activeCheck.number, date + " " + time));
            //Printer.PrintString("--------------------------------------");
            //for (int i = 0; i < activeCheck.specification.Count; i++)
            //{
            //    string itemToPrint = Printer.FormatString(activeCheck.specification[i]);
            //    Printer.PrintString(itemToPrint);
            //}

            //Printer.SetBoldFont();
            //Printer.PrintString("--------------------------------------");

            Printer.BeginTransaction();
            if (!Properties.Settings.Default.FontB)
            {
                if (activeCheck.returnMode)
                {
                    //Printer.PrintString("\x1b|bC\x1b|3C" + Printer.FormatTwoStrings("XAMMASI (К ВЫПЛАТЕ):", activeCheck.TopaySum.ToString() + " So'm"));
                    Printer.PrintString("\x1b|1C" + Printer.FormatTwoStrings("XAMMASI (К ВЫПЛАТЕ):", activeCheck.TopaySum.ToString() + " So'm"));
                    Printer.PrintString(Printer.FormatTwoStrings("TO'LANDI (ВЫДАНО):", activeCheck.CashSum.ToString() + " So'm"));
                    if (width == 38)
                    {
                        Printer.PrintString("--------------------------------------");
                        Printer.PrintString("          RAXMAT! - СПАСИБО!          ");
                        Printer.PrintString("");
                        if (Vars.fiscalMode)
                            Printer.PrintString("                 -FR-                 ");
                        else
                            Printer.PrintString("                -NFR-                 ");
                        Printer.PrintString(Printer.CenterText(Properties.Settings.Default.CompanyName));
                        Printer.PrintString(Printer.CenterText(Properties.Settings.Default.CompanyAddress));
                        Printer.CutPaper();
                        Printer.PrintString(Printer.FormatTwoStrings("INN: " + Properties.Settings.Default.INN, "S/N " + Properties.Settings.Default.SerialNumber));
                    }
                    else if (width == 44)
                    {
                        Printer.PrintString("\x1b|bC" + "--------------------------------------------");
                        Printer.PrintString("\x1b|bC" + "             RAXMAT! - СПАСИБО!             ");
                        Printer.PrintString("");
                        if (Vars.fiscalMode)
                            Printer.PrintString("\x1b|bC" + "                    -FR-                    ");
                        else
                            Printer.PrintString("\x1b|bC" + "                   -NFR-                    ");
                        Printer.PrintString("\x1b|bC" + Printer.CenterText(Properties.Settings.Default.CompanyName));
                        Printer.PrintString("\x1b|bC" + Printer.CenterText(Properties.Settings.Default.CompanyAddress));
                        Printer.CutPaper();
                        Printer.PrintString("\x1b|bC" + Printer.FormatTwoStrings("INN: " + Properties.Settings.Default.INN, "S/N " + Properties.Settings.Default.SerialNumber));
                    }
                }
                else
                {
                    if (width == 38)
                    {
                        //Printer.PrintString("\x1b|bC\x1b|3C" + Printer.FormatTwoStrings("XAMMASI (К ОПЛАТЕ):", activeCheck.TopaySum.ToString() + " So'm"));
                        Printer.PrintString("\x1b|1C" + Printer.FormatTwoStrings("XAMMASI (К ОПЛАТЕ):", activeCheck.TopaySum.ToString() + " So'm"));
                        if (activeCheck.VAT7 != 0)
                            Printer.PrintString(Printer.FormatTwoStrings("S.J. QQS (В Т.Ч. НДС) 7%:", activeCheck.VAT7.ToString() + " So'm"));
                        if (activeCheck.VAT10 != 0)
                            Printer.PrintString(Printer.FormatTwoStrings("S.J. QQS (В Т.Ч. НДС) 10%:", activeCheck.VAT10.ToString() + " So'm"));
                        Printer.PrintString(Printer.FormatTwoStrings("S.J. QQS (В Т.Ч. НДС) 15%:", activeCheck.VAT15.ToString() + " So'm"));
                        if (activeCheck.VAT20 != 0)
                            Printer.PrintString(Printer.FormatTwoStrings("S.J. QQS (В Т.Ч. НДС) 20%:", activeCheck.VAT20.ToString() + " So'm"));
                        Printer.PrintString(Printer.FormatTwoStrings("TO'LANDI (ОПЛАЧЕНО):", activeCheck.CashSum.ToString() + " So'm"));
                        if (activeCheck.CardSum > 0)
                            Printer.PrintString(Printer.FormatTwoStrings("LOYAL (КАРТОЙ ЛОЯЛЬНОСТИ):", activeCheck.CardSum.ToString() + " So'm"));
                        if (activeCheck.ChangeSum != 0)
                            Printer.PrintString(Printer.FormatTwoStrings("QAYTIM (СДАЧА):", activeCheck.ChangeSum.ToString() + " So'm"));
                        if (activeCheck.loyalty.Code != null)
                        {
                            Printer.PrintString("--------------------------------------");
                            Printer.PrintString(Printer.FormatTwoStrings("LOYAL:", activeCheck.loyalty.Sums.ToString() + "+" + activeCheck.loyalty.SumAdd.ToString() + "-" + activeCheck.loyalty.SumSub.ToString() + "=" + activeCheck.loyalty.SumNew.ToString()));
                        }
                        Printer.PrintString("--------------------------------------");
                        Printer.PrintString("          RAXMAT! - СПАСИБО!          ");
                        Printer.PrintString("");
                        if (Vars.fiscalMode)
                            Printer.PrintString("                 -FR-                 ");
                        else
                            Printer.PrintString("                -NFR-                 ");
                        //Printer.PrintRegistanBitmap();
                        //Printer.PrintString("--------------------------------------");
                        //Printer.CutPaper();
                        Printer.PrintString(Printer.CenterText(Properties.Settings.Default.CompanyName));
                        Printer.PrintString(Printer.CenterText(Properties.Settings.Default.CompanyAddress));
                        Printer.CutPaper();
                        Printer.PrintString(Printer.FormatTwoStrings("INN: " + Properties.Settings.Default.INN, "S/N " + Properties.Settings.Default.SerialNumber));
                    }
                    else if (width == 44)
                    {
                        //Printer.PrintString("\x1b|bC\x1b|3C" + Printer.FormatTwoStrings("XAMMASI (К ОПЛАТЕ):", activeCheck.TopaySum.ToString() + " So'm"));
                        Printer.PrintString("\x1b|1C" + Printer.FormatTwoStrings("XAMMASI (К ОПЛАТЕ):", activeCheck.TopaySum.ToString() + " So'm"));
                        if (activeCheck.VAT7 != 0)
                            //Printer.PrintString("\x1b|bC\x1b|1C" + Printer.FormatTwoStrings("S.J. QQS (В Т.Ч. НДС) 7%:", activeCheck.VAT7.ToString() + " So'm"));
                            Printer.PrintString("\x1b|1C" + Printer.FormatTwoStrings("S.J. QQS (В Т.Ч. НДС) 7%:", activeCheck.VAT7.ToString() + " So'm"));
                        if (activeCheck.VAT10 != 0)
                            //Printer.PrintString("\x1b|bC" + Printer.FormatTwoStrings("S.J. QQS (В Т.Ч. НДС) 10%:", activeCheck.VAT10.ToString() + " So'm"));
                            Printer.PrintString("\x1b|1C" + Printer.FormatTwoStrings("S.J. QQS (В Т.Ч. НДС) 10%:", activeCheck.VAT10.ToString() + " So'm"));
                        Printer.PrintString("\x1b|bC" + Printer.FormatTwoStrings("S.J. QQS (В Т.Ч. НДС) 15%:", activeCheck.VAT15.ToString() + " So'm"));
                        if (activeCheck.VAT20 != 0)
                            //Printer.PrintString("\x1b|bC" + Printer.FormatTwoStrings("S.J. QQS (В Т.Ч. НДС) 20%:", activeCheck.VAT20.ToString() + " So'm"));
                            Printer.PrintString("\x1b|1C" + Printer.FormatTwoStrings("S.J. QQS (В Т.Ч. НДС) 20%:", activeCheck.VAT20.ToString() + " So'm"));
                        //Printer.PrintString("\x1b|bC" + Printer.FormatTwoStrings("TO'LANDI (ОПЛАЧЕНО):", activeCheck.CashSum.ToString() + " So'm"));
                        Printer.PrintString("\x1b|1C" + Printer.FormatTwoStrings("TO'LANDI (ОПЛАЧЕНО):", activeCheck.CashSum.ToString() + " So'm"));
                        if (activeCheck.CardSum > 0)
                            //Printer.PrintString("\x1b|bC" + Printer.FormatTwoStrings("LOYAL (КАРТОЙ ЛОЯЛЬНОСТИ):", activeCheck.CardSum.ToString() + " So'm"));
                            Printer.PrintString("\x1b|1C" + Printer.FormatTwoStrings("LOYAL (КАРТОЙ ЛОЯЛЬНОСТИ):", activeCheck.CardSum.ToString() + " So'm"));
                        if (activeCheck.ChangeSum != 0)
                            //Printer.PrintString("\x1b|bC" + Printer.FormatTwoStrings("QAYTIM (СДАЧА):", activeCheck.ChangeSum.ToString() + " So'm"));
                            Printer.PrintString("\x1b|1C" + Printer.FormatTwoStrings("QAYTIM (СДАЧА):", activeCheck.ChangeSum.ToString() + " So'm"));
                        if (activeCheck.loyalty.Code != null)
                        {
                            Printer.PrintString("--------------------------------------------");
                            //Printer.PrintString("\x1b|bC" + Printer.FormatTwoStrings("LOYAL:", activeCheck.loyalty.Sums.ToString() + "+" + activeCheck.loyalty.SumAdd.ToString() + "-" + activeCheck.loyalty.SumSub.ToString() + "=" + activeCheck.loyalty.SumNew.ToString()));
                            Printer.PrintString("\x1b|1C" + Printer.FormatTwoStrings("LOYAL:", activeCheck.loyalty.Sums.ToString() + "+" + activeCheck.loyalty.SumAdd.ToString() + "-" + activeCheck.loyalty.SumSub.ToString() + "=" + activeCheck.loyalty.SumNew.ToString()));
                        }
                        Printer.PrintString("--------------------------------------------");
                        Printer.PrintString("\x1b|bC" + "             RAXMAT! - СПАСИБО!             ");
                        Printer.PrintString("");
                        if (Vars.fiscalMode)
                            Printer.PrintString("\x1b|bC" + "                    -FR-                    ");
                        else
                            Printer.PrintString("\x1b|bC" + "                   -NFR-                    ");
                        Printer.PrintString("\x1b|bC" + Printer.CenterText(Properties.Settings.Default.CompanyName));
                        Printer.PrintString("\x1b|bC" + Printer.CenterText(Properties.Settings.Default.CompanyAddress));
                        Printer.CutPaper();
                        Printer.PrintString("\x1b|bC" + Printer.FormatTwoStrings("INN: " + Properties.Settings.Default.INN, "S/N " + Properties.Settings.Default.SerialNumber));
                    }
                }
            }
            else
            {
                if (activeCheck.returnMode)
                {
                    //Printer.FormatAndPrintTwoStringsB("\x1b|bC\x1b|3CXAMMASI (К ВЫПЛАТЕ):", "\x1b|bC\x1b|3C" + activeCheck.TopaySum.ToString() + " So'm");
                    Printer.FormatAndPrintTwoStringsB("\x1b|1CXAMMASI (К ВЫПЛАТЕ):", "\x1b|1C" + activeCheck.TopaySum.ToString() + " So'm");
                    Printer.FormatAndPrintTwoStringsB("\x1b|1CTO'LANDI (ВЫДАНО):", "\x1b|1C" + activeCheck.CashSum.ToString() + " So'm");
                    Printer.CenterAndPrintTextB("----------------------------------------------------------------");
                    Printer.CenterAndPrintTextB("\x1b|bC" + "RAXMAT! - СПАСИБО!");
                    Printer.CenterAndPrintTextB("");
                    if (Vars.fiscalMode)
                        Printer.CenterAndPrintTextB("\x1b|bC-FR-");
                    else
                        Printer.CenterAndPrintTextB("\x1b|bC-NFR-");
                    Printer.CenterAndPrintTextB("\x1b|bC" + Printer.CenterText(Properties.Settings.Default.CompanyName));
                    Printer.CutPaper();
                    Printer.CenterAndPrintTextB("\x1b|bC" + Printer.CenterText(Properties.Settings.Default.CompanyAddress));
                    Printer.FormatAndPrintTwoStringsB("INN: " + Properties.Settings.Default.INN, "S/N " + Properties.Settings.Default.SerialNumber);
                }
                else
                {
                    //Printer.FormatAndPrintTwoStringsB("\x1b|bC\x1b|3CXAMMASI (К ОПЛАТЕ):", "\x1b|bC\x1b|3C" + activeCheck.TopaySum.ToString() + " So'm");
                    Printer.FormatAndPrintTwoStringsB("\x1b|1CXAMMASI (К ОПЛАТЕ):", "\x1b|1C" + activeCheck.TopaySum.ToString() + " So'm");
                    if (activeCheck.VAT7 != 0)
                        Printer.FormatAndPrintTwoStringsB("\x1b|1CS.J. QQS (В Т.Ч. НДС) 7%:", "\x1b|1C" + activeCheck.VAT7.ToString() + " So'm");
                    if (activeCheck.VAT10 != 0)
                        Printer.FormatTwoStrings("\x1b|1CS.J. QQS (В Т.Ч. НДС) 10%:", "\x1b|1C" + activeCheck.VAT10.ToString() + " So'm");
                    Printer.FormatAndPrintTwoStringsB("\x1b|1CS.J. QQS (В Т.Ч. НДС) 15%:", "\x1b|1C" + activeCheck.VAT15.ToString() + " So'm");
                    if (activeCheck.VAT20 != 0)
                        Printer.FormatAndPrintTwoStringsB("\x1b|1CS.J. QQS (В Т.Ч. НДС) 20%:", "\x1b|1C" + activeCheck.VAT20.ToString() + " So'm");
                    Printer.FormatAndPrintTwoStringsB("\x1b|1CTO'LANDI (ОПЛАЧЕНО):", "\x1b|1C" + activeCheck.CashSum.ToString() + " So'm");
                    if (activeCheck.CardSum > 0)
                        Printer.FormatAndPrintTwoStringsB("\x1b|1CLOYAL (КАРТОЙ ЛОЯЛЬНОСТИ):", "\x1b|1C" + activeCheck.CardSum.ToString() + " So'm");
                    if (activeCheck.ChangeSum != 0)
                        Printer.FormatAndPrintTwoStringsB("\x1b|1CQAYTIM (СДАЧА):", "\x1b|1C" + activeCheck.ChangeSum.ToString() + " So'm");
                    if (activeCheck.loyalty.Code != null)
                    {
                        Printer.CenterAndPrintTextB("----------------------------------------------------------------");
                        Printer.FormatAndPrintTwoStringsB("\x1b|bCLOYAL:", "\x1b|1C" + activeCheck.loyalty.Sums.ToString() + "+" + activeCheck.loyalty.SumAdd.ToString() + "-" + activeCheck.loyalty.SumSub.ToString() + "=" + activeCheck.loyalty.SumNew.ToString());
                    }
                    Printer.CenterAndPrintTextB("----------------------------------------------------------------");
                    Printer.CenterAndPrintTextB("\x1b|bC" + "RAXMAT! - СПАСИБО!");
                    Printer.CenterAndPrintTextB("");
                    if (Vars.fiscalMode)
                        Printer.CenterAndPrintTextB("\x1b|bC-FR-");
                    else
                        Printer.CenterAndPrintTextB("\x1b|bC-NFR-");
                    Printer.CenterAndPrintTextB("\x1b|bC" + Printer.CenterText(Properties.Settings.Default.CompanyName));
                    Printer.CutPaper();
                    Printer.CenterAndPrintTextB("\x1b|bC" + Printer.CenterText(Properties.Settings.Default.CompanyAddress));
                    Printer.FormatAndPrintTwoStringsB("INN: " + Properties.Settings.Default.INN, "S/N " + Properties.Settings.Default.SerialNumber);
                }
            }
            Printer.EndTransaction();
        }

        public void PrintFooterAndHeaderWindows()
        {
            FormattedText descriptionText;

            Point point1;
            Point point2;

            double leftMargin = Properties.Settings.Default.LeftMargin;
            double rightMargin = Properties.Settings.Default.RightMargin;

            double printableAreaWidth = printDialog.PrintableAreaWidth;

            if (activeCheck.returnMode)
                descriptionText = new FormattedText("XAMMASI (К ВЫПЛАТЕ): ",
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"), 12, Brushes.Black);
            else
                descriptionText = new FormattedText("XAMMASI (К ОПЛАТЕ): ",
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"), 12, Brushes.Black);
            descriptionText.MaxTextWidth = Properties.Settings.Default.DescriptionWidth;
            descriptionText.LineHeight = 10;
            descriptionText.TextAlignment = TextAlignment.Left;
            descriptionText.SetFontWeight(FontWeights.Bold);
            Size textSize = new Size(descriptionText.Width, descriptionText.Height);
            Point point = new Point(leftMargin, printedHeight);
            dc.DrawText(descriptionText, point);

            FormattedText sumText = new FormattedText("≡" + activeCheck.TopaySum.ToString() + " So'm",
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"), 12, Brushes.Black);
            sumText.MaxTextWidth = Properties.Settings.Default.SumWidth;
            sumText.LineHeight = 10;
            sumText.TextAlignment = TextAlignment.Right;
            sumText.SetFontWeight(FontWeights.Bold);
            point = new Point(printableAreaWidth - rightMargin - Properties.Settings.Default.SumWidth, printedHeight);
            dc.DrawText(sumText, point);

            printedHeight += descriptionText.Height;

            descriptionText = new FormattedText("В Т.Ч. НДС 15%:",
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"), 10, Brushes.Black);
            descriptionText.MaxTextWidth = Properties.Settings.Default.DescriptionWidth;
            descriptionText.LineHeight = 10;
            descriptionText.TextAlignment = TextAlignment.Left;
            descriptionText.SetFontWeight(FontWeights.Bold);
            point = new Point(leftMargin, printedHeight);
            dc.DrawText(descriptionText, point);

            sumText = new FormattedText(activeCheck.VAT15.ToString() + " So'm",
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"), 10, Brushes.Black);
            sumText.MaxTextWidth = Properties.Settings.Default.SumWidth;
            sumText.LineHeight = 10;
            sumText.TextAlignment = TextAlignment.Right;
            sumText.SetFontWeight(FontWeights.Bold);
            point = new Point(printableAreaWidth - rightMargin - Properties.Settings.Default.SumWidth, printedHeight);
            dc.DrawText(sumText, point);

            printedHeight += descriptionText.Height;

            if (activeCheck.returnMode)
                descriptionText = new FormattedText("TO'LANDI (ВЫДАНО):",
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"), 10, Brushes.Black);
            else
                descriptionText = new FormattedText("TO'LANDI (ОПЛАЧЕНО):",
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"), 10, Brushes.Black);
            descriptionText.MaxTextWidth = Properties.Settings.Default.DescriptionWidth;
            descriptionText.LineHeight = 10;
            descriptionText.TextAlignment = TextAlignment.Left;
            descriptionText.SetFontWeight(FontWeights.Bold);
            point = new Point(leftMargin, printedHeight);
            dc.DrawText(descriptionText, point);

            sumText = new FormattedText(activeCheck.CashSum.ToString() + " So'm",
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"), 10, Brushes.Black);
            sumText.MaxTextWidth = Properties.Settings.Default.SumWidth;
            sumText.LineHeight = 10;
            sumText.TextAlignment = TextAlignment.Right;
            sumText.SetFontWeight(FontWeights.Bold);
            point = new Point(printableAreaWidth - rightMargin - Properties.Settings.Default.SumWidth, printedHeight);
            dc.DrawText(sumText, point);

            printedHeight += descriptionText.Height;

            if (activeCheck.CardSum > 0)
            {
                descriptionText = new FormattedText("LOYAL (КАРТОЙ ЛОЯЛЬНОСТИ):",
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"), 10, Brushes.Black);
                descriptionText.MaxTextWidth = Properties.Settings.Default.DescriptionWidth;
                descriptionText.LineHeight = 10;
                descriptionText.TextAlignment = TextAlignment.Left;
                descriptionText.SetFontWeight(FontWeights.Bold);
                point = new Point(leftMargin, printedHeight);
                dc.DrawText(descriptionText, point);

                sumText = new FormattedText(activeCheck.CardSum.ToString() + " So'm",
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"), 10, Brushes.Black);
                sumText.MaxTextWidth = Properties.Settings.Default.SumWidth;
                sumText.LineHeight = 10;
                sumText.TextAlignment = TextAlignment.Right;
                sumText.SetFontWeight(FontWeights.Bold);
                point = new Point(printableAreaWidth - rightMargin - Properties.Settings.Default.SumWidth, printedHeight);
                dc.DrawText(sumText, point);

                printedHeight += descriptionText.Height;
            }

            if (activeCheck.ChangeSum != 0)
            {
                descriptionText = new FormattedText("QAYTIM (СДАЧА):",
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"), 10, Brushes.Black);
                descriptionText.MaxTextWidth = Properties.Settings.Default.DescriptionWidth;
                descriptionText.LineHeight = 10;
                descriptionText.TextAlignment = TextAlignment.Left;
                descriptionText.SetFontWeight(FontWeights.Bold);
                point = new Point(leftMargin, printedHeight);
                dc.DrawText(descriptionText, point);

                sumText = new FormattedText(activeCheck.ChangeSum.ToString() + " So'm",
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"), 10, Brushes.Black);
                sumText.MaxTextWidth = Properties.Settings.Default.SumWidth;
                sumText.LineHeight = 10;
                sumText.TextAlignment = TextAlignment.Right;
                sumText.SetFontWeight(FontWeights.Bold);
                point = new Point(printableAreaWidth - rightMargin - Properties.Settings.Default.SumWidth, printedHeight);
                dc.DrawText(sumText, point);

                printedHeight += descriptionText.Height;
            }

            if (activeCheck.loyalty.Code != null)
            {
                point1 = new Point(leftMargin, printedHeight);
                point2 = new Point(printableAreaWidth - rightMargin, printedHeight);
                dc.DrawLine(new Pen(Brushes.Black, 1), point1, point2);

                printedHeight += 1;

                descriptionText = new FormattedText("LOYAL:",
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"), 10, Brushes.Black);
                descriptionText.MaxTextWidth = Properties.Settings.Default.DescriptionWidth;
                descriptionText.LineHeight = 10;
                descriptionText.TextAlignment = TextAlignment.Left;
                descriptionText.SetFontWeight(FontWeights.Bold);
                point = new Point(leftMargin, printedHeight);
                dc.DrawText(descriptionText, point);

                sumText = new FormattedText(activeCheck.loyalty.Sums.ToString() + "+" + activeCheck.loyalty.SumAdd.ToString() + "-" + activeCheck.loyalty.SumSub.ToString() + "=" + activeCheck.loyalty.SumNew.ToString(),
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"), 10, Brushes.Black);
                sumText.MaxTextWidth = Properties.Settings.Default.SumWidth;
                sumText.LineHeight = 10;
                sumText.TextAlignment = TextAlignment.Right;
                sumText.SetFontWeight(FontWeights.Bold);
                point = new Point(printableAreaWidth - rightMargin - Properties.Settings.Default.SumWidth, printedHeight);
                dc.DrawText(sumText, point);

                printedHeight += descriptionText.Height;
            }

            point1 = new Point(leftMargin, printedHeight);
            point2 = new Point(printableAreaWidth - rightMargin, printedHeight);
            dc.DrawLine(new Pen(Brushes.Black, 1), point1, point2);

            printedHeight += 1;

            descriptionText = new FormattedText("RAXMAT! - СПАСИБО!",
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"), 10, Brushes.Black);
            descriptionText.MaxTextWidth = printableAreaWidth - leftMargin - rightMargin;
            descriptionText.LineHeight = 10;
            descriptionText.TextAlignment = TextAlignment.Center;
            descriptionText.SetFontWeight(FontWeights.Bold);
            point = new Point(leftMargin, printedHeight);
            dc.DrawText(descriptionText, point);
            printedHeight += descriptionText.Height;

            if (Vars.fiscalMode)
            {
                descriptionText = new FormattedText("-FR-",
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"), 10, Brushes.Black);
            }
            else
            {
                descriptionText = new FormattedText("-NFR-",
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"), 10, Brushes.Black);
            }
            descriptionText.MaxTextWidth = printableAreaWidth - leftMargin - rightMargin;
            descriptionText.LineHeight = 10;
            descriptionText.TextAlignment = TextAlignment.Center;
            descriptionText.SetFontWeight(FontWeights.Bold);
            point = new Point(leftMargin, printedHeight);
            dc.DrawText(descriptionText, point);
            printedHeight += descriptionText.Height;

            descriptionText = new FormattedText(".",
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"), 10, Brushes.Black);
            descriptionText.MaxTextWidth = printableAreaWidth - leftMargin - rightMargin;
            descriptionText.LineHeight = 10;
            descriptionText.TextAlignment = TextAlignment.Center;
            descriptionText.SetFontWeight(FontWeights.Bold);
            point = new Point(leftMargin, printedHeight);
            dc.DrawText(descriptionText, point);
            printedHeight += descriptionText.Height;

            descriptionText = new FormattedText(Properties.Settings.Default.CompanyName,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"), 10, Brushes.Black);
            descriptionText.MaxTextWidth = printableAreaWidth - leftMargin - rightMargin;
            descriptionText.LineHeight = 10;
            descriptionText.TextAlignment = TextAlignment.Center;
            descriptionText.SetFontWeight(FontWeights.Bold);
            point = new Point(leftMargin, printedHeight);
            dc.DrawText(descriptionText, point);
            printedHeight += descriptionText.Height;

            descriptionText = new FormattedText(Properties.Settings.Default.CompanyAddress,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"), 10, Brushes.Black);
            descriptionText.MaxTextWidth = printableAreaWidth - leftMargin - rightMargin;
            descriptionText.LineHeight = 10;
            descriptionText.TextAlignment = TextAlignment.Center;
            descriptionText.SetFontWeight(FontWeights.Bold);
            point = new Point(leftMargin, printedHeight);
            dc.DrawText(descriptionText, point);
            printedHeight += descriptionText.Height;

            dc.Close();
            printDialog.PrintVisual(visual, "Печать с помощью классов визуального уровня");
        }

        public void PrintFooterAndHeaderAnnulationWindows()
        {
            FormattedText descriptionText;

            Point point;
            //Point point1;
            //Point point2;

            double leftMargin = Properties.Settings.Default.LeftMargin;
            double rightMargin = Properties.Settings.Default.RightMargin;

            double printableAreaWidth = printDialog.PrintableAreaWidth;

            //point1 = new Point(leftMargin, printedHeight);
            //point2 = new Point(printableAreaWidth - rightMargin, printedHeight);
            //dc.DrawLine(new Pen(Brushes.Black, 1), point1, point2);
            //printedHeight += 1;

            descriptionText = new FormattedText("ЧЕК АННУЛИРОВАН",
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"), 10, Brushes.Black);
            descriptionText.MaxTextWidth = printableAreaWidth - leftMargin - rightMargin;
            descriptionText.LineHeight = 10;
            descriptionText.TextAlignment = TextAlignment.Center;
            descriptionText.SetFontWeight(FontWeights.Bold);
            point = new Point(leftMargin, printedHeight);
            dc.DrawText(descriptionText, point);
            printedHeight += descriptionText.Height;

            if (Vars.fiscalMode)
            {
                descriptionText = new FormattedText("-FR-",
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"), 10, Brushes.Black);
            }
            else
            {
                descriptionText = new FormattedText("-NFR-",
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"), 10, Brushes.Black);
            }
            descriptionText.MaxTextWidth = printableAreaWidth - leftMargin - rightMargin;
            descriptionText.LineHeight = 10;
            descriptionText.TextAlignment = TextAlignment.Center;
            descriptionText.SetFontWeight(FontWeights.Bold);
            point = new Point(leftMargin, printedHeight);
            dc.DrawText(descriptionText, point);
            printedHeight += descriptionText.Height;

            descriptionText = new FormattedText(".",
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"), 10, Brushes.Black);
            descriptionText.MaxTextWidth = printableAreaWidth - leftMargin - rightMargin;
            descriptionText.LineHeight = 10;
            descriptionText.TextAlignment = TextAlignment.Center;
            descriptionText.SetFontWeight(FontWeights.Bold);
            point = new Point(leftMargin, printedHeight);
            dc.DrawText(descriptionText, point);
            printedHeight += descriptionText.Height;

            descriptionText = new FormattedText(Properties.Settings.Default.CompanyName,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"), 10, Brushes.Black);
            descriptionText.MaxTextWidth = printableAreaWidth - leftMargin - rightMargin;
            descriptionText.LineHeight = 10;
            descriptionText.TextAlignment = TextAlignment.Center;
            descriptionText.SetFontWeight(FontWeights.Bold);
            point = new Point(leftMargin, printedHeight);
            dc.DrawText(descriptionText, point);
            printedHeight += descriptionText.Height;

            descriptionText = new FormattedText(Properties.Settings.Default.CompanyAddress,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"), 10, Brushes.Black);
            descriptionText.MaxTextWidth = printableAreaWidth - leftMargin - rightMargin;
            descriptionText.LineHeight = 10;
            descriptionText.TextAlignment = TextAlignment.Center;
            descriptionText.SetFontWeight(FontWeights.Bold);
            point = new Point(leftMargin, printedHeight);
            dc.DrawText(descriptionText, point);
            printedHeight += descriptionText.Height;

            dc.Close();
            printDialog.PrintVisual(visual, "Печать с помощью классов визуального уровня");
        }

        private void ActiveCheckWindow_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Topmost = true;
            Activate();
            //Focus();
            e.Handled = true;
        }
    }
}