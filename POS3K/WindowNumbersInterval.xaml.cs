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
    /// Логика взаимодействия для WindowNumbersInterval.xaml
    /// </summary>
    public partial class WindowNumbersInterval : Window
    {
        public string StartNumber
        {
            get { return tbStartNumber.Text; }
        }

        public string EndNumber
        {
            get { return tbEndNumber.Text; }
        }

        public WindowNumbersInterval()
        {
            InitializeComponent();
            //PreviewTextInput += TextBoxPreviewTextInput;
            //tbStartNumber.PreviewTextInput += TextBoxPreviewTextInput;
            //tbEndNumber.PreviewTextInput += TextBoxPreviewTextInput;
            Loaded += WindowNumbersInterval_Loaded;
        }

        private void WindowNumbersInterval_Loaded(object sender, RoutedEventArgs e)
        {
            tbStartNumber.Focus();
        }

        private void TextBoxPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex r = new Regex("^[0-9]*$");
            e.Handled = !r.IsMatch(e.Text);
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Number_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.End)
            {
                if (tbStartNumber.IsFocused)
                    tbStartNumber.Text += "1";
                else if (tbEndNumber.IsFocused)
                    tbEndNumber.Text += "1";
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                if (tbStartNumber.IsFocused)
                    tbStartNumber.Text += "2";
                else if (tbEndNumber.IsFocused)
                    tbEndNumber.Text += "2";
                e.Handled = true;
            }
            else if (e.Key == Key.Next)
            {
                if (tbStartNumber.IsFocused)
                    tbStartNumber.Text += "3";
                else if (tbEndNumber.IsFocused)
                    tbEndNumber.Text += "3";
                e.Handled = true;
            }
            else if (e.Key == Key.Left)
            {
                if (tbStartNumber.IsFocused)
                    tbStartNumber.Text += "4";
                else if (tbEndNumber.IsFocused)
                    tbEndNumber.Text += "4";
                e.Handled = true;
            }
            else if (e.Key == Key.Clear)
            {
                if (tbStartNumber.IsFocused)
                    tbStartNumber.Text += "5";
                else if (tbEndNumber.IsFocused)
                    tbEndNumber.Text += "5";
                e.Handled = true;
            }
            else if (e.Key == Key.Right)
            {
                if (tbStartNumber.IsFocused)
                    tbStartNumber.Text += "6";
                else if (tbEndNumber.IsFocused)
                    tbEndNumber.Text += "6";
                e.Handled = true;
            }
            else if (e.Key == Key.Home)
            {
                if (tbStartNumber.IsFocused)
                    tbStartNumber.Text += "7";
                else if (tbEndNumber.IsFocused)
                    tbEndNumber.Text += "7";
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                if (tbStartNumber.IsFocused)
                    tbStartNumber.Text += "8";
                else if (tbEndNumber.IsFocused)
                    tbEndNumber.Text += "8";
                e.Handled = true;
            }
            else if (e.Key == Key.PageUp)
            {
                if (tbStartNumber.IsFocused)
                    tbStartNumber.Text += "9";
                else if (tbEndNumber.IsFocused)
                    tbEndNumber.Text += "9";
                e.Handled = true;
            }
            else if (e.Key == Key.Insert)
            {
                if (tbStartNumber.IsFocused)
                    tbStartNumber.Text += "0";
                else if (tbEndNumber.IsFocused)
                    tbEndNumber.Text += "0";
                e.Handled = true;
            }
            else if (e.Key == Key.Back)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                request.Wrapped = true;
                ((Control)sender).MoveFocus(request);
                e.Handled = true;
            }
            if (tbStartNumber.IsFocused)
                tbStartNumber.CaretIndex = tbStartNumber.Text.Length;
            else if (tbEndNumber.IsFocused)
                tbEndNumber.CaretIndex = tbEndNumber.Text.Length;
        }
    }
}
