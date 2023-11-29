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

        /// <summary>
        /// How many storms are available in the Rainfall/Discharge Data sheet
        /// </summary>
        public const int NumberOfStorms = 7;

        public const string ChooseMessage = "Choose...";

        public const string ClearedMessage = "Cleared.";

        private const string _importedStatusMessage = "Imported from file";

        public static string DrainageAreaInvalidEntryMessage  = "Drainage area must be in the range 1 to 2000 acres!";
        public static string RunoffCurveNumberInvalidEntryMessage = "Curve number must be in the range 40 to 98!";
        public static string WatershedLengthInvalidEntryMessage = "Watershed length must be in the range 200 to 26000 feet!";
        public static string WatershedSlopeInvalidEntryMessage = "Watershed slope must be the range 0.5 and 64 percent!";
        public static string TimeOfConcentrationInvalidEntryMessage = "Time of concentration cannot be greater than 10.0 hours and cannot be less than 0.1 hours!";

        public TextBox? _previousFocusedTextBox { get; set; }

        /// <summary>
        /// View model for the basic data page
        /// </summary>
        public BasicDataViewModel BasicVM { get; set; }

        /// <summary>
        /// View model for the rainfall/discharge page
        /// </summary>
        public RainfallDataViewModel RainfallVM { get; set; }

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

            BasicVM = new();
            RainfallVM = new();

        }

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
                    Writer w = new(writer);
                    // version
                    w.WriteQuoted("version", true);

                    // basic data
                    // lines 2 - 12
                    foreach (object o in BasicVM.Summary)
                    {
                        w.WriteQuoted(o, true);
                    }

                    // lines 13 - 15
                    w.WriteQuoted("", true);
                    w.WriteQuoted("", true);
                    w.WriteQuoted("", true);

                    // lines 16 - 30
                    foreach (object o in RainfallVM.Summary1)
                    {
                        w.WriteQuoted(o, true);
                    }

                    // line 31 
                    w.WriteQuoted("", true);
                    
                    // lines 32 - 38
                    for (int i = 0; i < 7; i++)
                    {
                        w.WriteQuoted("", true);
                    }

                    // line 39
                    StringBuilder selectedHydrographs = new("\"     ");

                    for (int i = 0; i < MainWindow.NumberOfStorms; i++)
                    {
                        char enabled = ' ';
                        if (RainfallVM.Storms[i].DisplayHydrograph) { enabled = '*'; }

                        selectedHydrographs.Append(enabled);
                    }
                    selectedHydrographs.Append(" \"");

                    StringBuilder line38 = new();
                    line38.Append("0,0,");
                    line38.Append(selectedHydrographs);

                    w.Write(line38, true);

                    // lines 40 - 46 
                    foreach (object o in RainfallVM.Summary2)
                    {
                        w.WriteQuoted(o, true);
                    }

                }
            }

        }

        /// <summary>
        /// Opens a file dialog so user can import data to the application
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="e">Object that holds information about the event</param>
        private async void OpenClicked(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigate(typeof(RDDataPage));
            contentFrame.Navigate(typeof(BasicDataPage));
            uxNavigationView.SelectedItem = uxBasicDataPageNav;

            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.FileTypeFilter.Add(".efm");

            var window = new Microsoft.UI.Xaml.Window();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hwnd);

            StorageFile file = await openPicker.PickSingleFileAsync();

            try
            {
                if (file != null)
                {
                    using (StreamReader reader = new(file.Path))
                    {
                        Reader r = new(reader);
                        string version = r.ReadQuoted();

                        BasicVM.By = r.ReadQuoted();
                        BasicVM.Date = DateTimeOffset.Parse(r.ReadQuoted());
                        BasicVM.Client = r.ReadQuoted();
                        string county = r.ReadQuoted();
                        BasicVM.SelectedState = r.ReadQuoted();
                        BasicVM.SelectedCounty = county;
                        BasicVM.Practice = r.ReadQuoted();
                        BasicVM.DrainageArea = r.ParseInt(r.ReadQuoted());
                        BasicVM.DrainageAreaStatus = _importedStatusMessage;
                        BasicVM.RunoffCurveNumber = r.ParseFloat(r.ReadQuoted());
                        BasicVM.RunoffCurveNumberStatus = _importedStatusMessage;
                        BasicVM.WatershedLength = r.ParseInt(r.ReadQuoted());
                        BasicVM.WatershedLengthStatus = _importedStatusMessage;
                        BasicVM.WatershedSlope = r.ParseFloat(r.ReadQuoted());
                        BasicVM.WatershedSlopeStatus = _importedStatusMessage;
                        BasicVM.TimeOfConcentration = r.ParseFloat(r.ReadQuoted());
                        BasicVM.TimeOfConcentrationStatus = _importedStatusMessage;

                        // not sure what these are 
                        string line13 = r.Read();
                        string line14 = r.Read();
                        string line15 = r.Read();

                        // line 16
                        string type = r.ReadQuoted();

                        string[] types = type.Split(", ");
                        RainfallVM.SelectedRainfallDistributionType = types[0];
                        if (types.Length == 2) { RainfallVM.SelectedDUHType = types[1]; }
                        else { RainfallVM.SelectedDUHType = "<standard>"; }
                        RainfallVM.RainfallDistributionTypeStatus = _importedStatusMessage;
                        RainfallVM.DUHTypeStatus = _importedStatusMessage;

                        // lines 17 - 30
                        int[] frequencies = new int[MainWindow.NumberOfStorms];
                        float[] dayRains = new float[MainWindow.NumberOfStorms];

                        for (int i = 0; i < MainWindow.NumberOfStorms; i++)
                        {
                            RainfallVM.Storms[i].Frequency = r.ParseInt(r.ReadQuoted());
                            RainfallVM.Storms[i].DayRain = r.ParseFloat(r.ReadQuoted());
                        }

                        string line31 = r.Read();
                        string line32 = r.Read();
                        string line33 = r.Read();
                        string line34 = r.Read();
                        string line35 = r.Read();
                        string line36 = r.Read();
                        string line37 = r.Read();
                        string line38 = r.Read();

                        // Line with selected hydrograph
                        string line39 = r.Read();

                        string[] line39Split = line39.Split(',');
                        string hydrographs = line39Split[2].Trim('"');

                        for (int i = 5; i < MainWindow.NumberOfStorms + 5; i++)
                        {
                            RainfallVM.Storms[i - 5].DisplayHydrograph = hydrographs[i] == '*';
                        }

                        // lines 40 - end
                        for (int i = 0; i < MainWindow.NumberOfStorms; i++)
                        {
                            string line = r.ReadQuoted();
                            string[] splitLine = line.Split(',');

                            RainfallVM.Storms[i].PeakFlow = r.ParseDouble(splitLine[2].Trim('"'));
                            RainfallVM.Storms[i].Runoff = r.ParseDouble(splitLine[3].Trim('"'));
                        }
                    }
                }
            }
            catch (Exception err)
            {
                ContentDialog dialog = new ContentDialog();

                dialog.XamlRoot = uxRootPanel.XamlRoot;
                dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                dialog.Title = "An error occured while opening the file.";
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

                    BasicVM.Default();
                    RainfallVM.Default();
                }
            }
        }

        private void NewClicked(object sender, RoutedEventArgs e)
        {
            BasicVM.Clear();
            RainfallVM.Clear();
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

            FileOperations.PrintData(BasicVM, RainfallVM, file.Path);
        }
    }

}
