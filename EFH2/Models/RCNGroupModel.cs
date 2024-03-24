using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
    public class RcnGroupModel
    {
        [XmlElement("Entries")]
        public List<RcnEntryModel> Entries = new List<RcnEntryModel>();

        [XmlIgnore]
        public double AccumulatedArea
        {
            get
            {
                double total = 0;
                foreach (RcnEntryModel entry in Entries)
                {
                    if (!(entry.Area.Equals(double.NaN))) total += entry.Area;
                }
                return total;
            }
        }

        [XmlIgnore]
        public double AccumulatedWeightedArea
        {
            get
            {
                double total = 0;
                foreach (RcnEntryModel entry in Entries)
                {
                    if (!(entry.Area.Equals(double.NaN))) total += entry.WeightedArea;
                }
                return total;
            }
        }
    }
}
