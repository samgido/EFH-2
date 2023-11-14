/* MainWindow.xaml.cs
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH_2
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        public MainWindow()
        {
            this.InitializeComponent();

            contentFrame.Navigate(typeof(IntroPage));
            Tabs.SelectedItem = uxIntroPageNav;

            uxSlopeCalulatorButton.IsEnabled = false;
            uxHSGButton.IsEnabled = false;
        }

        private void TabsSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            Type target = typeof(IntroPage);

            var selectedItem = (NavigationViewItem)args.SelectedItem;
            if (selectedItem != null)
            {
                string selectedItemTag = ((string)selectedItem.Tag);
                target = Type.GetType("EFH_2." + selectedItemTag);

                uxHSGButton.IsEnabled = (target == typeof(RCNPage));
                uxSlopeCalulatorButton.IsEnabled = (target == typeof(BasicDataPage));
            }

            

            contentFrame.Navigate(target);
        }

        private async void SaveClick(object sender, RoutedEventArgs e)
        {

            FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.FileTypeChoices.Add("efm", new List<string> { ".efm" });

            var window = new Microsoft.UI.Xaml.Window();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();

            if (file != null)
            {
                using (StreamWriter writer = new(file.Path))
                {
                    WriteEntry(writer, BasicDataPage.StateSelection);
                }
            }

        }
        
        private void WriteEntry(StreamWriter writer, string content)
        {
            writer.WriteLine('"' + content + '"');
        }

    }
}
