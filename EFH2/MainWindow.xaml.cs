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
using Microsoft.UI.Xaml.Printing;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH2
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private PrintManager _printManager;
        private PrintDocument _printDocument;
        private IPrintDocumentSource _printDocumentSource;
        
        public MainViewModel MainViewModel { get; set; }

        public TextBox? previousFocusedTextBox { get; set; }

        public MainWindow()
        {
            this.InitializeComponent();
            RegisterPrinting();

            Title = "EFH-2 Estimating Runoff Volume and Peak Discharge";

            ExtendsContentIntoTitleBar = true;

            MainViewModel = new MainViewModel();
            FileOperations.LoadMainViewModel(MainViewModel);

            Navigation.SelectedItem = IntroNavButton;

            BasicDataControl.DataContext = MainViewModel.BasicDataViewModel;
            BasicDataControl.SetDataContext();

            RainfallDischargeDataControl.DataContext = MainViewModel.RainfallDischargeDataViewModel;
			RainfallDischargeDataControl.CreateHydrograph += RainfallDischargeDataControl_CreateHydrograph;

            RcnDataControl.DataContext = MainViewModel.RcnDataViewModel;
            RcnDataControl.UnitsChanged += RcnDataControl_UnitsChanged;
            RcnDataControl.AcceptRcnValues += AcceptRcnValues;

            FocusManager.GotFocus += FocusManagerGotFocus;

            RcnDataControl.Visibility = Visibility.Visible;
            BasicDataControl.Visibility = Visibility.Visible;
            RainfallDischargeDataControl.Visibility = Visibility.Visible;
            IntroControl.Visibility = Visibility.Visible;
        }

		private void RainfallDischargeDataControl_CreateHydrograph(object sender, EventArgs e)
		{
            HydrographDataViewModel model = new HydrographDataViewModel(
                MainViewModel.BasicDataViewModel.selectedCounty,
                MainViewModel.BasicDataViewModel.selectedState);

            // check if any are selected to be plotted
            List<HydrographLineModel> plots = new List<HydrographLineModel>();

            bool plotGraph = false;
            foreach (StormViewModel storm in MainViewModel.RainfallDischargeDataViewModel.Storms)
            {
                if (storm.DisplayHydrograph) { plotGraph = true; break; }
            }
            if (!plotGraph) return;

            plots = FileOperations.GetHydrographData(MainViewModel.RainfallDischargeDataViewModel.Storms);
            plots.ForEach(plot => model.AddPlot(plot));

            Window newWindow = new Window();
            ShowHydrographPage page = new ShowHydrographPage() { DataContext = model };
            newWindow.Content = page;
            newWindow.Title = "Input / Output Plots";
            newWindow.Activate();

            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(newWindow);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            appWindow.Resize(new Windows.Graphics.SizeInt32 { Height = 500, Width = 650 });
        }

        private void RcnDataControl_UnitsChanged(object sender, RoutedEventArgs e)
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

        private void FocusManagerGotFocus(object sender, FocusManagerGotFocusEventArgs e)
        {
            if (e.NewFocusedElement is TextBox textBox) previousFocusedTextBox = textBox;
        }

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
            if (previousFocusedTextBox != null)
            {
                text = previousFocusedTextBox.SelectedText;
                int index = previousFocusedTextBox.Text.IndexOf(text);

                previousFocusedTextBox.Text = previousFocusedTextBox.Text.Replace(text, "");
                previousFocusedTextBox.Select(index, 0);
            }
            dataPackage.SetText(text);
            Clipboard.SetContent(dataPackage);
        }

        private void CopyClicked(object sender, RoutedEventArgs e)
        {
            DataPackage dataPackage = new();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText(previousFocusedTextBox?.SelectedText);
            Clipboard.SetContent(dataPackage);
        }

        private async void PasteClicked(object sender, RoutedEventArgs e)
        {
            DataPackageView dataPackageView = Clipboard.GetContent();

            if (dataPackageView.Contains(StandardDataFormats.Text) is true)
            {
                int index = (int)(previousFocusedTextBox?.SelectionStart);
                string replace = await dataPackageView.GetTextAsync();

                if (previousFocusedTextBox.SelectionLength == 0)
                {
                    previousFocusedTextBox.Text = previousFocusedTextBox.Text.Insert(index, replace);
                }
                else
                {
                    string initial = previousFocusedTextBox.Text;
                    initial = initial.Replace(previousFocusedTextBox.SelectedText, replace);
                    previousFocusedTextBox.Text = initial;
                }

                previousFocusedTextBox.SelectionStart = index + replace.Length;
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
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

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

        private void RegisterPrinting()
        {
			// register for printing
			var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

			_printManager = PrintManagerInterop.GetForWindow(hWnd);
			_printManager.PrintTaskRequested += _printManager_PrintTaskRequested;

			// Build a PrintDocument and register for callbacks
			_printDocument = new PrintDocument();
			_printDocumentSource = _printDocument.DocumentSource;
			_printDocument.Paginate += _printDocument_Paginate;
			_printDocument.GetPreviewPage += _printDocument_GetPreviewPage;
			_printDocument.AddPages += _printDocument_AddPages;
        }

        private async void PrintClicked(object sender, RoutedEventArgs e)
        {
            if (true) 
            { // for live xaml preview
                Page1 page = new Page1() { DataContext = new PrintableMainViewModel(MainViewModel) };
                //Page2 page = new Page2() { DataContext = new PrintableMainViewModel(MainViewModel) };

				Window newWindow = new Window();
				newWindow.Content = page;
				newWindow.Title = "Preview";
				newWindow.Activate();

				IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(newWindow);
				var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
				var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

				appWindow.Resize(new Windows.Graphics.SizeInt32 { Height = 1200, Width = 1000 });
            }
            else
            { // actual printing
				if (PrintManager.IsSupported())
				{
					try
					{
						// Show print UI
						var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
						await PrintManagerInterop.ShowPrintUIForWindowAsync(hWnd);
					}
					catch
					{
						// Printing cannot proceed at this time
						ContentDialog noPrintingDialog = new ContentDialog()
						{
							XamlRoot = (sender as Button).XamlRoot,
							Title = "Printing error",
							Content = "\nSorry, printing can't proceed at this time.",
							PrimaryButtonText = "OK"
						};
						await noPrintingDialog.ShowAsync();
					}
				}
				else
				{
					// Printing is not supported on this device
					ContentDialog noPrintingDialog = new ContentDialog()
					{
						XamlRoot = (sender as Button).XamlRoot,
						Title = "Printing not supported",
						Content = "\nSorry, printing is not supported on this device.",
						PrimaryButtonText = "OK"
					};
					await noPrintingDialog.ShowAsync();
				}
            }
        }

		private void _printDocument_AddPages(object sender, AddPagesEventArgs e)
		{
            PrintableMainViewModel model = new PrintableMainViewModel(MainViewModel);
            Page1 page1 = new Page1() { DataContext = model };
            Page2 page2 = new Page2() { DataContext = model };

            if (RcnDataViewModel.Used)
            {
                page1.ChangePageNumber();
                _printDocument.AddPage(page1);
                _printDocument.AddPage(page2);
            }
            else _printDocument.AddPage(page1);

            _printDocument.AddPagesComplete();
        }

		private void _printDocument_GetPreviewPage(object sender, GetPreviewPageEventArgs e)
		{
            //TODO: Add control to print as second parameter
            PrintableMainViewModel model = new PrintableMainViewModel(MainViewModel);
            Page1Wrapper page = new Page1Wrapper() { DataContext = model };
            Page2Wrapper page2 = new Page2Wrapper() { DataContext = model };

			_printDocument.SetPreviewPage(1, page);
            if (RcnDataViewModel.Used) _printDocument.SetPreviewPage(2, page2);
        }

        private void _printDocument_Paginate(object sender, PaginateEventArgs e)
		{
            _printDocument.SetPreviewPageCount(RcnDataViewModel.Used ? 2 : 1, PreviewPageCountType.Final);
		}

		private void _printManager_PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
		{
            var printTask = args.Request.CreatePrintTask("Print", PrintTaskSourceRequested);

			printTask.Completed += PrintTask_Completed;
		}

		private void PrintTask_Completed(PrintTask sender, PrintTaskCompletedEventArgs args)
		{
            // notify if failure
			if (args.Completion == PrintTaskCompletion.Failed)
			{
				this.DispatcherQueue.TryEnqueue(async () =>
				{
					ContentDialog noPrintingDialog = new ContentDialog()
					{
						XamlRoot = this.Content.XamlRoot,
						Title = "Printing error",
						Content = "\nSorry, failed to print.",
						PrimaryButtonText = "OK"
					};
					await noPrintingDialog.ShowAsync();
				});
			}
		}

		private void PrintTaskSourceRequested(PrintTaskSourceRequestedArgs args)
        {
            args.SetSource(_printDocumentSource);
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

		private void HelpContentsClick(object sender, RoutedEventArgs e)
		{
            HelpContentsPage page = new HelpContentsPage();         
    
			Window newWindow = new Window();
			newWindow.Content = page;
			newWindow.Title = "Preview";
			newWindow.Activate();

			IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(newWindow);
			var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
			var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
		}

		private void UserManualclick(object sender, RoutedEventArgs e)
		{
            string pdfPath = "C:\\Users\\samue\\Source\\Repos\\samgido\\EFH-2\\EFH2\\Assets\\EFH-2 Users Manual.pdf";

            Process.Start(new ProcessStartInfo(pdfPath) { UseShellExecute = true });
		}
	}
}
