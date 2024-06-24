using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
            BasicDataViewModel = new BasicDataViewModel();
            RainfallDischargeDataViewModel = new RainfallDischargeDataViewModel();
            RcnDataViewModel = new RcnDataViewModel();

            RainfallDischargeDataViewModel.ValueChanged += CreateWinTr20InputFile;

            BasicDataViewModel.ValueChanged += CreateWinTr20InputFile;
            BasicDataViewModel.CountyChanged += BasicDataViewModel_CountyChanged;
        }

        private void CreateWinTr20InputFile(object sender, EventArgs e)
        {
            TryWinTr20();
        }

        public void TryWinTr20()
        {
            string fileName = FileOperations.CreateInpFile(this);

            // file being null doubles as a message that not all data is ready
            if (fileName != null)
            {
                FileOperations.RunWinTr20(fileName);
				FileOperations.ParseWinTR20Output(RainfallDischargeDataViewModel.Storms);
            }
            
        }

		private void BasicDataViewModel_CountyChanged(object sender, EventArgs e)
		{
            FileOperations.SearchForDataAfterCountyChanged(this, BasicDataViewModel.selectedState, BasicDataViewModel.selectedCounty);
            foreach (StormViewModel storm in RainfallDischargeDataViewModel.Storms) storm.DisplayHydrograph = false;
            RainfallDischargeDataViewModel.Storms[0].DisplayHydrograph = true;
            RainfallDischargeDataViewModel.Storms[3].DisplayHydrograph = true;
            RainfallDischargeDataViewModel.Storms[5].DisplayHydrograph = true;
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
