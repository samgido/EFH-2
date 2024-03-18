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
using Microsoft.UI.Windowing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Popups;
using WinRT;
using Microsoft.UI;
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using EFH_2.Misc;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH_2
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        #region Private Fields
        #endregion

        #region Public Fields

        /// <summary>
        /// How many storms are available in the Rainfall/Discharge Data sheet
        /// </summary>
        public const int NumberOfStorms = 9;

        public const string ChooseMessage = "Choose...";

        public const string ClearedMessage = "Cleared.";

        private const string _importedStatusMessage = "Imported from file";

        public static string DrainageAreaInvalidEntryMessage  = "Drainage area must be in the range 1 to 2000 acres!";
        public static string RunoffCurveNumberInvalidEntryMessage = "Curve number must be in the range 40 to 98!";
        public static string WatershedLengthInvalidEntryMessage = "Watershed length must be in the range 200 to 26000 feet!";
        public static string WatershedSlopeInvalidEntryMessage = "Watershed slope must be the range 0.5 and 64 percent!";
        public static string TimeOfConcentrationInvalidEntryMessage = "Time of concentration cannot be greater than 10.0 hours and cannot be less than 0.1 hours!";

        #endregion

        #region Observable Properties
        #endregion

        #region Properties

        public TextBox? _previousFocusedTextBox { get; set; }

        public Models.MainDataModel MainModel { get; set; }

        #endregion

        #region Methods

        private void FocusManager_GotFocus(object sender, FocusManagerGotFocusEventArgs e)
        {
            if (e.NewFocusedElement is TextBox textBox)
            {
                _previousFocusedTextBox = textBox;
            }
        }

        private void MainWindowActivated(object sender, WindowActivatedEventArgs args)
        {
            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.SetIcon(@"Assets\EFH2.ico");
        }

        /// <summary>
        /// Changes the contents of contentFrame to whichever page the user selects
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="args">Object that holds information about the event</param>
        private void NavigationViewSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            Type target = typeof(IntroPage);

            var selectedItem = (NavigationViewItem)args.SelectedItem;
            if (selectedItem != null)
            {
                string selectedItemTag = ((string)selectedItem.Tag);
                target = Type.GetType("EFH_2." + selectedItemTag);

                uxHSGButton.IsEnabled = (target == typeof(RCNPage));
                uxHSGFlyout.IsEnabled = (target == typeof(RCNPage));
                uxAverageSlopeCalulatorButton.IsEnabled = (target == typeof(BasicDataPage));
                uxAverageSlopeCalculatorFlyout.IsEnabled = (target == typeof(BasicDataPage));
            }

            contentFrame.Navigate(target);
        }

        /// <summary>
        /// Saves the data currently stored in the application to a .efm file 
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="e">Object that holds information about the event</param>
        private async void SaveClicked(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Opens a file dialog so user can import data to the application
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="e">Object that holds information about the event</param>
        private async void OpenClicked(object sender, RoutedEventArgs e)
        

        private void NewClicked(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigate(typeof(RDDataPage));
            contentFrame.Navigate(typeof(BasicDataPage));
            uxNavigationView.SelectedItem = uxNavigationView.MenuItems[1];

            BasicDataViewModel.Clear();
            RainfallDataViewModel.Clear();
        }

        private void ToggleToolbarClicked(object sender, RoutedEventArgs e)
        {
            ToggleMenuFlyoutItem toggle = (ToggleMenuFlyoutItem)sender;    

            if (toggle.IsChecked)
            {
                uxToolbar.Visibility = Visibility.Visible;
            }
            else
            {
                uxToolbar.Visibility = Visibility.Collapsed;
            }
        }

        private void ExitClicked(object sender, RoutedEventArgs e)
        {
            App.Current.Exit();
        }

        private void CutClicked(object sender, RoutedEventArgs e)
        {
            DataPackage dataPackage = new();
            dataPackage.RequestedOperation = DataPackageOperation.Move;

            string text = "";
            if (_previousFocusedTextBox != null)
            {
                text = _previousFocusedTextBox.SelectedText;
                int index = _previousFocusedTextBox.Text.IndexOf(text);

                _previousFocusedTextBox.Text = _previousFocusedTextBox.Text.Replace(text, "");
                _previousFocusedTextBox.Select(index, 0);
            }
            dataPackage.SetText(text);
            Clipboard.SetContent(dataPackage);
        }

        private void CopyClicked(object sender, RoutedEventArgs e)
        {
            DataPackage dataPackage = new();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText(_previousFocusedTextBox.SelectedText);
            Clipboard.SetContent(dataPackage);
        }

        private async void PasteClicked(object sender, RoutedEventArgs e)
        {
            DataPackageView dataPackageView = Clipboard.GetContent();

            if (dataPackageView.Contains(StandardDataFormats.Text) is true)
            {
                int index = _previousFocusedTextBox.SelectionStart;
                string replace = await dataPackageView.GetTextAsync();

                if (_previousFocusedTextBox.SelectionLength == 0)
                {
                    _previousFocusedTextBox.Text = _previousFocusedTextBox.Text.Insert(index, replace);
                }
                else
                {
                    string initial = _previousFocusedTextBox.Text;
                    initial = initial.Replace(_previousFocusedTextBox.SelectedText, replace);

                    _previousFocusedTextBox.Text = initial;
                }
                _previousFocusedTextBox.SelectionStart = index + replace.Length;
            }
        }

        private async void PrintClicked(object sender, RoutedEventArgs e)
        {
            FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.FileTypeChoices.Add("pdf", new List<string> { ".pdf" });

            var window = new Microsoft.UI.Xaml.Window();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();

            FileOperations.PrintData(BasicDataModel, RainfallDataModel, file.Path);
        }

        private void ShowSlopeCalculatorClick(object sender, RoutedEventArgs e)
        {
            Window newWindow = new Window();

            newWindow.Content = new SlopeCalcPage();
            newWindow.Title = "Average Slope Calculator";
            newWindow.Activate();

            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(newWindow);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            appWindow.Resize(new Windows.Graphics.SizeInt32 { Height = 500, Width = 650 });

        }

        private async void HSGSearchClicked(object sender, RoutedEventArgs e)
        {
            HSGPage hsgPage = new();

            ContentDialog dialog = new ContentDialog()
            {
                Title = "Search for Hydrologic Soil Group",
                Content = hsgPage,
                CloseButtonText = "Close",
                XamlRoot = this.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }

        public void AcceptRCNValues(int totalArea, int curveNumber)
        {
            BasicDataViewModel.AcceptRCNValues(totalArea, curveNumber);

            contentFrame.Navigate(typeof(BasicDataPage));
            uxNavigationView.SelectedItem = uxNavigationView.MenuItems[1];
        }

        #endregion

        public MainWindow()
        {
            this.InitializeComponent();
            this.Activated += MainWindowActivated;
            FocusManager.GotFocus += FocusManager_GotFocus;

            this.Title = "EFH-2 Estimating Runoff Volume and Peak Discharge";
            this.AppWindow.SetIcon("C:\\Users\\samue\\Source\\Repos\\samgido\\EFH - 2\\EFH - 2\\ProgramData\\EFH2.ico");

            contentFrame.Navigate(typeof(IntroPage));
            uxNavigationView.SelectedItem = uxIntroPageNav;

            uxAverageSlopeCalulatorButton.IsEnabled = false;
            uxHSGButton.IsEnabled = false;
            uxToolbarToggle.IsChecked = true;

            Models.MainDataModel mainModel = new Models.MainDataModel();
            MainModel = mainModel;

            RootPanel.DataContext = MainModel;
        }
    }
}
