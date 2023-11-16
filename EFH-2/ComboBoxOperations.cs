using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2
{
    class ComboBoxOperations
    {
        /// <summary>
        /// Adds all elements from an array to a combo box
        /// </summary>
        /// <param name="cb">The target combo box</param>
        /// <param name="elements"></param>
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

        public static void PopulateComboBox(ObservableCollection<ComboBoxItem> coll, string[] elements)
        {
            coll.Clear();
                
            foreach (string s in elements)
            {
                ComboBoxItem n = new();
                n.Content = s;

                coll.Add(n);
            }
        }
    }
}
