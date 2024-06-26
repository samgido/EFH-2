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
using Microsoft.VisualStudio.PlatformUI;
using Windows.Data.Pdf;
using Microsoft.Web.WebView2.Core;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;
using System.Windows.Forms;
using MigraDoc.DocumentObjectModel;
using TextBox = Microsoft.UI.Xaml.Controls.TextBox;
using Windows.Storage.Streams;
using Microsoft.UI.Xaml.Media.Imaging;

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
			page.CloseWindow += async (o, e) => newWindow.Close();
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

            MainViewModel.BasicDataViewModel.drainageAreaEntry.InputStatus = InputStatus.Calculated;
            MainViewModel.BasicDataViewModel.runoffCurveNumberEntry.InputStatus = InputStatus.Calculated;

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
                try
                {
					using (StreamReader reader = new StreamReader(file.Path))
					{
						reader.BaseStream.Position = 0;
						MainViewModel? model = FileOperations.DeserializeData(reader);
						if (model != null) MainViewModel.Load(model);
						// TODO - show error
					}
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        private async void SaveClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                double basicDataArea = MainViewModel.BasicDataViewModel.DrainageArea;
                double basicDataCurveNumber = MainViewModel.BasicDataViewModel.RunoffCurveNumber;

                double rcnDataArea = MainViewModel.RcnDataViewModel.AccumulatedArea;
                double rcnDataCurveNumber = MainViewModel.RcnDataViewModel.WeightedCurveNumber;

                if ((double.IsNormal(rcnDataArea) && double.IsNormal(rcnDataCurveNumber)) && !(basicDataArea.Equals(rcnDataArea) && basicDataCurveNumber.Equals(rcnDataCurveNumber))) 
                { // show dialog asking to verify data or keep
                    InconsistentDataDialogControl content = new InconsistentDataDialogControl();
					content.SetValues(MainViewModel.BasicDataViewModel.DrainageArea, MainViewModel.BasicDataViewModel.RunoffCurveNumber,
						MainViewModel.RcnDataViewModel.AccumulatedArea, MainViewModel.RcnDataViewModel.WeightedCurveNumber);

                    ContentDialog dialog = new ContentDialog()
                    {
                        XamlRoot = this.Content.XamlRoot,
                        Title = "Inconsistent data",
                        Content = content,
                        PrimaryButtonText = "Yes",
                        SecondaryButtonText = "No",
                    };

                    ContentDialogResult result = await dialog.ShowAsync();

                    if (result == ContentDialogResult.Secondary) return;
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }

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
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
        }

        private void CopyClicked(object sender, RoutedEventArgs e)
        {
            DataPackage dataPackage = new();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText(PreviousFocusedTextBox?.SelectedText);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
        }

        private async void PasteClicked(object sender, RoutedEventArgs e)
        {
            DataPackageView dataPackageView = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();

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
            System.Windows.Forms.Help.ShowHelp(null, Path.Combine(Windows.ApplicationModel.Package.Current.InstalledPath, "Assets", "Help", "EFH2.chm"));
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
                uiElements.Add(page.Plot);
                Task _ = StartPrintAsync();
            }
        }

        private async void PrintClicked(object sender, RoutedEventArgs e)
        {
            uiElements = new List<UIElement>();

			string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			string filename = appDataPath + "\\EFH2\\temp.pdf";
			bool success = PrintInfo.Print(MainViewModel, filename);

            if (!success)
            {
                ContentDialog failureDialog = new ContentDialog()
				{
					XamlRoot = this.Content.XamlRoot,
					Title = "Printing Error",
					Content = "Failed to print, please try again",
					PrimaryButtonText = "OK",
				};

                await failureDialog.ShowAsync();

                return;
            }

			PdfDocument pdfDocument = await PdfDocument.LoadFromFileAsync(await StorageFile.GetFileFromPathAsync(filename));

			for (uint i = 0; i < pdfDocument.PageCount; i++)
			{
				PdfPage pdfPage = pdfDocument.GetPage(i);

				InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
				await pdfPage.RenderToStreamAsync(stream);

				BitmapImage bitmapImage = new BitmapImage();
				await bitmapImage.SetSourceAsync(stream);

				Image image = new Image()
				{
					Source = bitmapImage,
					Width = pdfPage.Size.Width,
					Height = pdfPage.Size.Height,
				};

				uiElements.Add(image);
			}

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
            try
            {

                PrintTask printTask = args.Request.CreatePrintTask("Print", PrintTaskSourceRequested);
				printTask.Completed += PrintTaskCompleted;
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
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
            try
            {
				uiElements.ForEach(page => _printDocument.AddPage(page));

				_printDocument.AddPagesComplete();
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
		}

		private void GetPreviewPage(object sender, GetPreviewPageEventArgs e)
		{
            try
            {
				_printDocument.SetPreviewPage(e.PageNumber, uiElements[0]);
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
		}

		private void Paginate(object sender, PaginateEventArgs e)
		{
            try
            {
                _printDocument.SetPreviewPageCount(1, PreviewPageCountType.Final);
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
        }

        private void PrintTaskSourceRequested(PrintTaskSourceRequestedArgs args)
        {
            try
            {
				args.SetSource(_printDocumentSource);
			}
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
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

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //MainViewModel.BasicDataViewModel.drainageAreaEntry.Value = 500;
            //MainViewModel.BasicDataViewModel.runoffCurveNumberEntry.Value = 50;
            //MainViewModel.BasicDataViewModel.watershedLengthEntry.Value = 5000;
            //MainViewModel.BasicDataViewModel.watershedSlopeEntry.Value = 5;

            //MainViewModel.BasicDataViewModel.SelectedStateIndex = 2;
            //MainViewModel.BasicDataViewModel.SelectedCountyIndex = 2;

            try
            {
				string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				string filename = appDataPath + "\\EFH2\\temp.pdf";
                PrintInfo.Print(MainViewModel, filename);

				//var pdfDocument = await PdfDocument.LoadFromFileAsync(await StorageFile.GetFileFromPathAsync(filename));

    //            //await WebView.EnsureCoreWebView2Async();
    //            //WebView.CoreWebView2.SetVirtualHostNameToFolderMapping("pdfjs", appDataPath + "\\EFH2", CoreWebView2HostResourceAccessKind.Allow);
    //            //WebView.Source = new Uri("https://pdfjs/web/viewer.html?file=" + "C:\\Users\\samue\\AppData\\EFH2\\temp.pdf");

    //            //MainGrid.Children.Add(WebView);

    //            //WebView.CoreWebView2.NavigationCompleted += (sender, args) =>
    //            //{
    //            //	if (args.IsSuccess)
    //            //	{
    //            //		WebView.CoreWebView2.ShowPrintUI(CoreWebView2PrintDialogKind.System);
    //            //	}
    //            //}; 

    //            int currentPageIndex;
    //            PdfSharp.Pdf.PdfDocument document;

				//document = PdfReader.Open(filename, PdfDocumentOpenMode.Import);
				//currentPageIndex = 0;

				//System.Drawing.Printing.PrintDocument printDocument = new System.Drawing.Printing.PrintDocument();
    //            printDocument.PrintPage += async (sender, args) =>
    //            {
    //                if (currentPageIndex < document.PageCount)
				//	{
    //                    XGraphics gfx = XGraphics.FromPdfPage(document.Pages[currentPageIndex]);
				//		XPdfForm form = XPdfForm.FromFile(filename);
				//		form.PageNumber = currentPageIndex + 1;
				//		gfx.DrawImage(form, 0, 0, args.PageBounds.Width, args.PageBounds.Height);

				//		currentPageIndex++;
				//		args.HasMorePages = currentPageIndex < document.PageCount;
				//	}
				//	else
				//	{
				//		args.HasMorePages = false;
				//	}
				//};

				//PrintDialog printDialog = new PrintDialog();
				//if (printDialog.ShowDialog() == DialogResult.OK)
				//{
				//	printDocument.PrinterSettings = printDialog.PrinterSettings;
				//	printDocument.Print();
				//}

			}
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
		}
    }
}
