using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2
{
    /// <summary>
    /// Convert DateTime to strings and viceversa
    /// </summary>
    public class DateTimeToString : IValueConverter
    {
        /// <summary>
        /// Converts a string to a DateTime object
        /// </summary>
        /// <param name="value">The string</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns>The DateTime object made from the string</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return DateTime.Parse((string)value);
        }

        /// <summary>
        /// Converts a DateTime object to a string
        /// </summary>
        /// <param name="value">The DateTime object</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns>The DateTime object's date as a string</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            DateTime dt = (DateTime)value;

            return dt.Date.ToString("MM/dd/yyyy");
        }
    }
}
