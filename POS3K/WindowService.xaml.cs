using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для WindowService.xaml
    /// </summary>
    public partial class WindowService : Window
    {
        Fiscal fiscal = new Fiscal();
        string fiscalDate;
        string fiscalTime;
        string ownerCode;
        string regCode;
        string serialNumber;

        public WindowService()
        {
            InitializeComponent();
            this.Cursor = Cursors.None;
            Activated += WindowService_Activated;
        }

        private void WindowService_Activated(object sender, EventArgs e)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
            BeginAnimation(OpacityProperty, doubleAnimation);
            btnMainMenu1.RaiseEvent(new RoutedEventArgs(LoadedEvent));
            btnMainMenu2.RaiseEvent(new RoutedEventArgs(LoadedEvent));
            btnMainMenu3.RaiseEvent(new RoutedEventArgs(LoadedEvent));
            btnMainMenu4.RaiseEvent(new RoutedEventArgs(LoadedEvent));
            btnMainMenu5.RaiseEvent(new RoutedEventArgs(LoadedEvent));
            btnMainMenu6.RaiseEvent(new RoutedEventArgs(LoadedEvent));
            btnMainMenu7.RaiseEvent(new RoutedEventArgs(LoadedEvent));

            if (fiscal.OpenRead())
            {
                if (fiscal.GetHeader())
                {
                    if (fiscal.fiscalHeader.fiscalMode[0] != '1')
                    {
                        btnMainMenu1.Content = "1. Фискализация";
                    }
                    else
                    {
                        btnMainMenu1.Content = "1. Перерегистрация";
                    }

                    if (fiscal.GetUsers())
                    {
                        fiscalDate = new string(fiscal.fiscalUser[fiscal.currentUser - 1].fiscalDate);
                        fiscalTime = new string(fiscal.fiscalUser[fiscal.currentUser - 1].fiscalTime);
                        ownerCode = new string(fiscal.fiscalUser[fiscal.currentUser - 1].ownerCode) + "\n";
                        regCode = new string(fiscal.fiscalUser[fiscal.currentUser - 1].regCode) + "\n";
                        serialNumber = Properties.Settings.Default.SerialNumber;
                    }
                    else
                    {
                        fiscal.Close();
                        MessageBox.Show("Ошибка фискального модуля!");
                        return;
                    }
                }
                else
                {
                    fiscal.Close();
                    MessageBox.Show("Ошибка фискального модуля!");
                    return;
                }
                fiscal.Close();
            }
            else
            {
                MessageBox.Show("Ошибка фискального модуля!");
                return;
            }
        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            Topmost = true;
        }

        private void BtnMainMenu7_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Owner.Activate();
            this.Owner.Opacity = 1;
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
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
        }

        private void BtnMainMenu1_Click(object sender, RoutedEventArgs e)   //  Фискализация/перерегистрация
        {
            WindowRegistration windowRegistration = new WindowRegistration();

            if (windowRegistration.ShowDialog() == true)
            {
                if (fiscal.currentUser == 9)
                {
                    MessageBox.Show("Количество перерегистраций исчерпано");
                    return;
                }
                if (fiscal.OpenWrite())
                {
                    if (fiscal.SeekNewReg())
                    {
                        string regNumber = windowRegistration.RegNumber;
                        string ownerCode = windowRegistration.OwnerCode;
                        string date, time;
                        DateTime dateTime = DateTime.Now;

                        date = dateTime.Date.ToString();
                        date = date.Substring(0, 2) + date.Substring(3, 2) + date.Substring(6, 4);

                        time = dateTime.TimeOfDay.ToString();
                        time = time.Substring(0, 2) + time.Substring(3, 2) + time.Substring(6, 2);

                        while (regNumber.Length < 13)
                            regNumber = "0" + regNumber;
                        while (ownerCode.Length < 12)
                            ownerCode = "0" + ownerCode;
                        if (fiscal.WriteNewReg(regNumber, ownerCode, date, time))
                        {
                            Properties.Settings.Default.INN = ownerCode;
                            //Properties.Settings.Default.SerialNumber = regNumber;
                            Properties.Settings.Default.Save();

                            fiscal.Close();
                            if (fiscal.OpenRead())
                            {
                                if (fiscal.GetHeader())
                                {
                                    if (fiscal.GetUsers())
                                    {
                                        fiscalDate = new string(fiscal.fiscalUser[fiscal.currentUser - 1].fiscalDate);
                                        fiscalTime = new string(fiscal.fiscalUser[fiscal.currentUser - 1].fiscalTime);
                                        ownerCode = new string(fiscal.fiscalUser[fiscal.currentUser - 1].ownerCode) + "\n";
                                        regCode = new string(fiscal.fiscalUser[fiscal.currentUser - 1].regCode) + "\n";
                                        serialNumber = Properties.Settings.Default.SerialNumber;
                                    }
                                    else
                                    {
                                        fiscal.Close();
                                        MessageBox.Show("Ошибка фискального модуля!");
                                        return;
                                    }
                                }
                                else
                                {
                                    fiscal.Close();
                                    MessageBox.Show("Ошибка фискального модуля!");
                                    return;
                                }
                                fiscal.Close();
                            }
                            else
                            {
                                MessageBox.Show("Ошибка фискального модуля!");
                                return;
                            }

                            int width = 44;
                            try
                            {
                                width = Convert.ToInt32(Properties.Settings.Default.SymbolsPerLine);
                            }
                            catch { }

                            Printer.SetRegularFont();
                            Printer.BeginTransaction();
                            if (fiscal.currentUser == 0)
                            {
                                if (width == 38)
                                    Printer.PrintString("          FISKALIZATSIYASI            ");
                                else
                                    Printer.PrintString("             FISKALIZATSIYASI               ");
                            }
                            else
                            {
                                if (width == 38)
                                    Printer.PrintString("         PEREREGISTRATSIYASI          ");
                                else
                                    Printer.PrintString("            PEREREGISTRATSIYASI             ");
                            }
                            if (width == 38)
                                Printer.PrintString("**************************************");
                            else
                                Printer.PrintString("********************************************");
                            Printer.PrintString(Properties.Settings.Default.CompanyName);
                            Printer.PrintString(Properties.Settings.Default.CompanyAddress);
                            Printer.PrintString("INN: " + ownerCode);
                            Printer.PrintString("");
                            if (width == 38)
                                Printer.PrintString("--------------------------------------");
                            else
                                Printer.PrintString("--------------------------------------------");
                            Printer.PrintString("S/N KKM: " + serialNumber);
                            Printer.PrintString("REG. No: " + regCode);
                            Printer.PrintString("");
                            fiscalDate = new string(fiscal.fiscalUser[fiscal.currentUser - 1].fiscalDate);
                            fiscalDate = fiscalDate.Substring(0, 2) + "." + fiscalDate.Substring(2, 2) + "." + fiscalDate.Substring(4, 4);
                            Printer.PrintString("FISKAL DATA: " + fiscalDate);
                            if (width == 38)
                                Printer.PrintString("--------------------------------------");
                            else
                                Printer.PrintString("--------------------------------------------");
                            if (width == 38)
                                Printer.PrintString("     KASSA FISKALIZATSIYALANGAN       ");
                            else
                                Printer.PrintString("        KASSA FISKALIZATSIYALANGAN          ");
                            if (width == 38)
                                Printer.PrintString("**************************************");
                            else
                                Printer.PrintString("********************************************");
                            Printer.EndTransaction();
                        }
                        else
                            MessageBox.Show("Ошибка фискализации/перерегистрации");
                    }
                    fiscal.Close();
                }
            }
        }

        private void BtnMainMenu2_Click(object sender, RoutedEventArgs e)
        {
            Display.ClearText();
            Display.DisplayText("SERVICE");

            int width = 44;
            try
            {
                width = Convert.ToInt32(Properties.Settings.Default.SymbolsPerLine);
            }
            catch { }

            Printer.SetRegularFont();
            Printer.BeginTransaction();
            if (width == 38)
                Printer.PrintString("**************************************");
            else
                Printer.PrintString("********************************************");
            Printer.PrintString(Properties.Settings.Default.CompanyName);
            Printer.PrintString(Properties.Settings.Default.CompanyAddress);
            Printer.PrintString("INN: " + ownerCode);
            Printer.PrintString("");
            Printer.PrintString("FISKAL XISOBOTI");
            Printer.PrintString("REPORT DATA: " + DateTime.Now.ToString());
            if (width == 38)
                Printer.PrintString("--------------------------------------");
            else
                Printer.PrintString("--------------------------------------------");
            Printer.PrintString("S/N KKM: " + serialNumber);
            Printer.PrintString("REG. No: " + regCode);
            Printer.PrintString("");
            fiscalDate = new string(fiscal.fiscalUser[fiscal.currentUser - 1].fiscalDate);
            fiscalDate = fiscalDate.Substring(0, 2) + "." + fiscalDate.Substring(2, 2) + "." + fiscalDate.Substring(4, 4);
            Printer.PrintString("FISKAL DATA: " + fiscalDate);
            if (width == 38)
                Printer.PrintString("--------------------------------------");
            else
                Printer.PrintString("--------------------------------------------");
            if (width == 38)
                Printer.PrintString("           FISKAL REZHIMI             ");
            else
                Printer.PrintString("              FISKAL REZHIMI                ");
            if (width == 38)
                Printer.PrintString("**************************************");
            else
                Printer.PrintString("********************************************");
            Printer.EndTransaction();

            //Printer.PrintString(Printer.FormatTwoStrings("OWNER CODE:", ownerCode));
            //Printer.PrintString(Printer.FormatTwoStrings("REG CODE:", regCode));
            //Printer.PrintString(Printer.FormatTwoStrings("FISCAL DATE:", fiscalDate));
            //Printer.PrintString(Printer.FormatTwoStrings("FISCAL TIME:", fiscalTime));
        }

        private void BtnMainMenu3_Click(object sender, RoutedEventArgs e)   //  Краткий между заданными датами
        {
            string startDate, endDate;
            string total, vat7, vat10, vat15;
            string fiscalDate;

            WindowDatesInterval windowDatesInterval = new WindowDatesInterval();
            if (windowDatesInterval.ShowDialog() == true)
            {
                startDate = windowDatesInterval.StartDate.Replace(".", "");
                endDate = windowDatesInterval.EndDate.Replace(".", "");
                if (fiscal.GetFiscalReportByDates("short", startDate, endDate))
                {
                    int width = 44;
                    try
                    {
                        width = Convert.ToInt32(Properties.Settings.Default.SymbolsPerLine);
                    }
                    catch { }

                    Printer.SetRegularFont();
                    Printer.BeginTransaction();
                    if (width == 38)
                        Printer.PrintString("**************************************");
                    else
                        Printer.PrintString("********************************************");
                    Printer.PrintString(Properties.Settings.Default.CompanyName);
                    Printer.PrintString(Properties.Settings.Default.CompanyAddress);
                    Printer.PrintString("INN: " + ownerCode);
                    Printer.PrintString("");
                    Printer.PrintString("REPORT DATA: " + DateTime.Now.ToString());
                    if (width == 38)
                        Printer.PrintString("--------------------------------------");
                    else
                        Printer.PrintString("--------------------------------------------");
                    if (width == 38)
                        Printer.PrintString("            FISKALIZACIA              ");
                    else
                        Printer.PrintString("               FISKALIZACIA                 ");
                    Printer.PrintString("S/N KKM: " + Properties.Settings.Default.SerialNumber);
                    Printer.PrintString("REG. No: " + regCode);
                    Printer.PrintString("");
                    fiscalDate = new string(fiscal.fiscalUser[fiscal.currentUser - 1].fiscalDate);
                    fiscalDate = fiscalDate.Substring(0, 2) + "." + fiscalDate.Substring(2, 2) + "." + fiscalDate.Substring(4, 4);
                    Printer.PrintString("FISKAL DATA: " + fiscalDate);
                    if (width == 38)
                        Printer.PrintString("--------------------------------------");
                    else
                        Printer.PrintString("--------------------------------------------");
                    Printer.PrintString("PERIOD: " + windowDatesInterval.StartDate + "-" + windowDatesInterval.EndDate);
                    Printer.PrintString("KUND. XISOBOT");
                    if (width == 38)
                        Printer.PrintString("--------------------------------------");
                    else
                        Printer.PrintString("--------------------------------------------");
                    using (StreamReader reader = new StreamReader("queryds.out", encoding: Encoding.Default))
                    {
                        total = reader.ReadLine();
                        vat7 = reader.ReadLine();
                        vat10 = reader.ReadLine();
                        vat15 = reader.ReadLine();
                    }
                    Printer.PrintString("XAMMASI SOTMOK ITOGI: " + total);
                    Printer.PrintString("S.J. QQS 15%: " + vat15);
                    if (width == 38)
                        Printer.PrintString("--------------------------------------");
                    else
                        Printer.PrintString("--------------------------------------------");
                    if (width == 38)
                        Printer.PrintString("           FISKAL REZHIMI             ");
                    else
                        Printer.PrintString("              FISKAL REZHIMI                ");
                    if (width == 38)
                        Printer.PrintString("**************************************");
                    else
                        Printer.PrintString("********************************************");
                    Printer.EndTransaction();
                }
            }
        }

        private void BtnMainMenu4_Click(object sender, RoutedEventArgs e)   //  Краткий между заданными номерами отчетов
        {
            string startNumber, endNumber;
            string total, vat7, vat10, vat15;
            string fiscalDate;
            int fromNumber, toNumber;

            WindowNumbersInterval windowNumbersInterval = new WindowNumbersInterval();
            if (windowNumbersInterval.ShowDialog() == true)
            {
                startNumber = windowNumbersInterval.StartNumber;
                endNumber = windowNumbersInterval.EndNumber;
                try
                {
                    fromNumber = Convert.ToInt32(startNumber);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                try
                {
                    toNumber = Convert.ToInt32(endNumber);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                if (fromNumber > toNumber)
                {
                    MessageBox.Show("Проверьте номера отчетов");
                    return;
                }
                if (fiscal.GetFiscalReportByNumbers("short", startNumber, endNumber))
                {
                    int width = 44;
                    try
                    {
                        width = Convert.ToInt32(Properties.Settings.Default.SymbolsPerLine);
                    }
                    catch { }

                    Printer.SetRegularFont();
                    Printer.BeginTransaction();
                    if (width == 38)
                        Printer.PrintString("**************************************");
                    else
                        Printer.PrintString("********************************************");
                    Printer.PrintString(Properties.Settings.Default.CompanyName);
                    Printer.PrintString(Properties.Settings.Default.CompanyAddress);
                    Printer.PrintString("INN: " + ownerCode);
                    Printer.PrintString("");
                    Printer.PrintString("REPORT DATA: " + DateTime.Now.ToString());
                    if (width == 38)
                        Printer.PrintString("--------------------------------------");
                    else
                        Printer.PrintString("--------------------------------------------");
                    if (width == 38)
                        Printer.PrintString("            FISKALIZACIA              ");
                    else
                        Printer.PrintString("               FISKALIZACIA                 ");
                    Printer.PrintString("S/N KKM: " + Properties.Settings.Default.SerialNumber);
                    Printer.PrintString("REG. No: " + regCode);
                    Printer.PrintString("");
                    fiscalDate = new string(fiscal.fiscalUser[fiscal.currentUser - 1].fiscalDate);
                    fiscalDate = fiscalDate.Substring(0, 2) + "." + fiscalDate.Substring(2, 2) + "." + fiscalDate.Substring(4, 4);
                    Printer.PrintString("FISKAL DATA: " + fiscalDate);
                    if (width == 38)
                        Printer.PrintString("--------------------------------------");
                    else
                        Printer.PrintString("--------------------------------------------");
                    Printer.PrintString("KUND. XISOBOTLAR: " + startNumber + "-" + endNumber);
                    if (width == 38)
                        Printer.PrintString("--------------------------------------");
                    else
                        Printer.PrintString("--------------------------------------------");
                    using (StreamReader reader = new StreamReader("queryns.out", encoding: Encoding.Default))
                    {
                        total = reader.ReadLine();
                        vat7 = reader.ReadLine();
                        vat10 = reader.ReadLine();
                        vat15 = reader.ReadLine();
                    }
                    Printer.PrintString("XAMMASI SOTMOK ITOGI: " + total);
                    Printer.PrintString("S.J. QQS 15%: " + vat15);
                    if (width == 38)
                        Printer.PrintString("--------------------------------------");
                    else
                        Printer.PrintString("--------------------------------------------");
                    if (width == 38)
                        Printer.PrintString("           FISKAL REZHIMI             ");
                    else
                        Printer.PrintString("              FISKAL REZHIMI                ");
                    if (width == 38)
                        Printer.PrintString("**************************************");
                    else
                        Printer.PrintString("********************************************");
                    Printer.EndTransaction();
                }
            }
        }

        private void BtnMainMenu5_Click(object sender, RoutedEventArgs e)   //  Подробный между заданными датами
        {
            string startDate, endDate;
            string reportDate, reportNumber, reportTotal, reportVat7, reportVat10, reportVat15;
            double total = 0, vat7 = 0, vat10 = 0, vat15 = 0;
            string fiscalDate;

            WindowDatesInterval windowDatesInterval = new WindowDatesInterval();
            if (windowDatesInterval.ShowDialog() == true)
            {
                startDate = windowDatesInterval.StartDate.Replace(".", "");
                endDate = windowDatesInterval.EndDate.Replace(".", "");
                if (fiscal.GetFiscalReportByDates("detailed", startDate, endDate))
                {
                    int width = 44;
                    try
                    {
                        width = Convert.ToInt32(Properties.Settings.Default.SymbolsPerLine);
                    }
                    catch { }

                    Printer.SetRegularFont();
                    Printer.BeginTransaction();
                    if (width == 38)
                        Printer.PrintString("**************************************");
                    else
                        Printer.PrintString("********************************************");
                    Printer.PrintString(Properties.Settings.Default.CompanyName);
                    Printer.PrintString(Properties.Settings.Default.CompanyAddress);
                    Printer.PrintString("INN: " + ownerCode);
                    Printer.PrintString("");
                    Printer.PrintString("REPORT DATA: " + DateTime.Now.ToString());
                    if (width == 38)
                        Printer.PrintString("--------------------------------------");
                    else
                        Printer.PrintString("--------------------------------------------");
                    if (width == 38)
                        Printer.PrintString("            FISKALIZACIA              ");
                    else
                        Printer.PrintString("               FISKALIZACIA                 ");
                    Printer.PrintString("S/N KKM: " + Properties.Settings.Default.SerialNumber);
                    Printer.PrintString("REG. No: " + regCode);
                    Printer.PrintString("");
                    fiscalDate = new string(fiscal.fiscalUser[fiscal.currentUser - 1].fiscalDate);
                    fiscalDate = fiscalDate.Substring(0, 2) + "." + fiscalDate.Substring(2, 2) + "." + fiscalDate.Substring(4, 4);
                    Printer.PrintString("FISKAL DATA: " + fiscalDate);
                    if (width == 38)
                        Printer.PrintString("--------------------------------------");
                    else
                        Printer.PrintString("--------------------------------------------");
                    Printer.PrintString("PERIOD: " + windowDatesInterval.StartDate + "-" + windowDatesInterval.EndDate);
                    if (width == 38)
                        Printer.PrintString("--------------------------------------");
                    else
                        Printer.PrintString("--------------------------------------------");
                    using (StreamReader reader = new StreamReader("querydd.out", encoding: Encoding.Default))
                    {
                        while (!reader.EndOfStream)
                        {
                            reportDate = reader.ReadLine();
                            reportDate = reportDate.Substring(0, 2) + "." + reportDate.Substring(2, 2) + "." + reportDate.Substring(4, 4);
                            reportNumber = reader.ReadLine();
                            reportTotal = reader.ReadLine();
                            reportVat7 = reader.ReadLine();
                            reportVat10 = reader.ReadLine();
                            reportVat15 = reader.ReadLine();
                            total += Convert.ToDouble(reportTotal);
                            try
                            {
                                vat7 += Convert.ToDouble(reportVat7);
                            }
                            catch { }
                            try
                            {
                                vat10 += Convert.ToDouble(reportVat10);
                            }
                            catch { }
                            try
                            {
                                vat15 += Convert.ToDouble(reportVat15);
                            }
                            catch { }
                            Printer.PrintString("KUND. XISOBOT: No." + reportNumber + " " + reportDate);
                            //Printer.PrintString("FISKAL ITOGI:  \x1b|bC" + reportTotal + "\x1b|N SO'M");
                            Printer.PrintString("FISKAL ITOGI:  " + reportTotal + " SO'M");
                            if (reportVat7 != "-" && reportVat7 != "0")
                                Printer.PrintString("S.J. QQS 7%:   " + reportVat7 + " SO'M");
                            if (reportVat10 != "-" && reportVat10 != "0")
                                Printer.PrintString("S.J. QQS 10%:  " + reportVat10 + " SO'M");
                            if (reportVat15 != "-")
                                Printer.PrintString("S.J. QQS 15%:  " + reportVat15 + " SO'M");
                        }
                    }
                    if (width == 38)
                        Printer.PrintString("--------------------------------------");
                    else
                        Printer.PrintString("--------------------------------------------");
                    Printer.PrintString("XAMMASI SOTMOK ITOGI: " + total + " SO'M");
                    if (vat7 > 0)
                        Printer.PrintString("S.J. QQS 7%: " + vat7 + " SO'M");
                    if (vat10 > 0)
                        Printer.PrintString("S.J. QQS 10%: " + vat10 + " SO'M");
                    Printer.PrintString("S.J. QQS 15%: " + vat15 + " SO'M");
                    if (width == 38)
                        Printer.PrintString("--------------------------------------");
                    else
                        Printer.PrintString("--------------------------------------------");
                    if (width == 38)
                        Printer.PrintString("           FISKAL REZHIMI             ");
                    else
                        Printer.PrintString("              FISKAL REZHIMI                ");
                    if (width == 38)
                        Printer.PrintString("**************************************");
                    else
                        Printer.PrintString("********************************************");
                    Printer.EndTransaction();
                }
            }
        }

        private void BtnMainMenu6_Click(object sender, RoutedEventArgs e)   //  Подробный между заданными номерами отчетов
        {
            string startNumber, endNumber;
            int fromNumber, toNumber;
            string reportDate, reportNumber, reportTotal, reportVat7, reportVat10, reportVat15;
            double total = 0, vat7 = 0, vat10 = 0, vat15 = 0;
            string fiscalDate;

            WindowNumbersInterval windowNumbersInterval = new WindowNumbersInterval();
            if (windowNumbersInterval.ShowDialog() == true)
            {
                startNumber = windowNumbersInterval.StartNumber;
                endNumber = windowNumbersInterval.EndNumber;
                try
                {
                    fromNumber = Convert.ToInt32(startNumber);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                try
                {
                    toNumber = Convert.ToInt32(endNumber);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                if (fromNumber > toNumber)
                    MessageBox.Show("Проверьте номера");
                if (fiscal.GetFiscalReportByNumbers("detailed", startNumber, endNumber))
                {
                    int width = 44;
                    try
                    {
                        width = Convert.ToInt32(Properties.Settings.Default.SymbolsPerLine);
                    }
                    catch { }

                    Printer.SetRegularFont();
                    Printer.BeginTransaction();
                    if (width == 38)
                        Printer.PrintString("**************************************");
                    else
                        Printer.PrintString("********************************************");
                    Printer.PrintString(Properties.Settings.Default.CompanyName);
                    Printer.PrintString(Properties.Settings.Default.CompanyAddress);
                    Printer.PrintString("INN: " + ownerCode);
                    Printer.PrintString("");
                    Printer.PrintString("REPORT DATA: " + DateTime.Now.ToString());
                    if (width == 38)
                        Printer.PrintString("--------------------------------------");
                    else
                        Printer.PrintString("--------------------------------------------");
                    if (width == 38)
                        Printer.PrintString("            FISKALIZACIA              ");
                    else
                        Printer.PrintString("               FISKALIZACIA                 ");
                    Printer.PrintString("S/N KKM: " + Properties.Settings.Default.SerialNumber);
                    Printer.PrintString("REG. No: " + regCode);
                    Printer.PrintString("");
                    fiscalDate = new string(fiscal.fiscalUser[fiscal.currentUser - 1].fiscalDate);
                    fiscalDate = fiscalDate.Substring(0, 2) + "." + fiscalDate.Substring(2, 2) + "." + fiscalDate.Substring(4, 4);
                    Printer.PrintString("FISKAL DATA: " + fiscalDate);
                    if (width == 38)
                        Printer.PrintString("--------------------------------------");
                    else
                        Printer.PrintString("--------------------------------------------");
                    Printer.PrintString("PERIOD: " + startNumber + "-" + endNumber);
                    if (width == 38)
                        Printer.PrintString("--------------------------------------");
                    else
                        Printer.PrintString("--------------------------------------------");
                    using (StreamReader reader = new StreamReader("querynd.out", encoding: Encoding.Default))
                    {
                        while (!reader.EndOfStream)
                        {
                            reportDate = reader.ReadLine();
                            reportDate = reportDate.Substring(0, 2) + "." + reportDate.Substring(2, 2) + "." + reportDate.Substring(4, 4);
                            reportNumber = reader.ReadLine();
                            reportTotal = reader.ReadLine();
                            reportVat7 = reader.ReadLine();
                            reportVat10 = reader.ReadLine();
                            reportVat15 = reader.ReadLine();
                            total += Convert.ToDouble(reportTotal);
                            try
                            {
                                vat7 += Convert.ToDouble(reportVat7);
                            }
                            catch { }
                            try
                            {
                                vat10 += Convert.ToDouble(reportVat10);
                            }
                            catch { }
                            try
                            {
                                vat15 += Convert.ToDouble(reportVat15);
                            }
                            catch { }
                            Printer.PrintString("KUND. XISOBOT: No." + reportNumber + " " + reportDate);
                            //Printer.PrintString("FISKAL ITOGI:  \x1b|bC" + reportTotal + "\x1b|N SO'M");
                            Printer.PrintString("FISKAL ITOGI:  " + reportTotal + " SO'M");
                            if (reportVat7 != "-" && reportVat7 != "0")
                                Printer.PrintString("S.J. QQS 7%:   " + reportVat7 + " SO'M");
                            if (reportVat10 != "-" && reportVat10 != "0")
                                Printer.PrintString("S.J. QQS 10%:  " + reportVat10 + " SO'M");
                            if (reportVat15 != "-")
                                Printer.PrintString("S.J. QQS 15%:  " + reportVat15 + " SO'M");
                        }
                    }
                    if (width == 38)
                        Printer.PrintString("--------------------------------------");
                    else
                        Printer.PrintString("--------------------------------------------");
                    Printer.PrintString("XAMMASI SOTMOK ITOGI: " + total + " SO'M");
                    if (vat7 > 0)
                        Printer.PrintString("S.J. QQS 7%: " + vat7 + " SO'M");
                    if (vat10 > 0)
                        Printer.PrintString("S.J. QQS 10%: " + vat10 + " SO'M");
                    Printer.PrintString("S.J. QQS 15%: " + vat15 + " SO'M");
                    if (width == 38)
                        Printer.PrintString("--------------------------------------");
                    else
                        Printer.PrintString("--------------------------------------------");
                    if (width == 38)
                        Printer.PrintString("           FISKAL REZHIMI             ");
                    else
                        Printer.PrintString("              FISKAL REZHIMI                ");
                    if (width == 38)
                        Printer.PrintString("**************************************");
                    else
                        Printer.PrintString("********************************************");
                    Printer.EndTransaction();
                }
            }
        }
    }
}
