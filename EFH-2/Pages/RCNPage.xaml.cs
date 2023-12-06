/* RCNPage.xaml.cs
 * Author: Samuel Gido
 */

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

            using (StreamReader reader = new("C:\\Users\\samue\\Documents\\EFH-2 project\\source code\\src\\ProgramData\\EFH2\\COVER.txt"))
            {
                RCNVM.LoadRCNTableEntries(reader);
            }

        }
    }
}
