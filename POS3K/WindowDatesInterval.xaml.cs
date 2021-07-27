using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POS3K
{
    /// <summary>
    /// Логика взаимодействия для WindowDatesInterval.xaml
    /// </summary>
    public partial class WindowDatesInterval : Window
    {
        public string StartDate
        {
            get { return tbStartDate.Text; }
        }

        public string EndDate
        {
            get { return tbEndDate.Text; }
        }

        public WindowDatesInterval()
        {
            InitializeComponent();
            //tbStartDate.PreviewTextInput += TbStartDate_PreviewTextInput;
            Loaded += WindowDatesInterval_Loaded;
            //KeyDown += WindowDatesInterval_KeyDown;
            //PreviewKeyDown += WindowDatesInterval_KeyDown;
            //tbStartDate.KeyDown += WindowDatesInterval_KeyDown;
            //tbStartDate.PreviewKeyDown += WindowDatesInterval_KeyDown;
            //tbEndDate.KeyDown += WindowDatesInterval_KeyDown;
        }

        private void WindowDatesInterval_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.End)
            {
                if (tbStartDate.IsFocused)
                    tbStartDate.Text += "1";
                else if (tbEndDate.IsFocused)
                    tbEndDate.Text += "1";
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                if (tbStartDate.IsFocused)
                    tbStartDate.Text += "2";
                else if (tbEndDate.IsFocused)
                    tbEndDate.Text += "2";
                e.Handled = true;
            }
            else if (e.Key == Key.Next)
            {
                if (tbStartDate.IsFocused)
                    tbStartDate.Text += "3";
                else if (tbEndDate.IsFocused)
                    tbEndDate.Text += "3";
                e.Handled = true;
            }
            else if (e.Key == Key.Left)
            {
                if (tbStartDate.IsFocused)
                    tbStartDate.Text += "4";
                else if (tbEndDate.IsFocused)
                    tbEndDate.Text += "4";
                e.Handled = true;
            }
            else if (e.Key == Key.Clear)
            {
                if (tbStartDate.IsFocused)
                    tbStartDate.Text += "5";
                else if (tbEndDate.IsFocused)
                    tbEndDate.Text += "5";
                e.Handled = true;
            }
            else if (e.Key == Key.Right)
            {
                if (tbStartDate.IsFocused)
                    tbStartDate.Text += "6";
                else if (tbEndDate.IsFocused)
                    tbEndDate.Text += "6";
                e.Handled = true;
            }
            else if (e.Key == Key.Home)
            {
                if (tbStartDate.IsFocused)
                    tbStartDate.Text += "7";
                else if (tbEndDate.IsFocused)
                    tbEndDate.Text += "7";
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                if (tbStartDate.IsFocused)
                    tbStartDate.Text += "8";
                else if (tbEndDate.IsFocused)
                    tbEndDate.Text += "8";
                e.Handled = true;
            }
            else if (e.Key == Key.PageUp)
            {
                if (tbStartDate.IsFocused)
                    tbStartDate.Text += "9";
                else if (tbEndDate.IsFocused)
                    tbEndDate.Text += "9";
                e.Handled = true;
            }
            else if (e.Key == Key.Insert)
            {
                if (tbStartDate.IsFocused)
                    tbStartDate.Text += "0";
                else if (tbEndDate.IsFocused)
                    tbEndDate.Text += "0";
                e.Handled = true;
            }
            else if (e.Key == Key.Delete)
            {
                if (tbStartDate.IsFocused)
                    tbStartDate.Text += ".";
                else if (tbEndDate.IsFocused)
                    tbEndDate.Text += ".";
                e.Handled = true;
            }
            else if (e.Key == Key.Back)
            {
                //if (tbStartDate.IsFocused)
                //    tbEndDate.Focus();
                //else if (tbEndDate.IsFocused)
                //    btnOK.Focus();
                //else if (btnOK.IsFocused)
                //    btnCancel.Focus();
                //else if (btnCancel.IsFocused)
                //    tbStartDate.Focus();
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                request.Wrapped = true;
                ((Control)sender).MoveFocus(request);
                e.Handled = true;
            }
            if (tbStartDate.IsFocused)
                tbStartDate.CaretIndex = tbStartDate.Text.Length;
            else if (tbEndDate.IsFocused)
                tbEndDate.CaretIndex = tbEndDate.Text.Length;
        }

        private void WindowDatesInterval_Loaded(object sender, RoutedEventArgs e)
        {
            tbStartDate.Focus();
        }

        private void TbStartDate_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex r = new Regex(@"([0][1-9]|[1][0-9|][2][0-9]|[3][0-1])\/([0][1-9]|[1][0-2])\/[1-2][0-9][0-9][0-9]");
            //Regex r = new Regex("^[0-9]*$");
            e.Handled = !r.IsMatch(e.Text);
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
