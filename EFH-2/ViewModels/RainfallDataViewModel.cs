using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2
{
    /// <summary>
    /// View model for the rainfall/discharge page
    /// </summary>
    public class RainfallDataViewModel : BindableBase
    {
        /// <summary>
        /// The selected rainfall distribution type
        /// </summary>
        private string _selectedRainfallDistributionType = "";
        public string SelectedRainfallDistributionType
        {
            get { return this._selectedRainfallDistributionType; }
            set
            {
                this.SetProperty(ref this._selectedRainfallDistributionType, value);

                int i = 0;
                foreach (ComboBoxItem c in _rainfallDistributionTypes)
                {
                    if (c.Content as string == value)
                    {
                        SelectedRainfallDistributionTypeIndex = i;

                        return;
                    }
                    i++;
                }

                SelectedRainfallDistributionTypeIndex = 0;
            }
        }

        /// <summary>
        /// The selected index of the rainfall distribution type combo box 
        /// </summary>
        private int _selectedRainfallDistributionTypeIndex = 0;
        public int SelectedRainfallDistributionTypeIndex
        {
            get { return this._selectedRainfallDistributionTypeIndex; }
            set { this.SetProperty(ref this._selectedRainfallDistributionTypeIndex, value); }
        }

        /// <summary>
        /// Collection that holds the rainfall distribution types as ComboBoxItems
        /// </summary>
        private ObservableCollection<ComboBoxItem> _rainfallDistributionTypes = new();
        public ObservableCollection<ComboBoxItem> RainfallDistributionTypes
        {
            get { return this._rainfallDistributionTypes; }
            set { this.SetProperty(ref this._rainfallDistributionTypes, value); }
        }

        /// <summary>
        /// The selected dimensionless unit hydrograph type
        /// </summary>
        private string _selectedDUHType = "";
        public string SelectedDUHType
        {
            get { return this._selectedDUHType; }
            set
            {
                this.SetProperty(ref this._selectedDUHType, value);

                int i = 0;
                foreach (ComboBoxItem c in _duhTypes)
                {
                    if (c.Content as string == value)
                    {
                        SelectedDUHTypeIndex = i;
                        return;
                    }
                    i++;
                }

                SelectedDUHTypeIndex = 0;
            }
        }

        /// <summary>
        /// The selected index of the duh type combo box
        /// </summary>
        private int _selectedDUHTypeIndex = 0;
        public int SelectedDUHTypeIndex
        {
            get { return this._selectedDUHTypeIndex; }
            set { this.SetProperty(ref this._selectedDUHTypeIndex, value); }
        }

        /// <summary>
        /// Collection that holds the duh types as ComboBoxItems
        /// </summary>
        private ObservableCollection<ComboBoxItem> _duhTypes = new();
        public ObservableCollection<ComboBoxItem> DUHTypes
        {
            get { return this._duhTypes; }
            set { this.SetProperty(ref this._duhTypes, value); }
        }

        /// <summary>
        /// Array holding the StormModels for the page
        /// </summary>
        private StormModel[] _storms = new StormModel[MainWindow.NumberOfStorms];
        public StormModel[] Storms
        {
            get
            {
                return this._storms;
            }
        }

        public StormModel Storm1
        {
            get { return this._storms[0]; }
        }
        public StormModel Storm2
        {
            get { return this._storms[1]; }
        }
        public StormModel Storm3
        {
            get { return this._storms[2]; }
        }
        public StormModel Storm4
        {
            get { return this._storms[3]; }
        }
        public StormModel Storm5
        {
            get { return this._storms[4]; }
        }
        public StormModel Storm6
        {
            get { return this._storms[5]; }
        }
        public StormModel Storm7
        {
            get { return this._storms[6]; }
        }

        /// <summary>
        /// Summarizes the data in this page to a list of objects
        /// </summary>
        public List<object> Summary
        {
            get
            {
                List<object> list = new();

                // Rainfall / duh type
                string type = _selectedRainfallDistributionType;
                if (_selectedDUHType.Trim() != "<standard>")
                {
                    type += ", " + _selectedDUHType;
                }
                list.Add(type);

                for (int i = 0; i < MainWindow.NumberOfStorms; i++)
                {
                    list.Add(Storms[i].Frequency);
                    list.Add(Storms[i].DayRain);
                }

                list.Add("");

                for (int i = 0; i < MainWindow.NumberOfStorms; i++)
                {
                    StringBuilder s = new();
                    StormModel storm = Storms[i];

                    s.Append(storm.Frequency);
                    s.Append('"');
                    s.Append(',');
                    s.Append('"');
                    s.Append(storm.DayRain);
                    s.Append('"');
                    s.Append(',');
                    s.Append('"');
                    s.Append(storm.PeakFlow);
                    s.Append('"');
                    s.Append(',');
                    s.Append('"');
                    s.Append(storm.Runoff);

                    list.Add(s);
                }

                return list;
            }
        }

        public RainfallDataViewModel()
        {
            for (int i = 0; i < MainWindow.NumberOfStorms; i++)
            {
                this.Storms[i] = new();
            }
        }
    }
}
