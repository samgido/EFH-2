using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
	public class DoubleToEmptyStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			try
			{
				if (value is double d && double.IsNormal(d)) return string.Format("{0:F2}", value);
				else return string.Empty;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
			return string.Empty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
