using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace MyPerfectOnboarding.Api
{
    public static class JsonSerializerConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var jsonFormatter = config.Formatters.JsonFormatter;

            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonFormatter.UseDataContractJsonSerializer = false;
        }
    }
}