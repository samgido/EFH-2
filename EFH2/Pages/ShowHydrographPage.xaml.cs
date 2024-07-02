using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using OxyPlot;
using Svg;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH2
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ShowHydrographPage : Page
	{
		public PlotController Controller = new PlotController();

		public event EventHandler PrintHydrograph;
		public event EventHandler CloseWindow;

		public OxyPlot.PlotView Plot => this.PlottedHydrograph;

		public ShowHydrographPage()
		{
			this.InitializeComponent();
			PlottedHydrograph.Controller = Controller;
			Controller.UnbindAll();
		}

		private async void SaveAsClick(object sender, RoutedEventArgs e)
		{
			try
			{
				var savePicker = new FileSavePicker();
				savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
				savePicker.FileTypeChoices.Add("Bitmap", new List<string> { ".bmp" });

				var hwnd = WindowNative.GetWindowHandle((Application.Current as App)?.m_window as MainWindow);
				InitializeWithWindow.Initialize(savePicker, hwnd);

				StorageFile file = await savePicker.PickSaveFileAsync();
				if (file != null)
				{
					using (var stream = new MemoryStream())
					{
						//var exporter = new OxyPlot.SvgExporter { Width = 650, Height = 450 };
						//exporter.Export(Plot.Model, stream);
						//stream.Seek(0, SeekOrigin.Begin);
						//var svgString = new StreamReader(stream).ReadToEnd();
						//File.WriteAllText(file.Path, svgString);
					}

					Bitmap image = GetBitmap();

					image.Save(file.Path);
				}

				CachedFileManager.CompleteUpdatesAsync(file);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		private void PrintPreviewClick(object sender, RoutedEventArgs e)
		{
		}

		private void PrintClick(object sender, RoutedEventArgs e)
		{
			this.PrintHydrograph?.Invoke(this, new EventArgs());
		}

		private void ExitClick(object sender, RoutedEventArgs e)
		{
			this.CloseWindow?.Invoke(this, new EventArgs());
		}

		private void CopyClick(object sender, RoutedEventArgs e)
		{
			Bitmap image = GetBitmap();
			var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
				image.GetHbitmap(),
				IntPtr.Zero,
				System.Windows.Int32Rect.Empty,
				BitmapSizeOptions.FromEmptyOptions());
			System.Windows.Clipboard.SetImage(bitmapSource);
		}

		private void ZoomInClick(object sender, RoutedEventArgs e)
		{
			Controller.UnbindAll();
			Controller.Bind(new OxyMouseDownGesture(OxyMouseButton.Left, OxyModifierKeys.None, 2), OxyPlot.PlotCommands.ZoomInAt);
			Controller.BindMouseDown(OxyMouseButton.Left, OxyPlot.PlotCommands.ZoomRectangle);
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

		private Bitmap GetBitmap()
		{
			var exporter = new OxyPlot.SvgExporter { Width = 650, Height = 450 };
			string svgString = exporter.ExportToString(Plot.Model);

			var mySvg = SvgDocument.FromSvg<SvgDocument>(svgString);

			Bitmap bitmap = mySvg.Draw();

			return bitmap;
		}
	}
}
