using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
    public partial class StormViewModel : ObservableObject
    {
        [XmlIgnore]
        private double _dayRain = double.NaN;

        [XmlElement("Name")]
        public string Name { get; init; }

        [ObservableProperty]
        [XmlElement("Frequency")]
        private double _years = double.NaN;

        [ObservableProperty]
        [XmlElement("Peak Flow")]
        private double _peakFlow = double.NaN;

        [ObservableProperty]
        [XmlElement("Runoff")]
        private double _runoff = double.NaN;

        [ObservableProperty]
        [XmlElement("Display Hydrograph")]
        public bool _displayHydrograph = false;

        [XmlElement("24-hr Rain")]
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

        public void Load(StormViewModel model)
        {
            DayRain = model.DayRain;
            Years = model.Years;
            PeakFlow = model.PeakFlow;
            Runoff = model.Runoff;
            DisplayHydrograph = model.DisplayHydrograph;
        }
    }
}
