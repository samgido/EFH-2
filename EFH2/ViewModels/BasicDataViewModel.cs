using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.Diagnostics;

namespace EFH2
{
    public partial class BasicDataViewModel : ObservableObject, ICreateInputFile
    {
        public event EventHandler<EventArgs>? ValueChanged;

        public event EventHandler? CountyChanged;

        public BasicDataEntryViewModel drainageAreaEntry = new BasicDataEntryViewModel(1, 2000, "Drainage Area", "Drainage area must be in the range 1 to 2000 acres!");
        public BasicDataEntryViewModel runoffCurveNumberEntry = new BasicDataEntryViewModel(40, 98, "Runoff Curve Number", "Curve number must be in the range 40 to 98!");
        public BasicDataEntryViewModel watershedLengthEntry = new BasicDataEntryViewModel(200, 26000, "Watershed Length", "Watershed length must be in the range 200 to 26000 feet!");
        public BasicDataEntryViewModel watershedSlopeEntry = new BasicDataEntryViewModel(.5, 64, "Watershed Slope", "Watershed slope must be the range 0.5 and 64 percent!");
        public BasicDataEntryViewModel timeOfConcentrationEntry = new BasicDataEntryViewModel(.1, 10, "Time Of Concentration", "Time of concentration cannot be greater than 10.0 hours and cannot be less than 0.1 hours!");

        public double DrainageArea => drainageAreaEntry.Value;

        public double RunoffCurveNumber => runoffCurveNumberEntry.Value;

        public double WatershedLength => watershedLengthEntry.Value;

        public double WatershedSlope => watershedSlopeEntry.Value;

        public double TimeOfConcentration => timeOfConcentrationEntry.Value;

        public Dictionary<string, List<string>> stateCountyDictionary = new Dictionary<string, List<string>>();

        [ObservableProperty]
        private string _client = "";

        [ObservableProperty]
        private string _practice = "";

        [ObservableProperty]
        private string _by = "";

        [ObservableProperty]
        private Nullable<DateTimeOffset> _date = null;

        [ObservableProperty]
        private ObservableCollection<ComboBoxItem> _states = new();

        [ObservableProperty]
        private ObservableCollection<ComboBoxItem> _counties = new();

        public string selectedState = "";

        public string selectedCounty = "";

        private int _selectedStateIndex = 0;

        private int _selectedCountyIndex = 0;

        public int SelectedStateIndex
        {
            get => _selectedStateIndex;
            set
            {
                this.SetProperty(ref this._selectedStateIndex, value);
                this.selectedState = States[_selectedStateIndex].Content.ToString();

                SetCounties(stateCountyDictionary[selectedState]);
            }
        }

        public int SelectedCountyIndex
        {
            get => _selectedCountyIndex;
            set
            {
                if (value == -1) return;
                this.SetProperty(ref this._selectedCountyIndex, value);
                this.selectedCounty = Counties[_selectedCountyIndex].Content.ToString();

                this.CountyChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public BasicDataViewModel()
        {
            drainageAreaEntry.ValueChanged += EntryChanged;
            runoffCurveNumberEntry.ValueChanged += EntryChanged;
            watershedLengthEntry.ValueChanged += EntryChanged;
            watershedSlopeEntry.ValueChanged += EntryChanged;
            timeOfConcentrationEntry.ValueChanged += EntryChanged;
        }

        public void CalculateTimeOfConcentration()
        {
            double final = (Math.Pow(this.WatershedLength, 0.8) * Math.Pow(((1000 / this.RunoffCurveNumber) - 10) + 1, 0.7)) / (1140 * Math.Pow(this.WatershedSlope, 0.5));

            if (double.IsNaN(final))
            {
                //this.timeOfConcentrationEntry.Value = double.NaN;
                this.timeOfConcentrationEntry.SetSilent(double.NaN);
                this.timeOfConcentrationEntry.InputStatus = InputStatus.None;
            }
            else
            {
                //this.timeOfConcentrationEntry.Value = Math.Round(final, 2);
                this.timeOfConcentrationEntry.SetSilent(Math.Round(final, 2));
                this.timeOfConcentrationEntry.InputStatus = InputStatus.Calculated;
            }
        }

        private void EntryChanged(object sender, EventArgs e)
        {
            this.CalculateTimeOfConcentration();
            this.ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SetCounties(List<string> list)
        {
            Counties.Clear();

            foreach (string county in list)
            {
                ComboBoxItem countyItem = new();
                countyItem.Content = county;

                Counties.Add(countyItem);
            }

            if (Counties.Count != 0) SelectedCountyIndex = 0;
        }

        public void Default()
        {
            try
            {
                Client = "";

                this._selectedStateIndex = 0;

                Counties.Clear();
                this.OnPropertyChanged(nameof(SelectedCountyIndex));
                this.OnPropertyChanged(nameof(SelectedStateIndex));

                Practice = "";
                Date = DateTime.Now;
                By = "";
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }

            drainageAreaEntry.Default();
            runoffCurveNumberEntry.Default();
            watershedLengthEntry.Default();
            watershedSlopeEntry.Default();
            timeOfConcentrationEntry.Default();
        }

        public void Clear()
        {
            Default();

            timeOfConcentrationEntry.InputStatus = InputStatus.Cleared;
            watershedSlopeEntry.InputStatus = InputStatus.Cleared;
            watershedLengthEntry.InputStatus = InputStatus.Cleared;
            runoffCurveNumberEntry.InputStatus = InputStatus.Cleared;
            drainageAreaEntry.InputStatus = InputStatus.Cleared;
        }

        public int? GetCountyIndexByName(string county)
        {
            for (int i = 0; i < Counties.Count; i++)
            {
                if (Counties[i].Content.ToString() == county) return i;
            }

            return null;
        }

        public int? GetStateIndexByName(string name)
        {
            for (int i = 0; i < States.Count; i++)
			{
				if (States[i].Content.ToString() == name) return i;
			}

            return null;
		}
    }
}
