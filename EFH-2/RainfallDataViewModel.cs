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
            set { this.SetProperty(ref this._selectedRainfallDistributionType, value); }
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
            set { this.SetProperty(ref this._selectedDUHType, value); }
        }

        private ObservableCollection<ComboBoxItem> _duhTypes = new();
        public ObservableCollection<ComboBoxItem> DUHTypes
        {
            get { return this._duhTypes; }
            set { this.SetProperty(ref this._duhTypes, value); }
        }

        public int[] _freq = new int[MainWindow._numberOfStorms];

        public float[] _dayRain = new float[MainWindow._numberOfStorms];

        public float[] _peakFlow = new float[MainWindow._numberOfStorms];

        public float[] _runoff = new float[MainWindow._numberOfStorms];

        public bool[] _selectedGraphs = new bool[MainWindow._numberOfStorms];

    }
}
