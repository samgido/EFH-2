using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HsgPage : Page
    {
        public HsgPage()
        {
            this.InitializeComponent();
        }

        private void SearchBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is RcnDataViewModel model)
            {
                string filter = (sender as TextBox).Text.ToUpper();

                uxDataGrid.ItemsSource = new ObservableCollection<HsgEntry>(
                    from item in model.HsgEntries where item.Field1.Contains(filter) select item);
            }
        }
    }
}
