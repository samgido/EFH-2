/* StormModel.cs
 * Author: Samuel Gido
 */

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
        private double _frequency = double.NaN;
        /// <summary>
        /// The storm's frequency field
        /// </summary>
        public double Frequency
        {
            get { return this._frequency; }
            set { this.SetProperty(ref this._frequency, value); }
        }

        private double _dayRain = double.NaN;
        /// <summary>
        /// The storm's "24-HR Rain" field
        /// </summary>
        public double DayRain
        {
            get { return this._dayRain; }
            set
            {
                if (value <= 26)
                {
                    this.SetProperty(ref this._dayRain, value);
                }
            } 
        }

        private double _peakFlow = double.NaN;
        /// <summary>
        /// The storm's peak flow field
        /// </summary>
        public double PeakFlow
        {
            get { return this._peakFlow; }
            set { this.SetProperty(ref this._peakFlow, value); }
        }

        private double _runoff = double.NaN;
        /// <summary>
        /// The storm's runoff field
        /// </summary>
        public double Runoff
        {
            get { return this._runoff; }
            set { this.SetProperty(ref this._runoff, value); }
        }

        private bool _displayHydrograph = false;
        /// <summary>
        /// Whether or not this storm will be displayed on the hydrograph
        /// </summary>
        public bool DisplayHydrograph
        {
            get { return this._displayHydrograph; }
            set { this.SetProperty(ref this._displayHydrograph, value); }
        }

        /// <summary>
        /// Sets all values to their defualt state
        /// </summary>
        public void Default()
        {
            Frequency = double.NaN;
            DayRain = double.NaN;
            PeakFlow = double.NaN;
            Runoff = double.NaN;

            DisplayHydrograph = false;
        }
    }
}
