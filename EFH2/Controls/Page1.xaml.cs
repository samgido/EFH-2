using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

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

		public void SetSecondPage() => PageNumber.Text = "Page 1 of 2";
	}
}
