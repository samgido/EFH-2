using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
    public partial class RcnCategory : ObservableObject
    {
        [ObservableProperty]
        private string _label = "";

        [ObservableProperty]
        private string _extra = "";

        [ObservableProperty]
        private List<RcnCategory> _rcnSubcategories = new List<RcnCategory>();

        [ObservableProperty]
        private List<RcnRow> _rows = new List<RcnRow>();

        public IEnumerable<RcnRow> AllRows
        {
            get
            {
                List<RcnRow> rows = new List<RcnRow>();
                foreach (RcnRow row in Rows) rows.Add(row);
                foreach (RcnCategory category in RcnSubcategories) rows.Concat(category.AllRows);
                return rows;
            }
        }
    }
}
