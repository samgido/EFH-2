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
                rowDef.Height = new GridLength(25);
    
                uxInputGrid.RowDefinitions.Add(rowDef);
            }

            ReadRCNTableData();

            PopulateRCNData();
        }

        private void PopulateRCNData()
        {
            List<TextBlock>[] labeledColumns = new List<TextBlock>[7];
            for(int i = 0; i < 7; i++)
            {
                List<TextBlock> items = new();

                for(int j = 0; j < 120; j++)
                {
                    TextBlock label = new();
                    label.Text = RCNVM.RCNTableEntries[i][j];
                    items.Add(label); 
                }

                labeledColumns[i] = items;
            }

            for(int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 120; j++)
                {
                    TextBlock block = labeledColumns[i][j];
                    block.VerticalAlignment = VerticalAlignment.Center;
                    uxInputGrid.Children.Add(block);
                    Grid.SetColumn(block, i);
                    Grid.SetRow(block, j);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                int next = 0;
                for (int j = 0; j < 120; j++)
                {
                    TextBlock textBlock = labeledColumns[3 + i][j];
                    textBlock.VerticalAlignment = VerticalAlignment.Center;
                    textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                    uxInputGrid.Children.Add(textBlock);
                    Grid.SetColumn(textBlock, 4 + i * 2);
                    Grid.SetRow(textBlock, j);

                    if(labeledColumns[3+i][j].Text.Trim() != "" && labeledColumns[3 + i][j].Text.Trim() != "**")
                    {
                        NumberBox inputBox = new();
                        uxInputGrid.Children.Add(inputBox);
                        Grid.SetColumn(inputBox, 3 + i * 2);
                        Grid.SetRow(inputBox, j);

                        //Binding b = new();
                        //b.Mode = BindingMode.TwoWay;
                        //if ( i == 0)
                        //{
                        //    b.Source = RCNVM.GroupAInputs[next];
                        //}
                        //inputBox.SetBinding(NumberBox.ValueProperty, b);

                        next++;
                    }
                }
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

        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            BasicVM.DrainageArea = 5.0;
            RCNVM.GroupAInputs[0] = 5.0;
        }
    }
}
