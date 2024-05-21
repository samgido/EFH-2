using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
	public partial class PrintedPageViewModel : ObservableObject
	{
		[ObservableProperty]
		private ObservableCollection<StormVMWrapper> _storms = new ObservableCollection<StormVMWrapper>();

		[ObservableProperty]
		private MainViewModel _mainViewModel;

		public PrintedPageViewModel(MainViewModel model)
		{
			MainViewModel = model;	

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
