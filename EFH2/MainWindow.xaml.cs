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
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void NewClicked(object sender, RoutedEventArgs e)
        {

        }

        private void OpenClicked(object sender, RoutedEventArgs e)
        {

        }

        private void SaveClicked(object sender, RoutedEventArgs e)
        {

        }

        private void ExitClicked(object sender, RoutedEventArgs e)
        {

        }

        private void CutClicked(object sender, RoutedEventArgs e)
        {

        }

        private void CopyClicked(object sender, RoutedEventArgs e)
        {

        }

        private void PasteClicked(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleToolbar(object sender, RoutedEventArgs e)
        {

        }

        private void ShowAverageSlopeCalculator(object sender, RoutedEventArgs e)
        {

        }

        private void ShowHydrologicSoilGroups(object sender, RoutedEventArgs e)
        {

        }

        private void PrintClicked(object sender, RoutedEventArgs e)
        {

        }

        private void NavigationSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            HideControls();
            if ((NavigationViewItem)sender.SelectedItem == IntroNavButton) IntroControl.Visibility = Visibility.Visible;
            else if ((NavigationViewItem)sender.SelectedItem == BasicDataNavButton) BasicDataControl.Visibility = Visibility.Visible;
            else if ((NavigationViewItem)sender.SelectedItem == RainfallDischargeDataNavButton) RainfallDischargeDataControl.Visibility = Visibility.Visible;
            else if ((NavigationViewItem)sender.SelectedItem == RCNDataNavButton) RCNDataControl.Visibility = Visibility.Visible;
        }

        private void HideControls()
        {
            IntroControl.Visibility = Visibility.Collapsed;
            BasicDataControl.Visibility = Visibility.Collapsed;
            RainfallDischargeDataControl.Visibility = Visibility.Collapsed;
            RCNDataControl.Visibility = Visibility.Collapsed;
        }
    }
}
