using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SlopeCalculatorPage : Page
    {
        public SlopeCalculatorPage()
        {
            this.InitializeComponent();
        }

        public void SetDataContext()
        {
            if (DataContext is BasicDataViewModel model)
            {
                DrainageAreaBox.DataContext = model.drainageAreaEntry;
                AverageSlopeBox.DataContext = model.watershedSlopeEntry;
            }
        }

        private void NumberBoxesChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (DataContext is BasicDataViewModel model)
            {
                if (ContourLengthBox.Value >= 0.01 && ContourIntervalBox.Value >= 0.01 && DrainageAreaBox.Value >= 0.01)
                {
                    double contourLength = ContourLengthBox.Value;
                    double contourInterval = ContourIntervalBox.Value;
                    double drainageArea = DrainageAreaBox.Value;

                    double slope = (contourLength * contourInterval) / (drainageArea * 435.6);
                    if (slope > 0 && slope != double.PositiveInfinity)
                    {
                        model.watershedSlopeEntry.Value = Math.Round(slope, 2);
                    }
                }
            }
        }
    }
}
