using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace EFH2
{
    public partial class RainfallDischargeDataViewModel : ObservableObject, ICreateInputFile
    {
        public event EventHandler<EventArgs>? ValueChanged;

        public string selectedRainfallDistributionType
        {
            get
            {
                if (RainfallDistributionTypes.Count < this._selectedRainfallDistributionTypeIndex) return "Choose";
                return RainfallDistributionTypes[this._selectedRainfallDistributionTypeIndex].Content as string;
            }
        }

        public string selectedDuhType
        {
            get
            {
                if (DuhTypes.Count < this._selectedDuhTypeIndex) return "Choose";
				return DuhTypes[this._selectedDuhTypeIndex].Content as string;
			}
        }

        private int _selectedRainfallDistributionTypeIndex = 0;

        private int _selectedDuhTypeIndex = 0;

        public Dictionary<string, string> rfTypeToFileName = new Dictionary<string, string>();

        [ObservableProperty]
        private ObservableCollection<StormViewModel> _storms = new();

        [ObservableProperty]
        private ObservableCollection<ComboBoxItem> _rainfallDistributionTypes = new();

        [ObservableProperty]
        private ObservableCollection<ComboBoxItem> _duhTypes = new();

        private InputStatus _rainfallDistributionTypeInputStatus = InputStatus.None;

        public InputStatus RainfallDistributionTypeInputStatus
		{
			get => this._rainfallDistributionTypeInputStatus;
			set
			{
                this._rainfallDistributionTypeInputStatus = value;
				this.OnPropertyChanged(nameof(RainfallDistributionTypeStatus));
			}
		}

        private InputStatus _duhTypeInputStatus = InputStatus.None;

        public InputStatus DuhTypeInputStatus
		{
			get => this._duhTypeInputStatus;
			set
			{
                this._duhTypeInputStatus = value;
				this.OnPropertyChanged(nameof(DuhTypeStatus));
			}
		}

        //[ObservableProperty]
        //[property: XmlIgnore]
        //private string _rainfallDistributionTypeStatus = "";

        //[ObservableProperty]
        //[property: XmlIgnore]
        //private string _duhTypeStatus = "";

        public string RainfallDistributionTypeStatus
        {
            get
            {
                switch (_rainfallDistributionTypeInputStatus)
                {
                    case InputStatus.UserSelected:
                        return "user selected";
                    default:
						return "";
                }
            }
        }

        public string DuhTypeStatus
        {
            get
            {
                switch (_duhTypeInputStatus)
                {
                    case InputStatus.UserSelected:
                        return "user selected";
                    default:
						return "";
                }
            }
        }


        [XmlIgnore]
        public static int PrecipitationMax => 26;

        [XmlIgnore]
        public int SelectedRainfallDistributionTypeIndex
        {
            get => this._selectedRainfallDistributionTypeIndex;
            set
            {
                this.SetProperty(ref this._selectedRainfallDistributionTypeIndex, value);
                //this.selectedRainfallDistributionType = RainfallDistributionTypes[value].Content as string;
                if (value != 0) this.RainfallDistributionTypeInputStatus = InputStatus.UserSelected;
                else this.RainfallDistributionTypeInputStatus = InputStatus.None;

                this.ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [XmlIgnore]
        public int SelectedDuhTypeIndex
        {
            get => this._selectedDuhTypeIndex;
            set
            {
                this.SetProperty(ref this._selectedDuhTypeIndex, value);
                //this.selectedDuhType = DuhTypes[value].Content as string;
                if (value != 0) this.DuhTypeInputStatus = InputStatus.UserSelected;
                else this.DuhTypeInputStatus = InputStatus.None;

                this.ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetRainfallType(string type)
        {
            for (int i = 0; i < RainfallDistributionTypes.Count; i++)
            {
                if ((RainfallDistributionTypes[i].Content as string) == type)
                {
                    SelectedRainfallDistributionTypeIndex = i;
                    this.RainfallDistributionTypeInputStatus = InputStatus.None;
                }
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
                //Storms[i].SetSilent(model.Storms[i]);
                //Storms[i].ValueChanged += StormPropertyChanged;
            }

            this.OnPropertyChanged(nameof(SelectedRainfallDistributionTypeIndex));
            this.OnPropertyChanged(nameof(SelectedDuhTypeIndex));
        }

        public void Default()
        {
            SelectedRainfallDistributionTypeIndex = 0;
            SelectedDuhTypeIndex = 0;

            //selectedRainfallDistributionType = MainViewModel.ChooseMessage;
            //selectedDuhType = MainViewModel.ChooseMessage;

            foreach (StormViewModel storm in Storms)
            {
                storm.Default();
            }
        }

        public void Clear()
        {
            Default();
            this.DuhTypeInputStatus = InputStatus.Cleared;
            this.RainfallDistributionTypeInputStatus = InputStatus.Cleared;
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
            this.ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Sets all properties and only triggers the ValueChanged event once, instead of 15 times
        /// </summary>
        /// <param name="newModel"></param>
        public void SetSilent(RainfallDischargeDataViewModel newModel)
        {
            for (int i = 0; i < newModel.Storms.Count; i++)
            {
                this.Storms[i].ValueChanged -= StormPropertyChanged;
                this.Storms[i].SetSilent(newModel.Storms[i]);
                this.Storms[i].ValueChanged += StormPropertyChanged;
            }

            this._selectedDuhTypeIndex = newModel.SelectedDuhTypeIndex;
            this._selectedRainfallDistributionTypeIndex = newModel.SelectedRainfallDistributionTypeIndex;

            this.OnPropertyChanged(nameof(Storms));
            this.OnPropertyChanged(nameof(SelectedDuhTypeIndex));
            this.OnPropertyChanged(nameof(SelectedRainfallDistributionTypeIndex));

            this.ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
