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
        private List<string> _duhFieldNames = new();

        /// <summary>
        /// All names of the Rainfall Distribution-Types
        /// </summary>
        private List<string> _rfTypeNames = new();

        public BasicDataViewModel BasicVM => ((Application.Current as App)?.Window as MainWindow).BasicVM;

        public RainfallDataViewModel RainfallVM => ((Application.Current as App)?.Window as MainWindow).RainfallVM;

        public RDDataPage()
        {
            this.InitializeComponent();
            
            try
            {
                using (StreamReader input = new StreamReader("C:\\ProgramData\\USDA\\Shared Engineering Data\\EFH2\\duh.txt"))
                {
                    string line = input.ReadLine();

                    while (line != "")
                    {
                        _duhFieldNames.Add(line);

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

                        _rfTypeNames.Add(splitLine[0].Trim('"'));

                        line = input.ReadLine();
                    }
                }
            }
            catch
            { 
                var messageBox = new MessageDialog("something went wrong");
            }

            ComboBoxOperations.PopulateComboBox(RainfallVM.RainfallDistributionTypes, _rfTypeNames.ToArray());
            ComboBoxOperations.PopulateComboBox(RainfallVM.DUHTypes, _duhFieldNames.ToArray());
            uxDUH.SelectedIndex = 0;
        }

        private void PlotHydrographsClick(object sender, RoutedEventArgs e)
        {

        }

        public void SetRainfallDistType(string s)
        {
            foreach (ComboBoxItem c in uxRainfallDistType.Items)
            {
                if (c.Content.ToString() == s)
                {
                    uxRainfallDistType.SelectedItem = c;
                    RainfallVM.SelectedRainfallDistributionType = s;

                    return;
                }
            }
        }

        private void uxRainfallDistType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var window = (Application.Current as App)?.Window as MainWindow;

            window.RainfallVM.SelectedRainfallDistributionType = (e.AddedItems[0] as ComboBoxItem).Content as string;
        }

        public void SetDUHType(string s)
        {
            foreach (ComboBoxItem c in uxDUH.Items)
            {
                if (c.Content.ToString() == s)
                {
                    uxDUH.SelectedItem = c;
                    RainfallVM.SelectedDUHType = s;

                    return;
                }
            }
        }

        private void uxDUH_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var window = (Application.Current as App)?.Window as MainWindow;

            window.RainfallVM.SelectedDUHType = (e.AddedItems[0] as ComboBoxItem).Content as string;
        }

        private void uxSelectHydroButton2_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ToggleSwitch t = (sender as ToggleSwitch);
        }
    }
}
