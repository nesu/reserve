using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Reserve.API.Configurations
{
    public static class SerializerConfigurations
    {
        public static JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };
    }
}