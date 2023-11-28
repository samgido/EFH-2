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
        /// <summary>
        /// All names of the dimensionless unit hydrographs
        /// </summary>
        private List<string> _duhTypeNames = new();

        /// <summary>
        /// All names of the Rainfall Distribution-Types
        /// </summary>
        private List<string> _rainfallDistributionTypeNames = new();

        /// <summary>
        /// The BasicDataViewModel of the parent, main window
        /// </summary>
        public BasicDataViewModel BasicVM => ((Application.Current as App)?.Window as MainWindow).BasicVM;

        /// <summary>
        /// The RainfallDataViewModel of the parent, main window
        /// </summary>
        public RainfallDataViewModel RainfallVM => ((Application.Current as App)?.Window as MainWindow).RainfallVM;

        public RDDataPage()
        {
            this.InitializeComponent();

            ReadData();
        }

        private async void ReadData()
        {
            try
            {
                using (StreamReader input = new StreamReader("C:\\ProgramData\\USDA\\Shared Engineering Data\\EFH2\\duh.txt"))
                {
                    string line = input.ReadLine();

                    while (line != "")
                    {
                        _duhTypeNames.Add(line);

                        line = input.ReadLine();
                    }
                }

                using (StreamReader input = new StreamReader("C:\\ProgramData\\USDA\\Shared Engineering Data\\EFH2\\rftype.txt"))
                {
                    string line = input.ReadLine();

                    while (line != "")
                    {
                        char[] sep = { ','};
                        string[] splitLine = line.Split(sep);

                        _rainfallDistributionTypeNames.Add(splitLine[0].Trim('"'));

                        line = input.ReadLine();
                    }
                }

                ComboBoxOperations.PopulateComboBox(RainfallVM.RainfallDistributionTypes, _rainfallDistributionTypeNames.ToArray());
                ComboBoxOperations.PopulateComboBox(RainfallVM.DUHTypes, _duhTypeNames.ToArray());
                uxDUHType.SelectedIndex = 0;
            }
            catch (Exception err)
            {
                ContentDialog dialog = new ContentDialog();

                dialog.XamlRoot = uxRootPanel.XamlRoot;
                dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                dialog.Title = "An error occured while reading the program data.";
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
            int selectedIndex = RainfallVM.SelectedRainfallDistributionTypeIndex;

            if(selectedIndex != 0)
            {
                RainfallVM.RainfallDistributionTypeStatus = "User selected.";
            }
        }

        /// <summary>
        /// Updates the DUH type status when it's changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DUHTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = RainfallVM.SelectedDUHTypeIndex;

            if (selectedIndex != 0)
            {
                RainfallVM.DUHTypeStatus = "User selected.";
            }
        }
    }
}
