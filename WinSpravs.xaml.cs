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
	/// Логика взаимодействия для WinSpravs.xaml
	/// </summary>
	public partial class WinSpravs : UserControl
	{
		public WinSpravs()
		{
			InitializeComponent();
		}

		private void SpravsWindow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}
		/// <summary>
		/// Клик на вершине дерева справочников - показ параметров объекта справочника
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void treeSpravs_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (treeSpravs.SelectedItem == null) return;

			TreeViewItem TreeItem = new TreeViewItem();
			TreeItem = (TreeViewItem)treeSpravs.SelectedItem;
			if (TreeItem.Header.ToString() == "Справочники")
			{
				dataGridParamSprav.ItemsSource = null;
				lblParsObjSpravs.Content = "";
				return;
			}
			
			TreeViewItem SpravItem = (TreeViewItem)treeSpravs.SelectedItem;
			TreeViewItem SelItemParent = (TreeViewItem)SpravItem.Parent;

			if (SelItemParent.Header.ToString() == "Справочники")
			{
				dataGridParamSprav.ItemsSource = null;
				lblParsObjSpravs.Content = "";
				return;
			}

			int _idSpravObj = (int)TreeItem.Tag;
			lblParsObjSpravs.Content = "Параметры объекта <" + TreeItem.Header.ToString().Trim() + ">";
			//	dataGridParamSprav_SourseCreate(_idSpravObj);
			GridOfParams.Grid_Load_From_ObjSprav(ref dataGridParamSprav, _idSpravObj);
		}
		
		// Добавить справочник в дерево
		private void contextMenuTreeInsert_Click(object sender, RoutedEventArgs e)
		{
			btnSpravsAdd_Click(null, null);
		}
		// Удаление справочника или его объекта 
		private void contextMenuTreeDel_Click(object sender, RoutedEventArgs e)
		{
			btnSpravsItemDel_Click(null, null);
		}
		// Добавить объект справочника в дерево
		private void contextMenuTreeAddSprav_Click(object sender, RoutedEventArgs e)
		{
			btnSpravObjAdd_Click(null, null);
		}
		/// <summary>
		/// Создадим дерево справочников!
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSpravsCreate_Click(object sender, RoutedEventArgs e)
		{
			using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
			{
				var nobody = from x in context.SL_Calcs where x.idParentCalcItem == -1 select x;
				if (nobody.Count() > 0)
				{
					MessageBoxButton buttons = MessageBoxButton.YesNo;
					MessageBoxResult result = MessageBox.Show("Справочники уже введены, очистить?", "Внимание, всё пропадёт!", buttons);
					if (result == MessageBoxResult.No) return;
					// Удалим все справочники и их объекты.  
					foreach (var item in nobody)
					{
						context.SL_Calcs.Remove(item);

					}
				}
				// Добавим корневой элемент всех справочников - в БД
				// idDictItem надо найти в Diction как элемент с именем Разделы! 
				Diction RootItem = context.Diction.Where(o => o.idParentDictItem == -1).FirstOrDefault();
				SL_Calcs SL_Calc2 = new SL_Calcs()
				{ idDictItem = RootItem.idDictItem, nameCalcItem = "Справочники", idParentCalcItem = -1 };
				context.SL_Calcs.Add(SL_Calc2);

				try
				{
					context.SaveChanges();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Ошибка при создании корневой вершины справочников");
					return;
				}
				// Добавим корневые элементы основных справочников - в дерево 
				treeSpravs.Items.Clear();

				
				TreeViewItem Treeitem2 = new TreeViewItem();
				Treeitem2.Header = "Справочники"; Treeitem2.Name = "Справочники"; Treeitem2.Items.Add("*");
				treeSpravs.Items.Add(Treeitem2);
				// Найдём определения всех справочников в Словаре Diction
				var Spravs_def = context.Diction.Where(o => o.idParentDictItem == RootItem.idDictItem);
				// Создадим заголовки справочников 
				// в SL_Calcs
				foreach (Diction oneRazdel in Spravs_def)
				{
					if (context.Diction.Where(o => o.idParentDictItem == oneRazdel.idDictItem).Count() == 0)
					{
						// Добавим  справочники - в БД
						SL_Calcs SL_Calc3 = new SL_Calcs()
						{
							idDictItem = oneRazdel.idDictItem,
							nameCalcItem = oneRazdel.nameDictItem.Trim(),
							idParentCalcItem = SL_Calc2.idCalcItem
						};
						context.SL_Calcs.Add(SL_Calc3);

						TreeViewItem Treeitem3 = new TreeViewItem();
						Treeitem3.Tag = oneRazdel.idDictItem;
						Treeitem3.Header = oneRazdel.nameDictItem.Trim();
						treeSpravs.Items.Add(Treeitem3);
					}
				}
				try
				{
					context.SaveChanges();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Ошибка при создании справочников");
					return;
				}
			}
		}

		private void btnSpravsItemEdit_Click(object sender, RoutedEventArgs e)
		{

		}
		//
		// Удаление Справочника или объекта Справочника
		private void btnSpravsItemDel_Click(object sender, RoutedEventArgs e)
		{
			// Есть отмеченая вершина в дереве справочников?
			if (treeSpravs.SelectedItem == null)
			{
				MessageBox.Show("Отметьте в дереве удаляемый Справочник!", "Внимание!"); return;
			}

			TreeViewItem TreeItem = new TreeViewItem();
			TreeItem = (TreeViewItem)treeSpravs.SelectedItem;
			if (TreeItem.Header.ToString() == "Справочники")
			{
				MessageBox.Show("Удалить корневую вершину <Справочники> нельзя!", "Внимание!"); return;
			}

			int _idSpravItem = (int)TreeItem.Tag;

			MessageBoxButton buttons = MessageBoxButton.YesNo;
			MessageBoxResult result = MessageBox.Show("Действительно хотите удалить объект <" + TreeItem.Header.ToString().Trim() +
				">" + " и всех его потомков?",
				"Внимание, весь объект пропадёт!", buttons, MessageBoxImage.Question);
			if (result == MessageBoxResult.No) return;

			// Удалим все элементы Справочника. Определения их параметров удалятся автоматом? 

			// Удалим запись из БД
			using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
			{
				SL_Calcs Sprav = context.SL_Calcs.Find(_idSpravItem);
				WinShumCalcs.DeleteItemsCalcRecurs(context, Sprav);
				try
				{
					context.SaveChanges();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Проблемы с удалением объектов справочника");
					return;
				}
			}
			// Удалим вершины элемента справочника и всех его потомков
			if (TreeItem.Header.ToString() == "Разделы Словаря") return;
			TreeViewItem parent = (TreeViewItem)TreeItem.Parent;
			parent.Items.Remove(TreeItem);
		}

		// Рекурсивное удаление поддерева справочника  
		//public void DeleteItemsSpravRecurs(ShumCalcs_DBEntities context, SL_Calcs Sprav)
		//{
		//	if (Sprav == null) return;
		//	int _idDictItem = Sprav.idDictItem;
		//	// найдём потомков DictItem и удалим их рекурсивно
		//	var children = from x in context.SL_Calcs where x.idParentCalcItem == _idDictItem select x;
		//	if (children.Count() == 0) return;
		//	foreach (var child in children)
		//	{
		//		DeleteItemsSpravRecurs(context, child);
		//	}
			
		//	context.SL_Calcs.Remove(Sprav);
		//}

		private void btnSpravEdit_Click(object sender, RoutedEventArgs e)
		{

		}
