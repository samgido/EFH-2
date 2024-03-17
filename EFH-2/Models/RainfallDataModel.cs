/* RainfallDataViewModel.cs
 * Author: Samuel Gido
 */

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EFH_2
{
    [JsonObject("MemberSerialization.OptIn")]
    /// <summary>
    /// A class that holds all the data in the rainfall/discharge page
    /// </summary>
    public partial class RainfallDataModel : ObservableObject
    {

        #region Private Fields

        [JsonProperty]
        private string _selectedRainfallDistributionType = "";

        private int _selectedRainfallDistributionTypeIndex = 0;

        [JsonProperty]
        private string _selectedDUHType = "";

        private int _selectedDUHTypeIndex = 0;

        #endregion

        #region Public Fields

        [JsonProperty]
        public ObservableCollection<StormModel> Storms { get; set; }

        #endregion

        #region Observable Properties

        [ObservableProperty]
        private ObservableCollection<ComboBoxItem> _rainfallDistributionTypes = new();

        [ObservableProperty]
        private ObservableCollection<ComboBoxItem> _duhTypes = new();

        [ObservableProperty]
        private string _rainfallDistributionTypeStatus = "";

        [ObservableProperty]
        private string _duhTypeStatus = "";

        #endregion

        #region Properties

        public int DayRainMax => 26;

        /// <summary>
        /// Gets or sets the selected rainfall distribution type
        /// </summary>
        public string SelectedRainfallDistributionType
        {
            get => this._selectedRainfallDistributionType; 
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

        /// <summary>
        /// Gets or sets the selected index of the rainfall distribution type combo box 
        /// </summary>
        public int SelectedRainfallDistributionTypeIndex
        {
            get => this._selectedRainfallDistributionTypeIndex; 
            set 
            { 
                this.SetProperty(ref this._selectedRainfallDistributionTypeIndex, value);
                this._selectedRainfallDistributionType = _rainfallDistributionTypes[value].Content as string;
            }
        }

        /// <summary>
        /// Gets or sets the selected dimensionless unit hydrograph type
        /// </summary>
        public string SelectedDUHType
        {
            get => this._selectedDUHType; 
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

        /// <summary>
        /// Gets or sets the selected index of the duh type combo box
        /// </summary>
        public int SelectedDUHTypeIndex
        {
            get => this._selectedDUHTypeIndex; 
            set 
            { 
                this.SetProperty(ref this._selectedDUHTypeIndex, value);
                this._selectedDUHType = _duhTypes[value].Content as string;
            }
        }

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

        #endregion

        #region Methods

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

        public void LoadDUHTypes(StreamReader reader)
        {
            DuhTypes.Clear();
            string line = reader.ReadLine();

            while (line != "")
            {
                ComboBoxItem c = new();
                c.Content = line;

                DuhTypes.Add(c);
                line = reader.ReadLine();
            }
            SelectedDUHTypeIndex = 0;
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
            DuhTypeStatus = MainWindow.ClearedMessage;
            RainfallDistributionTypeStatus = MainWindow.ClearedMessage;
        }

        #endregion


        public RainfallDataModel()
        {
            Storms = new ObservableCollection<StormModel>();

            for (int i = 0; i < MainWindow.NumberOfStorms; i++)
            {
                this.Storms.Add(new());
            }
        }
    }
}
