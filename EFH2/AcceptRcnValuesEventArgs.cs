using System;

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
