using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShumCalcs
{
    public  class CalcElement
    {
        public enum TypeEn : byte { Logic, Param, ValueText, ValueNumber, LogicValue, Math, UnknownQuantity, Brackets };
        public enum TypeLogicEn : byte { Esli, To, Inachi, Bolshe, Menshe, BolsheRavno, MensheRavno, Ravno, IzSpiska, I , Ili, VDiadazone };
        public enum TypeMathEn : byte { Plus, Minus, Umnozhit, Delit, Ln, Pow, Stepen };
        public enum TypeParamEn : byte { Number, Text };
        public enum TypeBracketsEn : byte { Open, Close };

        private string name = null;
        private string valueText = null;
        private bool? logicValue = null;
        private double? valueNumber = null;
        private TypeEn type ;
        private TypeLogicEn? typeLogic = null ;
        private TypeMathEn? typeMath = null;
        private TypeBracketsEn? typeBrackets = null;
        public TypeParamEn? typeParam = null;

        public int position;

        public List<string> listValue = new List<string>();
        public CollectionCalcElements<CalcElement> listCalcElements = new CollectionCalcElements<CalcElement>();
        public override string ToString()
        {
            if (this.type == TypeEn.ValueNumber)
            {
                return valueNumber.ToString().Replace(",",".");
            }
            else if (this.type == TypeEn.ValueText)
            {
                return "\"" +valueText+ "\"";
            }
            else if (this.type == TypeEn.LogicValue)
            {
                return logicValue.Value.ToString();
            }
            else if (this.type == TypeEn.UnknownQuantity)
            {
                if (name == null)
                {
                    if (listCalcElements!=null)
                    {
                        string returnStr = " ";
                        foreach (var ce in listCalcElements)
                        {
                            returnStr += (ce + " ");
                        }
                        return returnStr;
                    }
                }
                return name;
            }
            else if (this.type == TypeEn.Param)
            {
                return "<"+name+">";
            }
            else if (this.type == TypeEn.Brackets)
            {
                if (this.typeBrackets == TypeBracketsEn.Open)
                {
                    return "(";
                }
                else if (this.typeBrackets == TypeBracketsEn.Close)
                {
                    return ")";
                }
            }
            else if (this.type == TypeEn.Logic)
            {
                if (this.TypeLogic == TypeLogicEn.Bolshe)
                {
                    return ">";
                }
                if (this.TypeLogic == TypeLogicEn.BolsheRavno)
                {
                    return "≥";
                }
                if (this.TypeLogic == TypeLogicEn.Esli)
                {
                    return "Если";
                }
                if (this.TypeLogic == TypeLogicEn.Inachi)
                {
                    return "Иначе";
                }
                if (this.TypeLogic == TypeLogicEn.I)
                {
                    return "И";
                }
                if (this.TypeLogic == TypeLogicEn.Ili)
                {
                    return "Или";
                }
                if (this.TypeLogic == TypeLogicEn.IzSpiska)
                {
                 

                    if ((listCalcElements != null && listCalcElements.Count == 0) || listCalcElements == null)
                    {
                        return "ИзСписка";
                    }
                    else
                    {
                        string returnStr = "ИзСписка[";
                        bool firstEll = true;
                        foreach (var calcEl in listCalcElements)
                        {
                            returnStr += firstEll ? "" : ",";
                            firstEll = false;
                            returnStr +=" "+ calcEl  ;
                        }
                        returnStr += " ]";
                        return returnStr;
                    }


                }
                if (this.TypeLogic == TypeLogicEn.VDiadazone)
                {
                    if ((listCalcElements !=null && listCalcElements.Count < 2) || listCalcElements == null)
                    {
                        return "ВДиапазоне";
                    }
                    else
                    {
                        return "ВДиапазоне[ " + listCalcElements[0]+ " - "+ listCalcElements[1] + " ]";
                    }


                }
                if (this.TypeLogic == TypeLogicEn.Menshe)
                {
                    return "<";
                }
                if (this.TypeLogic == TypeLogicEn.MensheRavno)
                {
                    return "≤"; 
                }

                if (this.TypeLogic == TypeLogicEn.Ravno)
                {
                    return "=";
                }
                if (this.TypeLogic == TypeLogicEn.To)
                {
                    return "То";
                }
            }
            else if (this.type == TypeEn.Math)
            {
                if (this.TypeMath == TypeMathEn.Delit)
                {
                    return "/";
                }
                if (this.TypeMath == TypeMathEn.Ln)
                {
                    if ((listCalcElements != null && listCalcElements.Count < 1) || listCalcElements == null)
                    {
                        return "Ln";
                    }
                    else
                    {
                        return "Ln( " + listCalcElements[0]  + " )";
                    }
                  
                }
                if (this.TypeMath == TypeMathEn.Minus)
                {
                    return "-";
                }
                if (this.TypeMath == TypeMathEn.Plus)
                {
                    return "+";
                }
                if (this.TypeMath == TypeMathEn.Stepen)
                {
                    return "^";
                }
                if (this.TypeMath == TypeMathEn.Pow)
                {
                    if ((listCalcElements != null && listCalcElements.Count < 2) || listCalcElements == null)
                    {
                        return "Pow";
                    }
                    else
                    {
                        return "Pow( " + listCalcElements[0] + ", " + listCalcElements[1] + " )";
                    }
                   
                }
                if (this.TypeMath == TypeMathEn.Umnozhit)
                {
                    return "*";
                }
            }

                return "error";
        }
        public string Name
        {
            get { return name; }
        }
        public string ValueText
        {
            get { return valueText; }
        }
        public double? ValueNumber
        {
            get { return valueNumber; }
        }
        public bool? LogicValue
        {
            get { return logicValue; }
        }
        public TypeEn Type
        {
            get { return type; }
        }
        public TypeLogicEn? TypeLogic
        {
            get { return typeLogic; }
        }
        public TypeMathEn? TypeMath
        {
            get { return typeMath; }
        }
        public TypeBracketsEn? TypeBrackets
        {
            get { return typeBrackets; }
        }


        public CalcElement(string value)
        {
            this.type = TypeEn.ValueText;
            this.valueText = value;
        }

        public CalcElement(bool value)
        {
            this.type = TypeEn.LogicValue;
            this.logicValue = value;
        }
        /// <summary>
        /// ValueNumber
        /// </summary>
        /// <param name="value"></param>
        public CalcElement(double value)
        {
            this.type = TypeEn.ValueNumber;
            this.valueNumber = value;
        }
        public CalcElement(string name, double? value)
        {
            this.type = TypeEn.Param;
            this.name = name;
            this.valueNumber = value;
        }
        public CalcElement(string name, string value)
        {
            this.type = TypeEn.Param;
            this.name = name;
            this.valueText = value;
        }
        public CalcElement(string name, List<string> value)
        {
            this.type = TypeEn.Param;
            this.name = name;
            this.listValue = value;
        }
        public CalcElement(string name, CalcElement.TypeEn type)
        {
            this.type = type;
            this.name = name;
        }
        public CalcElement(string name, CalcElement.TypeEn type, string value)
        {
            this.type = type;
            this.name = name;
            this.valueText = value;
        }
        public CalcElement(string name, CalcElement.TypeEn type, double value)
        {
            this.type = type;
            this.name = name;
            this.valueNumber = value;
        }


        public CalcElement(TypeLogicEn value)
        {
            this.type = TypeEn.Logic;
            this.typeLogic = value;
        }
        public CalcElement(TypeMathEn value)
        {
            this.type = TypeEn.Math;
            this.typeMath = value;
        }
        public CalcElement(TypeBracketsEn value)
        {
            this.type = TypeEn.Brackets;
            this.typeBrackets = value;
        }
    }
}
