using System;
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
            string programDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            string filePath = Path.Combine(programDataPath, "EFH2/tr20.inp");

            if (!File.Exists(filePath))
            {
                StringBuilder content = new StringBuilder();
                content.AppendLine("WinTR-20: Version 3.30                  0         0         0.01      0\r");
                content.AppendLine("Single watershed using lag method for Tc");
                content.AppendLine("");
                content.AppendLine("SUB-AREA:");
                // no clue where the .78125 came from
                //content.AppendLine($"          Area      Outlet              .78125    {model.BasicDataViewModel.RunoffCurveNumber}       {model.BasicDataViewModel.TimeOfConcentration}\r");
                content.AppendLine(String.Format("          {0,10}{1,20}{2,10}{3,10}", "Area", "Outlet", .78125, model.BasicDataViewModel.RunoffCurveNumber, model.BasicDataViewModel.TimeOfConcentration));
                content.AppendLine("");
                content.AppendLine("");
                content.AppendLine("");
                content.AppendLine("STORM ANALYSIS:");
                foreach (StormViewModel storm in model.RainfallDischargeDataViewModel.Storms)
                {
                    content.AppendLine(String.Format("          {0,30}{1,10}{2,10}{3}", storm.Years + "-Yr", storm.DayRain, model.RainfallDischargeDataViewModel.selectedRainfallDistributionType, 2));
                }
                content.AppendLine("");
                content.AppendLine("RAINFALL DISTRIBUTION:");

                string rainfallDistributionFilePath = Path.Combine(programDataPath, "USDA/Shared Engineering Data/EFH2/RainfallDistributions/Type " + model.RainfallDischargeDataViewModel.selectedRainfallDistributionType + ".tbl");

                if (File.Exists(rainfallDistributionFilePath))
                {
                    content.Append(File.ReadAllText(rainfallDistributionFilePath));
                }
            }
        }
    }
}
