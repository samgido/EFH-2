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

		private void PrintClick(object sender, RoutedEventArgs e)
		{
			//Task _ = PrintHydrographAsync();
			this.PrintHydrograph?.Invoke(this, new EventArgs());
		}

		private async Task PrintHydrographAsync()
		{
			RegisterPrinting();
			await PrintManager.ShowPrintUIAsync();
			UnregisterPrinting();
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
			var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

			_printManager = PrintManagerInterop.GetForWindow(hWnd);
			_printManager.PrintTaskRequested += _printManager_PrintTaskRequested;

			// Build a PrintDocument and register for callbacks
			_printDocument = new Microsoft.UI.Xaml.Printing.PrintDocument();
			_printDocumentSource = _printDocument.DocumentSource;
			_printDocument.Paginate += _printDocument_Paginate;
			_printDocument.GetPreviewPage += _printDocument_GetPreviewPage;
			_printDocument.AddPages += _printDocument_AddPages;
		}

		private void UnregisterPrinting()
		{
			if (_printDocument == null)
			{
				return;
			}


			_printDocument.Paginate -= _printDocument_Paginate;
			_printDocument.GetPreviewPage -= _printDocument_GetPreviewPage;
			_printDocument.AddPages -= _printDocument_AddPages;
			_printDocument = null;

			// Remove the handler for printing initialization.
			PrintManager printMan = PrintManager.GetForCurrentView();
			printMan.PrintTaskRequested -= _printManager_PrintTaskRequested;

			PrintContainer.Children.Clear();
		}

		// from  https://github.com/microsoft/Windows-universal-samples/blob/main/Samples/Printing/cs/PrintHelper.cs#L85
		private void _printManager_PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)
		{
			PrintTask printTask = null;
			printTask = e.Request.CreatePrintTask("C# Printing SDK Sample", sourceRequested =>
			{
				// Print Task event handler is invoked when the print job is completed.
				printTask.Completed += async (s, args) =>
				{
					// Notify the user when the print operation fails.
					if (args.Completion == PrintTaskCompletion.Failed)
					{
						await PrintContainer.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
						{
							//MainPage.Current.NotifyUser("Failed to print.", NotifyType.ErrorMessage);
						});
					}
				};

				sourceRequested.SetSource(_printDocumentSource);
			});
		}

		private void _printDocument_AddPages(object sender, AddPagesEventArgs e)
		{
			// page that'll be printed
			ShowHydrographPage page = new ShowHydrographPage() { DataContext = (this.DataContext as HydrographDataViewModel) };

			Microsoft.UI.Xaml.Printing.PrintDocument printDocument = (Microsoft.UI.Xaml.Printing.PrintDocument)sender;
			printDocument.AddPage(page);
			printDocument.AddPagesComplete();
		}

		private void _printDocument_GetPreviewPage(object sender, GetPreviewPageEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void _printDocument_Paginate(object sender, PaginateEventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}
