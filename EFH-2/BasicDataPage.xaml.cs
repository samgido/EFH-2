using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using IronXL;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EFH_2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BasicDataPage : Page
    {

        private Dictionary<string, string> _stateCountyDictionary = new();

        private List<string> _stateAbbreviations = new();

        private string _clientEntry = "";

        private string _practiceEntry = "";

        private string _byEntry = "";

        private DateTime _dateEntry = new();

        public BasicDataPage()
        {
            this.InitializeComponent();

            WorkBook wb = WorkBook.Load("C:\\ProgramData\\USDA\\Shared Engineering Data\\RainFall_Data.xlsx");
            WorkSheet statesSheet = wb.GetWorkSheet("States");

            for (int i = 2; !statesSheet["A" + i.ToString()].IsEmpty; i++)
            {
                _stateAbbreviations.Add(statesSheet["B" + i.ToString()].StringValue);
            }

            ComboBoxOperations.PopulateComboBox(uxStateBox, _stateAbbreviations.ToArray());
        }
    }
}
