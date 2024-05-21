using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
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
		private ObservableCollection<RcnCategory> _categories = new ObservableCollection<RcnCategory>();

		[ObservableProperty]
		private MainViewModel _mainViewModel;

		public Visibility CurveNumberMatches
		{
			get
			{
				BasicDataViewModel basicData = MainViewModel.BasicDataViewModel;
				RcnDataViewModel rcnData = MainViewModel.RcnDataViewModel;

				//if ((!double.IsNaN(basicData.DrainageArea) && !double.IsNaN(basicData.RunoffCurveNumber) &&
				//	!double.IsNaN(rcnData.WeightedCurveNumber) && !double.IsNaN(rcnData.AccumulatedArea)) ||
				//	(basicData.DrainageArea.Equals(rcnData.AccumulatedArea) && basicData.RunoffCurveNumber.Equals(rcnData.WeightedCurveNumber)))
				if ((basicData.DrainageArea.Equals(rcnData.AccumulatedArea) && basicData.RunoffCurveNumber.Equals(rcnData.WeightedCurveNumber)) || 
					(!double.IsNaN(basicData.DrainageArea) && !double.IsNaN(basicData.RunoffCurveNumber) && rcnData.AccumulatedArea.Equals(0) && rcnData.WeightedCurveNumber.Equals(0) ))
				{
					return Visibility.Collapsed;
				}
				else return Visibility.Visible;
			}
		}

		public PrintedPageViewModel(MainViewModel model)
		{
			MainViewModel = model;	

			foreach (StormViewModel storm in model.RainfallDischargeDataViewModel.Storms)
			{
				Storms.Add(new StormVMWrapper(MainViewModel.BasicDataViewModel.DrainageArea, storm));
			}

			List<RcnCategory> list = new List<RcnCategory>();
			foreach (RcnCategory category in model.RcnDataViewModel.RcnCategories)
			{
				foreach (RcnRow row in category.Rows)
				{
					if (!double.IsNaN(row.AccumulatedArea))
					{
						if (!list.Contains(category)) list.Add(category);
					}
				}
			}

			_categories = new ObservableCollection<RcnCategory>();
			foreach (RcnCategory filledCategory in list)
			{
				RcnCategory newCat = new RcnCategory();
				newCat.Label = filledCategory.Label;
				newCat.Rows = new List<RcnRow>();
				foreach (RcnRow row in filledCategory.Rows)
				{
					if (!double.IsNaN(row.AccumulatedArea)) newCat.Rows.Add(row);
				}
				_categories.Add(newCat);
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
