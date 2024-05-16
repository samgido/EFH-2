using System;
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

        [XmlElement("Basic Data")]
        public BasicDataViewModel BasicDataViewModel { get; set; }

        [XmlElement("Rainfall Discharge Data")]
        public RainfallDischargeDataViewModel RainfallDischargeDataViewModel { get; set; }

        // Only have to store areas and weights,
        [XmlElement("RCN Data")]
        public RcnDataModel RcnDataModel { get; set; }

        [XmlIgnore]
        public RcnDataViewModel RcnDataViewModel { get; set; }

        public MainViewModel()
        {
            FileOperations.LoadMainViewModel(this);
        }

        public void Load(MainViewModel newData)
        {
            BasicDataViewModel.Load(newData.BasicDataViewModel);
            RcnDataViewModel.LoadDataModel(newData.RcnDataModel);
            RainfallDischargeDataViewModel.Load(newData.RainfallDischargeDataViewModel);
        }

        public void Default()
        {
            BasicDataViewModel.Default();
            RainfallDischargeDataViewModel.Default();
            RcnDataViewModel.Default();
        }
    }
}
