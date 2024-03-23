using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
    public class RcnGroupModel
    {
        public List<RcnEntryModel> Entries = new List<RcnEntryModel>();

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
