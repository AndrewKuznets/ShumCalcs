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
	/// Логика взаимодействия для WinNewDictItem.xaml
	/// </summary>
	public partial class WinNewDictItem : Window
	{
		public WinNewDictItem()
		{
			InitializeComponent();
		}

        // Свойство для имени объекта Словаря
        public string WorkObjectType
        {
            get
            {
                return textBoxName.Text.Trim();
            }
            set
            {
                textBoxName.Text = value;
            }
        }

        // Свойство для имени родителя объекта Словаря
        public string WorkTypeParent
        {
            set
            {
                textBoxPartOf.Text = value;
            }
        }
        // Обработчик клика на Ok

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            string nameItem = textBoxName.Text.Trim();
            if (nameItem == "")
            {
                MessageBox.Show("Имя Словаря не введено", "Внимание!");
                textBoxName.Focus();
                return;
            }
            DialogResult = true;
            Close();
        }
        // Загрузка формы
        private void WinNewDI_Loaded(object sender, RoutedEventArgs e)
        {
            textBoxName.Focus();
        }
    }
}
