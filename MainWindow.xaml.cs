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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using WPF.MDI;

namespace ShumCalcs
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			// Контроль разделителя дробной и целой части числа
			if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == ",")
			{
				MessageBox.Show("Установите в настройках Windows разделитель целой и дробной части числа как <.>", "Внимание!",
					MessageBoxButton.OK, MessageBoxImage.Exclamation); Close();
			}
		}
        private string nameLoadBlockOfCalcs = string.Empty;
        bool closeWhithOutQuestion = false;
        // Работа со Словарём БД

        private void DictionWork_Click(object sender, RoutedEventArgs e)
        {
            //menuItemOpen.IsEnabled = false;
            //menuItemSave.IsEnabled = false;

            // Cursor OldCursor = this.Cursor;
            //  this.Cursor = Cursors.Wait;

            bool windowsIsLoad = false;
            foreach (var mdiChild in MainMdiContainer.Children)
            {
                if (mdiChild.Content is WinDiction)
                {
                    mdiChild.Focus();
                    windowsIsLoad = true;
                    break;
                }
            }
            if (!windowsIsLoad)
            {
        //        this.Cursor = Cursors.Wait;
                MainMdiContainer.Children.Add(new MdiChild()
                {
                    Title = "Словарь параметров БД",
                    Height = mainWindowsDocPanelMdi.ActualHeight - 20,
                    Width = mainWindowsDocPanelMdi.ActualWidth,
                    Style = null,
                    //Here compRegistration is the class that you have created for mainWindow.xaml user control.
                    Content = new WinDiction()

                });
                mainWindowsDocPanelMdi_SizeChanged(null, null);
                this.Cursor = Cursors.Arrow;
            }


        }
        // Обработчик загрузки главного окна
        private void MdiWindow_Loaded(object sender, RoutedEventArgs e)
        {

            //txtUserPars.Text = UserData.UserRole + ": " + UserData.UserName + ""; // Пользователя - в статусную строку

            //if (UserData.UserRole == "Пользователь") menuHome.IsEnabled = false;

            try
            {
                using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
                {

                    foreach (var eco in context.Diction)
                    { }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка подключения к базе данных. Приложение будет закрыто");
                closeWhithOutQuestion = true;
                this.Close();
            }
            //      DictionWork_Click(sender, e);

        }
        private void mainWindowsDocPanelMdi_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            foreach (var mdiWin in MainMdiContainer.Children)
            {
                mdiWin.Height = mainWindowsDocPanelMdi.ActualHeight;
                mdiWin.Width = mainWindowsDocPanelMdi.ActualWidth + 20;
                MainMdiContainer.Height = mainWindowsDocPanelMdi.ActualHeight;
                MainMdiContainer.Width = mainWindowsDocPanelMdi.ActualWidth + 20;
                mainWindowsDocPanelMdi.UpdateLayout();
            }

        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {

        }
        // Вызов окна справочников!
        private void SpravsWork_Click(object sender, RoutedEventArgs e)
        {
            nameLoadBlockOfCalcs = "Справочники";
            bool windowsIsLoad = false;
            foreach (var mdiChild in MainMdiContainer.Children)
            {
                if (mdiChild.Content is WinSpravs)
                {
                    mdiChild.Focus();
                    windowsIsLoad = true;
                    break;
                }
            }
            if (!windowsIsLoad)
            {
                MainMdiContainer.Children.Add(new MdiChild()
                {

                    Name = nameLoadBlockOfCalcs,
                    Title = "Справочники",
                    Height = mainWindowsDocPanelMdi.ActualHeight - 20,
                    Width = mainWindowsDocPanelMdi.ActualWidth - 20,
                    Style = null,
                    //Here compRegistration is the class that you have created for mainWindow.xaml user control.
                    Content = new WinSpravs()
                });
                mainWindowsDocPanelMdi_SizeChanged(null, null);
            }
        }

        private void MdiWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (closeWhithOutQuestion)
            {
                return;
            }

            MessageBoxButton buttons = MessageBoxButton.YesNo;
            MessageBoxResult result = MessageBox.Show("Действительно хотите закончить работу?", "",
                buttons, MessageBoxImage.Question);

            e.Cancel = result == MessageBoxResult.No;
        }

        // Загрузка окна расчётов уровней шума 
        private void menuCalcs_Click(object sender, RoutedEventArgs e)
        {
            nameLoadBlockOfCalcs = "Расчёты";
            bool windowsIsLoad = false;
            foreach (var mdiChild in MainMdiContainer.Children)
            {
                if (mdiChild.Content is WinShumCalcs)
                {
                    mdiChild.Focus();
                    windowsIsLoad = true;
                    break;
                }
            }
            if (!windowsIsLoad)
            {
                MainMdiContainer.Children.Add(new MdiChild()
                {
                    Name = nameLoadBlockOfCalcs,
                    Title = "Расчёты уровней шума в помещениях блока",
                    Height = mainWindowsDocPanelMdi.ActualHeight - 20,
                    Width = mainWindowsDocPanelMdi.ActualWidth - 20,
                    Style = null,
                    //Here compRegistration is the class that you have created for mainWindow.xaml user control.
                    Content = new WinShumCalcs()
                });
                mainWindowsDocPanelMdi_SizeChanged(null, null);
            }
        }
   
        // Авторизация нового пользователя
        //private void menuItemAutorise_Click(object sender, RoutedEventArgs e)
        //{
        //    // Авторизация пользователя
        //    winLoginForm dialog = new winLoginForm();
        //    dialog.ShowDialog();
        //    if (dialog.DialogResult == false)
        //    {
        //        MessageBox.Show("Пользователь не сменён!", "Внимание",
        //           MessageBoxButton.OK, MessageBoxImage.Exclamation);
        //    }
        //}
        // Регистрация нового пользователя
        //private void menuItemRegistre_Click(object sender, RoutedEventArgs e)
        //{
        //    // Регистрация нового пользователя
        //    if (UserData.UserRole == "Пользователь")
        //    {
        //        MessageBox.Show("Зарегистрировать нового пользователя может только Администратор!", "Внимание",
        //            MessageBoxButton.OK, MessageBoxImage.Exclamation);
        //        return;
        //    }

        //    WinRegForm dialog = new WinRegForm();
        //    dialog.ShowDialog();
        //    if (dialog.DialogResult == false)
        //    {
        //        MessageBox.Show("Новый пользователь не зарегистрирован!", "Внимание",
        //           MessageBoxButton.OK, MessageBoxImage.Exclamation);
        //    }
        //}

    }
}
