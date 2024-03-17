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
using Newtonsoft.Json;
using Windows.Storage;

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
            this._wrapper = new DataWrapper()
            {
                BasicDataModel = basicDataModel,
                RainfallDataModel = rainfallDataModel, 
                RCNDataModel = rCNDataModel
            };
        }

        public async void WriteFile(StorageFile file)
        {
            string json = JsonConvert.SerializeObject(this._wrapper.BasicDataModel);

            await Windows.Storage.FileIO.WriteTextAsync(file, json);
        }
    }
}
