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

namespace EFH2{
    public sealed partial class BasicDataControl : UserControl
    {
        public BasicDataControl()
        {
            this.InitializeComponent();
        }

        private void DrainageArea_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {

        }

        private void RunoffCurveNumber_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {

        }

        private void WatershedLength_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {

        }

        private void uxWatershedSlope_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {

        }

        private void WatershedSlope_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {

        }

        private void TimeOfConcentration_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {

        }
    }
}
