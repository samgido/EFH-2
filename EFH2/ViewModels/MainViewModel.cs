﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using EFH2.Models;
using Windows.Storage.Pickers;

namespace EFH2
{
    [XmlRoot("MainViewModel")]
    public class MainViewModel
    {
        public static bool WinTr20Ready = true;

        public event EventHandler<DisplayHydrographsReadyEventArgs> DisplayHydrographReady;

        public event EventHandler SyncRcnUnits;

        public const int NumberOfStorms = 10;

        public const string UserEnteredMessage = "User Entered.";

        public const string FromRcnCalculatorMessage = "from RCN Calculator";

        public const string CalculatedStatusMessage = "Calculated.";

        public const string ChooseMessage = "Choose...";

        public const string ClearedMessage = "Cleared.";

        private const string _importedStatusMessage = "Imported from file";

        public const PickerLocationId defaultFileLocation = PickerLocationId.DocumentsLibrary;

        public BasicDataViewModel BasicDataViewModel { get; set; }

        public RainfallDischargeDataViewModel RainfallDischargeDataViewModel { get; set; }

        // Only have to store areas and weights,
        public RcnDataModel RcnDataModel { get; set; }

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

            bool ready = false;
            foreach (StormViewModel storm in RainfallDischargeDataViewModel.Storms)
			{
                if (double.IsNaN(storm.Precipitation) || double.IsNaN(storm.Frequency) || double.IsNaN(storm.PeakFlow) || double.IsNaN(storm.Runoff)) continue;
                else
                {
                    ready = true;
					break;
                }
			}

            this.DisplayHydrographReady?.Invoke(this, new DisplayHydrographsReadyEventArgs(ready));
		}

        public async void TryWinTr20()
        {
            string inputContents = FileOperations.CreateInputFileContents(this);
			FileOperations.WriteToInputFile(inputContents);

            if (inputContents != null && MainViewModel.WinTr20Ready)
            {
				FileOperations.RunWinTr20Async();

				string outputContents = FileOperations.ReadOutputFile();

                if (!FileOperations.DidWinTr20HaveError())
                {
					FileOperations.ParseOutput(outputContents, RainfallDischargeDataViewModel.Storms);
                }
            }
        }

		private void BasicDataViewModel_CountyChanged(object sender, EventArgs e)
		{
            FileOperations.SearchForDataAfterCountyChanged(this, BasicDataViewModel.selectedState, BasicDataViewModel.selectedCounty);
            foreach (StormViewModel storm in RainfallDischargeDataViewModel.Storms) storm.DisplayHydrograph = false;
        }

		public void Load(SerializedDataModel newData)
        {
            BasicDataViewModel.Client = newData.Client;
            BasicDataViewModel.Practice = newData.Practice;
            BasicDataViewModel.By = newData.By;
            BasicDataViewModel.Date = newData.Date;

            BasicDataViewModel.SelectedStateIndex = (int)newData.SelectedStateIndex;
            BasicDataViewModel.SelectedCountyIndex = (int)newData.SelectedCountyIndex;

            BasicDataViewModel.drainageAreaEntry.SetSilent(newData.DrainageArea);
            BasicDataViewModel.runoffCurveNumberEntry.SetSilent(newData.RunoffCurveNumber);
            BasicDataViewModel.watershedLengthEntry.SetSilent(newData.WatershedLength);
            BasicDataViewModel.watershedSlopeEntry.SetSilent(newData.WatershedSlope);
            BasicDataViewModel.timeOfConcentrationEntry.SetSilent(newData.TimeOfConcentration);


            RainfallDischargeDataViewModel.SelectedRainfallDistributionTypeIndex = (int)newData.SelectedRainfallDistributionTypeIndex;
            RainfallDischargeDataViewModel.SelectedDuhTypeIndex = (int)newData.SelectedDuhTypeIndex;

            for (int i = 0; i < MainViewModel.NumberOfStorms; i++)
            {
                try
                {
                    RainfallDischargeDataViewModel.Storms[i].SetSilent(newData.Storms[i]);
                }
                catch { }
            }


            int j = 0;
            foreach (RcnCategory category in RcnDataViewModel.RcnCategories)
			{
				foreach (RcnRow row in category.AllRows)
				{
                    row.Entries[0].Area = newData.GroupA.Entries[j].Area;
                    row.Entries[1].Area = newData.GroupB.Entries[j].Area;
                    row.Entries[2].Area = newData.GroupC.Entries[j].Area;
                    row.Entries[3].Area = newData.GroupD.Entries[j].Area;
					j++;
				}
			}

			this.RcnDataViewModel.AcresSelected = newData.AcresSelected;
			this.SyncRcnUnits?.Invoke(this, EventArgs.Empty);

            TryWinTr20();
        }

        public SerializedDataModel CreateSerializableModel()
        {
            SerializedDataModel model = new();

            model.Client = BasicDataViewModel.Client;
            model.Practice = BasicDataViewModel.Practice;
            model.By = BasicDataViewModel.By;
            model.Date = BasicDataViewModel.Date;
            model.SelectedStateIndex = (uint)BasicDataViewModel.SelectedStateIndex;
            model.SelectedCountyIndex = (uint)BasicDataViewModel.SelectedCountyIndex;
            model.DrainageArea = BasicDataViewModel.DrainageArea;
            model.RunoffCurveNumber = BasicDataViewModel.RunoffCurveNumber;
            model.WatershedLength = BasicDataViewModel.WatershedLength;
            model.WatershedSlope = BasicDataViewModel.WatershedSlope;
            model.TimeOfConcentration = BasicDataViewModel.TimeOfConcentration;

            model.SelectedRainfallDistributionTypeIndex = (uint)RainfallDischargeDataViewModel.SelectedRainfallDistributionTypeIndex;
            model.SelectedDuhTypeIndex = (uint)RainfallDischargeDataViewModel.SelectedDuhTypeIndex;
            model.Storms = new List<SerializedStormModel>();

            foreach (StormViewModel storm in RainfallDischargeDataViewModel.Storms)
			{
				model.Storms.Add(new SerializedStormModel
				{
					Precipitation = storm.Precipitation,
					Frequency = storm.Frequency,
					PeakFlow = storm.PeakFlow,
					Runoff = storm.Runoff,
					DisplayHydrograph = storm.DisplayHydrograph
				});
			}

            model.GroupA = new RcnGroupModel();
            model.GroupB = new RcnGroupModel();
            model.GroupC = new RcnGroupModel();
            model.GroupD = new RcnGroupModel();

            foreach (RcnCategory category in RcnDataViewModel.RcnCategories)
            {
                foreach (RcnRow row in category.AllRows)
                {
					model.GroupA.Entries.Add(new RcnEntryModel()
					{
						Area = row.Entries[0].Area,
						Weight = row.Entries[0].Weight,
					});

					model.GroupB.Entries.Add(new RcnEntryModel()
					{
						Area = row.Entries[1].Area,
						Weight = row.Entries[1].Weight,
					});

					model.GroupC.Entries.Add(new RcnEntryModel()
					{
						Area = row.Entries[2].Area,
						Weight = row.Entries[2].Weight,
					});

					model.GroupD.Entries.Add(new RcnEntryModel()
					{
						Area = row.Entries[3].Area,
						Weight = row.Entries[3].Weight,
					});
                }
            }

            model.AcresSelected = RcnDataViewModel.AcresSelected;

            return model;
        }

        public void Default()
        {
            BasicDataViewModel.Default();
            RainfallDischargeDataViewModel.Default();
            RcnDataViewModel.Default();
        }
    }
}
