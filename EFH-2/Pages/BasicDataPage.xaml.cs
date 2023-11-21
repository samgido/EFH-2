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

            List<string> stateAbbreviations = new();

            _stateCountyDict.Add(MainWindow.ChooseMessage, new());

            using (var reader = new StreamReader("C:\\ProgramData\\USDA\\Shared Engineering Data\\States.csv"))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    char[] seperator = { ',' };
                    string[] elements = line.Split(seperator);

                    stateAbbreviations.Add(elements[1]);
                }
            }

            for (int i = 1; i < stateAbbreviations.Count; i++)
            {
                string state = stateAbbreviations[i];

                if(state == "VIL") { state = "VI"; } 
                _stateCountyDict.Add(stateAbbreviations[i], new());
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

            stateAbbreviations.RemoveAt(0);
            ComboBoxOperations.PopulateComboBox(BasicVM.States, stateAbbreviations.ToArray());
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

        public void DrainageAreaChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            BasicVM.DrainageArea = (int)sender.Value;
            BasicVM.CheckDrainageArea();
        }

        /// <summary>
        /// Updates the status of the runoff curve number field
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="args">Object that holds information about the event</param>
        private void RunoffCurveNumberChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            BasicVM.RunoffCurveNumber = (int)sender.Value;
            BasicVM.CheckRunoffCurveNumber();
        }



        /// <summary>
        /// Updates the status of the watershed length field
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="args">Object that holds information about the event</param>
        private void WatershedLengthChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            BasicVM.WatershedLength = (int)sender.Value;
            BasicVM.CheckWatershedLength();
        }

        /// <summary>
        /// Updates the status of the watershed slope field
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="args">Object that holds information about the event</param>
        private void WatershedSlopeChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            BasicVM.WatershedSlope = (double)sender.Value;
            BasicVM.CheckWatershedSlope();
        }

        /// <summary>
        /// Updates the status of the time of contentration field
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="args">Object that holds information about the event</param>
        private void TimeOfConcentrationChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            BasicVM.TimeOfConcentration = (double)sender.Value;
            BasicVM.CheckTimeOfConcentration();
        }
    }
}
