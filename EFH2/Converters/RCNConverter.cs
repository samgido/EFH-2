using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
    public class RCNConverter : IValueConverter
    {
        // from the model
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((Int32)value == -1) return "**";
            else return value.ToString();
        }

        // to the model
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return double.Parse((value as string).Trim());
        }
    }
}
