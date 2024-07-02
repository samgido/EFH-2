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
        }

        private void CreateCategoryItemsView()
        {
            ItemsView primaryItemsView = new ItemsView();

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(CreateColumn(8));
            grid.ColumnDefinitions.Add(CreateColumn(8));
            grid.ColumnDefinitions.Add(CreateColumn(4));
            grid.ColumnDefinitions.Add(CreateColumn(3));
            grid.ColumnDefinitions.Add(CreateColumn(1));
            grid.ColumnDefinitions.Add(CreateColumn(3));
            grid.ColumnDefinitions.Add(CreateColumn(1));
            grid.ColumnDefinitions.Add(CreateColumn(3));
            grid.ColumnDefinitions.Add(CreateColumn(1));
            grid.ColumnDefinitions.Add(CreateColumn(3));
            grid.ColumnDefinitions.Add(CreateColumn(1));

            TextBlock label = new TextBlock();
            Grid.SetColumn(label, 0);
            Grid.SetColumnSpan(label, 11);


            grid.Children.Add(label);
        }

        private ColumnDefinition CreateColumn(int weight)
        {
            return new ColumnDefinition() { Width = new GridLength(weight , GridUnitType.Star) };
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
			if (sender as RadioButton == DevUrbanAreaRadioButton) { index = 5; }
			else if (sender as RadioButton == CultivatedAgRadioButton) { index = 5; }
			else if (sender as RadioButton == OtherAgRadioButton) { index = 7; }
			else if (sender as RadioButton == AridRangelandRadioButton) { index = 8; }

            if (index < CategoriesItemsView.Items.Count && DataContext is RcnDataViewModel model)
            {
                var targetItem = model.RcnCategories[index];
                CategoriesItemsView.ScrollIntoView(targetItem);
            }
        }
    }
}
