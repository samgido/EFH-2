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
    /// View model for the basic data page
    /// </summary>
    public class BasicDataViewModel : BindableBase
    {
        /// <summary>
        /// The client's title
        /// </summary>
        private string _client = "";
        public string Client
        {
            get { return this._client; }
            set { this.SetProperty(ref this._client, value); }
        }

        /// <summary>
        /// The selected state in the state ComboBox
        /// </summary>
        public string SelectedState
        {
            get { return this.SelectedState; }
            set 
            { 
                for(int i = 0; i < _states.Count; i++)
                {
                    var c = _states[i];
                    if (c.Content as string == value)
                    {
                        SelectedStateIndex = i;
                        return;
                    }
                }

                SelectedStateIndex = 0;
            }
        }

        /// <summary>
        /// The selected index in the state ComboBox
        /// </summary>
        private int _selectedStateIndex = 0;
        public int SelectedStateIndex
        {
            get { return this._selectedStateIndex; }
            set { this.SetProperty(ref this._selectedStateIndex, value); }
        }

        /// <summary>
        /// All abbbreviated states as ComboBoxItems
        /// </summary>
        private ObservableCollection<ComboBoxItem> _states = new();
        public ObservableCollection<ComboBoxItem> States
        {
            get { return this._states; }
            set { this.SetProperty(ref this._states, value); }
        }

        /// <summary>
        /// The selected county in the county ComboBox
        /// </summary>
        public string SelectedCounty
        {
            get { return this.SelectedCounty; }
            set 
            { 
                for(int i = 0; i < _counties.Count; i++)
                {
                    var c = _counties[i];
                    if (c.Content as string == value)
                    {
                        SelectedCountyIndex = i;
                        return;
                    }
                }

                SelectedCountyIndex = 0;
            }
        }

        /// <summary>
        /// The selected index of the county ComboBox
        /// </summary>
        private int _selectedCountyIndex = 0;
        public int SelectedCountyIndex
        {
            get { return this._selectedCountyIndex; }
            set { this.SetProperty(ref this._selectedCountyIndex, value); }
        }

        /// <summary>
        /// All counties as ComboBoxItems
        /// 
        /// This collection changes when the state selection changes
        /// </summary>
        private ObservableCollection<ComboBoxItem> _counties = new();
        public ObservableCollection<ComboBoxItem> Counties
        {
            get { return this._counties; }
            set { this.SetProperty(ref this._counties, value); }
        }

        /// <summary>
        /// The practice field
        /// </summary>
        private string _practice = "";
        public string Practice
        {
            get { return this._practice; }
            set { this.SetProperty(ref this._practice, value); }
        }

        /// <summary>
        /// The title of who entered the data
        /// </summary>
        private string _by = "";
        public string By
        {
            get { return this._by; }
            set { this.SetProperty(ref this._by, value); }
        }

        /// <summary>
        /// The date field
        /// </summary>
        private DateTimeOffset _date = new();
        public DateTimeOffset Date
        {
            get { return this._date; }
            set { this.SetProperty(ref this._date, value); }
        }

        /// <summary>
        /// The drainge area field 
        /// </summary>
        private int _drainageArea = 0;
        public int DrainageArea
        {
            get { return this._drainageArea; }
            set { this.SetProperty(ref this._drainageArea, value); }
        }

        public void CheckDrainageArea()
        {
            if (DrainageArea >= DrainageAreaMin && DrainageArea <= DrainageAreaMax)
            {
                DrainageAreaStatus = "User entered.";
                return;
            }
            else if(DrainageArea > DrainageAreaMax)
            {
                DrainageArea = DrainageAreaMax;
            }
            else
            {
                DrainageArea = 0;
            }

            DrainageAreaStatus = "Drainage area must be in the range 1 to 2000 acres!";
        }

        public static int DrainageAreaMin => 1;
        public static int DrainageAreaMax => 2000;

        /// <summary>
        /// The curve number field
        /// </summary>
        private float _curveNumber = 0;
        public float RunoffCurveNumber
        {
            get { return this._curveNumber; }
            set { this.SetProperty(ref this._curveNumber, value); }
        }

        public void CheckRunoffCurveNumber()
        {
            if (RunoffCurveNumber >= RunoffCurveNumberMin && RunoffCurveNumber <= RunoffCurveNumberMax)
            {
                RunoffCurveNumberStatus = "User entered.";
                return;
            }
            else if(RunoffCurveNumber > RunoffCurveNumberMax)
            {
                RunoffCurveNumber = RunoffCurveNumberMax;
            }
            else
            {
                RunoffCurveNumber = 0;
            }

            RunoffCurveNumberStatus = "Curve number must be in the range 40 to 98!";
        }

        public static int RunoffCurveNumberMin => 40;
        public static int RunoffCurveNumberMax => 98;

        /// <summary>
        /// The watershed length field
        /// </summary>
        private int _watershedLength = 0;
        public int WatershedLength
        {
            get { return this._watershedLength; }
            set { this.SetProperty(ref this._watershedLength, value); }
        }

        public void CheckWatershedLength()
        {
            if (WatershedLength >= WatershedLengthMin && WatershedLength <= WatershedLengthMax)
            {
                WatershedLengthStatus = "User entered.";
                return;
            }
            else if(WatershedLength > WatershedLengthMax)
            {
                WatershedLength = WatershedLengthMax;
            }
            else
            {
                WatershedLength = 0;
            }
            
            WatershedLengthStatus = "Watershed length must be in the range 200 to 26000 feet!";
        }

        public static int WatershedLengthMin => 200;
        public static int WatershedLengthMax => 26000;

        /// <summary>
        /// The watershed slope field
        /// </summary>
        private double _watershedSlope = 0;
        public double WatershedSlope
        {
            get { return this._watershedSlope; }
            set { this.SetProperty(ref this._watershedSlope, value); }
        }

        public void CheckWatershedSlope()
        {
            if (WatershedSlope >= WatershedSlopeMin && WatershedSlope <= WatershedSlopeMax)
            {
                WatershedSlopeStatus = "User entered";
                return;
            }
            else if(WatershedSlope > WatershedSlopeMax)
            {
                WatershedSlope = WatershedSlopeMax;
            }
            else
            {
                WatershedSlope = 0;
            }
            
            WatershedSlopeStatus = "Watershed slope must be the range 0.5 and 64 percent!";
        }

        public static double WatershedSlopeMin => 0.5;
        public static double WatershedSlopeMax => 64;

        /// <summary>
        /// The time of concentration field
        /// </summary>
        private double _timeOfConcentration = 0;
        public double TimeOfConcentration
        {
            get { return this._timeOfConcentration; }
            set { this.SetProperty(ref this._timeOfConcentration, value); }
        }

        public void CheckTimeOfConcentration()
        {
            if (TimeOfConcentration >= TimeOfConcentrationMin && TimeOfConcentration <= TimeOfConcentrationMax)
            {
                TimeOfConcentrationStatus = "User entered";
                return;
            }
            else if(TimeOfConcentration > TimeOfConcentrationMax)
            {
                TimeOfConcentration = TimeOfConcentrationMax;
            }
            else
            {
                TimeOfConcentration = 0;
            }

            TimeOfConcentrationStatus = "Time of concentration cannot be greater than 10.0 hours and cannot be less than 0.1 hours!";
        }

        public static double TimeOfConcentrationMin => 0.1;
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
        public string DrainageAreaStatus
        {
            get { return this._drainageStatus; }
            set { this.SetProperty(ref this._drainageStatus, value); }
        }

        private string _curveNumberStatus = "";
        public string RunoffCurveNumberStatus
        {
            get { return this._curveNumberStatus; }
            set { this.SetProperty(ref this._curveNumberStatus, value); }
        }

        private string _watershedLengthStatus = "";
        public string WatershedLengthStatus
        {
            get { return this._watershedLengthStatus; }
            set { this.SetProperty(ref this._watershedLengthStatus, value); }
        }

        private string _watershedSlopeStatus = "";
        public string WatershedSlopeStatus
        {
            get { return this._watershedSlopeStatus; }
            set { this.SetProperty(ref this._watershedSlopeStatus, value); }
        }

        private string _timeOfConcentrationStatus = "";
        public string TimeOfConcentrationStatus
        {
            get { return this._timeOfConcentrationStatus; }
            set { this.SetProperty(ref this._timeOfConcentrationStatus, value); }
        }

        public BasicDataViewModel()
        {
            Date = DateTimeOffset.Parse(DateTime.Now.ToString("MM/dd/yyyy"));
        }

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
