/* RCNPage.xaml.cs
 * Author: Samuel Gido
 */

using CommunityToolkit.WinUI.UI.Controls;
using EFH_2.Misc;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class RCNPage : Page
    {
        private MainWindow _mainWindow = ((Application.Current as App)?.Window as MainWindow);

        /// <summary>
        /// Gets the BasicDataViewModel of the main window
        /// </summary>
        public BasicDataModel BasicVM => _mainWindow.BasicVM;

        /// <summary>
        /// Gets the RainfallDataViewModel of the main window
        /// </summary>
        public RainfallDataModel RainfallVM => _mainWindow.RainfallVM;

        /// <summary>
        /// Gets the RCNDataViewModel of main window
        /// </summary>
        public RCNDataModel RCNVM => _mainWindow.RCNVM;

        public RCNPage()
        {
            this.InitializeComponent();

            //for(int i = 0; i < 120; i++)
            //{
            //    RowDefinition rowDef = new RowDefinition();
            //    rowDef.Height = new GridLength(30);
    
            //    uxInputGrid.RowDefinitions.Add(rowDef);
            //}

            ReadRCNTableData();
        }

        private async void ReadRCNTableData()
        {
            try
            {
                using (StreamReader reader = new("C:\\ProgramData\\USDA-dev\\Cover.txt"))
                {
                    RCNVM.LoadRCNTableEntries(reader);
                }
            }
            catch (Exception err)
            {
                ContentDialog dialog = new()
                {
                    XamlRoot = _mainWindow.Content.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "An error occurred while reading the program data.",
                    CloseButtonText = "Close",
                    PrimaryButtonText = "Show full error"
                };

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    ContentDialog fullError = new()
                    {
                        XamlRoot = uxRootPanel.XamlRoot,
                        Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                        CloseButtonText = "Close",

                        Content = err.ToString()
                    };

                    var _ = await fullError.ShowAsync();
                }
            }
        }

        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            RCNVM.Default();
        }

        private void RCNValuesChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            RCNVM.Update();
        }
    }
}
