using Microsoft.UI.Xaml;
//using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using OxyPlot;
using Svg;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Interop;
using System.Windows.Media;
using Windows.Graphics.Imaging;

//using System.Windows.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;
using Image = Microsoft.UI.Xaml.Controls.Image;

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

		public event EventHandler PrintHydrographPdf;
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
				savePicker.FileTypeChoices.Add("PNG", new List<string> { ".png" });

				var hwnd = WindowNative.GetWindowHandle((Application.Current as App)?.m_window as MainWindow);
				InitializeWithWindow.Initialize(savePicker, hwnd);

				StorageFile file = await savePicker.PickSaveFileAsync();
				if (file == null)
				{
					App.LogException("Save hydrograph image file save picker error", new Exception("File not selected"));
					return; // TODO handle dis
				}

				if (file.FileType == ".bmp")
				{
					Bitmap bmp = GetPlotBitmap();
					bmp.Save(file.Path);

					App.LogMessage("Save hydrograph as BMP at: " + file.Path);
				}
				else if (file.FileType == ".png")
				{
					RenderTargetBitmap renderBmp = new RenderTargetBitmap();
					await renderBmp.RenderAsync(Plot);

					var pixelBuffer = await renderBmp.GetPixelsAsync();

					using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
					{
						Windows.Graphics.Imaging.BitmapEncoder bmpEncoder = await Windows.Graphics.Imaging.BitmapEncoder.CreateAsync(Windows.Graphics.Imaging.BitmapEncoder.PngEncoderId, fileStream);
						bmpEncoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)renderBmp.PixelWidth, (uint)renderBmp.PixelHeight, 96, 96, pixelBuffer.ToArray());
						await bmpEncoder.FlushAsync();
					}

					App.LogMessage("Save hydrograph as PNG at: " + file.Path);
				}
				else { App.LogException("Save hydrograph image file save picker error", new Exception("File type not supported")); }

				await CachedFileManager.CompleteUpdatesAsync(file);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				App.LogException("Saving Hydrograph image", ex);
			}
		}

		private async void PrintPreviewClick(object sender, RoutedEventArgs e)
		{
            Window newWindow = new Window();

			// Add snapshot of plot to window
			Page page = new Page();
			RenderTargetBitmap plotBmp = new RenderTargetBitmap();
			await plotBmp.RenderAsync(Plot);
			Image plotImage = new Image();
			plotImage.Source = plotBmp;

			page.Content = plotImage;

			newWindow.Content = page;
            newWindow.Title = "Print Preview";
            newWindow.Activate();

			IntPtr hWnd = WindowNative.GetWindowHandle(newWindow);
			var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
			var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

			appWindow.TitleBar.ForegroundColor = Microsoft.UI.Colors.Black;
		}

		private void PrintPdfClick(object sender, RoutedEventArgs e)
		{
			this.PrintHydrographPdf?.Invoke(this, EventArgs.Empty);
		}

		private void ExitClick(object sender, RoutedEventArgs e)
		{
			this.CloseWindow?.Invoke(this, EventArgs.Empty);
		}

		private void CopyClick(object sender, RoutedEventArgs e)
		{
			Bitmap image = GetPlotBitmap();
			var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
				image.GetHbitmap(),
				IntPtr.Zero,
				System.Windows.Int32Rect.Empty,
				System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
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

		private Bitmap GetPlotBitmap()
		{
			var exporter = new OxyPlot.SvgExporter { Width = 650, Height = 450 };
			string svgString = exporter.ExportToString(Plot.Model);

			var mySvg = SvgDocument.FromSvg<SvgDocument>(svgString);

			Bitmap bitmap = mySvg.Draw();

			return bitmap;
		}
	}
}
