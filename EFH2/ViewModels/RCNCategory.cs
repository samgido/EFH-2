using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
    public partial class RCNCategory : ObservableObject
    {
        [ObservableProperty]
        private string _label = "";

        [ObservableProperty]
        private List<RCNRow> _rows = new();
    }
}
