using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
    public partial class RcnMainCategory : ObservableObject
    {
        [ObservableProperty]
        private string _label = "";

        [ObservableProperty]
        private List<RcnSubcategory> _rcnSubcategories = new List<RcnSubcategory>();

        public void Default() => RcnSubcategories.ForEach(cat => cat.Default());
    }
}
