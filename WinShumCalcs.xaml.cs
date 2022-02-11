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
	/// Логика взаимодействия для WinShumCalcs.xaml
	/// </summary>

	public partial class WinShumCalcs : UserControl
	{
		public WinShumCalcs()
		{
			InitializeComponent();
		}
		// id 
		private int idZakazDictItem, idProjectDictItem;

///  <summary>
/// Создать дерево расчётов
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
		private void btnTreeTrudCalcsCreate_Click(object sender, RoutedEventArgs e)
		{
			using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
			{
				var nobody = from x in context.SL_Calcs where x.nameCalcItem.Trim() == "Расчёты" select x;
				if (nobody.Count() > 0)
				{
					MessageBoxButton buttons = MessageBoxButton.YesNo;
					MessageBoxResult result = MessageBox.Show("Раздел <Расчёты> уже есть в базе данных, удалить его?",
						"Внимание, все расчёты в БД пропадут!", buttons);
					if (result == MessageBoxResult.No) return;
					// Удалим весь раздел <Расчёты>.  
					DeleteItemsSL_CalcsRecurs ( context, nobody.FirstOrDefault());
				}

				// Добавим корневой элемент всех расчётов - в БД
				SL_Calcs SL_CalcRoot = new SL_Calcs()
				{ idDictItem = 2, nameCalcItem = "Расчёты", idParentCalcItem = -1 };
				context.SL_Calcs.Add(SL_CalcRoot);

				try
				{
					context.SaveChanges();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Ошибка при создании раздела <Расчёты>");
					return;
				}
				// Добавим типы судов - в дерево 
				treeTrudCalcs.Items.Clear();

				TreeViewItem TreeitemRoot = new TreeViewItem();
				TreeitemRoot.Tag = 2; TreeitemRoot.Header = "Расчёты"; TreeitemRoot.Name = "Расчёты";
				//TreeitemRoot.Items.Add("*");
				treeTrudCalcs.Items.Add(TreeitemRoot);
				// Найдём  все типы судов  в Словаре Diction
				Diction RootItem = context.Diction.Where(o => o.nameDictItem.Trim() == "Трудоёмкость").FirstOrDefault();
				var ShipTypes = context.Diction.Where(o => o.idParentDictItem == RootItem.idDictItem);
				// Создадим записи типов судов 
				// в SL_Calcs
				foreach (Diction oneST in ShipTypes)
				{
					// Добавим  типы судов - в SL_CAlCS из Diction 
					SL_Calcs SL_Calc3 = new SL_Calcs()
					{
						idDictItem = oneST.idDictItem,
						nameCalcItem = oneST.nameDictItem.Trim(),
						idParentCalcItem = SL_CalcRoot.idCalcItem
					};
					context.SL_Calcs.Add(SL_Calc3);

					TreeViewItem TreeitemShipType = new TreeViewItem();
					TreeitemShipType.Tag = oneST.idDictItem;
					TreeitemShipType.Header = oneST.nameDictItem.Trim();
					TreeitemRoot.Items.Add(TreeitemShipType);
				}

				try
				{
					context.SaveChanges();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Ошибка при создании раздела <Расчёты>");
					return;
				}

				// Прочитаем заголовки типов судов в SL_Calcs 
				var ShipTypes2 = context.SL_Calcs.Where(o => o.idParentCalcItem == SL_CalcRoot.idCalcItem);
				foreach (SL_Calcs oneST in ShipTypes2)
				{
					// Определим подтипы данного типа судна в Diction
					var ShipSubTypes = from x in context.Diction where x.idParentDictItem == oneST.idDictItem select x;
					if (ShipSubTypes.Count() > 0)
					{
						foreach (var oneSubST in ShipSubTypes)
						{
							// Добавим  подтипы судов - в SL_CAlCS из Diction 
							SL_Calcs SL_Calc4 = new SL_Calcs()
							{
								idDictItem = oneSubST.idDictItem,
								nameCalcItem = oneSubST.nameDictItem.Trim(),
								idParentCalcItem = oneST.idCalcItem
							};
							context.SL_Calcs.Add(SL_Calc4);

							// Добавим  подтипы судов - в дерево 
							TreeViewItem TreeitemSubType = new TreeViewItem();
							TreeitemSubType.Tag = oneSubST.idDictItem;
							TreeitemSubType.Header = oneSubST.nameDictItem.Trim();
							TreeViewItem TreeitemShipType = TreeViewItemFind_by_Header(TreeitemRoot.Items, oneST.nameCalcItem);
							TreeitemShipType.Items.Add(TreeitemSubType);
						
					    }
				    }
				}
						 try { context.SaveChanges(); }
						 catch (Exception ex)
						  { MessageBox.Show(ex.Message, "Ошибка при создании раздела <Расчёты>");
							return;}
			}
		}

		// Рекурсивное удаление объектов-расчётов  
		public void DeleteItemsSL_CalcsRecurs(ShumCalcs_DBEntities context, SL_Calcs One_calc)
		{
			if (One_calc == null) return;
			int _idCalcItem = One_calc.idCalcItem;
			context.SL_Calcs.Remove(One_calc);

			// найдём потомков SL_Calcs и удалим их рекурсивно
			var children = from x in context.SL_Calcs where x.idParentCalcItem == _idCalcItem select x;
			if (children.Count() == 0) return;
			foreach (var child in children)
			{
				DeleteItemsSL_CalcsRecurs(context, child);
			}
		}
		// Рекурсивный поиск вершины дерева с именем 
		private TreeViewItem TreeViewItemFind_by_Header(ItemCollection Items, string _HeaderSearch)
		{
			if (Items.Count == 0) return null;
			foreach (TreeViewItem objTreeViewItem in Items)
			{
				string _Header = objTreeViewItem.Header.ToString().Trim();
				if (_Header == _HeaderSearch)
				{
					objTreeViewItem.IsSelected = true;
					objTreeViewItem.IsExpanded = true;
					return objTreeViewItem;
				}
			}
			// Не нашли на первом уровне 
			foreach (TreeViewItem objTreeViewItem in Items)
			{
				ItemCollection ChildrenItems = objTreeViewItem.Items;
				TreeViewItem objTreeViewItemFind = TreeViewItemFind_by_Header(ChildrenItems, _HeaderSearch);
				if (objTreeViewItemFind != null) return objTreeViewItemFind;
			}
			return null;
		}
		// Добавить тип судна
	
// Загрузка окна Расчётов
		private void CalcsWindow_Loaded(object sender, RoutedEventArgs e)
		{
	//		lblUserPars.Content = UserData.UserRole + ": " + UserData.UserName + ""; // Запись в статусную строку!

			using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
			{
				
			}
		}

		// Загрузка поддерева  раздела <Расчёты>  
		public void LoadNodeSubTree(int iParent, TreeViewItem Node)
		{
			// iParent - id родительской вершины дерева; Node -  объект родительской вершины 

			using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
			{
				var children = from x in context.SL_Calcs where x.idParentCalcItem == iParent select x;
				if (children.Count() == 0) return;
				foreach (var child in children)
				{
					TreeViewItem TreeItem = new TreeViewItem();
					TreeItem.Tag = child.idCalcItem;
					if (child.Father.nameCalcItem.Trim() == "Расчёты") TreeItem.Header = child.nameCalcItem.Trim();
					else if (child.Father.Father.nameCalcItem.Trim() == "Расчёты")
						TreeItem.Header =   child.nameCalcItem.Trim();
					else  if (child.Father.Father.Father.nameCalcItem.Trim() == "Расчёты") 
						TreeItem.Header = "Проект: " + child.nameCalcItem.Trim();
					else if (child.Father.Father.Father.Father.nameCalcItem.Trim() == "Расчёты")
						TreeItem.Header = "Заказ: " + child.nameCalcItem.Trim();

					Node.Items.Add(TreeItem);
					LoadNodeSubTree(child.idCalcItem, TreeItem);
				}
			}
		}

// Удалить объект расчёта трудоёмкости  и его потомков
		private void btnTreeItemDel_Click(object sender, RoutedEventArgs e)
		{
			// Есть отмеченая вершина в дереве расчётов?
			if (treeTrudCalcs.SelectedItem == null)
			{
				MessageBox.Show("Отметьте в дереве удаляемый объект расчёта!", "Внимание!"); return;
			}

			TreeViewItem TreeItem = new TreeViewItem();
			TreeItem = (TreeViewItem)treeTrudCalcs.SelectedItem;
			if (TreeItem.Header.ToString() == "Расчёты")
			{
				MessageBox.Show("Удалить корневую вершину <Расчёты> нельзя!", "Внимание!"); return;
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
				DeleteItemsCalcRecurs(context, Sprav);
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
			// Удалим вершину объекта расчёта и всех его потомков
			if (TreeItem.Header.ToString() == "Расчёты") return;
			TreeViewItem parent = (TreeViewItem)TreeItem.Parent;
			parent.Items.Remove(TreeItem);
		}

		// Рекурсивное удаление поддерева объекта трудоёмкости  
		static public void DeleteItemsCalcRecurs(ShumCalcs_DBEntities context, SL_Calcs TrudObj)
		{
			// Удалим сначала параметры TrudObj!
			var pars = from x in context.SL_CalcPars
					   where x.idCalcItem == TrudObj.idCalcItem
					   select x;
			if (pars.Count() > 0)
			{
				foreach (var par in pars)
				{
					context.SL_CalcPars.Remove(par);
				}
			}

			if (TrudObj == null) return;
			int _idCalcItem = TrudObj.idCalcItem;
			// Теперь удалим объект трудоёмкости!
			context.SL_Calcs.Remove(TrudObj);

			// найдём потомков TrudObj и удалим их рекурсивно
			var children = from x in context.SL_Calcs where x.idParentCalcItem == _idCalcItem select x;
			if (children.Count() == 0) return;
			foreach (var child in children)
			{
				DeleteItemsCalcRecurs(context, child);
			}
		}
		// Клик на вершине дерева расчётов - показ параметров объекта расчёта!
		private void treeTrudCalcs_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			TreeViewItem TreeItem = new TreeViewItem();
			TreeItem = (TreeViewItem) treeTrudCalcs.SelectedItem;
			if (TreeItem == null) return;

			if (TreeItem.Header.ToString() == "Расчёты")
			{
				dataGridParsTrudCalc.ItemsSource = null;
				lblParsObj.Content = "";
				return;
			}

			TreeViewItem SpravItem = (TreeViewItem)treeTrudCalcs.SelectedItem;
			TreeViewItem SelItemParent = (TreeViewItem)SpravItem.Parent;

			if (SelItemParent.Header.ToString() == "Расчёты")
			{
				dataGridParsTrudCalc.ItemsSource = null;
				lblParsObj.Content = "";
				return;
			}

			int _idSpravObj = (int)TreeItem.Tag;
			lblParsObj.Content = "Параметры объекта <" + TreeItem.Header.ToString().Trim() + ">";
			
			GridOfParams.Grid_Load_From_ObjSprav(ref dataGridParsTrudCalc, _idSpravObj);
		}
		// Добавить заказ в отмеченный ПРОЕКТ
		
// Изменить значение параметра
		private void btnCalcsObjEdit_Click(object sender, RoutedEventArgs e)
		{
			GridOfParams.ValueParInputToGrid(ref dataGridParsTrudCalc);
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			GridOfParams.ValueParInputToGrid(ref dataGridParsTrudCalc);
		}

		private void dataGridParsTrudCalc_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			GridOfParams.ValueParInputToGrid(ref dataGridParsTrudCalc);
		}

	

	
		private void dataGridParsTrudCalc_LoadingRow(object sender, DataGridRowEventArgs e)
		{
			GridOfParams.DataGrid_LoadingRow(ref dataGridParsTrudCalc, ref e);
		}

		
	}
}
