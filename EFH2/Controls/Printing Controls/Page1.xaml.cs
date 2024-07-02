using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH2
{
	public sealed partial class Page1 : UserControl
	{
		public PrintableMainViewModel ViewModel 
		{
			get
			{
				if (DataContext is PrintableMainViewModel model) return model;
				else return null;
			}
		}

		public Page1()
		{
			this.InitializeComponent();
		}

		public void ChangePageNumber() => PageNumber.Text = "Page 1 of 2";
	}
}
