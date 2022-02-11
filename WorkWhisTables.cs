using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace ShumCalcs
{
    internal class WorkWhisTables
    {
        /// <summary>
        /// из DataGrid переписывает codeParam и valuePar в List<XmlFile.DataRecord>
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        internal static List<XmlFile.DataRecord> GetElements(DataGrid grid)
        {

            List<XmlFile.DataRecord> returnList = new List<XmlFile.DataRecord>();


            int iColVal = 0;
            int iColCode = 0;
            int columnIndex = 0;
            foreach (var column in grid.Columns)
            {
                if (column.SortMemberPath == "codeParam")
                {
                    iColCode = columnIndex;
                }
                if (column.SortMemberPath == "valuePar")
                {
                    iColVal = columnIndex;
                }
                columnIndex++;
            }
 // Пробежимся по всем строкам Грида
            string valuePar, codePar;
            for (int iRow = 0; iRow < grid.Items.Count; iRow++)
            {
                valuePar = GetCellValueFromGrid(grid, iColVal, iRow);
                codePar = GetCellValueFromGrid(grid, iColCode, iRow);
                if (grid.Items.Count - iRow == 1 && codePar == "")
                {
                    break;
                }
                returnList.Add(new XmlFile.DataRecordString(codePar, valuePar));
            }
            return returnList;

        }
        /// <summary>
        /// Получение значение из iCol колонки и iRow строки
        /// </summary>
        /// <param name="dataGrid">таблица </param>
        /// <param name="iCol">столбец</param>
        /// <param name="iRow">строка</param>
        /// <returns>значение</returns>
        public static string GetCellValueFromGrid(DataGrid dataGrid, int iCol, int iRow)
        {


            var itSource = dataGrid.ItemsSource;
            int jRow = 0;
            foreach (var item in itSource)
            {

                if (jRow == iRow)
                {
                    return GetValueFromItem(iCol, item);
                }
                jRow++;
            }
            return string.Empty;
        }

        /// <summary>
        /// из item получить значение по индексу
        /// </summary>
        /// <param name="iCol"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetValueFromItem(int iCol, object item)
        {
            if (iCol == 0)
            {
                return (item as GridOfParams.gdrOneRow).nameParam;
            }
            else if (iCol == 1)
            {
                return (item as GridOfParams.gdrOneRow).codeParam;
            }
            else if (iCol == 2)
            {
                return (item as GridOfParams.gdrOneRow).sortParam;
            }
            else if (iCol == 3)
            {
                return (item as GridOfParams.gdrOneRow).unitParam;
            }
            else if (iCol == 4)
            {
                return (item as GridOfParams.gdrOneRow).valuePar;
            }
            else if (iCol == 5)
            {
                return (item as GridOfParams.gdrOneRow).valuesParam;
            }
            else return string.Empty;
        }

        /// <summary>
        /// вставить значения из XML в таблицу
        /// </summary>
        /// <param name="dataGrid"></param>
        /// <param name="list"></param>
        /// <param name="blockNumber"></param>
        /// <param name="calcNumber"></param>
        internal static void InsertToTableFromXML(ref DataGrid dataGrid, List<XmlFile.DataRecord> list, ref int totalStringXML, ref int totalStringTable, ref int insertString)
        {
            int iCol = 0;
            int iColCode = 0;
            int columnIndex = 0;
            foreach (var column in dataGrid.Columns)
            {
                if (column.SortMemberPath == "codeParam")
                {
                    iColCode = columnIndex;
                }
                if (column.SortMemberPath == "valuePar")
                {
                    iCol = columnIndex;
                }
                columnIndex++;
            }
             insertString=0;
             totalStringTable= dataGrid.Items.Count;
             totalStringXML= list.Count;
            for (int iRow = 0; iRow < dataGrid.Items.Count; iRow++)
            {
                string codePar = GetCellValueFromGrid(dataGrid, iColCode, iRow);
                foreach (var dr in list)
                {
                    
                    if (codePar.Trim() == dr.Key.Trim())
                    {
                        string value = (dr is XmlFile.DataRecordString) ? (dr as XmlFile.DataRecordString).Data : (dr as XmlFile.DataRecordValue).Data.ToString();
                        InsertValueInCell(dataGrid, iCol, iRow, value);
                        insertString++;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Вставить значение параметра в таблицу
        /// </summary>
        /// <param name="dataGrid">таблица</param>
        /// <param name="iCol">столбец</param>
        /// <param name="iRow">строка</param>
        /// <param name="value">значение</param>
        public static void InsertValueInCell(DataGrid dataGrid, int iCol, int iRow, string value)
        {

            var itomSource = dataGrid.ItemsSource;
            List<Object> listIS = new List<Object>();
            int jRow = 0;
            foreach (var item in itomSource)
            {

                if (jRow != iRow)
                {
                    listIS.Add(item);
                }
                else
                {

                    if (iCol == 0)
                    {
                        listIS.Add(new GridOfParams.gdrOneRow { nameParam = value, codeParam = (item as GridOfParams.gdrOneRow).codeParam, sortParam = (item as GridOfParams.gdrOneRow).sortParam, unitParam = (item as GridOfParams.gdrOneRow).unitParam, valuePar = (item as GridOfParams.gdrOneRow).valuePar, valuesParam = (item as GridOfParams.gdrOneRow).valuesParam });

                    }
                    else if (iCol == 1)
                    {
                        listIS.Add(new GridOfParams.gdrOneRow { nameParam = (item as GridOfParams.gdrOneRow).nameParam, codeParam = value, sortParam = (item as GridOfParams.gdrOneRow).sortParam, unitParam = (item as GridOfParams.gdrOneRow).unitParam, valuePar = (item as GridOfParams.gdrOneRow).valuePar, valuesParam = (item as GridOfParams.gdrOneRow).valuesParam });
                    }
                    else if (iCol == 2)
                    {
                        listIS.Add(new GridOfParams.gdrOneRow { nameParam = (item as GridOfParams.gdrOneRow).nameParam, codeParam = (item as GridOfParams.gdrOneRow).codeParam, sortParam = value, unitParam = (item as GridOfParams.gdrOneRow).unitParam, valuePar = (item as GridOfParams.gdrOneRow).valuePar, valuesParam = (item as GridOfParams.gdrOneRow).valuesParam });
                    }
                    else if (iCol == 3)
                    {
                        listIS.Add(new GridOfParams.gdrOneRow { nameParam = (item as GridOfParams.gdrOneRow).nameParam, codeParam = (item as GridOfParams.gdrOneRow).codeParam, sortParam = (item as GridOfParams.gdrOneRow).sortParam, unitParam = value, valuePar = (item as GridOfParams.gdrOneRow).valuePar, valuesParam = (item as GridOfParams.gdrOneRow).valuesParam });
                    }
                    else if (iCol == 4)
                    {
                        listIS.Add(new GridOfParams.gdrOneRow { idCalcItem = (item as GridOfParams.gdrOneRow).idCalcItem, nameParam = (item as GridOfParams.gdrOneRow).nameParam, 
                            codeParam = (item as GridOfParams.gdrOneRow).codeParam, sortParam = (item as GridOfParams.gdrOneRow).sortParam, unitParam = (item as GridOfParams.gdrOneRow).unitParam, valuePar = value, valuesParam = (item as GridOfParams.gdrOneRow).valuesParam } );
                    }
                    else if (iCol == 5)
                    {
                        listIS.Add(new GridOfParams.gdrOneRow { nameParam = (item as GridOfParams.gdrOneRow).nameParam, codeParam = (item as GridOfParams.gdrOneRow).codeParam, sortParam = (item as GridOfParams.gdrOneRow).sortParam, unitParam = (item as GridOfParams.gdrOneRow).unitParam, valuePar = (item as GridOfParams.gdrOneRow).valuePar, valuesParam = value });
                    }
                    else listIS.Add(new GridOfParams.gdrOneRow { nameParam = (item as GridOfParams.gdrOneRow).nameParam, codeParam = (item as GridOfParams.gdrOneRow).codeParam, sortParam = (item as GridOfParams.gdrOneRow).sortParam, unitParam = (item as GridOfParams.gdrOneRow).unitParam, valuePar = (item as GridOfParams.gdrOneRow).valuePar, valuesParam = (item as GridOfParams.gdrOneRow).valuesParam });
                }
                jRow++;

            }
            
            dataGrid.ItemsSource = listIS;
           
            dataGrid.Focus();
            dataGrid.SelectedIndex = iRow;
            
            dataGrid.Columns[0].Header = "Наименование параметра";
			dataGrid.Columns[1].Header = "Код";
            dataGrid.Columns[2].Header = "Тип";
            dataGrid.Columns[3].Header = "Ед.Изм.";
            dataGrid.Columns[4].Header = "Значение";
			dataGrid.Columns[5].Header = "Возможные значения";
			dataGrid.Columns[6].Header = "idCalcItem";
			dataGrid.Columns[7].Header = "idDefParam";

            dataGrid.Columns[4].HeaderStyle = (Style)App.Current.TryFindResource("DataGridHeaderCentered");
            dataGrid.Columns[4].CellStyle = (Style)App.Current.TryFindResource("DataGridCellCentered");

            dataGrid.Columns[1].MaxWidth = 0;
            dataGrid.Columns[2].MaxWidth = 0;

            dataGrid.Columns[5].MaxWidth = 0;
			dataGrid.Columns[6].MaxWidth = 0;
			dataGrid.Columns[7].MaxWidth = 0;
		}
        /// <summary>
        /// Перекрасить строки, участвующие в формуле (если код совпадает с одним из значений входного листа)
        /// </summary>
        /// <param name="dataGrid">таблица</param>
        /// <param name="iCol">столбец</param>
        /// <param name="iRow">строка</param>
        /// <param name="value">значение</param>
        public static void AddColor(DataGrid dataGrid, List<string> listParams)
        {
            try
            {
                for (int i = 0; i < (dataGrid.ItemsSource as List<Object>).Count; i++)
                {
                    string par = GetCellValueFromGrid(dataGrid, 1, i);
                    string par2 = GetCellValueFromGrid(dataGrid, 2, i);
                    string par3 = GetCellValueFromGrid(dataGrid, 4, i);
                    if (listParams.IndexOf(par) >= 0 && par3 == "")
                    {
                        DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(i);
                        row.Background = Brushes.Aqua;
                    }
                    else if ("Процедура" == par2)
                    {
                        DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(i);
                        row.Background = Brushes.Orange;
                    }
                    else
                    {
                        DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(i);
                        row.Background = Brushes.White;
                    }
                }
            }
            catch (Exception)
            {}
        }
    }
}