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

        public static void CreateInpFile(MainViewModel model)
        {
			if (!IsWinTR20Ready(model)) return;

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filePath = Path.Combine(appDataPath, "EFH2\\tr20.inp");

   //         if (File.Exists(filePath)) File.Delete(filePath);
			//File.Create(filePath);

			using (StreamWriter writer = new StreamWriter(filePath, append: false))
			{
				StringBuilder content = new StringBuilder();
				//content.AppendLine("WinTR-20: Version 3.30                  0         0         0.01      0");
				content.AppendLine(String.Format("WinTR-20: {0,-30}{1,-10}{2,-10}{3,-10}{4}", "Version 3.30", 0, 0, 0.01, 0));
				content.AppendLine("Single watershed using lag method for Tc");
				content.AppendLine("");
				content.AppendLine("SUB-AREA:");
				content.AppendLine(String.Format("          {0,-10}{1,-20}{2,-10}{3,-10}{4,-10}", "Area", "Outlet",
					(model.BasicDataViewModel.DrainageArea / 640).ToString("0.00000"),
					(model.BasicDataViewModel.RunoffCurveNumber + "."),
					model.BasicDataViewModel.TimeOfConcentration.ToString("0.00")));

				content.AppendLine("");
				content.AppendLine("");
				content.AppendLine("");
				content.AppendLine("STORM ANALYSIS:");
				foreach (StormViewModel storm in model.RainfallDischargeDataViewModel.Storms)
				{
					if (!storm.Frequency.Equals(double.NaN) && !storm.DayRain.Equals(double.NaN))
					{
						content.AppendLine(String.Format("          {0,-30}{1,-10}{2,-10}{3}",
							storm.Frequency + "-Yr",
							storm.DayRain.ToString("0.0"),
							model.RainfallDischargeDataViewModel.selectedRainfallDistributionType,
							2));
					}
				}
				content.AppendLine("");
				content.AppendLine("RAINFALL DISTRIBUTION:");

				string rainfallDistributionTypeFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), 
					"USDA\\Shared Engineering Data\\EFH2\\RainfallDistributions\\Type " + model.RainfallDischargeDataViewModel.selectedRainfallDistributionType + ".tbl");

				if (File.Exists(rainfallDistributionTypeFilePath))
				{
					content.Append(File.ReadAllText(rainfallDistributionTypeFilePath));
				}

				//File.WriteAllText(filePath, content.ToString());
				writer.Write(content.ToString());

				RunWinTr20(filePath);
			}
		}

		private static void RunWinTr20(string filePath)
		{
			try
			{
				string executableDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\USDA\\EFH2\\";
				string executableName = executableDirectory + "WinTR20_V32.exe";
				ProcessStartInfo psi = new ProcessStartInfo()
				{
					FileName = executableName,
					Arguments = filePath,
					CreateNoWindow = true,
					UseShellExecute = false,
				};

				using (Process process = new Process() { StartInfo = psi })
				{
					process.Start();
					process.WaitForExit();
				}
			}
			catch (Exception ex)
			{
				
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
				if (storm.Frequency.Equals(double.NaN) !^ storm.DayRain.Equals(double.NaN)) return false;
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
