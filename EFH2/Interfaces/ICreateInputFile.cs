using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
	public interface ICreateInputFile
	{
		event EventHandler<EventArgs>? CreateInputFile;
	}
}
