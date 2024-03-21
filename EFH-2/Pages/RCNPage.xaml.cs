/* RCNPage.xaml.cs
 * Author: Samuel Gido
 */

using CommunityToolkit.WinUI.UI.Controls;
using EFH_2.Misc;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH_2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RCNPage : Page
    {
        #region Properties

        #endregion

        #region Methods

        private async void ReadRCNTableData()
        {
            try
            {
                if (this.DataContext is MainViewModel VM)
                {
                    using (StreamReader reader = new("C:\\ProgramData\\USDA-dev\\Cover.txt"))
                    {
                        VM.RCNDataViewModel.LoadRCNTableEntries(reader);
                    }
                }
            }
            catch (Exception err)
            {
                ContentDialog dialog = new()
                {
                    XamlRoot = ((Application.Current as App)?.Window as MainWindow).Content.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "An error occurred while reading the program data.",
                    CloseButtonText = "Close",
                    PrimaryButtonText = "Show full error"
                };

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    ContentDialog fullError = new()
                    {
                        XamlRoot = uxRootPanel.XamlRoot,
                        Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                        CloseButtonText = "Close",

                        Content = err.ToString()
                    };

                    var _ = await fullError.ShowAsync();
                }
            }
        }

        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is MainViewModel VM)
            {
                VM.RCNDataViewModel.Default();
            }
        }

        private void AcceptButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is MainViewModel VM)
            {
                ((Application.Current as App)?.Window as MainWindow).AcceptRCNValues(
                    (int)Math.Round(VM.RCNDataViewModel.AccumulatedArea), (int)Math.Round(VM.RCNDataViewModel.WeightedCurveNumber));
            }
        }

        #endregion

        public RCNPage()
        {
            this.InitializeComponent();

            ReadRCNTableData();
        }
    }
}
