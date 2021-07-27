using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Data;
using System.Data.OleDb;
using System.Windows.Media.Animation;

using POS.Devices;
using System.Threading;
using System.Reflection;

namespace POS3K
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static OPOSCashDrawer drawer;
        public static OPOSLineDisplay display;
        public static OPOSPOSPrinter printer;
        public static OPOSPOSKeyboard keyboard;
        public static OPOSMSR msr;
        public static OPOSKeylock keylock;

        public static OPOSScanner scanner;

        public static OPOSPOSPrinterConstants printerConstants;

        public Fiscal fiscal = new Fiscal();

        public Exchange exchange = new Exchange();

#region DEBUG
        //  в рабочем режиме выставить значение в false
        public bool Lenovo = false;
#endregion DEBUG

        public int escapes = 0;

        DoubleAnimation doubleAnimation;
        RoutedCommand enterCommand;
        Window1 window1;
        DataTable dt;
        OleDbConnection connection;
        OleDbCommand command;
        DateTime dateTime;

        public MainWindow()
        {
            InitializeComponent();

            this.Cursor = Cursors.None;

    #region DEBUG
            //  раскомментировать в рабочем режиме
            Thread.Sleep(13000);
    #endregion DEBUG

            enterCommand = new RoutedCommand();
            enterCommand.InputGestures.Add(new KeyGesture(Key.Return, ModifierKeys.None));
            CommandBindings.Add(new CommandBinding(enterCommand, enterCommand_event_handler));

            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 10 });

            //POSKeyboard.Initialize();
        }

        private void enterCommand_event_handler(object sender, ExecutedRoutedEventArgs e)
        {
            escapes = 0;
            try
            {
                //string FilePath = @"C:\Registan\POS\Bases\";
                string FilePath = Properties.Settings.Default.DBF;
                string FileName = "Cashiers";

                connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
                connection.Open();
                command = new OleDbCommand("select * from " + FileName + " where id = '" + txtPassword.Password + "'", connection);

                dt = new DataTable();
                dt.Load(command.ExecuteReader());

                connection.Close();
                connection.Dispose();
                command.Dispose();

                if (dt.Rows.Count == 0)
                {
                    //MessageBox.Show("Пользователь не найден!");
                }
                else
                {
                    Vars.cashierId = txtPassword.Password;
                    Vars.cashierName = dt.Rows[0].Field<string>("Name");
                    Vars.cashierStatus = dt.Rows[0].Field<bool>("Status");
                    dt.Dispose();
                    doubleAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
                    doubleAnimation.Completed += DoubleAnimation_Completed;
                    BeginAnimation(OpacityProperty, doubleAnimation);

                    dateTime = DateTime.Now;
                    if (!Lenovo)
                        while (!Drawer.Initialize())
                        {
                            //if (DateTime.Compare(DateTime.Now, dateTime.AddSeconds(60 * 2)) > 0)
                            if (DateTime.Compare(DateTime.Now, dateTime.AddSeconds(10)) > 0)
                            {
                                MessageBox.Show("Ошибка инициализации денежного ящика!");
                                Close();
                                System.Windows.Application.Current.Shutdown();
                                return;
                            }
                        }
                    else
                    {
                        try
                        {
                            Drawer.Initialize();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка инициализации денежного ящика " + ex.Message);
                        }
                    }

                    dateTime = DateTime.Now;
                    if (!Lenovo)
                        while (!Display.Initialize())
                        {
                            //if (DateTime.Compare(DateTime.Now, dateTime.AddSeconds(60 * 2)) > 0)
                            if (DateTime.Compare(DateTime.Now, dateTime.AddSeconds(10)) > 0)
                            {
                                MessageBox.Show("Ошибка инициализации табло покупателя!");
                                Close();
                                System.Windows.Application.Current.Shutdown();
                                return;
                            }
                        }
                    else
                    {
                        try
                        {
                            Display.Initialize();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка инициализации табло покупателя " + ex.Message);
                        }
                    }
                    Display.DisplayText("Registon Supermarket");
                    Display.DisplayTextAt(0, 1, "ДОБРО ПОЖАЛОВАТЬ!");

                    dateTime = DateTime.Now;
                    if (!Lenovo)
                        while (!Printer.Initialize())
                        {
                            //if (DateTime.Compare(DateTime.Now, dateTime.AddSeconds(60 * 3)) > 0)
                            if (DateTime.Compare(DateTime.Now, dateTime.AddSeconds(10)) > 0)
                            {
                                MessageBox.Show("Ошибка инициализации принтера чеков!");
                                Close();
                                System.Windows.Application.Current.Shutdown();
                                return;
                            }
                        }
                    else
                    {
                        try
                        {
                            Printer.Initialize();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка инициализации табло покупателя " + ex.Message);
                        }
                    }
                }

                txtPassword.Password = "";
            }
            catch (OleDbException exp)
            {
                MessageBox.Show("Error: " + exp.Message);
                Close();
            }
        }

        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            doubleAnimation.Completed -= DoubleAnimation_Completed;
            window1 = new Window1();
            Vars.regime = 0;
            window1.Show();
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            Topmost = true;
            txtPassword.Focus();
        }

        private void TxtPassword_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.End)
                txtPassword.Password += "1";
            else if (e.Key == Key.Down)
                txtPassword.Password += "2";
            else if (e.Key == Key.Next)
                txtPassword.Password += "3";
            else if (e.Key == Key.Left)
                txtPassword.Password += "4";
            else if (e.Key == Key.Clear)
                txtPassword.Password += "5";
            else if (e.Key == Key.Right)
                txtPassword.Password += "6";
            else if (e.Key == Key.Home)
                txtPassword.Password += "7";
            else if (e.Key == Key.Up)
                txtPassword.Password += "8";
            else if (e.Key == Key.PageUp)
                txtPassword.Password += "9";
            else if (e.Key == Key.Insert)
                txtPassword.Password += "0";
            else if (e.Key == Key.Escape)
            {
                escapes++;
                if (escapes > 4)
                    Close();
            }
            else if (e.Key == Key.Scroll)   //  клавиша КУПОН
                e.Handled = true;
            else if (e.Key == Key.NumLock)   //  клавиша ВАУЧЕР
                e.Handled = true;

            // set the cursor position to Length
            SetSelection(txtPassword, txtPassword.Password.Length, 0);
            // focus the control to update the selection
            txtPassword.Focus();
        }

        private void SetSelection(PasswordBox passwordBox, int start, int length)
        {
            passwordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(passwordBox, new object[] { start, length });
        }

        private void txtPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            txtPassword.Focus();
        }
    }
}
