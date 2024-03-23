using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
    public struct HsgEntry
    {
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
    }

    public partial class RcnDataViewModel : ObservableObject
    {
        [ObservableProperty]
        private List<RCNCategory> rcnCategories;

        public ObservableCollection<HsgEntry> HsgEntries { get; } = new();

        public double AccumulatedArea
        {
            get
            {
                double total = 0;
                foreach (RCNCategory category in RcnCategories)
                {
                    if (!(category.AccumulatedArea.Equals(double.NaN))) total += category.AccumulatedArea;
                }
                return total;
            }
        }

        public double AccumulatedWeightedArea
        {
            get
            {
                double total = 0;
                foreach (RCNCategory category in RcnCategories)
                {
                    if (!(category.AccumulatedArea.Equals(double.NaN))) total += category.AccumulatedWeightedArea;
                }

                //if (AccumulatedArea.Equals(double.NaN) || AccumulatedArea.Equals(0)) return double.NaN;
                //else return total / AccumulatedArea;

                if (AccumulatedArea > 0) return total / AccumulatedArea;
                else return 0;
            }
        }

        public void LoadRCNTableEntries(StreamReader reader)
        {
            var _ = reader.ReadLine();

            RCNCategory currentCategory = new();
            List<RCNCategory> categories = new();

            while (!reader.EndOfStream)
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

                    if (splitLine[5] == "**") row.Entries[0] = new WeightAreaPair() { Weight = -1 };
                    else row.Entries[0] = new WeightAreaPair() { Weight = int.Parse(splitLine[5].Trim()) };

                    row.Entries[1] = new WeightAreaPair() { Weight = int.Parse(splitLine[7].Trim()) };
                    row.Entries[2] = new WeightAreaPair() { Weight = int.Parse(splitLine[9].Trim()) };
                    row.Entries[3] = new WeightAreaPair() { Weight = int.Parse(splitLine[11].Trim()) };

                    currentCategory.Rows.Add(row);
                }
            }

            categories.Add(currentCategory);

            RcnCategories = categories;

            foreach (RCNCategory category in RcnCategories)
            {
                foreach (RCNRow row in category.Rows)
                {
                    row.Entries[0].PropertyChanged += EntryChanged;
                    row.Entries[1].PropertyChanged += EntryChanged;
                    row.Entries[2].PropertyChanged += EntryChanged;
                    row.Entries[3].PropertyChanged += EntryChanged;
                }
            }
        }

        public void LoadHsgEntries(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                string[] lineParts = line.Split("\t");

                HsgEntries.Add(new HsgEntry()
                {
                    Field1 = lineParts[0],
                    Field2 = lineParts[1],
                    Field3 = lineParts[2],
                });

            }
        }

        private void EntryChanged(object? sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(nameof(AccumulatedArea));
            this.OnPropertyChanged(nameof(AccumulatedWeightedArea));
        }

        public void Default()
        {
            foreach (RCNCategory category in RcnCategories) category.Default();
        }

    }
}
