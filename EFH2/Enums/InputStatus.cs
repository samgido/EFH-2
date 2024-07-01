using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
	public enum InputStatus
	{
		None,
		UserEnteredValue,
		UserSelected,
		Calculated,
		FromRcnCalculator,
		LoadedFromFile,
		Invalid,
		Cleared,
	}
}
