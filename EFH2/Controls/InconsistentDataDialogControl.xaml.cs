using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH2
{
    public sealed partial class InconsistentDataDialogControl : UserControl
    {

        public MainViewModel MainViewModel
        {
            get
            {
                if (DataContext is MainViewModel model)
                {
                    return model;
                }
                return null;
            }
        }

        public InconsistentDataDialogControl()
        {
            this.InitializeComponent();
        }

        public async Task<ContentDialogResult> ShowAsync()
        {
            ContentDialogResult result = await this.InconsistentDataDialog.ShowAsync();
            return result;
        }
    }
}
