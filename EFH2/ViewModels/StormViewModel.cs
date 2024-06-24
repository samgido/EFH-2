using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace EFH2
{
    public partial class StormViewModel : ObservableObject, ICreateInputFile, INotifyPropertyChanged
    {
        public event EventHandler<EventArgs>? ValueChanged;

        [XmlIgnore]
        private double _dayRain = double.NaN;

        [XmlIgnore]
        private double _frequency = double.NaN;

        [XmlElement("Name")]
        public string Name => "Storm #" + Number;

        [XmlIgnore]
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
                this.SetProperty(ref this._dayRain, value);
				this.ValueChanged?.Invoke(this, new EventArgs());
            }
        }

        [XmlElement("Frequency")]
        public double Frequency
        {
            get => _frequency;
            set
            {
				this.SetProperty(ref this._frequency, value);
				this.ValueChanged?.Invoke(this, new EventArgs());
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

        public void SetSilent(StormViewModel newModel)
        {
            this._dayRain = newModel.DayRain;
            this._frequency = newModel.Frequency;
            this._peakFlow = newModel.PeakFlow;
            this._runoff = newModel.Runoff;

            this.OnPropertyChanged(nameof(DayRain));
            this.OnPropertyChanged(nameof(Frequency));
            this.OnPropertyChanged(nameof(PeakFlow));
            this.OnPropertyChanged(nameof(Runoff));
        }
    }
}
