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
        private List<string> _duhFieldNames = new();
        private List<string> _rfTypeNames = new();

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

            PopulateComboBox(uxRainfallDistType, _rfTypeNames.ToArray());
            PopulateComboBox(uxDUH, _duhFieldNames.ToArray());
            uxDUH.SelectedValuePath = "Item1";
        }

        private void uxPlotHydrographs_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PopulateComboBox(ComboBox cb, string[] elements)
        {
            
            foreach (string s in elements)
            {
                ComboBoxItem n = new();
                n.Content = s;

                cb.Items.Add(n);
            }
        }
    }
}
