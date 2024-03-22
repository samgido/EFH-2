using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
using Windows.Storage.Pickers;
using Windows.Storage;

namespace EFH2
{
    public static class FileOperations
    {
        public static string SerializeData(BasicDataViewModel model)
        {
            return JsonSerializer.Serialize(model);
        }
    }
}
