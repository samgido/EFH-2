/* MainWindow.xaml.cs
 * Author: Samuel Gido
 */

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Windowing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Popups;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH_2
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        /// <summary>
        /// How many storms are available in the Rainfall/Discharge Data sheet
        /// </summary>
        public const int NumberOfStorms = 7; 

        /// <summary>
        /// View model for the basic data page
        /// </summary>
        public BasicDataViewModel BasicVM { get; set; }

        /// <summary>
        /// View model for the rainfall/discharge page
        /// </summary>
        public RainfallDataViewModel RainfallVM { get; set; }

        public MainWindow()
        {
            this.InitializeComponent();

            this.Title = "EFH-2 Estimating Runoff Volume and Peak Discharge";
            this.AppWindow.SetIcon("C:\\Users\\samue\\Source\\Repos\\samgido\\EFH - 2\\EFH - 2\\ProgramData\\EFH2.ico");

            contentFrame.Navigate(typeof(IntroPage));
            Tabs.SelectedItem = uxIntroPageNav;

            uxSlopeCalulatorButton.IsEnabled = false;
            uxHSGButton.IsEnabled = false;

            BasicVM = new();
            RainfallVM = new();
        }

        /// <summary>
        /// Changes the contents of contentFrame to whichever page the user selects
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="args">Object that holds information about the event</param>
        private void TabsSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            Type target = typeof(IntroPage);

            var selectedItem = (NavigationViewItem)args.SelectedItem;
            if (selectedItem != null)
            {
                string selectedItemTag = ((string)selectedItem.Tag);
                target = Type.GetType("EFH_2." + selectedItemTag);

                uxHSGButton.IsEnabled = (target == typeof(RCNPage));
                uxSlopeCalulatorButton.IsEnabled = (target == typeof(BasicDataPage));
            }

            contentFrame.Navigate(target);
        }

        /// <summary>
        /// Saves the data currently stored in the application to a .efm file 
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="e">Object that holds information about the event</param>
        private async void SaveClick(object sender, RoutedEventArgs e)
        {
            FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.FileTypeChoices.Add("efm", new List<string> { ".efm" });

            var window = new Microsoft.UI.Xaml.Window();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();

            if (file != null)
            {
                using (StreamWriter writer = new(file.Path))
                {
                    Writer w = new(writer);
                    // version

                    // basic data
                    foreach (object o in BasicVM.Summary)
                    {
                        w.WriteQuoted(o, true);
                    }

                    w.WriteQuoted("", true);
                    w.WriteQuoted("", true);
                    w.WriteQuoted("", true);

                    foreach (object o in RainfallVM.Summary)
                    {
                        w.WriteQuoted(o, true);
                    }
                }
            }

        }

        /// <summary>
        /// Opens a file dialog so user can import data to the application
        /// </summary>
        /// <param name="sender">Object that sent the event</param>
        /// <param name="e">Object that holds information about the event</param>
        private async void OpenClick(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.FileTypeFilter.Add(".efm");

            var window = new Microsoft.UI.Xaml.Window();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hwnd);

            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                using (StreamReader reader = new(file.Path))
                {
                    Reader r = new(reader);
                    string version = r.ReadQuoted();

                    BasicVM.By = r.ReadQuoted();
                    BasicVM.Date = DateTimeOffset.Parse(r.ReadQuoted());
                    BasicVM.Client = r.ReadQuoted();
                    BasicVM.SelectedCounty = r.ReadQuoted();
                    BasicVM.SelectedState = r.ReadQuoted();
                    BasicVM.Practice = r.ReadQuoted();
                    BasicVM.DrainageArea = r.ParseInt(r.ReadQuoted());
                    BasicVM.CurveNumber = r.ParseFloat(r.ReadQuoted());
                    BasicVM.WatershedLength = r.ParseInt(r.ReadQuoted());
                    BasicVM.WatershedSlope = r.ParseFloat(r.ReadQuoted());
                    BasicVM.TimeOfConcentration = r.ParseFloat(r.ReadQuoted());

                    // not sure what these are 
                    string line13 = r.Read();
                    string line14 = r.Read();
                    string line15 = r.Read();

                    string type = r.ReadQuoted();

                    string[] types = type.Split(", ");
                    RainfallVM.SelectedRainfallDistributionType = types[0];
                    if(types.Length == 2) { RainfallVM.SelectedDUHType = types[1]; }
                    else { RainfallVM.SelectedDUHType = "<standard>"; }

                    int[] frequencies = new int[MainWindow.NumberOfStorms];
                    float[] dayRains = new float[MainWindow.NumberOfStorms];

                    for (int i = 0; i < MainWindow.NumberOfStorms; i++)
                    {
                        RainfallVM.Storms[i].Frequency = r.ParseInt(r.ReadQuoted());
                        RainfallVM.Storms[i].DayRain = r.ParseFloat(r.ReadQuoted());
                    }

                }
            }
        }
    }

    /// <summary>
    /// Helper class that writes to the save file
    /// </summary>
    internal class Writer
    {
        /// <summary>
        /// Writes to a file
        /// </summary>
        private StreamWriter _writer;

        public Writer(StreamWriter w)
        {
            this._writer = w;
        }

        /// <summary>
        /// Writes an object's .ToString(), wrapped with quotes, to the file,
        /// optionally moves the writer to the next line
        /// </summary>
        /// <param name="s">Object who's ToString will be written</param>
        /// <param name="next">Whether or not the writer moves to the next line</param>
        public void WriteQuoted(object s, bool next)
        {
            string content = '"' + s.ToString() + '"';
            this._writer.Write(content);
            if(next) { _writer.WriteLine(""); }
        }

        /// <summary>
        /// Writes an object's .ToString() to the file,
        /// optionally moves the writer to the next line
        /// </summary>
        /// <param name="s">Object who's ToString will be written</param>
        /// <param name="next">Whether or not the writer moves to the next line</param>
        public void Write(object s, bool next)
        {
            this._writer.Write(s.ToString());
            if(next) { _writer.WriteLine(""); }
        }
    }

    internal class Reader
    {
        private StreamReader _r;

        public Reader(StreamReader r)
        {
            _r = r;
        }

        /// <summary>
        /// Reads a line from the reader, but removes at the beginning and end of the line
        /// </summary>
        /// <returns>The unquoted line</returns>
        public string ReadQuoted()
        {
            string line = this._r.ReadLine();

            line.Remove(0);
            line.Remove(line.Length - 1);

            line = line.Replace("\"", "");

            return line.Trim();
        }

        /// <summary>
        /// Reads a line from the reader
        /// </summary>
        /// <returns></returns>
        public string Read()
        {
            return this._r.ReadLine().Trim();
        }

        public int ParseInt(string s)
        {
            if (Int32.TryParse(s, out var temp))
            {
                return temp;
            }
            else { return 0; }
        }

        public double ParseDouble(string s)
        {
            if (double.TryParse(s, out var temp))
            {
                return temp;
            }
            else { return 0; }
        }

        public float ParseFloat(string s)
        {
            if (float.TryParse(s, out var temp))
            {
                return temp;
            }
            else { return 0; }
        }
    }
}
