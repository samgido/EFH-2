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

        public static string? CreateInpFile(MainViewModel model)
        {
			if (!IsWinTR20Ready(model)) return null;

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filePath = Path.Combine(appDataPath, "EFH2\\tr20.inp");

   //         if (File.Exists(filePath)) File.Delete(filePath);
			//File.Create(filePath);

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
					if (!storm.Frequency.Equals(double.NaN) && !storm.DayRain.Equals(double.NaN))
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
						//writer.Write(File.ReadAllText(duhTypeFilePath));

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

				//File.WriteAllText(filePath, content.ToString());
				//writer.Write(content.ToString());

				RunWinTr20(filePath);

				return filePath;
			}
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
					while (!process.StandardOutput.EndOfStream) Debug.WriteLine(process.StandardOutput.ReadLine());

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
			// Rainfall discharge data check
			foreach (StormViewModel storm in model.RainfallDischargeDataViewModel.Storms)
			{
				// If any entry is invalid data, ie one field in storm is filled but the other isn't, data isn't ready
				if (storm.Frequency.Equals(double.NaN) ^ storm.DayRain.Equals(double.NaN)) return false;
			}

			if (model.RainfallDischargeDataViewModel.SelectedRainfallDistributionTypeIndex == 0) return false;

			// Basic data check 
			if (model.BasicDataViewModel.drainageAreaEntry.Value.Equals(double.NaN)) return false;
			if (model.BasicDataViewModel.runoffCurveNumberEntry.Value.Equals(double.NaN)) return false;
			if (model.BasicDataViewModel.timeOfConcentrationEntry.Value.Equals(double.NaN)) return false;

			// If everything is ready
			return true;
		}
    }
}
