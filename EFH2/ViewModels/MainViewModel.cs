using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFH2
{
    public class MainViewModel
    {
        public const int NumberOfStorms = 9;

        public const string ChooseMessage = "Choose...";

        public const string ClearedMessage = "Cleared.";

        private const string _importedStatusMessage = "Imported from file";

        public static string DrainageAreaInvalidEntryMessage  = "Drainage area must be in the range 1 to 2000 acres!";
        public static string RunoffCurveNumberInvalidEntryMessage = "Curve number must be in the range 40 to 98!";
        public static string WatershedLengthInvalidEntryMessage = "Watershed length must be in the range 200 to 26000 feet!";
        public static string WatershedSlopeInvalidEntryMessage = "Watershed slope must be the range 0.5 and 64 percent!";
        public static string TimeOfConcentrationInvalidEntryMessage = "Time of concentration cannot be greater than 10.0 hours and cannot be less than 0.1 hours!";

        public BasicDataViewModel BasicDataViewModel { get; set; }

        public MainViewModel()
        {
            BasicDataViewModel = new BasicDataViewModel();

            using (StreamReader reader = new StreamReader("C:\\ProgramData\\USDA-dev\\Shared Engineering Data\\Rainfall_Data.csv"))
            {
                BasicDataViewModel.LoadStatesAndCounties(reader);
            }
        }
    }
}
