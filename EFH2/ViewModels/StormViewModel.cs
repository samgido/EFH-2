using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using EFH2.Models;

namespace EFH2
{
	public partial class StormViewModel : ObservableObject, ICreateInputFile, INotifyPropertyChanged
    {
        public event EventHandler<EventArgs>? ValueChanged;

        private double _precipitation = double.NaN;

        private double _frequency = double.NaN;

        public string Name => "Storm #" + Number;

        public int Number { get; init; }

        [ObservableProperty]
        private double _peakFlow = double.NaN;

        [ObservableProperty]
        private double _runoff = double.NaN;

        [ObservableProperty]
        public bool _displayHydrograph = false;

        public double Precipitation
        {
            get => _precipitation;
            set
            {
                this.SetProperty(ref this._precipitation, value);
				this.ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

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

        public void SetSilent(SerializedStormModel newModel)
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

        public void SetSilent(StormViewModel newModel)
		{
			this._precipitation = newModel.Precipitation;
			this._frequency = newModel.Frequency;
			this._peakFlow = newModel.PeakFlow;
			this._runoff = newModel.Runoff;
			this._displayHydrograph = newModel.DisplayHydrograph;

			this.OnPropertyChanged(nameof(Precipitation));
			this.OnPropertyChanged(nameof(Frequency));
			this.OnPropertyChanged(nameof(PeakFlow));
			this.OnPropertyChanged(nameof(Runoff));
			this.OnPropertyChanged(nameof(DisplayHydrograph));
		}
    }
}
