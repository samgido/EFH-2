using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace EFH2
{
	public partial class StormViewModel : ObservableObject, ICreateInputFile, INotifyPropertyChanged
    {
        public event EventHandler<EventArgs>? ValueChanged;

        [XmlIgnore]
        private double _precipitation = double.NaN;

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
        public double Precipitation
        {
            get => _precipitation;
            set
            {
                this.SetProperty(ref this._precipitation, value);
				this.ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [XmlElement("Frequency")]
        public double Frequency
        {
            get => _frequency;
            set
            {
				this.SetProperty(ref this._frequency, value);
				this.ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Default()
        {
            Frequency = double.NaN;
            Precipitation = double.NaN;
            PeakFlow = double.NaN;
            Runoff = double.NaN;

            DisplayHydrograph = false;
        }

        public void Load(StormViewModel model)
        {
            Precipitation = model.Precipitation;
            Frequency = model.Frequency;
            PeakFlow = model.PeakFlow;
            Runoff = model.Runoff;
            DisplayHydrograph = model.DisplayHydrograph;
        }

        public void SetSilent(StormViewModel newModel)
        {
            this._precipitation = newModel.Precipitation;
            this._frequency = newModel.Frequency;
            this._peakFlow = newModel.PeakFlow;
            this._runoff = newModel.Runoff;

            this.OnPropertyChanged(nameof(Precipitation));
            this.OnPropertyChanged(nameof(Frequency));
            this.OnPropertyChanged(nameof(PeakFlow));
            this.OnPropertyChanged(nameof(Runoff));
        }
    }
}
