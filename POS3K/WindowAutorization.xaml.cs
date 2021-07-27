using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POS3K
{
    /// <summary>
    /// Логика взаимодействия для WindowAutorization.xaml
    /// </summary>
    public partial class WindowAutorization : Window
    {
        private string mode = "";
        public string Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        private bool result;
        public bool Result { get; set; }

        public WindowAutorization()
        {
            InitializeComponent();

            //  сделаем это в другом месте  20201215
            //Vars.cashierStatus = false;
            //Vars.cashierId = "";
            //Vars.cashierName = "";

            this.Cursor = Cursors.None;

            RoutedCommand enterCommand = new RoutedCommand();
            enterCommand.InputGestures.Add(new KeyGesture(Key.Return, ModifierKeys.None));
            CommandBindings.Add(new CommandBinding(enterCommand, enterCommand_event_handler));

            //  скорее всего этот вариант реагирования на Escape более аккуратен, но я не знаю, как это сработает на рабочей машине, поэтому оставим как было
            //RoutedCommand escapeCommand = new RoutedCommand();
            //escapeCommand.InputGestures.Add(new KeyGesture(Key.Escape, ModifierKeys.None));
            //CommandBindings.Add(new CommandBinding(escapeCommand, escapeCommand_event_handler));
        }

        //private void escapeCommand_event_handler(object sender, ExecutedRoutedEventArgs e)
        //{
        //    MessageBox.Show("Escape key was pressed");
        //}

        //  метод события подтверждения ввода пользователя
        private void enterCommand_event_handler(object sender, ExecutedRoutedEventArgs e)
        {
            if (Mode == "")
            {
                //  перенес присваивания из Initialize  20201215
                Vars.cashierStatus = false;
                Vars.cashierId = "";
                Vars.cashierName = "";

                try
                {
                    //string FilePath = @"C:\Registan\POS\Bases\";
                    string FilePath = Properties.Settings.Default.DBF;
                    string FileName = "Cashiers";

                    OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FilePath + ";Extended Properties=dBase IV;User ID=;Password=");
                    connection.Open();
                    OleDbCommand command = new OleDbCommand("select * from " + FileName + " where id = '" + txtPassword.Password + "'", connection);

                    DataTable dt = new DataTable();
                    dt.Load(command.ExecuteReader());

                    connection.Close();

                    //  если в Cashiers.dbf не найден ни один пользователь с введенным кодом
                    if (dt.Rows.Count == 0)
                    {
                        //MessageBox.Show("Пользователь не найден!");
                        //  прятать окно не будем - предоставим пользователю возможность повторного ввода кода
                    }
                    //  пользователь с введенным кодом найден
                    else
                    {
                        Vars.cashierId = txtPassword.Password;
                        Vars.cashierName = dt.Rows[0].Field<string>("Name");
                        Vars.cashierStatus = dt.Rows[0].Field<bool>("Status");
                        //DoubleAnimation doubleAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
                        //doubleAnimation.Completed += DoubleAnimation_Completed;
                        //BeginAnimation(OpacityProperty, doubleAnimation);
                        Vars.regime = 0;
                        //  прячем это окно (окно авторизации)
                        Hide();
                    }

                    txtPassword.Password = "";
                }
                catch (OleDbException exp)
                {
                    MessageBox.Show("Error: " + exp.Message);
                    //Close();
                    Hide();
                }
            }
            else if (Mode == "fiscal")
            {
                if (txtPassword.Password == "12345")
                {
                    Result = true;
                    //DoubleAnimation doubleAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
                    //doubleAnimation.Completed += DoubleAnimation_Completed;
                    //BeginAnimation(OpacityProperty, doubleAnimation);
                    Vars.regime = 0;
                    Hide();
                }
                else
                    Result = false;
            }
            else if (Mode == "super")
            {
                DateTime dateTime = DateTime.Now;
                string pass = (dateTime.Year - (dateTime.Day + dateTime.Month)).ToString();


                if (txtPassword.Password.Equals(pass))
                {
                    Vars.regime = 0;
                    Result = true;
                    Hide();
                }
                else
                {
                    Result = false;
                    Hide();
                }
            }
        }

        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            Vars.regime = 0;
            //Close();
            Hide();
        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            Topmost = true;
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
            //  если нажата клавиша Escape (СБРОС)
            else if (e.Key == Key.Escape)
            {
                //  также скопировал присваивания из Initialize сюда, но с условием  20201215
                if (Mode != "super")
                {
                    Vars.cashierStatus = false;
                    Vars.cashierId = "";
                    Vars.cashierName = "";
                }

                //Close();
                //  спрячем это окно (окно авторизации)
                Hide();
                //  окно спрятано, теперь активно окно главного меню, но оно прозрачно, поэтому мы его не видим
            }

            // set the cursor position to 2...
            SetSelection(txtPassword, txtPassword.Password.Length, 0);
            // focus the control to update the selection
            txtPassword.Focus();
        }

        private void SetSelection(PasswordBox passwordBox, int start, int length)
        {
            passwordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(passwordBox, new object[] { start, length });
        }
    }
}
