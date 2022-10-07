using Microsoft.Graph;
using Azure.Identity;

namespace WebAPI.Common.Extensions.Middlewares
{
    public static class GraphApiMiddlewareExtension
    {
        public static void AddGraphApi(this IServiceCollection services, GraphSettings graphSettings)
        {
            var clientCredential = new ClientSecretCredential(graphSettings.AzureAdTenant, graphSettings.ClientId, graphSettings.ClientSecret);
            var scopes = new[] {$"{graphSettings.ApiUrl}/.default"};

            var httpClient = GraphClientFactory.Create(new TokenCredentialAuthProvider(clientCredential, scopes));

            var graphServiceClient = new GraphServiceClient(httpClient);

            services.AddSingleton(graphServiceClient);
        }
    }
}