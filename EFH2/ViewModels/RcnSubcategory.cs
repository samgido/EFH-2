using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EFH2
{
    public partial class RcnSubcategory : ObservableObject
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
                    if (!(double.IsNaN(row.AccumulatedArea))) total += row.AccumulatedArea;
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
                    if (!(double.IsNaN(row.AccumulatedArea))) total += row.AccumulatedWeightedArea;
                }
                return total;
            }
        }

        public void Default() => Rows.ForEach(row => row.Default());
    }
}
