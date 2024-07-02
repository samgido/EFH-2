using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH2
{
	public sealed partial class RcnRowsControl : UserControl
	{
		public RcnRow ViewModel
		{
			get
			{
				if (DataContext is RcnRow row) return row;
				else return null;
			}
		}

		public RcnRowsControl()
		{
			this.InitializeComponent();
		}
	}
}
