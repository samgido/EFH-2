using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH2
{
    public sealed partial class RcnDataControl : UserControl
    {
        public event EventHandler<AcceptRcnValuesEventArgs>? AcceptRcnValues;

        public RcnDataControl()
        {
            this.InitializeComponent();
        }

        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
        }

        private void AcceptButtonClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is RcnDataViewModel model)
            {
                this.AcceptRcnValues?.Invoke(this, new AcceptRcnValuesEventArgs(model.AccumulatedArea, model.WeightedCurveNumber));
            }
        }
    }
}
