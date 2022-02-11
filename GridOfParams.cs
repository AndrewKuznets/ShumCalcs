using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ShumCalcs
{
      class GridOfParams
    {
        public class gdrOneRow // Класс данных одной строки грида!
        {
            public string nameParam { get; set; }
            public string codeParam { get; set; }
            public string sortParam { get; set; } 
            public string unitParam { get; set; }
            public string valuePar { get; set; }
            public string valuesParam { get; set; }
            public int idCalc_par { get; set; }
            public int idCalcItem { get; set; }
        
   //         public int idDefParam { get; set; } 
         }
        /// <summary>
        /// Заполнене грида параметрами Словаря с заданными кодами и c пустыми значениями
        /// </summary>
        /// <param name="grid"></param>
        //public   static void Grid_Load_From_Dict( ref DataGrid grid, string[] codesForGrid)
        //{
        //    using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
        //    {
        //        // Загрузим Грид вкладки 1 определений параметров расчёта №1 блока1 из Словаря - по кодам параметров, участвующих в расчёте                
        //        var pars = from x in context.DefParams
        //                   where codesForGrid.Contains(x.codeParam.Trim())
        //                   select new
        //                   {
        //                       x.idDefParam,
        //                       x.idDictItem,
        //                       x.nameParam,
        //                       x.codeParam,
        //                       x.sortParam,
        //                       x.unitParam,
        //                       valuePar = "", // Поле для значения параметра - пусто!
        //                       x.valuesParam
        //                   };
        //        // Отсортируем параметры по списку codesForGrid
        //        List<object> gridSourse = new List<object>();
        //        foreach (var codeGrid in codesForGrid)
        //        {
        //            foreach (var OnePar in pars)
        //            {
        //                if (codeGrid == OnePar.codeParam.Trim())
        //                {
        //                    gridSourse.Add(new gdrOneRow
        //                    {
        //                        nameParam = OnePar.nameParam.Trim(),
        //                        codeParam = OnePar.codeParam.Trim(), 
        //                        unitParam = OnePar.unitParam.Trim(), 
        //                        valuePar = OnePar.valuePar.Trim(),
        //                        sortParam = OnePar.sortParam.Trim(),
        //                        idDefParam = OnePar.idDefParam
        //                    });
        //                    break;
        //                }
        //            }
        //        }
               
        //        // Grid.ItemsSource = pars.ToList();

        //        grid.AutoGenerateColumns = true;
        //        grid.ItemsSource = gridSourse;
        //        if (grid.Columns.Count == 0) return;

        //        grid.Columns[0].Header = "Наименование параметра";
        //        grid.Columns[1].Header = "Код";
        //        grid.Columns[2].Header = "Ед.Изм.";
        //        grid.Columns[3].Header = "Значение";
        //        grid.Columns[4].Header = "Тип";
        //        grid.Columns[5].Header = "Возможные значения";
        //        grid.Columns[6].Header = "idCalcItem";
        //        grid.Columns[7].Header = "idDefParam";

        //        //grid.Columns[7].MaxWidth = 0;
        //        //grid.Columns[5].MaxWidth = 0;
        //    }
        //}

        /// <summary>
        /// Заполнене грида атрибутами параметров объекта справочника с заданным ID 
        /// </summary>
        /// <param name="grid"></param>
        public static void Grid_Load_From_ObjSprav(ref DataGrid grid, int _idSpravObj)
        {
            // Загрузим Грид определений параметров из БД              
            using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
            {
                //int _idDictSprav = context.SL_Calcs.Find(_idSpravObj).idDictItem;
                var pars = from x in context.SL_CalcPars
                           where x.idCalcItem == _idSpravObj
                           join o in context.DefParams on x.idDefParam equals o.idDefParam
                           orderby o.sortParam descending,o.nameParam ascending
                           select new
                           {
                               x.idCalc_par,
                               x.idCalcItem,
                               o.nameParam,
                               o.codeParam,
                               o.sortParam,
                               o.unitParam,
                               x.valueCalc_par,
                               o.valuesParam,
                               x.idDefParam
                           };

                if (pars.Count() == 0)
                {
                    grid.ItemsSource = null;
                    return;
                    //MessageBox.Show("Внимание!", "Нет параметров у объекта!");
                }


                //       grid.ItemsSource = pars.ToList();
                List<object> gridSourse = new List<object>();
                foreach (var OnePar in pars)
                {
                    gridSourse.Add(new gdrOneRow
                    {
                        nameParam = OnePar.nameParam.Trim(),
                        codeParam = OnePar.codeParam.Trim(),
                        sortParam = OnePar.sortParam.Trim(),
                        unitParam = OnePar.unitParam.Trim(),
                        valuePar = OnePar.valueCalc_par.Trim(),
                        valuesParam = OnePar.valuesParam.Trim(),
                        idCalc_par = OnePar.idCalc_par,
                        idCalcItem = OnePar.idCalcItem
                    });
                }

                grid.AutoGenerateColumns = true;
                grid.ItemsSource = gridSourse;
                if (grid.Columns.Count == 0) return;

                grid.Columns[0].Header = "Наименование параметра";
                grid.Columns[1].Header = "Код";
                grid.Columns[2].Header = "Тип";
                grid.Columns[3].Header = "Ед.Изм.";
                grid.Columns[4].Header = "Значение";
                
                grid.Columns[4].HeaderStyle =  (Style) App.Current.TryFindResource("DataGridHeaderCentered");
                grid.Columns[4].CellStyle = (Style)App.Current.TryFindResource("DataGridCellCentered");

                grid.Columns[5].Header = "Возможные значения";
                grid.Columns[6].Header = "idCalc_par";
                grid.Columns[7].Header = "idCalcItem";

                grid.Columns[1].MaxWidth = 0;
                grid.Columns[2].MaxWidth = 0;
                grid.Columns[5].MaxWidth = 0;
               grid.Columns[6].MaxWidth = 0;
               grid.Columns[7].MaxWidth = 0;
                
            }
        }
        /// <summary>
        /// растянем первый стобец до нужной величины
        /// </summary>
        static public void ElastFirstColumn(DataGrid dataGrid)
        {
            if (dataGrid.Columns.Count > 5)
            {
                dataGrid.Columns[0].Width = dataGrid.ActualWidth - dataGrid.Columns[1].ActualWidth - dataGrid.Columns[3].ActualWidth - dataGrid.Columns[4].ActualWidth ;
            }
        }

        //--------------------------------
        // Ввод значения параметра в отмеченную строку DataGrid
        public static void ValueParInputToGrid(ref DataGrid dataGrid)
        {
                      
            int SelRowInd = dataGrid.SelectedIndex;
            if (SelRowInd == -1)
            {
                MessageBox.Show("Отметьте параметр в таблице !", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            string _nameParam = string.Empty;
            string _codeParam = string.Empty;
            string _sortParam = string.Empty;
            string _unitParam = string.Empty;
            string _valuePar = string.Empty;
            string _valuesParam = string.Empty;
            int _idCalc_par, _idCalcItem;

            // Извлечём из отмеченной строки DataGrid значения атрибутов параметра
            List<string> attribPar = new List<string>();
            //for (int iStolbec = 0; Grid.Columns.Count > iStolbec; iStolbec++)
            //{
            int Number_nameCOL = 0; // номер столбца DataGrid c именем параметра
            DataGridCellInfo cell = new DataGridCellInfo(dataGrid.SelectedItem, dataGrid.Columns[Number_nameCOL]);//iStolbec

            if (cell.Item is gdrOneRow)
            {
                var rowTable = cell.Item as gdrOneRow;
                _nameParam = rowTable.nameParam;
                _codeParam = rowTable.codeParam;
                _sortParam = rowTable.sortParam;
                _idCalc_par = rowTable.idCalc_par;
                _idCalcItem = rowTable.idCalcItem;

                if (_sortParam.Trim() == "Процедура")
                {
                    int iColValues = 5;
                    string _ProcText = WorkWhisTables.GetCellValueFromGrid(dataGrid, iColValues, SelRowInd);
                    MessageBox.Show(_ProcText, "Текст процедуры расчёта параметра");
                    //параметры , которые участвуют в формуле, и их надо выделить цветом
                    List<string> listParams = UtilsCalc.TakeParams(_ProcText);
                    listParamsColorShow = listParams;
                    WorkWhisTables.AddColor(dataGrid, listParams);
                    return;
                }

                if (_sortParam.Trim() == "Расчёт")
                {
                    MessageBox.Show("Значение параметра расчитывается программно!", "Внимание!");
                    return;
                }

                if (_sortParam.Trim() == "Список ID")
                {
                    MessageBox.Show("Значение параметра формируется программно!", "Внимание!");
                    return;
                }


                _unitParam = rowTable.unitParam;
                _valuePar = rowTable.valuePar.Trim();
                _valuesParam = rowTable.valuesParam.Trim();
            }
            else return;   
            
            // Вызываем форму ввода значения параметра 

            WinOneParamVal dialog = new WinOneParamVal();
           
        
            // string [] AllPars = kaa_convert.ParamsOfProc(_valuesParam);

            if (_sortParam == "Строка" && _valuesParam.Length > 0)
            {
   //             _valuesParam = _valuesParam.Replace('\n', ' ').Replace('\r', ' ');
                char[] charSeparators = new char[] { ',','\n','\r' };
                dialog.comboBoxValue.ItemsSource = _valuesParam.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries); // Сделаем список возможных значений параметра из строки возможных значений
                dialog.comboBoxValue.SelectedIndex = 0; // Сделаем список возможных значений параметра из строки возможных значений
                dialog.comboBoxValue.IsEnabled = true;
                dialog.textBoxValue.IsEnabled = false;
            }
            else if (_sortParam == "Ссылка" && _valuesParam.Length > 0)
			{
                // Соберём список объектов Справочника, на который указывает ссылка 
                using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
                {
                    string SpravName = _valuesParam;  
                    var pars = from x in context.SL_Calcs
                               where x.Father.nameCalcItem.Trim()  == SpravName
                               orderby x.idCalcItem
                               select new
                               {
                                   x.nameCalcItem
                               };
                if (pars.Count() > 0)
                    {
                        List<object> gridSourse = new List<object>();
                        foreach (var one_name in pars) gridSourse.Add(one_name.nameCalcItem);
                        dialog.comboBoxValue.ItemsSource = gridSourse;
                    }
                }
                // Построить ItemSource для dialog.comboBoxValu
            }
            else
            {
                dialog.comboBoxValue.IsEnabled = false;
                dialog.textBoxValue.IsEnabled = true;
            }

            dialog.ParName = _nameParam;
            dialog.ParUnit = _unitParam;
            dialog.ParValues = _valuesParam;
            dialog.ParVal = _valuePar;
            dialog.ParSort = _sortParam;

            dialog.ShowDialog();
            if (dialog.DialogResult == false) return;

            // Запишем результат в Грид
            int iRow = dataGrid.SelectedIndex;
            WorkWhisTables.InsertValueInCell(dataGrid, 4, iRow, dialog.ParVal);
            
            // Измененный параметр запишем в БД
            using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
            {
                SL_CalcPars one_par  = context.SL_CalcPars.Find(_idCalc_par);
                if (one_par != null)
                {
                 //   one_par.idCalcItem = _idCalcItem;
                 //   one_par.nameCalc_par = _nameParam;
                    one_par.valueCalc_par = dialog.ParVal;
                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Проблемы с записью значения параметра в БД !" + ex.Message, "Внимание",
                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }

            }

        }

        // Расчёт одной процедуры по её тексту и значениям параметров в гриде и запись в грид!
        public static bool OneCalcInGrid(ref DataGrid Grid, int iRowCurrent, string textProc, out string Result)
        {
            // Все параметры процедуры введены или вычислены?

            string[] AllParsOfProc = kaa_convert.ParamsOfProc(textProc);
            if (!IsAllParsCalculed(ref Grid, AllParsOfProc))
            {
                Result = "";
                return false;
            }

            // Соберём массив кодов параметров процедуры со значениями
            string[,] ParsWithVals = new string[2, AllParsOfProc.Length];
            int iColVal = 4;
            int iColCode = 1;
            string _valuePar, _codePar;
            int indAllParsOfProc = -1;
       //     DataGridCellInfo cell;
          
            foreach (string _ProcCode in AllParsOfProc) // Просмотрим все параметры процедуры
            {
                bool isParFound = false;
                for (int iRow = 0; iRow < Grid.Items.Count; iRow++) // Бежим по гриду в поисках _ProcCode
                {
                    _codePar =  WorkWhisTables.GetCellValueFromGrid(Grid, iColCode, iRow);

                    if (_codePar == _ProcCode) // Нашли!
                    {
                        _valuePar =  WorkWhisTables.GetCellValueFromGrid(Grid, iColVal, iRow);
                        indAllParsOfProc = indAllParsOfProc + 1;
                        ParsWithVals[0, indAllParsOfProc] = _codePar;
                        ParsWithVals[1, indAllParsOfProc] = _valuePar;
                        isParFound = true;
                        break;
                    }
                }
                if (isParFound == false)
                {
                    MessageBox.Show("В таблице параметров не найден параметр с кодом <" + _ProcCode + ">", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    Result = "";
                    return false;
                }
            }

            // Вычислим значение процедуры и запишем её в ячейку Грид
            CollectionCalcElements<CalcElement> collectionCalcElements = new CollectionCalcElements<CalcElement>();
            collectionCalcElements.ParsingSting(textProc);
             Result = collectionCalcElements.CalcResult(ParsWithVals);
          //   MessageBox.Show(Result);
            // В Грид!
            WorkWhisTables.InsertValueInCell(Grid, iColVal, iRowCurrent, Result);
            return true;
        }
   
        /// <summary>
        /// Все ли параметры из списка введены в грид
        /// </summary>
        /// <param name="Grid"></param>
        /// <param name="ParamsProc"></param>
        /// <returns></returns>
        public static bool IsAllParsCalculed(ref DataGrid Grid, string[] ParamsProc)
        {
            // Пробежимся по всем строкам Грида
            int iColVal = 4;
            int iColCode = 1;
            string _valuePar, _codePar;
            string ErrMess = "";
    //        DataGridCellInfo cell;
            for (int iRow = 0; iRow < Grid.Items.Count; iRow++)
            {
                _valuePar = WorkWhisTables.GetCellValueFromGrid(Grid, iColVal, iRow);
                _codePar =  WorkWhisTables.GetCellValueFromGrid(Grid, iColCode, iRow);
                if (ParamsProc.Contains(_codePar) && _valuePar == "")
                    ErrMess = ErrMess + _codePar + ',';
            }
            if (ErrMess != "")
            {
                ErrMess = ErrMess.Substring(0, ErrMess.Length - 1);
                MessageBox.Show("Не введены в таблицу значения параметров с кодами: "
                    + ErrMess, "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            else return true;
        }
        
        //----------------------------------------------
        // Расчёт всех процедур грида
        // Result - значение расчёта последней процедуры 
        public static void CalcAllProcInGrid(ref DataGrid Grid, out string Result)
        {
            // Соберём массив кодов параметров процедуры со значениями
            int iColProc = 5;
            int iColSort = 2;
            string _sortPar, _textProc;
            Result = "";

            for (int iRow = 0; iRow < Grid.Items.Count; iRow++) // Бежим по гриду в поисках _ProcCode
            {
                _sortPar = WorkWhisTables.GetCellValueFromGrid(Grid, iColSort, iRow);

                if (_sortPar == "Процедура") // Нашли!
                {
                    _textProc =  WorkWhisTables.GetCellValueFromGrid(Grid, iColProc, iRow);
                    if (!OneCalcInGrid(ref Grid, iRow, _textProc, out Result)) return;
                }
            }
            MessageBox.Show("Расчёт выполнен");
        }

        private static SolidColorBrush hb = new SolidColorBrush(Colors.Orange);
        private static SolidColorBrush yellowb = new SolidColorBrush(Colors.LightYellow);
        private static SolidColorBrush _LightBlue = new SolidColorBrush(Colors.LightBlue);

        private static SolidColorBrush nb = new SolidColorBrush(Colors.White);
        private static SolidColorBrush ab = new SolidColorBrush(Colors.Gray);

        public static List<string> listParamsColorShow = new List<string>();
       ///Раскраска грида параметров
        public static void DataGrid_LoadingRow(ref DataGrid Grid, ref DataGridRowEventArgs e)
        {
            Grid.Columns[0].Header = "Наименование параметра";

            //Grid.Columns[1].Header = "Код";
            //Grid.Columns[2].Header = "Тип";
            //Grid.Columns[3].Header = "Ед.Изм.";
            //Grid.Columns[4].Header = "Значение";

            //Grid.CanUserReorderColumns = false;
            //Grid.CanUserSortColumns = false;
            //Grid.CanUserAddRows = false;
            //Grid.CanUserDeleteRows = false;
           
            //Grid.RowHeight = 20; // Высота всех строк

            //Grid.Columns[0].Width = 400;
            //Grid.Columns[2].MaxWidth = 0;
            //Grid.Columns[5].MaxWidth = 0;
            
            if (e.Row.DataContext is gdrOneRow)
            {
                string sortParam = (e.Row.DataContext as gdrOneRow).sortParam.Trim();
                string codeParam = (e.Row.DataContext as gdrOneRow).codeParam.Trim();

                if (sortParam == "Расчёт" || sortParam == "Список ID")
                    e.Row.Background = yellowb;
                else
                    e.Row.Background = nb;
                // Отметим главные параметры трудоёмкости и конкурентоспособности 
                if (codeParam == "PTSSTR" || codeParam == "VCSVerf" || codeParam == "Ccgt" || codeParam == "Vcgt")
                    e.Row.Background = _LightBlue;


                // Раскрасим серым строки с пустыми значениями параметров

                foreach (var item in listParamsColorShow)
                {
                    codeParam = (e.Row.DataContext as gdrOneRow).codeParam.Trim();
                    string value  = (e.Row.DataContext as gdrOneRow).valuePar.Trim();
                    if (codeParam == item && value == "")
                        e.Row.Background = ab;
                }
            }

            for (int i = 0; i < 5; i++)
            {
                Grid.Columns[i].IsReadOnly = true;
            }
        }
    }
}
