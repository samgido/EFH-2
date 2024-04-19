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
    public sealed partial class BasicDataControl : UserControl
    {
        public BasicDataControl()
        {
            this.InitializeComponent();
        }

        public void SetDataContext()
        {
            if (DataContext is BasicDataViewModel model)
            {
                DrainageAreaControls.DataContext = model.drainageAreaEntry;
                RunoffCurveNumberControls.DataContext = model.runoffCurveNumberEntry;
                WatershedLengthControls.DataContext = model.watershedLengthEntry;
                WatershedSlopeControls.DataContext = model.watershedSlopeEntry;
                TimeOfConcentrationControls.DataContext = model.timeOfConcentrationEntry;
            }
        }

        private void BasicDataFieldValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (DataContext is BasicDataViewModel viewModel)
            {
                if (sender.DataContext is BasicDataEntryViewModel model)
                {
                    viewModel.CheckFieldChange(model, sender.Value);
                    CalculateTimeOfConcentration();
                }
            }
        }

        private void CalculateTimeOfConcentration()
        {
            if (DataContext is BasicDataViewModel model)
            {
                double final = (Math.Pow(model.WatershedLength, 0.8) * Math.Pow(((1000 / model.RunoffCurveNumber) - 10) + 1, 0.7)) / (1140 * Math.Pow(model.WatershedSlope, 0.5));

                if (final.Equals(double.NaN))
                {
                    model.timeOfConcentrationEntry.Value = double.NaN;
                    model.timeOfConcentrationEntry.Status = "";
                }
                else
                {
                    model.timeOfConcentrationEntry.Value = Math.Round(final, 2);
                    model.timeOfConcentrationEntry.Status = "Calculated";
                }
            }
        }
    }
}
