using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH_2
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        private BasicDataPage _basicDataPage = new();

        private IntroPage _introPage = new();

        private RCNPage _rcnPage = new();

        private RDDataPage _rdDataPage = new();

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void Tabs_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            Type target;

            target = typeof(IntroPage);

            var selectedItem = (NavigationViewItem)args.SelectedItem;
            if (selectedItem != null)
            {
                string selectedItemTag = ((string)selectedItem.Tag);
                target = Type.GetType("EFH_2." + selectedItemTag);

                var reference = new Page();

                if (target == typeof(BasicDataPage))
                {
                    reference = _basicDataPage;
                }
                else if (target == typeof(RDDataPage))
                {
                    reference = _rdDataPage;
                }
                else if (target == typeof(RCNPage))
                {
                    reference = _rcnPage;
                }
                else 
                {
                    reference = _introPage; 
                }

                contentFrame.Navigate(target, reference);
            }
        }
    }
}
