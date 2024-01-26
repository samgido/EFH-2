using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2.Models
{
    public partial class RCNColumn : ObservableObject
    {
        public RCNColumn()
        {
            _text = new string[3];
            _weightAreaPairs = new WeightAreaPair[4];
        }

        [ObservableProperty]
        private WeightAreaPair[] _weightAreaPairs;

        [ObservableProperty]
        private string[] _text;
    }
}
