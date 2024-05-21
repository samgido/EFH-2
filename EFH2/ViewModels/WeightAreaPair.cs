using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
    public partial class WeightAreaPair : ObservableObject
    {
        [ObservableProperty]
        private double _area = double.NaN;

        public int Weight { get; set; }

        public double WeightedArea
        {
            get
            {
                if (!double.IsNaN(Area)) return Area * Weight;
                else return double.NaN;
            }
        }

        public bool Enabled => Weight != -1;

        public void Default()
        {
            Area = double.NaN;
        }
    }
}
