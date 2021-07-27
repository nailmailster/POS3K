using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using POS.Devices;

namespace POS3K
{
    public static class Display
    {
        public static bool Initialize()
        {
            MainWindow.display = new OPOSLineDisplayClass();
            MainWindow.display.Open("Display");
            MainWindow.display.ClaimDevice(1000);
            if (MainWindow.display.CheckHealth(1) == 105)
            {
                MainWindow.display.DeviceEnabled = true;
                CharacterSetRus();
                return true;
            }
            else
                return false;
        }

        public static void ClearText()
        {
            MainWindow.display.ClearText();
        }

        public static void DisplayText(string text)
        {
            if (Properties.Settings.Default.DisplayCodePage == "866")
                Convert866(ref text);

            MainWindow.display.DisplayText(text, 0);    //  проверь атрибут
        }

        public static void DisplayTextAt(int x, int y, string text)
        {
            if (Properties.Settings.Default.DisplayCodePage == "866")
                Convert866(ref text);

            MainWindow.display.DisplayTextAt(y, x, text, 0);    //  проверь атрибут
        }

        public static void Convert866(ref string text)
        {
            string str = "", s;

            for (int i = 0; i < text.Length; i++)
            {
                s = text.Substring(i, 1);
                switch (s)
                {
                    case "А":
                        //str = str + ((byte)0x80).ToString();
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x80 });
                        break;
                    case "Б":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x81 });
                        break;
                    case "В":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x82 });
                        break;
                    case "Г":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x83 });
                        break;
                    case "Д":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x84 });
                        break;
                    case "Е":
                    case "Ё":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x85 });
                        break;
                    case "Ж":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x86 });
                        break;
                    case "З":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x87 });
                        break;
                    case "И":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x88 });
                        break;
                    case "Й":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x89 });
                        break;
                    case "К":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x8A });
                        break;
                    case "Л":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x8B });
                        break;
                    case "М":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x8C });
                        break;
                    case "Н":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x8D });
                        break;
                    case "О":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x8E });
                        break;
                    case "П":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x8F });
                        break;
                    case "Р":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x90 });
                        break;
                    case "С":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x91 });
                        break;
                    case "Т":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x92 });
                        break;
                    case "У":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x93 });
                        break;
                    case "Ф":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x94 });
                        break;
                    case "Х":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x95 });
                        break;
                    case "Ц":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x96 });
                        break;
                    case "Ч":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x97 });
                        break;
                    case "Ш":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x98 });
                        break;
                    case "Щ":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x99 });
                        break;
                    case "Ъ":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x9A });
                        break;
                    case "Ы":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x9B });
                        break;
                    case "Ь":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x9C });
                        break;
                    case "Э":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x9D });
                        break;
                    case "Ю":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x9E });
                        break;
                    case "Я":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0x9F });
                        break;

                    case "а":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xA0 });
                        break;
                    case "б":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xA1 });
                        break;
                    case "в":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xA2 });
                        break;
                    case "г":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xA3 });
                        break;
                    case "д":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xA4 });
                        break;
                    case "е":
                    case "ё":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xA5 });
                        break;
                    case "ж":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xA6 });
                        break;
                    case "з":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xA7 });
                        break;
                    case "и":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xA8 });
                        break;
                    case "й":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xA9 });
                        break;
                    case "к":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xAA });
                        break;
                    case "л":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xAB });
                        break;
                    case "м":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xAC });
                        break;
                    case "н":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xAD });
                        break;
                    case "о":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xAE });
                        break;
                    case "п":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xAF });
                        break;
                    case "р":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xE0 });
                        break;
                    case "с":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xE1 });
                        break;
                    case "т":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xE2 });
                        break;
                    case "у":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xE3 });
                        break;
                    case "ф":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xE4 });
                        break;
                    case "х":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xE5 });
                        break;
                    case "ц":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xE6 });
                        break;
                    case "ч":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xE7 });
                        break;
                    case "ш":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xE8 });
                        break;
                    case "щ":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xE9 });
                        break;
                    case "ъ":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xEA });
                        break;
                    case "ы":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xEB });
                        break;
                    case "ь":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xEC });
                        break;
                    case "э":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xED });
                        break;
                    case "ю":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xEE });
                        break;
                    case "я":
                        str = str + System.Text.Encoding.Default.GetString(new[] { (byte)0xEF });
                        break;

                    default:
                        str = str + s;
                        break;
                }
            }

            text = str;
        }

        public static void CharacterSetRus()
        {
            try
            {
                int displayCodePage = Convert.ToInt32(Properties.Settings.Default.DisplayCodePage);
                MainWindow.display.CharacterSet = displayCodePage;
            }
            catch
            {
                MainWindow.display.CharacterSet = 855;    //  TOSHIBA
            }
            //MainWindow.display.CharacterSet = 866;    //  не помогло
            MainWindow.display.MapCharacterSet = true;
        }

        public static string FormatTwoStrings(string s1, string s2)
        {
            string s = s1 + " " + s2;

            if (s.Length < 20)
            {
                string spaces = "";
                for (int i = 0; i < 20 - s.Length; i++)
                    spaces += " ";
                s = s1 + " " + spaces + s2;
            }
            else if (s.Length > 20)
            {
                int delta = s.Length - 20;
                s1 = s1.Substring(0, s1.Length - delta);
                s = s1 + " " + s2;
            }

            return s;
        }
    }
}
