using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace ShumCalcs
{
    public class CollectionCalcElements<T> : List<CalcElement>
    {
        /// <summary>
        /// вывод элементов , перед если и иначи - энтер
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {

            string resultParsing = string.Empty;
            foreach (var ce in this)
            {
                if (ce.Type == CalcElement.TypeEn.Logic && (ce.TypeLogic == CalcElement.TypeLogicEn.Esli || ce.TypeLogic == CalcElement.TypeLogicEn.Inachi))
                {
                    resultParsing += "\n";
                }
                resultParsing += ce + " ";
            }
            return resultParsing;
        }
        /// <summary>
        /// вывод элементов по порядку с заданным разделителем
        /// </summary>
        /// <param name="sep"></param>
        /// <returns></returns>
        public string ToString(string sep)
        {

            string resultParsing = string.Empty;
            foreach (var ce in this)
            {

                resultParsing += ce + sep;
            }
            return resultParsing;
        }


        public List<string> GetParamsCalc(List<CalcElement> lce, ref List<string> listParams)
        {
            List<CalcElement> cceIn;
            if (lce != null)
            {
                cceIn = lce;
            }
            else
            {
                cceIn = this;
            }
            if (listParams == null)
            {
                listParams = new List<string>();
            }

            for (int i = 0; i < cceIn.Count; i++)
            {
                CalcElement ce = cceIn[i];
                //ищем cписок параметров. помечаем , текстовый он или нет
                if (ce.Type == CalcElement.TypeEn.Param)
                {
                    if (listParams.Where(o => o == ce.Name).Count() == 0)
                    {

                        listParams.Add(ce.Name);
                    }
                }
                if (ce.listCalcElements.Count > 0)
                {
                    GetParamsCalc(ce.listCalcElements, ref listParams);
                }
            }
            return listParams;
        }

        /// <summary>
        /// проверка процедуры
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        public void CheckCalc(out int startPosition, out int endPosition, List<string> listStringParamsName)
        {
            CollectionCalcElements<CalcElement> ceParams = new CollectionCalcElements<CalcElement>();
            CollectionCalcElements<CalcElement> ceUncknows = new CollectionCalcElements<CalcElement>();
            CollectionCalcElements<CalcElement> cceInRaw = new CollectionCalcElements<CalcElement>(); //исходная колекция элементов
            string nameCE;
            cceInRaw.AddRange(this.ToArray());
            for (int i = 0; i < this.Count; i++)
            {
                CalcElement ce = this[i];
                CalcElement ceNext;
                CalcElement ceNextNext;
                if (this.Count > i + 1)
                {
                    ceNext = this[i + 1];
                }
                else
                {
                    ceNext = null;
                }
                if (this.Count > i + 2)
                {
                    ceNextNext = this[i + 2];
                }
                else
                {
                    ceNextNext = null;
                }

                //ищем cписок параметров. помечаем , текстовый он или нет
                if (ce.Type == CalcElement.TypeEn.Param)
                {
                    if (ceParams.Where(o => o.Name == ce.Name).Count() == 0)//параметр уникален
                    {
                        if (listStringParamsName.Where(o => o == ce.Name).Count() > 0)
                        {
                            ceParams.Add(ce);
                        }
                        else
                        {
                            MessageBox.Show("Неизвестный параметр: " + ce.Name, "Ошибка процедуры");
                            startPosition = ce.position;
                            SearchEndPosition(cceInRaw, startPosition, out endPosition, out nameCE);
                            return;
                        }
                    }
                }

                else if (ce.Type == CalcElement.TypeEn.UnknownQuantity)
                {
                    if (ce.listCalcElements.Count == 0 && ceUncknows.Where(o => o.Name == ce.Name).Count() == 0)
                    {
                        ceUncknows.Add(ce);
                    }
                }
            }
           //   MessageBox.Show("Список параметро функции: \n"+ ceParams.ToString("\n"));//список параметров
            //foreach (var ce in ceUncknows)//если неизвестный параметр не коэффициент и не трудоемкость
            //{
            //    int iTrash;
            //    //если начальная буква K и остальное парсится в число , или равно T , то ничего не делаем
            //    if (!((ce.Name.IndexOf("K") == 0 && int.TryParse(ce.Name.Remove(0, 1), out iTrash)) || ce.Name == "T"))
            //    {
            //        startPosition = ce.position;
            //        SearchEndPosition(cceInRaw, startPosition, out endPosition, out nameCE);
            //        MessageBox.Show("Не распознана часть процедуры: " + nameCE, "Ошибка процедуры");

            //        return;
            //    }
            //}

            //проверка если...то

            bool esli = false;
            foreach (var ce in this)
            {
                if (ce.Type == CalcElement.TypeEn.Logic && ce.TypeLogic == CalcElement.TypeLogicEn.Esli)
                {
                    if (esli)
                    {
                        startPosition = ce.position;
                        SearchEndPosition(cceInRaw, startPosition, out endPosition, out nameCE);
                        MessageBox.Show("Ошибка в операторе \"То\", нет соответствующего ему \"Если\"", "Ошибка процедуры");
                        return;
                    }
                    esli = true;
                }
                if (ce.Type == CalcElement.TypeEn.Logic && ce.TypeLogic == CalcElement.TypeLogicEn.To)
                {
                    if (!esli)
                    {
                        startPosition = ce.position;
                      
                        SearchEndPosition(cceInRaw, startPosition, out endPosition, out nameCE);
                        MessageBox.Show("Ошибка в операторе \"Если\" , нет соответствующего ему \"То\"", "Ошибка процедуры");
                        return;
                    }
                    esli = false;
                }
            }


            bool IzmeneniyaBili = false;
            int rang = 0;


            int countCalc = 1;
            int stop = 1000; // Что такое <stop>, Коля? 
            CollectionCalcElements<CalcElement> calcZnach = new CollectionCalcElements<CalcElement>();
            while (countCalc > 0 && stop > 0)
            {
                stop--;
                countCalc = 0;
                //массив строк в список параметров

                int posichenError = 0;
                try
                {
                    int resultCheckProcRecurs = CheckProcRecurs(this, null, calcZnach, rang, ref posichenError, listStringParamsName);
                    if (resultCheckProcRecurs == -1)
                    {
                        startPosition = posichenError;
                      
                        SearchEndPosition(cceInRaw, startPosition, out endPosition, out nameCE);
                        return;
                    }
                    countCalc = resultCheckProcRecurs;
                }
                catch
                {
                    startPosition = posichenError;
                   
                    SearchEndPosition(cceInRaw, startPosition, out endPosition, out nameCE);
                    return;
                }
                if (countCalc > 0) //что то изменилось
                {
                    IzmeneniyaBili = true;
                    rang = 0;
                }
                if (IzmeneniyaBili && countCalc == 0)  // при этом ранге нет изменений но в этот перебор рангов изменения были
                {
                    IzmeneniyaBili = false;
                    rang = 0;
                    countCalc++;
                }
                else if (!IzmeneniyaBili)//в этот проход рангов измений не было 
                {
                    rang++;
                    countCalc++;
                }
                if (rang == 7) //конец
                {
                    countCalc = 0;
                }

            }
            if (this.Count != 0)
            {

                startPosition = this[0].position;
               
                SearchEndPosition(cceInRaw, startPosition, out endPosition, out nameCE);

                MessageBox.Show("Ошибка в процедуре. Оператор: " + nameCE + ". Позиция " + this[0].position, "Ошибка в тексте процедуры");

                //MessageBox.Show("Ошибка проверки", "Ошибка");
                return;
            }

            endPosition = -1;
            startPosition = -1;
            return;
        }
        /// <summary>
        /// перебор исходного списка элементов процедуры. по позиции ошиибочного элемент поиск позиции , до которой стоит выделить формулу. так же выдает имя элемента
        /// </summary>
        /// <param name="cceInRaw"></param>
        /// <param name="position"></param>
        /// <param name="endPosition"></param>
        /// <param name="nameCE"></param>
        public void SearchEndPosition(CollectionCalcElements<CalcElement> cceInRaw, int position, out int endPosition, out string nameCE)
        {
            nameCE = string.Empty;
            bool searchEndEll = false;
            foreach (var ce in cceInRaw)
            {
                if ((ce.position == position) && !searchEndEll)
                {
                    if (ce.Type == CalcElement.TypeEn.Logic && (ce.TypeLogic == CalcElement.TypeLogicEn.Esli || ce.TypeLogic == CalcElement.TypeLogicEn.To || ce.TypeLogic == CalcElement.TypeLogicEn.Inachi))
                    {
                        nameCE = ce.ToString();
                        searchEndEll = true;
                        continue;
                    }
                    else
                    {
                        endPosition = position + ce.ToString().Count();
                        nameCE = ce.ToString();
                        return;
                    }
                }
                if (searchEndEll && (ce.Type == CalcElement.TypeEn.Logic && (ce.TypeLogic == CalcElement.TypeLogicEn.Esli || ce.TypeLogic == CalcElement.TypeLogicEn.To || ce.TypeLogic == CalcElement.TypeLogicEn.Inachi)))
                {
                    endPosition = +ce.position;
                    return;
                }
            }
            endPosition = cceInRaw[cceInRaw.Count() - 1].position;
        }


        private int CheckProcRecurs(CollectionCalcElements<T> cceInT, CollectionCalcElements<CalcElement> cceInCalcElement, CollectionCalcElements<CalcElement> calcZnach, int rang, ref int posichenError, List<string> listStringParamsName)
        {
            List<CalcElement> cceIn;
            int countCalc = 0;
            if (cceInT == null)
            {
                cceIn = cceInCalcElement;
            }
            else
            {
                cceIn = cceInT;
            }


            for (int i = 0; i < cceIn.Count; i++)
            {
                try
                {
                    var currCE = cceIn[i];
                    CalcElement preeCE = null;
                    if (i > 0)
                    {
                        preeCE = cceIn[i - 1];
                    }
                    CalcElement aftarCE = null;
                    if (i + 1 < cceIn.Count)
                    {
                        aftarCE = cceIn[i + 1];
                    }
                    CalcElement aftarAfterCE = null;
                    if (i + 1 + 1 < cceIn.Count)
                    {
                        aftarAfterCE = cceIn[i + 1 + 1];
                    }

                    //логические действия
                    if ((rang == 3 || rang == 4 || rang == 5 || rang == 6) && currCE.Type == CalcElement.TypeEn.Logic)
                    {
                        //Если
                        if (rang == 3 || rang == 5 || currCE.TypeLogic == CalcElement.TypeLogicEn.Esli)
                        {
                            ////
                            if (!((i + 3 < cceIn.Count) && cceIn[i + 3].Type == CalcElement.TypeEn.UnknownQuantity && cceIn[i + 3].Name == "T" && rang == 3)) //НЕ (трудоемкость и ранг 3)
                            {
                                if ((i + 5 < cceIn.Count) && aftarCE.Type == CalcElement.TypeEn.LogicValue &&
                                    (aftarAfterCE.Type == CalcElement.TypeEn.Logic && aftarAfterCE.TypeLogic == CalcElement.TypeLogicEn.To) &&
                                    (cceIn[i + 3].Type == CalcElement.TypeEn.UnknownQuantity && cceIn[i + 3].Name != null) &&
                                    (cceIn[i + 4].Type == CalcElement.TypeEn.Logic && cceIn[i + 4].TypeLogic == CalcElement.TypeLogicEn.Ravno) &&
                                    (cceIn[i + 5].Type == CalcElement.TypeEn.ValueNumber || cceIn[i + 5].Type == CalcElement.TypeEn.ValueText))
                                {

                                    if (true)
                                    {
                                        if (cceIn[i + 5].Type == CalcElement.TypeEn.ValueNumber)
                                        {
                                            calcZnach.Add(new CalcElement(cceIn[i + 3].Name, CalcElement.TypeEn.UnknownQuantity, cceIn[i + 5].ValueNumber.Value));
                                        }
                                        else
                                        {
                                            calcZnach.Add(new CalcElement(cceIn[i + 3].Name, CalcElement.TypeEn.UnknownQuantity, cceIn[i + 5].ValueText));
                                        }
                                    }
                                    cceIn.Remove(cceIn[i + 5]);
                                    cceIn.Remove(cceIn[i + 4]);
                                    cceIn.Remove(cceIn[i + 3]);


                                    cceIn.Remove(currCE);
                                    cceIn.Remove(aftarCE);
                                    cceIn.Remove(aftarAfterCE);

                                    countCalc++;
                                    break;
                                }
                            }
                        }
                        if ((rang == 4 || rang == 6) && currCE.TypeLogic == CalcElement.TypeLogicEn.Inachi)
                        {
                            if (!(aftarCE != null && aftarCE.Type == CalcElement.TypeEn.UnknownQuantity && aftarCE.Name == "T" && rang == 4)) //НЕ (трудоемкость и ранг 4)
                            {
                                if ((i + 3 < cceIn.Count) && aftarCE.Type == CalcElement.TypeEn.UnknownQuantity &&
                                (aftarAfterCE.Type == CalcElement.TypeEn.Logic && aftarAfterCE.TypeLogic == CalcElement.TypeLogicEn.Ravno) &&
                                (cceIn[i + 3].Type == CalcElement.TypeEn.ValueNumber || cceIn[i + 3].Type == CalcElement.TypeEn.ValueText))
                                {

                                    if (cceIn[i + 3].Type == CalcElement.TypeEn.ValueNumber)
                                    {
                                        calcZnach.Add(new CalcElement(cceIn[i + 1].Name, CalcElement.TypeEn.UnknownQuantity, cceIn[i + 3].ValueNumber.Value));
                                    }
                                    else
                                    {
                                        calcZnach.Add(new CalcElement(cceIn[i + 1].Name, CalcElement.TypeEn.UnknownQuantity, cceIn[i + 3].ValueText));
                                    }

                                    cceIn.Remove(cceIn[i + 3]);
                                    cceIn.Remove(currCE);
                                    cceIn.Remove(aftarCE);
                                    cceIn.Remove(aftarAfterCE);

                                    countCalc++;
                                    break;
                                }
                            }
                        }
                    }


                    //неизвестное ставшее известным заменим на его значение. 
                    if (currCE.Type == CalcElement.TypeEn.UnknownQuantity)
                    {
                        //перед неизвестным нет инчи или то 
                        if (!(i > 0 && (preeCE.Type == CalcElement.TypeEn.Logic && (preeCE.TypeLogic == CalcElement.TypeLogicEn.To || preeCE.TypeLogic == CalcElement.TypeLogicEn.Inachi))))
                        {

                            foreach (var ceForeach in calcZnach)
                            {
                                if (ceForeach.Type == CalcElement.TypeEn.UnknownQuantity && currCE.Name != null && ceForeach.Name == currCE.Name)
                                {
                                    cceIn.Remove(currCE);
                                    if (ceForeach.ValueNumber != null)
                                    {
                                        cceIn.Insert(i, new CalcElement(ceForeach.ValueNumber.Value) { position = currCE.position });
                                    }
                                    else
                                    {
                                        cceIn.Insert(i, new CalcElement(ceForeach.ValueText) { position = currCE.position });
                                    }
                                }

                            }
                        }
                        //есл это ранг 6(конец вычислений) перед неизвестным нет нечего, за ним идет равно и далее значение
                        if (rang == 6 && preeCE == null && aftarCE != null && aftarAfterCE != null && aftarCE.Type == CalcElement.TypeEn.Logic && aftarCE.TypeLogic == CalcElement.TypeLogicEn.Ravno)
                        {

                            if (aftarAfterCE.Type == CalcElement.TypeEn.ValueNumber)
                            {
                                calcZnach.Add(new CalcElement(currCE.Name, CalcElement.TypeEn.UnknownQuantity, aftarAfterCE.ValueNumber.Value));
                            }
                            else
                            {
                                calcZnach.Add(new CalcElement(currCE.Name, CalcElement.TypeEn.UnknownQuantity, aftarAfterCE.ValueText));
                            }
                            cceIn.Remove(currCE);
                            cceIn.Remove(aftarCE);
                            cceIn.Remove(aftarAfterCE);

                        }
                    }

                    //неизвестное с вложением
                    if (currCE.Type == CalcElement.TypeEn.UnknownQuantity)
                    {

                        if (currCE.listCalcElements.Count == 1)
                        {

                            cceIn.Insert(i, currCE.listCalcElements[0]);

                            cceIn.Remove(currCE);
                            countCalc++;
                        }
                        if (currCE.listCalcElements.Count > 1)
                        {
                            int resultCheckProcRecurs = CheckProcRecurs(null, currCE.listCalcElements, calcZnach, rang, ref posichenError, listStringParamsName);
                            if (resultCheckProcRecurs == -1)
                            {
                                if (posichenError == 0)
                                {
                                    posichenError = currCE.position;

                                }
                                return -1;
                            }
                            countCalc += resultCheckProcRecurs;
                        }
                    }

                    //если у элементов Ln и  Pow вложенного списка есть вложенные
                    if (currCE.Type == CalcElement.TypeEn.Math && (currCE.TypeMath == CalcElement.TypeMathEn.Ln || currCE.TypeMath == CalcElement.TypeMathEn.Pow))
                    {

                        for (int iListCalcElements = 0; iListCalcElements < currCE.listCalcElements.Count; iListCalcElements++)
                        {
                            //if (currCE.listCalcElements[iListCalcElements].listCalcElements.Count > 0)
                            if (currCE.listCalcElements[iListCalcElements].Type != CalcElement.TypeEn.ValueNumber)
                            {


                                int resultCheckProcRecurs = CheckProcRecurs(null, currCE.listCalcElements, calcZnach, rang, ref posichenError, listStringParamsName);
                                if (resultCheckProcRecurs == -1)
                                {

                                    if (posichenError == 0)
                                    {
                                        posichenError = currCE.position;
                                    }
                                    return -1;
                                }
                                countCalc += resultCheckProcRecurs;
                                break;
                            }
                        }

                    }

                    //параметр
                    if (currCE.Type == CalcElement.TypeEn.Param)
                    {
                        if (listStringParamsName.Where(o => o == currCE.Name).Count() > 0)
                        {
                            cceIn.Remove(currCE);
                            countCalc++;
                            cceIn.Insert(i, new CalcElement(1) { typeParam = CalcElement.TypeParamEn.Number, position = currCE.position });
                        }
                        else
                        {
                            MessageBox.Show("Неизвестный параметр: " + currCE.Name, "Ошибка в тексте процедуры");
                            posichenError = currCE.position;
                            return -1;
                        }


                    }
                    //скобки
                    if (currCE.Type == CalcElement.TypeEn.Brackets)
                    {
                        if (currCE.TypeBrackets == CalcElement.TypeBracketsEn.Open)
                        {
                            if ((aftarCE.Type == CalcElement.TypeEn.ValueNumber || aftarCE.Type == CalcElement.TypeEn.LogicValue) &&
                                aftarAfterCE.Type == CalcElement.TypeEn.Brackets &&
                                aftarAfterCE.TypeBrackets == CalcElement.TypeBracketsEn.Close) //если в скобачках число или логический результат, то скобочки убираем
                            {
                                cceIn.Remove(currCE);
                                cceIn.Remove(aftarAfterCE);
                                countCalc++;
                                break;
                            }
                        }
                    }
                    //Матем действия
                    if (currCE.Type == CalcElement.TypeEn.Math)
                    {
                        //число с минусом в отриц число
                        if (rang == 0 && currCE.TypeMath == CalcElement.TypeMathEn.Minus)
                        {
                            if (preeCE.Type != CalcElement.TypeEn.ValueNumber &&
                                (preeCE.Type != CalcElement.TypeEn.Brackets || (preeCE.Type == CalcElement.TypeEn.Brackets && preeCE.TypeBrackets != CalcElement.TypeBracketsEn.Close))
                                && (preeCE.Type != CalcElement.TypeEn.Math || (preeCE.Type == CalcElement.TypeEn.Math && preeCE.TypeMath != CalcElement.TypeMathEn.Pow && preeCE.TypeMath != CalcElement.TypeMathEn.Ln))) //если перед минусом не число и не закрывающаяся скобка
                            {

                                cceIn.Insert(i, new CalcElement(-1.0 * aftarCE.ValueNumber.Value) { position = currCE.position });
                                cceIn.Remove(currCE);
                                cceIn.Remove(aftarCE);
                                countCalc++;
                                break;

                            }

                        }
                        //ln и pow
                        if (rang == 0 && (currCE.TypeMath == CalcElement.TypeMathEn.Ln || currCE.TypeMath == CalcElement.TypeMathEn.Pow || currCE.TypeMath == CalcElement.TypeMathEn.Stepen))
                        {
                            if (currCE.TypeMath == CalcElement.TypeMathEn.Ln)
                            {
                                if (currCE.listCalcElements != null && currCE.listCalcElements.Count == 1 && currCE.listCalcElements[0].Type == CalcElement.TypeEn.ValueNumber)
                                {
                                    cceIn.Remove(currCE);
                                    cceIn.Insert(i, new CalcElement(Math.Log(currCE.listCalcElements[0].ValueNumber.Value)) { position = currCE.position });
                                    countCalc++;
                                    break;
                                }
                            }
                            else if (rang == 0 && currCE.TypeMath == CalcElement.TypeMathEn.Pow)
                            {
                                if (currCE.listCalcElements != null && currCE.listCalcElements.Count == 2 && currCE.listCalcElements[0].Type == CalcElement.TypeEn.ValueNumber && currCE.listCalcElements[1].Type == CalcElement.TypeEn.ValueNumber)
                                {
                                    cceIn.Remove(currCE);
                                    cceIn.Insert(i, new CalcElement(Math.Pow(currCE.listCalcElements[0].ValueNumber.Value, currCE.listCalcElements[1].ValueNumber.Value)) { position = currCE.position });
                                    countCalc++;
                                    break;
                                }
                            }
                            else if (currCE.TypeMath == CalcElement.TypeMathEn.Stepen)
                            {
                                if (preeCE.Type == CalcElement.TypeEn.ValueNumber && aftarCE.Type == CalcElement.TypeEn.ValueNumber)
                                {

                                    cceIn.Insert(i, new CalcElement(Math.Pow(preeCE.ValueNumber.Value, aftarCE.ValueNumber.Value)) { position = currCE.position });
                                    cceIn.Remove(currCE);
                                    cceIn.Remove(preeCE);
                                    cceIn.Remove(aftarCE);
                                    countCalc++;
                                    break;

                                }
                            }
                        }
                        //делить и умножить
                        else if (rang == 0 && (currCE.TypeMath == CalcElement.TypeMathEn.Delit || currCE.TypeMath == CalcElement.TypeMathEn.Umnozhit))

                        {
                            if (currCE.TypeMath == CalcElement.TypeMathEn.Delit)
                            {
                                if (preeCE.Type == CalcElement.TypeEn.ValueNumber && aftarCE.Type == CalcElement.TypeEn.ValueNumber)
                                {
                                    if (aftarCE.ValueNumber.Value != 0)
                                    {
                                        cceIn.Insert(i, new CalcElement(preeCE.ValueNumber.Value / aftarCE.ValueNumber.Value) { position = currCE.position });
                                        cceIn.Remove(currCE);
                                        cceIn.Remove(preeCE);
                                        cceIn.Remove(aftarCE);
                                        countCalc++;
                                        break;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Делить на 0 нельзя");
                                        posichenError = currCE.position;
                                        return -1;
                                    }

                                }
                            }
                            else if (currCE.TypeMath == CalcElement.TypeMathEn.Umnozhit)
                            {
                                if (preeCE.Type == CalcElement.TypeEn.ValueNumber && aftarCE.Type == CalcElement.TypeEn.ValueNumber)
                                {

                                    cceIn.Insert(i, new CalcElement(preeCE.ValueNumber.Value * aftarCE.ValueNumber.Value) { position = currCE.position });
                                    cceIn.Remove(currCE);
                                    cceIn.Remove(preeCE);
                                    cceIn.Remove(aftarCE);
                                    countCalc++;
                                    break;

                                }
                            }
                        }

                        //плюc и минус
                        else if (rang == 1 && currCE.TypeMath == CalcElement.TypeMathEn.Plus || currCE.TypeMath == CalcElement.TypeMathEn.Minus)
                        {
                            if (currCE.TypeMath == CalcElement.TypeMathEn.Minus)
                            {
                                if (preeCE.Type == CalcElement.TypeEn.ValueNumber && aftarCE.Type == CalcElement.TypeEn.ValueNumber)
                                {

                                    cceIn.Insert(i, new CalcElement(preeCE.ValueNumber.Value - aftarCE.ValueNumber.Value) { position = currCE.position });
                                    cceIn.Remove(currCE);
                                    cceIn.Remove(preeCE);
                                    cceIn.Remove(aftarCE);

                                    countCalc++;
                                    break;
                                }
                            }
                            else if (currCE.TypeMath == CalcElement.TypeMathEn.Plus)
                            {
                                if (preeCE.Type == CalcElement.TypeEn.ValueNumber && aftarCE.Type == CalcElement.TypeEn.ValueNumber)
                                {

                                    cceIn.Insert(i, new CalcElement(preeCE.ValueNumber.Value + aftarCE.ValueNumber.Value) { position = currCE.position });
                                    cceIn.Remove(currCE);
                                    cceIn.Remove(preeCE);
                                    cceIn.Remove(aftarCE);
                                    countCalc++;
                                    break;
                                }
                            }
                        }
                    }

                    //логические действия
                    if (rang == 2 && currCE.Type == CalcElement.TypeEn.Logic)
                    {
                        //больше
                        if (currCE.TypeLogic == CalcElement.TypeLogicEn.Bolshe)
                        {
                            //
                            if (preeCE.Type == CalcElement.TypeEn.ValueNumber && aftarCE.Type == CalcElement.TypeEn.ValueNumber)
                            {

                                cceIn.Insert(i, (preeCE.ValueNumber.Value > aftarCE.ValueNumber.Value) ? (new CalcElement(true) { position = currCE.position }) : (new CalcElement(false) { position = currCE.position }));
                                cceIn.Remove(currCE);
                                cceIn.Remove(aftarCE);
                                cceIn.Remove(preeCE);
                                countCalc++;
                                break;
                            }
                        }
                        //меньше
                        if (currCE.TypeLogic == CalcElement.TypeLogicEn.Menshe)
                        {
                            if (preeCE.Type == CalcElement.TypeEn.ValueNumber && aftarCE.Type == CalcElement.TypeEn.ValueNumber)
                            {

                                cceIn.Insert(i, (preeCE.ValueNumber.Value < aftarCE.ValueNumber.Value) ? (new CalcElement(true) { position = currCE.position }) : (new CalcElement(false) { position = currCE.position }));
                                cceIn.Remove(currCE);
                                cceIn.Remove(aftarCE);
                                cceIn.Remove(preeCE);
                                countCalc++;
                                break;
                            }
                        }
                        //больше равно
                        if (currCE.TypeLogic == CalcElement.TypeLogicEn.BolsheRavno)
                        {
                            if (preeCE.Type == CalcElement.TypeEn.ValueNumber && aftarCE.Type == CalcElement.TypeEn.ValueNumber)
                            {

                                cceIn.Insert(i, (preeCE.ValueNumber.Value >= aftarCE.ValueNumber.Value) ? (new CalcElement(true) { position = currCE.position }) : (new CalcElement(false) { position = currCE.position }));
                                cceIn.Remove(currCE);
                                cceIn.Remove(aftarCE);
                                cceIn.Remove(preeCE);
                                countCalc++;
                                break;
                            }
                        }
                        //Меньше равно
                        if (currCE.TypeLogic == CalcElement.TypeLogicEn.MensheRavno)
                        {
                            if (preeCE.Type == CalcElement.TypeEn.ValueNumber && aftarCE.Type == CalcElement.TypeEn.ValueNumber)
                            {

                                cceIn.Insert(i, (preeCE.ValueNumber.Value <= aftarCE.ValueNumber.Value) ? (new CalcElement(true) { position = currCE.position }) : (new CalcElement(false) { position = currCE.position }));
                                cceIn.Remove(currCE);
                                cceIn.Remove(aftarCE);
                                cceIn.Remove(preeCE);

                                countCalc++;
                                break;
                            }
                        }
                        //равно
                        if (currCE.TypeLogic == CalcElement.TypeLogicEn.Ravno)
                        {
                            //перед равно нет то или инче 
                            if (!(i > 1 && (cceIn[i - 2].Type == CalcElement.TypeEn.Logic && (cceIn[i - 2].TypeLogic == CalcElement.TypeLogicEn.To || cceIn[i - 2].TypeLogic == CalcElement.TypeLogicEn.Inachi))))
                            {

                                if (preeCE.Type == CalcElement.TypeEn.ValueNumber && aftarCE.Type == CalcElement.TypeEn.ValueNumber)
                                {
                                    cceIn.Insert(i, (preeCE.ValueNumber.Value == aftarCE.ValueNumber.Value) ? (new CalcElement(true) { position = currCE.position }) : (new CalcElement(false) { position = currCE.position }));
                                    cceIn.Remove(currCE);
                                    cceIn.Remove(aftarCE);
                                    cceIn.Remove(preeCE);
                                    countCalc++;
                                    break;
                                }
                                //если был параметр, который заменился на число
                                if ((preeCE.Type == CalcElement.TypeEn.ValueText || preeCE.typeParam != null) && (aftarCE.Type == CalcElement.TypeEn.ValueText ))
                                {

                                    cceIn.Insert(i, (preeCE.ValueText == aftarCE.ValueText) ? (new CalcElement(true) { position = currCE.position }) : (new CalcElement(false) { position = currCE.position }));
                                    cceIn.Remove(currCE);
                                    cceIn.Remove(aftarCE);
                                    cceIn.Remove(preeCE);
                                    countCalc++;
                                    break;
                                }
                            }
                        }

                        //И
                        if (currCE.TypeLogic == CalcElement.TypeLogicEn.I)
                        {
                            if (preeCE.Type == CalcElement.TypeEn.LogicValue && aftarCE.Type == CalcElement.TypeEn.LogicValue)
                            {

                                cceIn.Insert(i, (preeCE.LogicValue.Value && aftarCE.LogicValue.Value) ? (new CalcElement(true) { position = currCE.position }) : (new CalcElement(false) { position = currCE.position }));
                                cceIn.Remove(currCE);
                                cceIn.Remove(aftarCE);
                                cceIn.Remove(preeCE);
                                countCalc++;
                                break;
                            }
                        }
                        //Или
                        if (currCE.TypeLogic == CalcElement.TypeLogicEn.Ili)
                        {
                            if (preeCE.Type == CalcElement.TypeEn.LogicValue && aftarCE.Type == CalcElement.TypeEn.LogicValue)
                            {

                                cceIn.Insert(i, (preeCE.LogicValue.Value || aftarCE.LogicValue.Value) ? (new CalcElement(true) { position = currCE.position }) : (new CalcElement(false) { position = currCE.position }));
                                cceIn.Remove(currCE);
                                cceIn.Remove(aftarCE);
                                cceIn.Remove(preeCE);
                                countCalc++;
                                break;
                            }
                        }
                        //из списка
                        if (currCE.TypeLogic == CalcElement.TypeLogicEn.IzSpiska)
                        {
                            if (preeCE.Type == CalcElement.TypeEn.ValueText || preeCE.Type == CalcElement.TypeEn.ValueNumber)
                            {

                                bool izSpiskaTrue = false;
                                //foreach (var ceFor in currCE.listCalcElements)
                                //{
                                //    if (ceFor.ValueText != null)
                                //    {
                                //        if (ceFor.ValueText.ToString() == preeCE.ValueText.ToString())
                                //        {
                                //            izSpiskaTrue = true;
                                //            break;
                                //        }
                                //    }
                                //}
                                cceIn.Insert(i, new CalcElement(izSpiskaTrue) { position = currCE.position });
                                cceIn.Remove(currCE);

                                cceIn.Remove(preeCE);
                                countCalc++;
                                break;
                            }
                        }
                        //в диапазоне
                        if (currCE.TypeLogic == CalcElement.TypeLogicEn.VDiadazone)
                        {
                            if (preeCE.Type == CalcElement.TypeEn.ValueNumber)
                            {


                                cceIn.Insert(i, new CalcElement(preeCE.ValueNumber.Value >= currCE.listCalcElements[0].ValueNumber.Value && preeCE.ValueNumber.Value < currCE.listCalcElements[1].ValueNumber.Value) { position = currCE.position });
                                cceIn.Remove(currCE);

                                cceIn.Remove(preeCE);
                                countCalc++;
                                break;
                            }
                        }

                    }
                }
                catch (NullReferenceException ex)
                {
                    posichenError = cceIn[i].position;
                    MessageBox.Show("Ошибка расчета в операторе " + cceIn[i] + ", с позицией в процедуре " + cceIn[i].position);
                    throw new NullReferenceException();
                }
                catch (InvalidOperationException ex)
                {
                    posichenError = cceIn[i].position;
                    MessageBox.Show("Ошибка расчета в операторе " + cceIn[i] + ", с позицией в процедуре " + cceIn[i].position);
                    throw new InvalidOperationException();
                }
                catch
                {
                    posichenError = cceIn[i].position;
                    MessageBox.Show("Ошибка расчета в операторе " + cceIn[i] + ", с позицией в процедуре " + cceIn[i].position);
                    throw new Exception();
                }
            }
            return countCalc;
        }
// Вычисление результата расчёта процедуры по списку параметров valuesParams
        public string CalcResult(string[,] valueParams) //, List<CalcElement> listParams
        {

            CollectionCalcElements<CalcElement> ceUncknows = new CollectionCalcElements<CalcElement>();


            for (int i = 0; i < this.Count; i++)
            {
                CalcElement ce = this[i];
                if (ce.Type == CalcElement.TypeEn.UnknownQuantity)
                {
                    if (ce.listCalcElements.Count == 0 && ceUncknows.Where(o => o.Name == ce.Name).Count() == 0)
                    {
                        ceUncknows.Add(ce);
                    }
                }
            }


            bool IzmeneniyaBili = false;
            int rang = -1;

            int countCalc = 1;
            int stop = 6000; // КАА это максимальное число параметров процедуры? 
            List<CalcElement> listCEPar = new List<CalcElement>();
            if (valueParams != null) 
            {
                for (int i = 0; i < valueParams.Length / 2; i++)
                {
                    var valueP = UtilsCalc.UniversalParsing(valueParams[1, i]);
                    if (valueP == null)
                    {
                        listCEPar.Add(new CalcElement(valueParams[0, i], valueParams[1, i]));
                    }
                    else
                    {
                        listCEPar.Add(new CalcElement(valueParams[0, i], valueP.Value));

                    }
                }
            }
            while (countCalc > 0 && stop > 0)
            {
                stop--;
                countCalc = 0;
                //массив строк в список параметров

                
                try
                {
                    countCalc = CalcProcRecurs(this, null, listCEPar, rang);
                }
                catch
                {
                    return null;
                }
                if (countCalc > 0) //что то изменилось
                {
                    IzmeneniyaBili = true;
                    rang = -1;
                }
                if (IzmeneniyaBili && countCalc == 0)  // при этом ранге нет изменений но в этот перебор рангов изменения были
                {
                    IzmeneniyaBili = false;
                    rang = -1;
                    countCalc++;
                }
                else if (!IzmeneniyaBili)//в этот проход рангов измений не было 
                {
                    rang++;
                    countCalc++;
                }
                if (rang == 7) //конец
                {
                    countCalc = 0;
                }

            }

            bool neVichislennieT = false;
            if (this.Count != 0)
            {
                MessageBox.Show("Ошибка в процедуре расчета", "Ошибка");
                neVichislennieT = true;
            }
            string vichislennieKoef = string.Empty;
            string neVichislennieKoef = string.Empty;
           
            double returnDb = -1.0;
            foreach (var uCe in ceUncknows)
            {

                bool koeffNaiden = false;
                foreach (var ce in listCEPar)
                {

                    if (ce.Type == CalcElement.TypeEn.UnknownQuantity && ce.Name == uCe.Name)
                    {
                        koeffNaiden = true;

                        if (ce.Name.Trim() == "Result")
                        {
                            returnDb = ce.ValueNumber.Value;
                        }
                        else
                        {
                            vichislennieKoef += ce.ValueNumber.Value.ToString("F2") + ", ";
                          //  vichislennieKoef += ce.Name + "= " + ce.ValueNumber.Value.ToString("F2") + ", ";
                        }
                    }
                }
                if (!koeffNaiden)
                {

                    if (uCe.Name == "T")
                    {
                        neVichislennieT = true;
                    }
                    else
                    {
                        neVichislennieKoef += uCe.Name + ", ";
                    }
                }
            }
            string returnString = null;

            if (neVichislennieT)
            {
                MessageBox.Show("При введённых значениях параметров не удалось вычислить результат", "Ошибка");
            }
            else if (!neVichislennieT)
            {
                //MessageBox.Show(returnDb.ToString("F2"), "Сообщение");
                returnString = returnDb.ToString("F2");
            }
            returnDb = Math.Round(returnDb, 2);


            return returnString;
        }

        /// <summary>
        /// расчет куска процедуры
        /// </summary>
        /// <param name="cceInT">входной массив элементов</param>
        /// <param name="cceInCalcElement">тоже входной массив элементов. один из них не null . связано с типом массива</param>
        /// <param name="listCEPar">параметры</param>
        /// <param name="rang">ранг общитовыемых операций</param>
        /// <returns>количество расчитаных операций</returns>
        private static int CalcProcRecurs(CollectionCalcElements<T> cceInT, CollectionCalcElements<CalcElement> cceInCalcElement, List<CalcElement> listCEPar, int rang)
        {
            List<CalcElement> cceIn;//входной массив элементов
            int countCalc = 0;
            if (cceInT == null)
            {
                cceIn = cceInCalcElement;
            }
            else
            {
                cceIn = cceInT;
            }
            int oldCountCceIn=-1;

            //перебор входн массива
            for (int i = 0; i < cceIn.Count; i++)
            {
                if (oldCountCceIn > 0 && oldCountCceIn != cceIn.Count)
                {
                    oldCountCceIn = cceIn.Count;
                }
                oldCountCceIn = cceIn.Count;
                try
                {
                    //для удобства создаем элемент до текущего и после 
                    var currCE = cceIn[i];
                    CalcElement preeCE = null;
                    if (i > 0)
                    {
                        preeCE = cceIn[i - 1];
                    }
                    CalcElement aftarCE = null;
                    if (i + 1 < cceIn.Count)
                    {
                        aftarCE = cceIn[i + 1];
                    }
                    CalcElement aftarAfterCE = null;
                    if (i + 1 + 1 < cceIn.Count)
                    {
                        aftarAfterCE = cceIn[i + 1 + 1];
                    }

                    //логические действия
                    if ((rang == 3 || rang == 4 || rang == 5 || rang == 6) && currCE.Type == CalcElement.TypeEn.Logic)
                    {
                        //Если
                        if (rang == 3 || rang == 5 || currCE.TypeLogic == CalcElement.TypeLogicEn.Esli)//ранг 5 для расчета трудоемкости
                        {
                            ////
                            if (!((i + 3 < cceIn.Count) && cceIn[i + 3].Type == CalcElement.TypeEn.UnknownQuantity && cceIn[i + 3].Name == "T" && rang == 3)) //НЕ (трудоемкость и ранг 3)
                            {
                                if ((i + 5 < cceIn.Count) && aftarCE.Type == CalcElement.TypeEn.LogicValue &&
                                    (aftarAfterCE.Type == CalcElement.TypeEn.Logic && aftarAfterCE.TypeLogic == CalcElement.TypeLogicEn.To) &&
                                    (cceIn[i + 3].Type == CalcElement.TypeEn.UnknownQuantity && cceIn[i + 3].Name != null) &&
                                    (cceIn[i + 4].Type == CalcElement.TypeEn.Logic && cceIn[i + 4].TypeLogic == CalcElement.TypeLogicEn.Ravno) &&
                                    (cceIn[i + 5].Type == CalcElement.TypeEn.ValueNumber || cceIn[i + 5].Type == CalcElement.TypeEn.ValueText))
                                {

                                    if (aftarCE.LogicValue.Value)
                                    {
                                        if (cceIn[i + 5].Type == CalcElement.TypeEn.ValueNumber)
                                        {
                                            listCEPar.Add(new CalcElement(cceIn[i + 3].Name, CalcElement.TypeEn.UnknownQuantity, cceIn[i + 5].ValueNumber.Value));
                                        }
                                        else
                                        {
                                            listCEPar.Add(new CalcElement(cceIn[i + 3].Name, CalcElement.TypeEn.UnknownQuantity, cceIn[i + 5].ValueText));
                                        }
                                    }
                                    cceIn.Remove(cceIn[i + 5]);
                                    cceIn.Remove(cceIn[i + 4]);
                                    cceIn.Remove(cceIn[i + 3]);


                                    cceIn.Remove(currCE);
                                    cceIn.Remove(aftarCE);
                                    cceIn.Remove(aftarAfterCE);

                                    countCalc++;
                                    break;
                                }
                            }
                        }
                        if ((rang == 4 || rang == 6) && currCE.TypeLogic == CalcElement.TypeLogicEn.Inachi)
                        {
                            if (!(aftarCE != null && aftarCE.Type == CalcElement.TypeEn.UnknownQuantity && aftarCE.Name == "T" && rang == 4)) //НЕ (трудоемкость и ранг 4)
                            {
                                if ((i + 3 < cceIn.Count) && aftarCE.Type == CalcElement.TypeEn.UnknownQuantity &&
                                (aftarAfterCE.Type == CalcElement.TypeEn.Logic && aftarAfterCE.TypeLogic == CalcElement.TypeLogicEn.Ravno) &&
                                (cceIn[i + 3].Type == CalcElement.TypeEn.ValueNumber || cceIn[i + 3].Type == CalcElement.TypeEn.ValueText))
                                {
                                    bool elementPodschitan = false;
                                    foreach (var cePar in listCEPar)
                                    {
                                        if (cePar.Name == aftarCE.Name)
                                        {
                                            elementPodschitan = true;
                                        }
                                    }

                                    if (!elementPodschitan)
                                    {
                                        if (cceIn[i + 3].Type == CalcElement.TypeEn.ValueNumber)
                                        {
                                            listCEPar.Add(new CalcElement(cceIn[i + 1].Name, CalcElement.TypeEn.UnknownQuantity, cceIn[i + 3].ValueNumber.Value));
                                        }
                                        else
                                        {
                                            listCEPar.Add(new CalcElement(cceIn[i + 1].Name, CalcElement.TypeEn.UnknownQuantity, cceIn[i + 3].ValueText));
                                        }
                                    }
                                    cceIn.Remove(cceIn[i + 3]);
                                    cceIn.Remove(currCE);
                                    cceIn.Remove(aftarCE);
                                    cceIn.Remove(aftarAfterCE);

                                    countCalc++;
                                    break;
                                }
                            }
                        }
                    }



                    //неизвестное ставшее известным заменим на его значение. 
                    if (currCE.Type == CalcElement.TypeEn.UnknownQuantity)
                    {
                        //перед неизвестным нет инчи или то 
                        if (!(i > 0 && (preeCE.Type == CalcElement.TypeEn.Logic && (preeCE.TypeLogic == CalcElement.TypeLogicEn.To || preeCE.TypeLogic == CalcElement.TypeLogicEn.Inachi))))
                        {
                            //if (preeCE.Type != CalcElement.TypeEn.Logic || (preeCE.Type == CalcElement.TypeEn.Logic && preeCE.TypeLogic != CalcElement.TypeLogicEn.To))
                            //{
                            foreach (var ceForeach in listCEPar)
                            {
                                if (ceForeach.Type == CalcElement.TypeEn.UnknownQuantity && currCE.Name != null && ceForeach.Name == currCE.Name)
                                {
                                    cceIn.Remove(currCE);
                                    if (ceForeach.ValueNumber != null)
                                    {
                                        cceIn.Insert(i, new CalcElement(ceForeach.ValueNumber.Value) { position = currCE.position });
                                    }
                                    else
                                    {
                                        cceIn.Insert(i, new CalcElement(ceForeach.ValueText) { position = currCE.position });
                                    }
                                }
                            }
                        }
                        //есл это ранг 6(конец вычислений) перед неизвестным нет нечего, за ним идет равно и далее значение
                        if (rang == 6 && preeCE == null && aftarCE != null && aftarAfterCE != null && aftarCE.Type == CalcElement.TypeEn.Logic && aftarCE.TypeLogic == CalcElement.TypeLogicEn.Ravno)
                        {

                            if (aftarAfterCE.Type == CalcElement.TypeEn.ValueNumber)
                            {
                                listCEPar.Add(new CalcElement(currCE.Name, CalcElement.TypeEn.UnknownQuantity, aftarAfterCE.ValueNumber.Value));
                            }
                            else
                            {
                                listCEPar.Add(new CalcElement(currCE.Name, CalcElement.TypeEn.UnknownQuantity, aftarAfterCE.ValueText));
                            }

                            cceIn.Remove(currCE);
                            cceIn.Remove(aftarCE);
                            cceIn.Remove(aftarAfterCE);

                        }
                    }

                    //неизвестное с вложением
                    if (currCE.Type == CalcElement.TypeEn.UnknownQuantity)
                    {

                        if (currCE.listCalcElements.Count == 1)
                        {

                            cceIn.Insert(i, currCE.listCalcElements[0]);

                            cceIn.Remove(currCE);
                            countCalc++;
                        }
                        if (currCE.listCalcElements.Count > 1)
                        {
                            countCalc += CalcProcRecurs(null, currCE.listCalcElements, listCEPar, rang);
                        }
                    }

                    //если у элементов Ln и  Pow вложенного списка есть вложенные
                    if (currCE.Type == CalcElement.TypeEn.Math && (currCE.TypeMath == CalcElement.TypeMathEn.Ln || currCE.TypeMath == CalcElement.TypeMathEn.Pow))
                    {

                        for (int iListCalcElements = 0; iListCalcElements < currCE.listCalcElements.Count; iListCalcElements++)
                        {
                            //if (currCE.listCalcElements[iListCalcElements].listCalcElements.Count > 0)
                            if (currCE.listCalcElements[iListCalcElements].Type != CalcElement.TypeEn.ValueNumber)
                            {
                                countCalc += CalcProcRecurs(null, currCE.listCalcElements, listCEPar, rang);
                                break;
                            }
                        }

                    }

                    //параметр
                    if (currCE.Type == CalcElement.TypeEn.Param)
                    {

                        bool parametrNaide = false;

                        foreach (var param in listCEPar)
                        {

                            if (currCE.Name == param.Name)
                            {
                                cceIn.Remove(currCE);
                                countCalc++;
                                parametrNaide = true;
                                if (param.ValueText != null)
                                {
                                    cceIn.Insert(i, new CalcElement(param.ValueText) { position = currCE.position });
                                }
                                else if (param.ValueNumber != null)
                                {
                                    cceIn.Insert(i, new CalcElement(param.ValueNumber.Value) { position = currCE.position });
                                }

                                break;
                            }


                        }
                        if (!parametrNaide)
                        {
                            MessageBox.Show("Не найден параметр " + currCE.Name, "Ошибка!");
                            return -1;
                        }

                    }
                    //скобки
                    if (currCE.Type == CalcElement.TypeEn.Brackets)
                    {
                        if (currCE.TypeBrackets == CalcElement.TypeBracketsEn.Open)
                        {
                            if ((aftarCE.Type == CalcElement.TypeEn.ValueNumber || aftarCE.Type == CalcElement.TypeEn.LogicValue) &&
                                aftarAfterCE.Type == CalcElement.TypeEn.Brackets &&
                                aftarAfterCE.TypeBrackets == CalcElement.TypeBracketsEn.Close) //если в скобачках число или логический результат, то скобочки убираем
                            {
                                cceIn.Remove(currCE);
                                cceIn.Remove(aftarAfterCE);
                                countCalc++;
                                break;
                            }
                        }
                    }
                    //Матем действия
                    if (currCE.Type == CalcElement.TypeEn.Math)
                    {
                        //число с минусом в отриц число
                        if (rang == 0 && currCE.TypeMath == CalcElement.TypeMathEn.Minus)
                        {
                            if (preeCE.Type != CalcElement.TypeEn.ValueNumber &&
                                (preeCE.Type != CalcElement.TypeEn.Brackets || (preeCE.Type == CalcElement.TypeEn.Brackets && preeCE.TypeBrackets != CalcElement.TypeBracketsEn.Close))
                                && (preeCE.Type != CalcElement.TypeEn.Math || (preeCE.Type == CalcElement.TypeEn.Math && preeCE.TypeMath != CalcElement.TypeMathEn.Pow && preeCE.TypeMath != CalcElement.TypeMathEn.Ln))) //если перед минусом не число и не закрывающаяся скобка
                            {

                                cceIn.Insert(i, new CalcElement(-1.0 * aftarCE.ValueNumber.Value) { position = currCE.position });
                                cceIn.Remove(currCE);
                                cceIn.Remove(aftarCE);
                                countCalc++;
                                break;

                            }

                        }
                        //ln и pow
                        if (rang == -1 && (currCE.TypeMath == CalcElement.TypeMathEn.Ln || currCE.TypeMath == CalcElement.TypeMathEn.Pow || currCE.TypeMath == CalcElement.TypeMathEn.Stepen))
                        {


                            if (currCE.TypeMath == CalcElement.TypeMathEn.Ln)
                            {
                                if (currCE.listCalcElements != null && currCE.listCalcElements.Count == 1 && currCE.listCalcElements[0].Type == CalcElement.TypeEn.ValueNumber)
                                {
                                    cceIn.Remove(currCE);
                                    cceIn.Insert(i, new CalcElement(Math.Log(currCE.listCalcElements[0].ValueNumber.Value)) { position = currCE.position });
                                    countCalc++;
                                    break;
                                }
                            }
                            else if ( currCE.TypeMath == CalcElement.TypeMathEn.Pow)
                            {
                                if (currCE.listCalcElements != null && currCE.listCalcElements.Count == 2 && currCE.listCalcElements[0].Type == CalcElement.TypeEn.ValueNumber && currCE.listCalcElements[1].Type == CalcElement.TypeEn.ValueNumber)
                                {
                                    cceIn.Remove(currCE);
                                    cceIn.Insert(i, new CalcElement(Math.Pow(currCE.listCalcElements[0].ValueNumber.Value, currCE.listCalcElements[1].ValueNumber.Value)) { position = currCE.position });
                                    countCalc++;
                                    break;
                                }
                            }
                            else if (currCE.TypeMath == CalcElement.TypeMathEn.Stepen)
                            {
                                if (preeCE.Type == CalcElement.TypeEn.ValueNumber && aftarCE.Type == CalcElement.TypeEn.ValueNumber)
                                {
                                    cceIn.Insert(i, new CalcElement(Math.Pow(preeCE.ValueNumber.Value, aftarCE.ValueNumber.Value)) { position = currCE.position });
                                    cceIn.Remove(currCE);
                                    cceIn.Remove(preeCE);
                                    cceIn.Remove(aftarCE);
                                    countCalc++;
                                    break;
                                }
                            }
                        }
                        //делить и умножить
                        else if (rang == 0 && (currCE.TypeMath == CalcElement.TypeMathEn.Delit || currCE.TypeMath == CalcElement.TypeMathEn.Umnozhit))

                        {
                            if (currCE.TypeMath == CalcElement.TypeMathEn.Delit)
                            {
                                if (preeCE.Type == CalcElement.TypeEn.ValueNumber && aftarCE.Type == CalcElement.TypeEn.ValueNumber)
                                {
                                    if (aftarCE.ValueNumber.Value != 0)
                                    {

                                        cceIn.Insert(i, new CalcElement(preeCE.ValueNumber.Value / aftarCE.ValueNumber.Value) { position = currCE.position });
                                        cceIn.Remove(currCE);
                                        cceIn.Remove(preeCE);
                                        cceIn.Remove(aftarCE);
                                        countCalc++;
                                        break;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Делить на 0 нельзя");
                                        return -1;
                                    }

                                }
                            }
                            else if (currCE.TypeMath == CalcElement.TypeMathEn.Umnozhit)
                            {
                                if (preeCE.Type == CalcElement.TypeEn.ValueNumber && aftarCE.Type == CalcElement.TypeEn.ValueNumber)
                                {

                                    cceIn.Insert(i, new CalcElement(preeCE.ValueNumber.Value * aftarCE.ValueNumber.Value) { position = currCE.position });
                                    cceIn.Remove(currCE);
                                    cceIn.Remove(preeCE);
                                    cceIn.Remove(aftarCE);
                                    countCalc++;
                                    break;

                                }
                            }
                        }

                        //плю и минус
                        else if (rang == 1 && (currCE.TypeMath == CalcElement.TypeMathEn.Plus || currCE.TypeMath == CalcElement.TypeMathEn.Minus))
                        {
                            if (currCE.TypeMath == CalcElement.TypeMathEn.Minus)
                            {
                                if (preeCE.Type == CalcElement.TypeEn.ValueNumber && aftarCE.Type == CalcElement.TypeEn.ValueNumber)
                                {

                                    cceIn.Insert(i, new CalcElement(preeCE.ValueNumber.Value - aftarCE.ValueNumber.Value) { position = currCE.position });
                                    cceIn.Remove(currCE);
                                    cceIn.Remove(preeCE);
                                    cceIn.Remove(aftarCE);

                                    countCalc++;
                                    break;
                                }
                            }
                            else if (currCE.TypeMath == CalcElement.TypeMathEn.Plus)
                            {
                                if (preeCE.Type == CalcElement.TypeEn.ValueNumber && aftarCE.Type == CalcElement.TypeEn.ValueNumber)
                                {

                                    cceIn.Insert(i, new CalcElement(preeCE.ValueNumber.Value + aftarCE.ValueNumber.Value) { position = currCE.position });
                                    cceIn.Remove(currCE);
                                    cceIn.Remove(preeCE);
                                    cceIn.Remove(aftarCE);
                                    countCalc++;
                                    break;
                                }
                            }
                        }
                    }

                    //логические действия
                    if (rang == 2 && currCE.Type == CalcElement.TypeEn.Logic)
                    {
                        //больше
                        if (currCE.TypeLogic == CalcElement.TypeLogicEn.Bolshe)
                        {
                            //
                            if (preeCE.Type == CalcElement.TypeEn.ValueNumber && aftarCE.Type == CalcElement.TypeEn.ValueNumber)
                            {

                                cceIn.Insert(i, (preeCE.ValueNumber.Value > aftarCE.ValueNumber.Value) ? (new CalcElement(true) { position = currCE.position }) : (new CalcElement(false) { position = currCE.position }));
                                cceIn.Remove(currCE);
                                cceIn.Remove(aftarCE);
                                cceIn.Remove(preeCE);
                                countCalc++;
                                break;
                            }
                        }
                        //меньше
                        if (currCE.TypeLogic == CalcElement.TypeLogicEn.Menshe)
                        {
                            if (preeCE.Type == CalcElement.TypeEn.ValueNumber && aftarCE.Type == CalcElement.TypeEn.ValueNumber)
                            {

                                cceIn.Insert(i, (preeCE.ValueNumber.Value < aftarCE.ValueNumber.Value) ? (new CalcElement(true) { position = currCE.position }) : (new CalcElement(false) { position = currCE.position }));
                                cceIn.Remove(currCE);
                                cceIn.Remove(aftarCE);
                                cceIn.Remove(preeCE);
                                countCalc++;
                                break;
                            }
                        }
                        //больше равно
                        if (currCE.TypeLogic == CalcElement.TypeLogicEn.BolsheRavno)
                        {
                            if (preeCE.Type == CalcElement.TypeEn.ValueNumber && aftarCE.Type == CalcElement.TypeEn.ValueNumber)
                            {

                                cceIn.Insert(i, (preeCE.ValueNumber.Value >= aftarCE.ValueNumber.Value) ? (new CalcElement(true) { position = currCE.position }) : (new CalcElement(false) { position = currCE.position }));
                                cceIn.Remove(currCE);
                                cceIn.Remove(aftarCE);
                                cceIn.Remove(preeCE);
                                countCalc++;
                                break;
                            }
                        }
                        //Меньше равно
                        if (currCE.TypeLogic == CalcElement.TypeLogicEn.MensheRavno)
                        {
                            if (preeCE.Type == CalcElement.TypeEn.ValueNumber && aftarCE.Type == CalcElement.TypeEn.ValueNumber)
                            {

                                cceIn.Insert(i, (preeCE.ValueNumber.Value <= aftarCE.ValueNumber.Value) ? (new CalcElement(true) { position = currCE.position }) : (new CalcElement(false) { position = currCE.position }));
                                cceIn.Remove(currCE);
                                cceIn.Remove(aftarCE);
                                cceIn.Remove(preeCE);

                                countCalc++;
                                break;
                            }
                        }
                        //равно
                        if (currCE.TypeLogic == CalcElement.TypeLogicEn.Ravno)
                        {
                            //перед равно нет то или инче 
                            if (!(i > 1 && (cceIn[i - 2].Type == CalcElement.TypeEn.Logic && (cceIn[i - 2].TypeLogic == CalcElement.TypeLogicEn.To || cceIn[i - 2].TypeLogic == CalcElement.TypeLogicEn.Inachi))))
                            {

                                if (preeCE.Type == CalcElement.TypeEn.ValueNumber && aftarCE.Type == CalcElement.TypeEn.ValueNumber)
                                {
                                    cceIn.Insert(i, (preeCE.ValueNumber.Value == aftarCE.ValueNumber.Value) ? (new CalcElement(true) { position = currCE.position }) : (new CalcElement(false) { position = currCE.position }));
                                    cceIn.Remove(currCE);
                                    cceIn.Remove(aftarCE);
                                    cceIn.Remove(preeCE);
                                    countCalc++;
                                    break;
                                }
                                if (preeCE.Type == CalcElement.TypeEn.ValueText && aftarCE.Type == CalcElement.TypeEn.ValueText)
                                {

                                    cceIn.Insert(i, (preeCE.ValueText == aftarCE.ValueText) ? (new CalcElement(true) { position = currCE.position }) : (new CalcElement(false) { position = currCE.position }));
                                    cceIn.Remove(currCE);
                                    cceIn.Remove(aftarCE);
                                    cceIn.Remove(preeCE);
                                    countCalc++;
                                    break;
                                }
                            }
                        }

                        //И
                        if (currCE.TypeLogic == CalcElement.TypeLogicEn.I)
                        {
                            if (preeCE.Type == CalcElement.TypeEn.LogicValue && aftarCE.Type == CalcElement.TypeEn.LogicValue)
                            {

                                cceIn.Insert(i, (preeCE.LogicValue.Value && aftarCE.LogicValue.Value) ? (new CalcElement(true) { position = currCE.position }) : (new CalcElement(false) { position = currCE.position }));
                                cceIn.Remove(currCE);
                                cceIn.Remove(aftarCE);
                                cceIn.Remove(preeCE);
                                countCalc++;
                                break;
                            }
                        }
                        //Или
                        if (currCE.TypeLogic == CalcElement.TypeLogicEn.Ili)
                        {
                            if (preeCE.Type == CalcElement.TypeEn.LogicValue && aftarCE.Type == CalcElement.TypeEn.LogicValue)
                            {

                                cceIn.Insert(i, (preeCE.LogicValue.Value || aftarCE.LogicValue.Value) ? (new CalcElement(true) { position = currCE.position }) : (new CalcElement(false) { position = currCE.position }));
                                cceIn.Remove(currCE);
                                cceIn.Remove(aftarCE);
                                cceIn.Remove(preeCE);
                                countCalc++;
                                break;
                            }
                        }
                        //из списка
                        if (currCE.TypeLogic == CalcElement.TypeLogicEn.IzSpiska)
                        {
                            if (preeCE.Type == CalcElement.TypeEn.ValueText)
                            {

                                bool izSpiskaTrue = false;
                                foreach (var ceFor in currCE.listCalcElements)
                                {
                                    if (ceFor.ValueText != null)
                                    {
                                        if (ceFor.ValueText.ToString() == preeCE.ValueText.ToString())
                                        {
                                            izSpiskaTrue = true;
                                            break;
                                        }
                                    }
                                }
                                cceIn.Insert(i, new CalcElement(izSpiskaTrue) { position = currCE.position });
                                cceIn.Remove(currCE);

                                cceIn.Remove(preeCE);
                                countCalc++;
                                break;
                            }
                        }
                        //в диапозоне
                        if (currCE.TypeLogic == CalcElement.TypeLogicEn.VDiadazone)
                        {
                            if (preeCE.Type == CalcElement.TypeEn.ValueNumber)
                            {


                                cceIn.Insert(i, new CalcElement(preeCE.ValueNumber.Value >= currCE.listCalcElements[0].ValueNumber.Value && preeCE.ValueNumber.Value <= currCE.listCalcElements[1].ValueNumber.Value) { position = currCE.position });
                                cceIn.Remove(currCE);

                                cceIn.Remove(preeCE);
                                countCalc++;
                                break;
                            }
                        }


                    }
                }
                catch (NullReferenceException ex)
                {
                    MessageBox.Show("Ошибка расчета в операторе " + cceIn[i] + ", с позицией в процедуре " + cceIn[i].position);
                    throw new NullReferenceException();
                }
            }
            return countCalc;
        }


        /// <summary>
        /// разбор строки в коллекцию
        /// </summary>
        /// <param name="stringIn"></param>
        /// <param name="listParams"></param>
        /// <returns></returns>
        public int  ParsingSting(string stringIn) //, List<CalcElement> listParams
        {
            stringIn = UtilsCalc.PastSpace(stringIn);
            int posichin = 0;// позиция элемента в строке
            int countZamen;
            int posishenError = ParsingStringFirst(stringIn, out countZamen, ref posichin);
            if (posishenError > -1)
            {
                return posishenError;//содержится позиция ошибочного элемента со знаком 
            }
            //перебор всех вложенных неизвестных и попытка их разобрать
            int countZamenCh;
            int countZamenSumm = countZamen;
            int iStop = 500;
            while (countZamenSumm > 0 && iStop > 0)//пока есть замены
            {
                countZamenSumm = 0;
                iStop--;
                CollectionCalcElements<T> cceIn = this;
                countZamenCh = ParsingStringRecurs(ref countZamenSumm, cceIn, null, ref posichin);
                if (countZamenCh < 0)
                {
                    return countZamenCh*-1;//содержится позиция ошибочного элемента со знаком -
                }
            }
            //  var a = this;
            return -1;

        }


        /// <summary>
        /// рекурссивная функция (с двумя списками, какую то криворукую хрень сделал)
        /// </summary>
        /// <param name="countZamenSumm"></param>
        /// <param name="cceInT"></param>
        /// <param name="cceInCalcElement"></param>
        /// <returns></returns>
        private static int ParsingStringRecurs(ref int countZamenSumm, CollectionCalcElements<T> cceInT, CollectionCalcElements<CalcElement> cceInCalcElement, ref int posichin)
        {

            List<CalcElement> collectCalcEllIn;
            int countZamenCh = 0;
            if (cceInT == null)
            {
                collectCalcEllIn = cceInCalcElement;
            }
            else
            {
                collectCalcEllIn = cceInT;
            }

            foreach (var calcElIn in collectCalcEllIn) // перебор элементов
            {
                if (calcElIn.listCalcElements.Count > 0)//если есть список вложенных элементов
                {
                    int retInt = ParsingStringRecurs(ref countZamenSumm, null, calcElIn.listCalcElements, ref posichin);
                    if ( retInt<0)
                    {
                        return retInt;
                    }
                    for (int i = 0; i < calcElIn.listCalcElements.Count; i++)//переберем его
                    {

                        if (calcElIn.listCalcElements[i].Type == CalcElement.TypeEn.UnknownQuantity) //если он "Неизвестный"
                        {
                            CollectionCalcElements<CalcElement> collCh = new CollectionCalcElements<CalcElement>();
                            if (calcElIn.listCalcElements[i].Name != null) //переведем его имя в новую коллекцию
                            {

                                collCh.ParsingStringFirst(calcElIn.listCalcElements[i].Name, out countZamenCh, ref posichin);
                                //Распарсенный неизвестный элемент из дочерней коллекции
                                countZamenSumm += countZamenCh;
                                if (collCh.Count == 1)
                                {
                                    calcElIn.listCalcElements[i] = collCh[0]; //добавляем список элементов
                                }
                                else if (collCh.Count > 1 && countZamenCh > 1) //добавляем элемент со списком элементов
                                {
                                    calcElIn.listCalcElements[i] = new CalcElement(null, CalcElement.TypeEn.UnknownQuantity) { listCalcElements = collCh };//добавим эту коллекцию в дочерние элементы
                                }

                                else
                                {
                                    MessageBox.Show("Ошибка в операторе " + calcElIn + ", (позиция в тексте процедуры - " + calcElIn.position+") ", "Ошибка");
                                    return calcElIn.position*-1;
                                }
                            }

                        }
                    }
                }
            }

            return countZamenCh;
        }

        /// <summary>
        /// /// разбор строки в коллекцию. вместо выражений под функциями в скобках вставляет как строку с типом неизвестное
        /// </summary>
        /// <param name="stringIn"></param>
        /// <param name="countZamen"></param>
        /// <returns></returns>
        private int ParsingStringFirst(string stringIn, out int countZamen, ref int posishen)
        {
            countZamen = 0;

            if (stringIn == null)
            {
                return 0;

            }
            string stIn = stringIn;

            int returnPosishen = -1; //в случае ошибки необходимо вернуть место ошибки

            posishen += CountSpaceInStart(stIn);
            stIn = stIn.Trim();//входная строка (обрезается обработанная часть)
            List<CalcElement> listElemens = UtilsCalc.GetOperations();
            //listElemens.AddRange(listParams);


            bool izSpiska = false;//этосписок
            bool vDiapazone = false;//это диапозон
            bool fUno = false;//это функция над одним операндом
            bool fDual = false;//это функция над двумя операндами
            int iCoun = 0; //чтоб остановить while если зависнет
            while (stIn.Length > 0) //цикл над строкой, пока строка не кончится
            {

                if (iCoun > stringIn.Length)//остановка цикла while при ошибке
                {
                    MessageBox.Show("Ошибка в процедуре расчета трудоемкости", "Ошибка");
                    returnPosishen = posishen;
                    break;
                }
                iCoun++;

                bool addElement = false;//элемент - операнд, и был добавлен


                foreach (var ce in listElemens) //перебор всех операндов
                {
                    posishen += CountSpaceInStart(stIn);
                    stIn = stIn.Trim();//отбросили пробелы в начале и конце
                    if (stIn.IndexOf(ce.ToString()) == 0) //если строка начинается с операнда
                    {
                        //если это знак меньше, и после него не идет пробел, значит это открывающая ковычка
                        if (ce.Type == CalcElement.TypeEn.Logic && ce.TypeLogic == CalcElement.TypeLogicEn.Menshe && (stIn.IndexOf(ce.ToString() + " ") != 0))
                        {
                            continue;
                        }
                        if (fUno || fDual)
                        {
                            //this[this.Count - 1].listCalcElements.Add(ce); //параметр
                            continue;
                        }
                        else
                        {
                            //добавляем в список этот операнд
                            if (ce.Type == CalcElement.TypeEn.Logic)
                            {
                                this.Add(new CalcElement(ce.TypeLogic.Value) { position = posishen });
                            }
                            else if (ce.Type == CalcElement.TypeEn.Math)
                            {
                                this.Add(new CalcElement(ce.TypeMath.Value) { position = posishen });
                            }
                            else if (ce.Type == CalcElement.TypeEn.Brackets)
                            {
                                this.Add(new CalcElement(ce.TypeBrackets.Value) { position = posishen });
                            }
                        }
                        countZamen++;
                        posishen += ce.ToString().Length;
                        stIn = stIn.Remove(0, ce.ToString().Length); //удаление из строки его
                        addElement = true;
                    }
                    //если это из списка или в диапазоне или функция 
                    if (addElement)/* (ce.Type == CalcElement.TypeEn.Logic && (ce.TypeLogic == CalcElement.TypeLogicEn.VDiadazone || ce.TypeLogic == CalcElement.TypeLogicEn.IzSpiska) && (stIn.IndexOf("ВДиапазоне" + "[") == 0 || stIn.IndexOf("ИзСписка" + "[") == 0))*/
                    {
                        //возможные случаи
                        if ((ce.TypeLogic == CalcElement.TypeLogicEn.VDiadazone)/*&& (stIn.IndexOf("ВДиапазоне" + "[") == 0)*/)
                        {
                            posishen += CountSpaceInStart(stIn);
                            stIn = stIn.Trim();//отбросили пробелы в начале и конце
                            if (stIn.IndexOf("[") != 0)
                            {
                                MessageBox.Show("Ошибка в операторе  " + ce.ToString() + ": нет символа [ (позиция в тексте процедуры - " + this[this.Count - 1].position + ")", "Ошибка");
                                return this[this.Count - 1].position;
                             //   break;
                            }
                            posishen += 1;
                            stIn = stIn.Remove(0, 1);//удаление первой скобки

                            vDiapazone = true;
                        }
                        else if ((ce.TypeLogic == CalcElement.TypeLogicEn.IzSpiska)/*&&(stIn.IndexOf("ИзСписка" + "[") == 0)*/)
                        {
                            posishen += CountSpaceInStart(stIn);
                            stIn = stIn.Trim();//отбросили пробелы в начале и конце
                            if (stIn.IndexOf("[") != 0)
                            {
                                MessageBox.Show("Ошибка в операторе  " + ce.ToString() + ": нет символа [ (позиция в тексте процедуры - " + this[this.Count - 1].position + ")", "Ошибка");
                                return this[this.Count - 1].position;
                            //    break;
                            }
                            posishen += 1;
                            stIn = stIn.Remove(0, 1);//удаление первой скобки

                            izSpiska = true;
                        }

                        else if ((ce.TypeMath == CalcElement.TypeMathEn.Ln)/*&&(stIn.IndexOf("Ln" + "(") == 0)*/)
                        {
                            fUno = true;
                            posishen += CountSpaceInStart(stIn);
                            stIn = stIn.Trim();//отбросили пробелы в начале и конце
                            if (stIn.IndexOf("(") != 0)
                            {
                                MessageBox.Show("Ошибка в операторе  " + ce.ToString() + ": нет символа '(' (позиция в тексте процедуры - " + this[this.Count - 1].position + ")", "Ошибка");
                                return this[this.Count - 1].position;
                              //  break;
                            }
                            posishen += 1;
                            stIn = stIn.Remove(0, 1);//удаление первой скобки

                        }
                        else if ((ce.TypeMath == CalcElement.TypeMathEn.Pow)/*&&(stIn.IndexOf("Pow" + "(") == 0)*/)
                        {
                            fDual = true;
                            posishen += CountSpaceInStart(stIn);
                            stIn = stIn.Trim();//отбросили пробелы в начале и конце
                            if (stIn.IndexOf("(") != 0)
                            {
                                MessageBox.Show("Ошибка в операторе  " + ce.ToString() + ": нет символа '(' (позиция в тексте процедуры - " + this[this.Count - 1].position + ")", "Ошибка");
                                return this[this.Count - 1].position;
                             //   break;
                            }
                            posishen += 1;
                            stIn = stIn.Remove(0, 1);//удаление первой скобки

                        }
                        break;
                    }

                }


                bool endEll = false; //последний элемент в списке или диапазоне

                if (!addElement)//это не операция
                {
                    int indexSeparat = stIn.IndexOf(" "); //разделитель пробел
                    if (indexSeparat < 0)
                    {
                        indexSeparat = stIn.Length;
                    }
                    if (vDiapazone) //разбор того что в скобках диапазона
                    {
                        int stMinus = stIn.IndexOf("-");
                        int stSkobka = stIn.IndexOf("]");
                        if (stSkobka < 0)
                        {

                            MessageBox.Show("Ошибка в операторе ВДиапазоне: нет символа ] (позиция в тексте процедуры - " + this[this.Count - 1].position + ")", "Ошибка");
                            
                            return this[this.Count - 1].position;
                        }
                        else if (stMinus < 0)
                        {
                            endEll = true;//последний элемент диапазона
                            indexSeparat = stSkobka;
                        }
                        else if (stMinus < 0 || stSkobka < 0) //если нет ни ] ни -
                        {
                            MessageBox.Show("Ошибка в операторе ВДиапазоне: нет символа ] либо - (позиция в тексте процедуры - " + this[this.Count - 1].position + ")", "Ошибка");
                            return this[this.Count - 1].position;
                        }
                        else
                        {
                            indexSeparat = stMinus < stSkobka ? stMinus : stSkobka;
                            endEll = stMinus > stSkobka;
                        }
                    }
                    else if (izSpiska) //разбор списка
                    {
                        int stZpt = stIn.IndexOf(",");
                        int stSkobka = stIn.IndexOf("]");
                        if (stSkobka < 0)
                        {
                            MessageBox.Show("Ошибка в операторе ИзСписка: нет символа ] (позиция в тексте процедуры " + this[this.Count - 1].position+ ")", "Ошибка");
                            
                            return this[this.Count - 1].position;
                        }
                        else if (stZpt < 0)
                        {
                            endEll = true;
                            indexSeparat = stSkobka;
                        }
                        else if (stZpt < 0 || stSkobka < 0)
                        {
                            {
                                MessageBox.Show("Ошибка в операторе ИзСписка(позиция в тексте процедуры " + this[this.Count - 1].position + ")", "Ошибка");
                                return this[this.Count - 1].position;
                            }
                        }
                        else
                        {
                            indexSeparat = stZpt < stSkobka ? stZpt : stSkobka;
                            endEll = stZpt > stSkobka;
                        }
                    }
                    else if (fDual)
                    {
                        int stZpt = stIn.IndexOf(",");
                        int stSkobka = SearchCloseBreyk(stIn);
                        if (stSkobka < 0)
                        {
                            MessageBox.Show("Ошибка в операторе " + this[this.Count - 1] + ": нет символа ')'(позиция в тексте процедуры - " + this[this.Count - 1].position + ")", "Ошибка");
                            return this[this.Count - 1].position;
                        }
                        else if (stZpt < 0)
                        {
                            endEll = true;
                            indexSeparat = stSkobka;
                        }
                        else if (stZpt < 0 || stSkobka < 0)
                        {
                            MessageBox.Show("Ошибка в операторе " + this[this.Count - 1] + "(позиция в тексте процедуры - " + this[this.Count - 1].position + ")", "Ошибка");

                            return this[this.Count - 1].position;
                        }
                        else
                        {
                            indexSeparat = stZpt < stSkobka ? stZpt : stSkobka;
                            endEll = stZpt > stSkobka;
                        }
                    }
                    else if (fUno)
                    {
                        int stSkobka = SearchCloseBreyk(stIn);
                        endEll = true;
                        indexSeparat = stSkobka;
                        if (stSkobka < 0)
                        {
                            MessageBox.Show("Ошибка в операторе " + this[this.Count - 1] + "(позиция в тексте процедуры - " + this[this.Count - 1].position + ")", "Ошибка");
                            return this[this.Count - 1].position;
                        }
                    }

                    string stEl = ((indexSeparat < 0) || stIn.Length == indexSeparat) ? stIn.Trim() : stIn.Remove(indexSeparat).Trim();//если отрезать нечего берем всю строку
                    //кусок строки до разделителя, либо остаток строки

                    double? dValue = UtilsCalc.UniversalParsing(stEl.Trim()); //если в куске строки число
                    if (stIn.IndexOf("\"") == 0) //это значение параметра 
                    {
                        int stSeparatKovichk = stIn.IndexOf("\"", 1); //закрыв ковычка
                        if (stSeparatKovichk < 0)
                        {
                            MessageBox.Show("Ошибка в операторе " + stEl + ": нет закрывающей кавычки (позиция в тексте процедуры - " + posishen + ")", "Ошибка");
                            return posishen;
                        }

                        if (stSeparatKovichk > 0)
                        {
                            stEl = stIn.Remove(stSeparatKovichk).Trim(); //текст в ковычках
                        }
                        if (izSpiska /*|| vDiapazone||fDual||fUno*/)
                        {
                            indexSeparat += 1; //убрать знак разделения(запятую или ])
                            this[this.Count - 1].listCalcElements.Add(new CalcElement(stEl.Replace("\"", string.Empty))); //значение
                            countZamen++;
                            //if (((vDiapazone ||fDual) && this[this.Count - 1].listCalcElements.Count > 2)|| (fUno && this[this.Count - 1].listCalcElements.Count > 1))
                            //{
                            //    MessageBox.Show("Ошибка разбора строки. Ошибка разбора функции" + this[this.Count - 1], "Ошибка");
                            //    return false;
                            //}
                        }
                        else
                        {
                            this.Add(new CalcElement(stEl.Replace("\"", string.Empty)) { position = posishen }); //значение
                            countZamen++;
                            indexSeparat = stSeparatKovichk + 1; //по это место будет отрезана строка
                        }
                    }

                    else if (stIn.IndexOf("<") == 0) //это имя параметра
                    {
                        int indexSkobka = stIn.IndexOf(">");
                        if (indexSkobka <= 0)
                        {
                            MessageBox.Show("Ошибка в операторе " + stEl + ": нет символа ')' (позиция в тексте процедуры - " + posishen + ")", "Ошибка");
                            return posishen;
                        }
                        if (fDual || fUno)
                        {
                            indexSeparat += 1; //убрать знак разделения

                            stEl = stIn.Remove(indexSkobka).Trim();
                            this[this.Count - 1].listCalcElements.Add(new CalcElement(stEl.Replace("<", string.Empty).Replace(">", string.Empty), (double?)null)); //параметр
                            if (((vDiapazone || fDual) && this[this.Count - 1].listCalcElements.Count > 2) || (fUno && this[this.Count - 1].listCalcElements.Count > 1))
                            {
                                MessageBox.Show("Ошибка в операторе " + this[this.Count - 1] + "(позиция в тексте процедуры - " + this[this.Count - 1].position + ")", "Ошибка");
                                return this[this.Count - 1].position;
                            }
                        }
                        else
                        {
                            if (indexSkobka > 0)
                            {
                                stEl = stIn.Remove(indexSkobka).Trim();
                                indexSeparat = indexSkobka + 1;
                            }
                            this.Add(new CalcElement(stEl.Replace("<", string.Empty).Replace(">", string.Empty), (double?)null) { position = posishen });
                            countZamen++;
                        }
                    }


                    else if (dValue != null)
                    {
                        if (izSpiska || vDiapazone || fDual || fUno)
                        {
                            indexSeparat += 1; //убрать знак разделения
                            this[this.Count - 1].listCalcElements.Add(new CalcElement(dValue.Value)); //параметр
                            if (((vDiapazone || fDual) && this[this.Count - 1].listCalcElements.Count > 2) || (fUno && this[this.Count - 1].listCalcElements.Count > 1))
                            {
                                MessageBox.Show("Ошибка в операторе " + this[this.Count - 1] + "(позиция в тексте процедуры - " + this[this.Count - 1].position + ")", "Ошибка");
                                return this[this.Count - 1].position;
                            }
                        }
                        else
                        {
                            this.Add(new CalcElement(dValue.Value) { position = posishen }); //значение цифровое
                            countZamen++;
                        }
                    }
                    else
                    {
                        if (izSpiska || vDiapazone || fDual || fUno)
                        {

                            this[this.Count - 1].listCalcElements.Add(new CalcElement(stEl.Trim(), CalcElement.TypeEn.UnknownQuantity)); //параметр
                            //countZamen++;
                            if (((vDiapazone || fDual) && this[this.Count - 1].listCalcElements.Count > 2) || (fUno && this[this.Count - 1].listCalcElements.Count > 1))
                            {
                                MessageBox.Show("Ошибка в операторе " + this[this.Count - 1] + "(позиция в тексте процедуры - " + this[this.Count - 1].position + ")", "Ошибка");
                                return this[this.Count - 1].position;
                            }
                            indexSeparat += 1; //убрать знак разделения
                        }
                        else
                        {
                            this.Add(new CalcElement(stEl, CalcElement.TypeEn.UnknownQuantity) { position = posishen });//неизвестное
                            //countZamen++;
                        }
                    }
                    posishen += indexSeparat;
                    stIn = stIn.Remove(0, indexSeparat);
                    if (endEll)
                    {
                        izSpiska = false;
                        vDiapazone = false;
                        fUno = false;
                        fDual = false;
                    }




                }
                posishen += CountSpaceInStart(stIn);
                stIn = stIn.Trim();
            }
            return -1;
        }
        /// <summary>
        /// возращает позицию буквы символа не пробела
        /// </summary>
        /// <param name="stIn"></param>
        /// <returns></returns>
        private static int CountSpaceInStart(string stIn)
        {
            var iForeach = 0;
            foreach (var ch in stIn)
            {
                if (ch != ' ' && ch != '\n')
                {
                    return iForeach;
                }
                else
                {
                    iForeach++;
                }
            }
            return iForeach;
        }

        /// <summary>
        /// индекс nuzhnoeVhozhdenie-его вхождения symbol в строку stIn
        /// </summary>
        /// <param name="stIn"></param>
        /// <param name="symbol"></param>
        /// <param name="nuzhnoeVhozhdenie"></param>
        /// <returns></returns>
        public static int IndexSymbol(string stIn, string symbol, int nuzhnoeVhozhdenie)
        {
            int index = 0;
            index = stIn.IndexOf(symbol);//первое вхождение
            for (int i = 0; i < nuzhnoeVhozhdenie - 1; i++)
            {
                index = stIn.IndexOf(symbol, index + 1);
            }
            return index;
        }

        /// <summary>
        /// Количество вхождений символа в строку
        /// </summary>
        /// <param name="stIn"></param>
        /// <param name="symbol"></param>
        /// <param name="nuzhnoeVhozhdenie"></param>
        /// <returns></returns>
        public static int CountSymbolInString(string stIn, string symbol)
        {
            int iCount = 0;
            int index = 0;
            index = stIn.IndexOf(symbol);
            while (index >= 0)
            {
                iCount++;
                index = stIn.IndexOf(symbol, index + 1);
            }

            return iCount;
        }


        /// <summary>
        /// Поиск не парной закрывающейся скобки
        /// </summary>
        /// <param name="stIn"></param>
        /// <param name="symbol"></param>
        /// <param name="nuzhnoeVhozhdenie"></param>
        /// <returns></returns>
        public static int SearchCloseBreyk(string stIn)
        {
            int iCount = 0;
            int index = 0;
            index = stIn.IndexOf(")");
            string stCrop;//кусок до )
            int countBrClose = 1;//количество скобок

            int istop = 0;

            while (index >= 0 && istop < 500)
            {
                istop++;
                index = IndexSymbol(stIn, ")", countBrClose);
                if (index > 0)
                {
                    stCrop = stIn.Remove(index).Trim();
                }
                else
                {
                    return -1;
                }
                if (CountSymbolInString(stCrop, "(") == countBrClose - 1) //если количество открывающихся скобок на этом куске меньше на одну количеству скобок откр
                {
                    return index;
                }
                else
                {
                    countBrClose++;
                }
            }

            return iCount;
        }
    }
}
