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
    public partial class BasicDataViewModel : ObservableObject, ICreateInputFile
    {
        public event EventHandler<EventArgs>? ValueChanged;

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
        private readonly Dictionary<string, List<string>> _stateCountyDictionary = new();

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

			if (final.Equals(double.NaN))
			{
				this.timeOfConcentrationEntry.Value = double.NaN;
				this.timeOfConcentrationEntry.Status = "";
			}
			else
			{
				this.timeOfConcentrationEntry.Value = Math.Round(final, 2);
				this.timeOfConcentrationEntry.Status = "Calculated";
			}
        }

		private void EntryChanged(object sender, EventArgs e)
		{
            this.CalculateTimeOfConcentration();
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

        public void Default()
        {
            Client = "";
            selectedState = MainViewModel.ChooseMessage;
            selectedCounty = "";
            SelectedStateIndex = 0;
            Practice = "";
            Date = DateTime.Now;
            By = "";

            drainageAreaEntry.Default();
            runoffCurveNumberEntry.Default();
            watershedLengthEntry.Default();
            watershedSlopeEntry.Default();
            timeOfConcentrationEntry.Default();
        }

        public void Clear()
        {
            Default();

            timeOfConcentrationEntry.Status = MainViewModel.ClearedMessage;
            watershedSlopeEntry.Status = MainViewModel.ClearedMessage;
            watershedLengthEntry.Status = MainViewModel.ClearedMessage;
            runoffCurveNumberEntry.Status = MainViewModel.ClearedMessage;
            drainageAreaEntry.Status = MainViewModel.ClearedMessage;
        }

        public void Load(BasicDataViewModel model)
        {
            Client = model.Client;
            Practice = model.Practice;
            By = model.By;
            Date = model.Date;

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

            drainageAreaEntry.Value = model.DrainageArea;
            runoffCurveNumberEntry.Value = model.RunoffCurveNumber;
            watershedLengthEntry.Value = model.WatershedLength;
            watershedSlopeEntry.Value = model.WatershedSlope;
            timeOfConcentrationEntry.Value = model.TimeOfConcentration;
        }
    }
}
