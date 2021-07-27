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

namespace POS3K
{
    /// <summary>
    /// Логика взаимодействия для WindowRegistration.xaml
    /// </summary>
    public partial class WindowRegistration : Window
    {
        public string OwnerCode
        {
            get { return tbOwnerCode.Text; }
        }

        public string RegNumber
        {
            get { return tbRegNumber.Text; }
        }

        public WindowRegistration()
        {
            InitializeComponent();
            Loaded += WindowRegistration_Loaded;
        }

        private void WindowRegistration_Loaded(object sender, RoutedEventArgs e)
        {
            tbOwnerCode.Focus();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Number_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.End)
            {
                if (tbOwnerCode.IsFocused)
                    tbOwnerCode.Text += "1";
                else if (tbRegNumber.IsFocused)
                    tbRegNumber.Text += "1";
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                if (tbOwnerCode.IsFocused)
                    tbOwnerCode.Text += "2";
                else if (tbRegNumber.IsFocused)
                    tbRegNumber.Text += "2";
                e.Handled = true;
            }
            else if (e.Key == Key.Next)
            {
                if (tbOwnerCode.IsFocused)
                    tbOwnerCode.Text += "3";
                else if (tbRegNumber.IsFocused)
                    tbRegNumber.Text += "3";
                e.Handled = true;
            }
            else if (e.Key == Key.Left)
            {
                if (tbOwnerCode.IsFocused)
                    tbOwnerCode.Text += "4";
                else if (tbRegNumber.IsFocused)
                    tbRegNumber.Text += "4";
                e.Handled = true;
            }
            else if (e.Key == Key.Clear)
            {
                if (tbOwnerCode.IsFocused)
                    tbOwnerCode.Text += "5";
                else if (tbRegNumber.IsFocused)
                    tbRegNumber.Text += "5";
                e.Handled = true;
            }
            else if (e.Key == Key.Right)
            {
                if (tbOwnerCode.IsFocused)
                    tbOwnerCode.Text += "6";
                else if (tbRegNumber.IsFocused)
                    tbRegNumber.Text += "6";
                e.Handled = true;
            }
            else if (e.Key == Key.Home)
            {
                if (tbOwnerCode.IsFocused)
                    tbOwnerCode.Text += "7";
                else if (tbRegNumber.IsFocused)
                    tbRegNumber.Text += "7";
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                if (tbOwnerCode.IsFocused)
                    tbOwnerCode.Text += "8";
                else if (tbRegNumber.IsFocused)
                    tbRegNumber.Text += "8";
                e.Handled = true;
            }
            else if (e.Key == Key.PageUp)
            {
                if (tbOwnerCode.IsFocused)
                    tbOwnerCode.Text += "9";
                else if (tbRegNumber.IsFocused)
                    tbRegNumber.Text += "9";
                e.Handled = true;
            }
            else if (e.Key == Key.Insert)
            {
                if (tbOwnerCode.IsFocused)
                    tbOwnerCode.Text += "0";
                else if (tbRegNumber.IsFocused)
                    tbRegNumber.Text += "0";
                e.Handled = true;
            }
            else if (e.Key == Key.Back)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                request.Wrapped = true;
                ((Control)sender).MoveFocus(request);
                e.Handled = true;
            }
            if (tbOwnerCode.IsFocused)
                tbOwnerCode.CaretIndex = tbOwnerCode.Text.Length;
            else if (tbRegNumber.IsFocused)
                tbRegNumber.CaretIndex = tbRegNumber.Text.Length;
        }
    }
}
