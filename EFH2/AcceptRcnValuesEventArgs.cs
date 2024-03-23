using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
    public class AcceptRcnValuesEventArgs : EventArgs
    {
        public double AccumulatedArea { get; private set; }
        public double WeightedCurveNumber { get; private set; }

        public AcceptRcnValuesEventArgs(double accumulatedArea, double weightedCurveNumber)
        {
            AccumulatedArea = accumulatedArea;
            WeightedCurveNumber = weightedCurveNumber;
        }
    }
}
