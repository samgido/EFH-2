using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH2
{
	public sealed partial class RcnDataControl : UserControl
    {
        public event EventHandler<AcceptRcnValuesEventArgs>? AcceptRcnValues;

        public event EventHandler<RoutedEventArgs>? UnitsChanged;

        public RcnDataViewModel ViewModel
        {
            get
            {
                if (DataContext is RcnDataViewModel model) return model;
                else return null; 
            }
        }

        public RcnDataControl()
        {
            this.InitializeComponent();

            CultivatedAgRadioButton.IsChecked = true;
            RcnCategoriesListView.ScrollIntoView(ViewModel?.RcnCategories[5]);
        }

        public async void CreateUnitChangePopup(MainViewModel model)
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
            this.ViewModel.Default();
            Default();
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

		private void NavigationRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            int index = 0;
			if (sender as RadioButton == DevUrbanAreaRadioButton) { index = 5; }
			else if (sender as RadioButton == CultivatedAgRadioButton) { index = 5; }
			else if (sender as RadioButton == OtherAgRadioButton) { index = 7; }
			else if (sender as RadioButton == AridRangelandRadioButton) { index = 8; }

            if (index < RcnCategoriesListView.Items.Count && DataContext is RcnDataViewModel model)
            {
                var targetItem = model.RcnCategories[index];
                RcnCategoriesListView.ScrollIntoView(targetItem);
            }
        }

        public void Default()
        {
            AcresButton.IsChecked = true;
            PercentageButton.IsChecked = false;
        }
    }
}
