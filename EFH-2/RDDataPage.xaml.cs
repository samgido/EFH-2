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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH_2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RDDataPage : Page
    {
        public RDDataPage()
        {
            this.InitializeComponent();

            List<string> duhNames = new();

            try
            {
                using (StreamReader input = new("C:\\ProgramData\\USDA\\Shared Engineering Data\\EFH2\\duh.txt"))
                {
                    string? line = input.ReadLine();
                    while (!line.Equals(""))
                    {
                        char[] seperator = { ' ', '\n', '\r' };
                        string[] words = line.Split(seperator);

                        duhNames.Add(words[0]);

                        ComboBoxItem curr = new();
                        curr.Content = "words[0]";
                        uxDUH.Items.Add(curr);

                        line = input.ReadLine();
                    }

                }
            }
            catch (Exception err)
            {
            }
        }

        private void PlotHydrographs(object sender, RoutedEventArgs e)
        {

        }
    }
}
