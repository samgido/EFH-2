using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2.Models
{
    public partial class RCNColumn : ObservableObject
    {
        public RCNColumn()
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
                    total += pair.Area;
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
                    total += pair.WeightedArea;
                }

                return total;
            }
        }

        public double GroupAWeightedArea => WeightAreaPairs[0].WeightedArea;

        public double GroupBWeightedArea => WeightAreaPairs[1].WeightedArea;
        
        public double GroupCWeightedArea => WeightAreaPairs[2].WeightedArea;

        public double GroupDWeightedArea => WeightAreaPairs[3].WeightedArea;

        public double GroupAAccumulatedArea => WeightAreaPairs[0].Area == double.NaN ? WeightAreaPairs[0].Area : 0;

        public double GroupBAccumulatedArea => WeightAreaPairs[1].Area == double.NaN ? WeightAreaPairs[1].Area : 0;
        
        public double GroupCAccumulatedArea => WeightAreaPairs[2].Area == double.NaN ? WeightAreaPairs[2].Area : 0;

        public double GroupDAccumulatedArea => WeightAreaPairs[3].Area == double.NaN ? WeightAreaPairs[3].Area : 0;
    }
}
