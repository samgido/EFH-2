using System;
using System.Collections.Generic;
using System.Diagnostics;
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

		public double DrainageArea { get; set; } = double.NaN;
		public double RunoffCurveNumber { get; set; } = double.NaN;
		public double WatershedLength { get; set; } = double.NaN;
		public double WatershedSlope { get; set; } = double.NaN;
		public double TimeOfConcentration { get; set; } = double.NaN;
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

		public void SetRcnValueFromOldFormat(double position, double pageNumber, int weight, double area)
		{
			try
			{
				//int[] rowsInSections = { 15, 12, 6, 14, 15, 8, 8, 4 };
				int[] rowsInOrder = { 3, 4, 5, 6, 7, 0, 1, 2 };
				int[] rowsInSections = { 8, 8, 4, 15, 12, 6, 14, 15 };

				int row = (int)Math.Floor((position - 27) / 4);
				int column = (int)Math.Floor(((position - 3) % 4) * 2 + 4);

				if (pageNumber != 6)
				{
					for (int i = 0; i < rowsInOrder[(int)pageNumber - 1]; i++)
						row += rowsInSections[i];
				}

				column = (column - 4) / 2;

				RcnGroupModel group = null;
				switch (column)
				{
					case 1:
						group = GroupB;
						break;
					case 2:
						group = GroupC;
						break;
					case 3:
						group = GroupD;
						break;
					default:
						group = GroupA;
						break;
				}

				if (row == 19) row++;
				if (row == 66) row++;

				if (group.Entries[row - 1].Weight != weight)
					return;

				group.Entries[row - 1].Area = area;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}
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
