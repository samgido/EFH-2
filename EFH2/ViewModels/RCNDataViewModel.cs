using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EFH2
{
	public partial class RcnDataViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        public static bool Used = false;

        [ObservableProperty]
        private bool _acresSelected = true;

        [ObservableProperty]
        private bool _percentSelected = false;

        [ObservableProperty]
        private List<RcnCategory> _rcnCategories = new();

        [ObservableProperty]
        private ObservableCollection<HsgEntryViewModel> _hsgEntries = new();

        public double AccumulatedArea
        {
            get
            {
                double total = 0;
                foreach (RcnCategory category in RcnCategories)
                {
                    foreach (RcnRow row in category.AllRows)
                    {
						if (!(double.IsNaN(row.AccumulatedArea))) total += row.AccumulatedArea;
                    }
                }
                return total;
            }
        }

        public double WeightedCurveNumber
        {
            get
            {
                double total = 0;
                foreach (RcnCategory category in RcnCategories)
                {
					if (!(double.IsNaN(category.AccumulatedArea))) total += category.AccumulatedWeightedArea;
                }

                if (AccumulatedArea > 0) return Math.Round(total / AccumulatedArea);
                else return 0;
            }
        }

        public void EntryChanged(object? sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(nameof(AccumulatedArea));
            this.OnPropertyChanged(nameof(WeightedCurveNumber));

            Used = true;
        }

        public RcnDataModel ToRcnDataModel()
        {
            RcnDataModel model = new RcnDataModel();

            foreach (RcnCategory category in RcnCategories)
            {
                foreach (RcnRow row in category.AllRows)
                {
					model.GroupA.Entries.Add(new RcnEntryModel()
					{
						Area = row.Entries[0].Area,
						Weight = row.Entries[0].Weight,
					});

					model.GroupB.Entries.Add(new RcnEntryModel()
					{
						Area = row.Entries[1].Area,
						Weight = row.Entries[1].Weight,
					});

					model.GroupC.Entries.Add(new RcnEntryModel()
					{
						Area = row.Entries[2].Area,
						Weight = row.Entries[2].Weight,
					});

					model.GroupD.Entries.Add(new RcnEntryModel()
					{
						Area = row.Entries[3].Area,
						Weight = row.Entries[3].Weight,
					});
                }
            }

            return model;
        }

        public void LoadDataModel(RcnDataModel data)
        {
            int i = 0;
            if (data == null) return;

            foreach (RcnCategory category in RcnCategories)
            {
                foreach (RcnRow row in category.AllRows)
                {
					if (data.GroupA.Entries.Count > i)
					{
						row.Entries[0].Area = data.GroupA.Entries[i].Area;
						row.Entries[1].Area = data.GroupB.Entries[i].Area;
						row.Entries[2].Area = data.GroupC.Entries[i].Area;
						row.Entries[3].Area = data.GroupD.Entries[i].Area;
						i++;
					}
                }
            }

            this.OnPropertyChanged(nameof(RcnCategories));
        }

        public void Default()
        {
            foreach (RcnCategory category in RcnCategories) category.Default();
        }

        public void ConvertToPercentageFromAcres(double accumulatedArea)
        {
            foreach (RcnCategory category in RcnCategories)
            {
                foreach (RcnRow row in category.AllRows)
                {
					foreach (WeightAreaPair values in row.Entries)
					{
						if (!double.IsNaN(values.Area))
						{
							values.Area = (values.Area / accumulatedArea) * 100;
						}
                    }
                }
            }

            this.AcresSelected = false;
            this.PercentSelected = true;
        }

        public void ConvertToAcresFromPercentage(double accumulatedArea)
        {
            foreach (RcnCategory category in RcnCategories)
            {
                foreach (RcnRow row in category.AllRows)
                {
					foreach (WeightAreaPair values in row.Entries)
					{
						if (!double.IsNaN(values.Area))
						{
							values.Area = (values.Area * accumulatedArea) / 100;
                        }
                    }
                }
            }
        }
    }
}
