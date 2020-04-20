using System;
using System.Collections.Generic;
using System.Text;

namespace House.IService.Common.XML
{
    public interface IXMLHelperSingle
    {
        T DESerializer<T>(string strXML) where T : class;
        string XmlSerialize<T>(T obj);

        string DicToXmlStr(Dictionary<string, object> obj);
        Dictionary<string, object> XmlStrToDic(string xml);
    }
}
