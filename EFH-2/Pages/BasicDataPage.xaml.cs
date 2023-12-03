/* BasicDataPage.xaml.cs
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
using IronXL;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using System.Text.RegularExpressions;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH_2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BasicDataPage : Page
    {
        /// <summary>
        /// Contains each state's available counties 
        /// </summary>
        private Dictionary<string, List<string>> _stateCountyDict = new();

        private MainWindow _mainWindow = ((Application.Current as App)?.Window as MainWindow);

        /// <summary>
        /// The BasicDataViewModel of the parent, main window
        /// </summary>
        public BasicDataViewModel BasicVM => _mainWindow.BasicVM;

        /// <summary>
        /// The RainfallDataViewModel of the parent, main window
        /// </summary>
        public RainfallDataViewModel RainfallVM => _mainWindow.RainfallVM;

        public BasicDataPage()
        {
            this.InitializeComponent();

            using (var reader = new StreamReader("C:\\ProgramData\\USDA\\Shared Engineering Data\\Rainfall_Data.csv"))
            {
                BasicVM.LoadStatesAndCounties(reader);
            }
        }

        public void DrainageAreaChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            BasicVM.CheckDrainageArea();
        }

        /// <summary>
        /// Updates the status of the runoff curve number field
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="args">Object that holds information about the event</param>
        private void RunoffCurveNumberChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            BasicVM.CheckRunoffCurveNumber();
        }

        /// <summary>
        /// Updates the status of the watershed length field
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="args">Object that holds information about the event</param>
        private void WatershedLengthChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            BasicVM.CheckWatershedLength();
        }

        /// <summary>
        /// Updates the status of the watershed slope field
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="args">Object that holds information about the event</param>
        private void WatershedSlopeChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            BasicVM.CheckWatershedSlope();
        }

        /// <summary>
        /// Updates the status of the time of contentration field
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="args">Object that holds information about the event</param>
        private void TimeOfConcentrationChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            BasicVM.CheckTimeOfConcentration();
        }
    }
}
