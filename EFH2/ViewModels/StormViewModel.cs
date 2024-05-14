using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
    public partial class StormViewModel : ObservableObject, ICreateInputFile
    {
        public event EventHandler<EventArgs>? ValueChanged;

        [XmlIgnore]
        private double _dayRain = double.NaN;

        [XmlIgnore]
        private double _frequency = double.NaN;

        [XmlElement("Name")]
        public string Name => "Storm #" + Number;

        public int Number { get; init; }

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
                else if (value <= RainfallDischargeDataViewModel.DayRainMax)
                {
                    this.SetProperty(ref this._dayRain, value);
                    this.ValueChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        [XmlElement("Frequency")]
        public double Frequency
        {
            get => _frequency;
            set
            {
                if (value == 0) this.SetProperty(ref this._frequency, double.NaN);
                else 
                {
                    this.SetProperty(ref this._frequency, value);
                    this.ValueChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        public void Default()
        {
            Frequency = double.NaN;
            DayRain = 0;
            PeakFlow = double.NaN;
            Runoff = double.NaN;

            DisplayHydrograph = false;
        }

        public void Load(StormViewModel model)
        {
            DayRain = model.DayRain;
            Frequency = model.Frequency;
            PeakFlow = model.PeakFlow;
            Runoff = model.Runoff;
            DisplayHydrograph = model.DisplayHydrograph;
        }
    }
}
