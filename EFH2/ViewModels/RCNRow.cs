using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
    public partial class RCNRow : ObservableObject
    {
        [ObservableProperty]
        private string[] _text = { "", "", "", "" };

        [ObservableProperty]
        private WeightAreaPair[] _entries = { new WeightAreaPair(), new WeightAreaPair(), new WeightAreaPair(), new WeightAreaPair() };

        public double AccumulatedArea
        {
            get
            {
                double total = 0;
                foreach (WeightAreaPair entry in Entries)
                {
                    if (!(entry.Area.Equals(double.NaN))) total += entry.Area;
                }
                return total;
            }
        }

        public double AccumulatedWeightedArea
        {
            get
            {
                double total = 0;
                foreach (WeightAreaPair entry in Entries)
                {
                    if (!(entry.Area.Equals(double.NaN))) total += entry.WeightedArea;
                }
                return total;
            }
        }

        public void Default()
        {
            foreach (WeightAreaPair entry in  Entries) entry.Default(); 
        }
    }
}
