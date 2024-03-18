using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2.Models
{
    public class RainfallDischargeDataModel
    {
        public string RainfallDistributionType = "";

        public string DUHType = "";

        public List<EFH_2.Models.StormModel> Storms = new List<EFH_2.Models.StormModel> { };
    }
}
