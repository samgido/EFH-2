using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2
{
    public class MainViewModel
    {
        public BasicDataViewModel BasicDataViewModel { get; set; }

        public RainfallDataViewModel RainfallDataViewModel { get; set; }

        public RCNDataViewModel RCNDataViewModel { get; set; }
    }
}
