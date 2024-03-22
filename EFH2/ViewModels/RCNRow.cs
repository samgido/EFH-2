using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
    public partial class RCNRow : ObservableObject
    {
        [ObservableProperty]
        private string[] _text = { "", "", "", "" };

        [ObservableProperty]
        private WeightAreaPair[] _entries = { new WeightAreaPair(), new WeightAreaPair(), new WeightAreaPair(), new WeightAreaPair() };
    }
}
