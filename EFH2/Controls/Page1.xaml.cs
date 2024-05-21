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
		public MainViewModel MainViewModel
		{
			get
			{
				if (DataContext is MainViewModel model) return model;
				else return null;
			}
		}

		public ObservableCollection<StormVMWrapper> Storms { get; private set; } = new ObservableCollection<StormVMWrapper>();

		public Page1()
		{
			this.InitializeComponent();
		}

		public void SetDataContext(MainViewModel model)
		{
			this.DataContext = model;
			foreach (StormViewModel storm in model.RainfallDischargeDataViewModel.Storms)
			{
				Storms.Add(new StormVMWrapper(MainViewModel.BasicDataViewModel.DrainageArea, storm));
			}
		}

		public class StormVMWrapper
		{
			private double _drainageArea;
			public StormViewModel BaseStorm { get; private set; }
			public double RunoffInAcreFeet => (_drainageArea * BaseStorm.Runoff) / 12;


			public StormVMWrapper(double drainageArea, StormViewModel storm)
			{
				_drainageArea = drainageArea;
				BaseStorm = storm;
			}
		}
	}
}