// Ввести значение параметра справочника
		private void btnSpravObjEdit_Click(object sender, RoutedEventArgs e)
		{
			GridOfParams.ValueParInputToGrid(ref dataGridParamSprav);
		}

		private void btnSpravObjDel_Click(object sender, RoutedEventArgs e)
		{


		}

		private int idPorfelDictItem; // ID Определения Производственнной программы в Словаре!

		// Загрузка окна справочников данными из БД
		private void SpravsWindow_Loaded(object sender, RoutedEventArgs e)
		{
			//	lblUserPars.Content = UserData.UserRole + ": " + UserData.UserName + "";

			using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
			{

			}

		}

		// Загрузка поддерева  объекта Словаря  
		public void LoadNodeSubTree(int iParent, TreeViewItem Node)
		{
			// iParent - id родительской вершины дерева; Node -  объект родительской вершины 

			using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
			{
				var children = from x in context.SL_Calcs where x.idParentCalcItem == iParent select x;
				if (children.Count() == 0) return;
				if (children.FirstOrDefault().idDictItem == idPorfelDictItem ) return; // Производственные программы не показываем! 

				foreach (var child in children)
				{
					TreeViewItem TreeItem = new TreeViewItem();
					TreeItem.Tag = child.idCalcItem; 
					if(child.Father.nameCalcItem.Trim() == "Справочники") TreeItem.Header = child.nameCalcItem.Trim();
					else if (child.Father.Father.nameCalcItem.Trim() == "Справочники") 
						TreeItem.Header = child.Father.nameCalcItem.Trim() +":" + child.nameCalcItem.Trim();
					else TreeItem.Header = child.nameCalcItem.Trim();
					
					Node.Items.Add(TreeItem);
					LoadNodeSubTree(child.idCalcItem, TreeItem);
				}
			}
		}
		private void contextMenuTreeEdit_Click(object sender, RoutedEventArgs e)
		{

		}

		private void dataGridParamDef_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}
		// Добавить справочник в дерево
		private void btnSpravsAdd_Click(object sender, RoutedEventArgs e)
		{
			// Вызываем окно ввода имени нового элемента
			WinNewSprav dialog = new WinNewSprav();
			TreeViewItem ParentItem = new TreeViewItem();
			ParentItem = (TreeViewItem)treeSpravs.SelectedItem;
			if (ParentItem == null)
			{
				MessageBox.Show("Отметьте элемент-контейнер нового справочника в дереве!", "Внимание!"); return;
			}

			if (ParentItem.Header.ToString().Trim() != "Справочники")
			{
				MessageBox.Show("Элемент-родитель не есть <Справочники>!", "Внимание!"); return;
			}

			dialog.ShowDialog();
			if (dialog.DialogResult == false) return;

			// Зафиксируем idDictItem выбранного справочника
			int _idDictNewSprav;
			using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
			{
				Diction RootItem = context.Diction.Where(o => o.idParentDictItem == -1).FirstOrDefault();
				var potomki = context.Diction.Where(o => o.idParentDictItem == RootItem.idDictItem);

				_idDictNewSprav = potomki.Where(o =>
				 o.nameDictItem.Trim() == dialog.SpravName.Trim()).FirstOrDefault().idDictItem;
			}

			// Уже есть такой справочник?  
			foreach (TreeViewItem child in ParentItem.Items)
			{
				if (child.Header.ToString().ToUpper().Trim() == dialog.SpravName.ToUpper())
				{
					MessageBox.Show("Уже есть справочник с именем <" + dialog.SpravName + ">", "Внимание!"); return;
				}
			}

			// Запишем в DB новый справочника
			using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
			{
				//	Diction RootItem = context.Diction.Where(o => o.idParentDictItem == -1).FirstOrDefault();
				SL_Calcs newSprav = new SL_Calcs()
				{
					idDictItem = _idDictNewSprav,
					idParentCalcItem = (int)ParentItem.Tag,
					nameCalcItem = dialog.SpravName.Trim()
				};
				context.SL_Calcs.Add(newSprav);
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

				// Добавим в дерево вершину нового справочника
				TreeViewItem TreeItemNew = new TreeViewItem();
				TreeItemNew.Tag = newSprav.idCalcItem;
				TreeItemNew.Header = newSprav.nameCalcItem;
				// TreeItemNew.IsEnabled = true;
				TreeItemNew.IsSelected = true;
				ParentItem.Items.Add(TreeItemNew);
				ParentItem.IsExpanded = true;
			}
		}

		private void dataGridParamDef_SizeChanged(object sender, SizeChangedEventArgs e)
		{

		}
		// Добавить объект в отмеченный справочник!
		private void btnSpravObjAdd_Click(object sender, RoutedEventArgs e)
		{
			// Есть отмеченая вершина в дереве справочников?
			if (treeSpravs.SelectedItem == null)
			{
				MessageBox.Show("Отметьте в дереве пополняемый cправочник!", "Внимание!"); return;
			}
			TreeViewItem SpravItem = (TreeViewItem)treeSpravs.SelectedItem;
			string Sprav_name = SpravItem.Header.ToString().Trim();
			
			if (Sprav_name == "Справочники")
			{
				MessageBox.Show("Отмеченная вершина - корень дерева справочников!", "Внимание!"); return;
			}

			TreeViewItem SelItemParent = (TreeViewItem)SpravItem.Parent;

			if (SelItemParent.Header.ToString() != "Справочники")
			{
				MessageBox.Show("Отмеченная вершина дерева - не есть справочник!", "Внимание!"); return;
			}

			int _idCalcSprav = (int)SpravItem.Tag;
			int _idDictSprav;
			string _nameSprav = SpravItem.Header.ToString().Trim();
			//---------------------------------------------------
			// Определим список параметров справочника
			using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
			{
				SL_Calcs ItemSprav = context.SL_Calcs.Find(_idCalcSprav);
				if (ItemSprav == null) return;
				_idDictSprav = ItemSprav.idDictItem;

				var spravPars = from x in context.DefParams
								where x.idDictItem == _idDictSprav
								orderby x.nameParam
								select x;

				if (spravPars.Count() == 0)
				{
					MessageBox.Show("Справочник не имеет параметров, описанных в Словаре!", "Определите параметры справочника!"); return;
				}
			}
			// Введём имя объекта
			 // Вызываем окно ввода имени нового элемента

			WinNewObjSprav dialog = new WinNewObjSprav();
			dialog.nameSprav = _nameSprav;
			dialog.ShowDialog();
			if (dialog.DialogResult == false) return;
		
			// Уже есть такой объект справочника?  
			foreach (TreeViewItem child in SelItemParent.Items)
			{
				if (child.Header.ToString().ToUpper().Trim() == dialog.nameObjSprav.ToUpper())
				{
					MessageBox.Show("Уже есть объект справочника с именем <" + dialog.nameObjSprav + ">", "Внимание!"); return;
				}
			}

			// Запишем в DB объект нового справочника
			using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
			{
				//	Diction RootItem = context.Diction.Where(o => o.idParentDictItem == -1).FirstOrDefault();
				SL_Calcs newObjSprav = new SL_Calcs()
				{
					idDictItem = _idDictSprav,
					idParentCalcItem = (int)SpravItem.Tag,
					nameCalcItem = dialog.nameObjSprav.Trim()
				};
				context.SL_Calcs.Add(newObjSprav);
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

				// Добавим в дерево вершину нового объекта справочника
				TreeViewItem TreeItemNew = new TreeViewItem();
				TreeItemNew.Tag = newObjSprav.idCalcItem;
				TreeItemNew.Header = Sprav_name + ":" + newObjSprav.nameCalcItem;
				// TreeItemNew.IsEnabled = true;
				TreeItemNew.IsSelected = true;
				SpravItem.Items.Add(TreeItemNew);
				SpravItem.IsExpanded = true;
								
				// Запишем в Грид пустые параметры объекта справочника
				var parsDef = from x in context.DefParams
							  where x.idDictItem == _idDictSprav
							  orderby x.nameParam
							   select new
							   {
								   x.idDefParam,
								   x.idDictItem,
								   x.nameParam,
								   x.codeParam,
								   x.unitParam,
								   ParVal = "                                                         "
							   };

					dataGridParamSprav.ItemsSource = parsDef.ToList();

				    dataGridParamSprav.Columns[0].Header = "IdDefParam";
				    dataGridParamSprav.Columns[1].Header = "IdDictItem";
					dataGridParamSprav.Columns[2].Header = "Имя параметра";
					dataGridParamSprav.Columns[3].Header = "Код";
					dataGridParamSprav.Columns[4].Header = "Ед.Изм.";
					dataGridParamSprav.Columns[5].Header = "Значение параметра";
					dataGridParamSprav.Columns[0].MaxWidth = 0;
					dataGridParamSprav.Columns[1].MaxWidth = 0;

				// Запишем в DB записи параметров объекта справочника
				foreach (var one_par in parsDef)
				{
					SL_CalcPars SL_CalcPar = new SL_CalcPars()
					{
						idCalcItem = newObjSprav.idCalcItem,
						//nameCalc_par = one_par.nameParam,
						idDefParam = one_par.idDefParam,
						valueCalc_par = ""
					};
					context.SL_CalcPars.Add(SL_CalcPar);
				}
				// Сбросим контекст в DB
				try
				{
					context.SaveChanges();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Ошибка записи параметров объекта справочника в  БД");
					return;
				}

				treeSpravs_MouseLeftButtonUp(null, null);
			}
		}
		// Двойной клик на гриде параметров
		private void dataGridParamSprav_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			GridOfParams.ValueParInputToGrid(ref dataGridParamSprav);
		}
	}
}


