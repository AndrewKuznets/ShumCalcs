using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows;
using System.IO;
using System.Xml.Schema;
using System.Xml.Linq;

namespace ShumCalcs
{/// <summary>
/// Запись и чтение файлов XML
/// </summary>
    class XmlFile
    {
        private List<DataRecord> enterDatas = new List<DataRecord>();
        private List<DataRecord> calcDatas = new List<DataRecord>();
        public List<DataRecord> EnterDatas
        {
            get { return enterDatas; }
            set { enterDatas = value; }
        }
        public List<DataRecord> CalcDatas
        {
            get { return calcDatas; }
            set { calcDatas = value; }
        }

        


        /// <summary>
        /// одна запись ключ значение
        /// </summary>
        public class DataRecord
        {
            private string key;
            
            public string Key
            {
                get { return key; }
                set { key = value; }
            }
            public override string ToString()
            {
                return key;
            }
        }
        public class DataRecordString : DataRecord
        {
            string data;
            public string Data
            {
                get { return data; }
                set { data = value; }
            }
           public DataRecordString(string key, string data)
            {
                this.Key = key;
                this.data = data;
             }
            public DataRecordString()
            {
                this.Key = string.Empty;
                this.data = string.Empty;
            }
            public override string ToString()
            {
                return base.ToString()+ " " + data;
            }
        }
        public class DataRecordValue : DataRecord
        {
            double data;
            public double Data
            {
                get { return data; }
                set { data = value; }
            }
            public DataRecordValue(string key, Double data)
            {
                this.Key = key;
                this.data = data;
            }

            public DataRecordValue()
            {
                this.Key = string.Empty;
                this.data = 0;
            }
        }


        /// <summary>
        /// считать данные из XML файла
        /// </summary>
        /// <param name="nameMap"></param>
        /// <param name="pachXML"></param>
        /// <returns></returns>
         public bool ReadDatasFromXml(string pachXML)
        {
            XmlDocument myxmlDoc = new XmlDocument();

            try
            {
                myxmlDoc.Load(pachXML);
            }
            catch (System.IO.FileNotFoundException ex)
            {
              
                MessageBox.Show("Файл не найден");
                return false;
            }
            catch (System.Xml.XmlException ex)
            {
              
                MessageBox.Show("Файл поврежден");
                return false;
            }

            //ValidXML(myxmlDoc);
            XmlNodeList datas; 
          
            
            datas = myxmlDoc.GetElementsByTagName("EnterDatas");
             GetFromXmlNodeListDatas(datas, enterDatas);
            datas = myxmlDoc.GetElementsByTagName("CalcDatas");
            GetFromXmlNodeListDatas(datas, calcDatas);

            return true;
        }
        /// <summary>
        /// получить список List<DataRecord> из XmlNodeList
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="enterDatas"></param>
        private static void GetFromXmlNodeListDatas(XmlNodeList datas, List<DataRecord> enterDatas)
        {
            XmlNodeList dataNodes;
            foreach (XmlNode data in datas)
            {
                dataNodes = data.ChildNodes;
                string name = null;
                string dataStr = null;
                double dataValue;
                foreach (XmlNode dataNode in dataNodes)
                {
                    //Data dataStrinr = new Data();



                    
                        name = dataNode.Name;
                   
                   
                        dataStr = dataNode.InnerText;
                        //if (Double.TryParse(dataStr, out dataValue))
                        //{
                        //    enterDatas.Add(new DataRecordValue { Key = name, Data = dataValue });
                        //}
                        //else
                        //{
                            enterDatas.Add(new DataRecordString { Key = name, Data = dataStr });
                        //}
                    
                    
                }
            }

           
        }
        /// <summary>
        /// Записать XML файл
        /// </summary>
        /// <param name="pachXML"></param>
        /// <returns></returns>
        public bool SaveXml( string pachXML)
        {



            XDocument myxmlDoc = new XDocument();

            XElement xElementDatas = new XElement("datas");
            XElement xElementEnterDatas = new XElement("EnterDatas");
            XElement xElementCalcDatas = new XElement("CalcDatas");
            if ((AddRecordListToXML(xElementEnterDatas, this.enterDatas) && AddRecordListToXML(xElementCalcDatas, this.calcDatas)) == false)
            {
                return false;
            }
            // добавляем корневой элемент в документ

            xElementDatas.Add(xElementEnterDatas);
            xElementDatas.Add(xElementCalcDatas);
            myxmlDoc.Add(xElementDatas);

            myxmlDoc.Save(pachXML);
            return true;
        }
        /// <summary>
        /// Из списка записей допавить в XElement xElementEnterDatas
        /// </summary>
        /// <param name="xElementEnterDatas"></param>
        /// <param name="recordList"></param>
        private static bool AddRecordListToXML(XElement xElementEnterDatas, List<DataRecord> recordList)
        {
            foreach (var dRec in recordList)
            {
                if (dRec.Key== "")
                {
                    MessageBox.Show("codeParam не может быть пустым");
                    return false;
                }
                
                XElement dataRec = new XElement(dRec.Key.Trim(), (dRec is DataRecordString) ? (dRec as DataRecordString).Data : (dRec as DataRecordValue).Data.ToString());
                xElementEnterDatas.Add(dataRec);
            }
            return true;
        }
        public static string OpenXMLFile()
        {
            string pachXMLFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            openFileDialog1.InitialDirectory = pachXMLFolder + @"\Экспертиза-МРТ";
            System.IO.Directory.CreateDirectory(pachXMLFolder);
            openFileDialog1.Filter = "XML files(*.xml)|*.xml|All files(*.*)|*.*";
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                return null;
            }

            var FileName = openFileDialog1.FileName;
            return FileName;
        }

        //public void test()
        //{
        //    enterDatas.Add(new DataRecordString("один", "два"));
        //    enterDatas.Add(new DataRecordValue("один", 2));
        //    calcDatas.Add(new DataRecordString("один", "два"));
        //    calcDatas.Add(new DataRecordValue("один", 2));
        //    SaveXml("test.xml");
        //    XmlFile xmlFile = new XmlFile();
        //    xmlFile.ReadDatasFromXml("test.xml");
        //   // MessageBox.Show(xmlFile.calcDatas.FirstOrDefault().Key);
        //}

        //static private void ValidXML(XmlDocument document)
        //{
        //    // XPathNavigator navigator = document.CreateNavigator();
        //    // 
        //    //System.IO.DirectoryInfo pachXSD = new System.IO.DirectoryInfo(".");

        //    string pachXSD = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        //    document.Schemas.Add(null, @"D:\project\Cutting3\Cutting3\bin\Debug\setingsShema.xsd");


        //    ValidationEventHandler validation = new ValidationEventHandler(SchemaValidationHandl);

        //    try
        //    {
        //        document.Validate(validation);
        //    }
        //    catch
        //    {
        //        MessageBox.Show("ошибка проверки формы на соответствие схемы");
        //    }
        //}


        //static private void SchemaValidationHandl(object sender, ValidationEventArgs e)
        //{
        //    switch (e.Severity)
        //    {
        //        case XmlSeverityType.Error:

        //            MessageBox.Show("Schema Validation Error: " + e.Message);
        //            break;
        //        case XmlSeverityType.Warning:

        //            MessageBox.Show("Schema Validation Warning: " + e.Message);
        //            break;
        //    }
        //}
    }
}
