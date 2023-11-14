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

        private void uxDrainageArea_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((Application.Current as App)?.Window as MainWindow).VM.DrainageArea = Int32.Parse(uxDrainageArea.Text);
        }

        private void uxRunoffCurveNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((Application.Current as App)?.Window as MainWindow).VM.CurveNumber= Int32.Parse(uxRunoffCurveNumber.Text);
        }

        private void uxWatershedLength_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((Application.Current as App)?.Window as MainWindow).VM.WatershedLength = Int32.Parse(uxWatershedLength.Text);
        }

        private void uxWatershedSlope_TextChanged(object sender, TextChangedEventArgs e)
        { 
            ((Application.Current as App)?.Window as MainWindow).VM.WatershedSlope = float.Parse(uxWatershedSlope.Text);
        }

        private void uxTimeOfConcentration_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((Application.Current as App)?.Window as MainWindow).VM.TimeOfConcentration = float.Parse(uxTimeOfConcentration.Text);
        }

        private void TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            var text = ((TextBox)sender).Text;

            var regex = new Regex("^[0-9]*$");

            if (!regex.IsMatch(text))
            {
                ((TextBox)sender).Undo();
            }
        }

        private void IsNumber()
        {

        }
    }
}
