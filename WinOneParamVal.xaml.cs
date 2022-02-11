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
using System.Windows.Shapes;

namespace ShumCalcs
{
    /// <summary>
    /// Логика взаимодействия для WinOneParamVal.xaml
    /// </summary>
    public partial class WinOneParamVal : Window
    {
        public WinOneParamVal()
        {
            InitializeComponent();
        }
        // Свойство для имени параметра
        public string ParName
        {
            set
            {
                txtParName.Text = value;
            }
            get
            {
                return txtParName.Text;
            }
        }
        // Свойство для единицы измерения
        public string ParUnit
        {
            set
            {
               lblUnits.Content = value;
            }
        }
        
        // Свойство для строки ЗНАЧЕНИЙ параметра
        private string _ParValues;
        public string ParValues
        {
            set
            {
                _ParValues = value;
            }
        }
        // Свойство для типа параметра
        private string _ParSort;
        public string ParSort
        {
            set
            {
                _ParSort = value;
            }
        }

        // Свойство для значения параметра
        public string ParVal
        {
            set
            {
                if (_ParSort == "Строка" || _ParSort == "Ссылка") comboBoxValue.Text = value; 
                else textBoxValue.Text = value;
            }
            get
            {
                if (_ParSort == "Строка" || _ParSort == "Ссылка") return  comboBoxValue.Text.Trim();   
                else return  textBoxValue.Text.Trim();
             }
        }
       
// Загрузка формы
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textBoxValue.Focus();
        }
// Сохранение значения параметра
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            // Контроль наличия ввода числа
            if (_ParSort == "Число" && textBoxValue.Text.Trim() == "")
            {
                MessageBox.Show("Значение параметра не введено", "Внимание!");
                textBoxValue.Focus();
                return;
            }
            // Контроль диапазона  
            if (_ParSort == "Число")
            {
                if (!kaa_convert.is_number(textBoxValue.Text.Trim()))
                {
                    MessageBox.Show("Значение параметра не есть положительное число", "Внимание!");
                    comboBoxValue.Focus();
                    return;
                }

                if (!kaa_convert.num_in_diap(UniversalParsingDoubleElseReturn0(textBoxValue.Text.Trim()), _ParValues))
                {
                    textBoxValue.Focus();
                    return;
                }
            }

            // Контроль наличия ввода строки
            if (_ParSort == "Строка" && comboBoxValue.Text.Trim() == "" && _ParValues!="")
            {
                MessageBox.Show("Значение параметра не выбрано", "Внимание!");
                comboBoxValue.Focus();
                return;
            }
            DialogResult = true;
            Close();
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


        // Выход без сохранения
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
// Изменение выбора 
		private void comboBoxValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
            //textBoxValue.Text = comboBoxValue.Text.Trim();
        }
	}
}
