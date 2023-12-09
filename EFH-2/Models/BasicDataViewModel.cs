/* BasicDataViewModel.cs
 * Author: Samuel Gido
 */

using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2
{
    /// <summary>
    /// A class that holds all the data in the basic data page
    /// </summary>
    public class BasicDataViewModel : BindableBase
    {
        private string _client = "";
        /// <summary>
        /// The client's title
        /// </summary>
        public string Client
        {
            get { return this._client; }
            set { this.SetProperty(ref this._client, value); }
        }

        /// <summary>
        /// Holds all of the available counties mapped to their respective states
        /// </summary>
        private Dictionary<string, List<string>> _stateCountyDictionary = new();

        /// <summary>
        /// Uses a stream reader to load all of the county and state data into the program
        /// </summary>
        /// <param name="reader"></param>
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

        private string _selectedState = "";
        /// <summary>
        /// Gets or sets the selected state in the combobox
        /// </summary>
        public string SelectedState
        {
            get { return this._selectedState; }
            set
            {
                for (int i = 0; i < States.Count; i++)
                {
                    if (States[i].Content as string == value)
                    {
                        SelectedStateIndex = i;
                        return;
                    }
                    SelectedStateIndex = 0;
                }
            }
        }

        private int _selectedStateIndex = 0;
        /// <summary>
        /// Gets or sets the selected index in the state ComboBox
        /// </summary>
        public int SelectedStateIndex
        {
            get { return this._selectedStateIndex; }
            set
            {
                this.SetProperty(ref this._selectedStateIndex, value);
                
                this._selectedState = _states[_selectedStateIndex].Content.ToString();

                SetCounties(_stateCountyDictionary[SelectedState]);
            }
        }

        private ObservableCollection<ComboBoxItem> _states = new();
        /// <summary>
        /// Gets or sets the abbreviated states as ComboBoxItems
        /// </summary>
        public ObservableCollection<ComboBoxItem> States
        {
            get { return this._states; }
            set { this.SetProperty(ref this._states, value); }
        }

        private string _selectedCounty = "";
        /// <summary>
        /// Gets or sets the selected county in the combobox
        /// </summary>
        public string SelectedCounty
        {
            get { return this._selectedCounty; }
            set
            {
                for (int i = 0; i < Counties.Count; i++)
                {
                    if (Counties[i].Content as string == value)
                    {
                        SelectedCountyIndex = i;
                        return;
                    }
                    SelectedCountyIndex = 0;
                }
            }
        }

        private int _selectedCountyIndex = 0;
        /// <summary>
        /// Gets or sets the selected index of the county ComboBox
        /// </summary>
        public int SelectedCountyIndex
        {
            get { return this._selectedCountyIndex; }
            set
            {
                this.SetProperty(ref this._selectedCountyIndex, value);
                if(value == -1) { return; }
                this._selectedCounty = _counties[_selectedCountyIndex].Content.ToString();
            }
        }

        /// <summary>
        /// Changes the contents of the counties combobox
        /// </summary>
        /// <param name="list">The county names that will be loaded into the combobox</param>
        private void SetCounties(List<string> list)
        {
            Counties.Clear();
            ComboBoxItem c = new();
            c.Content = MainWindow.ChooseMessage;
            Counties.Add(c);

            foreach (string county in list)
            {
                ComboBoxItem countyItem = new();
                countyItem.Content = county;

                Counties.Add(countyItem);
            }

            SelectedCountyIndex = 0;
        }

        private ObservableCollection<ComboBoxItem> _counties = new();
        /// <summary>
        /// Gets or sets the counties available in the combobox
        /// </summary>
        public ObservableCollection<ComboBoxItem> Counties
        {
            get { return this._counties; }
            set { this.SetProperty(ref this._counties, value); }
        }

        private string _practice = "";
        /// <summary>
        /// Gets or sets the practice field
        /// </summary>
        public string Practice
        {
            get { return this._practice; }
            set { this.SetProperty(ref this._practice, value); }
        }

        private string _by = "";
        /// <summary>
        /// Gets or sets the title of who entered the data
        /// </summary>
        public string By
        {
            get { return this._by; }
            set { this.SetProperty(ref this._by, value); }
        }

        private DateTimeOffset _date = new();
        /// <summary>
        /// Gets or sets the date field
        /// </summary>
        public DateTimeOffset Date
        {
            get { return this._date; }
            set { this.SetProperty(ref this._date, value); }
        }

        private double _drainageArea = double.NaN;
        /// <summary>
        /// Gets or sets drainge area field 
        /// </summary>
        public double DrainageArea
        {
            get { return this._drainageArea; }
            set { this.SetProperty(ref this._drainageArea, value); }
        }

        /// <summary>
        /// Checks and corrects the drainage area field
        /// </summary>
        public void CheckDrainageArea()
        {
            if (DrainageArea >= DrainageAreaMin && DrainageArea <= DrainageAreaMax)
            {
                DrainageAreaStatus = "User entered.";
                return;
            }

            DrainageAreaStatus = MainWindow.DrainageAreaInvalidEntryMessage;
        }

        /// <summary>
        /// Minimum value for the drainage area
        /// </summary>
        public static int DrainageAreaMin => 1;
        /// <summary>
        /// Maximum value for a the drainage are
        /// </summary>
        public static int DrainageAreaMax => 2000;

        private double _curveNumber = double.NaN;
        /// <summary>
        /// Gets or sets the curve number field
        /// </summary>
        public double RunoffCurveNumber
        {
            get { return this._curveNumber; }
            set { this.SetProperty(ref this._curveNumber, value); }
        }

        /// <summary>
        /// Checks and corrects the value of the runoff curve number field
        /// </summary>
        public void CheckRunoffCurveNumber()
        {
            if (_curveNumber >= RunoffCurveNumberMin && _curveNumber <= RunoffCurveNumberMax)
            {
                RunoffCurveNumberStatus = "User entered.";
                return;
            }

            RunoffCurveNumberStatus = MainWindow.RunoffCurveNumberInvalidEntryMessage;
        }

        /// <summary>
        /// Minimum value for the runoff curve number
        /// </summary>
        public static int RunoffCurveNumberMin => 40;
        /// <summary>
        /// Maximum value for the runoff curve number
        /// </summary>
        public static int RunoffCurveNumberMax => 98;

        private double _watershedLength = double.NaN;
        /// <summary>
        /// Gets or sets the watershed length field
        /// </summary>
        public double WatershedLength
        {
            get { return this._watershedLength; }
            set { this.SetProperty(ref this._watershedLength, value); }
        }

        /// <summary>
        /// Checks and corrects the watershed length field
        /// </summary>
        public void CheckWatershedLength()
        {
            if (WatershedLength >= WatershedLengthMin && WatershedLength <= WatershedLengthMax)
            {
                WatershedLengthStatus = "User entered.";
                return;
            }
            
            WatershedLengthStatus = MainWindow.WatershedLengthInvalidEntryMessage;
        }

        /// <summary>
        /// Minimum value for the watershed length field
        /// </summary>
        public static int WatershedLengthMin => 200;
        /// <summary>
        /// Minimum value for the watershed length field
        /// </summary>
        public static int WatershedLengthMax => 26000;

        private double _watershedSlope = double.NaN;
        /// <summary>
        /// Gets or sets the watershed slope field
        /// </summary>
        public double WatershedSlope
        {
            get { return this._watershedSlope; }
            set { this.SetProperty(ref this._watershedSlope, value); }
        }

        /// <summary>
        /// Checks and corrects the watershed slope field
        /// </summary>
        public void CheckWatershedSlope()
        {
            if (WatershedSlope >= WatershedSlopeMin && WatershedSlope <= WatershedSlopeMax)
            {
                WatershedSlopeStatus = "User entered";
                return;
            }
            
            WatershedSlopeStatus = MainWindow.WatershedSlopeInvalidEntryMessage;
        }

        /// <summary>
        /// Mimimum value for the watershed slope field
        /// </summary>
        public static double WatershedSlopeMin => 0.5;
        /// <summary>
        /// Maximum value for the watershed slope field
        /// </summary>
        public static double WatershedSlopeMax => 64;

        private double _timeOfConcentration = double.NaN;
        /// <summary>
        /// Gets or sets the time of concentration field
        /// </summary>
        public double TimeOfConcentration
        {
            get { return this._timeOfConcentration; }
            set { this.SetProperty(ref this._timeOfConcentration, value); }
        }

        /// <summary>
        /// Checks and corrects the time of concentration field
        /// </summary>
        public void CheckTimeOfConcentration()
        {
            if (TimeOfConcentration >= TimeOfConcentrationMin && TimeOfConcentration <= TimeOfConcentrationMax)
            {
                TimeOfConcentrationStatus = "User entered";
                return;
            }

            TimeOfConcentrationStatus = MainWindow.TimeOfConcentrationInvalidEntryMessage;
        }

        /// <summary>
        /// Minimum value for the time of concentration field
        /// </summary>
        public static double TimeOfConcentrationMin => 0.1;
        /// <summary>
        /// Maximum value for the time of concentration field
        /// </summary>
        public static double TimeOfConcentrationMax => 10;

        /// <summary>
        /// Summarizes the data into a list of objects
        /// </summary>
        public List<object> Summary
        {
            get
            {
                List<object> l = new();

                l.Add(By);
                l.Add(Date.Date.ToString("MM/dd/yyyy"));
                l.Add(Client);
                l.Add(SelectedCounty);
                l.Add(SelectedState);
                l.Add(Practice);
                l.Add(DrainageArea);
                l.Add(RunoffCurveNumber);
                l.Add(WatershedLength);
                l.Add(WatershedSlope);
                l.Add(TimeOfConcentration);

                return l;
            }
        }

        private string _drainageStatus = "";
        /// <summary>
        /// Gets or sets the current status of the drainage area field
        /// </summary>
        public string DrainageAreaStatus
        {
            get { return this._drainageStatus; }
            set { this.SetProperty(ref this._drainageStatus, value); }
        }

        private string _curveNumberStatus = "";
        /// <summary>
        /// Gets or sets the current status of the runoff curve number field
        /// </summary>
        public string RunoffCurveNumberStatus
        {
            get { return this._curveNumberStatus; }
            set { this.SetProperty(ref this._curveNumberStatus, value); }
        }

        private string _watershedLengthStatus = "";
        /// <summary>
        /// Gets or sets the current status of the watershed length field
        /// </summary>
        public string WatershedLengthStatus
        {
            get { return this._watershedLengthStatus; }
            set { this.SetProperty(ref this._watershedLengthStatus, value); }
        }

        private string _watershedSlopeStatus = "";
        /// <summary>
        /// Gets or sets the current status of the watershed slope field
        /// </summary>
        public string WatershedSlopeStatus
        {
            get { return this._watershedSlopeStatus; }
            set { this.SetProperty(ref this._watershedSlopeStatus, value); }
        }

        private string _timeOfConcentrationStatus = "";
        /// <summary>
        /// Gets or sets the current status of the time of concentration field
        /// </summary>
        public string TimeOfConcentrationStatus
        {
            get { return this._timeOfConcentrationStatus; }
            set { this.SetProperty(ref this._timeOfConcentrationStatus, value); }
        }

        public BasicDataViewModel()
        {
            Date = DateTimeOffset.Parse(DateTime.Now.ToString("MM/dd/yyyy"));
        }

        /// <summary>
        /// Sets every field to their default values
        /// </summary>
        public void Default()
        {
            Client = "";
            SelectedState = MainWindow.ChooseMessage;
            SelectedCounty = MainWindow.ChooseMessage;
            SelectedCountyIndex = 0;
            Practice = ""; 
            Date = DateTimeOffset.Parse(DateTime.Now.ToString("MM/dd/yyyy"));
            By = "";

            DrainageArea = 0;
            RunoffCurveNumber = 0;
            WatershedLength = 0;
            WatershedSlope = 0;
            TimeOfConcentration = 0;
        }

        /// <summary>
        /// Defaults all values and displays a clear message on all status labels
        /// </summary>
        public void Clear()
        {
            Default();

            TimeOfConcentrationStatus = MainWindow.ClearedMessage;
            WatershedSlopeStatus = MainWindow.ClearedMessage;
            WatershedLengthStatus = MainWindow.ClearedMessage;
            RunoffCurveNumberStatus = MainWindow.ClearedMessage;
            DrainageAreaStatus = MainWindow.ClearedMessage;
        }
    }
}
