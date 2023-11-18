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

        /// <summary>
        /// Available state abbreviations
        /// </summary>
        private List<string> _stateAbbreviations = new();

        /// <summary>
        /// The BasicDataViewModel of the parent, main window
        /// </summary>
        public BasicDataViewModel BasicVM => ((Application.Current as App)?.Window as MainWindow).BasicVM;

        /// <summary>
        /// The RainfallDataViewModel of the parent, main window
        /// </summary>
        public RainfallDataViewModel RainfallVM => ((Application.Current as App)?.Window as MainWindow).RainfallVM;

        public BasicDataPage()
        {
            this.InitializeComponent();

            using (var reader = new StreamReader("C:\\ProgramData\\USDA\\Shared Engineering Data\\States.csv"))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    char[] seperator = { ',' };
                    string[] elements = line.Split(seperator);

                    _stateAbbreviations.Add(elements[1]);
                }
            }

            for (int i = 1; i < _stateAbbreviations.Count; i++)
            {
                string state = _stateAbbreviations[i];

                if(state == "VIL") { state = "VI"; } 
                _stateCountyDict.Add(_stateAbbreviations[i], new());
            }

            using (var reader = new StreamReader("C:\\ProgramData\\USDA\\Shared Engineering Data\\Rainfall_Data.csv"))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    char[] seperator = { ',' };
                    string[] elements = line.Split(seperator);

                    string county = elements[2].Trim('"');

                    if (_stateCountyDict.ContainsKey(elements[1]))
                    {
                        _stateCountyDict[elements[1]].Add(county);
                    }
                }
            }

            _stateAbbreviations.RemoveAt(0);
            ComboBoxOperations.PopulateComboBox(BasicVM.States, _stateAbbreviations.ToArray());
        }

        /// <summary>
        /// Changes the contents of the county ComboBox when the state is changed
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="e">Object that holds information about the event</param>
        private void StateSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string state = (e.AddedItems[0] as ComboBoxItem).Content as string;

            ComboBoxOperations.PopulateComboBox(BasicVM.Counties, _stateCountyDict[state].ToArray());
        }

        private void DrainageAreaChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            int value = (int)sender.Value;

            if (value >= 1 && value <= 2000)
            {
                BasicVM.DrainageStatus = "User entered.";
            }
            else
            {
                BasicVM.DrainageStatus = "Drainage area must be in the range 1 to 2000 acres!";
            }
        }

        /// <summary>
        /// Updates the status of the runoff curve number field
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="args">Object that holds information about the event</param>
        private void RunoffCurveNumberChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            int value = (int)sender.Value;
            if (value >= 40 && value <= 98)
            {
                BasicVM.CurveNumberStatus = "User entered.";
            }
            else
            {
                BasicVM.CurveNumberStatus = "Curve number must be in the range 40 to 98!";
            }
        }

        /// <summary>
        /// Updates the status of the watershed length field
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="args">Object that holds information about the event</param>
        private void WatershedLengthChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            int value = (int)sender.Value;
            if (value >= 200 && value <= 26000)
            {
                BasicVM.WatershedLengthStatus = "User entered.";
            }
            else
            {
                BasicVM.WatershedLengthStatus = "Watershed length must be in the range 200 to 26000 feet!";
            }
        }

        /// <summary>
        /// Updates the status of the watershed slope field
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="args">Object that holds information about the event</param>
        private void WatershedSlopeChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            float value = (float)sender.Value;

            if (value >= .5 && value <= 64.0)
            {
                BasicVM.WatershedSlopeStatus = "User entered";
            }
            else
            {
                BasicVM.WatershedSlopeStatus = "Watershed slope must be the range 0.5 and 64 percent!";
            }
        }

        /// <summary>
        /// Updates the status of the time of contentration field
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="args">Object that holds information about the event</param>
        private void TimeOfConcentrationChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            float value = (float)sender.Value;
            if (value > .1 && value < 10.0)
            {
                BasicVM.TimeOfConcentrationStatus = "User entered";
            }
            else
            {
                BasicVM.TimeOfConcentrationStatus = "Time of concentration cannot be greater than 10.0 hours and cannot be less than 0.1 hours!";
            }
        }

    }
}
