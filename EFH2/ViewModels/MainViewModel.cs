﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EFH2
{
    [XmlRoot("MainViewModel")]
    public class MainViewModel
    {
        [XmlIgnore]
        public const int NumberOfStorms = 9;

        [XmlIgnore]
        public const string UserEnteredMessage = "User Entered.";

        [XmlIgnore]
        public const string ChooseMessage = "Choose...";

        [XmlIgnore]
        public const string ClearedMessage = "Cleared.";

        [XmlIgnore]
        private const string _importedStatusMessage = "Imported from file";

        [XmlIgnore]
        public static string DrainageAreaInvalidEntryMessage  = "Drainage area must be in the range 1 to 2000 acres!";
        [XmlIgnore]
        public static string RunoffCurveNumberInvalidEntryMessage = "Curve number must be in the range 40 to 98!";
        [XmlIgnore]
        public static string WatershedLengthInvalidEntryMessage = "Watershed length must be in the range 200 to 26000 feet!";
        [XmlIgnore]
        public static string WatershedSlopeInvalidEntryMessage = "Watershed slope must be the range 0.5 and 64 percent!";
        [XmlIgnore]
        public static string TimeOfConcentrationInvalidEntryMessage = "Time of concentration cannot be greater than 10.0 hours and cannot be less than 0.1 hours!";

        [XmlElement("Basic Data")]
        public BasicDataViewModel BasicDataViewModel { get; set; }

        [XmlElement("Rainfall Discharge Data")]
        public RainfallDischargeDataViewModel RainfallDischargeDataViewModel { get; set; }

        // Only have to store areas and weights, probably don't have to store weights really
        [XmlElement("RCN Data")]
        public RcnDataModel RcnDataModel { get; set; }

        [XmlIgnore]
        public RcnDataViewModel RcnDataViewModel { get; set; }

        public MainViewModel()
        {
            BasicDataViewModel = new BasicDataViewModel();
            RainfallDischargeDataViewModel = new RainfallDischargeDataViewModel();
            RcnDataViewModel = new RcnDataViewModel();

            using (StreamReader reader = new StreamReader("C:\\ProgramData\\USDA-dev\\Shared Engineering Data\\Rainfall_Data.csv")) BasicDataViewModel.LoadStatesAndCounties(reader);

            using (StreamReader reader = new StreamReader("C:\\ProgramData\\USDA-dev\\Shared Engineering Data\\EFH2\\duh.txt")) RainfallDischargeDataViewModel.LoadDuhTypes(reader);
            using (StreamReader reader = new StreamReader("C:\\ProgramData\\USDA-dev\\Shared Engineering Data\\EFH2\\rftype.txt")) RainfallDischargeDataViewModel.LoadRainfallDistributionTypes(reader);

            using (StreamReader reader = new StreamReader("C:\\ProgramData\\USDA-dev\\Cover.txt")) RcnDataViewModel.LoadRCNTableEntries(reader);
            using (StreamReader reader = new StreamReader("C:\\ProgramData\\USDA-dev\\Shared Engineering Data\\EFH2\\SOILS.hg")) RcnDataViewModel.LoadHsgEntries(reader);
        }

        public void Load(MainViewModel newData)
        {
        }

        public void Default()
        {
            BasicDataViewModel.Default();
            RainfallDischargeDataViewModel.Default();
            RcnDataViewModel.Default();
        }
    }
}
