using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using EFH_2.Misc;
using System.Collections.ObjectModel;
using static EFH_2.RCNDataViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH_2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HSGPage : Page
    {

        private MainWindow _mainWindow = ((Application.Current as App)?.Window as MainWindow);

        /// <summary>
        /// The BasicDataViewModel of the parent, main window
        /// </summary>
        public BasicDataViewModel BasicVM => _mainWindow.BasicVM;

        /// <summary>
        /// The RainfallDataViewModel of the parent, main window
        /// </summary>
        public RainfallDataViewModel RainfallVM => _mainWindow.RainfallVM;

        public RCNDataViewModel RCNVM => _mainWindow.RCNVM;

        public HSGPage()
        {
            this.InitializeComponent();


            using (StreamReader reader = new("C:\\Users\\samue\\Documents\\EFH-2 project\\source code\\src\\Source code\\SOILS.hg"))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    string[] lineParts = line.Split("\t");

                    RCNVM.AddHSGEntry(lineParts[0], lineParts[1], lineParts[2]);
                }
            }
        }

        private void SearchBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = (sender as TextBox).Text.ToUpper();


            uxDataGrid.ItemsSource = new ObservableCollection<HSGEntry>(
                from item in RCNVM.HSGEntries where item.Column1.Contains(filter) select item);
        }
    }
}
