using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Devices.Sms;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH2
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private static readonly string logFilePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EFH2", "EFH2.log");

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
        {
            this.InitializeComponent();


            string logFileDirectory = logFilePath.Replace("EFH2.log", "");
            if (!Directory.Exists(logFileDirectory))
            {
                Directory.CreateDirectory(logFileDirectory);
			}

            // Temporary solution for issue 8810
            this.AddOtherProvider(new Microsoft.UI.Xaml.XamlTypeInfo.XamlControlsXamlMetaDataProvider());
            this.AddOtherProvider(new CommunityToolkit.WinUI.UI.Controls.CommunityToolkit_WinUI_UI_Controls_DataGrid_XamlTypeInfo.XamlMetaDataProvider());
            //this.AddOtherProvider(new CommunityToolkit.WinUI.Controls.SettingsControlsRns.CommunityToolkit_WinUI_Controls_SettingsControls_XamlTypeInfo.XamlMetaDataProvider());

            FileOperations.InitEfh2Structure();

			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

		private void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
		{
            LogException("Non-UI Thread Exception", e.ExceptionObject as Exception);
		}

        public static void LogException(string src, Exception? ex)
        {
			string logMessage = $"[{DateTime.Now}] {src}: {ex?.Message}\nStack Trace: \n{ex?.StackTrace}\n";
            File.AppendAllText(logFilePath, logMessage);
        }

        public static void LogMessage(string message)
        {
            string logMessage = $"[{DateTime.Now}] {message}\n";
            File.AppendAllText(logFilePath, logMessage);
		}

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            MainViewModel mainViewModel = new MainViewModel();
            FileOperations.LoadMainViewModel(mainViewModel);

            m_model = mainViewModel;

            m_window = new MainWindow()
            {
                Title = "EFH-2 Estimating Runoff Volume and Peak Discharge",
                ExtendsContentIntoTitleBar = true,
            };

            m_window.Activate();
        }

        internal Window m_window;

        internal MainViewModel m_model;
    }
}
