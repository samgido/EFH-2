using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2
{
    public class Storm : BindableBase
    {

        private int _frequency = 0;
        public int Frequency
        {
            get { return this._frequency; }
            set { this.SetProperty(ref this._frequency, value); }
        }

        private double _dayRain = 0;
        public double DayRain
        {
            get { return this._dayRain; }
            set { this.SetProperty(ref this._dayRain, value); }
        }

        private double _peakFlow = 0;
        public double PeakFlow
        {
            get { return this._peakFlow; }
            set { this.SetProperty(ref this._peakFlow, value); }
        }

        private double _runoff = 0;
        public double Runoff
        {
            get { return this._runoff; }
            set { this.SetProperty(ref this._runoff, value); }
        }

        private bool _displayHydrograph = false;
        public bool DisplayHydrograph
        {
            get { return this._displayHydrograph; }
            set { this.SetProperty(ref this._displayHydrograph, value); }
        }
    }
}
