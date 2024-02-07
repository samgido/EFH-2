using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2.Models
{
    public partial class WeightAreaPair : ObservableObject
    {
        #region Observable Properties

        [ObservableProperty]
        private double _area;

        #endregion

        #region Properties

        public int Weight { get; set; }

        public double WeightedArea => Area * Weight;

        public bool Enabled => Weight != -1;

        #endregion

        #region Methods

        public void Default()
        {
            Area = double.NaN;
        }

        partial void OnAreaChanged(double oldValue, double newValue)
        {
            this.AreaChanged?.Invoke(this, (oldValue, newValue));
        }

        #endregion

        #region Events

        public event EventHandler<(double oldValue, double newValue)>? AreaChanged;

        #endregion

        public WeightAreaPair()
        {
            Area = double.NaN;
        }
    }
}
