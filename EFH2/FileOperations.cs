using System;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Xml.Serialization;
using System.IO;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using CommunityToolkit.WinUI.Helpers;
using Windows.Graphics.Printing;
using Microsoft.UI.Xaml;
using Windows.UI.Popups;
using System.Windows;

namespace EFH2
{
    public static class FileOperations
    {
        public static void SerializeData(MainViewModel model, TextWriter writer)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MainViewModel));
            serializer.Serialize(writer, model);
        }

        public static MainViewModel? DeserializeData(StreamReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MainViewModel));

			if (serializer.Deserialize(reader) is MainViewModel model) return model;
			else return null;
        }

		public static void LoadMainViewModel(MainViewModel model)
		{
			LoadBasicData(model.BasicDataViewModel);
			LoadRainfallDischargeData(model.RainfallDischargeDataViewModel);
			LoadRcnData(model.RcnDataViewModel);
		}

		private static void LoadBasicData(BasicDataViewModel model)
		{
            using (StreamReader reader = new StreamReader("C:\\ProgramData\\USDA-dev\\Shared Engineering Data\\Rainfall_Data.csv"))
			{
				reader.ReadLine();
				model.stateCountyDictionary.Add("Choose", new List<string>());

				while (!reader.EndOfStream)
				{
					string line = reader.ReadLine();

					string[] elements = line.Split(',');

					string state = elements[1];
					string county = elements[2].Trim('"');

					if (!model.stateCountyDictionary.ContainsKey(state))
					{ // found new state 

						model.stateCountyDictionary.Add(state, new());
						model.stateCountyDictionary[state].Add("Choose");
					}

					model.stateCountyDictionary[state].Add(county);
				}

				foreach (string state in model.stateCountyDictionary.Keys)
				{
					model.States.Add(new ComboBoxItem() { Content = state });
				}

			}
		}

		private static void LoadRainfallDischargeData(RainfallDischargeDataViewModel model)
		{
            using (StreamReader reader = new StreamReader("C:\\ProgramData\\USDA-dev\\Shared Engineering Data\\EFH2\\rftype.txt"))
			{
				ComboBoxItem c = new();
				c.Content = "";
				model.RainfallDistributionTypes.Clear();
				model.RainfallDistributionTypes.Add(c);

				string line = reader.ReadLine();

				while (!reader.EndOfStream)
				{
					string[] lineParts = line.Split(',');
					string type = lineParts[0];

					c = new();
					c.Content = type.Trim('"');

					model.RainfallDistributionTypes.Add(c);

					if (lineParts.Length == 2)
					{
						model.rfTypeToFileName.Add(type, lineParts[1].Trim('"'));
					}

					line = reader.ReadLine();
				}
				model.SelectedRainfallDistributionTypeIndex = 0;
			}

            using (StreamReader reader = new StreamReader("C:\\ProgramData\\USDA-dev\\Shared Engineering Data\\EFH2\\duh.txt"))
			{
				model.DuhTypes.Clear();
				string line = reader.ReadLine();

				while (!reader.EndOfStream)
				{
					ComboBoxItem c = new();
					c.Content = line;

					model.DuhTypes.Add(c);
					line = reader.ReadLine();
				}
				model.SelectedDuhTypeIndex = 0;
			}
		}

		private static void LoadRcnData(RcnDataViewModel model)
		{
			using (StreamReader reader = new StreamReader("C:\\ProgramData\\USDA-dev\\Cover2.txt"))
			{
				List<RcnCategory> topCategories = new List<RcnCategory>();
				RcnCategory topCategory = null;
				List<RcnRow> currentRowList = null;

				while (!reader.EndOfStream)
				{
					string line = reader.ReadLine();
					string[] elements = line.Split('\t');

					if (elements[0].Contains('*')) // New 
					{
						if (topCategory != null) topCategories.Add(topCategory);
						topCategory = new RcnCategory();
						topCategory.Label = elements[1].Trim('"');
						currentRowList = topCategory.Rows;
					}
					else if (elements[1] != string.Empty)
					{
						RcnCategory newSubCategory = new RcnCategory()
						{
							Label = elements[1].Trim('"'),
							Extra = elements[3]
						};
						currentRowList = newSubCategory.Rows;
						topCategory.RcnSubcategories.Add(newSubCategory);
					}
					else if (elements[5] != string.Empty)
					{
						RcnRow newRow = new RcnRow();
						newRow.Text = elements[2];

						//int.TryParse(elements[5], out int weight1);
						//newRow.Entries[0].Weight = weight1;

						if (elements[5] == "**") newRow.Entries[0].Weight = -1;
						else
						{
							int.TryParse(elements[5], out int weight1);
							newRow.Entries[0].Weight = weight1;
						}

						int.TryParse(elements[7], out int weight2);
						newRow.Entries[1].Weight = weight2;

						int.TryParse(elements[9], out int weight3);
						newRow.Entries[2].Weight = weight3;

						int.TryParse(elements[11], out int weight4);
						newRow.Entries[3].Weight = weight4;

						currentRowList.Add(newRow);
					}
				}
				topCategories.Add(topCategory);
				model.RcnCategories = topCategories;

				foreach (RcnCategory category in model.RcnCategories)
				{
					foreach (RcnRow row in category.AllRows)
					{
                        foreach (var entry in row.Entries)
                        {
							entry.PropertyChanged += model.EntryChanged; 
                        }
                    }
				}
			}

			using (StreamReader reader = new StreamReader("C:\\ProgramData\\USDA-dev\\Shared Engineering Data\\EFH2\\SOILS.hg"))
			{
				while (!reader.EndOfStream)
				{
					string line = reader.ReadLine();

					string[] lineParts = line.Split("\t");

					if (lineParts.Length == 3)
					{
						model.HsgEntries.Add(new HsgEntryViewModel()
						{
							Row = new ObservableCollection<string>
							{
								lineParts[0],
								lineParts[1],
								lineParts[2]
							}
						});
					}
				}
			}
		}

        public static string? CreateInpFile(MainViewModel model)
		{
			try
			{
				if (!IsWinTR20Ready(model)) return null;

				string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				string filePath = Path.Combine(appDataPath, "EFH2\\tr20.inp");

				using (StreamWriter writer = new StreamWriter(filePath, append: false))
				{
					StringBuilder content = new StringBuilder();

					writer.WriteLine(String.Format("WinTR-20: {0,-30}{1,-10}{2,-10}{3,-10}{4}", "Version 3.30", 0, 0, 0.01, 0));
					writer.WriteLine("Single watershed using lag method for Tc");
					writer.WriteLine("");
					writer.WriteLine("SUB-AREA:");
					writer.WriteLine(String.Format("          {0,-10}{1,-20}{2,-10}{3,-10}{4,-10}", "Area", "Outlet",
						(model.BasicDataViewModel.DrainageArea / 640).ToString("0.00000"),
						(model.BasicDataViewModel.RunoffCurveNumber + "."),
						model.BasicDataViewModel.TimeOfConcentration.ToString("0.00")));

					writer.WriteLine("");
					writer.WriteLine("");
					writer.WriteLine("");
					writer.WriteLine("STORM ANALYSIS:");
					foreach (StormViewModel storm in model.RainfallDischargeDataViewModel.Storms)
					{
						if (!double.IsNaN(storm.Frequency) && !double.IsNaN(storm.DayRain))
						{
							writer.WriteLine(String.Format("          {0,-30}{1,-10}{2,-10}{3}",
								storm.Frequency + "-Yr",
								storm.DayRain.ToString("0.0"),
								model.RainfallDischargeDataViewModel.selectedRainfallDistributionType,
								2));
						}
					}
					writer.WriteLine("");
					writer.WriteLine("RAINFALL DISTRIBUTION:");

					string programX86Path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

					string rainfallDistributionTypeFilePath = Path.Combine(programX86Path,
						"USDA\\Shared Engineering Data\\EFH2\\RainfallDistributions\\Type " + model.RainfallDischargeDataViewModel.selectedRainfallDistributionType + ".tbl");

					if (File.Exists(rainfallDistributionTypeFilePath))
					{
						writer.WriteLine(File.ReadAllText(rainfallDistributionTypeFilePath));
					}

					if (model.RainfallDischargeDataViewModel.SelectedDuhTypeIndex != 0)
					{
						writer.WriteLine("DIMENSIONLESS UNIT HYDROGRAPH:");

						string duhTypeFilePath = Path.Combine(programX86Path,
							"USDA\\Shared Engineering Data\\EFH2\\DimensionlessUnitHydrographs\\" + model.RainfallDischargeDataViewModel.selectedDuhType + ".duh");

						if (File.Exists(duhTypeFilePath))
						{
							using (StreamReader reader = new StreamReader(duhTypeFilePath, Encoding.UTF8))
							{
								reader.ReadLine();

								writer.Write(reader.ReadToEnd());
							}
						}
					}
					else
					{
						for (int i = 0; i < 11; i++) writer.WriteLine();
					}

					writer.WriteLine("GLOBAL OUTPUT:");
					writer.WriteLine(String.Format("          {0,-10}{1,-20}{2,-10}{3,-10}", 2, 0.01, "YY  Y", "NN  N"));

					RunWinTr20(filePath);

					return filePath;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}

			return null;
		}

		public static void RunWinTr20(string filePath)
		{
			try
			{
				string executableDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\USDA\\EFH2\\";
				string executableName = "WinTR20_V32.exe";

				ProcessStartInfo psi = new ProcessStartInfo()
				{
					FileName = executableDirectory + executableName,
					Arguments = filePath,
					RedirectStandardOutput = true,
					CreateNoWindow = true,
					UseShellExecute = false,
				};

				using (Process process = new Process() { StartInfo = psi })
				{
					process.Start();
					//while (!process.StandardOutput.EndOfStream) Debug.WriteLine(process.StandardOutput.ReadLine());

					process.WaitForExit();
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// Checks the main view model if all the data is ready to run through the WinTR20 program
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public static bool IsWinTR20Ready(MainViewModel model)
		{
			// If rainfall distribution type has been selected
			if (model.RainfallDischargeDataViewModel.SelectedRainfallDistributionTypeIndex == 0) return false;

			// Basic data check 
			if (double.IsNaN(model.BasicDataViewModel.drainageAreaEntry.Value)) return false;
			if (double.IsNaN(model.BasicDataViewModel.runoffCurveNumberEntry.Value)) return false;
			if (double.IsNaN(model.BasicDataViewModel.timeOfConcentrationEntry.Value)) return false;

			// If everything is ready
			return true;
		}

		/// <summary>
		/// Reads the peak-flow and runoff values from the tr20.out file and inserts them into the view model
		/// </summary>
		/// <param name="model"></param>
		public static void ParseWinTR20Output(IEnumerable<StormViewModel> storms)
		{
			try
			{
				string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				string filePath = Path.Combine(appDataPath, "EFH2\\tr20.out");

				foreach (StormViewModel storm in storms) storm.PeakFlow = storm.Runoff = double.NaN;

				if (File.Exists(filePath))
				{
					using (StreamReader reader = new StreamReader(filePath))
					{
						while (!reader.EndOfStream)
						{
							string line = reader.ReadLine();
							string[] splitLine = line.Split().Where(str => !string.IsNullOrEmpty(str)).ToArray();

							if (splitLine.Length == 2 && splitLine[1].Contains("-Yr"))
							{ // Now at the line "___STORM_##-YR____"
								string yrLabel = splitLine[1].Replace("-Yr", "");
								int year = int.Parse(yrLabel);

								splitLine = line.Split().Where(str => !string.IsNullOrEmpty(str)).ToArray();
								while (splitLine.Length != 6 || splitLine[0] != "Area")
								{ // At the line with the data, runoff should be the 3rd element and peak flow should be the 5th
									splitLine = SplitLine(reader.ReadLine()).ToArray();
								}

								double runoff = Math.Round(double.Parse(splitLine[2]), 2);
								double peakFlow = Math.Round(double.Parse(splitLine[4]), 2);

								foreach (StormViewModel storm in storms)
								{
									if (storm.Frequency == year && !double.IsNaN(storm.DayRain))
									{// found the match, put the runoff and peakflow values into this storm
										storm.PeakFlow = peakFlow;
										storm.Runoff = runoff;
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		public static List<HydrographLineModel> GetHydrographData(IEnumerable<StormViewModel> storms)
		{
			// First find all storms, specifically their frequencies, that need to be plotted
			List<int> plottedFrequencies = new List<int>();
			foreach (StormViewModel storm in storms) if (storm.DisplayHydrograph) plottedFrequencies.Add((int)storm.Frequency);

			List<HydrographLineModel> list = new List<HydrographLineModel>();

			string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			string filePath = Path.Combine(appDataPath, "EFH2\\tr20.hyd");

			if (File.Exists(filePath))
			{
				using (StreamReader reader = new StreamReader(filePath))
				{
					while (!reader.EndOfStream)
					{
						string line = reader.ReadLine();
						List<string> splitLine = SplitLine(line);

						if (splitLine.Count == 7 && splitLine[0] == "Area" && plottedFrequencies.Contains(int.Parse(splitLine[2].Replace("-Yr", ""))))
						{
							int frequency = int.Parse(splitLine[2].Replace("-Yr", ""));
							double startTime = double.Parse(splitLine[3]);
							double increment = double.Parse(splitLine[4]);

							List<double> values = new List<double>();

							line = reader.ReadLine(); 
							splitLine = SplitLine(line);
							while (splitLine.Count == 5)
							{
								foreach (string val in splitLine)
								{
									values.Add(double.Parse(val));
								}

								line = reader.ReadLine(); 
								splitLine = SplitLine(line);
							}

							list.Add(new HydrographLineModel(frequency, startTime, increment, values));
						}
					}
				}
			}

			return list;
		}

		public static void SearchForDataAfterCountyChanged(RainfallDischargeDataViewModel model, string state, string county)
		{
			string[] typesThatNeedFormatting = new string[] { "I", "II", "IA", "III", "N Pac" };
			double[] automaticStormFrequencies = new double[] { 1, 2, 5, 10, 25, 50, 100 };

            using (StreamReader reader = new StreamReader("C:\\ProgramData\\USDA-dev\\Shared Engineering Data\\Rainfall_Data.csv"))
			{
				while (!reader.EndOfStream)
				{
					string line = reader.ReadLine();
					string[] elements = line.Split(',');

					if (elements.Length == 11 && elements[1].Trim('"') == state && elements[2].Trim('"') == county)
					{
						string rfType = elements[3];
						if (typesThatNeedFormatting.Contains(rfType)) rfType = "Type " + rfType;

						model.SetRainfallType(rfType);

						for (int i = 0; i < 7; i++)
						{
							double dayRain = double.Parse(elements[4 + i]);
							if (dayRain != 0)
							{
								model.Storms[i].DayRain = dayRain;
								model.Storms[i].Frequency = automaticStormFrequencies[i];
							}
						}
					}
				}
			}
		}

		private static List<string> SplitLine(string line)
		{
			return line.Split().Where(elem => !string.IsNullOrEmpty(elem)).ToList();
		}
	}
}
