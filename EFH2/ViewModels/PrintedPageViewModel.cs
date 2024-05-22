﻿using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
	public partial class PrintableMainViewModel : ObservableObject
	{
		[ObservableProperty]
		private ObservableCollection<StormVMWrapper> _storms = new ObservableCollection<StormVMWrapper>();

		[ObservableProperty]
		private ObservableCollection<PrintableRcnCategory> _categories = new ObservableCollection<PrintableRcnCategory>();

		[ObservableProperty]
		private MainViewModel _mainViewModel;

		[ObservableProperty]
		private double _groupAAccumulatedArea;

		[ObservableProperty]
		private double _groupBAccumulatedArea;

		[ObservableProperty]
		private double _groupCAccumulatedArea;

		[ObservableProperty]
		private double _groupDAccumulatedArea;

		// to format for printing
		public int CurveNumber => (int)MainViewModel.RcnDataViewModel.WeightedCurveNumber;

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
					(!double.IsNaN(basicData.DrainageArea) && !double.IsNaN(basicData.RunoffCurveNumber) && rcnData.AccumulatedArea.Equals(0) && rcnData.WeightedCurveNumber.Equals(0)))
				{
					return Visibility.Collapsed;
				}
				else return Visibility.Visible;
			}
		}

		public PrintableMainViewModel(MainViewModel model)
		{
			MainViewModel = model;

			foreach (StormViewModel storm in model.RainfallDischargeDataViewModel.Storms)
			{
				Storms.Add(new StormVMWrapper(MainViewModel.BasicDataViewModel.DrainageArea, storm));
			}

			List<RcnSubcategory> list = new List<RcnSubcategory>();
			foreach (RcnMainCategory category in model.RcnDataViewModel.RcnCategories)
			{
				foreach (RcnSubcategory subcategory in category.RcnSubcategories)
				{
					foreach (RcnRow row in subcategory.Rows)
					{
						if (!row.AccumulatedArea.Equals(0))
						{
							if (!list.Contains(subcategory)) list.Add(subcategory);
						}
					}
				}
			}

			_categories = new ObservableCollection<PrintableRcnCategory>();
			foreach (RcnSubcategory filledCategory in list)
			{
				PrintableRcnCategory newCat = new PrintableRcnCategory(filledCategory.Label.Trim());
				foreach (RcnRow row in filledCategory.Rows)
				{
					if (!row.AccumulatedArea.Equals(0)) newCat.Rows.Add(new RcnRowWrapper(row));
				}
				_categories.Add(newCat);
			}

			foreach (RcnMainCategory category in model.RcnDataViewModel.RcnCategories)
			{
				foreach (RcnSubcategory subcategory in category.RcnSubcategories)
				{
					foreach (RcnRow row in subcategory.Rows)
					{
						if (!double.IsNaN(row.Entries[0].Area)) _groupAAccumulatedArea += row.Entries[0].Area;
						if (!double.IsNaN(row.Entries[1].Area)) _groupBAccumulatedArea += row.Entries[1].Area;
						if (!double.IsNaN(row.Entries[2].Area)) _groupCAccumulatedArea += row.Entries[2].Area;
						if (!double.IsNaN(row.Entries[3].Area)) _groupDAccumulatedArea += row.Entries[3].Area;
					}
				}
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

		public class PrintableRcnCategory
		{
			public string Label { get; private set; }
			public List<RcnRowWrapper> Rows { get; set; } = new List<RcnRowWrapper>();

			public PrintableRcnCategory(string label)
			{
				Label = label;
			}
		}

		public class RcnRowWrapper
		{
			public RcnRow BaseRow { get; private set; }
			public RowEntrySummary EntryA { get; private set; }
			public RowEntrySummary EntryB { get; private set; }
			public RowEntrySummary EntryC { get; private set; }
			public RowEntrySummary EntryD { get; private set; }

			public RcnRowWrapper(RcnRow row)
			{
				BaseRow = row;
				EntryA = new RowEntrySummary(row.Entries[0]);
				EntryB = new RowEntrySummary(row.Entries[1]);
				EntryC = new RowEntrySummary(row.Entries[2]);
				EntryD = new RowEntrySummary(row.Entries[3]);
			}
		}

		public class RowEntrySummary
		{
			private double _area;
			private double _weight;

			public string Summary
			{
				get
				{
					if (double.IsNaN(_area))
						return "-";
					else
						return _area + "(" + _weight + ")";
				}
			}

			public RowEntrySummary(WeightAreaPair pair)
			{
				_area = pair.Area;
				_weight = pair.Weight;
			}
		}
	}
}
