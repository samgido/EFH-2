using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
    public class MainDataModel
    {
    }

    public class BasicDataModel
    {
        public string Client = "";

        public string State = "";

        public string County = "";

        public string Practice = "";

        public string By = "";

        public DateTime Date = DateTime.Now;

        public double DrainageArea = double.NaN;

        public double RunoffCurveNumber = double.NaN;

        public double WatershedLength = double.NaN;

        public double WatershedSlope = double.NaN;

        public double TimeOfConcentration = double.NaN;
    }

    public class RainfallDischargeDataModel
    {
        public string RainfallDistributionType = "";

        public string DUHType = "";

        public List<StormModel> Storms = new List<StormModel> { };
    }

    public class RCNDataModel
    {
        public RCNGroupModel[] RCNGroups;
    }
}
