using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using WinRT;
using EFH2.Models;
using System.Threading.Tasks;
using Microsoft.ServiceHub.Resources;
using Microsoft;
using PdfSharp.Snippets;
using Windows.System;

namespace EFH2
{
	public static class FileOperations
    {
		private static string ProgramDataDirectory => Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
		private static string AppDataDirectory => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		public static string ProgramFilesDirectory => Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

		public static string MainExecutableDirectory => Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Replace(@"file:\", "");
		public static string HelpFileDirectory => Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase), "Assets", "HelpFiles").Replace(@"file:\", "");

		private static string WinTr20Path => Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase), "WinTR20_V32.exe").Replace(@"file:\", "");		
		private static string InputFilePath => Path.Combine(AppDataDirectory, "EFH2", "tr20.inp");
		private static string OutputFilePath => Path.Combine(AppDataDirectory, "EFH2", "tr20.out");

		public static string companyName = "USDA";

		public static bool WinTr20Found => File.Exists(WinTr20Path);

		/// <summary>
		/// Ensures all required folders and files are present
		/// </summary>
		public static bool InitEfh2Structure()
		{
			string inputDir = Directory.GetParent(InputFilePath).FullName;

			if (!Directory.Exists(inputDir))
			{
				Directory.CreateDirectory(inputDir);
				App.LogMessage($"Created directory: {inputDir}");
			}

			if (!File.Exists(InputFilePath))
			{
				File.Create(InputFilePath).Close();
				App.LogMessage($"Created blank input file at: {InputFilePath}");
			}

			LogFileExistance("WinTR20_V32.exe", Directory.GetParent(WinTr20Path).FullName);
			LogFileExistance("Rainfall_data.csv", Path.Combine(ProgramDataDirectory, companyName, "Shared Engineering Data", "EFH2"));
			LogFileExistance("rftype.txt", Path.Combine(ProgramDataDirectory, companyName, "Shared Engineering Data", "EFH2"));
			LogFileExistance("duh.txt", Path.Combine(ProgramDataDirectory, companyName, "Shared Engineering Data", "EFH2"));
			LogFileExistance("COVER.txt", Path.Combine(ProgramDataDirectory, companyName, "Shared Engineering Data", "EFH2"));
			LogFileExistance("SOILS.hg", Path.Combine(ProgramDataDirectory, companyName, "Shared Engineering Data", "EFH2"));

			string rainfallDistributionsDirectory = Path.Combine(ProgramDataDirectory, companyName, "Shared Engineering Data", "EFH2", "RainfallDistributions");
			if (Directory.Exists(rainfallDistributionsDirectory))
			{
				App.LogMessage("RainfallDistributions directory found at " + rainfallDistributionsDirectory);

				// Not going to check for all files, just to see if there are the correct amount of files
				int directoryItemsCount = Directory.GetFiles(rainfallDistributionsDirectory).Length;
				int expected = 52;

				App.LogMessage("RainfallDistributions has " + directoryItemsCount + " files, expected " + expected);
			}
			else
			{
				App.LogMessage("RainfallDistributions directory not found at " + rainfallDistributionsDirectory);
			}

			string duhDirectory = Path.Combine(ProgramDataDirectory, companyName, "Shared Engineering Data", "EFH2", "DimensionlessUnitHydrographs");
			if (Directory.Exists(duhDirectory))
			{
				App.LogMessage("DimensionlessUnitHydrographs directory found at " + duhDirectory);

				// Not going to check for all files, just to see if there are the correct amount of files
				int directoryItemsCount = Directory.GetFiles(duhDirectory).Length;
				int expected = 13;

				App.LogMessage("DimensionlessUnitHydrographs has " + directoryItemsCount + " files, expected " + expected);
			}
			else
			{
				App.LogMessage("DimensionlessUnitHydrographs directory not found at " + duhDirectory);
			}

			return true;
		}

		private static void LogFileExistance(string filename, string fileDirectory)
		{
			string filePath = Path.Combine(fileDirectory, filename);
			bool fileExists = File.Exists(filePath);
			App.LogMessage(filename + " " + (fileExists ? "found" : "not found") + " at " + filePath);
		}

        public static void SerializeData(MainViewModel model, TextWriter writer)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SerializedDataModel));
            serializer.Serialize(writer, model.CreateSerializableModel());
        }

        public static SerializedDataModel? DeserializeData(StreamReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SerializedDataModel));

			if (serializer.Deserialize(reader) is SerializedDataModel model) return model;
			else return null;
        }

		public static void LoadMainViewModel(MainViewModel model)
		{
			try
			{
				LoadBasicData(model.BasicDataViewModel);
				LoadRainfallDischargeData(model.RainfallDischargeDataViewModel);
				LoadRcnData(model.RcnDataViewModel);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				App.LogException("Loading main view model", ex);
			}
		}

		private static void LoadBasicData(BasicDataViewModel model)
		{
			try
			{
				string rainfallDataPath = Path.Combine(ProgramDataDirectory, companyName, "Shared Engineering Data", "EFH2", "Rainfall_data.csv");
				using (TextFieldParser parser = new TextFieldParser(rainfallDataPath))
				{
					parser.TextFieldType = FieldType.Delimited;
					parser.SetDelimiters(",");

					while (!parser.EndOfData)
					{
						string[] fields = parser.ReadFields();

						string state = fields[1];
						string county = fields[2].Trim('"');

						if (!model.stateCountyDictionary.ContainsKey(state))
						{ // found new state
							model.stateCountyDictionary.Add(state, new List<string>());
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
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				App.LogException("Load Basic Data", ex);
			}
		}

		private static void LoadRainfallDischargeData(RainfallDischargeDataViewModel model)
		{
			try
			{
				string rftypeDataPath = Path.Combine(ProgramDataDirectory, companyName, "Shared Engineering Data", "EFH2", "rftype.txt");
				using (StreamReader reader = new StreamReader(rftypeDataPath))
				{
					ComboBoxItem c = new();
					c.Content = "";
					model.RainfallDistributionTypes.Clear();
					model.RainfallDistributionTypes.Add(c);

					string line = reader.ReadLine();

					while (!reader.EndOfStream)
					{
						if (line == "") break;

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

				string duhtypeDataPath = Path.Combine(ProgramDataDirectory, companyName, "Shared Engineering Data", "EFH2", "duh.txt");
				using (StreamReader reader = new StreamReader(duhtypeDataPath))
				{
					model.DuhTypes.Clear();
					string line = reader.ReadLine();

					while (!reader.EndOfStream)
					{
						if (line.Trim() == "") break;

						ComboBoxItem c = new();
						c.Content = line;

						model.DuhTypes.Add(c);
						line = reader.ReadLine();
					}

					model.SelectedDuhTypeIndex = 0;
				}

			}
			catch (Exception ex) 
			{ 
				Debug.WriteLine(ex.Message); 
				App.LogException("Load Rainfall Discharge Data", ex);
			}
		}

		private static void LoadRcnData(RcnDataViewModel model)
		{
			try
			{
				string coverPath = Path.Combine(ProgramDataDirectory, companyName, "Shared Engineering Data", "EFH2", "COVER.txt");
				using (StreamReader reader = new StreamReader(coverPath))
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
							topCategory.Extra = elements[3].Trim();
							
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

							if (elements[3] != string.Empty) newRow.Quality = elements[3];

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

				string soilsPath = Path.Combine(ProgramDataDirectory, companyName, "Shared Engineering Data", "EFH2", "SOILS.hg");
				if (!File.Exists(soilsPath))
				{
					App.LogException("Load RCN data", new Exception($"SOILS.hg not found at {soilsPath}"));

					return;
				}
				else
				{
					using (StreamReader reader = new StreamReader(soilsPath))
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

			}
			catch (Exception ex) 
			{ 
				Debug.WriteLine(ex.Message);
				App.LogException("Load RCN data", ex);
			}
		}

        public static async Task<bool> CreateInpFileAsync(MainViewModel model)
		{
			try
			{
				if (!IsWinTR20Ready(model)) return false;

				StringBuilder input = new StringBuilder();
				string rainfallDistributionTable = "";
				string rainfallDistributionType = "";

				string rainfallDistributionTypeFilePath = Path.Combine(ProgramDataDirectory, companyName, "Shared Engineering Data", "EFH2", "RainfallDistributions", "Type " + model.RainfallDischargeDataViewModel.selectedRainfallDistributionType + ".tbl");

				if (File.Exists(rainfallDistributionTypeFilePath))
				{
					rainfallDistributionTable = File.ReadAllText(rainfallDistributionTypeFilePath);

					string[] parts = rainfallDistributionTable.Trim().Split();
					rainfallDistributionType = parts[0];
				}

				input.AppendLine(String.Format("WinTR-20: {0,-30}{1,-10}{2,-10}{3,-10}{4}", "Version 3.30", 0, 0, 0.01, 0));
				input.AppendLine("Single watershed using lag method for Tc");
				input.AppendLine("");
				input.AppendLine("SUB-AREA:");
				input.AppendLine(String.Format("          {0,-10}{1,-20}{2,-10}{3,-10}{4,-10}", "Area", "Outlet",
					(model.BasicDataViewModel.DrainageArea / 640).ToString("0.00000"),
					(model.BasicDataViewModel.RunoffCurveNumber + "."),
					model.BasicDataViewModel.TimeOfConcentration.ToString("0.00")));

				input.AppendLine("");
				input.AppendLine("");
				input.AppendLine("");
				input.AppendLine("STORM ANALYSIS:");
				foreach (StormViewModel storm in model.RainfallDischargeDataViewModel.Storms)
				{
					if (!double.IsNaN(storm.Frequency) && !double.IsNaN(storm.Precipitation))
					{
						input.AppendLine(String.Format("          {0,-30}{1,-10}{2,-10}{3}",
							storm.Frequency + "-Yr",
							storm.Precipitation.ToString("0.0"),
							rainfallDistributionType,
							2));
					}
				}
				input.AppendLine("");
				input.AppendLine("RAINFALL DISTRIBUTION:");

				if (rainfallDistributionTable != string.Empty)
				{
					input.AppendLine(rainfallDistributionTable);
				}

				if (model.RainfallDischargeDataViewModel.SelectedDuhTypeIndex != 0)
				{
					input.AppendLine("DIMENSIONLESS UNIT HYDROGRAPH:");

					string duhTypeFilePath = Path.Combine(ProgramDataDirectory, companyName, "Shared Engineering Data", "EFH2", "DimensionlessUnitHydrographs", model.RainfallDischargeDataViewModel.selectedDuhType + ".duh");
					if (File.Exists(duhTypeFilePath))
					{
						input.Append(File.ReadAllText(duhTypeFilePath));
					}
				}
				else
				{
					for (int i = 0; i < 11; i++) input.AppendLine();
				}

				input.AppendLine("GLOBAL OUTPUT:");
				input.AppendLine(String.Format("          {0,-10}{1,-20}{2,-10}{3,-10}", 2, 0.01, "YY  Y", "NN  N"));

				await File.WriteAllTextAsync(InputFilePath, input.ToString());

				Debug.WriteLine("Writing to .inp finished");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				App.LogException("Creating input file", ex);
			}

			return false;
		}

		public static string CreateInputFileContents(MainViewModel model)
		{
			if (!IsWinTR20Ready(model)) return null;

			StringBuilder input = new StringBuilder();
			string rainfallDistributionTable = "";
			string rainfallDistributionType = "";

			string rainfallDistributionTypeFilePath = Path.Combine(ProgramDataDirectory, companyName, "Shared Engineering Data", "EFH2", "RainfallDistributions", "Type " + model.RainfallDischargeDataViewModel.selectedRainfallDistributionType + ".tbl");

			if (File.Exists(rainfallDistributionTypeFilePath))
			{
				rainfallDistributionTable = File.ReadAllText(rainfallDistributionTypeFilePath);

				string[] parts = rainfallDistributionTable.Trim().Split();
				rainfallDistributionType = parts[0];
			}

			input.AppendLine(String.Format("WinTR-20: {0,-30}{1,-10}{2,-10}{3,-10}{4}", "Version 3.30", 0, 0, 0.01, 0));
			input.AppendLine("Single watershed using lag method for Tc");
			input.AppendLine("");
			input.AppendLine("SUB-AREA:");
			input.AppendLine(String.Format("          {0,-10}{1,-20}{2,-10}{3,-10}{4,-10}", "Area", "Outlet",
				(model.BasicDataViewModel.DrainageArea / 640).ToString("0.00000"),
				(model.BasicDataViewModel.RunoffCurveNumber + "."),
				model.BasicDataViewModel.TimeOfConcentration.ToString("0.00")));

			input.AppendLine("");
			input.AppendLine("");
			input.AppendLine("");
			input.AppendLine("STORM ANALYSIS:");
			foreach (StormViewModel storm in model.RainfallDischargeDataViewModel.Storms)
			{
				if (!double.IsNaN(storm.Frequency) && !double.IsNaN(storm.Precipitation))
				{
					input.AppendLine(String.Format("          {0,-30}{1,-10}{2,-10}{3}",
						storm.Frequency + "-Yr",
						storm.Precipitation.ToString("0.000"),
						rainfallDistributionType,
						2));
				}
			}
			input.AppendLine("");
			input.AppendLine("RAINFALL DISTRIBUTION:");

			if (rainfallDistributionTable != string.Empty)
			{
				input.AppendLine(rainfallDistributionTable);
			}

			if (model.RainfallDischargeDataViewModel.SelectedDuhTypeIndex != 0)
			{
				input.AppendLine("DIMENSIONLESS UNIT HYDROGRAPH:");

				string duhTypeFilePath = Path.Combine(ProgramDataDirectory, companyName, "Shared Engineering Data", "EFH2", "DimensionlessUnitHydrographs", model.RainfallDischargeDataViewModel.selectedDuhType + ".duh");
				if (File.Exists(duhTypeFilePath))
				{
					input.Append(File.ReadAllText(duhTypeFilePath));
				}
			}
			else
			{
				for (int i = 0; i < 11; i++) input.AppendLine();
			}

			input.AppendLine("GLOBAL OUTPUT:");
			input.AppendLine(String.Format("          {0,-10}{1,-20}{2,-10}{3,-10}", 2, 0.01, "YY  Y", "NN  N"));

			return input.ToString();
		}

		public static async void RunWinTr20Async()
		{
			try
			{
				Process process = new Process();
				ProcessStartInfo startInfo = new ProcessStartInfo();

				string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

				if (!File.Exists(WinTr20Path))
				{
					Debug.WriteLine("WinTr20 not found at: " + WinTr20Path);
					App.LogMessage("WinTr20 not found at: " + WinTr20Path);
					return;
				}
				else
				{
					Debug.WriteLine("Found WinTr20 executable file");
					App.LogMessage("Found WinTr20 executable file");
				}

				string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

				if (!File.Exists(InputFilePath))
				{
					Debug.WriteLine("Input file not found at: " + InputFilePath);
					App.LogMessage("Input file not found at: " + InputFilePath);
					return;
				}
				else
				{
					Debug.WriteLine("Found input file");
					App.LogMessage("Found input file");
				}

				startInfo.FileName = WinTr20Path;
				startInfo.Arguments = InputFilePath;
				startInfo.CreateNoWindow = true;

				process.StartInfo = startInfo;

				if (process.Start())
				{
					Debug.WriteLine("Process started");
					App.LogMessage(startInfo.ToString());
				}

				process.WaitForExit();

				process.Kill();

				Debug.WriteLine("Process finished");
				App.LogMessage("Process finished");
			}
			catch (Exception ex)
			{
				App.LogException("Running WinTR20", ex);
				Debug.WriteLine("Running WinTr20 failed, check log file");
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
		public static async void ParseWinTR20Output(IEnumerable<StormViewModel> storms)
		{
			foreach (StormViewModel storm in storms) storm.PeakFlow = storm.Runoff = double.NaN;

			if (!File.Exists(OutputFilePath))
			{
				App.LogMessage("Couldn't find output file at: " + OutputFilePath);
				return;
			}

			string output = await File.ReadAllTextAsync(OutputFilePath);

			foreach (string line in output.Split("\n"))
			{
				string[] splitLine = line.Split().Where(str => !string.IsNullOrEmpty(str)).ToArray();

				if (splitLine.Length == 2 && splitLine[1].Contains("-Yr"))
				{ // Now at the line "___STORM_##-YR____"
					string yrLabel = splitLine[1].Replace("-Yr", "");
					int year = int.Parse(yrLabel);

					splitLine = line.Split().Where(str => !string.IsNullOrEmpty(str)).ToArray();
					while (splitLine.Length != 6 || splitLine[0] != "Area")
					{ // At the line with the data, runoff should be the 3rd element and peak flow should be the 5th
						splitLine = SplitLine(line).ToArray();
					}

					double runoff = Math.Round(double.Parse(splitLine[2]), 2);
					double peakFlow = Math.Round(double.Parse(splitLine[4]), 2);

					foreach (StormViewModel storm in storms)
					{
						if (storm.Frequency == year && !double.IsNaN(storm.Precipitation))
						{ // found the match, put the runoff and peakflow values into this storm
							storm.PeakFlow = peakFlow;
							storm.Runoff = runoff;
						}
					}
				
				}
			}
		}

		public static void WriteToInputFile(string contents)
		{
			if (!File.Exists(InputFilePath)) File.Create(InputFilePath);

			File.WriteAllText(InputFilePath, contents);
		}

		public static string ReadOutputFile()
		{
			if (File.Exists(OutputFilePath))
			{
				return File.ReadAllText(OutputFilePath);
			}
			else
			{
				App.LogException("Reading output file", new Exception($"Output file not found at {OutputFilePath}"));
				return null;
			}
		}

		public static void ParseOutput(string output, IEnumerable<StormViewModel> storms)
		{
			try
			{
				Debug.WriteLine("Parsing started");

				foreach (StormViewModel storm in storms) storm.PeakFlow = storm.Runoff = double.NaN;

				string[] lines = output.Split("\n");
				//foreach (string line in output.Split("\n"))
				for (int i = 0; i < lines.Count(); i++)
				{
					string line = lines[i];
					string[] splitLine = line.Split().Where(str => !string.IsNullOrEmpty(str)).ToArray();

					if (splitLine.Length == 2 && splitLine[1].Contains("-Yr"))
					{ // Now at the line "___STORM_##-YR____"
						string yrLabel = splitLine[1].Replace("-Yr", "");
						int year = int.Parse(yrLabel);

						splitLine = line.Split().Where(str => !string.IsNullOrEmpty(str)).ToArray();
						while (splitLine.Length != 6 || splitLine[0] != "Area")
						{ // At the line with the data, runoff should be the 3rd element and peak flow should be the 5th
							line = lines[i++];
							splitLine = SplitLine(line).ToArray();
						}

						double runoff = Math.Round(double.Parse(splitLine[2]), 2);
						double peakFlow = Math.Round(double.Parse(splitLine[4]), 2);

						foreach (StormViewModel storm in storms)
						{
							if (storm.Frequency == year && !double.IsNaN(storm.Precipitation))
							{ // found the match, put the runoff and peakflow values into this storm
								storm.PeakFlow = peakFlow;
								storm.Runoff = runoff;
							}
						}
					}
				}

				Debug.WriteLine("Finished parsing");
			}
			catch (Exception ex)
			{
				App.LogException("Parsing output", ex);
			}
		}

		public static List<HydrographLineModel> GetHydrographData(IEnumerable<StormViewModel> storms)
		{
			// First find all storms, specifically their frequencies, that need to be plotted
			List<int> plottedFrequencies = new List<int>();
			foreach (StormViewModel storm in storms) if (storm.DisplayHydrograph) plottedFrequencies.Add((int)storm.Frequency);

			List<HydrographLineModel> list = new List<HydrographLineModel>();

			string filePath = Path.Combine(AppDataDirectory, "EFH2", "tr20.hyd");

			if (!File.Exists(filePath)) return null;
			
			using (StreamReader reader = new StreamReader(filePath))
			{
				while (!reader.EndOfStream)
				{
					List<string> splitLine = SplitLine(reader.ReadLine());

					if (splitLine.Count == 7 && splitLine[0] == "Area" && plottedFrequencies.Contains(int.Parse(splitLine[2].Replace("-Yr", ""))))
					{
						int frequency = int.Parse(splitLine[2].Replace("-Yr", ""));
						double startTime = double.Parse(splitLine[3]);
						double increment = double.Parse(splitLine[4]);

						List<double> values = new List<double>();

						splitLine = SplitLine(reader.ReadLine());
						while (splitLine.Count == 5)
						{
							foreach (string val in splitLine)
							{
								values.Add(double.Parse(val));
							}

							splitLine = SplitLine(reader.ReadLine());
						}

						list.Add(new HydrographLineModel(frequency, startTime, increment, values));
					}
				}
			}
			

			return list;
		}

		public static void ExportHydrograph(MainViewModel model, string outputFileName)
		{
			string hydrographFilePath = Path.Combine(AppDataDirectory, "EFH2", "tr20.hyd");

			StringBuilder content = new StringBuilder();

			if (!File.Exists(hydrographFilePath)) return;

			StreamWriter writer = new StreamWriter(outputFileName);
			StreamReader reader = new StreamReader(hydrographFilePath);

			writer.WriteLine("Hydrograph Frequency, Precipitation (in), Peak Flow (cfs), Runoff (in)");
			foreach (StormViewModel storm in model.RainfallDischargeDataViewModel.Storms)
			{
				writer.WriteLine(storm.Frequency + "-Yr, " + storm.Precipitation.ToString("0.##") + ", " + storm.PeakFlow.ToString("0.##") + ", " + storm.Runoff.ToString("0.##"));
			}

			while (!reader.EndOfStream)
			{
				List<string> splitLine = SplitLine(reader.ReadLine());

				if (splitLine.Count == 7 && splitLine[0] == "OUTLET")
				{
					int frequency = int.Parse(splitLine[2].Replace("-Yr", ""));
					double startTime = double.Parse(splitLine[3]);
					double increment = double.Parse(splitLine[4]);

					int i = 0;

					writer.WriteLine("\n" + frequency + "-Yr Hydrograph");
					writer.WriteLine("Time (hr), Discharge (cfs)");

					while (true) {
						splitLine = SplitLine(reader.ReadLine());

						if (i == 0 && splitLine.Count == 1 && splitLine[0] == "0.0") break;

						foreach (string valueString in splitLine)
						{
							double value = double.Parse(valueString);
							double time = startTime + i * increment;

							writer.WriteLine(time.ToString("0.#####") + ", " + value.ToString("0.##"));

							i++;
						}

						if (splitLine.Last() == "0.0") break;
					}
				}
			}

			writer.Close();
			reader.Close();
		}

		public static void SearchForDataAfterCountyChanged(MainViewModel model, string state, string county)
		{
			string[] typesThatNeedFormatting = new string[] { "I", "II", "IA", "III", "N Pac" };
			double[] automaticStormFrequencies = new double[] { 1, 2, 5, 10, 25, 50, 100, 200, 500, 1000 };

			RainfallDischargeDataViewModel newModel = new RainfallDischargeDataViewModel();
			newModel.RainfallDistributionTypes = model.RainfallDischargeDataViewModel.RainfallDistributionTypes;

			string rainfallDataPath = Path.Combine(ProgramDataDirectory, companyName, "Shared Engineering Data", "EFH2", "Rainfall_data.csv");
			//TextFieldParser parser = new TextFieldParser(rainfallDataPath) { Delimiters = new string[] { "," }, TextFieldType = FieldType.Delimited };
			using (TextFieldParser parser = new TextFieldParser(rainfallDataPath) { Delimiters = new string[] { "," }, TextFieldType = FieldType.Delimited })
			{
				while (!parser.EndOfData)
				{
					string[] elements = parser.ReadFields();

					if (elements.Length == 14 && elements[1].Trim('"') == state && elements[2].Trim('"') == county)
						ReadRainfallDataRowIntoModel(elements, newModel);
				}

				model.RainfallDischargeDataViewModel.SetSilent(newModel);
			}
			//parser.Close();
		}

		public static SerializedDataModel? DeserializeOldFormat(string contents, 
			GetSelectedIndexDelegate getStateIndex, GetSelectedIndexDelegate getCountyIndex, GetSelectedIndexDelegate getRainfallDistributionIndex, GetSelectedIndexDelegate getDuhIndex, GetEmptyRcnDataModel getRcnDataModel)
		{
			try
			{
				string[] lines = contents.Split("\r\n");

				SerializedDataModel model = new SerializedDataModel();

				if (lines.Count() >= 16)
				{
					for (int i = 0; i < 16; i++)
					{
						lines[i] = lines[i].Trim('"');
					}
					string dateFormatString = "MM/dd/yyyy";

					model.By = lines[1];
					model.Date = DateTime.ParseExact(lines[2].Trim('"'), dateFormatString, System.Globalization.CultureInfo.InvariantCulture);
					model.Client = lines[3];

					int? selectedStateIndex = getStateIndex(lines[5]);
					if (selectedStateIndex.HasValue) model.SelectedStateIndex = (uint)selectedStateIndex.Value;

					int? selectedCountyIndex = getStateIndex(lines[4]);
					if (selectedCountyIndex.HasValue) model.SelectedCountyIndex = (uint)selectedCountyIndex.Value;

					double drainageArea = double.NaN;
					double runoffCurveNumber = double.NaN;
					double watershedLength = double.NaN;
					double watershedSlope = double.NaN;
					double timeOfConcentration = double.NaN;

					if (double.TryParse(lines[7], out drainageArea)) model.DrainageArea = drainageArea;
					if (double.TryParse(lines[8], out runoffCurveNumber)) model.RunoffCurveNumber = runoffCurveNumber;
					if (double.TryParse(lines[9], out watershedLength)) model.WatershedLength = watershedLength;
					if (double.TryParse(lines[10], out watershedSlope)) model.WatershedSlope = watershedSlope;
					if (double.TryParse(lines[11], out timeOfConcentration)) model.TimeOfConcentration = timeOfConcentration;

					string[] rainfallDischargeSelections = lines[15].Split(", ");
					int? selectedRainfallDistributionTypeIndex = getRainfallDistributionIndex(rainfallDischargeSelections[0]);
					if (selectedRainfallDistributionTypeIndex.HasValue) model.SelectedRainfallDistributionTypeIndex = (uint)selectedRainfallDistributionTypeIndex.Value;

					if (rainfallDischargeSelections.Length == 2)
					{
						int? selectedDuhTypeIndex = getDuhIndex(rainfallDischargeSelections[1]);
						if (selectedDuhTypeIndex.HasValue) model.SelectedDuhTypeIndex = (uint)selectedDuhTypeIndex.Value;
					} // Else it should go to '<standard>'
				}

				//Search for '', '', '', "Acres" / "Percent", ''
				for (int i = 0; i < lines.Count(); i++)
					{
						string[] lineParts = lines[i].Split(',');

						if (lineParts.Length == 5 && (lineParts[3] == "Acres" || lineParts[3] == "Percent"))
						{
							if (lineParts[3] == "Acres") model.AcresSelected = true;
							else model.AcresSelected = false;
						}
					}

				RcnDataModel rcnModel = getRcnDataModel();

				model.GroupA = rcnModel.GroupA;
				model.GroupB = rcnModel.GroupB;
				model.GroupC = rcnModel.GroupC;
				model.GroupD = rcnModel.GroupD;

				for (int i = 0; i < lines.Count(); i++)
				{
					string[] lineParts = lines[i].Split(',');

					if (lineParts.Length == 4 && lineParts[3].Contains('"') && !lineParts[0].Contains('"'))
					{
						double pageNumber, positionCode;
						if (!double.TryParse(lineParts[1], out positionCode) || !double.TryParse(lineParts[0], out pageNumber)) continue;

						int weight;
						double area;

						if (!int.TryParse(lineParts[2], out weight) || !double.TryParse(lineParts[3].Trim('"'), out area)) continue;

						// TODO make method in serailized data model to accept row, column, weight, area and set in RCN
						model.SetRcnValueFromOldFormat(positionCode, pageNumber, weight, area);
					}
				}

				List<SerializedStormModel> reversedStorms = new List<SerializedStormModel>();
				for (int i = lines.Length - 1; i >= 0; i--)
				{
					string[] lineParts = lines[i].Split(',');

					if (lineParts.Length != 4) continue;
					if (!lineParts[0].Contains('"')) break;

					SerializedStormModel storm = new SerializedStormModel();

					int frequency = 0;
					if (int.TryParse(lineParts[0].Trim('"'), out frequency)) storm.Frequency = frequency;

					int precipitation = 0;
					if (int.TryParse(lineParts[1].Trim('"'), out precipitation)) storm.Precipitation = precipitation;

					int peakFlow = 0;
					if (int.TryParse(lineParts[2].Trim('"'), out peakFlow)) storm.PeakFlow = peakFlow;

					int runoff = 0;
					if (int.TryParse(lineParts[3].Trim('"'), out runoff)) storm.Runoff = runoff;

					reversedStorms.Add(storm);
				}

				model.Storms = new List<SerializedStormModel>();
				for (int i = reversedStorms.Count - 1; i >= 0; i--)
				{
					model.Storms.Add(reversedStorms[i]);
				}

				return model;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				return null;
			}
		}

		private static void ReadRainfallDataRowIntoModel(string[] elements, RainfallDischargeDataViewModel model)
		{
			string[] typesThatNeedFormatting = new string[] { "I", "II", "IA", "III", "N Pac" };
			double[] automaticStormFrequencies = new double[] { 1, 2, 5, 10, 25, 50, 100, 200, 500, 1000 };

			string rfType = elements[3];
			if (typesThatNeedFormatting.Contains(rfType)) rfType = "Type " + rfType;

			model.SetRainfallType(rfType);

			for (int j = 0; j < MainViewModel.NumberOfStorms; j++)
			{
				int i = j + 4;
				if (elements[i] == string.Empty) continue;

				double precipitation = double.Parse(elements[i]); 
				if (precipitation != 0)
				{
					model.Storms[j].Frequency = automaticStormFrequencies[j];
					model.Storms[j].Precipitation = precipitation;
				}
			}
		}

		private static List<string> SplitLine(string line)
		{
			if (line == null) return new List<string>();
			return line.Split().Where(elem => !string.IsNullOrEmpty(elem)).ToList();
		}

		public static bool IsWinTr20ExecutableFound()
		{
			return File.Exists(WinTr20Path);
		}
	}

	public delegate int? GetSelectedIndexDelegate(string type);

	public delegate RcnDataModel GetEmptyRcnDataModel();
}
