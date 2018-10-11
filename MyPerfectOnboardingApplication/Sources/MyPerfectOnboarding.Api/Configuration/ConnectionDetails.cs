using System.Configuration;
using MyPerfectOnboarding.Contracts.Services.Database;

namespace MyPerfectOnboarding.Api.Configuration
{
    internal class ConnectionDetails : IConnectionDetails
    {
        private static readonly string GetDataConnectionString = ConfigurationManager.ConnectionStrings["ListDBConnection"]?.ConnectionString ?? string.Empty;

        public string DataConnectionString { get; } = GetDataConnectionString;
    }
}