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

            using (StreamReader reader = new StreamReader("C:\\ProgramData\\USDA\\Shared Engineering Data\\Rainfall_Data.csv"))
            {
                BasicVM.LoadStatesAndCounties(reader);
            }
        }

        public void DrainageAreaChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            BasicVM.DrainageArea = sender.Value;
            BasicVM.CheckDrainageArea();
        }

        /// <summary>
        /// Updates the status of the runoff curve number field
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="args">Object that holds information about the event</param>
        private void RunoffCurveNumberChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            BasicVM.RunoffCurveNumber = sender.Value;
            BasicVM.CheckRunoffCurveNumber();
        }

        /// <summary>
        /// Updates the status of the watershed length field
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="args">Object that holds information about the event</param>
        private void WatershedLengthChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            BasicVM.WatershedLength = sender.Value;
            BasicVM.CheckWatershedLength();
        }

        /// <summary>
        /// Updates the status of the watershed slope field
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="args">Object that holds information about the event</param>
        private void WatershedSlopeChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            BasicVM.WatershedSlope = sender.Value;
            BasicVM.CheckWatershedSlope();
        }

        /// <summary>
        /// Updates the status of the time of contentration field
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="args">Object that holds information about the event</param>
        private void TimeOfConcentrationChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            BasicVM.TimeOfConcentration = sender.Value;
            BasicVM.CheckTimeOfConcentration();
        }
    }
}
