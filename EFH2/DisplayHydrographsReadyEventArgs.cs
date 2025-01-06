using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
	public class DisplayHydrographsReadyEventArgs : EventArgs
	{
		public bool IsReady { get; set; }

		public DisplayHydrographsReadyEventArgs(bool isReady) => IsReady = isReady;
	}
}
