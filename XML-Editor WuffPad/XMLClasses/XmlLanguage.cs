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
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("base")]
        public string Base { get; set; }
        [XmlAttribute("variant")]
        public string Variant { get; set; }
        [XmlAttribute("owner")]
        public string Owner { get; set; }
    }
}
