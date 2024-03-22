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

namespace EFH2
{
    public partial class BasicDataViewModel : ObservableObject
    {
        public static int DrainageAreaMin => 1;
        public static int DrainageAreaMax => 2000;

        public static int RunoffCurveNumberMin => 40;
        public static int RunoffCurveNumberMax => 98;

        public static int WatershedLengthMin => 200;
        public static int WatershedLengthMax => 26000;

        public static double WatershedSlopeMin => 0.5;
        public static double WatershedSlopeMax => 64;

        public static double TimeOfConcentrationMin => 0.1;
        public static double TimeOfConcentrationMax => 10;

        private Dictionary<string, List<string>> _stateCountyDictionary = new();

        [ObservableProperty]
        [JsonPropertyName("Client")]
        private string _client = "";

        [ObservableProperty]
        [JsonPropertyName("Practice")]
        private string _practice = "";

        [ObservableProperty]
        [JsonPropertyName("By")]
        private string _by = "";

        [ObservableProperty]
        [JsonPropertyName("Date")]
        private Nullable<DateTimeOffset> _date = null;

        [ObservableProperty]
        private ObservableCollection<ComboBoxItem> _states = new();

        [ObservableProperty]
        private ObservableCollection<ComboBoxItem> _counties = new();

        [JsonPropertyName("SelectedState")]
        private string _selectedState = "";

        [JsonPropertyName("SelectedCounty")]
        private string _selectedCounty = "";

        private int _selectedStateIndex = 0;

        private int _selectedCountyIndex = 0;

        [ObservableProperty]
        [JsonPropertyName("DrainageArea")]
        private double _drainageArea = double.NaN;

        [ObservableProperty]
        [JsonPropertyName("RunoffCurveNumber")]
        private double _runoffCurveNumber = double.NaN;

        [ObservableProperty]
        [JsonPropertyName("WatershedLength")]
        private double _watershedLength = double.NaN;

        [ObservableProperty]
        [JsonPropertyName("WatershedSlope")]
        private double _watershedSlope = double.NaN;

        [ObservableProperty]
        [JsonPropertyName("TimeOfConcentration")]
        private double _timeOfConcentration = double.NaN;

        [ObservableProperty]
        private string _drainageAreaStatus = "";

        [ObservableProperty]
        private string _runoffCurveNumberStatus = "";

        [ObservableProperty]
        private string _watershedLengthStatus = "";

        [ObservableProperty]
        private string _watershedSlopeStatus = "";

        [ObservableProperty]
        private string _timeOfConcentrationStatus = "";

        public int SelectedStateIndex
        {
            get => _selectedStateIndex;
            set
            {
                this.SetProperty(ref this._selectedStateIndex, value);
                this._selectedState = States[_selectedStateIndex].Content.ToString();

                SetCounties(_stateCountyDictionary[_selectedState]);                
            }
        }

        public int SelectedCountyIndex
        {
            get => _selectedCountyIndex;
            set
            {
                this.SetProperty(ref this._selectedCountyIndex, value);
                if (value == -1) return;
                this._selectedCounty = Counties[_selectedCountyIndex].Content.ToString();
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

                string[] elements = line.Split(',');

                string state = elements[1];
                string county = elements[2].Trim('"');

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
    }
}
