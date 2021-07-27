using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using POS.Devices;

namespace POS3K
{
    public static class Printer
    {
        public static bool Initialize()
        {
            MainWindow.printer = new OPOSPOSPrinterClass();
            while (IsCoverOpen())
                MessageBox.Show("Проверьте чековую ленту и закройте крышку принтера!");
            MainWindow.printer.Open("Printer");
            while (IsCoverOpen())
                MessageBox.Show("Проверьте чековую ленту и закройте крышку принтера!");
            MainWindow.printer.ClaimDevice(1000);
            while (IsCoverOpen())
                MessageBox.Show("Проверьте чековую ленту и закройте крышку принтера!");
            if (MainWindow.printer.CheckHealth(1) == 105)
            {
                MainWindow.printer.DeviceEnabled = true;
                MainWindow.printer.AsyncMode = true;

                SetRegularFont();

                return true;
            }
            else
            {
                while (IsCoverOpen())
                    MessageBox.Show("Проверьте чековую ленту и закройте крышку принтера!");
                return false;
            }
        }

        public static void BeginTransaction()
        {
            while (IsCoverOpen())
                MessageBox.Show("Проверьте чековую ленту и закройте крышку принтера!");

            MainWindow.printer.TransactionPrint(2, 11);
        }

        public static void EndTransaction()
        {
            while (IsCoverOpen())
                MessageBox.Show("Проверьте чековую ленту и закройте крышку принтера!");

            MainWindow.printer.TransactionPrint(2, 12);
        }

        public static void SetRegularFont()
        {
            while (IsCoverOpen())
                MessageBox.Show("Проверьте чековую ленту и закройте крышку принтера!");

            MainWindow.printer.CharacterSet = Convert.ToInt32(Properties.Settings.Default.RegularFont);  // 102;
            MainWindow.printer.PrintNormal(2, "\x1b|2fT");
        }

        public static void SetBoldFont()
        {
            while (IsCoverOpen())
                MessageBox.Show("Проверьте чековую ленту и закройте крышку принтера!");

            MainWindow.printer.CharacterSet = Convert.ToInt32(Properties.Settings.Default.BoldFont);   //  103;
            MainWindow.printer.PrintNormal(2, "\x1b|2fT");
        }

        public static void Print(string text)
        {
            while (IsCoverOpen())
                MessageBox.Show("Проверьте чековую ленту и закройте крышку принтера!");

            MainWindow.printer.PrintNormal(2, "\x1b|2fT" + text);
        }

        public static void PrintString(string text)
        {
            while (IsCoverOpen())
                MessageBox.Show("Проверьте чековую ленту и закройте крышку принтера!");

            MainWindow.printer.PrintNormal(2, "\x1b|2fT" + text + "\n");
        }

        public static bool IsCoverOpen()
        {
            return MainWindow.printer.CoverOpen;
        }

        public static void CutPaper(int percent = 70)
        {
            while (IsCoverOpen())
                MessageBox.Show("Проверьте чековую ленту и закройте крышку принтера!");

            MainWindow.printer.CutPaper(percent);
        }

        public static void SetRegistanBitmap()
        {
            return;
            //MainWindow.printer.SetBitmap(1, 2, "LogoPaint.bmp", 500, -2);
            //MainWindow.printer.SetBitmap(1, 2, "LogoPaint.bmp", 300, -2);
            MainWindow.printer.SetBitmap(1, 2, "Text3.bmp", 500, -2);
        }

        //public static void SetElasticaBitmap()
        //{
        //    MainWindow.printer.SetBitmap(2, 2, "elastica.bmp", 500, -2);
        //}

