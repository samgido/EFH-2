using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2
{
    public class RainfallDataViewModel : BindableBase
    {
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

        private int _selectedRainfallDistributionTypeIndex = 0;
        public int SelectedRainfallDistributionTypeIndex
        {
            get { return this._selectedRainfallDistributionTypeIndex; }
            set { this.SetProperty(ref this._selectedRainfallDistributionTypeIndex, value); }
        }

        private ObservableCollection<ComboBoxItem> _rainfallDistributionTypes = new();
        public ObservableCollection<ComboBoxItem> RainfallDistributionTypes
        {
            get { return this._rainfallDistributionTypes; }
            set { this.SetProperty(ref this._rainfallDistributionTypes, value); }
        }

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

        private int _selectedDUHTypeIndex = 0;
        public int SelectedDUHTypeIndex
        {
            get { return this._selectedDUHTypeIndex; }
            set { this.SetProperty(ref this._selectedDUHTypeIndex, value); }
        }

        private ObservableCollection<ComboBoxItem> _duhTypes = new();
        public ObservableCollection<ComboBoxItem> DUHTypes
        {
            get { return this._duhTypes; }
            set { this.SetProperty(ref this._duhTypes, value); }
        }

        private int[] _frequency = new int[MainWindow._numberOfStorms];
        public int[] Frequency
        {
            get { return this._frequency; }
            set { this.SetProperty(ref this._frequency, value); }
        }

        public Storm _storm1 = new();
        public Storm _storm2 = new();
        public Storm _storm3 = new();
        public Storm _storm4 = new();
        public Storm _storm5 = new();
        public Storm _storm6 = new();
        public Storm _storm7 = new();

        public bool[] _selectedGraphs = new bool[MainWindow._numberOfStorms];

        public List<object> Summary
        {
            get
            {
                List<object> list = new();

                string type = _selectedRainfallDistributionType;
                if (_selectedDUHType != "")
                {
                    type += ", " + _selectedDUHType;
                }

                list.Add(type);

                return list;
            }
        }
    }
}
