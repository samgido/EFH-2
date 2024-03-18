/* StormModel.cs
 * Author: Samuel Gido
 */

using CommunityToolkit.Mvvm.ComponentModel;
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
    public partial class StormModel : ObservableObject
    {

        #region Private Fields

        private double _dayRain = double.NaN;

        #endregion

        #region Observable Properties

        [ObservableProperty]
        private double _frequency = double.NaN;

        [ObservableProperty]
        private double _peakFlow = double.NaN;

        [ObservableProperty]
        private double _runoff = double.NaN;

        [ObservableProperty]
        private bool _displayHydrograph = false;

        #endregion

        #region Properties

        /// <summary>
        /// The storm's "24-HR Rain" field
        /// </summary>
        public double DayRain
        {
            get => this._dayRain; 
            set
            {
                if (value == 0)
                {
                    this.SetProperty(ref this._dayRain, double.NaN);
                }
                else if (value <= 26)
                {
                    this.SetProperty(ref this._dayRain, value);
                }
            } 
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets all values to their defualt state
        /// </summary>
        public void Default()
        {
            Frequency = double.NaN;
            DayRain = 0;
            PeakFlow = double.NaN;
            Runoff = double.NaN;

            DisplayHydrograph = false;
        }

        #endregion
    }
}
