using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
    public partial class StormModel : ObservableObject
    {
        public string Name { get; init; }

        [ObservableProperty]
        private double _years = double.NaN;

        private double _dayRain = double.NaN;

        [ObservableProperty]
        private double _peakFlow = double.NaN;

        [ObservableProperty]
        private double _runoff = double.NaN;

        [ObservableProperty]
        public bool _displayHydrograph = false;

        public double DayRain
        {
            get => _dayRain;
            set
            {
                if (value == 0) this.SetProperty(ref this._dayRain, double.NaN);
                else if (value <= RainfallDischargeDataViewModel.DayRainMax) this.SetProperty(ref this._dayRain, value);
            }
        }

        public void Default()
        {
            Years = double.NaN;
            DayRain = 0;
            PeakFlow = double.NaN;
            Runoff = double.NaN;

            DisplayHydrograph = false;
        }
    }
}
