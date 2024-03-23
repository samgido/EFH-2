using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
    public partial class RCNCategory : ObservableObject
    {
        [ObservableProperty]
        private string _label = "";

        [ObservableProperty]
        private List<RCNRow> _rows = new();

        public double AccumulatedArea
        {
            get
            {
                double total = 0;
                foreach (RCNRow row in Rows)
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
                foreach (RCNRow row in Rows)
                {
                    if (!(row.AccumulatedArea.Equals(double.NaN))) total += row.AccumulatedWeightedArea;
                }
                return total;
            }
        }

        public void Default()
        {
            foreach (RCNRow row in Rows) row.Default();
        }
    }
}
