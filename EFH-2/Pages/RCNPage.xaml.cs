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
        public BasicDataViewModel BasicVM => _mainWindow.BasicVM;

        /// <summary>
        /// Gets the RainfallDataViewModel of the main window
        /// </summary>
        public RainfallDataViewModel RainfallVM => _mainWindow.RainfallVM;

        /// <summary>
        /// Gets the RCNDataViewModel of main window
        /// </summary>
        public RCNDataViewModel RCNVM => _mainWindow.RCNVM;

        public RCNPage()
        {
            this.InitializeComponent();

            for(int i = 0; i < 120; i++)
            {
                RowDefinition rowDef = new RowDefinition();
                uxInputGrid.RowDefinitions.Add(rowDef);
            }

            ReadRCNTableData();

            PopulateRCNData();
        }

        private void PopulateRCNData()
        {
            List<TextBlock> items = new();

            for(int i = 0; i < 120; i++)
            {
                TextBlock label = new();
                label.Text = RCNVM.RCNTableEntries[0][i];
                items.Add(label); 
            }

            for(int i = 0; i < 120; i++)
            {
                uxInputGrid.Children.Add(items[i]);
                Grid.SetColumn(items[i], 0);
                Grid.SetRow(items[i], i);
            }
        }

        private async void ReadRCNTableData()
        {
            try
            {
                using (StreamReader reader = new("C:\\Users\\samue\\Documents\\EFH-2 project\\source code\\src\\ProgramData\\EFH2\\COVER.txt"))
                {
                    RCNVM.LoadRCNTableEntries(reader);
                }
            }
            catch (Exception err)
            {
                ContentDialog dialog = new ContentDialog();

                dialog.XamlRoot = _mainWindow.Content.XamlRoot;
                dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                dialog.Title = "An error occured while reading the program data.";
                dialog.CloseButtonText = "Close";
                dialog.PrimaryButtonText = "Show full error";

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    ContentDialog fullError = new ContentDialog();

                    fullError.XamlRoot = uxRootPanel.XamlRoot;
                    fullError.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                    fullError.CloseButtonText = "Close";

                    fullError.Content = err.ToString();

                    var _ = await fullError.ShowAsync();
                }
            }
        }
    }
}
