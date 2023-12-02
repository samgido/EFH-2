using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2
{
    public class RCNDataViewModel : BindableBase
    {

        public RCNDataViewModel()
        {
            Entries = new();
        }
        
        public struct HSGEntry
        {
            public string Name { get; set; }
            public string Column2 { get; set; }
            public string Group { get; set; }
        }

        public ObservableCollection<HSGEntry> Entries { get; }

        public void AddEntry(string name, string column2, string group)
        {
            Entries.Add(new()
            {
                Name = name,
                Column2 = column2,
                Group = group
            });
        }
    }
}
