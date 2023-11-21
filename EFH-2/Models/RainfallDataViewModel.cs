/* RainfallDataViewModel.cs
 * Author: Samuel Gido
 */

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

        public int DayRainMax => 26;
        
        /// <summary>
        /// The selected rainfall distribution type
        /// </summary>
        public string SelectedRainfallDistributionType
        {
            get { return this.SelectedRainfallDistributionType; }
            set
            {
                for (int i = 0; i < _rainfallDistributionTypes.Count; i++)
                {
                    var c = _rainfallDistributionTypes[i];
                    if (c.Content as string == value)
                    {
                        SelectedRainfallDistributionTypeIndex = i;

                        return;
                    }
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
        public string SelectedDUHType
        {
            get { return this.SelectedDUHType; }
            set
            {
                for(int i = 0; i < _duhTypes.Count; i++)
                {
                    var c = _duhTypes[i];
                    if (c.Content as string == value)
                    {
                        SelectedDUHTypeIndex = i;
                        return;
                    }
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

        public ObservableCollection<StormModel> Storms { get; set; }

        /// <summary>
        /// Summarizes the data in this page to a list of objects
        /// </summary>
        public List<object> Summary1
        {
            get
            {
                List<object> list = new();

                // Rainfall / duh type
                string type = SelectedRainfallDistributionType;
                if (SelectedDUHType.Trim() != "<standard>")
                {
                    type += ", " + SelectedDUHType;
                }
                list.Add(type);

                for (int i = 0; i < MainWindow.NumberOfStorms; i++)
                {
                    list.Add(Storms[i].Frequency);
                    list.Add(Storms[i].DayRain);
                }

                return list;
            }
        }

        public List<object> Summary2
        {
            get
            {
                List<object> list = new();

                // lines 39 - 45
                foreach(StormModel storm in Storms)
                {
                    StringBuilder line = new();

                    line.Append(storm.Frequency.ToString());
                    line.Append("\",\"");
                    line.Append(storm.DayRain.ToString());
                    line.Append("\",\"");
                    line.Append(storm.PeakFlow.ToString());
                    line.Append("\",\"");
                    line.Append(storm.Runoff.ToString());

                    list.Add(line);
                }
                
                return list;
            }
        }

        private string _rainfallDistributionTypeStatus = "";
        public string RainfallDistributionTypeStatus
        {
            get { return this._rainfallDistributionTypeStatus; }
            set { this.SetProperty(ref this._rainfallDistributionTypeStatus, value); }
        }

        private string _duhTypeStatus = "";
        public string DUHTypeStatus
        {
            get { return this._duhTypeStatus; }
            set { this.SetProperty(ref this._duhTypeStatus, value); }
        }

        public RainfallDataViewModel()
        {
            Storms = new ObservableCollection<StormModel>();

            for (int i = 0; i < MainWindow.NumberOfStorms; i++)
            {
                this.Storms.Add(new());
            }
        }

        public void Default()
        {
            SelectedRainfallDistributionType = MainWindow.ChooseMessage;
            SelectedDUHType = MainWindow.ChooseMessage;

            foreach (StormModel storm in Storms)
            {
                storm.Default();
            }
        }

        public void Clear()
        {
            Default();
            DUHTypeStatus = MainWindow.ClearedMessage;
            RainfallDistributionTypeStatus = MainWindow.ClearedMessage;
        }
    }
}
