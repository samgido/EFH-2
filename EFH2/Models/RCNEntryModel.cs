using System.Xml.Serialization;

namespace EFH2
{
    public class RcnEntryModel
    {
        [XmlElement("Weight")]
        public int Weight { get; set; }

        [XmlElement("Area")]
        public double Area { get; set; }
    }
}
