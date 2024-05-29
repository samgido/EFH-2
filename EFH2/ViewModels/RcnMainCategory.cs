using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
    public partial class RcnCategory : ObservableObject
    {
        [ObservableProperty]
        private string _label = "";

        [ObservableProperty]
        private string _extra = "";

        [ObservableProperty]
        private List<RcnCategory> _rcnSubcategories = new List<RcnCategory>();

        [ObservableProperty]
        private List<RcnRow> _rows = new List<RcnRow>();

        public IEnumerable<RcnRow> AllRows
        {
            get
            {
                List<RcnRow> rows = new List<RcnRow>();
                foreach (RcnRow row in Rows) rows.Add(row);
                foreach (RcnCategory category in RcnSubcategories)
                {
                    foreach (var row in category.AllRows) rows.Add(row);
                }
                return rows;
            }
        }

        public double AccumulatedArea
        {
            get
            {
                double total = 0;
                foreach (RcnRow row in AllRows) if (double.IsNormal(row.AccumulatedArea)) total += row.AccumulatedArea;
                return total;
            }
        }

        public double AccumulatedWeightedArea
        {
            get
            {
                double total = 0;
                foreach (RcnRow row in AllRows) if (double.IsNormal(row.AccumulatedArea)) total += row.AccumulatedWeightedArea;
                return total;
            }
        }

        public void Default()
        {
            foreach (RcnRow row in AllRows) { row.Default(); }
        }
    }
}
