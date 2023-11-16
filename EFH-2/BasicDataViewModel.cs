/* ViewModel.cs
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
    public class BasicDataViewModel : BindableBase
    {
        private string _client = "";
        public string Client
        {
            get { return this._client; }
            set { this.SetProperty(ref this._client, value); }
        }

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

        private int _selectedStateIndex = 0;
        public int SelectedStateIndex
        {
            get { return this._selectedStateIndex; }
            set { this.SetProperty(ref this._selectedStateIndex, value); }
        }

        private ObservableCollection<ComboBoxItem> _states = new();
        public ObservableCollection<ComboBoxItem> States
        {
            get { return this._states; }
            set { this.SetProperty(ref this._states, value); }
        }

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

        private int _selectedCountyIndex = 0;
        public int SelectedCountyIndex
        {
            get { return this._selectedCountyIndex; }
            set { this.SetProperty(ref this._selectedCountyIndex, value); }
        }

        private ObservableCollection<ComboBoxItem> _counties = new();
        public ObservableCollection<ComboBoxItem> Counties
        {
            get { return this._counties; }
            set { this.SetProperty(ref this._counties, value); }
        }

        private string _practice = "";
        public string Practice
        {
            get { return this._practice; }
            set { this.SetProperty(ref this._practice, value); }
        }

        private string _by = "";
        public string By
        {
            get { return this._by; }
            set { this.SetProperty(ref this._by, value); }
        }

        private DateTimeOffset _date = new();
        public DateTimeOffset Date
        {
            get { return this._date; }
            set { this.SetProperty(ref this._date, value); }
        }

        private int _drainageArea = 0;
        public int DrainageArea
        {
            get { return this._drainageArea; }
            set { this.SetProperty(ref this._drainageArea, value); }
        }

        private float _curveNumber = 0;
        public float CurveNumber
        {
            get { return this._curveNumber; }
            set { this.SetProperty(ref this._curveNumber, value); }
        }

        private int _watershedLength = 0;
        public int WatershedLength
        {
            get { return this._watershedLength; }
            set { this.SetProperty(ref this._watershedLength, value); }
        }

        private float _watershedSlope = 0;
        public float WatershedSlope
        {
            get { return this._watershedSlope; }
            set { this.SetProperty(ref this._watershedSlope, value); }
        }

        private float _timeOfConcentration = 0;
        public float TimeOfConcentration
        {
            get { return this._timeOfConcentration; }
            set { this.SetProperty(ref this._timeOfConcentration, value); }
        }

        public List<object> Summary
        {
            get
            {
                List<object> l = new();

                l.Add(By);
                l.Add(Date);
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

        private void WriteLine(object s, StreamWriter writer)
        {
            if (s == null) { s = ""; }
            writer.WriteLine('"' + s.ToString() + '"');
        }

        private void Write(object s, StreamWriter writer)
        {
            if (s == null) { s = ""; }
            writer.Write('"' + s.ToString() + '"');
        }

        public void OpenFile(string fn)
        {
            using (StreamReader reader = new StreamReader(fn))
            {
                string _ = reader.ReadLine();
                By = reader.ReadLine();
            }
        }
    }
}
