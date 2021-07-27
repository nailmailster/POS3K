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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POS3K
{
    /// <summary>
    /// Логика взаимодействия для WindowSettings.xaml
    /// </summary>
    public partial class WindowSettings : Window
    {
        public WindowSettings()
        {
            InitializeComponent();

            textBoxDBF.Text = Properties.Settings.Default.DBF;
            textBoxTAX.Text = Properties.Settings.Default.TAX;
            textBoxVAT.Text = Properties.Settings.Default.VAT;
            textBoxCompanyName.Text = Properties.Settings.Default.CompanyName;
            textBoxCompanyAddress.Text = Properties.Settings.Default.CompanyAddress;
            textBoxINNFull.Text = Properties.Settings.Default.INN;
            textBoxSerialNumber.Text = Properties.Settings.Default.SerialNumber;
            textBoxPOSNumber.Text = Properties.Settings.Default.POSNumber;
            textBoxPOSName.Text = Properties.Settings.Default.POSName;
            textBoxInPath.Text = Properties.Settings.Default.InPath;
            textBoxLoyaltyPath.Text = Properties.Settings.Default.LoyaltyPath;
            textBoxRegularFontCodepage.Text = Properties.Settings.Default.RegularFont;
            textBoxBoldFontCodepage.Text = Properties.Settings.Default.BoldFont;
            textBoxSymbolsPerLine.Text = Properties.Settings.Default.SymbolsPerLine;
            checkBoxAutonomodeShader.IsChecked = Properties.Settings.Default.UseShaderInAutonomode;
            textBoxDisplayCodepage.Text = Properties.Settings.Default.DisplayCodePage;
            checkBoxUseSystemTone.IsChecked = Properties.Settings.Default.UseSystemTone;
            checkBoxUseOPOSScanner.IsChecked = Properties.Settings.Default.UseOPOSScanner;
            checkBoxUseWindowsDriverForPrinting.IsChecked = Properties.Settings.Default.UseWindowsDriverForPrinting;
            textBoxLeftMargin.Text = Properties.Settings.Default.LeftMargin.ToString();
            textBoxRightMargin.Text = Properties.Settings.Default.RightMargin.ToString();
            textBoxDescriptionWidth.Text = Properties.Settings.Default.DescriptionWidth.ToString();
            textBoxSumWidth.Text = Properties.Settings.Default.SumWidth.ToString();
            checkBoxUseFontB.IsChecked = Properties.Settings.Default.FontB;
            textBoxSalesOutPath.Text = Properties.Settings.Default.SalesOutPath;
            textBoxSalesOutBackupPath.Text = Properties.Settings.Default.SalesOutBackupPath;
            textBoxShutDownTime.Text = Properties.Settings.Default.ShutDownTime;
        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            Topmost = true;
        }

        private void ButtonSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.DBF = textBoxDBF.Text;
            Properties.Settings.Default.TAX = textBoxTAX.Text;
            Properties.Settings.Default.VAT = textBoxVAT.Text;
            Properties.Settings.Default.CompanyName = textBoxCompanyName.Text;
            Properties.Settings.Default.CompanyAddress = textBoxCompanyAddress.Text;
            Properties.Settings.Default.INN = textBoxINNFull.Text;
            Properties.Settings.Default.SerialNumber = textBoxSerialNumber.Text;
            Properties.Settings.Default.POSNumber = textBoxPOSNumber.Text;
            Properties.Settings.Default.POSName = textBoxPOSName.Text;
            Properties.Settings.Default.InPath = textBoxInPath.Text;
            Properties.Settings.Default.LoyaltyPath = textBoxLoyaltyPath.Text;
            Properties.Settings.Default.RegularFont = textBoxRegularFontCodepage.Text;
            Properties.Settings.Default.BoldFont = textBoxBoldFontCodepage.Text;
            Properties.Settings.Default.SymbolsPerLine = textBoxSymbolsPerLine.Text;
            Properties.Settings.Default.UseShaderInAutonomode = checkBoxAutonomodeShader.IsChecked.Value;
            Properties.Settings.Default.DisplayCodePage = textBoxDisplayCodepage.Text;
            Properties.Settings.Default.UseSystemTone = checkBoxUseSystemTone.IsChecked.Value;
            Properties.Settings.Default.UseOPOSScanner = checkBoxUseOPOSScanner.IsChecked.Value;
            Properties.Settings.Default.UseWindowsDriverForPrinting = checkBoxUseWindowsDriverForPrinting.IsChecked.Value;
            Properties.Settings.Default.LeftMargin = Convert.ToInt32(textBoxLeftMargin.Text);
            Properties.Settings.Default.RightMargin = Convert.ToInt32(textBoxRightMargin.Text);
            Properties.Settings.Default.DescriptionWidth = Convert.ToInt32(textBoxDescriptionWidth.Text);
            Properties.Settings.Default.SumWidth = Convert.ToInt32(textBoxSumWidth.Text);
            Properties.Settings.Default.FontB = checkBoxUseFontB.IsChecked.Value;
            Properties.Settings.Default.SalesOutPath = textBoxSalesOutPath.Text;
            Properties.Settings.Default.SalesOutBackupPath = textBoxSalesOutBackupPath.Text;
            Properties.Settings.Default.ShutDownTime = textBoxShutDownTime.Text;

            Properties.Settings.Default.Save();

            Owner.Show();
            //  сделаем окно-владелец (окно меню) непрозрачным (видимым)
            DoubleAnimation doubleAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
            //  стартуем анимацию непрозрачности
            Owner.BeginAnimation(OpacityProperty, doubleAnimation);
            Close();
        }

        private void ButtonCloseSettings_Click(object sender, RoutedEventArgs e)
        {
            Owner.Show();
            //  сделаем окно-владелец (окно меню) непрозрачным (видимым)
            DoubleAnimation doubleAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
            //  стартуем анимацию непрозрачности
            Owner.BeginAnimation(OpacityProperty, doubleAnimation);
            Close();
        }
    }
}
