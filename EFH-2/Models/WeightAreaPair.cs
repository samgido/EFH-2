using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2.Models
{
    public partial class WeightAreaPair : ObservableObject
    {
        public int Weight { get; }

        [ObservableProperty]
        private double _area;

        public double WeightedArea => Area * Weight;

        public WeightAreaPair(int weight)
        {
            Weight = weight;
            Area = double.NaN;
        }

        public void Default()
        {
            Area = double.NaN;
        }
    }
}
