using System;

namespace EFH2
{
	public class AcceptRcnValuesEventArgs : EventArgs
    {
        public double AccumulatedArea { get; private set; }
        public double WeightedCurveNumber { get; private set; }
        public bool AcresSelected { get; private set; }

        public AcceptRcnValuesEventArgs(double accumulatedArea, double weightedCurveNumber, bool acresSelected)
		{
			AccumulatedArea = accumulatedArea;
			WeightedCurveNumber = weightedCurveNumber;
			AcresSelected = acresSelected;
		}
	}
}
