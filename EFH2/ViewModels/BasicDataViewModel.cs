using CommunityToolkit.Mvvm.ComponentModel;
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

        [XmlElement("SelectedState")]
        public string selectedState = "";

        [XmlElement("SelectedCounty")]
        public string selectedCounty = "";

        [XmlIgnore]
        private int _selectedStateIndex = 0;

        [XmlIgnore]
        private int _selectedCountyIndex = 0;

        [ObservableProperty]
        [XmlElement("DrainageArea")]
        private double _drainageArea = double.NaN;

        [ObservableProperty]
        [XmlElement("RunoffCurveNumber")]
        private double _runoffCurveNumber = double.NaN;

        [ObservableProperty]
        [XmlElement("WatershedLength")]
        private double _watershedLength = double.NaN;

        [ObservableProperty]
        [XmlElement("WatershedSlope")]
        private double _watershedSlope = double.NaN;

        [ObservableProperty]
        [XmlElement("TimeOfConcentration")]
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
            ComboBoxItem c = new();
            c.Content = MainViewModel.ChooseMessage;
            Counties.Add(c);

            foreach (string county in list)
            {
                ComboBoxItem countyItem = new();
                countyItem.Content = county;

                Counties.Add(countyItem);
            }

            SelectedCountyIndex = 0;
        }

        public void LoadStatesAndCounties(StreamReader reader)
        {
            reader.ReadLine();
            _stateCountyDictionary.Add("Choose", new List<string>() { "Choose" });

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                string[] XmlElements = line.Split(',');

                string state = XmlElements[1];
                string county = XmlElements[2].Trim('"');

                if (!_stateCountyDictionary.ContainsKey(state))
                {
                    _stateCountyDictionary.Add(state, new());
                }

                _stateCountyDictionary[state].Add(county);
            }

            foreach (string s in _stateCountyDictionary.Keys)
            {
                ComboBoxItem cbox = new();
                cbox.Content = s;

                States.Add(cbox);
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

        public void Default()
        {
            Client = "";
            selectedState = MainViewModel.ChooseMessage;
            selectedCounty = MainViewModel.ChooseMessage;
            SelectedCountyIndex = 0;
            Practice = "";
            Date = DateTime.Now;
            By = "";

            DrainageArea = double.NaN;
            RunoffCurveNumber = double.NaN;
            WatershedLength = double.NaN;
            WatershedSlope = double.NaN;
            TimeOfConcentration = double.NaN;
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
