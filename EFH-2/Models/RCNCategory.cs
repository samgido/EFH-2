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
        private List<RCNColumn> _columns = new();

        public void Default()
        {
            foreach(RCNColumn col in Columns)
            {
                col.Default();
            }
        }

        public double AccumulatedArea
        {
            get
            {
                double total = 0;

                foreach(RCNColumn col in Columns)
                {
                    total += col.AccumulatedArea;
                }

                return total;
            }
        }

        public double AccumulatedWeightedArea
        {
            get
            {
                double total = 0;

                foreach (RCNColumn col in Columns)
                {
                    total += col.AccumulatedWeightedArea;
                }

                return total;
            }
        }

    }
}
