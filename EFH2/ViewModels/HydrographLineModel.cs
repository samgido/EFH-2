using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
	public class HydrographLineModel
	{
		public int Frequency { get; set; }
		public double Start { get; set; }
		public double Increment { get; set; }
		public List<double> Values { get; set; }

		public HydrographLineModel(int frequency, double start, double increment, List<double> values)
		{
			Frequency = frequency;
			Start = start;
			Increment = increment;
			Values = values;
		}
	}
}
