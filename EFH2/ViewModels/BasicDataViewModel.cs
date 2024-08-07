﻿using CommunityToolkit.Mvvm.ComponentModel;
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

        [XmlElement("Drainage Area")]
        public BasicDataEntryViewModel drainageAreaEntry = new BasicDataEntryViewModel(1, 2000, "Drainage Area", "Drainage area must be in the range 1 to 2000 acres!");
        [XmlElement("Runoff Curve Number")]
        public BasicDataEntryViewModel runoffCurveNumberEntry = new BasicDataEntryViewModel(40, 98, "Runoff Curve Number", "Curve number must be in the range 40 to 98!");
        [XmlElement("Watershed Length")]
        public BasicDataEntryViewModel watershedLengthEntry = new BasicDataEntryViewModel(200, 26000, "Watershed Length", "Watershed length must be in the range 200 to 26000 feet!");
        [XmlElement("Watershed Slope")]
        public BasicDataEntryViewModel watershedSlopeEntry = new BasicDataEntryViewModel(.5, 64, "Watershed Slope", "Watershed slope must be the range 0.5 and 64 percent!");
        [XmlElement("Time Of Concentration")]
        public BasicDataEntryViewModel timeOfConcentrationEntry = new BasicDataEntryViewModel(.1, 10, "Time Of Concentration", "Time of concentration cannot be greater than 10.0 hours and cannot be less than 0.1 hours!");

        [XmlIgnore]
        public double DrainageArea => drainageAreaEntry.Value;

        [XmlIgnore]
        public double RunoffCurveNumber => runoffCurveNumberEntry.Value;

        [XmlIgnore]
        public double WatershedLength => watershedLengthEntry.Value;

        [XmlIgnore]
        public double WatershedSlope => watershedSlopeEntry.Value;

        [XmlIgnore]
        public double TimeOfConcentration => timeOfConcentrationEntry.Value;

        [XmlIgnore]
        public Dictionary<string, List<string>> stateCountyDictionary = new Dictionary<string, List<string>>();

        [ObservableProperty]
        [XmlElement("Client")]
        private string _client = "";

        [ObservableProperty]
        [XmlElement("Practice")]
        private string _practice = "";

        [ObservableProperty]
        [XmlElement("By")]
        private string _by = "";

        [ObservableProperty]
        [XmlElement("Date")]
        private Nullable<DateTimeOffset> _date = null;

        [property: XmlIgnore]
        [ObservableProperty]
        private ObservableCollection<ComboBoxItem> _states = new();

        [property: XmlIgnore]
        [ObservableProperty]
        private ObservableCollection<ComboBoxItem> _counties = new();

        [XmlElement("Selected State")]
        public string selectedState = "";

        [XmlElement("Selected County")]
        public string selectedCounty = "";

        [XmlIgnore]
        private int _selectedStateIndex = 0;

        [XmlIgnore]
        private int _selectedCountyIndex = 0;

        [XmlIgnore]
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

        [XmlIgnore]
        public int SelectedCountyIndex
        {
            get => _selectedCountyIndex;
            set
            {
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
                this.timeOfConcentrationEntry.Value = double.NaN;
				//this.timeOfConcentrationEntry.SetSilent(double.NaN);
                this.timeOfConcentrationEntry.InputStatus = InputStatus.None;
			}
			else
			{
                this.timeOfConcentrationEntry.Value = Math.Round(final, 2);
                //this.timeOfConcentrationEntry.SetSilent(Math.Round(final, 2));
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

        public void Load(BasicDataViewModel model)
        {
            _client = model.Client;
            _practice = model.Practice;
            _by = model.By;
            _date = model.Date;

            for (int i = 0; i < States.Count; i++)
            {
                string content = States[i].Content as string;
                if (content == model.selectedState)
                {
                    SelectedStateIndex = i;
                    break;
                }
            }

            for (int i = 0; i < Counties.Count; i++)
            {
                string content = Counties[i].Content as string;
                if (content == model.selectedCounty)
                {
                    SelectedCountyIndex = i;
                    break;
                }
            }

            drainageAreaEntry.SetSilent(model.DrainageArea);
            runoffCurveNumberEntry.SetSilent(model.RunoffCurveNumber);
            watershedLengthEntry.SetSilent(model.WatershedLength);
            watershedSlopeEntry.SetSilent(model.WatershedSlope);
            timeOfConcentrationEntry.SetSilent(model.TimeOfConcentration);

            this.OnPropertyChanged(nameof(Client));
            this.OnPropertyChanged(nameof(Practice));
            this.OnPropertyChanged(nameof(By));
            this.OnPropertyChanged(nameof(Date));
        }
    }
}
