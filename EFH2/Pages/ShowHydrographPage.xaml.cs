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
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;
using SharpVectors.Converters;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using OxyPlot.Wpf;
using System.Drawing.Imaging;

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
				savePicker.FileTypeChoices.Add("SVG", new List<string> { ".svg" });

				var hwnd = WindowNative.GetWindowHandle((Application.Current as App)?.m_window as MainWindow);
				InitializeWithWindow.Initialize(savePicker, hwnd);

				StorageFile file = await savePicker.PickSaveFileAsync();
				if (file != null)
				{
					//CachedFileManager.DeferUpdates(file);

					//BitmapImage image = GetBitmap();
					//if (image == null) { throw new InvalidOperationException("Bitmap not initialized"); }


					//BitmapEncoder encoder = new PngBitmapEncoder();
					//encoder.Frames.Add(BitmapFrame.Create(image));

					//using (var fileStream = new FileStream(file.Path, FileMode.Create))
					//{
					//	encoder.Save(fileStream);
					//}

					using (var stream = new MemoryStream())
					{
						var exporter = new OxyPlot.SvgExporter { Width = 650, Height = 450 };
						exporter.Export(Plot.Model, stream);
						stream.Seek(0, SeekOrigin.Begin);
						var svgString = new StreamReader(stream).ReadToEnd();
						File.WriteAllText(file.Path, svgString);
					}
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

		}

		private void CopyClick(object sender, RoutedEventArgs e)
		{
			using (var stream = new MemoryStream())
			{
				OxyPlot.SvgExporter exporter = new OxyPlot.SvgExporter { Width = 650, Height = 450 };
				exporter.Export(Plot.Model, stream);
				stream.Seek(0, SeekOrigin.Begin);
				string svgString = new StreamReader(stream).ReadToEnd();
				System.Windows.Clipboard.SetText(svgString);
			}
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

		private BitmapImage GetBitmap()
		{
			try
			{
				// ATTEMPT 1
				//var stream = new MemoryStream();
				//var svgExporter = new SvgExporter() { Width = 650, Height = 450 };
				//svgExporter.Export(Plot.Model, stream);

				//var converter = new FileSvgReader(new SharpVectors.Renderers.Wpf.WpfDrawingSettings());
				//var drawing = converter.Read(stream);

				//var bitmap = new Bitmap(650, 450);

				//var renderTargetBitmap = new RenderTargetBitmap(650, 450, 96, 96, pixelFormat: PixelFormats.Pbgra32);
				//var drawingVisual = new DrawingVisual();

				//using (var drawingContext = drawingVisual.RenderOpen())
				//{
				//	drawingContext.DrawDrawing(drawing);
				//}

				//renderTargetBitmap.Render(drawingVisual);

				//using (var memoryStream = new MemoryStream())
				//{
				//	var bitmapEncoder = new BmpBitmapEncoder();
				//	bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
				//	bitmapEncoder.Save(memoryStream);

				//	memoryStream.Seek(0, SeekOrigin.Begin);
				//	bitmap = new Bitmap(memoryStream);
				//}
				//return bitmap

				// ATTEMPT 2
				//var pngExporter = new PngExporter { Width = 650, Height = 450 };
				//BitmapSource bitmapSource = pngExporter.ExportToBitmap(Plot.Model);
				//Bitmap bitmap = new Bitmap(bitmapSource.PixelWidth, bitmapSource.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

				//BitmapData data = bitmap.LockBits(new Rectangle(System.Drawing.Point.Empty, bitmap.Size), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

				//bitmapSource.CopyPixels(System.Windows.Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);

				////bitmap.UnlockBits(data);
				//return bitmap;

				// ATTEMPT 3
				//var pngExporter = new PngExporter { Width = 650, Height = 450 };
				//using (var stream = new MemoryStream())
				//{
				//	pngExporter.Export(Plot.Model, stream);
				//	stream.Seek(0, SeekOrigin.Begin);

				//	var bitmapImage = new BitmapImage();
				//	bitmapImage.BeginInit();
				//	bitmapImage.StreamSource = stream;
				//	bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				//	bitmapImage.EndInit();
				//	bitmapImage.Freeze();

				//	return bitmapImage;
				//}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}

			return null;
		}
	}
}
