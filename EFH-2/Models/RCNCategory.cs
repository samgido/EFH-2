using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2.Models
{
    public partial class RCNCategory : ObservableObject
    {

        [ObservableProperty]
        private string _label;

        [ObservableProperty]
        private List<RCNRow> _columns = new();

        public void Default()
        {
            foreach(RCNRow col in Columns)
            {
                col.Default();
            }
        }

        public double AccumulatedArea
        {
            get
            {
                double total = 0;

                foreach(RCNRow col in Columns)
                {
                    if (!col.AccumulatedArea.Equals(double.NaN)) total += col.AccumulatedArea;
                }

                return total;
            }
        }

        public double AccumulatedWeightedArea
        {
            get
            {
                double total = 0;

                foreach (RCNRow col in Columns)
                {
                    if (!col.AccumulatedWeightedArea.Equals(double.NaN)) total += col.AccumulatedWeightedArea;
                }

                return total;
            }
        }

    }
}
