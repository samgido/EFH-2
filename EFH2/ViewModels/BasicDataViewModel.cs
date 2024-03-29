﻿using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.IO;
using System.Xml.Serialization;

namespace EFH2
{
    public partial class BasicDataViewModel : ObservableObject
    {
        [XmlIgnore]
        public static int DrainageAreaMin => 1;
        [XmlIgnore]
        public static int DrainageAreaMax => 2000;

        [XmlIgnore]
        public static int RunoffCurveNumberMin => 40;
        [XmlIgnore]
        public static int RunoffCurveNumberMax => 98;

        [XmlIgnore]
        public static int WatershedLengthMin => 200;
        [XmlIgnore]
        public static int WatershedLengthMax => 26000;

        [XmlIgnore]
        public static double WatershedSlopeMin => 0.5;
        [XmlIgnore]
        public static double WatershedSlopeMax => 64;

        [XmlIgnore]
        public static double TimeOfConcentrationMin => 0.1;
        [XmlIgnore]
        public static double TimeOfConcentrationMax => 10;

        [XmlIgnore]
        private Dictionary<string, List<string>> _stateCountyDictionary = new();

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

        [XmlElement("Selected State Index")]
        private int _selectedStateIndex = 0;

        [XmlElement("Selected County Index")]
        private int _selectedCountyIndex = 0;

        [ObservableProperty]
        [XmlElement("Drainage Area")]
        private double _drainageArea = double.NaN;

        [ObservableProperty]
        [XmlElement("Runoff Curve Number")]
        private double _runoffCurveNumber = double.NaN;

        [ObservableProperty]
        [XmlElement("Watershed Length")]
        private double _watershedLength = double.NaN;

        [ObservableProperty]
        [XmlElement("Watershed Slope")]
        private double _watershedSlope = double.NaN;

        [ObservableProperty]
        [XmlElement("Time Of Concentration")]
        private double _timeOfConcentration = double.NaN;

        [ObservableProperty]
        [property: XmlIgnore]
        private string _drainageAreaStatus = "";

        [ObservableProperty]
        [property: XmlIgnore]
        private string _runoffCurveNumberStatus = "";

        [ObservableProperty]
        [property: XmlIgnore]
        private string _watershedLengthStatus = "";

        [ObservableProperty]
        [property: XmlIgnore]
        private string _watershedSlopeStatus = "";

        [ObservableProperty]
        [property: XmlIgnore]
        private string _timeOfConcentrationStatus = "";

        [XmlIgnore]
        public int SelectedStateIndex
        {
            get => _selectedStateIndex;
            set
            {
                this.SetProperty(ref this._selectedStateIndex, value);
                this.selectedState = States[_selectedStateIndex].Content.ToString();

                SetCounties(_stateCountyDictionary[selectedState]);                
            }
        }

        [XmlIgnore]
        public int SelectedCountyIndex
        {
            get => _selectedCountyIndex;
            set
            {
                this.SetProperty(ref this._selectedCountyIndex, value);
                if (value == -1) return;
                this.selectedCounty = Counties[_selectedCountyIndex].Content.ToString();
            }
        }

        private void SetCounties(List<string> list)
        {
            Counties.Clear();
            //ComboBoxItem c = new();
            //c.Content = MainViewModel.ChooseMessage;
            //Counties.Add(c);

            foreach (string county in list)
            {
                ComboBoxItem countyItem = new();
                countyItem.Content = county;

                Counties.Add(countyItem);
            }

            if (Counties.Count != 0) SelectedCountyIndex = 0;
        }

        public void LoadStatesAndCounties(StreamReader reader)
        {
            reader.ReadLine();
            _stateCountyDictionary.Add("Choose", new List<string>());

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                string[] XmlElements = line.Split(',');

                string state = XmlElements[1];
                string county = XmlElements[2].Trim('"');

                if (!_stateCountyDictionary.ContainsKey(state))
                {
                    _stateCountyDictionary.Add(state, new());
                    _stateCountyDictionary[state].Add("Choose");
                }

                _stateCountyDictionary[state].Add(county);
            }

            foreach (string s in _stateCountyDictionary.Keys)
            {
                States.Add(new ComboBoxItem() { Content = s });
            }
        }

        public void CheckDrainageArea()
        {
            if (DrainageArea >= DrainageAreaMin && DrainageArea <= DrainageAreaMax) DrainageAreaStatus = MainViewModel.UserEnteredMessage; 
            else DrainageAreaStatus = MainViewModel.DrainageAreaInvalidEntryMessage;
        }

        public void CheckRunoffCurveNumber()
        {
            if (RunoffCurveNumber >= RunoffCurveNumberMin && RunoffCurveNumber <= RunoffCurveNumberMax) RunoffCurveNumberStatus = MainViewModel.UserEnteredMessage; 
            else RunoffCurveNumberStatus = MainViewModel.RunoffCurveNumberInvalidEntryMessage;
        }

        public void CheckWatershedLength()
        {
            if (WatershedLength >= WatershedLengthMin && WatershedLength <= WatershedLengthMax) WatershedLengthStatus = MainViewModel.UserEnteredMessage; 
            else WatershedLengthStatus = MainViewModel.WatershedLengthInvalidEntryMessage;
        }

        public void CheckWatershedSlope()
        {
            if (WatershedSlope >= WatershedSlopeMin && WatershedSlope <= WatershedSlopeMax) WatershedSlopeStatus = MainViewModel.UserEnteredMessage;
            else WatershedSlopeStatus = MainViewModel.WatershedSlopeInvalidEntryMessage;
        }

        public void CheckTimeOfConcentration()
        {
            if (TimeOfConcentration >= TimeOfConcentrationMin && TimeOfConcentration <= TimeOfConcentrationMax) TimeOfConcentrationStatus = MainViewModel.UserEnteredMessage;
            else TimeOfConcentrationStatus = MainViewModel.TimeOfConcentrationInvalidEntryMessage;
        }

        public void Refresh()
        {
            this.OnPropertyChanged(nameof(Client));
            this.OnPropertyChanged(nameof(SelectedStateIndex));
            this.OnPropertyChanged(nameof(SelectedCountyIndex));
            this.OnPropertyChanged(nameof(Practice));
            this.OnPropertyChanged(nameof(By));

            this.OnPropertyChanged(nameof(DrainageArea));
            this.OnPropertyChanged(nameof(RunoffCurveNumber));
            this.OnPropertyChanged(nameof(WatershedLength));
            this.OnPropertyChanged(nameof(WatershedSlope));
            this.OnPropertyChanged(nameof(TimeOfConcentration));
        }

        public void Default()
        {
            Client = "";
            selectedState = MainViewModel.ChooseMessage;
            selectedCounty = "";
            SelectedStateIndex = 0;
            Practice = "";
            Date = DateTime.Now;
            By = "";

            DrainageArea = double.NaN;
            RunoffCurveNumber = double.NaN;
            WatershedLength = double.NaN;
            WatershedSlope = double.NaN;
            TimeOfConcentration = double.NaN;

            TimeOfConcentrationStatus = "";
            WatershedSlopeStatus = "";
            WatershedLengthStatus = "";
            RunoffCurveNumberStatus = "";
            DrainageAreaStatus = "";
        }

        public void Clear()
        {
            Default();

            TimeOfConcentrationStatus = MainViewModel.ClearedMessage;
            WatershedSlopeStatus = MainViewModel.ClearedMessage;
            WatershedLengthStatus = MainViewModel.ClearedMessage;
            RunoffCurveNumberStatus = MainViewModel.ClearedMessage;
            DrainageAreaStatus = MainViewModel.ClearedMessage;
        }
    }
}
