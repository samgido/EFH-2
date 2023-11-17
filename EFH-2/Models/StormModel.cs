using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2
{
    /// <summary>
    /// Model for one storm
    /// </summary>
    public class StormModel : BindableBase
    {
        /// <summary>
        /// The storm's frequency field
        /// </summary>
        private int _frequency = 0;
        public int Frequency
        {
            get { return this._frequency; }
            set { this.SetProperty(ref this._frequency, value); }
        }

        /// <summary>
        /// The storm's "24-HR Rain" field
        /// </summary>
        private double _dayRain = 0;
        public double DayRain
        {
            get { return this._dayRain; }
            set { this.SetProperty(ref this._dayRain, value); }
        }

        /// <summary>
        /// The storm's peak flow field
        /// </summary>
        private double _peakFlow = 0;
        public double PeakFlow
        {
            get { return this._peakFlow; }
            set { this.SetProperty(ref this._peakFlow, value); }
        }

        /// <summary>
        /// The storm's runoff field
        /// </summary>
        private double _runoff = 0;
        public double Runoff
        {
            get { return this._runoff; }
            set { this.SetProperty(ref this._runoff, value); }
        }

        /// <summary>
        /// Whether or not this storm will be displayed on the hydrograph
        /// </summary>
        private bool _displayHydrograph = false;
        public bool DisplayHydrograph
        {
            get { return this._displayHydrograph; }
            set { this.SetProperty(ref this._displayHydrograph, value); }
        }
    }
}
