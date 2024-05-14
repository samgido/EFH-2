using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization.NumberFormatting;

namespace EFH2
{
	public class DoubleFormatter : INumberFormatter2, INumberParser
	{
		public virtual double? ParseDouble(string text) => double.TryParse(text, out var dbl) ? dbl : null;
		public virtual string Format { get; set; } = "{0:F2}"; 
		public virtual string FormatDouble(double value) => string.Format(Format, value);

		// Only need to do doubles 
		public string FormatInt(long value) => throw new NotSupportedException();
		public string FormatUInt(ulong value) => throw new NotSupportedException();
		public long? ParseInt(string text) => throw new NotSupportedException();
		public ulong? ParseUInt(string text) => throw new NotSupportedException();
	}
}
