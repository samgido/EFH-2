// Author: Samuel Gido

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Printing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Pdf;
using Windows.Graphics.Printing;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using WinRT.Interop;
using TextBox = Microsoft.UI.Xaml.Controls.TextBox;
using EFH2.Models;

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

            this.AppWindow.TitleBar.ButtonForegroundColor = Colors.Black;

            Navigation.SelectedItem = IntroNavButton;

            this.MainViewModel = (App.Current as App)?.m_model;

            this.MainViewModel.WinTr20Ran += (s, e) =>
            {
                ExportHydrographsButton.IsEnabled = true;
                RainfallDischargeDataControl.PlotSelectedHydrographsButton.IsEnabled = true;
            };

            BasicDataControl.DataContext = MainViewModel.BasicDataViewModel;
            BasicDataControl.SetDataContext();

            RainfallDischargeDataControl.DataContext = MainViewModel.RainfallDischargeDataViewModel;
			RainfallDischargeDataControl.CreateHydrograph += CreateHydrograph;

            RcnDataControl.DataContext = MainViewModel.RcnDataViewModel;
            RcnDataControl.UnitsChanged += ChangeRcnUnits;
            RcnDataControl.AcceptRcnValues += AcceptRcnValues;

            MainViewModel.ChangeRcnUnits += RcnDataControl.SetUnits;

            FocusManager.GotFocus += FocusManagerGotFocus;

            RcnDataControl.Visibility = Visibility.Visible;
            BasicDataControl.Visibility = Visibility.Visible;
            RainfallDischargeDataControl.Visibility = Visibility.Visible;
            IntroControl.Visibility = Visibility.Visible;

            var root = this.Content as FrameworkElement;
            if (root != null)
            {
				root.Loaded += (s, e) => ShowWelcomePageAsync();
            }
        }

		#region Event Handlers

		private void RecalculateClick(object sender, RoutedEventArgs e)
        {
			MainViewModel.BasicDataViewModel.CalculateTimeOfConcentration();
            MainViewModel.TryWinTr20();
        }

		private async void ShowWelcomePageAsync()
        {
            WelcomePage welcomePage = new WelcomePage();
            ContentDialog welcomeDialog = new ContentDialog()
            {
                XamlRoot = this.Content.XamlRoot,
                Title = "Welcome to EFH-2",
                Content = welcomePage,
                PrimaryButtonText = "Continue",
            };

            welcomeDialog.IsPrimaryButtonEnabled = false;

            welcomeDialog.ShowAsync();

			await Task.Delay(3000);

            welcomeDialog.IsPrimaryButtonEnabled = true;
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
			page.CloseWindow += (o, e) => newWindow.Close();
            newWindow.Content = page;
            newWindow.Title = "Input / Output Plots";
            newWindow.Activate();

            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(newWindow);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
           
            appWindow.TitleBar.ButtonForegroundColor = Colors.Black;

            appWindow.Resize(new Windows.Graphics.SizeInt32 { Height = 800, Width = 1000 });
        }

		private void ChangeRcnUnits(object sender, RoutedEventArgs e)
        {
            if (MainViewModel.RcnDataViewModel.AccumulatedArea.Equals(0)) return;

            RcnDataControl.CreateUnitChangePopup(MainViewModel);
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

		private async void ExportHydrographClick(object sender, RoutedEventArgs e)
		{
            var filePicker = new FileSavePicker();
            filePicker.FileTypeChoices.Add("CSV", new List<string> { ".csv" });
            filePicker.SuggestedFileName = "hydrograph.csv";
         
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            InitializeWithWindow.Initialize(filePicker, hWnd);

            var file = await filePicker.PickSaveFileAsync();

            if (file != null)
            {
				FileOperations.ExportHydrograph(MainViewModel, file.Path); 
            }
		}

		#endregion

		#region Toolbar Button Handlers
		private void NewClicked(object sender, RoutedEventArgs e)
        {
            MainViewModel.Default();
            this.RcnDataControl.Default();
        }

        private async void OpenClicked(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker();

            var window = this;
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hwnd);

            openPicker.FileTypeFilter.Add(".xml");
            openPicker.SuggestedStartLocation = MainViewModel.defaultFileLocation;
            var file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                try
                {
					using (StreamReader reader = new StreamReader(file.Path))
					{
						reader.BaseStream.Position = 0;
						SerializedDataModel? model = FileOperations.DeserializeData(reader);
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

            var savePicker = new FileSavePicker();
			savePicker.SuggestedStartLocation = MainViewModel.defaultFileLocation;
            savePicker.FileTypeChoices.Add("XML", new List<string> { ".xml" });
            savePicker.SuggestedFileName = DateTime.Now.ToString();

            var window = this;
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);
                TextWriter writer = new StreamWriter(file.Path);
                FileOperations.SerializeData(MainViewModel, writer);
                writer.Close();
            }
        }

        private void ExitClicked(object sender, RoutedEventArgs e) => App.Current.Exit();

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
            page.Close += (o, e) => newWindow.Close();
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

		private async void HelpContentsClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string helpFilePath = Path.Combine(FileOperations.programFilesDirectory, FileOperations.companyName, "EFH2", "EFH2.chm");

                Process.Start(new ProcessStartInfo(helpFilePath) { UseShellExecute = true });
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
        }

        private void UserManualClick(object sender, RoutedEventArgs e)
		{
            string pdfPath = Path.Combine(FileOperations.programFilesDirectory, FileOperations.companyName, "EFH2", "EFH-2 Users Manual.pdf");

            Process.Start(new ProcessStartInfo(pdfPath) { UseShellExecute = true });
		}

		private async void AboutClick(object sender, RoutedEventArgs e)
		{
            ContentDialog aboutDialog = new ContentDialog()
            {
                XamlRoot = this.Content.XamlRoot,
                Title = "About EFH-2 Engineering Field Handbook, Chapter 2",
                Content = new AboutControl(),
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

	}
}
