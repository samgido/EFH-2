using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH2
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ShowHydrographPage : Page
	{
		private PrintHelper _printHelper;
		private DataTemplate customPrintTemplate;
		
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
			Task _ = PrintHydrographAsync();
		}

		private async Task PrintHydrographAsync()
		{
			try
			{
				MainGrid.Children.Remove(PlottedHydrograph); // detach so plot can be printed

				_printHelper = new PrintHelper(PrintContainer);
				_printHelper.AddFrameworkElementToPrint(PlottedHydrograph);

				_printHelper.OnPrintCanceled += _printHelper_OnPrintCanceled;
				_printHelper.OnPrintFailed += _printHelper_OnPrintFailed;
				_printHelper.OnPrintSucceeded += _printHelper_OnPrintSucceeded;

				await _printHelper.ShowPrintUIAsync("Print Hydrograph");
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		private void _printHelper_OnPrintSucceeded()
		{
			ReleasePrinter();
			Debug.WriteLine("Success printing hydrograph");
		}

		private void _printHelper_OnPrintFailed()
		{
			ReleasePrinter();
			Debug.WriteLine("Failure printing hydrograph");
		}

		private void _printHelper_OnPrintCanceled()
		{
			ReleasePrinter();
			Debug.WriteLine("Printing hydrograph canceled");
		}

		private void ReleasePrinter()
		{
			_printHelper.Dispose();
			if (!MainGrid.Children.Contains(PlottedHydrograph))
			{
				MainGrid.Children.Add(PlottedHydrograph);
				Grid.SetRow(PlottedHydrograph, 3);
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
	}
}
