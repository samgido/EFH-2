using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using CommunityToolkit.WinUI.Helpers;
using Windows.Graphics.Printing;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH2
{
    public sealed partial class PrintHelperPage : UserControl
    {
        private PrintHelper _printHelper;
        private DataTemplate customPrintTemplate;

        public PrintHelperPage()
        {
            this.InitializeComponent();
        }

        public async void Print()
        {
            DirectPrintContainer.Children.Remove(PrintableContent);
            
            _printHelper = new PrintHelper(Container);
            _printHelper.AddFrameworkElementToPrint(PrintableContent);

			_printHelper.OnPrintCanceled += _printHelper_OnPrintCanceled;
			_printHelper.OnPrintFailed += _printHelper_OnPrintFailed;
			_printHelper.OnPrintSucceeded += _printHelper_OnPrintSucceeded;

			await _printHelper.ShowPrintUIAsync("MyTitle");
		}

		private void ReleasePrintHelper()
		{
			_printHelper.Dispose();

			if (!DirectPrintContainer.Children.Contains(PrintableContent))
			{
				DirectPrintContainer.Children.Add(PrintableContent);
			}
		}

		private void _printHelper_OnPrintSucceeded()
		{
			ReleasePrintHelper();
		}

		private void _printHelper_OnPrintFailed()
		{
			ReleasePrintHelper();
			// show dialog that it failed
		}

		private void _printHelper_OnPrintCanceled()
		{
			ReleasePrintHelper();
		}
	}
}
