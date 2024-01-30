using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2.Models
{
    public partial class RCNRow : ObservableObject
    {
        public RCNRow()
        {
            _text = new string[3];
            _weightAreaPairs = new WeightAreaPair[4];
        }

        [ObservableProperty]
        private WeightAreaPair[] _weightAreaPairs;

        [ObservableProperty]
        private string[] _text;

        public void Default()
        {
            foreach(WeightAreaPair pair in WeightAreaPairs)
            {
                pair.Default();
            }
        }

        public double AccumulatedArea
        {
            get
            {
                double total = 0;
                foreach(WeightAreaPair pair in WeightAreaPairs)
                {
                    if(!pair.Area.Equals(double.NaN)) total += pair.Area;
                }

                return total;
            }
        }

        public double AccumulatedWeightedArea
        {
            get
            {
                double total = 0;
                foreach(WeightAreaPair pair in WeightAreaPairs)
                {
                    if(!pair.WeightedArea.Equals(double.NaN)) total += pair.WeightedArea;
                }

                return total;
            }
        }
    }
}
