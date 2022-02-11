using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;


namespace ShumCalcs
{
	/// <summary>
	/// Логика взаимодействия для App.xaml
	/// </summary>
	public partial class App : Application
	{
    }

    public class UserData // Данные текущего пользователя
    {
        public static string UserName { get; set; } // имя пользователя
        public static string UserRole { get; set; } // роль  пользователя
    }

    // Преобразования типов и контроль значений
    static class kaa_convert
    {
        // Контроль строки на конвертируемость в положительное число
        public static bool is_number(string _string)
        {
            Regex re = new Regex("^[0-9]+([.][0-9]+)?$");
            return re.IsMatch(_string);
        }

        // Контроль строки на форму числового диапазона
        public static bool is_diap(string _string)
        {
            Regex re = new Regex("^\\[[0-9]+([.][0-9]+)? *- *[0-9]+([.][0-9]+)?\\]$");

            try
            {
                if (!re.IsMatch(_string)) return false;

                //MatchCollection mc = Regex.Matches(_string, " *[0-9]+(.[0-9]+)? *");
                //if (mc.Count != 2)  return false;

                string mc1 = Regex.Match(_string, "[0-9]+([.][0-9]+)? *-").Value;
                string mc2 = Regex.Match(_string, "- *[0-9]+([.][0-9]+)?").Value;
                mc1 = mc1.Substring(0, mc1.Length - 1);
                mc2 = mc2.Substring(1);

                // MessageBox.Show(mc1, "mc1");
                // MessageBox.Show(mc2, "mc2");

                double MinD = UniversalParsingDoubleElseReturn0(mc1);
                double MaxD = UniversalParsingDoubleElseReturn0(mc2);

                if (MinD < MaxD) return true;
                else
                {
                    MessageBox.Show("Число не принадлежит диапазону возможных значений [" + mc1 + "-" + mc2 + "]");
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }


        public static bool ThisIsDouble(string returnSt) //проверка на возможность парсировать строку в дабл
        {
            bool thisIsDouble = true;
            try
            {
                Double.Parse(returnSt);
            }
            catch
            {
                thisIsDouble = false;
            }
            return thisIsDouble;
        }
        /// <summary>
        /// замена . на , ,или наоборот. и пытаеться преобразовать в double
        /// исключение System.FormatException();
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        public static double UniversalParsingDoubleNoNull(string st)
        {
            if (ThisIsDouble(st.Replace(".", ",")))
            {
                return Double.Parse(st.Replace(".", ","));
            }
            else if (ThisIsDouble(st.Replace(",", ".")))
            {
                return Double.Parse(st.Replace(",", "."));
            }
            else
                throw new System.FormatException();
        }
        /// <summary>
        /// замена . на , ,или наоборот. и пытаеться преобразовать в double
        /// исключение System.FormatException();
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        public static double UniversalParsingDoubleElseReturn0(string st)
        {
            try
            {
                return UniversalParsingDoubleNoNull(st);
            }
            catch
            {
                return 0.0;
            }
        }


        // Контроль числа на попадание в диапазон, заданный строкой [MIN-MAX]
        public static bool num_in_diap(double num, string _string)
        {
            Regex re = new Regex("^\\[[0-9]+(.[0-9]+)? *- *[0-9]+(.[0-9]+)?\\]$");

            if (_string == "") return true; // Диапазон не задан

            try
            {
                if (!re.IsMatch(_string)) return false; // Диапазон задан неверно

                string mc1 = Regex.Match(_string, "[0-9]+(.[0-9]+)? *-").Value;
                string mc2 = Regex.Match(_string, "- *[0-9]+(.[0-9]+)?").Value;
                mc1 = mc1.Substring(0, mc1.Length - 1);
                mc2 = mc2.Substring(1);

                // MessageBox.Show(mc1, "mc1");
                // MessageBox.Show(mc2, "mc2");

                double MinD = UniversalParsingDoubleElseReturn0(mc1);
                double MaxD = UniversalParsingDoubleElseReturn0(mc2);

                if (num >= MinD && num <= MaxD) return true;
                else
                {
                    MessageBox.Show("Число не принадлежит диапазону возможных значений [" + mc1 + "-" + mc2 + "]");
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static BitmapImage ConvertBitmapToBitmapImage(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
        // Извлечение параметров из текста процедуры 
        public static string[] ParamsOfProc(string ProcText)
        {

            MatchCollection mc = Regex.Matches(ProcText, "<[^<>]*?>");
            string[] ParsArr = new string[mc.Count];

            int i = 0;
            int len;
            string onePar;
            foreach (System.Text.RegularExpressions.Match m in mc)
            {
                len = m.ToString().Length;
                onePar = m.ToString().Substring(1, len - 2);

                //if (!ParsArr.Contains(onePar))
                //{
                ParsArr[i++] = onePar;
                //}

            }
            return ParsArr.Distinct().ToArray();
        }
    } // kaa_convert

}
