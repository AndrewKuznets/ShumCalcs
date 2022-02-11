using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShumCalcs

{
    public class UtilsCalc
    {

        /// <summary>
        /// вставить пробелы перед операндами и скобками
        /// </summary>
        /// <param name="stringIn"></param>
        /// <returns></returns>
        public static string PastSpace(string stringIn)
        {
            stringIn = stringIn.Replace("\r", "");
            string[] commands = { "*", "/", "-", "^", "+", "<", "(", ")", "≥","=", "≤", "\n" };

            var arrStringIn = stringIn.Split('\"');
            List<string> listStr = new List<string>();
            for (int i = 0; i < arrStringIn.Count(); i++)
            {
                if (i % 2 == 0)
                {
                    
                    string st = arrStringIn[i];
                    st = st.Replace(">=", "≥");
                    st = st.Replace("<=", "≤");
                    foreach (var comm in commands)
                    {

                        st = st.Replace(comm, " " + comm);
                        st = st.Replace("  " + comm, " " + comm);

                    }
                    listStr.Add(st);
                }
                else
                {
                    listStr.Add('\"' + arrStringIn[i] + '\"');
                }
            }
            stringIn = string.Empty;
            foreach (var strFH in listStr)
            {
                stringIn += strFH;
            }

            return stringIn;
            //string pattern = @"[^0-9](\-[0-9]+(\.[0-9]+)?)";
            //stringIn = Regex.Replace(stringIn, pattern, "($1)");
            // throw new NotImplementedException();
        }

        static void TestCalc()
        {
            List<CalcElement> listParams = new List<CalcElement>();
            listParams.Add(new CalcElement("Толщина обшивки", 20));
            listParams.Add(new CalcElement("Высота обечайки", 200));
            listParams.Add(new CalcElement("Диаметр обечайки", 1000));
            listParams.Add(new CalcElement("Форма детали", "обечайка-цилиндр"));



            CollectionCalcElements<CalcElement> cce = new CollectionCalcElements<CalcElement>();

            cce.ParsingSting( " Если <Толщина обшивки> ВДиапазоне[ 20-30 ] И <Высота обечайки> = 200" +
                " И  <Форма детали>  =  \"обечайка - цилиндр\" То K1 =  Ln(100) + Pow(100,3) Если K1 > 1000  То T = K1 * 10");

            //double trud =  cce.CalcProcedur( listParams );
        }
        // Вычисление значения процедуры по её тексту и значениям параметров! - КАА  
        static public string GetResult(string procText, string[,] arrStryingParams)
        {
               CollectionCalcElements<CalcElement> collectionCalcElements = new CollectionCalcElements<CalcElement>();
               collectionCalcElements.ParsingSting(procText);
               return collectionCalcElements.CalcResult(arrStryingParams);
        }
 
        /// <summary>
        /// получить список параметров процедуры
        /// </summary>
        /// <param name="idBItem"></param>
        /// <returns></returns>
        static public string[] GetCalcParams(int idBItem)
        {
            using (ShumCalcs_DBEntities context = new ShumCalcs_DBEntities())
            {
                Diction hbItem = context.Diction.Where(o => o.idDictItem == idBItem).FirstOrDefault();//
                if (hbItem != null)
                {
                    var a = hbItem.DefParams.Where(o => o.codeParam.Trim() == "prt");
                    string proc = a.FirstOrDefault().valuesParam.Trim();
                    if (proc== string.Empty)
                    {
                        return null;
                    }
                    CollectionCalcElements<CalcElement> collectionCalcElements = new CollectionCalcElements<CalcElement>();
                    collectionCalcElements.ParsingSting(proc);
                    List<string> ls = null;
                    return collectionCalcElements.GetParamsCalc(null,ref ls).ToArray();
                }
                else
                {
                    MessageBox.Show("Процедура расчета не найдена", "Ошибка");
                }
            }
            return null;
        }

        /// <summary>
        /// разбивает строку в список по запятым
        /// </summary>
        /// <param name="valuesParam"></param>
        /// <returns></returns>
        static public List<string> SeparZptToList(string valuesParam)
        {
            string inStr = valuesParam;
            List<string> retList = new List<string>();
            int indexZpt = inStr.IndexOf(",");
            if (inStr.IndexOf("[") < 0)
            {

                while (indexZpt >= 0)
                {
                    retList.Add(inStr.Remove(indexZpt).Trim());
                    inStr = inStr.Remove(0, indexZpt + 1).Trim();
                    indexZpt = inStr.IndexOf(",");
                }
            }

            retList.Add(inStr.Trim());
            return retList;

        }
        /// <summary>
        /// /вставляет строку в строку на нужное место
        /// </summary>
        /// <param name="text"></param>
        /// <param name="v"></param>
        /// <param name="selectSt"></param>
        /// <returns></returns>
        internal static string PastToIndexStr(string text, string v, ref int selectSt)
        {
            string returnSt = text.Insert(selectSt, v);
            selectSt = selectSt + v.Length;
            return returnSt;
        }
        /// <summary>
        /// Список параметров
        /// </summary>
        /// <param name="procText"></param>
        /// <returns></returns>
        internal static List<string> TakeParams(string procText)
        {
            List<string> listPar = new List<string>();
            string st = procText;
                string stPar;

            while (st.IndexOf("<") > 0)
            {
                st = st.Remove(0, st.IndexOf("<") + 1);
                stPar = st.Remove(st.IndexOf(">"));
                bool breakPar = false;
                foreach (var item in     listPar)
                {
                    if (item == stPar)
                    {
                        breakPar = true;
                        break;
                    }
                }
                if (!breakPar)
                {
                    listPar.Add(stPar);
                }
                st = st.Remove(0, st.IndexOf(">") + 1);
            }


            return listPar;
        }


        /// <summary>
        /// параметры ICollection<PanelCut_DB> ParamsDict переводит в список. строку возможных значений разбивает
        /// </summary>
        /// <returns></returns>
        public static List<CalcElement> GetParam(ICollection<DefParams> paramsDict)
        {
            List<CalcElement> returnListParam = new List<CalcElement>();
            //paramsDict.OrderBy(p=>p.nameParam);
            foreach (var paramdict in paramsDict)
            {
              //  if (paramdict.sortParam.Trim() == "Процедура") continue; // Кузнецов 27-2-2018
                List<string> lStrIn = UtilsCalc.SeparZptToList(paramdict.valuesParam);
                returnListParam.Add(new CalcElement(paramdict.nameParam.Trim(), lStrIn));
                
            }
            returnListParam.OrderBy(ce=>ce.Name);
            returnListParam.Sort(delegate (CalcElement ce1, CalcElement ce2)
            { return ce1.Name.CompareTo(ce2.Name); });
            var i = returnListParam.Count;
            return returnListParam;
        }
        /// <summary>
        /// Преобразует строку в доубле, подбирая разделитель(, или .)
        /// или выдает null 
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        public static double? UniversalParsing(string st)
        {
            if (st == null)
                throw new System.FormatException();

            if (ThisIsDouble(st.Replace(".", ",")))
            {
                return Double.Parse(st.Replace(".", ","));
            }
            else if (ThisIsDouble(st.Replace(",", ".")))
            {
                return Double.Parse(st.Replace(",", "."));
            }
            else
                return null;
        }
        private static bool ThisIsDouble(string returnSt) //проверка на возможность парсировать строку в дабл
        {
            bool thisIsDouble = true;
            Double treshResOut;

            thisIsDouble = Double.TryParse(returnSt, out treshResOut);
            
                
            return thisIsDouble;
        }




        /// <summary>
        /// составляет список всех операций
        /// </summary>
        /// <returns></returns>
        public static List<CalcElement> GetOperations()
        {
            List<CalcElement> returnListParam = new List<CalcElement>();


            returnListParam.Add(new CalcElement(CalcElement.TypeLogicEn.IzSpiska));
            returnListParam.Add(new CalcElement(CalcElement.TypeLogicEn.VDiadazone));
            returnListParam.Add(new CalcElement(CalcElement.TypeLogicEn.BolsheRavno));

            returnListParam.Add(new CalcElement(CalcElement.TypeLogicEn.MensheRavno));
            returnListParam.Add(new CalcElement(CalcElement.TypeLogicEn.Menshe));
            returnListParam.Add(new CalcElement(CalcElement.TypeLogicEn.Bolshe));
            returnListParam.Add(new CalcElement(CalcElement.TypeLogicEn.Ravno));
            returnListParam.Add(new CalcElement(CalcElement.TypeLogicEn.Esli));
            returnListParam.Add(new CalcElement(CalcElement.TypeLogicEn.To));
            returnListParam.Add(new CalcElement(CalcElement.TypeLogicEn.Inachi));
            returnListParam.Add(new CalcElement(CalcElement.TypeLogicEn.Ili));
            returnListParam.Add(new CalcElement(CalcElement.TypeLogicEn.I));


            

            returnListParam.Add(new CalcElement(CalcElement.TypeMathEn.Delit));

            returnListParam.Add(new CalcElement(CalcElement.TypeMathEn.Ln));

            returnListParam.Add(new CalcElement(CalcElement.TypeMathEn.Minus));

            returnListParam.Add(new CalcElement(CalcElement.TypeMathEn.Plus));

            returnListParam.Add(new CalcElement(CalcElement.TypeMathEn.Pow));
            returnListParam.Add(new CalcElement(CalcElement.TypeMathEn.Stepen));
            returnListParam.Add(new CalcElement(CalcElement.TypeMathEn.Umnozhit));
            returnListParam.Add(new CalcElement(CalcElement.TypeBracketsEn.Open));

            returnListParam.Add(new CalcElement(CalcElement.TypeBracketsEn.Close));
            return returnListParam;


        }



    }
}
