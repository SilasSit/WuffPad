using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XML_Editor_WuffPad.XMLClasses
{
    [XmlRoot("strings")]
    public class XmlStrings
    {
        public XmlStrings()
        {
            Strings = new ObservableCollection<XmlString>();
            Language = new XmlLanguage();
        }
        [XmlElement("language")]
        public XmlLanguage Language { get; set; }
        [XmlElement("string")]
        public ObservableCollection<XmlString> Strings { get; set; }
    }
}
