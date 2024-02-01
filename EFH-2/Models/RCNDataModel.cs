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

        #endregion

        #region Public Fields

        #endregion

        #region Observable Properties

        [ObservableProperty]
        private List<RCNCategory> _rcnCategories = new();

        [ObservableProperty]
        static private double _weightedCurveNumber = 0;

        [ObservableProperty]
        private double _accumulatedArea = 0;

        #endregion

        #region Properties

        public ObservableCollection<HSGEntry> HSGEntries { get; } = new();

        public string[][] RCNTableEntries { get; }

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
            var _ = reader.ReadLine();

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

                    if (splitLine[5] == "**") row.WeightAreaPairs[0] = new(-1);
                    else row.WeightAreaPairs[0] = new(int.Parse(splitLine[5].Trim()));

                    row.WeightAreaPairs[1] = new(int.Parse(splitLine[7].Trim()));
                    row.WeightAreaPairs[2] = new(int.Parse(splitLine[9].Trim()));
                    row.WeightAreaPairs[3] = new(int.Parse(splitLine[11].Trim()));

                    currentCategory.Rows.Add(row);
                }
            }

            categories.Add(currentCategory);

            RcnCategories = categories;
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
            double totalArea = 0;
            double totalWeightedArea = 0;

            foreach(RCNCategory category in RcnCategories)
            {
                if (!category.AccumulatedArea.Equals(double.NaN))
                {
                    totalArea += category.AccumulatedArea;
                    totalWeightedArea += category.AccumulatedWeightedArea;
                }
            }

            AccumulatedArea = totalArea;
            WeightedCurveNumber = totalWeightedArea;
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

        public RCNDataModel()
        {
            RCNTableEntries = new string[7][];
            for (int i = 0; i < RCNTableEntries.Length; i++)
            {
                RCNTableEntries[i] = new string[120];
            }
        }

    }
}
