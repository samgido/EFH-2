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
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals,
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true,
            };

            return JsonSerializer.Serialize(model, options);
        }
    }
}
