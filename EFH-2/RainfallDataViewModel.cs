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

        private Storm[] _storms = new Storm[MainWindow._numberOfStorms];
        public Storm[] Storms
        {
            get
            {
                return this._storms;
            }
        }

        public Storm Storm1
        {
            get { return this._storms[0]; }
        }
        public Storm Storm2
        {
            get { return this._storms[1]; }
        }
        public Storm Storm3
        {
            get { return this._storms[2]; }
        }
        public Storm Storm4
        {
            get { return this._storms[3]; }
        }
        public Storm Storm5
        {
            get { return this._storms[4]; }
        }
        public Storm Storm6
        {
            get { return this._storms[5]; }
        }
        public Storm Storm7
        {
            get { return this._storms[6]; }
        }

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

        public RainfallDataViewModel()
        {
            for (int i = 0; i < MainWindow._numberOfStorms; i++)
            {
                this.Storms[i] = new();
            }
        }
    }
}
