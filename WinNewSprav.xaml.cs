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
	/// Логика взаимодействия для WinNewSprav.xaml
	/// </summary>
	public partial class WinNewSprav : Window
	{
		public WinNewSprav()
		{
			InitializeComponent();
		}

		// Свойство для имени нового Справочника
		public string SpravName
		{
			get
			{
				return cmbNewSprav.Text.Trim();
			}
			set
			{
				cmbNewSprav.Text = value;
			}
		}
		
		// Загрузка окна!
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{

			using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
			{
				// Найдём имена корневых справочников
				Diction RootItem = context.Diction.Where(o => o.idParentDictItem == -1).FirstOrDefault();

				var potomki = context.Diction.Where(o => o.idParentDictItem == RootItem.idDictItem);
				cmbNewSprav.Items.Clear();

				foreach (Diction oneRazdel in potomki)
				{
					cmbNewSprav.Items.Add(oneRazdel.nameDictItem);
				}
			}
		}
// Выбрали имя Справочника
		private void cmbNewSprav_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			
		}
// Ок
		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}
// Cancel
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}
