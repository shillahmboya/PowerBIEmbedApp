using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.Rest;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Newtonsoft.Json;

namespace AppOwnsData.Services {

    // A view model class to pass the data needed to embed a single report
    public class EmbeddedReportViewModel {
        public string Id;
        public string Name;
        public string EmbedUrl;
        public string Token;
    }

    public class PowerBiServiceApi {

        private ITokenAcquisition tokenAcquisition { get; }
        private string urlPowerBiServiceApiRoot { get; }

        public PowerBiServiceApi(IConfiguration configuration, ITokenAcquisition tokenAcquisition) {
            this.urlPowerBiServiceApiRoot = configuration["PowerBi:ServiceRootUrl"];
            this.tokenAcquisition = tokenAcquisition;
        }

        public const string powerbiApiDefaultScope = "https://analysis.windows.net/powerbi/api/.default";

        // A method to get the Azure AD token (also known as 'access token')
        public string GetAccessToken() {
            return this.tokenAcquisition.GetAccessTokenForAppAsync(powerbiApiDefaultScope).Result;
        }

        public PowerBIClient GetPowerBiClient() {
            var tokenCredentials = new TokenCredentials(GetAccessToken(), "Bearer");
            return new PowerBIClient(new Uri(urlPowerBiServiceApiRoot), tokenCredentials);
        }

        public async Task<EmbeddedReportViewModel> GetReport(Guid WorkspaceId, Guid ReportId) {

            PowerBIClient pbiClient = GetPowerBiClient();

            // Call the Power BI service API to get the embedding data.
            var report = await pbiClient.Reports.GetReportInGroupAsync(WorkspaceId, ReportId);

            // Generate a read-only embed token for the report.
            var datasetId = report.DatasetId;
            var tokenRequest = new GenerateTokenRequest(TokenAccessLevel.View, datasetId);
            var embedTokenResponse = await pbiClient.Reports.GenerateTokenAsync(WorkspaceId, ReportId, tokenRequest);
            var embedToken = embedTokenResponse.Token;

            // Return the report embedded data to caller.
            return new EmbeddedReportViewModel {
                Id = report.Id.ToString(),
                EmbedUrl = report.EmbedUrl,
                Name = report.Name,
                Token = embedToken
            };
        }

    }
}