using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace MyPerfectOnboarding.Api
{
    public class JsonSerializerConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;
        }
    }
}