using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace EFH_2.Misc
{

    public class FileOperations
    {
        public class DataWrapper
        {
            public BasicDataModel BasicDataModel;
            public RainfallDataModel RainfallDataModel;
            public RCNDataModel RCNDataModel;
        }

        private DataWrapper _wrapper;

        public FileOperations(BasicDataModel basicDataModel, RainfallDataModel rainfallDataModel, RCNDataModel rCNDataModel)
        {
            DataWrapper _wrapper = new DataWrapper()
            {
                BasicDataModel = basicDataModel, 
                RainfallDataModel = rainfallDataModel, 
                RCNDataModel = rCNDataModel
            };
        }

        public void WriteFile(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataWrapper));
            TextWriter writer = new StreamWriter(fileName);

            serializer.Serialize(writer, _wrapper);
        }
    }
}
