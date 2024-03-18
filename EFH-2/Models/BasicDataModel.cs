using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2.Models
{
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
}
