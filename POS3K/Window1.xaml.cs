using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace POS3K
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public int width = 44;
        Fiscal fiscal = new Fiscal();

        //  объявляю таймер для проверки Autonomode
        public DispatcherTimer timer;
        //  объявляю таймер для обновления текущего времени
        public DispatcherTimer dateTimeTimer;

        static bool previousAutonomode = false;

        bool wasPressed = false;

        DoubleAnimation doubleAnimation;
        WindowAutorization windowAutorization;
        ProgressBarWindow progressBarWindow;

        SolidColorBrush animatedBrush = new SolidColorBrush()
        {
            Color = (Color)ColorConverter.ConvertFromString("#000025")
        };

        public Window1()
        {
            InitializeComponent();

            //  теперь экземпляр окна прогресса создается перед его непосредственным использованием, поэтому здесь комментарий
            //progressBarWindow = new ProgressBarWindow();

            try
            {
                width = Convert.ToInt32(Properties.Settings.Default.SymbolsPerLine);
            }
            catch { }
            //MessageBox.Show(Vars.cashierName);
            //RoutedCommand mainMenu1Command = new RoutedCommand();
            //mainMenu1Command.InputGestures.Add(new KeyGesture(Key.D1));
            //CommandBindings.Add(new CommandBinding(enterCommand, enterCommand_event_handler));
            //CommandBindings.Add(new CommandBinding(mainMenu1Command, mainMenu1Command_event_hadler));
            Activated += Window1_Activated;
            //btnMainMenu1.Loaded += BtnMainMenu1_Loaded;
            this.Cursor = Cursors.None;
        }

        //  проверка Autonomode
        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.IsEnabled = false;
            //  я думаю не стоит лишний раз терзать систему если источник проблемы найден
            //if (GC.GetTotalMemory(false) > 46000)
            //{
            //    GC.Collect();
            //    GC.WaitForPendingFinalizers();
            //    GC.Collect();
            //}
            if (Vars.autonomode)
            {
                if (previousAutonomode != Vars.autonomode)
                {
                    //border.Background = Brushes.Red;
                    border.Background = animatedBrush;
                    ColorAnimation colorAnimationlabelAutonomode = new ColorAnimation(System.Windows.Media.Colors.Yellow, new Duration(TimeSpan.FromSeconds(1)))
                    {
                        AutoReverse = true,
                        RepeatBehavior = RepeatBehavior.Forever
                    };
                    animatedBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimationlabelAutonomode, HandoffBehavior.SnapshotAndReplace);
                }
            }
            else
            {
                if (previousAutonomode != Vars.autonomode)
                {
                    animatedBrush.BeginAnimation(SolidColorBrush.ColorProperty, null);
                    border.Background = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x25));
                    //border.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000025"));
                }
            }

            previousAutonomode = Vars.autonomode;

            timer.IsEnabled = true;
        }

        private void BtnMainMenu1_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation doubleAnimation1 = new DoubleAnimation(230, 260, new Duration(TimeSpan.FromMilliseconds(300)));
            doubleAnimation1.AutoReverse = true;
            doubleAnimation1.RepeatBehavior = RepeatBehavior.Forever;
            btnMainMenu1.BeginAnimation(WidthProperty, doubleAnimation1);
            DoubleAnimation doubleAnimation2 = new DoubleAnimation(16, 18, new Duration(TimeSpan.FromMilliseconds(300)));
            doubleAnimation2.AutoReverse = true;
            doubleAnimation2.RepeatBehavior = RepeatBehavior.Forever;
            btnMainMenu1.BeginAnimation(FontSizeProperty, doubleAnimation2);
            DoubleAnimation doubleAnimation3 = new DoubleAnimation(35, 40, new Duration(TimeSpan.FromMilliseconds(300)));
            doubleAnimation3.AutoReverse = true;
            doubleAnimation3.RepeatBehavior = RepeatBehavior.Forever;
            btnMainMenu3.BeginAnimation(HeightProperty, doubleAnimation3);
        }

        private void Window1_Activated(object sender, EventArgs e)
        {
            //DoubleAnimation doubleAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.25)));
            //BeginAnimation(OpacityProperty, doubleAnimation);
            //btnMainMenu1.RaiseEvent(new RoutedEventArgs(LoadedEvent));
            //btnMainMenu2.RaiseEvent(new RoutedEventArgs(LoadedEvent));
            //btnMainMenu3.RaiseEvent(new RoutedEventArgs(LoadedEvent));
            //btnMainMenu4.RaiseEvent(new RoutedEventArgs(LoadedEvent));
            //btnMainMenu5.RaiseEvent(new RoutedEventArgs(LoadedEvent));
            //btnMainMenu6.RaiseEvent(new RoutedEventArgs(LoadedEvent));
            //btnMainMenu7.RaiseEvent(new RoutedEventArgs(LoadedEvent));
            //btnMainMenu8.RaiseEvent(new RoutedEventArgs(LoadedEvent));
            //btnMainMenu9.RaiseEvent(new RoutedEventArgs(LoadedEvent));

            Activated -= Window1_Activated;

            //  настраиваем и стартуем таймер проверки Autonomode
            if (timer == null)
                timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += Timer_Tick;
            timer.Start();

            if (dateTimeTimer == null)
                dateTimeTimer = new DispatcherTimer();
            dateTimeTimer.Interval = TimeSpan.FromMilliseconds(500);
            dateTimeTimer.Tick += DateTimeTimer_Tick;
            dateTimeTimer.Start();

            if (fiscal.OpenRead())
            {
                if (fiscal.GetHeader())
                {
                    if (fiscal.fiscalHeader.fiscalMode[0] != '1')
                    {
                        fiscal.Close();
                        labelLastZDateTime.Content = "";
                        Vars.fiscalMode = false;
                        return;
                    }
                    else
                    {
                        Vars.fiscalMode = true;
                    }
                    if (fiscal.GetUsers())
                    {
                        if (fiscal.GetLatestZ())
                        {
                            Vars.lastZDateTime = fiscal.fiscalData.closeDate.Substring(0, 2) + "." + fiscal.fiscalData.closeDate.Substring(2, 2) + "." + fiscal.fiscalData.closeDate.Substring(4, 4) + " " + fiscal.fiscalData.closeTime.Substring(0, 2) + ":" + fiscal.fiscalData.closeTime.Substring(2, 2) + ":" + fiscal.fiscalData.closeTime.Substring(4, 2);
                            labelLastZDateTime.Content = Vars.lastZDateTime;
                        }
                        else
                        {
                            fiscal.Close();
                            labelLastZDateTime.Content = "";
                            return;
                        }
                    }
                    else
                    {                        fiscal.Close();
                        labelLastZDateTime.Content = "";
                        return;
                    }
                }
                fiscal.Close();
            }
            else
            {
                MessageBox.Show("Ошибка фискального модуля!");
                return;
            }
        }

        //  обновление текущего времени
        private void DateTimeTimer_Tick(object sender, EventArgs e)
        {
            labelLastZDateTime.Content = Vars.lastZDateTime;
            string dateTimeString = DateTime.Now.ToString();
            labelDateTime.Content = dateTimeString;

            string hhmm = dateTimeString.Substring(11, 5);
            if (hhmm.Equals(Properties.Settings.Default.ShutDownTime))
            {
                System.Diagnostics.Process.Start("cmd", "/c shutdown -r -f -t 00");
            }
        }

        //private void mainMenu1Command_event_hadler(object sender, ExecutedRoutedEventArgs e)
        //{
        //    BtnMainMenu1_Click(sender, e);
        //}

        //  нажата кнопка btnMainMenu1 - $$$ €€€ £££ ¥¥¥ ¤¤¤
        private void BtnMainMenu1_Click(object sender, RoutedEventArgs e)
        {
            wasPressed = true;

            Vars.mainMenuSelected = 1;

                            //< DoubleAnimation From = "230" To = "420"
                            //                     Storyboard.TargetProperty = "Width"
                            //                     Duration = "0:0:00.25" />
                            //< DoubleAnimation From = "35" To = "75"
                            //                     Storyboard.TargetProperty = "Height"
                            //                     Duration = "0:0:00.25" />
                            //< DoubleAnimation From = "15" To = "25"
                            //                     Storyboard.TargetProperty = "FontSize"
                            //                     Duration = "0:0:00.25" />
                            //< ThicknessAnimation From = "40, 0, 0, 0" To = "75, 0, 0, 0"
                            //                     Storyboard.TargetProperty = "Padding"
                            //                     Duration = "0:0:00.25" />

            DoubleAnimation doubleAnimationWidth = new DoubleAnimation(230, 420, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimationWidth.AutoReverse = true;
            btnMainMenu1.BeginAnimation(WidthProperty, doubleAnimationWidth);

            DoubleAnimation doubleAnimationHeight = new DoubleAnimation(35, 75, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimationHeight.AutoReverse = true;
            btnMainMenu1.BeginAnimation(HeightProperty, doubleAnimationHeight);

            DoubleAnimation doubleAnimationFontSize = new DoubleAnimation(15, 25, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimationFontSize.AutoReverse = true;
            btnMainMenu1.BeginAnimation(FontSizeProperty, doubleAnimationFontSize);

            ThicknessAnimation thicknessAnimationPadding = new ThicknessAnimation(new Thickness(40, 0, 0, 0), new Thickness(75, 0, 0, 0), new Duration(TimeSpan.FromSeconds(0.25)));
            thicknessAnimationPadding.AutoReverse = true;
            btnMainMenu1.BeginAnimation(PaddingProperty, thicknessAnimationPadding);

            //  подготовим анимацию прозрачности для окна - за пол секунды окно станет совершенно прозрачным
            doubleAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
            //  по завершении анимации выполним функционал, для этого привяжем событие завершения анимации
            doubleAnimation.Completed += DoubleAnimationBtnMainMenu1_Completed;
            //  стартуем анимацию прозрачности
            BeginAnimation(OpacityProperty, doubleAnimation);
        }

        //  переходим в функционал кнопки $$$ €€€ £££ ¥¥¥ ¤¤¤ сразу после того как текущее окно (окно меню) стало прозрачным
        private void DoubleAnimationBtnMainMenu1_Completed(object sender, EventArgs e)
        {
            //  отвязываем данный метод от события завершения анимации
            doubleAnimation.Completed -= DoubleAnimationBtnMainMenu1_Completed;
            //  отвязываем события от таймера
            timer.Tick -= Timer_Tick;
            //  отвязываем события от таймера
            dateTimeTimer.Tick -= DateTimeTimer_Tick;

            wasPressed = false;

            //  если окно авторизации еще не создано (или было уничтожено)
            if (windowAutorization == null)
                //  создадим его
                windowAutorization = new WindowAutorization();

            //  покажем окно авторизации в модальном режиме
            windowAutorization.ShowDialog();
            //  после завершения диалога закроем окно авторизации
            windowAutorization.Close();
            //  и обнулим объект
            windowAutorization = null;
            //  если авторизация успешно пройдена (найден кассир с введенным кодом)
            if (Vars.cashierId != "")
            {
                //  если фискальный модуль успешно открыт (для чтения)
                if (fiscal.OpenRead())
                {
                    //  если успешно считан фискальный заголовок
                    if (fiscal.GetHeader())
                    {
                        //  если касса находится в нефискальном режиме
                        if (fiscal.fiscalHeader.fiscalMode[0] != '1')
                        {
                            fiscal.Close();
                            MessageBox.Show("Касса не фискализирована!");
                            Vars.fiscalMode = false;
                            return;
                        }
                        //  если касса находится в фискальном режиме
                        else
                        {
                            Vars.fiscalMode = true;
                        }
                        //  если потребительский блок считан успешно
                        if (fiscal.GetUsers())
                        {
                            //  если самый поздний Z-отчет успешно считан
                            if (fiscal.GetLatestZ())
                            {
                                //  если текущие датавремя превышают датувремя прошлого суточного отчета
                                if (!fiscal.CompareDateTime())
                                {
                                    fiscal.Close();
                                    MessageBox.Show("Не сделан суточный отчет!");
                                    //  снова сделаем это окно (окно меню) непрозрачным (видимым)
                                    doubleAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
                                    //  стартуем анимацию непрозрачности
                                    BeginAnimation(OpacityProperty, doubleAnimation);
                                    return;
                                }
                            }
                            //  если попытка считывания самого позднего суточного отчета оказалась неудачной
                            else
                            {
                                fiscal.Close();
                                MessageBox.Show("Ошибка чтения закрытия смены!");
                                return;
                            }
                        }
                        //  если попытка считывания потребительского блока оказалась неудачной
                        else
                        {
                            fiscal.Close();
                            MessageBox.Show("Ошибка чтения журнала регистраций!");
                            return;
                        }
                    }
                    fiscal.Close();
                }
                //  если попытка считывания фискального заголовка оказалась неудачной
                else
                {
                    MessageBox.Show("Ошибка фискального модуля!");
                    return;
                }
                //Window2 window2 = new Window2();
                //window2.Show();
                //progressBarWindow.Show();
                //Close();

                if (progressBarWindow == null || !progressBarWindow.IsLoaded)
                    progressBarWindow = new ProgressBarWindow();
                //  назначим владельцем окна прогресса это окно (окно меню)
                progressBarWindow.Owner = this;
                //  спрячем текущее окно
                Hide();
                //  остановим таймер
                timer.Stop();
                //  остановим таймер
                dateTimeTimer.Stop();
                //  покажем окно прогресса
                progressBarWindow.Show();
            }
            //  авторизация не пройдена
            else
            {
                //  снова сделаем это окно (окно меню) непрозрачным (видимым)
                doubleAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
                //  стартуем анимацию непрозрачности
                BeginAnimation(OpacityProperty, doubleAnimation);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (wasPressed)
                return;

            WindowAutorization windowAutorization = new WindowAutorization();

            if (e.Key.ToString() == "D1" || e.Key == Key.End)
            {
                btnMainMenu1.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            if (e.Key.ToString() == "D2" || e.Key == Key.Down)
            {
                btnMainMenu2.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            if (e.Key.ToString() == "D3" || e.Key == Key.Next)
            {
                btnMainMenu3.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            if (e.Key.ToString() == "D4" || e.Key == Key.Left)
            {
                btnMainMenu4.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            if (e.Key.ToString() == "D5" || e.Key == Key.Clear)
            {
                btnMainMenu5.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            if (e.Key.ToString() == "D6" || e.Key == Key.Right)
            {
                btnMainMenu6.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            if (e.Key.ToString() == "D7" || e.Key == Key.Home)
            {
                btnMainMenu7.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            if (e.Key.ToString() == "D8" || e.Key == Key.Up)
            {
                btnMainMenu8.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            if (e.Key.ToString() == "D9" || e.Key == Key.PageUp)
            {
                btnMainMenu9.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            Topmost = true;
        }

        //  btnMainMenu3 - Настройки
        private void BtnMainMenu3_Click(object sender, RoutedEventArgs e)
        {
            wasPressed = true;

            Vars.mainMenuSelected = 3;

            DoubleAnimation doubleAnimationWidth = new DoubleAnimation(230, 420, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimationWidth.AutoReverse = true;
            btnMainMenu3.BeginAnimation(WidthProperty, doubleAnimationWidth);

            DoubleAnimation doubleAnimationHeight = new DoubleAnimation(35, 75, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimationHeight.AutoReverse = true;
            btnMainMenu3.BeginAnimation(HeightProperty, doubleAnimationHeight);

            DoubleAnimation doubleAnimationFontSize = new DoubleAnimation(15, 25, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimationFontSize.AutoReverse = true;
            btnMainMenu3.BeginAnimation(FontSizeProperty, doubleAnimationFontSize);

            ThicknessAnimation thicknessAnimationPadding = new ThicknessAnimation(new Thickness(40, 0, 0, 0), new Thickness(75, 0, 0, 0), new Duration(TimeSpan.FromSeconds(0.25)));
            thicknessAnimationPadding.AutoReverse = true;
            btnMainMenu3.BeginAnimation(PaddingProperty, thicknessAnimationPadding);

            DoubleAnimation doubleAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
            doubleAnimation.Completed += DoubleAnimationBtnMainMenu3_Completed;
            BeginAnimation(OpacityProperty, doubleAnimation);
        }

        private void DoubleAnimationBtnMainMenu3_Completed(object sender, EventArgs e)
        {
            wasPressed = false;

            WindowAutorization windowAutorization = new WindowAutorization();
            windowAutorization.Mode = "";
            windowAutorization.ShowDialog();
            windowAutorization.Close();
            if (Vars.cashierStatus)
            {
                WindowSettings windowSettings = new WindowSettings();
                windowSettings.Owner = this;
                Hide();
                windowSettings.Show();
            }
            //else
            //    MessageBox.Show("Недостаточно прав!");
            else
            {
                //  снова сделаем это окно (окно меню) непрозрачным (видимым)
                doubleAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
                //  стартуем анимацию непрозрачности
                BeginAnimation(OpacityProperty, doubleAnimation);
            }
        }

        //  btnMainMenu6 - База данных кассы
        private void BtnMainMenu6_Click(object sender, RoutedEventArgs e)
        {
            wasPressed = true;

            Vars.mainMenuSelected = 6;

            DoubleAnimation doubleAnimationWidth = new DoubleAnimation(230, 420, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimationWidth.AutoReverse = true;
            btnMainMenu6.BeginAnimation(WidthProperty, doubleAnimationWidth);

            DoubleAnimation doubleAnimationHeight = new DoubleAnimation(35, 75, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimationHeight.AutoReverse = true;
            btnMainMenu6.BeginAnimation(HeightProperty, doubleAnimationHeight);

            DoubleAnimation doubleAnimationFontSize = new DoubleAnimation(15, 25, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimationFontSize.AutoReverse = true;
            btnMainMenu6.BeginAnimation(FontSizeProperty, doubleAnimationFontSize);

            ThicknessAnimation thicknessAnimationPadding = new ThicknessAnimation(new Thickness(40, 0, 0, 0), new Thickness(75, 0, 0, 0), new Duration(TimeSpan.FromSeconds(0.25)));
            thicknessAnimationPadding.AutoReverse = true;
            btnMainMenu6.BeginAnimation(PaddingProperty, thicknessAnimationPadding);

            DoubleAnimation doubleAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
            doubleAnimation.Completed += DoubleAnimationBtnMainMenu6_Completed;
            BeginAnimation(OpacityProperty, doubleAnimation);
        }

        private void DoubleAnimationBtnMainMenu6_Completed(object sender, EventArgs e)
        {
            wasPressed = false;

            WindowAutorization windowAutorization = new WindowAutorization();
            windowAutorization.ShowDialog();
            windowAutorization.Close();
            if (Vars.cashierStatus)
            {
                WindowChecks windowChecks = new WindowChecks();

                windowChecks.Owner = this;
                Hide();
                windowChecks.Show();
            }
            //else
            //    MessageBox.Show("Недостаточно прав!");
            else
            {
                //  снова сделаем это окно (окно меню) непрозрачным (видимым)
                doubleAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
                //  стартуем анимацию непрозрачности
                BeginAnimation(OpacityProperty, doubleAnimation);
            }
        }

        //  btnMainMenu7 - Закрыть программу
        private void BtnMainMenu7_Click(object sender, RoutedEventArgs e)
        {
            wasPressed = true;

            Vars.mainMenuSelected = 7;

            DoubleAnimation doubleAnimationWidth = new DoubleAnimation(230, 420, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimationWidth.AutoReverse = true;
            btnMainMenu7.BeginAnimation(WidthProperty, doubleAnimationWidth);

            DoubleAnimation doubleAnimationHeight = new DoubleAnimation(35, 75, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimationHeight.AutoReverse = true;
            btnMainMenu7.BeginAnimation(HeightProperty, doubleAnimationHeight);

            DoubleAnimation doubleAnimationFontSize = new DoubleAnimation(15, 25, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimationFontSize.AutoReverse = true;
            btnMainMenu7.BeginAnimation(FontSizeProperty, doubleAnimationFontSize);

            ThicknessAnimation thicknessAnimationPadding = new ThicknessAnimation(new Thickness(40, 0, 0, 0), new Thickness(75, 0, 0, 0), new Duration(TimeSpan.FromSeconds(0.25)));
            thicknessAnimationPadding.AutoReverse = true;
            btnMainMenu7.BeginAnimation(PaddingProperty, thicknessAnimationPadding);

            DoubleAnimation doubleAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
            doubleAnimation.Completed += DoubleAnimationBtnMainMenu7_Completed; ;
            BeginAnimation(OpacityProperty, doubleAnimation);
        }

        private void DoubleAnimationBtnMainMenu7_Completed(object sender, EventArgs e)
        {
            wasPressed = false;

            //Printer.PrintViaDriver();

            WindowAutorization windowAutorization = new WindowAutorization();
            windowAutorization.ShowDialog();
            windowAutorization.Close();
            if (Vars.cashierStatus)
            {
                //MainWindow.keyboard.Close();
                Close();
                System.Windows.Application.Current.Shutdown();
            }
            //else
            //    MessageBox.Show("Недостаточно прав!");
            else
            {
                //  снова сделаем это окно (окно меню) непрозрачным (видимым)
                doubleAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
                //  стартуем анимацию непрозрачности
                BeginAnimation(OpacityProperty, doubleAnimation);
            }
        }

        //  btnMainMenu4 - Z-отчет
        private void BtnMainMenu4_Click(object sender, RoutedEventArgs e)
        {
            wasPressed = true;

            Vars.mainMenuSelected = 4;

            DoubleAnimation doubleAnimationWidth = new DoubleAnimation(230, 420, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimationWidth.AutoReverse = true;
            btnMainMenu4.BeginAnimation(WidthProperty, doubleAnimationWidth);

            DoubleAnimation doubleAnimationHeight = new DoubleAnimation(35, 75, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimationHeight.AutoReverse = true;
            btnMainMenu4.BeginAnimation(HeightProperty, doubleAnimationHeight);

            DoubleAnimation doubleAnimationFontSize = new DoubleAnimation(15, 25, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimationFontSize.AutoReverse = true;
            btnMainMenu4.BeginAnimation(FontSizeProperty, doubleAnimationFontSize);

            ThicknessAnimation thicknessAnimationPadding = new ThicknessAnimation(new Thickness(40, 0, 0, 0), new Thickness(75, 0, 0, 0), new Duration(TimeSpan.FromSeconds(0.25)));
            thicknessAnimationPadding.AutoReverse = true;
            //thicknessAnimationPadding.Completed += DoubleAnimationBtnMainMenu4_Completed;
            btnMainMenu4.BeginAnimation(PaddingProperty, thicknessAnimationPadding);

            DoubleAnimation doubleAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
            doubleAnimation.Completed += DoubleAnimationBtnMainMenu4_Completed;
            BeginAnimation(OpacityProperty, doubleAnimation);
        }

        private void DoubleAnimationBtnMainMenu4_Completed(object sender, EventArgs e)
        {
            wasPressed = false;

            WindowAutorization windowAutorization = new WindowAutorization();
            windowAutorization.ShowDialog();
            windowAutorization.Close();
            if (Vars.cashierStatus)
            {
                fiscal.OpenRead();
                if (fiscal.GetHeader())
                {
                    if (fiscal.fiscalHeader.fiscalMode[0] != '1')
                    {
                        fiscal.Close();
                        MessageBox.Show("Касса не фискализирована!");
                        Vars.fiscalMode = false;
                        return;
                    }
                    else
                    {
                        Vars.fiscalMode = true;
                    }
                }
                //if (fiscal.AddReport("datafile.tax"))
                if (fiscal.AddReport(Properties.Settings.Default.TAX))
                {
                    Display.ClearText();
                    Display.DisplayText("Z-REPORT");

                    Printer.SetRegularFont();
                    Printer.BeginTransaction();
                    Printer.PrintString(Printer.FormatTwoStrings("KASSIR: " + Vars.cashierName, " POS № " + Properties.Settings.Default.POSNumber));
                    Printer.SetBoldFont();
                    if (width == 38)
                        Printer.PrintString("*********  Z - R E P O R T   *********");
                    else
                        Printer.PrintString("************  Z - R E P O R T   ************");
                    Printer.SetRegularFont();
                    Printer.PrintString("YOPIQ XISOBOT N " + fiscal.fiscalData.closeNumber);
                    string closeDate = fiscal.fiscalData.closeDate;
                    closeDate = closeDate.Substring(0, 2) + "/" + closeDate.Substring(2, 2) + "/" + closeDate.Substring(4, 4);
                    string closeTime = fiscal.fiscalData.closeTime;
                    closeTime = closeTime.Substring(0, 2) + ":" + closeTime.Substring(2, 2) + ":" + closeTime.Substring(4, 2);
                    //Printer.PrintString(Printer.FormatTwoStrings("Date:", fiscal.fiscalData.closeDate + " " + fiscal.fiscalData.closeTime));
                    Printer.PrintString(Printer.FormatTwoStrings("Date:", closeDate + " " + closeTime));
                    Printer.SetBoldFont();
                    Printer.PrintString("CHEKLAR: " + fiscal.fiscalData.checkSum);
                    Printer.PrintString("SUMMASI: " + fiscal.fiscalData.total + " So'm");
                    if (fiscal.vATData.VAT7 != 0)
                        Printer.PrintString("В Т.Ч. НДС 7%: " + fiscal.vATData.VAT7 + " So'm");
                    if (fiscal.vATData.VAT10 != 0)
                        Printer.PrintString("В Т.Ч. НДС 10%: " + fiscal.vATData.VAT10 + " So'm");
                    Printer.PrintString("В Т.Ч. НДС 15%: " + fiscal.vATData.VAT15 + " So'm");
                    if (fiscal.vATData.VAT20 != 0)
                        Printer.PrintString("В Т.Ч. НДС 20%: " + fiscal.vATData.VAT20 + " So'm");

                    Printer.SetBoldFont();
                    if (width == 38)
                        Printer.PrintString("*************************************");
                    else
                        Printer.PrintString("********************************************");
                    Printer.PrintString("");
                    //Printer.PrintString("");
                    //Printer.PrintString("");
                    //Printer.PrintString("");
                    //Printer.PrintRegistanBitmap();
                    Printer.SetBoldFont();
                    //Printer.PrintString("--------------------------------------");
                    //Printer.CutPaper();
                    Printer.PrintString("");
                    if (Vars.fiscalMode)
                    {
                        if (width == 38)
                            Printer.PrintString("                 -FR-                 ");
                        else
                            Printer.PrintString("                    -FR-                    ");
                    }
                    else
                    {
                        if (width == 38)
                            Printer.PrintString("                -NFR-                 ");
                        else
                            Printer.PrintString("                   -NFR-                    ");
                    }
                    Printer.PrintString(Printer.CenterText(Properties.Settings.Default.CompanyName));
                    Printer.PrintString(Printer.CenterText(Properties.Settings.Default.CompanyAddress));
                    Printer.CutPaper();
                    Printer.PrintString(Printer.FormatTwoStrings("INN: " + Properties.Settings.Default.INN, "S/N " + Properties.Settings.Default.SerialNumber));
                    Printer.EndTransaction();
                }
                else
                {
                    MessageBox.Show("Отчет не выполнен - повторите попытку");
                }
                fiscal.Close(); //  данный вызов избыточен, т.к. Fiscal::AddReport() закрывает себя самостоятельно
            }
            //else
            //    MessageBox.Show("Недостаточно прав!");

            //  снова сделаем это окно (окно меню) непрозрачным (видимым)
            doubleAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
            //  стартуем анимацию непрозрачности
            BeginAnimation(OpacityProperty, doubleAnimation);

            //  если мы не в Autonomode (есть сетка)
            if (!Vars.autonomode)
                //  копируем продажи для загрузки в 1С
                CopySalesFor1C();
        }

        //  btnMainMenu5 - X-отчет
        private void BtnMainMenu5_Click(object sender, RoutedEventArgs e)
        {
            wasPressed = true;

            Vars.mainMenuSelected = 5;

            DoubleAnimation doubleAnimationWidth = new DoubleAnimation(230, 420, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimationWidth.AutoReverse = true;
            btnMainMenu5.BeginAnimation(WidthProperty, doubleAnimationWidth);

            DoubleAnimation doubleAnimationHeight = new DoubleAnimation(35, 75, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimationHeight.AutoReverse = true;
            btnMainMenu5.BeginAnimation(HeightProperty, doubleAnimationHeight);

            DoubleAnimation doubleAnimationFontSize = new DoubleAnimation(15, 25, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimationFontSize.AutoReverse = true;
            btnMainMenu5.BeginAnimation(FontSizeProperty, doubleAnimationFontSize);

            ThicknessAnimation thicknessAnimationPadding = new ThicknessAnimation(new Thickness(40, 0, 0, 0), new Thickness(75, 0, 0, 0), new Duration(TimeSpan.FromSeconds(0.25)));
            thicknessAnimationPadding.AutoReverse = true;
            //thicknessAnimationPadding.Completed += DoubleAnimationBtnMainMenu5_Completed;
            btnMainMenu5.BeginAnimation(PaddingProperty, thicknessAnimationPadding);

            DoubleAnimation doubleAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
            doubleAnimation.Completed += DoubleAnimationBtnMainMenu5_Completed;
            BeginAnimation(OpacityProperty, doubleAnimation);
        }

        private void DoubleAnimationBtnMainMenu5_Completed(object sender, EventArgs e)
        {
            wasPressed = false;

            WindowAutorization windowAutorization = new WindowAutorization();
            windowAutorization.ShowDialog();
            windowAutorization.Close();
            if (Vars.cashierStatus)
            {
                fiscal.OpenRead();
                if (fiscal.GetHeader())
                {
                    if (fiscal.fiscalHeader.fiscalMode[0] != '1')
                    {
                        fiscal.Close();
                        MessageBox.Show("Касса не фискализирована!");
                        Vars.fiscalMode = false;
                        return;
                    }
                    else
                    {
                        Vars.fiscalMode = true;
                    }
                }
                if (fiscal.GetLatestZ())
                {
                    //string zDate = fiscal.fiscalData.closeDate.Substring(4, 4) + fiscal.fiscalData.closeDate.Substring(2, 2) + fiscal.fiscalData.closeDate.Substring(0, 2);
                    //string zTime = fiscal.fiscalData.closeTime;
                    string zDate = DateTime.Now.ToString();
                    zDate = zDate.Substring(6, 4) + zDate.Substring(3, 2) + zDate.Substring(0, 2);
                    string zTime = "000000";

                    ChequesDBF.GetTotalAndCheckSumForXReport(zDate, zTime,
                        out double total, out int checkSum,
                        out double annulatedSum, out int annulatedCheckSum,
                        out double returnSum, out int returnCheckSum,
                        out double saleSum, out int saleCheckSum);
                    Display.ClearText();
                    Display.DisplayText("X-REPORT");

                    Printer.SetRegularFont();
                    Printer.BeginTransaction();
                    Printer.PrintString(Printer.FormatTwoStrings("KASSIR: " + Vars.cashierName, " POS № " + Properties.Settings.Default.POSNumber));
                    Printer.SetBoldFont();
                    if (width == 38)
                        Printer.PrintString("*********  X - R E P O R T   *********");
                    else
                        Printer.PrintString("************  X - R E P O R T   ************");
                    Printer.SetRegularFont();
                    Printer.PrintString("KUNDALIK XISOBOT");
                    //string closeDate = zDate;
                    //closeDate = closeDate.Substring(0, 2) + "/" + closeDate.Substring(2, 2) + "/" + closeDate.Substring(4, 4);
                    //string closeTime = fiscal.fiscalData.closeTime;
                    //closeTime = closeTime.Substring(0, 2) + ":" + closeTime.Substring(2, 2) + ":" + closeTime.Substring(4, 2);
                    //Printer.PrintString(Printer.FormatTwoStrings("Date:", closeDate + " " + closeTime));
                    Printer.PrintString(DateTime.Now.ToString());

                    Printer.PrintString("ANNULACIYA CHEKLAR: " + annulatedCheckSum);
                    Printer.PrintString("SUMMASI: " + annulatedSum + " So'm");
                    if (width == 38)
                        Printer.PrintString("*************************************");
                    else
                        Printer.PrintString("********************************************");
                    Printer.PrintString("QAYTAR CHEKLAR: " + returnCheckSum);
                    Printer.PrintString("SUMMASI: " + returnSum + " So'm");
                    if (width == 38)
                        Printer.PrintString("*************************************");
                    else
                        Printer.PrintString("********************************************");
                    Printer.PrintString("SAVDO CHEKLAR: " + saleCheckSum);
                    Printer.PrintString("SUMMASI: " + saleSum + " So'm");
                    if (width == 38)
                        Printer.PrintString("*************************************");
                    else
                        Printer.PrintString("********************************************");
                    Printer.SetBoldFont();
                    Printer.PrintString("QOLDIQ: " + total + " So'm");
                    if (width == 38)
                        Printer.PrintString("*************************************");
                    else
                        Printer.PrintString("********************************************");
                    Printer.PrintString("");
                    //Printer.PrintString("");
                    //Printer.PrintString("");
                    //Printer.PrintString("");
                    //Printer.PrintRegistanBitmap();
                    Printer.SetBoldFont();
                    //Printer.PrintString("--------------------------------------");
                    //Printer.CutPaper();
                    Printer.PrintString("");
                    if (Vars.fiscalMode)
                    {
                        if (width == 38)
                            Printer.PrintString("                 -FR-                 ");
                        else
                            Printer.PrintString("                    -FR-                    ");
                    }
                    else
                    {
                        if (width == 38)
                            Printer.PrintString("                -NFR-                 ");
                        else
                            Printer.PrintString("                   -NFR-                    ");
                    }
                    Printer.PrintString(Printer.CenterText(Properties.Settings.Default.CompanyName));
                    Printer.PrintString(Printer.CenterText(Properties.Settings.Default.CompanyAddress));
                    Printer.CutPaper();
                    Printer.PrintString(Printer.FormatTwoStrings("INN: " + Properties.Settings.Default.INN, "S/N " + Properties.Settings.Default.SerialNumber));
                    Printer.SetRegularFont();
                    Printer.EndTransaction();
                }
                fiscal.Close();
            }
            //else
            //    MessageBox.Show("Недостаточно прав!");

            //  снова сделаем это окно (окно меню) непрозрачным (видимым)
            doubleAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
            //  стартуем анимацию непрозрачности
            BeginAnimation(OpacityProperty, doubleAnimation);

            if (!Vars.autonomode)
                CopySalesFor1C();
        }

        //  btnMainMenu2 - Выключить кассу
        private void BtnMainMenu2_Click(object sender, RoutedEventArgs e)
        {
            wasPressed = true;

            Vars.mainMenuSelected = 2;

            DoubleAnimation doubleAnimationWidth = new DoubleAnimation(230, 420, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimationWidth.AutoReverse = true;
            btnMainMenu2.BeginAnimation(WidthProperty, doubleAnimationWidth);

            DoubleAnimation doubleAnimationHeight = new DoubleAnimation(35, 75, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimationHeight.AutoReverse = true;
            btnMainMenu2.BeginAnimation(HeightProperty, doubleAnimationHeight);

            DoubleAnimation doubleAnimationFontSize = new DoubleAnimation(15, 25, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimationFontSize.AutoReverse = true;
            btnMainMenu2.BeginAnimation(FontSizeProperty, doubleAnimationFontSize);

            ThicknessAnimation thicknessAnimationPadding = new ThicknessAnimation(new Thickness(40, 0, 0, 0), new Thickness(75, 0, 0, 0), new Duration(TimeSpan.FromSeconds(0.25)));
            thicknessAnimationPadding.AutoReverse = true;
            //thicknessAnimationPadding.Completed += DoubleAnimationBtnMainMenu2_Completed;
            btnMainMenu2.BeginAnimation(PaddingProperty, thicknessAnimationPadding);

            DoubleAnimation doubleAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
            doubleAnimation.Completed += DoubleAnimationBtnMainMenu2_Completed;
            BeginAnimation(OpacityProperty, doubleAnimation);
        }

        private void DoubleAnimationBtnMainMenu2_Completed(object sender, EventArgs e)
        {
            wasPressed = false;

            WindowAutorization windowAutorization = new WindowAutorization();
            windowAutorization.ShowDialog();
            windowAutorization.Close();
            if (Vars.cashierStatus)
            {
                System.Diagnostics.Process.Start("cmd", "/c shutdown -s -f -t 00");
            }
            //else
            //    MessageBox.Show("Недостаточно прав!");
            else
            {
                //  снова сделаем это окно (окно меню) непрозрачным (видимым)
                doubleAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
                //  стартуем анимацию непрозрачности
                BeginAnimation(OpacityProperty, doubleAnimation);
            }
        }

        //  btnMainMenu8 - Сервис
        private void BtnMainMenu8_Click(object sender, RoutedEventArgs e)
        {
            wasPressed = true;

            WindowAutorization windowAutorization = new WindowAutorization();
            windowAutorization.Mode = "fiscal";
            windowAutorization.ShowDialog();
            windowAutorization.Close();
            //if (Vars.cashierStatus)
            if (windowAutorization.Result)
            {
                //Topmost = false;
                //System.Diagnostics.Process p = new System.Diagnostics.Process();
                //p.StartInfo.FileName = "calc.exe";
                //p.Start();
                //p.WaitForExit();
                ////Topmost = true;
                ////System.Diagnostics.Process.Start("service.exe");
                WindowService windowService = new WindowService();

                windowService.Owner = this;
                Hide();
                windowService.Show();
            }
            //else
            //    MessageBox.Show("Недостаточно прав!");

            wasPressed = false;
        }

        //  btnMainMenu9 - Настройка клавиш
        private void BtnMainMenu9_Click(object sender, RoutedEventArgs e)
        {
            wasPressed = true;

            Vars.mainMenuSelected = 9;

            DoubleAnimation doubleAnimationWidth = new DoubleAnimation(230, 420, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimationWidth.AutoReverse = true;
            btnMainMenu9.BeginAnimation(WidthProperty, doubleAnimationWidth);

            DoubleAnimation doubleAnimationHeight = new DoubleAnimation(35, 75, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimationHeight.AutoReverse = true;
            btnMainMenu9.BeginAnimation(HeightProperty, doubleAnimationHeight);

            DoubleAnimation doubleAnimationFontSize = new DoubleAnimation(15, 25, new Duration(TimeSpan.FromSeconds(0.25)));
            doubleAnimationFontSize.AutoReverse = true;
            btnMainMenu9.BeginAnimation(FontSizeProperty, doubleAnimationFontSize);

            ThicknessAnimation thicknessAnimationPadding = new ThicknessAnimation(new Thickness(40, 0, 0, 0), new Thickness(75, 0, 0, 0), new Duration(TimeSpan.FromSeconds(0.25)));
            thicknessAnimationPadding.AutoReverse = true;
            btnMainMenu9.BeginAnimation(PaddingProperty, thicknessAnimationPadding);

            DoubleAnimation doubleAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
            doubleAnimation.Completed += DoubleAnimationBtnMainMenu9_Completed;
            BeginAnimation(OpacityProperty, doubleAnimation);
        }

        private void DoubleAnimationBtnMainMenu9_Completed(object sender, EventArgs e)
        {
            wasPressed = false;

            WindowAutorization windowAutorization = new WindowAutorization();
            windowAutorization.ShowDialog();
            windowAutorization.Close();
            if (Vars.cashierStatus)
            {
                WindowKeys windowKeys = new WindowKeys();
                //windowSettings.Show();
                //Close();

                windowKeys.Owner = this;
                Hide();
                windowKeys.Show();
                //DoubleAnimation doubleAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
                //BeginAnimation(OpacityProperty, doubleAnimation);
            }
            //else
            //    MessageBox.Show("Недостаточно прав!");
            else
            {
                //  снова сделаем это окно (окно меню) непрозрачным (видимым)
                doubleAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
                //  стартуем анимацию непрозрачности
                BeginAnimation(OpacityProperty, doubleAnimation);
            }
        }

        //  копируем продажи на сервер
        private void CopySalesFor1C()
        {
            string sourceDir = Properties.Settings.Default.DBF;
            string destDir = Properties.Settings.Default.SalesOutPath;
            string destDirBackups = Properties.Settings.Default.SalesOutBackupPath;

            if (!Directory.Exists(destDir))
            {
                MessageBox.Show("Проверьте путь выгрузки");
                return;
            }

            string todayDate = DateTime.Now.ToString().Substring(0, 10);
            destDir += @"\" + todayDate;
            if (!Directory.Exists(destDir))
            {
                try
                {
                    Directory.CreateDirectory(destDir);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка создания каталога: " + ex.Message);
                    return;
                }
            }

            string[] files = Directory.GetFiles(sourceDir, "cheq*.*");
            foreach (string file in files)
            {
                string fileName = file.Substring(sourceDir.Length + 1);
                try
                {
                    File.Copy(System.IO.Path.Combine(sourceDir, fileName), System.IO.Path.Combine(destDir, fileName), true);
                }
                catch (IOException copyError)
                {
                    MessageBox.Show("Ошибка копирования: " + copyError.Message);
                    return;
                }
            }

            if (!Directory.Exists(destDirBackups))
            {
                MessageBox.Show("Проверьте путь выгрузки");
                return;
            }

            todayDate = DateTime.Now.ToString().Substring(0, 10);
            destDirBackups += @"\" + todayDate;
            if (!Directory.Exists(destDirBackups))
            {
                try
                {
                    Directory.CreateDirectory(destDirBackups);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка создания каталога: " + ex.Message);
                    return;
                }
            }

            files = Directory.GetFiles(sourceDir, "cheq*.*");
            foreach (string file in files)
            {
                string fileName = file.Substring(sourceDir.Length + 1);
                try
                {
                    File.Copy(System.IO.Path.Combine(sourceDir, fileName), System.IO.Path.Combine(destDirBackups, fileName), true);
                }
                catch (IOException copyError)
                {
                    MessageBox.Show("Ошибка копирования: " + copyError.Message);
                    return;
                }
            }
        }
    }
}
