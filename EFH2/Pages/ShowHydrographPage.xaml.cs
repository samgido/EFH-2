using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Printing;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Printing;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH2
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ShowHydrographPage : Page
	{
        private PrintManager _printManager;
        private Microsoft.UI.Xaml.Printing.PrintDocument _printDocument;
        private IPrintDocumentSource _printDocumentSource;

		public event EventHandler? PrintHydrograph;

		public PlotController Controller = new PlotController();

		public ShowHydrographPage()
		{
			this.InitializeComponent();
			PlottedHydrograph.Controller = Controller;
			Controller.UnbindAll();
		}

		private void SaveAsClick(object sender, RoutedEventArgs e)
		{

		}

		private void PrintPreviewClick(object sender, RoutedEventArgs e)
		{
		}

		private async void PrintClick(object sender, RoutedEventArgs e)
		{
			//Task _ = PrintHydrographAsync();
			//this.PrintHydrograph?.Invoke(this, new EventArgs());

            if (PrintManager.IsSupported())
            {
                try
                {
					var window = (Application.Current as App)?.m_window as MainWindow;
					var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
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

		private void ExitClick(object sender, RoutedEventArgs e)
		{

		}

		private void CopyClick(object sender, RoutedEventArgs e)
		{

		}

		private void ZoomInClick(object sender, RoutedEventArgs e)
		{
			Controller.UnbindAll();
			Controller.Bind(new OxyMouseDownGesture(OxyMouseButton.Left, OxyModifierKeys.None, 2), PlotCommands.ZoomInAt);
			Controller.BindMouseDown(OxyMouseButton.Left, PlotCommands.ZoomRectangle);
		}

		private void ZoomOutClick(object sender, RoutedEventArgs e)
		{
			Controller.UnbindAll();
			PlottedHydrograph.ResetAllAxes();
		}

		private void UsePointerClick(object sender, RoutedEventArgs e)
		{
			Controller.UnbindAll();	
		}

		private void PlotPointsToggleClick(object sender, RoutedEventArgs e)
		{

		}

		private void PlotLinesToggleClick(object sender, RoutedEventArgs e)
		{

		}

		private void PlotGridToggleClick(object sender, RoutedEventArgs e)
		{

		}

		private void PlotView_PointerPressed(object sender, PointerRoutedEventArgs e)
		{

		}

		private void PlotSettingsChanged(object sender, RoutedEventArgs e)
		{
			if (DataContext is HydrographDataViewModel model)
			{
				model.ChangeSettings(plotPointsRadioButton.IsChecked, plotLinesRadioButton.IsChecked, plotGridRadioButton.IsChecked);

				try
				{
					PlottedHydrograph.InvalidatePlot();
				}
				catch (Exception ex) { Debug.WriteLine(ex); }
			}
		}

		private void RegisterPrinting()
		{
			// register for printing
			//var window = (Application.Current as App)?.m_window as MainWindow;
			var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

			_printManager = PrintManagerInterop.GetForWindow(hWnd);
			_printManager.PrintTaskRequested += PrintTaskRequested;

			// Build a PrintDocument and register for callbacks
			_printDocument = new Microsoft.UI.Xaml.Printing.PrintDocument();
			_printDocumentSource = _printDocument.DocumentSource;
			_printDocument.Paginate += Paginate;
			_printDocument.GetPreviewPage += GetPreviewPage;
			_printDocument.AddPages += AddPages;
		}

		private void AddPages(object sender, AddPagesEventArgs e)
		{
			Page page = new Page();
			//page.Content = new PlotView() { DataContext = this.DataContext };
			page.Content = new TextBlock() { Text = "HEllo" };

			_printDocument.AddPage(page);
			_printDocument.AddPagesComplete();
		}

		private void GetPreviewPage(object sender, GetPreviewPageEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void Paginate(object sender, PaginateEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
		{
            var printTask = args.Request.CreatePrintTask("Print", PrintTaskSourceRequested);
			printTask.Completed += Completed;
		}

		private void Completed(PrintTask sender, PrintTaskCompletedEventArgs args)
		{
			Dispose();
		}

		private void PrintTaskSourceRequested(PrintTaskSourceRequestedArgs args) => args.SetSource(_printDocumentSource);

        public void Dispose()
        {
            Task _ = DispatcherQueue.EnqueueAsync(() =>
            {
                _printDocument.Paginate -= Paginate;
                _printDocument.GetPreviewPage -= GetPreviewPage;
                _printDocument.AddPages -= AddPages;
            });
        }
	}
}
