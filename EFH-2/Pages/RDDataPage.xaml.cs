/* RDDataPage.xaml.cs
 * Author: Samuel Gido
 */

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
using Windows.Globalization.NumberFormatting;
using Windows.UI.Popups;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH_2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RDDataPage : Page
    {
        #region Properties
        #endregion

        #region Methods

        private async void ReadData()
        {
            try
            {
                if (this.DataContext is MainViewModel VM)
                {
                    using (StreamReader reader = new StreamReader("C:\\ProgramData\\USDA-dev\\Shared Engineering Data\\EFH2\\duh.txt"))
                    {
                        VM.RainfallDataViewModel.LoadDUHTypes(reader);
                    }

                    using (StreamReader reader = new StreamReader("C:\\ProgramData\\USDA-dev\\Shared Engineering Data\\EFH2\\rftype.txt"))
                    {
                        VM.RainfallDataViewModel.LoadRainfallDistributionTypes(reader);
                    }
                }
            }
            catch (Exception err)
            {
                ContentDialog dialog = new ContentDialog();

                dialog.XamlRoot = uxRootPanel.XamlRoot;
                dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                dialog.Title = "An error occurred while reading the program data.";
                dialog.CloseButtonText = "Close";
                dialog.PrimaryButtonText = "Show full error";

                var result = await dialog.ShowAsync();
        
                if (result == ContentDialogResult.Primary)
                {
                    ContentDialog fullError = new ContentDialog();

                    fullError.XamlRoot = uxRootPanel.XamlRoot;
                    fullError.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                    fullError.CloseButtonText = "Close";

                    fullError.Content = err.ToString();

                    var _ = await fullError.ShowAsync();
                }
            }
        }

        /// <summary>
        /// Updates the rainfall distribution type status when it's changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RainfallDistributionTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.DataContext is MainViewModel VM)
            {
                int selectedIndex = VM.RainfallDataViewModel.SelectedRainfallDistributionTypeIndex;

                if(selectedIndex != 0)
                {
                    VM.RainfallDataViewModel.RainfallDistributionTypeStatus = "User selected.";
                }
            }
        }

        /// <summary>
        /// Updates the DUH type status when it's changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DUHTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.DataContext is MainViewModel VM)
            {
                int selectedIndex = VM.RainfallDataViewModel.SelectedDUHTypeIndex;

                if (selectedIndex != 0)
                {
                    VM.RainfallDataViewModel.DuhTypeStatus = "User selected.";
                }
            }
        }

        #endregion

        public RDDataPage()
        {
            this.InitializeComponent();

            ReadData();
        }
    }
}
