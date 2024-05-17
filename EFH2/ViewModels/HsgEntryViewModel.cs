using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
	public partial class HsgEntryViewModel : ObservableObject
	{
		[ObservableProperty]
		private ObservableCollection<string> _row = new();
	}
}
