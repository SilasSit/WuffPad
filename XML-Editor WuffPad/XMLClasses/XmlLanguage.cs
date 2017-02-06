using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XML_Editor_WuffPad.XMLClasses
{
    public class XmlLanguage
    {
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("base")]
        public string Base { get; set; }
        [XmlElement("variant")]
        public string Variant { get; set; }
        [XmlElement("owner")]
        public string Owner { get; set; }
    }
}
