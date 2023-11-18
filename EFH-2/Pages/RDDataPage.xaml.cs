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
            }
            catch
            { 
                var messageBox = new MessageDialog("something went wrong");
            }

            ComboBoxOperations.PopulateComboBox(RainfallVM.RainfallDistributionTypes, _rainfallDistributionTypeNames.ToArray());
            ComboBoxOperations.PopulateComboBox(RainfallVM.DUHTypes, _duhTypeNames.ToArray());
            uxDUHType.SelectedIndex = 0;
        }

        /// <summary>
        /// Plots the hydrograph(s)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlotHydrographsClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
