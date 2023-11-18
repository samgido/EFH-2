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
        private string _selectedState = "";
        public string SelectedState
        {
            get { return this._selectedState; }
            set 
            { 
                this.SetProperty(ref this._selectedState, value);

                int i = 0; 
                foreach (ComboBoxItem c in _states)
                {
                    if (c.Content as string == value)
                    {
                        SelectedStateIndex = i;
                        return;
                    }
                    i++;
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
        private string _selectedCounty = "";
        public string SelectedCounty
        {
            get { return this._selectedCounty; }
            set 
            { 
                this.SetProperty(ref this._selectedCounty, value);

                int i = 0; 
                foreach (ComboBoxItem c in _counties)
                {
                    if (c.Content as string == value)
                    {
                        SelectedCountyIndex = i;
                        return;
                    }
                    i++;
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

        /// <summary>
        /// The curve number field
        /// </summary>
        private float _curveNumber = 0;
        public float CurveNumber
        {
            get { return this._curveNumber; }
            set { this.SetProperty(ref this._curveNumber, value); }
        }

        /// <summary>
        /// The watershed length field
        /// </summary>
        private int _watershedLength = 0;
        public int WatershedLength
        {
            get { return this._watershedLength; }
            set { this.SetProperty(ref this._watershedLength, value); }
        }

        /// <summary>
        /// The watershed slope field
        /// </summary>
        private float _watershedSlope = 0;
        public float WatershedSlope
        {
            get { return this._watershedSlope; }
            set { this.SetProperty(ref this._watershedSlope, value); }
        }

        /// <summary>
        /// The time of concentration field
        /// </summary>
        private float _timeOfConcentration = 0;
        public float TimeOfConcentration
        {
            get { return this._timeOfConcentration; }
            set { this.SetProperty(ref this._timeOfConcentration, value); }
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
                l.Add(CurveNumber);
                l.Add(WatershedLength);
                l.Add(WatershedSlope);
                l.Add(TimeOfConcentration);

                return l;
            }
        }

        private string _drainageStatus = "";
        public string DrainageStatus
        {
            get { return this._drainageStatus; }
            set { this.SetProperty(ref this._drainageStatus, value); }
        }

        private string _curveNumberStatus = "";
        public string CurveNumberStatus
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
    }
}
