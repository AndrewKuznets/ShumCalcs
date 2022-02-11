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
    /// Логика взаимодействия для WinOneParamDef.xaml
    /// </summary>
    public partial class WinOneParamDef : Window
    {
        public WinOneParamDef()
        {
            InitializeComponent();
        }
        // // СвойствА для ID элементов Словаря
        private int _idDiction;
        public int idDictItem
        {
            set
            {
                _idDiction = value;
            }
        }

        // Свойство для имени элемента Словаря
        public string nameDictItem
        {
            get
            {
                return lblNameDictionItem.Content.ToString().Trim();
            }
            set
            {
                lblNameDictionItem.Content = "<" + value + ">";
            }
        }


        // Свойство для имени параметра
        public string ParName
        {
            get
            {
                return txtParName.Text;
            }
            set
            {
                txtParName.Text = value;
            }
        }

        // Свойство для кода параметра
        public string ParCode
        {
            get { return txtParCode.Text; }
            set { txtParCode.Text = value; }
        }

        // Свойство для типа параметра
        public string ParType
        {
            get
            {
                return cmbParType.Text;
            }
            set
            {
                cmbParType.Text = value;
            }
        }
        // Свойство для единицы измерения
        public string ParUnit
        {
            get
            {
                return cmbParUnit.Text;
            }
            set
            {
                cmbParUnit.Text = value;
                LblUnits.Content = value;
            }
        }

        // Свойство для возможных значений параметра
        public string ParValues
        {
            get
            {
                return txtParValues.Text;
            }
            set
            {
                txtParValues.Text = value;
            }
        }

        // Сохранение описания параметра элемента Словаря в БД! 
        private void btnSaveParDef_Click(object sender, RoutedEventArgs e)
        {

            // Контроль ввода
            if (txtParName.Text.Trim() == "")
            {
                MessageBox.Show("Имя параметра не введено", "Внимание!");
                txtParName.Focus();
                return;
            }
            if (txtParCode.Text == "")
            {
                MessageBox.Show("Код параметра не введен", "Внимание!");
                txtParCode.Focus();
                return;
            }

            if (cmbParType.Text == "")
            {
                MessageBox.Show("Тип параметра не введен", "Внимание!");
                cmbParType.Focus();
                return;
            }

            if (cmbParType.Text.Trim() == "Число" && txtParValues.Text.Trim() != "")
            {
                string _str = txtParValues.Text.Trim();
                if (!kaa_convert.is_diap(_str) && !kaa_convert.is_number(_str))
                {
                    MessageBox.Show("Для параметров числового типа возможные значения указываются только диапазоном: [Min-Max], где Min < Max, или одним числом", "Внимание");
                    txtParValues.Focus();
                    return;
                }
            }

            if (cmbParType.Text.Trim() == "Ссылка" && cmbDirectRef.Text.Trim() == "")
            {
                MessageBox.Show("Имя справочника не выбрано", "Внимание!");
                cmbDirectRef.Focus();
                return;
            }

            // А нет уже в DB параметра с таким именем?
            //using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
            //{
            //    var pars = from x in context.DefParams
            //               where x.nameParam.ToUpper() == ParName.ToUpper()
            //               select x;
            //    if (pars.Count() > 0)
            //    {
            //        MessageBox.Show("В Словаре уже есть параметр с именем, похожим на <" + ParName.Trim() + ">",
            //            "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
            //        return;
            //    }
            //}

            if (cmbParType.Text.Trim() == "Ссылка" && cmbDirectRef.Text.Trim() != "")
            {
                txtParValues.Text = cmbDirectRef.Text.Trim();
            }

            DialogResult = true;
            Close();
        }
        // Загрузка окна   
        private void txtParValues_Loaded(object sender, RoutedEventArgs e)
        {
            txtParName.Focus();
        }
        // Изменили тип параметра!
        private void cmbParType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbParUnit == null) return;

            ComboBoxItem cmbPT = (ComboBoxItem)cmbParType.SelectedItem;

            if (cmbPT.Content.ToString() == "Число" || cmbPT.Content.ToString() == "Процедура" ||
                cmbPT.Content.ToString() == "Расчёт")
            {
                cmbParUnit.IsEnabled = true;
            }
            else
            {
                cmbParUnit.SelectedIndex = -1;
                cmbParUnit.IsEnabled = false;
            }

            if (cmbPT.Content.ToString() == "Процедура")
            {
                btnProcConstr.Visibility = Visibility.Visible;
                txtParValues.IsEnabled = false; // Заполнение поля только через конструктор!
                cmbDirectRef.Visibility = Visibility.Hidden; // Комбобокс выбора справочника - свободен! 
                lblOnPart.Visibility = Visibility.Hidden;
            }
           
            else if (cmbPT.Content.ToString() == "Ссылка") // Параметр - ссылка на Справочник!
            {
                btnProcConstr.Visibility = Visibility.Hidden;
                cmbDirectRef.Visibility = Visibility.Visible; // Комбобокс выбора справочника - свободен! 
                lblOnPart.Visibility = Visibility.Visible;
                txtParValues.IsEnabled = false;
            }
            else
            {
                btnProcConstr.Visibility = Visibility.Hidden;
                cmbDirectRef.Visibility = Visibility.Hidden; // Комбобокс выбора справочника - свободен! 
                lblOnPart.Visibility = Visibility.Hidden;
                txtParValues.IsEnabled = true;
            }

        }
        // Вызов конструктора процедур
        private void btnProcConstr_Click(object sender, RoutedEventArgs e)
        {

            using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
            {
                Diction SelFather = context.Diction.Where(o => o.idDictItem == _idDiction).FirstOrDefault().Father;
                int idictItemFather = SelFather.idDictItem; // ID отца отмеченного в дереве объекта 
                int idictItemGrandFather = (int) SelFather.idParentDictItem; // ID деда отмеченного в дереве объекта 

              
                // Выберем все параметры из Словаря отмеченного в дереве объекта и его двух предков, которые отличны от данного параметра!

                var pars = context.DefParams.Where(o => o.codeParam.Trim() != txtParCode.Text.Trim() && 
                (o.idDictItem == _idDiction || o.idDictItem == idictItemFather || o.idDictItem == idictItemGrandFather));
                if (pars.Count() == 0)
                {
                    MessageBox.Show("В Cловаре не обнаружено других параметров", "Внимание!");
                    return;
                }
                ICollection<DefParams> Ipars = pars.ToList();
                

                EditCalc dialogEditCalc = new EditCalc();
                try
                {
                    dialogEditCalc.ParamsDict = Ipars;
                }
                catch
                {
                    MessageBox.Show("Ошибка работы с БД. Не найдены параметры", "Ошибка");
                    return;
                }
                dialogEditCalc.textBoxProcedura.Text = txtParValues.Text.ToString();
                dialogEditCalc.Title = "Конструктор процедуры расчёта параметра <" + ParName + ">";

                dialogEditCalc.ShowDialog();
                if (dialogEditCalc.DialogResult == false)
                {
                    return;
                }

                // Запишем текст процедуры в её окно!
                txtParValues.Text = dialogEditCalc.calcProc;
            }

        }
// Загрузка окна 
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
            using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
            {
                // Выберем все корневые разделы Словаря!
               
                Diction RootItem = context.Diction.Where(o => o.idParentDictItem == -1).FirstOrDefault();//

                if (RootItem == null) return;
                 int idRootItem = RootItem.idDictItem;

                var parts = context.Diction.Where(o => o.idParentDictItem == idRootItem );
                if (parts.Count() == 0)
                {
                    MessageBox.Show("В Cловаре не обнаружено корневых разделов", "Внимание!");
                    return;
                }
                // Загрузим имена разделов Словаря в комбобокс имён справочников
                cmbDirectRef.Items.Clear();
                foreach (Diction onePart in parts)
				{
                    if ("<" + onePart.nameDictItem.Trim() + ">" != nameDictItem)
                            cmbDirectRef.Items.Add(onePart.nameDictItem.Trim());
                }
                if (ParType == "Ссылка")
                {
                    cmbDirectRef.Text = ParValues;
                    txtParValues.Text = "";
                }
            }
        }
	}
}
