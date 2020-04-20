using House.IService.Common.XML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace House.Service.Common.XML
{
    public class XMLHelperSingle : IXMLHelperSingle
    {


        public string XmlSerialize<T>(T obj)
        {
            using (StringWriter sw = new StringWriter())
            {
                Type t = obj.GetType();
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(sw, obj);
                sw.Close();
                return sw.ToString();
            }
        }

        public T DESerializer<T>(string strXML) where T : class
        {
            try
            {
                using (StringReader sr = new StringReader(strXML))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    return serializer.Deserialize(sr) as T;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string DicToXmlStr(Dictionary<string, object> obj)
        {
            XElement el = new XElement("xml",obj.Select(kv => new XElement(kv.Key, kv.Value)));
            return el.ToString();
        }

        public Dictionary<string, object> XmlStrToDic(string xml) {
            XElement rootElement = XElement.Parse(xml);
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (var el in rootElement.Elements())
            {
                dict.Add(el.Name.LocalName, el.Value);
            }
            return dict;
        }
    }
}
