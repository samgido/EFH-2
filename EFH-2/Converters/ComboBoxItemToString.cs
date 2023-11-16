using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2
{
    public class ComboBoxItemToString : IValueConverter
    {
        // from program -> UI
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ComboBoxItem c = new();
            c.Content = value as string;

            return c;
        }

        // from UI -> program
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null) { return ""; }
            ComboBoxItem c = value as ComboBoxItem;

            return c.Content.ToString();
        }
    }
}
