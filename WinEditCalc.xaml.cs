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
    /// Логика взаимодействия для WinEditCalc.xaml
    /// </summary>
    public partial class EditCalc : Window
    {

        private List<CalcElement> listCalcElementIn;//список параметров
        private List<CalcElement> listCalcElementOperations; //список операций
        internal string calcProc;
        // список параметров получаемый из базы
        public ICollection<DefParams> ParamsDict { get; internal set; }

        public EditCalc()
        {
            InitializeComponent();
        }

        private void buttonCanc_Click(object sender, RoutedEventArgs e)
        {
            //если текст изменялся
            if (strProceduraFiPrev != textBoxProcedura.Text)
            {
                MessageBoxButton buttons = MessageBoxButton.YesNo;
                MessageBoxResult result = MessageBox.Show(" Изменения, внесённые в текст процедуры, не будут сохранены", "Внимание!", buttons);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }
            DialogResult = false;
            this.Close();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            List<string> listStringParamsName = new List<string>();
            foreach (var papam in ParamsDict)
            {
                listStringParamsName.Add(papam.codeParam.Trim());
            }
            //проверка процедуры 
            CollectionCalcElements<CalcElement> collectionCalcElements = new CollectionCalcElements<CalcElement>();
            textBoxProcedura.Text = UtilsCalc.PastSpace(textBoxProcedura.Text);
            int posErrorStart = -1;
            int posErrorEnd = -1;
            int parsingEror = collectionCalcElements.ParsingSting(textBoxProcedura.Text);
            if (parsingEror > -1)
            {
                posErrorStart = parsingEror;
                posErrorEnd = posErrorStart + 5;


                string nameCE;
                collectionCalcElements.SearchEndPosition(collectionCalcElements, posErrorStart, out posErrorEnd, out nameCE);
            }

            if (posErrorStart == -1)
            {
                collectionCalcElements.CheckCalc(out posErrorStart, out posErrorEnd, listStringParamsName);
            }
            if (posErrorStart != -1)
            {
                textBoxProcedura.Focus();
                textBoxProcedura.SelectionStart = posErrorStart;
                if (posErrorEnd - posErrorStart < 0)
                {
                    posErrorEnd = posErrorStart + 5;
                }
                textBoxProcedura.Select(posErrorStart, posErrorEnd - posErrorStart);
                MessageBox.Show("В процедуре найдена ошибка, процедура не будет сохранена", "Внимание!");

            }
            else
            {
                //запись
                calcProc = UtilsCalc.PastSpace(textBoxProcedura.Text);
                DialogResult = true;
                this.Close();
            }
        }


        private string strProceduraFiPrev; //текст процедуры при загрузке формы. При закрытии окна с этим текстом сравним итоговый текст
        // Загрузка формы, заполнение боксов-слов процедуры
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            listCalcElementIn = UtilsCalc.GetParam(ParamsDict);
            List<string> listValue = new List<string>();
            listValue.Add("Result");
            CalcElement ceResult = new CalcElement("Результат расчёта", listValue);
            listCalcElementIn.Add(ceResult);
            listBoxParams.ItemsSource = listCalcElementIn;
            listCalcElementOperations = UtilsCalc.GetOperations();
            listBoxOperations.ItemsSource = listCalcElementOperations;
            strProceduraFiPrev = textBoxProcedura.Text;
        }

        private void listBoxParams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectStr = listBoxParams.SelectedIndex;
            if (selectStr < 0)
                return;
            listBoxValue.ItemsSource = listCalcElementIn[selectStr].listValue;

        }
        // Поиск кода параметра по его имени  
        private string FindParCode_byParName(string ParName)
        {
            int len = ParName.Length;
            string _substr = ParName.Trim().Substring(1, len - 2);
            foreach (DefParams defPar in ParamsDict)
            {
                if (defPar.nameParam.Trim() == _substr)
                {
                    return "<" + defPar.codeParam.Trim() + ">";
                }
            }
            return "";
        }

        // Поиск имени параметра по его коду  
        private string FindParName_byParCode(string ParCode)
        {
            foreach (DefParams defPar in ParamsDict)
            {
                if (defPar.codeParam.Trim() == ParCode)
                {
                    return defPar.nameParam.Trim();
                }
            }
            return "";
        }

        // Двойный клик на боксе имён параметров
        private void listBoxParams_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int selectSt = textBoxProcedura.SelectionStart;
            string ParCode = FindParCode_byParName(listCalcElementIn[listBoxParams.SelectedIndex].ToString()); // Найдём код отмеченного параметра

            textBoxProcedura.Text = UtilsCalc.PastToIndexStr(textBoxProcedura.Text, " " + ParCode, ref selectSt);
            textBoxProcedura.SelectionStart = selectSt;
        }

        private void listBoxValue_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int selectSt = textBoxProcedura.SelectionStart;
            if (listCalcElementIn[listBoxParams.SelectedIndex].Name == "Результат расчёта")
            {
                textBoxProcedura.Text = UtilsCalc.PastToIndexStr(textBoxProcedura.Text, " " + listCalcElementIn[listBoxParams.SelectedIndex].listValue[listBoxValue.SelectedIndex], ref selectSt);
            }

            else
            {
                textBoxProcedura.Text = UtilsCalc.PastToIndexStr(textBoxProcedura.Text, " \"" + listCalcElementIn[listBoxParams.SelectedIndex].listValue[listBoxValue.SelectedIndex] + "\"", ref selectSt);
            }
            textBoxProcedura.SelectionStart = selectSt;

        }

        private void listBoxOperations_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int selectSt = textBoxProcedura.SelectionStart;
            var select = listCalcElementOperations[listBoxOperations.SelectedIndex];
            if (select.TypeLogic == CalcElement.TypeLogicEn.IzSpiska)
            {


                textBoxProcedura.Text = UtilsCalc.PastToIndexStr(textBoxProcedura.Text, " " + select + "[ , ]", ref selectSt);
                textBoxProcedura.SelectionStart = selectSt - 3;
            }
            else if (select.TypeLogic == CalcElement.TypeLogicEn.VDiadazone)
            {

                textBoxProcedura.Text = UtilsCalc.PastToIndexStr(textBoxProcedura.Text, " " + select + "[ - ]", ref selectSt);
                textBoxProcedura.SelectionStart = selectSt - 3;

            }
            else if (select.TypeMath == CalcElement.TypeMathEn.Ln)
            {

                textBoxProcedura.Text = UtilsCalc.PastToIndexStr(textBoxProcedura.Text, " " + select + "()", ref selectSt);
                textBoxProcedura.SelectionStart = selectSt - 1;

            }
            else if (select.TypeMath == CalcElement.TypeMathEn.Pow)
            {
                textBoxProcedura.Text = UtilsCalc.PastToIndexStr(textBoxProcedura.Text, " " + select + "( , )", ref selectSt);
                textBoxProcedura.SelectionStart = selectSt - 3;
            }
            else
            {
                textBoxProcedura.Text = UtilsCalc.PastToIndexStr(textBoxProcedura.Text, " " + select, ref selectSt);
                textBoxProcedura.SelectionStart = selectSt;
            }
            textBoxProcedura.Focus();
        }
        /// <summary>
        /// Проверка корректности процедуры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Click(object sender, RoutedEventArgs e)
        {
            // Создадим список кодов всех параметров
            List<string> listStringParamsName = new List<string>();
            foreach (var param in ParamsDict)
            {
                listStringParamsName.Add(param.codeParam.Trim());
            }


            CollectionCalcElements<CalcElement> collectionCalcElements = new CollectionCalcElements<CalcElement>();
            textBoxProcedura.Text = UtilsCalc.PastSpace(textBoxProcedura.Text);
            int posErrorStart = -1;
            int posErrorEnd = -1;
            posErrorStart = collectionCalcElements.ParsingSting(textBoxProcedura.Text);
            if (posErrorStart > -1)
            {
                string nameCE;
                collectionCalcElements.SearchEndPosition(collectionCalcElements, posErrorStart, out posErrorEnd, out nameCE);
            }

            if (posErrorStart == -1)
            {
                collectionCalcElements.CheckCalc(out posErrorStart, out posErrorEnd, listStringParamsName);
            }
            if (posErrorStart != -1)
            {
                textBoxProcedura.Focus();
                textBoxProcedura.SelectionStart = posErrorStart;
                if (posErrorEnd - posErrorStart < 1)
                {
                    posErrorEnd = posErrorStart + 5;
                }
                textBoxProcedura.Select(posErrorStart, posErrorEnd - posErrorStart);
            }
            else
            {
                MessageBox.Show("Ошибок в процедуре не обнаружено");
            }

            //string resultParsing = string.Empty;
            //foreach (var ce in collectionCalcElements)
            //{
            //    if (ce.Type== CalcElement.TypeEn.Logic &&( ce.TypeLogic== CalcElement.TypeLogicEn.Esli || ce.TypeLogic == CalcElement.TypeLogicEn.Inachi))
            //    {
            //        resultParsing +=  "\n";
            //    }
            //    resultParsing += ce + " ";
            //}
            //MessageBox.Show(resultParsing,"Проверьте парсинг:");
        }

        private void buttonMakeCalc_Click(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// Изменение отметки в тексте процедуры отрисовка тултипа с именем параметра!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxProcedura_SelectionChanged(object sender, RoutedEventArgs e)
        {
            try
            {


                labelPosic.Content = textBoxProcedura.SelectionStart;
                //string _ParName = FindParName_byParCode(textBoxProcedura.SelectedText);
                //if (_ParName != "")
                //{
                //    textBoxProcedura.ToolTip = _ParName;
                //    //    MessageBox.Show(_ParName);
                //    NameParametr.Content = "Наименование параметра: " + _ParName;
                //}
                string parCode = textBoxProcedura.Text;
                int start = 0;
                if (parCode.Length >= textBoxProcedura.SelectionStart)
                {
                    var a = parCode.Remove(textBoxProcedura.SelectionStart);
                    start = a.LastIndexOf('<');
                }
                else
                {
                    textBoxProcedura.ToolTip = String.Empty;
                    //    MessageBox.Show(_ParName);
                    NameParametr.Content = String.Empty;
                    return;
                }



                int end = parCode.IndexOf('>', textBoxProcedura.SelectionStart);
                bool itIsPar = false;
                if (start >= 0 && end >= 0)
                {
                    parCode = parCode.Remove(end);
                    parCode = parCode.Remove(0, start + 1);
                    string parName = FindParName_byParCode(parCode);
                    if (parName != "")
                    {
                        textBoxProcedura.ToolTip = parName;
                        //    MessageBox.Show(_ParName);
                        NameParametr.Content = "Наименование параметра: " + parName;
                        itIsPar = true;
                    }
                }
                if (!itIsPar)
                {
                    textBoxProcedura.ToolTip = String.Empty;
                    //    MessageBox.Show(_ParName);
                    NameParametr.Content = String.Empty;
                }
            }
            catch (Exception)
            {

                textBoxProcedura.ToolTip = String.Empty;
                NameParametr.Content = String.Empty;
            }
        }

        // Сделать пробный расчёт
        private void btnDoCalc_Click(object sender, RoutedEventArgs e)
        {
            CollectionCalcElements<CalcElement> collectionCalcElements = new CollectionCalcElements<CalcElement>();
            collectionCalcElements.ParsingSting(textBoxProcedura.Text);
            //string[,] stArr = new string[2, 1];
            //stArr[0, 0] = "ShipType";
            //stArr[1, 0] = "Судно для насыпных и навалочных грузов";

            string[,] stArr = new string[2, 3];
            stArr[0, 0] = "KST";
            stArr[1, 0] = "48";
            stArr[0, 1] = "KSS";
            stArr[1, 1] = "0.57";
            stArr[0, 2] = "GT";
            stArr[1, 2] = "2000";
            string res = collectionCalcElements.CalcResult(stArr);
            MessageBox.Show(res);

            //string resultParsing = string.Empty;
            //foreach (var ce in collectionCalcElements)
            //{
            //    resultParsing += ce + " ";
            //}
            //MessageBox.Show(resultParsing, "Проверьте парсинг:");


        }
    }
}
