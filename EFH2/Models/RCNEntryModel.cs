using System.Xml.Serialization;

namespace EFH2
{
	public class RcnEntryModel
    {
        [XmlElement("Weight")]
        public int Weight { get; set; } = 0;

        [XmlElement("Area")]
        public double Area = double.NaN;

        [XmlIgnore]
        public double WeightedArea
        {
            get
            {
                if (!(double.IsNaN(Area))) return Area * Weight;
                else return double.NaN;
            }
        }
    }
}
