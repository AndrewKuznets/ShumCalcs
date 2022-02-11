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
	/// Логика взаимодействия для WinNewObjSprav.xaml
	/// </summary>
	public partial class WinNewObjSprav : Window
	{
		public WinNewObjSprav()
		{
			InitializeComponent();
		}
        // Свойство для имени объекта Словаря
        public string nameObjSprav
        {
            get
            {
                return txtNameObjSprav.Text.Trim();
            }
            set
            {
                txtNameObjSprav.Text = value;
            }
        }

        // Свойство для имени Словаря
        public string nameSprav
        {
            set
            {
                lblSpravName.Content = value;
            }
        }
        // Обработчик клика на <Принять>
        private void btnSave_Click(object sender, RoutedEventArgs e)
		{

            string nameItem = txtNameObjSprav.Text.Trim();
            if (nameItem == "")
            {
                MessageBox.Show("Имя объекта Словаря не введено", "Внимание!");
                txtNameObjSprav.Focus();
                return;
            }
            DialogResult = true;
            Close();
        }
        // Обработчик клика на <Покинуть>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
            DialogResult = false;
            Close();
        }
	}
}
