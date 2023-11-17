using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2
{
    /// <summary>
    /// Converts ComboBoxItems to strings and vice versa
    /// </summary>
    public class ComboBoxItemToString : IValueConverter
    {
        /// <summary>
        /// Converts string to ComboBoxItem
        /// </summary>
        /// <param name="value">The string</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns>A ComboBoxItem with contents matching the string</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ComboBoxItem c = new();
            c.Content = value as string;

            return c;
        }

        /// <summary>
        /// Converts a ComboBoxItem to a string
        /// </summary>
        /// <param name="value">The ComboBoxItem</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns>The content of that ComboBoxItem</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null) { return ""; }
            ComboBoxItem c = value as ComboBoxItem;

            return c.Content.ToString();
        }
    }
}
