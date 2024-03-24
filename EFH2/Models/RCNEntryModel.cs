using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                if (!(Area.Equals(double.NaN))) return Area * Weight;
                else return double.NaN;
            }
        }
    }
}
