using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
    public class MainViewModel
    {
        public BasicDataViewModel BasicDataViewModel { get; set; }

        public MainViewModel()
        {
            BasicDataViewModel = new BasicDataViewModel();
        }
    }
}
