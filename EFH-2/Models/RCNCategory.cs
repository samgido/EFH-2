using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2.Models
{
    public partial class RCNCategory : ObservableObject
    {

        [ObservableProperty]
        private string _label;

        [ObservableProperty]
        private List<RCNColumn> _columns = new();

    }
}
