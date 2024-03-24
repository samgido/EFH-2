using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EFH2
{
    public partial class RcnCategory : ObservableObject
    {
        [ObservableProperty]
        private string _label = "";

        [ObservableProperty]
        private List<RcnRow> _rows = new();

        public double AccumulatedArea
        {
            get
            {
                double total = 0;
                foreach (RcnRow row in Rows)
                {
                    if (!(row.AccumulatedArea.Equals(double.NaN))) total += row.AccumulatedArea;
                }
                return total;
            }
        }

        public double AccumulatedWeightedArea
        {
            get
            {
                double total = 0;
                foreach (RcnRow row in Rows)
                {
                    if (!(row.AccumulatedArea.Equals(double.NaN))) total += row.AccumulatedWeightedArea;
                }
                return total;
            }
        }

        public void Default()
        {
            foreach (RcnRow row in Rows) row.Default();
        }
    }
}
