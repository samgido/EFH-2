using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EFH2
{
    public partial class RainfallDischargeDataViewModel : ObservableObject, ICreateInputFile
    {
        public event EventHandler<EventArgs>? ValueChanged;

        [XmlElement("RainfallDistributionType")]
        public string selectedRainfallDistributionType = "";

        [XmlElement("DimensionlessUnitHydrographType")]
        public string selectedDuhType = "";

        [XmlIgnore]
        private int _selectedRainfallDistributionTypeIndex = 0;

        [XmlIgnore]
        private int _selectedDuhTypeIndex = 0;

        [XmlIgnore]
        public Dictionary<string, string> rfTypeToFileName = new Dictionary<string, string>();

        [ObservableProperty]
        [XmlElement("Storms")]
        private ObservableCollection<StormViewModel> _storms = new();

        [ObservableProperty]
        [property: XmlIgnore]
        private ObservableCollection<ComboBoxItem> _rainfallDistributionTypes = new();

        [ObservableProperty]
        [property: XmlIgnore]
        private ObservableCollection<ComboBoxItem> _duhTypes = new();

        [ObservableProperty]
        [property: XmlIgnore]
        private string _rainfallDistributionTypeStatus = "";

        [ObservableProperty]
        [property: XmlIgnore]
        private string _duhTypeStatus = "";

        [XmlIgnore]
        public static int DayRainMax => 26;

        [XmlIgnore]
        public int SelectedRainfallDistributionTypeIndex
        {
            get => this._selectedRainfallDistributionTypeIndex;
            set
            {
                this.SetProperty(ref this._selectedRainfallDistributionTypeIndex, value);
                this.selectedRainfallDistributionType = RainfallDistributionTypes[value].Content as string;

                this.ValueChanged?.Invoke(this, new EventArgs());
            }
        }

        [XmlIgnore]
        public int SelectedDuhTypeIndex
        {
            get => this._selectedDuhTypeIndex;
            set
            {
                this.SetProperty(ref this._selectedDuhTypeIndex, value);
                this.selectedDuhType = DuhTypes[value].Content as string;

                this.ValueChanged?.Invoke(this, new EventArgs());
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

        public void LoadDuhTypes(StreamReader reader)
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
            SelectedDuhTypeIndex = 0;
        }

        public void Load(RainfallDischargeDataViewModel model)
        {
            SelectedRainfallDistributionTypeIndex = model.SelectedRainfallDistributionTypeIndex;
            SelectedDuhTypeIndex = model.SelectedDuhTypeIndex;

            for (int i = 0; i < RainfallDistributionTypes.Count; i++)
            {
                string content = RainfallDistributionTypes[i].Content as string;
                if (content == model.selectedRainfallDistributionType)
                {
                    SelectedRainfallDistributionTypeIndex = i;
                    break;
                }
            }

            for (int i = 0; i < DuhTypes.Count; i++)
            {
                string content = DuhTypes[i].Content as string;
                if (content == model.selectedDuhType)
                {
                    SelectedDuhTypeIndex = i;
                    break;
                }
            }

            for (int i = 0; i < Storms.Count; i++)
            {
                Storms[i].Load(model.Storms[i]);
                Storms[i].ValueChanged += StormPropertyChanged;
            }
        }

        public void Default()
        {
            selectedRainfallDistributionType = MainViewModel.ChooseMessage;
            selectedDuhType = MainViewModel.ChooseMessage;

            foreach (StormViewModel storm in Storms)
            {
                storm.ValueChanged -= StormPropertyChanged;
                storm.Default();
            }
        }

        public void Clear()
        {
            Default();
            DuhTypeStatus = MainViewModel.ClearedMessage;
            RainfallDistributionTypeStatus = MainViewModel.ClearedMessage;
        }

        public RainfallDischargeDataViewModel()
        {
            for (int i = 0; i < MainViewModel.NumberOfStorms; i++)
            {
                StormViewModel storm = new StormViewModel() { Number = i + 1 };
                AddStorm(storm);
            }
        }

        private void AddStorm(StormViewModel storm)
        {
            storm.ValueChanged += StormPropertyChanged;
            Storms.Add(storm);
        }

        public void StormPropertyChanged(object? sender, EventArgs e)
        {
            this.ValueChanged?.Invoke(this, new EventArgs());
        }
    }
}
