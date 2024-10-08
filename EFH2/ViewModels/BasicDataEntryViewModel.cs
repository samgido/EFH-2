﻿using CommunityToolkit.Mvvm.ComponentModel;
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

        private double _value = double.NaN;
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
                        InputStatus = InputStatus.UserEnteredValue;
                        this.ValueChanged?.Invoke(this, EventArgs.Empty);
                    }
                    else if (double.IsNaN(value)) InputStatus = InputStatus.None;
                    else InputStatus = InputStatus.Invalid;
                }
            }
        }

        public double Min;

        public double Max;

        public string Name { get; private set; }

        public string InvalidEntryStatus { get; private set; }

        private InputStatus _inputStatus = InputStatus.None;

        public InputStatus InputStatus
        {
            get => _inputStatus;
            set
            {
                _inputStatus = value;
                this.OnPropertyChanged(nameof(Status));
            }
        }

        public string Status
        {
            get
            {
                switch (InputStatus)
                {
                    case InputStatus.Invalid:
                        return InvalidEntryStatus;
                    case InputStatus.UserEnteredValue:
                        return MainViewModel.UserEnteredMessage;
                    case InputStatus.Calculated:
                        return MainViewModel.CalculatedStatusMessage;
                    case InputStatus.Cleared:
                        return MainViewModel.ClearedMessage;
                    case InputStatus.FromRcnCalculator:
                        return MainViewModel.FromRcnCalculatorMessage;
                    default:
                        return "";
                }
            }
        }

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
            InputStatus = InputStatus.None;
        }

        public void SetSilent(double value)
        {
            _value = value;
            this.OnPropertyChanged(nameof(Value));
        }
    }
}
