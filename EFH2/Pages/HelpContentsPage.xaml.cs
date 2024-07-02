using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH2
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class HelpContentsPage : Page
	{
		public HelpContentsPage()
		{
			this.InitializeComponent();
			InitializeWebView();
			//WebView.Source = new Uri("ms-appx-web:///Assets/EFH2.chm");
		}

		private async void InitializeWebView()
		{
			await WebView.EnsureCoreWebView2Async();
			WebView.NavigateToString(LoadChm());
		}

		private string LoadChm()
		{
			string path = "C:\\Users\\samue\\Source\\Repos\\samgido\\EFH-2\\EFH2\\Assets\\EFH2.chm";
			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				using (StreamReader reader = new StreamReader(stream))
				{
					string contents = reader.ReadToEnd();
					return contents;
				}
			}
		}
	}
}
