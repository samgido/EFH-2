/* BasicDataViewModel.cs
 * Author: Samuel Gido
 */

using CommunityToolkit.Mvvm.ComponentModel;
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
    public partial class BasicDataViewModel : ObservableObject
    {
        #region Private Fields

        private Models.BasicDataModel model;

        /// <summary>
        /// Holds all of the available counties mapped to their respective states
        /// </summary>
        private Dictionary<string, List<string>> _stateCountyDictionary = new();

        private string _selectedState = "";

        private int _selectedStateIndex = 0;

        private string _selectedCounty = "";

        private int _selectedCountyIndex = 0;

        #endregion

        #region Public Fields

        /// <summary>
        /// Minimum value for the drainage area
        /// </summary>
        public static int DrainageAreaMin => 1;
        /// <summary>
        /// Maximum value for a the drainage are
        /// </summary>
        public static int DrainageAreaMax => 2000;

        /// <summary>
        /// Minimum value for the runoff curve number
        /// </summary>
        public static int RunoffCurveNumberMin => 40;
        /// <summary>
        /// Maximum value for the runoff curve number
        /// </summary>
        public static int RunoffCurveNumberMax => 98;

        /// <summary>
        /// Minimum value for the watershed length field
        /// </summary>
        public static int WatershedLengthMin => 200;
        /// <summary>
        /// Minimum value for the watershed length field
        /// </summary>
        public static int WatershedLengthMax => 26000;

        /// <summary>
        /// Minimum value for the time of concentration field
        /// </summary>
        public static double TimeOfConcentrationMin => 0.1;
        /// <summary>
        /// Maximum value for the time of concentration field
        /// </summary>
        public static double TimeOfConcentrationMax => 10;

        /// <summary>
        /// Mimimum value for the watershed slope field
        /// </summary>
        public static double WatershedSlopeMin => 0.5;
        /// <summary>
        /// Maximum value for the watershed slope field
        /// </summary>
        public static double WatershedSlopeMax => 64;

        #endregion

        #region Observable Properties 

        [ObservableProperty]
        private string _client = "";

        [ObservableProperty]
        private ObservableCollection<ComboBoxItem> _states = new();

        [ObservableProperty]
        private ObservableCollection<ComboBoxItem> _counties = new();

        [ObservableProperty]
        private string _practice = "";

        [ObservableProperty]
        private string _by = "";

        [ObservableProperty]
        private DateTimeOffset _date = new();

        [ObservableProperty]
        private double _drainageArea = double.NaN;

        [ObservableProperty]
        private double _runoffCurveNumber = double.NaN;

        [ObservableProperty]
        private double _watershedLength = double.NaN;

        [ObservableProperty]
        private double _watershedSlope = double.NaN;

        [ObservableProperty]
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

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the selected state in the combobox
        /// </summary>
        public string SelectedState
        {
            get => this._selectedState; 
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

        /// <summary>
        /// Gets or sets the selected index in the state ComboBox
        /// </summary>
        public int SelectedStateIndex
        {
            get => this._selectedStateIndex; 
            set
            {
                this.SetProperty(ref this._selectedStateIndex, value);
                
                this._selectedState = _states[_selectedStateIndex].Content.ToString();

                SetCounties(_stateCountyDictionary[SelectedState]);
            }
        }

        /// <summary>
        /// Gets or sets the selected county in the combobox
        /// </summary>
        public string SelectedCounty
        {
            get => this._selectedCounty; 
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

        /// <summary>
        /// Gets or sets the selected index of the county ComboBox
        /// </summary>
        public int SelectedCountyIndex
        {
            get => this._selectedCountyIndex; 
            set
            {
                this.SetProperty(ref this._selectedCountyIndex, value);
                if(value == -1) { return; }
                this._selectedCounty = _counties[_selectedCountyIndex].Content.ToString();
            }
        }

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

        #endregion

        #region Methods

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
        /// Checks and corrects the value of the runoff curve number field
        /// </summary>
        public void CheckRunoffCurveNumber()
        {
            if (_runoffCurveNumber >= RunoffCurveNumberMin && _runoffCurveNumber <= RunoffCurveNumberMax)
            {
                RunoffCurveNumberStatus = "User entered.";
                return;
            }

            RunoffCurveNumberStatus = MainWindow.RunoffCurveNumberInvalidEntryMessage;
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

            DrainageArea = double.NaN;
            RunoffCurveNumber = double.NaN;
            WatershedLength = double.NaN;
            WatershedSlope = double.NaN;
            TimeOfConcentration = double.NaN;
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

        public void AcceptRCNValues(int totalArea, int curveNumber)
        {
            DrainageArea = totalArea;
            RunoffCurveNumber = curveNumber;

            DrainageAreaStatus = "from RCN Calculator";
            RunoffCurveNumberStatus = "from RCN Calculator";
        }

        #endregion

        public BasicDataViewModel(Models.BasicDataModel model)
        {
            Date = DateTimeOffset.Parse(DateTime.Now.ToString("MM/dd/yyyy"));
            this.model = model;
        }
    }
}
