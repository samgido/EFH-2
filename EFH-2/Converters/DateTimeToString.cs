using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2
{
    public class DateTimeToString : IValueConverter
    {
        // string -> datetime
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return DateTime.Parse((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            DateTime dt = (DateTime)value;

            return dt.Date.ToString("MM/dd/yyyy");
        }
    }
}
