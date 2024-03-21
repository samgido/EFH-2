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
        #region Properties

        private MainWindow _mainWindow = ((Application.Current as App)?.Window as MainWindow);

        #endregion

        #region Methods

        public void DrainageAreaChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (this.DataContext is MainViewModel VM)
            {
                VM.BasicDataViewModel.DrainageArea = sender.Value;
                VM.BasicDataViewModel.CheckDrainageArea();
            }
        }

        /// <summary>
        /// Updates the status of the runoff curve number field
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="args">Object that holds information about the event</param>
        private void RunoffCurveNumberChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (this.DataContext is MainViewModel VM)
            {
                VM.BasicDataViewModel.RunoffCurveNumber = sender.Value;
                VM.BasicDataViewModel.CheckRunoffCurveNumber();
            }
        }

        /// <summary>
        /// Updates the status of the watershed length field
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="args">Object that holds information about the event</param>
        private void WatershedLengthChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (this.DataContext is MainViewModel VM)
            {
                VM.BasicDataViewModel.WatershedLength = sender.Value;
                VM.BasicDataViewModel.CheckWatershedLength();
            }
        }

        /// <summary>
        /// Updates the status of the watershed slope field
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="args">Object that holds information about the event</param>
        private void WatershedSlopeChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (this.DataContext is MainViewModel VM)
            {
                VM.BasicDataViewModel.WatershedSlope = sender.Value;
                VM.BasicDataViewModel.CheckWatershedSlope();
            }
        }

        /// <summary>
        /// Updates the status of the time of contentration field
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="args">Object that holds information about the event</param>
        private void TimeOfConcentrationChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (this.DataContext is MainViewModel VM)
            {
                VM.BasicDataViewModel.TimeOfConcentration = sender.Value;
                VM.BasicDataViewModel.CheckTimeOfConcentration();
            }
        }

        #endregion

        public BasicDataPage()
        {
            this.InitializeComponent();

            using (StreamReader reader = new StreamReader("C:\\ProgramData\\USDA\\Shared Engineering Data\\Rainfall_Data.csv"))
            {
                if (this.DataContext is MainViewModel VM)
                {
                    VM.BasicDataViewModel.LoadStatesAndCounties(reader);
                }
            }
        }
    }
}
