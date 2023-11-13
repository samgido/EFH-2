using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2
{
    class ComboBoxOperations
    {
        public static void PopulateComboBox(ComboBox cb, string[] elements)
        {
            cb.Items.Clear();
            
            foreach (string s in elements)
            {
                ComboBoxItem n = new();
                n.Content = s;

                cb.Items.Add(n);
            }
        }
    }
}
