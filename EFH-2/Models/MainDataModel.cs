using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2.Models
{
    public class MainDataModel
    {

        public BasicDataModel BasicDataModel { get; set; }

        public RainfallDischargeDataModel RainfallDischargeDataModel { get; set; }

        public RCNDataModel RCNDataModel { get; set; }

        public void Clear()
        {
            BasicDataModel = new BasicDataModel();
            RainfallDischargeDataModel = new RainfallDischargeDataModel();
            RCNDataModel = new RCNDataModel();
        }
    }
}
