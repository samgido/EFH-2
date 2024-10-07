using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2.Models
{
	public class SerializedDataModel
	{
		#region Basic Data
		public string Client { get; set; }

		public string Practice { get; set; }

		public string By { get; set; }

		public Nullable<DateTimeOffset> Date { get; set; }

		public uint SelectedStateIndex { get; set; }
		public uint SelectedCountyIndex { get; set; }

		public double DrainageArea { get; set; }
		public double RunoffCurveNumber { get; set; }
		public double WatershedLength { get; set; }
		public double WatershedSlope { get; set; }
		public double TimeOfConcentration { get; set; }
		#endregion

		#region Rainfall/Discharge Data
		public uint SelectedRainfallDistributionTypeIndex { get; set; }	
		public uint SelectedDuhTypeIndex { get; set; }	

		public List<SerializedStormModel> Storms { get; set; }
		#endregion

		#region RCN Data
        public RcnGroupModel GroupA { get; set; }
        public RcnGroupModel GroupB { get; set; }
        public RcnGroupModel GroupC { get; set; }
        public RcnGroupModel GroupD { get; set; }

		public bool AcresSelected { get; set; }
		#endregion
	}

	public class SerializedStormModel
	{
		public double Precipitation { get; set; }
		public double Frequency { get; set; }
		public double PeakFlow { get; set; }
		public double Runoff { get; set; }

		public bool DisplayHydrograph { get; set; }
	}
}
