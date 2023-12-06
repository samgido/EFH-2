/* RainfallDataViewModel.cs
 * Author: Samuel Gido
 */

using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2
{
    /// <summary>
    /// A class that holds all the data in the rainfall/discharge page
    /// </summary>
    public class RainfallDataViewModel : BindableBase
    {

        public int DayRainMax => 26;

        private string _selectedRainfallDistributionType = "";
        /// <summary>
        /// Gets or sets the selected rainfall distribution type
        /// </summary>
        public string SelectedRainfallDistributionType
        {
            get { return this._selectedRainfallDistributionType; }
            set
            {
                for (int i = 0; i < RainfallDistributionTypes.Count; i++)
                {
                    if (RainfallDistributionTypes[i].Content as string == value)
                    {
                        SelectedRainfallDistributionTypeIndex = i;
                        return;
                    }
                }
                SelectedRainfallDistributionTypeIndex = 0;
            }
        }

        private int _selectedRainfallDistributionTypeIndex = 0;
        /// <summary>
        /// Gets or sets the selected index of the rainfall distribution type combo box 
        /// </summary>
        public int SelectedRainfallDistributionTypeIndex
        {
            get { return this._selectedRainfallDistributionTypeIndex; }
            set 
            { 
                this.SetProperty(ref this._selectedRainfallDistributionTypeIndex, value);
                this._selectedRainfallDistributionType = _rainfallDistributionTypes[value].Content as string;
            }
        }
        
        public void LoadRainfallDistributionTypes(StreamReader reader)
        {
            ComboBoxItem c = new();
            c.Content = "";
            RainfallDistributionTypes.Clear();
            RainfallDistributionTypes.Add(c);

            string line = reader.ReadLine();
            
            while (line != "")
            {
                string[] lineParts = line.Split(',');
                string type = lineParts[0];

                c = new();
                c.Content = type.Trim('"');

                RainfallDistributionTypes.Add(c);
                line = reader.ReadLine();
            }
            SelectedRainfallDistributionTypeIndex = 0;
        }

        private ObservableCollection<ComboBoxItem> _rainfallDistributionTypes = new();
        /// <summary>
        /// Gets or sets the collection that holds the rainfall distribution types as ComboBoxItems
        /// </summary>
        public ObservableCollection<ComboBoxItem> RainfallDistributionTypes
        {
            get { return this._rainfallDistributionTypes; }
            set { this.SetProperty(ref this._rainfallDistributionTypes, value); }
        }

        private string _selectedDUHType = "";
        /// <summary>
        /// Gets or sets the selected dimensionless unit hydrograph type
        /// </summary>
        public string SelectedDUHType
        {
            get { return this._selectedDUHType; }
            set
            {
                for(int i = 0; i < _duhTypes.Count; i++)
                {
                    if (_duhTypes[i].Content as string == value)
                    {
                        SelectedDUHTypeIndex = i;
                        return;
                    }
                }
                SelectedDUHTypeIndex = 0;
            }
        }

        private int _selectedDUHTypeIndex = 0;
        /// <summary>
        /// Gets or sets the selected index of the duh type combo box
        /// </summary>
        public int SelectedDUHTypeIndex
        {
            get { return this._selectedDUHTypeIndex; }
            set 
            { 
                this.SetProperty(ref this._selectedDUHTypeIndex, value);
                this._selectedDUHType = _duhTypes[value].Content as string;
            }
        }

        public void LoadDUHTypes(StreamReader reader)
        {
            DUHTypes.Clear();
            string line = reader.ReadLine();

            while (line != "")
            {
                ComboBoxItem c = new();
                c.Content = line;

                DUHTypes.Add(c);
                line = reader.ReadLine();
            }
            SelectedDUHTypeIndex = 0;
        }

        private ObservableCollection<ComboBoxItem> _duhTypes = new();
        /// <summary>
        /// Gets or sets the collection that holds the duh types as ComboBoxItems
        /// </summary>
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
        /// <summary>
        /// Gets or sets the current status of the rainfall distribution type field
        /// </summary>
        public string RainfallDistributionTypeStatus
        {
            get { return this._rainfallDistributionTypeStatus; }
            set { this.SetProperty(ref this._rainfallDistributionTypeStatus, value); }
        }

        private string _duhTypeStatus = "";
        /// <summary>
        /// Gets or sets the current status of the rainfall distribution type field
        /// </summary>
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

        /// <summary>
        /// Sets all fields the values their default values
        /// </summary>
        public void Default()
        {
            SelectedRainfallDistributionType = MainWindow.ChooseMessage;
            SelectedDUHType = MainWindow.ChooseMessage;

            foreach (StormModel storm in Storms)
            {
                storm.Default();
            }
        }

        /// <summary>
        /// Defaults all fields and displays a clear message to all status labels
        /// </summary>
        public void Clear()
        {
            Default();
            DUHTypeStatus = MainWindow.ClearedMessage;
            RainfallDistributionTypeStatus = MainWindow.ClearedMessage;
        }
    }
}
