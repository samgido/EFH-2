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
        }

        public struct HSGEntry
        {
            public string Name { get; set; }
            public string Column2 { get; set; }
            public string Group { get; set; }
        }

        public ObservableCollection<HSGEntry> HSGEntries { get; } = new();

        public void AddHSGEntry(string name, string column2, string group)
        {
            HSGEntries.Add(new()
            {
                Name = name,
                Column2 = column2,
                Group = group
            });
        }

        public ObservableCollection<List<string>> RCNTableEntries { get; } = new();

        public void LoadRCNTableEntries(StreamReader reader)
        {
            while(!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] splitLine = line.Split('\t');
                List<string> list = new();
                
                foreach(string s in splitLine)
                {
                    list.Add(s);
                }

                RCNTableEntries.Add(list);
            }
        }
    }
}
