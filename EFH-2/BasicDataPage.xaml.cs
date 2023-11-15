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
        private Dictionary<string, List<string>> _stateCountyDictionary = new();

        /// <summary>
        /// Available state abbreviations
        /// </summary>
        private List<string> _stateAbbreviations = new();

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
                _stateCountyDictionary.Add(_stateAbbreviations[i], new());
            }

            using (var reader = new StreamReader("C:\\ProgramData\\USDA\\Shared Engineering Data\\Rainfall_Data.csv"))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    char[] seperator = { ',' };
                    string[] elements = line.Split(seperator);

                    string county = elements[2].Trim('"');

                    if (_stateCountyDictionary.ContainsKey(elements[1]))
                    {
                        _stateCountyDictionary[elements[1]].Add(county);
                    }
                }
            }

            _stateAbbreviations.RemoveAt(0);
            ComboBoxOperations.PopulateComboBox(uxStateBox, _stateAbbreviations.ToArray());
        }


        private void StateSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string state = (e.AddedItems[0] as ComboBoxItem).Content as string;

            ComboBoxOperations.PopulateComboBox(uxCountyBox, _stateCountyDictionary[state].ToArray());

            var window = (Application.Current as App).Window as MainWindow;

            window.VM.State = state;
        }

        private void uxClientBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((Application.Current as App)?.Window as MainWindow).VM.Client = uxClientBox.Text;
        }

        private void uxCountyBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((Application.Current as App)?.Window as MainWindow).VM.County = (e.AddedItems[0] as ComboBoxItem).Content as string;
        }

        private void uxByBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((Application.Current as App)?.Window as MainWindow).VM.By = uxByBox.Text;
        }

        private void uxDatePicker_SelectedDateChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args)
        {
            ((Application.Current as App)?.Window as MainWindow).VM.Date = (DateTimeOffset)uxDatePicker.SelectedDate;
        }

        private void uxDrainageArea_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            int value = (int)sender.Value;

            if (value >= 1 && value <= 2000)
            {
                ((Application.Current as App)?.Window as MainWindow).VM.DrainageArea = value;
                uxDrainageAreaStatus.Text = "User entered.";
            }
            else
            {
                uxDrainageAreaStatus.Text = "Drainage area must be in the range 1 to 2000 acres!";
            }
        }

        private void uxRunoffCurveNumber_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            int value = (int)sender.Value;
            if (value >= 40 && value <= 98)
            {
                ((Application.Current as App)?.Window as MainWindow).VM.CurveNumber = value;
                uxRunoffCurveStatus.Text = "User entered.";
            }
            else
            {
                uxRunoffCurveStatus.Text = "Curve number must be in the range 40 to 98!";
            }
        }

        private void uxWatershedLength_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            int value = (int)sender.Value;
            if (value >= 200 && value <= 26000)
            {
                ((Application.Current as App)?.Window as MainWindow).VM.WatershedLength = value;
                uxWatershedLengthStatus.Text = "User entered.";
            }
            else
            {
                uxWatershedLengthStatus.Text = "Watershed length must be in the range 200 to 26000 feet!";
            }
        }

        private void uxWatershedSlope_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            float value = (float)sender.Value;

            if (value >= .5 && value <= 64.0)
            {
                ((Application.Current as App)?.Window as MainWindow).VM.WatershedSlope = value;
                uxWatershedSlopeStatus.Text = "User entered";
            }
            else
            {
                uxWatershedSlopeStatus.Text = "Watershed slope must be the range 0.5 and 64 percent!";
            }
        }

        private void uxTimeOfConcentration_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            float value = (float)sender.Value;
            if (value > .1 && value < 10.0)
            {
                ((Application.Current as App)?.Window as MainWindow).VM.TimeOfConcentration = value;
                uxTimeOfConcentrationStatus.Text = "User entered";
            }
            else
            {
                uxTimeOfConcentrationStatus.Text = "Time of concentration cannot be greater than 10.0 hours and cannot be less than 0.1 hours!";
            }
        }

        private void uxPracticeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((Application.Current as App)?.Window as MainWindow).VM.Practice = ((TextBox)sender).Text;
        }
    }
}
