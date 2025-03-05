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
		public event EventHandler? CreateHydrograph;

        public event EventHandler? PlotRainfallDistribution;

        public event EventHandler? PlotDuh;

        public Button PlotSelectedHydrographsButton => this.PlotHydrograph;

        public RainfallDischargeDataControl()
        {
            this.InitializeComponent();

            this.RainfallDistributionType.SelectionChanged += (sender, e) =>
            {
                this.RainfallDistributionTypePlotButton.IsEnabled = this.RainfallDistributionType.SelectedIndex != 0;
            };
        }

        private void PlotHydrographClick(object sender, RoutedEventArgs e)
        {
            this.CreateHydrograph?.Invoke(this, EventArgs.Empty);
        }

		private void PlotRainfallDistributionClick(object sender, RoutedEventArgs e)
		{
            this.PlotRainfallDistribution?.Invoke(this, EventArgs.Empty);
		}

		private void PlotDuhClick(object sender, RoutedEventArgs e)
		{
            this.PlotDuh?.Invoke(this, EventArgs.Empty);
		}
	}
}
