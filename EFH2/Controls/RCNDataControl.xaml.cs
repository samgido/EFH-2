using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI;
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

            UrbanAreaRadioButton.IsChecked = true;
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
                this.AcceptRcnValues?.Invoke(this, new AcceptRcnValuesEventArgs(model.AccumulatedArea, model.WeightedCurveNumber, AcresButton.IsChecked.GetValueOrDefault()));
            }
        }

        private void UnitsButtonsClicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null) ViewModel.AcresSelected = AcresButton.IsChecked.GetValueOrDefault();

            this.UnitsChanged?.Invoke(this, e);
        }

		private async void NavigationRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            if (sender as RadioButton == DevUrbanAreaRadioButton) { ScrollToIndex(5); }
            else if (sender as RadioButton == CultivatedAgRadioButton) { ScrollToIndex(6); }
            else if (sender as RadioButton == OtherAgRadioButton) { ScrollToIndex(7); }
            else if (sender as RadioButton == AridRangelandRadioButton) { ScrollToIndex(8); }
            else { ScrollToIndex(0); }
		}

        private async void ScrollToIndex(int index)
        {
            if (RcnCategoriesListView.Items.Count > 0 && RcnCategoriesListView != null)
            {
				await RcnCategoriesListView.SmoothScrollIntoViewWithIndexAsync(index, ScrollItemPlacement.Top, true);
            }
        }

		public void Default()
        {
            AcresButton.IsChecked = true;
            PercentageButton.IsChecked = false;
        }

        public void SetUnits(object sender, EventArgs e)
        {
            if (sender is MainViewModel model)
            {
                if (model.RcnDataViewModel.AcresSelected && !AcresButton.IsChecked.GetValueOrDefault()) // Model says acres selected, actual disagrees
                {
                    ToggleUnits();            
                }
                else if (!model.RcnDataViewModel.AcresSelected && AcresButton.IsChecked.GetValueOrDefault()) // Model says percent selected, actual disagrees
                {
                    ToggleUnits();
                }
            }
        }

        private void ToggleUnits()
        {
			if (AcresButton.IsChecked.GetValueOrDefault())
			{
				PercentageButton.IsChecked = true;
			}
			else if (PercentageButton.IsChecked.GetValueOrDefault())
			{
				AcresButton.IsChecked = true;
			}
        }
    }
}
