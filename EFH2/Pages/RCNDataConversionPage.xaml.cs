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

namespace EFH2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RcnDataConversionPage : Page
    {
        public RcnDataConversionPage()
        {
            this.InitializeComponent();
        }

        public void SetDirection(bool acresSelected)
        {
            if (DataContext is MainViewModel model)
            {
                UseWorksheetSelection.Content = "Determine percentage from worksheet total area of " + model.RcnDataViewModel.AccumulatedArea;

                UseWorksheetSelection.IsEnabled = !acresSelected;
                UseBasicDataSelection.IsEnabled = acresSelected;

                if (acresSelected)
                {
                    Title.Text = "Convert Percent to Acres";
                    Prompt.Text = "How would you like to convert the values in the worksheet from percent to acres?";

                    UseWorksheetSelection.IsEnabled = false;

                    if (!model.BasicDataViewModel.drainageAreaEntry.Value.Equals(double.NaN))
                    {
                        UseBasicDataSelection.IsEnabled = true;
                        UseBasicDataSelection.Content = "Determine area by percentage of Basic Data drainage area of " + model.BasicDataViewModel.drainageAreaEntry.Value;
                    }
                    else
                    {
                        UseBasicDataSelection.IsEnabled = false;
                        UseBasicDataSelection.Content = "Determine area by percentage of Basic Data drainage area of ";
                    }
                }
                else
                {
                    Title.Text = "Convert Acres to Percent";
                    Prompt.Text = "How would you like to convert the values in the worksheet from acres to percent?";

                    UseBasicDataSelection.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton && DataContext is MainViewModel model)
            {
                
            }
        }
    }
}
