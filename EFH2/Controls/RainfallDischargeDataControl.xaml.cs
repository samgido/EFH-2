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
    public sealed partial class RainfallDischargeDataControl : UserControl
    {
        public RainfallDischargeDataControl()
        {
            this.InitializeComponent();
        }

        private void DUHTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is RainfallDischargeDataViewModel model)
            {
                if (model.SelectedRainfallDistributionTypeIndex != 0) model.RainfallDistributionTypeStatus = "User selected.";
            }
        }

        private void RainfallDistributionTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is RainfallDischargeDataViewModel model)
            {
                if (model.SelectedDuhTypeIndex != 0) model.DuhTypeStatus = "User selected.";
            }
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