        public static void PrintRegistanBitmap()
        {
            return;
            //SetBoldFont();
            MainWindow.printer.PrintNormal(2, System.Text.ASCIIEncoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'1', (byte)'B' }));
            //MainWindow.printer.PrintNormal(2, "\x0a\x1b|1hC\x1b|1vC\x0a\x0a\x1b|1C\x1b|2CREGISTON SUPERMARKET\n");
            //MainWindow.printer.PrintNormal(2, "\x0a\x1b|1hC\x1b|1vC\x0a\x0a\x1b|1C\x1b|4CREGISTON SUPERMARKET\n");
            //SetRegularFont();
        }

        //public static void PrintElasticaBitmap()
        //{
        //    MainWindow.printer.PrintNormal(2, System.Text.ASCIIEncoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'2', (byte)'B' }));
        //}

        public static string FormatString(ActivePosition position)
        {
            //string description = position.DescriptionLat;
            string description = position.Description;
            double qty = position.Quantity;
            double price = position.Price;
            double sum = position.Sum;
            int VATRate = position.VATRate;
            double VATSum = position.VATSum;

            int width = 44;
            try
            {
                width = Convert.ToInt32(Properties.Settings.Default.SymbolsPerLine);
            }
            catch { }

            //MessageBox.Show(position.DescriptionLat);
            //MessageBox.Show(position.Description);

            if (qty < -1 || qty > 1)
            {
                string s = description + " " + qty.ToString() + "*" + price.ToString() + " \x1b|bC" + sum.ToString();
                if (s.Length < width + 4)
                {
                    string spaces = "";
                    if (width == 38)
                        for (int i = 0; i < width - s.Length + 4; i++)
                            spaces += " ";
                    else if (width == 44)
                        for (int i = 0; i < width - s.Length - 1; i++)
                            spaces += " ";
                    //s = description + " " + spaces + qty.ToString() + "*" + price.ToString() + " So'm = " + sum.ToString() + " So'm";
                    if (width == 38)
                    {
                        if (sum >= 0)
                            s = description + " " + spaces + qty.ToString() + "*" + price.ToString() + " \x1b|bC" + sum.ToString();
                        else
                            s = "\x1b|bC" + description + " " + spaces + qty.ToString() + "*" + price.ToString() + " " + sum.ToString();
                    }
                    else if (width == 44)
                    {
                        if (sum >= 0)
                            s = description + " " + spaces + qty.ToString() + "*" + price.ToString() + " \x1b|bC" + sum.ToString() + "\x1b|N" + " So'm";
                        else
                            s = "\x1b|bC" + description + " " + spaces + qty.ToString() + "*" + price.ToString() + " " + sum.ToString() + "\x1b|N" + " So'm";
                    }
                    if (VATRate > 0)
                    {
                        //VATSum = Math.Round(sum / (100 + VATRate) * VATRate, 2);
                        s = s + "\x1b|N\n   в т.ч. НДС " + VATRate + "% = " + VATSum.ToString();
                    }
                }

                return s;
            }
            else
            {
                string s;
                //s = description + " " + sum.ToString() + " So'm";
                if (sum >= 0)
                    s = description + " \x1b|bC" + sum.ToString();
                else
                    s = "\x1b|bC" + description + " " + sum.ToString();
                if (s.Length < width + 4)
                {
                    string spaces = "";
                    if (width == 38)
                        for (int i = 0; i < width - s.Length + 4; i++)
                            spaces += " ";
                    else if (width == 44)
                        for (int i = 0; i < width - s.Length - 1; i++)
                            spaces += " ";
                    //s = description + " " + spaces + sum.ToString() + " So'm";
                    if (width == 38)
                    {
                        if (sum >= 0)
                            s = description + " " + spaces + "\x1b|bC" + sum.ToString();
                        else
                            s = "\x1b|bC" + description + " " + spaces + sum.ToString();
                    }
                    else if (width == 44)
                    {
                        if (sum >= 0)
                            s = description + " " + spaces + "\x1b|bC" + sum.ToString() + "\x1b|N" + " So'm";
                        else
                            s = "\x1b|bC" + description + " " + spaces + sum.ToString() + "\x1b|N" + " So'm";
                    }
                }
                if (VATRate > 0)
                {
                    //VATSum = Math.Round(sum / (100 + VATRate) * VATRate, 2);
                    s = s + "\x1b|N\n   в т.ч. НДС " + VATRate + "% = " + VATSum.ToString();
                }

                return s;
            }
        }

        public static string FormatTwoStrings(string s1, string s2)
        {
            int width = 44;
            try
            {
                width = Convert.ToInt32(Properties.Settings.Default.SymbolsPerLine);
            }
            catch { }

            string s = s1 + s2;
            if (s.Length < width)
            {
                string spaces = "";
                for (int i = 0; i < width - s.Length; i++)
                    spaces += " ";
                s = s1 + spaces + s2;
            }

            return s;
        }

        public static string CenterText(string s)
        {
            int width = 44;
            try
            {
                width = Convert.ToInt32(Properties.Settings.Default.SymbolsPerLine);
            }
            catch { }

            s.Trim();
            if (s.Length < width)
            {
                int delta = (width - s.Length) / 2;
                string spaces = "";
                for (int i = 0; i < delta; i++)
                    spaces += " ";
                s = spaces + s + spaces;
            }

            return s;
        }

        public static bool PrintViaDriver()
        {
            PrintDialog printDialog = new PrintDialog();
            //if (printDialog.ShowDialog() == true)
            //{
            // Создать визуальный элемент для страницы
            DrawingVisual visual = new DrawingVisual();

            // Получить контекст рисования
            using (DrawingContext dc = visual.RenderOpen())
            {
                // Определить текст, который необходимо печатать
                //FormattedText text = new FormattedText(@"Закон Лоренца - 'Если ладонь левой руки поместить между полюсами магнита так, чтобы в неё входили силовые линии магнитного поля, а вытянутые пальцы расположить по направлению тока, то отогнутый большой палец покажет направление отклонения проводника с током Ruz_Salyami Setka ve  360*50,550 So'm = 18198,00 So'm",
                FormattedText text = new FormattedText(@"Закон Лоренца - 'Если ладонь левой руки поместить между полюсами магнита так, чтобы в неё входили силовые линии магнитного поля, а вытянутые пальцы расположить по направлению тока, то отогнутый большой палец покажет направление отклонения Ruz_Salyami Setka ve  360*50,550 So'm = 18198,00 So'm",
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Courier New"), 10, Brushes.Black);

                // Указать максимальную ширину, в пределах которой выполнять перенос текста, 
                text.MaxTextWidth = printDialog.PrintableAreaWidth - 30;
                text.LineHeight = 10;
                //text.SetFontFamily("Courier New", 75, 10);
                //text.SetFontStyle(FontStyles.Oblique, 30, 15);
                text.TextAlignment = TextAlignment.Right;
                text.SetFontWeight(FontWeights.Bold);
                text.SetFontWeight(FontWeights.Black, 50, 20);

                // Получить размер выводимого текста. 
                Size textSize = new Size(text.Width, text.Height);

                // Найти верхний левый угол, куда должен быть помещен текст. 
                double margin = 15;
                Point point = new Point(margin, 0);
                    //(printDialog.PrintableAreaWidth - textSize.Width) / 2 - margin,
                    //(printDialog.PrintableAreaHeight - textSize.Height) / 2 - margin);

                // Нарисовать содержимое, 
                dc.DrawText(text, point);

                point = new Point(margin, textSize.Height + 10);
                text.SetFontSize(11);
                dc.DrawText(text, point);

                // Добавить рамку (прямоугольник без фона). 
                //dc.DrawRectangle(null, new Pen(Brushes.Black, 1),
                //    new Rect(margin, margin, printDialog.PrintableAreaWidth - margin * 2,
                //        printDialog.PrintableAreaHeight - margin * 2));
            }

            // Напечатать визуальный элемент. 
            printDialog.PrintVisual(visual, "Печать с помощью классов визуального уровня");
            //}

            return true;
        }

        public static void FormatAndPrintStringB(ActivePosition position)
        {
            string description = position.Description;
            double qty = position.Quantity;
            double price = position.Price;
            double sum = position.Sum;
            int VATRate = position.VATRate;
            double VATSum = position.VATSum;

            string s1, s2, s3;

            if (qty < -1 || qty > 1)
            {
                if (sum >= 0)
                {
                    s1 = description;
                    //s2 = "\x1b|bC" + qty.ToString() + "\x1b|N\x1b|rA*" + price.ToString() + "= \x1b|bC" + sum.ToString() + "\x1b|N\x1b|rA So'm";
                    s2 = "\x1b|1C" + qty.ToString() + "\x1b|N\x1b|rA*" + price.ToString() + "= \x1b|1C" + sum.ToString() + "\x1b|N\x1b|rA So'm";
                }
                else
                {
                    s1 = "\x1b|bC" + description;
                    s2 = "\x1b|bC" + qty.ToString() + "*" + price.ToString() + "= " + sum.ToString() + "\x1b|rA So'm";
                }
                PrintString(s1 + "\x1b|rA" + s2);
                if (VATRate > 0)
                {
                    s3 = "\x1b|N   в т.ч. НДС " + VATRate + "% = " + VATSum.ToString();
                    PrintString(s3);
                }
            }
            else
            {
                if (sum >= 0)
                {
                    s1 = description;
                    //s2 = "\x1b|bC" + sum.ToString() + "\x1b|N\x1C|rA So'm";
                    s2 = "\x1b|1C" + sum.ToString() + "\x1b|N\x1b|rA So'm";
                }
                else
                {
                    s1 = "\x1b|bC" + description;
                    s2 = "\x1b|bC" + sum.ToString() + "\x1b|N\x1b|rA So'm";
                }
                PrintString(s1 + "\x1b|rA" + s2);
                if (VATRate > 0)
                {
                    s3 = "\x1b|N   в т.ч. НДС " + VATRate + "% = " + VATSum.ToString();
                    PrintString(s3);
                }
            }
        }

        public static void FormatAndPrintTwoStringsB(string s1, string s2)
        {
            PrintString(s1 + "\x1b|rA" + s2);
        }

        public static void CenterAndPrintTextB(string s)
        {
            s.Trim();
            PrintString("\x1b|cA" + s);
        }
    }
}
