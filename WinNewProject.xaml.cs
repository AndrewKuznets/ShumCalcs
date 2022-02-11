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
	/// Логика взаимодействия для WinNewProject.xaml
	/// </summary>
	public partial class WinNewProject : Window
	{
		public WinNewProject()
		{
			InitializeComponent();
		}
		// Свойство для номера нового Проекта
		public string ProjNumber
		{
			get
			{
				return txtProjName.Text.Trim();
			}
			set
			{
				txtProjName.Text = value;
			}
		}
// Кнопка Принять!  
    	private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			if (txtProjName.Text.Trim() == "")
			{
				MessageBox.Show("Номер проекта не введён", "Внимание!");
				txtProjName.Focus();
				return;
			}	

			DialogResult = true;
			Close();
		}
// Кнопка Cancel!  
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}
