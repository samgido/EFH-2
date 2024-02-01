using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH_2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SlopeCalcPage : Page
    {
        #region Properties

        private MainWindow _mainWindow = ((Application.Current as App)?.Window as MainWindow);

        /// <summary>
        /// The BasicDataViewModel of the parent, main window
        /// </summary>
        public BasicDataModel BasicVM => _mainWindow.BasicVM;

        /// <summary>
        /// The RainfallDataViewModel of the parent, main window
        /// </summary>
        public RainfallDataModel RainfallVM => _mainWindow.RainfallVM;

        #endregion

        #region Methods

        private void NumberBoxesChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (uxContourLengthBox.Value >= 0.01 && uxContourIntervalBox.Value >= 0.01 && uxDrainageAreaBox.Value >= 0.01)
            {
                double contourLength = uxContourLengthBox.Value;
                double contourInterval = uxContourIntervalBox.Value;
                double drainageArea = uxDrainageAreaBox.Value;

                double slope = (contourLength * contourInterval) / (drainageArea * 435.6);
                if (slope > 0 && slope != double.PositiveInfinity)
                {
                    BasicVM.WatershedSlope = Math.Round(slope, 2);
                }
            }
        }

        #endregion

        public SlopeCalcPage()
        {
            this.InitializeComponent();
        }
    }
}
