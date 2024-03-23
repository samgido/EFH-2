using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
    public class RcnEntryModel
    {
        public int Weight { get; set; } = 0;

        public double Area = double.NaN;

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
