using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EFH2
{
    public partial class RcnDataViewModel : ObservableObject
    {
        [ObservableProperty]
        private List<RcnCategory> _rcnCategories;

        [ObservableProperty]
        private ObservableCollection<HsgEntryViewModel> _hsgEntries = new();

        public double AccumulatedArea
        {
            get
            {
                double total = 0;
                foreach (RcnCategory category in RcnCategories)
                {
                    if (!(category.AccumulatedArea.Equals(double.NaN))) total += category.AccumulatedArea;
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
                    if (!(category.AccumulatedArea.Equals(double.NaN))) total += category.AccumulatedWeightedArea;
                }

                //if (AccumulatedArea.Equals(double.NaN) || AccumulatedArea.Equals(0)) return double.NaN;
                //else return total / AccumulatedArea;

                if (AccumulatedArea > 0) return total / AccumulatedArea;
                else return 0;
            }
        }

        public void EntryChanged(object? sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(nameof(AccumulatedArea));
            this.OnPropertyChanged(nameof(WeightedCurveNumber));
        }

        public RcnDataModel ToRcnDataModel()
        {
            RcnDataModel model = new RcnDataModel();

            foreach (RcnCategory category in RcnCategories)
            {
                foreach (RcnRow row in category.Rows)
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
                foreach (RcnRow row in category.Rows)
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
                foreach (RcnRow row in category.Rows)
                {
                    foreach (WeightAreaPair values in row.Entries)
                    {
                        if (!values.Area.Equals(double.NaN))
                        {
                            values.Area = (values.Area / accumulatedArea) * 100;
                        }
                    }
                }
            }
        }

        public void ConvertToAcresFromPercentage(double accumulatedArea)
        {
            foreach (RcnCategory category in RcnCategories)
            {
                foreach (RcnRow row in category.Rows)
                {
                    foreach (WeightAreaPair values in row.Entries)
                    {
                        if (!values.Area.Equals(double.NaN))
                        {
                            values.Area = (values.Area * accumulatedArea) / 100;
                        }
                    }
                }
            }
        }
    }
}
