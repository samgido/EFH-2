/* ComboBoxOperations.cs
 * Author: Samuel Gido
 */

using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH_2
{
    /// <summary>
    /// Holds methods for combo boxes
    /// </summary>
    class ComboBoxOperations
    {
        /// <summary>
        /// Adds all elements from an array to a combo box
        /// </summary>
        /// <param name="cb">The target combo box</param>
        /// <param name="elements">The elements to be added to the ComboBox</param>
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

        /// <summary>
        /// Adds all elements from an array to a collection of ComboBoxItems
        /// </summary>
        /// <param name="coll">The collection</param>
        /// <param name="elements">The elements to be added to the collection</param>
        public static void PopulateComboBox(ObservableCollection<ComboBoxItem> coll, string[] elements)
        {
            if(coll.Count != 0) 
            {
                coll.Clear();
            }

            ComboBoxItem defaultItem = new();
            defaultItem.Content = MainWindow.ChooseMessage;

            coll.Add(defaultItem);
                
            foreach (string s in elements)
            {
                ComboBoxItem n = new();
                n.Content = s;

                coll.Add(n);
            }
        }
    }
}
