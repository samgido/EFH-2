using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH2
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class HsgPage : Page, INotifyPropertyChanged
    {
        public HsgPage()
        {
            this.InitializeComponent();
        }

		public event PropertyChangedEventHandler PropertyChanged;

        private string _searchBox = "";

        private ObservableCollection<HsgEntryViewModel> _filteredHsgEntries = new ObservableCollection<HsgEntryViewModel>();
        public ObservableCollection<HsgEntryViewModel> FilteredHsgEntries
        {
            get
            {
                if (DataContext is RcnDataViewModel model)
                {
                    List<HsgEntryViewModel> filteredList = model.HsgEntries.Where(entry =>
                        entry.Row[0].Contains(_searchBox, StringComparison.CurrentCultureIgnoreCase) ||
                        entry.Row[1].Contains(_searchBox, StringComparison.CurrentCultureIgnoreCase) ||
                        entry.Row[2].Contains(_searchBox, StringComparison.CurrentCultureIgnoreCase)).ToList();

                    _filteredHsgEntries.Clear();
                    filteredList.ForEach(entry => _filteredHsgEntries.Add(entry));

                    return _filteredHsgEntries;
                }
                else return new ObservableCollection<HsgEntryViewModel>();
            }
        }

		private void TextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
		{
            _searchBox = sender.Text;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FilteredHsgEntries)));
		}
	}
}
