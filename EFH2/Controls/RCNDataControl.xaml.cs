using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH2
{
    public sealed partial class RcnDataControl : UserControl
    {
        public event EventHandler<AcceptRcnValuesEventArgs>? AcceptRcnValues;

        public event EventHandler<RoutedEventArgs>? UnitsChanged;

        public RcnDataControl()
        {
            this.InitializeComponent();
        }

        public async void CreatePopup(MainViewModel model)
        {
            RcnDataConversionPage page = new RcnDataConversionPage();

            page.DataContext = model;
            page.SetDirection(AcresButton.IsChecked.GetValueOrDefault());

            ContentDialog contentDialog = new ContentDialog()
            {
                Content = page,
                Title = "RCN Data Conversion",
                CloseButtonText = "OK",
            };

            contentDialog.XamlRoot = this.Content.XamlRoot;

            await contentDialog.ShowAsync();

            page.FinalizeConversion();
        }

        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
        }

        private void AcceptButtonClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is RcnDataViewModel model)
            {
                this.AcceptRcnValues?.Invoke(this, new AcceptRcnValuesEventArgs(model.AccumulatedArea, model.WeightedCurveNumber));
            }
        }

        private void UnitsButtonsClicked(object sender, RoutedEventArgs e)
        {
            this.UnitsChanged?.Invoke(this, e);
        }

		private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            int index = 0;
			if (sender as RadioButton == DevUrbanAreaRadioButton) { index = 9; }
			else if (sender as RadioButton == CultivatedAgRadioButton) { index = 10; }
			else if (sender as RadioButton == OtherAgRadioButton) { index = 11; }
			else if (sender as RadioButton == AridRangelandRadioButton) { index = 12; }

            if (index < CategoriesListView.Items.Count && DataContext is RcnDataViewModel model)
            {
				var targetItem = model.RcnCategories[index];
				CategoriesListView.ScrollIntoView(targetItem);
            }
        }
    }
}
