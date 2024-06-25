// Author: Samuel Gido

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
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.ApplicationModel.DataTransfer;
using OxyPlot;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using System.Diagnostics;
using Windows.Graphics.Printing;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Helpers;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml.Printing;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH2
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public PrintManager _printManager;
        public PrintDocument _printDocument;
        public IPrintDocumentSource _printDocumentSource;

        private List<UIElement> uiElements;

		public MainViewModel MainViewModel { get; set; }

		private TextBox _previousFocusedTextBox;
        public TextBox? PreviousFocusedTextBox { get => _previousFocusedTextBox; set => _previousFocusedTextBox = value; }

        public MainWindow()
        {
            this.InitializeComponent();
            RegisterForPrinting();

            Title = "EFH-2 Estimating Runoff Volume and Peak Discharge";

            ExtendsContentIntoTitleBar = true;

            MainViewModel = new MainViewModel();
            FileOperations.LoadMainViewModel(MainViewModel);

            Navigation.SelectedItem = IntroNavButton;

            BasicDataControl.DataContext = MainViewModel.BasicDataViewModel;
            BasicDataControl.SetDataContext();

            RainfallDischargeDataControl.DataContext = MainViewModel.RainfallDischargeDataViewModel;
			RainfallDischargeDataControl.CreateHydrograph += CreateHydrograph;

            RcnDataControl.DataContext = MainViewModel.RcnDataViewModel;
            RcnDataControl.UnitsChanged += ChangeRcnUnits;
            RcnDataControl.AcceptRcnValues += AcceptRcnValues;

            FocusManager.GotFocus += FocusManagerGotFocus;

            RcnDataControl.Visibility = Visibility.Visible;
            BasicDataControl.Visibility = Visibility.Visible;
            RainfallDischargeDataControl.Visibility = Visibility.Visible;
            IntroControl.Visibility = Visibility.Visible;

            var root = this.Content as FrameworkElement;
			if (root != null)
			{
				root.Loaded += async (s, e) => await ShowWelcomePageAsync();
			}
        }

		#region Event Handlers

		private async Task ShowWelcomePageAsync()
        {
            WelcomePage welcomePage = new WelcomePage();
            ContentDialog welcomeDialog = new ContentDialog()
            {
                XamlRoot = this.Content.XamlRoot,
                Title = "Welcome to EFH-2",
                Content = welcomePage,
                PrimaryButtonText = "Continue",
            };

            await welcomeDialog.ShowAsync();
        }

		private void CreateHydrograph(object sender, EventArgs e)
		{
            HydrographDataViewModel model = new HydrographDataViewModel(
                MainViewModel.BasicDataViewModel.selectedCounty,
                MainViewModel.BasicDataViewModel.selectedState);


            // check if any are selected to be plotted
            bool plotGraph = false;
            foreach (StormViewModel storm in MainViewModel.RainfallDischargeDataViewModel.Storms)
            {
                if (storm.DisplayHydrograph) { plotGraph = true; break; }
            }
            if (!plotGraph) return;

            List<HydrographLineModel> plots = new List<HydrographLineModel>();
            plots = FileOperations.GetHydrographData(MainViewModel.RainfallDischargeDataViewModel.Storms);
            plots.ForEach(plot => model.AddPlot(plot));

            Window newWindow = new Window();
            newWindow.ExtendsContentIntoTitleBar = true;
            ShowHydrographPage page = new ShowHydrographPage() { DataContext = model };
            page.PrintHydrograph += PrintHydrograph;
            newWindow.Content = page;
            newWindow.Title = "Input / Output Plots";
            newWindow.Activate();

            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(newWindow);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            appWindow.Resize(new Windows.Graphics.SizeInt32 { Height = 800, Width = 1000 });
        }

		private void ChangeRcnUnits(object sender, RoutedEventArgs e)
        {
            RcnDataControl.CreatePopup(MainViewModel);
        }

        private void AcceptRcnValues(object sender, AcceptRcnValuesEventArgs e)
        {
            MainViewModel.BasicDataViewModel.drainageAreaEntry.Value = e.AccumulatedArea;
            MainViewModel.BasicDataViewModel.runoffCurveNumberEntry.Value = e.WeightedCurveNumber;

            MainViewModel.BasicDataViewModel.drainageAreaEntry.Status = "From RCN.";
            MainViewModel.BasicDataViewModel.runoffCurveNumberEntry.Status = "From RCN.";

            HideControls();
            BasicDataControl.Visibility = Visibility.Visible;
            Navigation.SelectedItem = BasicDataNavButton;
        }

		#endregion

		#region Toolbar Button Handlers
		private void NewClicked(object sender, RoutedEventArgs e)
        {
            MainViewModel.Default();
        }

        private async void OpenClicked(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker();

            var window = this;
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hwnd);

            openPicker.FileTypeFilter.Add(".xml");
            var file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                using (StreamReader reader = new StreamReader(file.Path))
                {
                    reader.BaseStream.Position = 0;
                    MainViewModel? model = FileOperations.DeserializeData(reader);
                    if (model != null) MainViewModel.Load(model);
                    // TODO - show error
                }
            }
        }

        private async void SaveClicked(object sender, RoutedEventArgs e)
        {
            MainViewModel.RcnDataModel = MainViewModel.RcnDataViewModel.ToRcnDataModel();

            var savePicker = new FileSavePicker();
			savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("XML", new List<string> { ".xml" });
            savePicker.SuggestedFileName = "WIP";

            var window = this;
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);
                TextWriter writer = new StreamWriter(file.Path);
                FileOperations.SerializeData(MainViewModel, writer);
                await CachedFileManager.CompleteUpdatesAsync(file);
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
            if (PreviousFocusedTextBox != null)
            {
                text = PreviousFocusedTextBox.SelectedText;
                int index = PreviousFocusedTextBox.Text.IndexOf(text);

                PreviousFocusedTextBox.Text = PreviousFocusedTextBox.Text.Replace(text, "");
                PreviousFocusedTextBox.Select(index, 0);
            }
            dataPackage.SetText(text);
            Clipboard.SetContent(dataPackage);
        }

        private void CopyClicked(object sender, RoutedEventArgs e)
        {
            DataPackage dataPackage = new();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText(PreviousFocusedTextBox?.SelectedText);
            Clipboard.SetContent(dataPackage);
        }

        private async void PasteClicked(object sender, RoutedEventArgs e)
        {
            DataPackageView dataPackageView = Clipboard.GetContent();

            if (dataPackageView.Contains(StandardDataFormats.Text) is true)
            {
                int index = (int)(PreviousFocusedTextBox?.SelectionStart);
                string replace = await dataPackageView.GetTextAsync();

                if (PreviousFocusedTextBox.SelectionLength == 0)
                {
                    PreviousFocusedTextBox.Text = PreviousFocusedTextBox.Text.Insert(index, replace);
                }
                else
                {
                    string initial = PreviousFocusedTextBox.Text;
                    initial = initial.Replace(PreviousFocusedTextBox.SelectedText, replace);
                    PreviousFocusedTextBox.Text = initial;
                }

                PreviousFocusedTextBox.SelectionStart = index + replace.Length;
            }
        }

        private void ToggleToolbar(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleMenuFlyoutItem toggle)
            {
                if (toggle.IsChecked) Toolbar.Visibility = Visibility.Visible;
                else Toolbar.Visibility = Visibility.Collapsed;
            }
        }

        private void ShowAverageSlopeCalculator(object sender, RoutedEventArgs e)
        {
            Window newWindow = new Window();
            SlopeCalculatorPage page = new SlopeCalculatorPage() { DataContext = MainViewModel.BasicDataViewModel };
            page.SetDataContext();
            newWindow.Content = page;
            newWindow.Title = "Average Slope Calculator";
            newWindow.Activate();

            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(newWindow);
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            appWindow.Resize(new Windows.Graphics.SizeInt32 { Height = 500, Width = 650 });
        }

        private async void ShowHydrologicSoilGroups(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog()
            {
                Title = "Search for Hydrologic Soil Group",
                Content = new HsgPage()
                {
                    DataContext = MainViewModel.RcnDataViewModel,
                },
                CloseButtonText = "Close",
                XamlRoot = this.Content.XamlRoot,
            };

            await dialog.ShowAsync();
        }

		private void HelpContentsClick(object sender, RoutedEventArgs e)
		{
            //ShowHelp(null, Path.Combine(Windows.ApplicationModel.Package.Current.InstalledPath, "Assets", "Help", "EFH2.chm"));
        }

        private void UserManualClick(object sender, RoutedEventArgs e)
		{
            string pdfPath = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledPath, "Assets", "EFH-2 Users Manual.pdf");

            Process.Start(new ProcessStartInfo(pdfPath) { UseShellExecute = true });
		}

		private async void AboutClick(object sender, RoutedEventArgs e)
		{
            AboutControl aboutControl = new AboutControl();
            ContentDialog aboutDialog = new ContentDialog()
            {
                XamlRoot = this.Content.XamlRoot,
                Title = "About EFH-2 Engineering Field Handbook, Chapter 2",
                Content = aboutControl,
                PrimaryButtonText = "OK",
            };

            await aboutDialog.ShowAsync();
        }

		#region Printing
		private async void PrintHydrograph(object sender, EventArgs e)
		{
            if (sender is ShowHydrographPage page)
            {
                uiElements = new List<UIElement>();
                //ShowHydrographPage p1 = new ShowHydrographPage() { DataContext = page.DataContext, Width = 600 };
                uiElements.Add(page.Plot);
                Task _ = StartPrintAsync();

                //var savePicker = new FileSavePicker();
                //savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                //savePicker.FileTypeChoices.Add("PDF", new List<string> { ".pdf" });

                //var hwnd = WindowNative.GetWindowHandle(this);
                //InitializeWithWindow.Initialize(savePicker, hwnd);

                //StorageFile file = await savePicker.PickSaveFileAsync();
                //if (file != null)
                //{
                //    CachedFileManager.DeferUpdates(file);
                //    using (Stream stream = File.Create(file.Path))
                //    {
                //        var pdfExporter = new PdfExporter { Width = 600, Height = 400 };
                //        pdfExporter.Export(page.Plot.Model, stream);
                //    }
                //}
            }
        }

        private void PrintClicked(object sender, RoutedEventArgs e)
        {
            uiElements = new List<UIElement>();

            Page1 page1 = new Page1() { DataContext = new PrintableMainViewModel(MainViewModel) };
            uiElements.Add(page1);

            Task _ = StartPrintAsync();
        }

        private async Task StartPrintAsync()
        {
            if (PrintManager.IsSupported())
            {
                try
                {
					var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
                    await PrintManagerInterop.ShowPrintUIForWindowAsync(hWnd);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            else
            {
                // TODO show content dialog to show failure
            }
        }

		public void RegisterForPrinting()
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            _printManager = PrintManagerInterop.GetForWindow(hWnd);
            _printManager.PrintTaskRequested += PrintTaskRequested;

            _printDocument = new PrintDocument();
            _printDocumentSource = _printDocument.DocumentSource;
            _printDocument.Paginate += Paginate;
            _printDocument.GetPreviewPage += GetPreviewPage;
            _printDocument.AddPages += AddPages;
        }

		private void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
		{
            var printTask = args.Request.CreatePrintTask("Print", PrintTaskSourceRequested);
            printTask.Completed += PrintTaskCompleted;
        }

		private void PrintTaskCompleted(PrintTask sender, PrintTaskCompletedEventArgs args)
		{
            if (args.Completion == PrintTaskCompletion.Failed)
            {
                this.DispatcherQueue.TryEnqueue(async () =>
                {
                    ContentDialog failureDialog = new ContentDialog()
                    {
                        XamlRoot = this.Content.XamlRoot,
                        Title = "Printing Error",
                        Content = "Failed to print",
                        PrimaryButtonText = "OK",
                    };

                    await failureDialog.ShowAsync();
                });
            }
		}

		private void AddPages(object sender, AddPagesEventArgs e)
		{
            uiElements.ForEach(page => _printDocument.AddPage(page));

            _printDocument.AddPagesComplete();
		}

		private void GetPreviewPage(object sender, GetPreviewPageEventArgs e)
		{
            _printDocument.SetPreviewPage(e.PageNumber, uiElements[0]);
		}

		private void Paginate(object sender, PaginateEventArgs e)
		{
            _printDocument.SetPreviewPageCount(1, PreviewPageCountType.Final);
        }

        private void PrintTaskSourceRequested(PrintTaskSourceRequestedArgs args)
        {
            args.SetSource(_printDocumentSource);
        }

        public void Dispose()
        {
			_printDocument.Paginate -= Paginate;
			_printDocument.GetPreviewPage -= GetPreviewPage;
			_printDocument.AddPages -= AddPages;
        }

		#endregion
		#endregion

		#region Other
		private void FocusManagerGotFocus(object sender, FocusManagerGotFocusEventArgs e)
        {
            if (e.NewFocusedElement is TextBox textBox) PreviousFocusedTextBox = textBox;
        }

		private void NavigationSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            HideControls();
            if ((NavigationViewItem)sender.SelectedItem == IntroNavButton) IntroControl.Visibility = Visibility.Visible;
            else if ((NavigationViewItem)sender.SelectedItem == BasicDataNavButton) BasicDataControl.Visibility = Visibility.Visible;
            else if ((NavigationViewItem)sender.SelectedItem == RainfallDischargeDataNavButton) RainfallDischargeDataControl.Visibility = Visibility.Visible;
            else if ((NavigationViewItem)sender.SelectedItem == RCNDataNavButton) RcnDataControl.Visibility = Visibility.Visible;

            HsgFlyout.IsEnabled = HsgButton.IsEnabled = (NavigationViewItem)sender.SelectedItem == RCNDataNavButton;
            SlopeCalculatorFlyout.IsEnabled = SlopeCalculatorButton.IsEnabled = (NavigationViewItem)sender.SelectedItem == BasicDataNavButton;
        }

        private void HideControls()
        {
            IntroControl.Visibility = Visibility.Collapsed;
            BasicDataControl.Visibility = Visibility.Collapsed;
            RainfallDischargeDataControl.Visibility = Visibility.Collapsed;
            RcnDataControl.Visibility = Visibility.Collapsed;
        }
		#endregion
	}
}
