using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
    public class MainDataModel
    {
    }

    public class BasicDataModel : INotifyPropertyChanged
    {
        private string _client = "";
        public string Client
        {
            get => _client;
            set
            {
                if (_client != value)
                {
                    _client = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _state = "";
        public string State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _county = "";
        public string County
        {
            get => _county;
            set
            {
                if (_county != value)
                {
                    _county = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _practice = "";
        public string Practice
        {
            get => _practice;
            set
            {
                if (_practice != value)
                {
                    _practice = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _by = "";
        public string By
        {
            get => _by;
            set
            {
            }
        }

        public DateTime Date = DateTime.Now;

        public double DrainageArea = double.NaN;

        public double RunoffCurveNumber = double.NaN;

        public double WatershedLength = double.NaN;

        public double WatershedSlope = double.NaN;

        public double TimeOfConcentration = double.NaN;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RainfallDischargeDataModel
    {
        public string RainfallDistributionType = "";

        public string DUHType = "";

        public List<StormModel> Storms = new List<StormModel> { };
    }

    public class RCNDataModel
    {
        public RCNGroupModel[] RCNGroups;
    }
}
