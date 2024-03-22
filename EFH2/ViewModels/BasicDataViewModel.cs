using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace EFH2
{
    public partial class BasicDataViewModel : ObservableObject
    {
        [ObservableProperty]
        [JsonPropertyName("Client")]
        private string _client = "";

        [ObservableProperty]
        [JsonPropertyName("Practice")]
        private string _practice = "";

        [ObservableProperty]
        [JsonPropertyName("By")]
        private string _by = "";

        [ObservableProperty]
        [JsonPropertyName("Date")]
        private Nullable<DateTimeOffset> _date = null;

        [JsonPropertyName("SelectedState")]
        private string _selectedState = "";

        [JsonPropertyName("SelectedCounty")]
        private string _selectedCounty = "";
    }
}
