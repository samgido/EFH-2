using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2
{
    /// <summary>
    /// A class that holds all the data in the RCN page
    /// </summary>
    public class RCNDataViewModel : BindableBase
    {
        public RCNDataViewModel()
        {
            RCNTableEntries = new string[7][];
            for (int i = 0; i < RCNTableEntries.Length; i++)
            {
                RCNTableEntries[i] = new string[120];
            }

            for (int i = 0; i < 81; i++)
            {
                _groupAInputs.Add(double.NaN);
            }
        }

        public struct HSGEntry
        {
            public string Column1 { get; set; }
            public string Column2 { get; set; }
            public string Column3 { get; set; }
        }

        public ObservableCollection<HSGEntry> HSGEntries { get; } = new();

        public void AddHSGEntry(string name, string column2, string group)
        {
            HSGEntries.Add(new()
            {
                Column1 = name,
                Column2 = column2,
                Column3 = group
            });
        }

        public string[][] RCNTableEntries { get; }

        public void LoadRCNTableEntries(StreamReader reader)
        {
            var _ = reader.ReadLine();
            int lineNumber = 0;
            while(lineNumber < 118)
            {
                string line = reader.ReadLine();
                string[] splitLine = line.Split('\t');

                if (splitLine.Length >= 2)
                {
                    RCNTableEntries[0][lineNumber] = splitLine[1];
                }
                if (splitLine.Length >= 3)
                {
                    RCNTableEntries[1][lineNumber] = splitLine[2];
                }
                if (splitLine.Length >= 4)
                {
                    RCNTableEntries[2][lineNumber] = splitLine[3];
                }
                if (splitLine.Length >= 6)
                {
                    RCNTableEntries[3][lineNumber] = splitLine[5];
                }
                if (splitLine.Length >= 8)
                {
                    RCNTableEntries[4][lineNumber] = splitLine[7];
                }
                if (splitLine.Length >= 10)
                {
                    RCNTableEntries[5][lineNumber] = splitLine[9];
                }
                if (splitLine.Length >= 12)
                {
                    RCNTableEntries[6][lineNumber] = splitLine[11];
                }

                lineNumber++;
            }
        }

        private ObservableCollection<double> _groupAInputs = new();
        public ObservableCollection<double> GroupAInputs
        {
            get
            {
                return _groupAInputs;
            }
            set
            {
                this.SetProperty(ref this._groupAInputs, value);
            }
        }

        private ObservableCollection<double> _groupBInputs = new();

        private ObservableCollection<double> _groupCInputs = new();
    }
}
