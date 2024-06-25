using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EFH2
{
    public partial class BasicDataEntryViewModel : ObservableObject, ICreateInputFile
    {
		public event EventHandler<EventArgs> ValueChanged;

        [XmlIgnore]
        private double _value = double.NaN;
        [XmlElement("Value")] 
        public double Value
        {
            get => _value;
            set
            {
                if (value != _value) // Dirty fix to stack overflow exception when calculating and setting time of conc.
                {
					this.SetProperty(ref _value, value);

					if (value <= Max && value >= Min)
					{
						Status = MainViewModel.UserEnteredMessage;
						this.ValueChanged?.Invoke(this, new EventArgs());
					}
					else if (double.IsNaN(value)) Status = "";
					else Status = InvalidEntryStatus;
                }
            }
        }

        [XmlIgnore]
        public double Min;

        [XmlIgnore]
        public double Max;

        [XmlIgnore]
        public string Name { get; private set; }

        [XmlIgnore]
        public string InvalidEntryStatus { get; private set; }

        [ObservableProperty]
        [property: XmlIgnore]
        private string _status = "";

		public BasicDataEntryViewModel()
        {
            Min = 0;
            Max = 0;
            Name = "";
            InvalidEntryStatus = "";
        }

        public BasicDataEntryViewModel(double min, double max, string name, string invalidStatus)
        {
            Min = min;
            Max = max;
            Name = name;
            InvalidEntryStatus = invalidStatus;
        }

        public void Default()
        {
            Value = double.NaN;
            Status = "";
        }

        public void SetSilent(double value)
        {
            _value = value;
            this.OnPropertyChanged(nameof(Value));
        }
    }
}
