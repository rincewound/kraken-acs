using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kraken.model;
using Newtonsoft.Json;

namespace kraken.serializer
{
    public class Serializer
    {
        public static void Serialize(OrganisationalModel model, string fileName)
        {
            System.IO.File.WriteAllText(fileName, getSerialized(model));
        }

        public static string getSerialized(OrganisationalModel model)
        {
            string json = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            return json;
        }

        public static OrganisationalModel Derserialize(string fileName)
        {
            var text = System.IO.File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<OrganisationalModel>(text, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects});
        }
    }
}
