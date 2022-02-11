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
	/// Логика взаимодействия для WinDiction.xaml
	/// </summary>
	public partial class WinDiction : UserControl
	{
		public WinDiction()
		{
			InitializeComponent();
          //  this.Cursor = Cursors.Wait;
        }

        // Создание дерева Словаря
        private void btnDictCreate_Click(object sender, RoutedEventArgs e)
        {
            using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
            {
                var nobody = from x in context.Diction where x.idParentDictItem == null select x;
                if (nobody.Count() > 0)
                {
                    MessageBoxButton buttons = MessageBoxButton.YesNo;
                    MessageBoxResult result = MessageBox.Show("Словарь не пуст, очистить?", "Внимание, всё пропадёт!", buttons);
                    if (result == MessageBoxResult.No) return;
                    // Удалим все объекты словаря Diction. Определения их параметров удалятся автоматом? 
                    foreach (var item in nobody)
                    {
                        //     context.Diction.Remove(item);
                        DeleteItemsDictionRecurs(context, item);
                    }
                }
                // Добавим корневые элементы основных справочников - в БД
                Diction DictItem2 = new Diction()
                { nameDictItem = "Разделы Словаря" }; //, idParentDictItem = null };
                
                context.Diction.Add(DictItem2);

                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка работы с БД");
                    return;
                }

                DictWindow_Loaded(null,null);
                // Добавим корневой элемент <Разделы Словаря> - в дерево 
                //treeDiction.Items.Clear();

                //TreeViewItem Treeitem2 = new TreeViewItem();
                //Treeitem2.Header = "Разделы Словаря";  
                //treeDiction.Items.Add(Treeitem2);
            }
        }
        // Загрузка окна Словаря данными из БД
        private void DictWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
     //       txtUserPars.Text = UserData.UserRole + ": " + UserData.UserName + "";

            Cursor OldCursor = this.Cursor;
            
         //   this.Cursor = Cursors.Wait;

            using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
            {
                var nobody = from x in context.Diction
                             where x.idParentDictItem == null
                             orderby x.nameDictItem
                             select x;
                try
                {
                    if (nobody.Count() == 0)
                    {
                        MessageBox.Show("Нажмите на кнопку <Создать дерево словаря>!","Словарь пуст!");
                        this.Cursor = OldCursor;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Нет доступа к серверу БД!");
                    return;
                }

                /* Формируем дерево  nobody - корневые вершины */
                treeDiction.Items.Clear();

                foreach (var item in nobody)
                {
                    TreeViewItem Treeitem = new TreeViewItem();
                    Treeitem.Tag = item.idDictItem; Treeitem.Header = item.nameDictItem.Trim();
              //      Treeitem.Name = item.nameDictItem.Trim();

                    treeDiction.Items.Add(Treeitem);
                    LoadNodeSubTree(item.idDictItem, Treeitem); // Загрузим всех потомков корневой вершины
                                                                //            ICollection<DictionConstr_order> potomki = item.Children;
                                                                //            DictionConstr_order predok = item.Father;
                }
           
                // Загрузим Грид параметров  

                int _idRoot = nobody.First().idDictItem; // IdDictitem вершины Разделы словаря          
                var pars = from x in context.DefParams
                           where x.idDictItem == _idRoot
                           select
            new
            {
                x.idDefParam,
                x.idDictItem,
                x.nameParam,
                x.sortParam,
                x.unitParam,
                x.valuesParam
            };

                if (pars.Count() > 0)
                {
                    dataGridParamDef.ItemsSource = pars.ToList();
                    //     dataGridParamDef.Columns[0].Header = "ID";
                    //     dataGridParamDef.Columns[1].Header = "IdDictItem";
                    dataGridParamDef.Columns[2].Header = "Имя";
                    dataGridParamDef.Columns[3].Header = "Код";
                    dataGridParamDef.Columns[4].Header = "Тип";
                    dataGridParamDef.Columns[5].Header = "Ед.Изм.";
                    //   dataGridParamDef.Columns[6].Header = "Значения";

                    //  MessageBox.Show("Формируем грид", "Внимание!");

                    //   dataGridParamDef.Columns[0].Visibility = System.Windows.Visibility.Hidden;
                    //   dataGridParamDef.Columns[1].Visibility = System.Windows.Visibility.Hidden;
                    dataGridParamDef.Columns[0].MaxWidth = 0;
                    dataGridParamDef.Columns[1].MaxWidth = 0;
                    dataGridParamDef.Columns[6].MaxWidth = 0;
                }
            }
            this.Cursor = Cursors.Arrow;
        }

        // Загрузка поддерева  объекта Словаря  
        public void LoadNodeSubTree(int iParent, TreeViewItem Node)
        {
            // iParent - id родительской вершины дерева; Node -  объект родительской вершины 

            using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
            {
                var children = from x in context.Diction where x.idParentDictItem == iParent select x;
                if (children.Count() == 0) return;
                foreach (var child in children)
                {
                    TreeViewItem TreeItem = new TreeViewItem();
                    TreeItem.Tag = child.idDictItem; TreeItem.Header = child.nameDictItem.Trim();
                    Node.Items.Add(TreeItem);
                    LoadNodeSubTree(child.idDictItem, TreeItem);
                }
            }
        }
        // Добавление в дерево элемента Словаря
        private void btnDictAdd_Click(object sender, RoutedEventArgs e)
        {
            // Есть отмеченая вершина в дереве объектов?

            if (treeDiction.SelectedItem == null)
            {
                MessageBox.Show("Отметьте элемент-родитель в дереве!", "Внимание!"); return;
            }

            TreeViewItem ParentItem = new TreeViewItem();
            ParentItem = (TreeViewItem)treeDiction.SelectedItem;
            string SI = ParentItem.Header.ToString().Trim();
          
            // Вызываем окно ввода имени нового элемента
            WinNewDictItem dialog = new WinNewDictItem();

            dialog.textBoxPartOf.Text = SI;
            dialog.ShowDialog();
            if (dialog.DialogResult == false) return;

            // Добавим в БД запись нового элемента Словаря

            foreach (TreeViewItem child in ParentItem.Items)
            {
                if (child.Header.ToString().ToUpper().Trim() == dialog.textBoxName.Text.ToUpper())
                {
                    MessageBox.Show("Уже есть потомок объекта <" + SI + ">" +
                        " с именем <" + dialog.textBoxName.Text + ">", "Внимание!"); return;
                }
            }

            using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
            {
                Diction DictItem = new Diction()
                {
                    nameDictItem = dialog.textBoxName.Text,
                    idParentDictItem = (int)ParentItem.Tag
                };
                context.Diction.Add(DictItem);
                // Сбросим контекст в DB
                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка работы с БД");
                    return;
                }

                // Добавим в дерево вершину нового элемента
                TreeViewItem TreeItemNew = new TreeViewItem();
                TreeItemNew.Tag = DictItem.idDictItem;
                TreeItemNew.Header = dialog.textBoxName.Text;
                // TreeItemNew.IsEnabled = true;
                TreeItemNew.IsSelected = true;
                ParentItem.Items.Add(TreeItemNew);
                ParentItem.IsExpanded = true;
            }
        }
        // Правка имени справочника Словаря
        private void btnDictItemEdit_Click(object sender, RoutedEventArgs e)
        {
            if (treeDiction.SelectedItem == null)
            {
                MessageBox.Show("Отметьте Словарь, подлежащий переименованию!", "Внимание!"); return;
            }
            // Определим ID элемента Словаря   
            TreeViewItem DictItem = new TreeViewItem();
            DictItem = (TreeViewItem)treeDiction.SelectedItem;
            int ID_DictItem = (int)DictItem.Tag;

            if (DictItem.Header.ToString().Trim() == "Разделы Словаря")
            {
                MessageBox.Show("Корневая вершина Словаря не подлежит переименованию!", "Внимание!"); return;
            }

            // Определим родителя отмеченного элемента
            TreeViewItem ParentItem = (TreeViewItem)DictItem.Parent;

            string SI = ParentItem.Header.ToString().Trim();

            // Вызываем окно ввода имени нового элемента
            WinNewDictItem dialog = new WinNewDictItem();

            dialog.WorkObjectType = DictItem.Header.ToString();
            dialog.Title = "Переименование раздела Словаря <" + dialog.WorkObjectType + ">";
            dialog.WorkTypeParent = SI;
            dialog.ShowDialog();
            if (dialog.DialogResult == false) return;

            // А такого нет среди братьев?
            foreach (TreeViewItem child in ParentItem.Items)
            {
                if (child.Header.ToString().ToUpper().Trim() == dialog.textBoxName.Text.ToUpper())
                {
                    MessageBox.Show("Уже есть потомок элемента Словаря <" + SI + ">" +
                        " с именем <" + dialog.textBoxName.Text + ">", "Внимание!",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation); return;
                }
            }
            // Исправим имя элемента в БД
            using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
            {
                Diction ObjDictItem = context.Diction.Find(ID_DictItem);
                if (ObjDictItem == null) return;
                ObjDictItem.nameDictItem = dialog.WorkObjectType;

                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка работы с БД");
                    return;
                }
            }

            // Исправим имя элемента в дереве
            DictItem.Header = dialog.WorkObjectType;
        }
        // Удаление элемента Словаря
        private void btnDictitemDel_Click(object sender, RoutedEventArgs e)
        {
            // Есть отмеченая вершина в дереве объектов?
            if (treeDiction.SelectedItem == null)
            {
                MessageBox.Show("Отметьте удаляемый элемент Словаря а дереве!", "Внимание!"); return;
            }

            TreeViewItem TreeItem = new TreeViewItem();
            TreeItem = (TreeViewItem)treeDiction.SelectedItem;
            if (TreeItem.Header.ToString() == "Разделы Словаря")
            {
                MessageBox.Show("Удалить корневую вершину Словаря нельзя!", "Внимание!"); return;
            }

            int _idDictItem = (int)TreeItem.Tag;

            MessageBoxButton buttons = MessageBoxButton.YesNo;
            MessageBoxResult result = MessageBox.Show("Действительно хотите удалить элемент Словаря <" + TreeItem.Header.ToString().Trim() +
                ">" + " и всех его потомков?",
                "Внимание, всё пропадёт!", buttons, MessageBoxImage.Question);
            if (result == MessageBoxResult.No) return;

            // Удалим все элементы словаря Diction. Определения их параметров удалятся автоматом? 

            // Удалим запись из БД
            using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
            {
                
                Diction DictItem = context.Diction.Find(_idDictItem);
                //context.Diction.Remove(DictItem);
                //context.SaveChanges();
                DeleteItemsDictionRecurs(context, DictItem);
                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Проблемы с удалением раздела Словаря");
                    return;
                }
            }
            // Удалим вершины элемента словаря и всех его потомков
            if (TreeItem.Header.ToString() == "Разделы Словаря") return;
            TreeViewItem parent = (TreeViewItem)TreeItem.Parent;
            parent.Items.Remove(TreeItem);
        }

        // Рекурсивное удаление объектов Словаря  
        public void DeleteItemsDictionRecurs(ShumCalcs_DBEntities context, Diction DictItem)
        {
           // Удалим сначала параметры DictItem!
            var pars = from x in context.DefParams
                       where x.idDictItem == DictItem.idDictItem
                       select x;
            if (pars.Count() > 0)
            {
                foreach (var par in pars)
				{
                    context.DefParams.Remove(par);
				}
            }

            if (DictItem == null) return;
            int _idDictItem = DictItem.idDictItem;

            context.Diction.Remove(DictItem);

            // найдём потомков DictItem и удалим их рекурсивно
            var children = from x in context.Diction where x.idParentDictItem == _idDictItem select x;
            if (children.Count() == 0) return;
            foreach (var child in children)
            {
                DeleteItemsDictionRecurs(context, child);
            }
            // context.SaveChanges();
        }
        // Добавление описания одного параметра
        private void btnDefParAdd_Click(object sender, RoutedEventArgs e)
        {
            if (treeDiction.SelectedItem == null)
            {
                MessageBox.Show("Отметьте элемент Словаря, к которому добавляется параметр!", "Внимание!"); return;
            }
            // Определим ID элемента Словаря   
            TreeViewItem DictItem = new TreeViewItem();
            DictItem = (TreeViewItem)treeDiction.SelectedItem;
            int ID_DictItem = (int)DictItem.Tag;

            // Вызываем окно ввода имени нового элемента
            WinOneParamDef dialog = new WinOneParamDef();

            dialog.nameDictItem = DictItem.Header.ToString();
            dialog.ParType = "Строка";
            dialog.ParUnit = "";
            dialog.ShowDialog();
            if (dialog.DialogResult == false) return;

            // Сформируем определение параметра
            using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
            {
                DefParams DictItemParDef = new DefParams()
                {
                    idDictItem = ID_DictItem,
                    nameParam = dialog.ParName,
                    codeParam = dialog.ParCode,
                    sortParam = dialog.ParType,
                    unitParam = dialog.ParUnit,
                    valuesParam = dialog.ParValues
                };
                // А нет уже в DB параметра с таким именем?
                var pars = from x in context.DefParams
                           where x.nameParam.ToUpper() == dialog.ParName.ToUpper() && x.idDictItem == ID_DictItem
                           select x;
                if (pars.Count() > 0)
                {
                    MessageBox.Show("В этом разделе уже есть параметр с именем, похожим на <" + dialog.ParName.Trim() + ">",
                        "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // А нет уже в DB параметра с таким кодом?
                pars = from x in context.DefParams
                       where x.codeParam.ToUpper() == dialog.ParCode.ToUpper() && x.idDictItem == ID_DictItem
                       select x;
                if (pars.Count() > 0)
                {
                    MessageBox.Show("В этом разделе уже есть параметр с кодом <" + dialog.ParCode.Trim() + ">",
                        "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Добавим определение параметра в DB
                context.DefParams.Add(DictItemParDef);

                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)

                {
                    MessageBox.Show("Проблемы с записью описания параметра в БД !" + ex.Message, "Внимание",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

            }
            // Включим параметр в источник DataGrid
            dataGridParamDef_SourseCreate(ID_DictItem);
        }

        // Создадим коллекцию определений параметров элемента Словаря как источник DataGrid
        // - из БД

        private void dataGridParamDef_SourseCreate(int _idDictItem)
        {
            // Загрузим Грид определений параметров из БД              
            using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
            {
                var pars = from x in context.DefParams
                           where x.idDictItem == _idDictItem
                           orderby x.nameParam
                           select new
                           {
                               x.idDefParam,
                               x.idDictItem,
                               nameParam = x.nameParam.Trim(),
                               x.codeParam,
                               x.sortParam,
                               x.unitParam,
                               x.valuesParam
                           };

                dataGridParamDef.ItemsSource = pars.ToList();

                //   dataGridParamDef.Columns[0].Header = "IdDefParam";
                //   dataGridParamDef.Columns[1].Header = "IdDictItem";
                dataGridParamDef.Columns[2].Header = "Имя параметра";
                dataGridParamDef.Columns[3].Header = "Код";
                dataGridParamDef.Columns[4].Header = "Тип";
                dataGridParamDef.Columns[5].Header = "Ед.Изм.";
                dataGridParamDef.Columns[6].Header = "Значения параметра";

                //dataGridParamDef.Columns[0].Visibility = System.Windows.Visibility.Hidden;
                //dataGridParamDef.Columns[1].Visibility = System.Windows.Visibility.Hidden;
                dataGridParamDef.Columns[0].MaxWidth = 0;
                dataGridParamDef.Columns[1].MaxWidth = 0;
         //       dataGridParamDef.Columns[6].MaxWidth = 0;
            }
        }


        // Показ определений параметров раздела Словаря!
        private void treeDiction_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (treeDiction.SelectedItem == null) return;

            TreeViewItem TreeItem = new TreeViewItem();
            TreeItem = (TreeViewItem)treeDiction.SelectedItem;
            if (TreeItem.Header.ToString().Trim() == "Разделы Словаря") return;

            TreeViewItem TreeItemParent = (TreeViewItem)TreeItem.Parent;
            
            int _idDictItem = (int)TreeItem.Tag;
            lblParsElemDict.Content = "Определения параметров раздела ...<" + TreeItemParent.Header.ToString().Trim() + "><" + TreeItem.Header.ToString().Trim() + ">";
        
            dataGridParamDef_SourseCreate(_idDictItem);
      
        }
        // Удалим описание параметра
        private void btnDelDefPar_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem TreeItem = new TreeViewItem();
            TreeItem = (TreeViewItem)treeDiction.SelectedItem;
            if (TreeItem == null)
            {
                MessageBox.Show("Отметьте в дереве элемент Словаря, параметр которого вы хотите удалить!",
                "Внимание!"); return;
            }

            int _idDictItem = (int)TreeItem.Tag;
            string _nameDictItem = TreeItem.Header.ToString().Trim();


            dataGridParamDef.SelectionMode = DataGridSelectionMode.Single;
            dataGridParamDef.SelectionUnit = DataGridSelectionUnit.FullRow;

            int SI = dataGridParamDef.SelectedIndex;
            if (SI == -1)
            {
                MessageBox.Show("Отметьте в таблице удаляемый параметр элемента Словаря!",
                "Внимание!"); return;
            }

            // Извлечём из DataGrid _nameParam отмеченного определения параметра
            int Number_nameCOL = 2; // номер столбца DataGrid c именем параметра
            var _cell = new DataGridCellInfo(dataGridParamDef.SelectedItem, dataGridParamDef.Columns[Number_nameCOL]);
            var cell_content = _cell.Column.GetCellContent(_cell.Item) as TextBlock;
            string _nameParam = cell_content.Text.Trim();


            // Удаляем определение параметра!
            MessageBoxButton buttons = MessageBoxButton.YesNo;
            MessageBoxResult result = MessageBox.Show("Действительно хотите удалить определение параметра <" + _nameParam +
                "> элемента Словаря <" + _nameDictItem + "> ?", "Внимание!",
                buttons, MessageBoxImage.Question);
            if (result == MessageBoxResult.No) return;

            // Извлечём из DataGrid имя (_nameParam) отмеченного параметра
            Number_nameCOL = 0; // номер столбца DataGrid c idDefParam параметра
            _cell = new DataGridCellInfo(dataGridParamDef.SelectedItem, dataGridParamDef.Columns[Number_nameCOL]);
            cell_content = _cell.Column.GetCellContent(_cell.Item) as TextBlock;
            int _idDefParam = Convert.ToInt32(cell_content.Text);

            // Удалим запись описания параметра из БД
            using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
            {
                DefParams DefOnePar = context.DefParams.Find(_idDefParam);
                if (DefOnePar != null)
                {
                    context.DefParams.Remove(DefOnePar);
                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception ex)

                    {
                        MessageBox.Show("Проблемы с удалением записи из БД !" + ex.Message, "Внимание",
                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }
            }

            dataGridParamDef_SourseCreate(_idDictItem); // Освежим грид параметров
        }
        // Изменить описание параметра
        private void btnEditDefPar_Click(object sender, RoutedEventArgs e)
        {
            if (treeDiction.SelectedItem == null)
            {
                MessageBox.Show("Отметьте элемент Словаря, параметр,  которого надо изменить !",
                    "Внимание!", MessageBoxButton.OK, MessageBoxImage.Exclamation); return;
            }

            // Определим ID элемента Словаря   
            TreeViewItem DictItem = new TreeViewItem();
            DictItem = (TreeViewItem)treeDiction.SelectedItem;
            int ID_DictItem = (int)DictItem.Tag;

            if (dataGridParamDef.SelectedItem == null)
            {
                MessageBox.Show("Отметьте в таблице параметр, определение которого надо изменить !",
                    "Внимание!", MessageBoxButton.OK, MessageBoxImage.Exclamation); return;
            }

            int SI = dataGridParamDef.SelectedIndex;
            // Извлечём из отмеченной строки DataGrid значения аттрибутов параметра
            //         dataGridParamDef.Columns[0].Visibility = System.Windows.Visibility.Visible;
            //         dataGridParamDef.Columns[1].Visibility = System.Windows.Visibility.Visible;
            dataGridParamDef.Columns[0].Width = 0;
            dataGridParamDef.Columns[1].Width = 0;

            //       MessageBox.Show("Just a moment!");

            string[] AttribPar = new string[7];

            for (int iAttr = 0; iAttr < AttribPar.Length; iAttr++)
            {
                int Number_nameCOL = iAttr; // номер столбца DataGrid c именем параметра
                var _cell = new DataGridCellInfo(dataGridParamDef.SelectedItem, dataGridParamDef.Columns[Number_nameCOL]);

                var cell_content = _cell.Column.GetCellContent(_cell.Item) as TextBlock;
                AttribPar[iAttr] = cell_content.Text.Trim();
            }

            //    dataGridParamDef.Columns[0].Visibility = System.Windows.Visibility.Hidden;
            //    dataGridParamDef.Columns[1].Visibility = System.Windows.Visibility.Hidden;


            // Вызываем окно редактирования параметра элемента Словаря
            WinOneParamDef dialog = new WinOneParamDef();

            // Инициализируем значения атрибутов
            dialog.nameDictItem =  DictItem.Header.ToString();
            dialog.ParName = AttribPar[2].Trim();
            dialog.ParCode = AttribPar[3];
            dialog.ParType = AttribPar[4];
            dialog.ParUnit = AttribPar[5];
            dialog.ParValues = AttribPar[6];
            dialog.idDictItem = ID_DictItem;

            dialog.ShowDialog();
            if (dialog.DialogResult == false) return;

            int _idDefParam = Convert.ToInt32(AttribPar[0]);

            Object SelectedRow = dataGridParamDef.SelectedItem;

            //  Запись описания параметра в БД
            using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
            {
                DefParams DefOnePar = context.DefParams.Find(_idDefParam);
                if (DefOnePar != null)
                {
                    DefOnePar.nameParam = dialog.ParName;
                    DefOnePar.codeParam = dialog.ParCode;
                    DefOnePar.sortParam = dialog.ParType;
                    DefOnePar.unitParam = dialog.ParUnit;
                    DefOnePar.valuesParam = dialog.ParValues.Trim();
                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Проблемы с записью определения параметра в БД !" + ex.Message, "Внимание",
                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }

            }
            // Включим параметр в источник DataGrid
            dataGridParamDef_SourseCreate(ID_DictItem);

            dataGridParamDef.Focus();
            dataGridParamDef.SelectedIndex = SI; // Не устанавливается текущая строка
                                                 //  MessageBox.Show(SI.ToString());
        }
        // Обработчики контекстного меню дерева элементов Словаря
        private void contextMenuTreeInsert_Click(object sender, RoutedEventArgs e)
        {
            btnDictAdd_Click(sender, null);
        }

        private void contextMenuTreeEdit_Click(object sender, RoutedEventArgs e)
        {
            btnDictItemEdit_Click(sender, null);
        }

        private void contextMenuTreeDel_Click(object sender, RoutedEventArgs e)
        {
            btnDictitemDel_Click(sender, null);
        }

        private void DictWindow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
        // Двойной клик на гриде - правка определения параметра
        private void dataGridParamDef_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btnEditDefPar_Click(sender, e);
        }

        private void dataGridParamDef_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (dataGridParamDef.Columns.Count > 6)
            {
                double summWidth = -dataGridParamDef.Columns[3].ActualWidth - dataGridParamDef.Columns[4].ActualWidth - dataGridParamDef.Columns[5].ActualWidth - dataGridParamDef.Columns[6].ActualWidth;
                if (summWidth > -199)
                {
                    summWidth = -198;
                }
                dataGridParamDef.Columns[2].Width = dataGridParamDef.ActualWidth + summWidth;
            }
        }
// Дублировать раздел Словаря с заданными параметрами
		private void btnDictitemDouble_Click(object sender, RoutedEventArgs e)
		{
            // Есть отмеченая вершина в дереве объектов?

            if (treeDiction.SelectedItem == null)
            {
                MessageBox.Show("Отметьте раздел-оригинал в дереве!", "Внимание!"); return;
            }

            TreeViewItem OriginItem = new TreeViewItem();
            OriginItem = (TreeViewItem)treeDiction.SelectedItem;
            string OI = OriginItem.Header.ToString().Trim();
            TreeViewItem ParentItem = (TreeViewItem) OriginItem.Parent;

            // Вызываем окно ввода имени нового элемента
            WinNewDictItem dialog = new WinNewDictItem();

            dialog.textBoxPartOf.Text = ParentItem.Header.ToString().Trim();
            dialog.ShowDialog();
            if (dialog.DialogResult == false) return;

            // Скопируем параметры отмеченного раздела
            int idDictItemSel = (int)OriginItem.Tag;
            using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
            {
                var OriginPars = from x in context.DefParams where x.idDictItem == idDictItemSel select x;
                if (OriginPars.Count() == 0)
                {
                    MessageBox.Show(" Параметров у раздела " + OI + " не найдено!", "Внимание!");
                    return;
                }
 
                // Добавим в БД запись нового элемента Словаря
                foreach (TreeViewItem child in ParentItem.Items)
                {
                    if (child.Header.ToString().ToUpper().Trim() == dialog.textBoxName.Text.ToUpper())
                    {
                        MessageBox.Show("Уже есть потомок объекта <" + ParentItem.Header.ToString().Trim() + ">" +
                        " с именем <" + dialog.textBoxName.Text + ">", "Внимание!"); return;
                    }
                }

                Diction NewDictItem = new Diction()
                {
                    nameDictItem = dialog.textBoxName.Text,
                    idParentDictItem = (int)ParentItem.Tag
                };
                context.Diction.Add(NewDictItem);
                // Сбросим контекст в DB
                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка работы с БД");
                    return;
                }

                // Добавим в дерево вершину нового элемента
                TreeViewItem TreeItemNew = new TreeViewItem();
                TreeItemNew.Tag = NewDictItem.idDictItem;
                TreeItemNew.Header = dialog.textBoxName.Text;
                // TreeItemNew.IsEnabled = true;
                TreeItemNew.IsSelected = true;
                ParentItem.Items.Add(TreeItemNew);
                ParentItem.IsExpanded = true;

                // Добавим в БД параметры  нового элемента
                foreach (DefParams onePar in OriginPars)
                {
                    DefParams newPar = new DefParams()
                    {
                        idDictItem = NewDictItem.idDictItem,
                        nameParam = onePar.nameParam,
                        codeParam = onePar.codeParam,
                        sortParam = onePar.sortParam,
                        unitParam = onePar.unitParam,
                        valuesParam = ""
                    };
                    context.DefParams.Add(newPar);
                }
                // Сбросим контекст в DB
                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка работы с БД");
                    return;
                }
                // Отрисуем новый раздел с параметрами
                treeDiction_MouseLeftButtonUp(null, null);
            }
        }
	}
}
