using System;
using System.Threading.Tasks;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using Microsoft.Identity.Client;

namespace AppOwnsData.Services
{
    // View model for embedded report
    public class EmbeddedReportViewModel
    {
        public string Id;
        public string Name;
        public string EmbedUrl;
        public string Token;
    }

    public class PowerBiServiceApi
    {
        private readonly string clientId = Environment.GetEnvironmentVariable("POWERBI_CLIENT_ID");
        private readonly string clientSecret = Environment.GetEnvironmentVariable("POWERBI_CLIENT_SECRET");
        private readonly string tenantId = Environment.GetEnvironmentVariable("POWERBI_TENANT_ID");
        private readonly string powerBiUrl = "https://api.powerbi.com/";

        private const string PowerBiScope = "https://analysis.windows.net/powerbi/api/.default";

        // Get app-only access token
        private async Task<string> GetAccessTokenAsync()
        {
            var app = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
                .Build();

            var result = await app.AcquireTokenForClient(new[] { PowerBiScope }).ExecuteAsync();
            return result.AccessToken;
        }

        // Get Power BI client
        private async Task<PowerBIClient> GetPowerBiClientAsync()
        {
            var token = await GetAccessTokenAsync();
            var tokenCredentials = new TokenCredentials(token, "Bearer");
            return new PowerBIClient(new Uri(powerBiUrl), tokenCredentials);
        }

        // Get embedded report info
        public async Task<EmbeddedReportViewModel> GetReport(Guid workspaceId, Guid reportId)
        {
            var pbiClient = await GetPowerBiClientAsync();

            // Get report
            var report = await pbiClient.Reports.GetReportInGroupAsync(workspaceId, reportId);

            // Generate embed token
            var tokenRequest = new GenerateTokenRequest(TokenAccessLevel.View, report.DatasetId);
            var embedTokenResponse = await pbiClient.Reports.GenerateTokenAsync(workspaceId, reportId, tokenRequest);

            return new EmbeddedReportViewModel
            {
                Id = report.Id.ToString(),
                EmbedUrl = report.EmbedUrl,
                Name = report.Name,
                Token = embedTokenResponse.Token
            };
        }
    }
}
