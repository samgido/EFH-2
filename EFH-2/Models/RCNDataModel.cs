using CommunityToolkit.Mvvm.ComponentModel;
using EFH_2.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2
{
    /// <summary>
    /// A class that holds all the data in the RCN page
    /// </summary>
    public partial class RCNDataModel : ObservableObject
    {
        #region Private Fields 

        private double _accumulatedArea;

        private double _weightedAccumulatedArea;

        #endregion 

        #region Observable Properties

        [ObservableProperty]
        private List<RCNCategory> _rcnCategories = new();

        [ObservableProperty]
        private double _groupAAccumulatedArea;
        [ObservableProperty]
        private double _groupAWeightedAccumulatedArea;

        [ObservableProperty]
        private double _groupBAccumulatedArea;
        [ObservableProperty]
        private double _groupBWeightedAccumulatedArea;

        [ObservableProperty]
        private double _groupCAccumulatedArea;
        [ObservableProperty]
        private double _groupCWeightedAccumulatedArea;

        [ObservableProperty]
        private double _groupDAccumulatedArea;
        [ObservableProperty]
        private double _groupDWeightedAccumulatedArea;

        private double _weightedCurveNumber;

        #endregion

        #region Public Properties

        public ObservableCollection<HSGEntry> HSGEntries { get; } = new();

        public double AccumulatedArea
        {
            get => _accumulatedArea;
            set
            {
                this.SetProperty(ref this._accumulatedArea, value);
            }
        }

        public double WeightedAccumulatedArea
        {
            get => _weightedAccumulatedArea;
            set
            {
                if (_accumulatedArea <= 0 || _accumulatedArea.Equals(double.NaN)) WeightedCurveNumber = 0; 
                else WeightedCurveNumber = value / _accumulatedArea;
                this.SetProperty(ref this._weightedAccumulatedArea, value);
            }
        }

        public double WeightedCurveNumber
        {
            get => Math.Round(_weightedCurveNumber);
            set => this.SetProperty(ref this._weightedCurveNumber, value);
        }
            

        #endregion

        #region Structs

        public struct HSGEntry
        {
            public string Column1 { get; set; }
            public string Column2 { get; set; }
            public string Column3 { get; set; }
        }

        #endregion

        #region Methods

        public void AddHSGEntry(string name, string column2, string group)
        {
            HSGEntries.Add(new()
            {
                Column1 = name,
                Column2 = column2,
                Column3 = group
            });
        }

        public void LoadRCNTableEntries(StreamReader reader)
        {
            reader.ReadLine();

            RCNCategory currentCategory = new();
            List<RCNCategory> categories = new();

            while(!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] splitLine = line.Split('\t');

                if (splitLine[0] == "") // start a new category if there aren't any input fields on a row
                {
                    categories.Add(currentCategory);

                    currentCategory = new();
                    currentCategory.Label = splitLine[1].Replace('"', (char)0);
                }
                else // add to the current category as long as there are input fields
                {
                    RCNRow row = new();
                    row.Text[0] = splitLine[1];
                    row.Text[1] = splitLine[2];
                    row.Text[2] = splitLine[3];

                    if (splitLine[5] == "**") row.WeightAreaPairs[0].Weight = -1;
                    else row.WeightAreaPairs[0].Weight = int.Parse(splitLine[5].Trim());

                    row.WeightAreaPairs[1].Weight = int.Parse(splitLine[7].Trim());
                    row.WeightAreaPairs[2].Weight = int.Parse(splitLine[9].Trim());
                    row.WeightAreaPairs[3].Weight = int.Parse(splitLine[11].Trim());

                    currentCategory.Rows.Add(row);
                }
            }

            categories.Add(currentCategory);

            RcnCategories = categories;

            foreach (RCNCategory category in RcnCategories)
            {
                foreach (RCNRow row in category.Rows)
                {
                    row.WeightAreaPairs[0].AreaChanged += GroupAFieldChanged;
                    row.WeightAreaPairs[1].AreaChanged += GroupBFieldChanged;
                    row.WeightAreaPairs[2].AreaChanged += GroupCFieldChanged;
                    row.WeightAreaPairs[3].AreaChanged += GroupDFieldChanged;
                }
            }
        }

        public void Default()
        {
            foreach(RCNCategory category in RcnCategories)
            {
                category.Default();
            }
        }

        public void Update()
        {
            AccumulatedArea = 0;
            WeightedAccumulatedArea = 0;

            foreach(RCNCategory category in RcnCategories)
            {
                if (!category.AccumulatedArea.Equals(double.NaN))
                {
                    AccumulatedArea += category.AccumulatedArea;
                    WeightedAccumulatedArea += category.AccumulatedWeightedArea;
                }
            }
        }

        private void GroupAFieldChanged(object? sender, (double oldValue, double newValue) e)
        {
            int weight = 0;
            if (sender is WeightAreaPair pair)
            {
                weight = pair.Weight;
            }

            if (!e.oldValue.Equals(double.NaN))
            {
                GroupAAccumulatedArea -= e.oldValue;
                AccumulatedArea -= e.oldValue;
                GroupAWeightedAccumulatedArea -= e.oldValue * weight;
                WeightedAccumulatedArea -= e.oldValue * weight;
            }
            if (!e.newValue.Equals(double.NaN))
            {
                GroupAAccumulatedArea += e.newValue;
                AccumulatedArea += e.newValue;
                GroupAWeightedAccumulatedArea += e.newValue * weight;
                WeightedAccumulatedArea += e.newValue * weight;
            }
        }

        private void GroupBFieldChanged(object? sender, (double oldValue, double newValue) e)
        {
            int weight = 0;
            if (sender is WeightAreaPair pair)
            {
                weight = pair.Weight;
            }

            if (!e.oldValue.Equals(double.NaN))
            {
                GroupBAccumulatedArea -= e.oldValue;
                AccumulatedArea -= e.oldValue;
                GroupBWeightedAccumulatedArea -= e.oldValue * weight;
                WeightedAccumulatedArea -= e.oldValue * weight;
            }
            if (!e.newValue.Equals(double.NaN))
            {
                GroupBAccumulatedArea += e.newValue;
                AccumulatedArea += e.newValue;
                GroupBWeightedAccumulatedArea += e.newValue * weight;
                WeightedAccumulatedArea += e.newValue * weight;
            }
        }

        private void GroupCFieldChanged(object? sender, (double oldValue, double newValue) e)
        {
            int weight = 0;
            if (sender is WeightAreaPair pair)
            {
                weight = pair.Weight;
            }

            if (!e.oldValue.Equals(double.NaN))
            {
                GroupCAccumulatedArea -= e.oldValue;
                AccumulatedArea -= e.oldValue;
                GroupCWeightedAccumulatedArea -= e.oldValue * weight;
                WeightedAccumulatedArea -= e.oldValue * weight;
            }
            if (!e.newValue.Equals(double.NaN))
            {
                GroupCAccumulatedArea += e.newValue;
                AccumulatedArea += e.newValue;
                GroupCWeightedAccumulatedArea += e.newValue * weight;
                WeightedAccumulatedArea += e.newValue * weight;
            }
        }

        private void GroupDFieldChanged(object? sender, (double oldValue, double newValue) e)
        {
            int weight = 0;
            if (sender is WeightAreaPair pair) weight = pair.Weight;

            if (!e.oldValue.Equals(double.NaN))
            {
                GroupDAccumulatedArea -= e.oldValue;
                AccumulatedArea -= e.oldValue;
                GroupDWeightedAccumulatedArea -= e.oldValue * weight;
                WeightedAccumulatedArea -= e.oldValue * weight;
            }
            if (!e.newValue.Equals(double.NaN))
            {
                GroupDAccumulatedArea += e.newValue;
                AccumulatedArea += e.newValue;
                GroupDWeightedAccumulatedArea += e.newValue * weight;
                WeightedAccumulatedArea += e.newValue * weight;
            }
        }

        #endregion
    }
}
