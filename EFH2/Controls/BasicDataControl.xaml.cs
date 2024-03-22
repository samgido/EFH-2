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

        private void DrainageAreaValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (DataContext is BasicDataViewModel model)
            {
                model.DrainageArea = sender.Value;
                model.CheckDrainageArea();
            }
        }

        private void RunoffCurveNumberValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (DataContext is BasicDataViewModel model)
            {
                model.RunoffCurveNumber = sender.Value;
                model.CheckRunoffCurveNumber();
            }
        }

        private void WatershedLengthValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (DataContext is BasicDataViewModel model)
            {
                model.WatershedLength = sender.Value;
                model.CheckWatershedLength();
            }
        }

        private void WatershedSlopeValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (DataContext is BasicDataViewModel model)
            {
                model.WatershedSlope = sender.Value;
                model.CheckWatershedSlope();
            }
        }

        private void TimeOfConcentrationValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (DataContext is BasicDataViewModel model)
            {
                model.TimeOfConcentration = sender.Value;
                model.CheckTimeOfConcentration();
            }
        }
    }
}
