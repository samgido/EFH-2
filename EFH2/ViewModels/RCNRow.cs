﻿using CommunityToolkit.Mvvm.ComponentModel;

namespace EFH2
{
	public partial class RcnRow : ObservableObject
    {
        [ObservableProperty]
        private string _text = "";

        [ObservableProperty]
        private string _quality = "";

        [ObservableProperty]
        private WeightAreaPair[] _entries = { new WeightAreaPair(), new WeightAreaPair(), new WeightAreaPair(), new WeightAreaPair() };

        public double AccumulatedArea
        {
            get
            {
                double total = 0;
                foreach (WeightAreaPair entry in Entries)
                {
                    if (!double.IsNaN(entry.Area)) total += entry.Area;
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
                    if (!double.IsNaN(entry.Area)) total += entry.WeightedArea;
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
