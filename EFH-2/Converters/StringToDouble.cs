/* StringToDouble.cs
 * Author: Samuel Gido 
 */

using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2
{
    /// <summary>
    /// Converts a string to a double and vice versa
    /// 
    /// Used for Xaml conversions
    /// </summary>
    public class StringToDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (((double)value).Equals(double.NaN))
            {
                return "";
            }
            else
            {
                return value.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return double.Parse(value as string);
        }
    }
}
