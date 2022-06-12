using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TraderesourcesApi.Client;
using Yoda.Application.Helpers;

namespace TraderesourcesApi.Client {
    public static class TraderesourcesConfigurationExtension {
        public static IServiceCollection AddTraderesources(this IServiceCollection services, IConfiguration configuration) {
            services.Configure<ApiConfig>(options => {
                options.AuthorityUrl = configuration["TraderesourcesApi:authorityUrl"];
                options.ApiUrl = configuration["TraderesourcesApi:url"];
                options.ClientId = configuration["TraderesourcesApi:client:clientId"];
                options.Password = configuration["TraderesourcesApi:client:pwd"];
            });
            services.AddHttpClient();
            services.AddSingleton<ITraderesourcesApiClientFactory, TraderesourcesApiClientFactory>();
            return services;
        }
    }

    public interface ITraderesourcesApiClientFactory {
        string GetTraderesourcesUrl(ObjectType objectType);
        Task<TraderesourcesApiClient> CreateClient();
    }

    public class TraderesourcesApiClientFactory : ITraderesourcesApiClientFactory {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiUrl;
        private readonly AccessTokenFactory _accessTokenFactory;

        public TraderesourcesApiClientFactory(IHttpClientFactory httpClientFactory, IOptions<ApiConfig> apiConfig) {
            _apiUrl = apiConfig.Value.ApiUrl;
            if (string.IsNullOrEmpty(_apiUrl)) {
                throw new ArgumentException("Не задана ссылка на api сервис -> TraderesourcesApi:url");
            }

            _httpClientFactory = httpClientFactory;
            _accessTokenFactory = new AccessTokenFactory(
                httpClientFactory,
                apiConfig.Value.AuthorityUrl,
                apiConfig.Value.ClientId,
                apiConfig.Value.Password,
                "TraderesourcesApi",
                60
            );

        }

        private async Task<HttpClient> createHttpClientWithToken() {
            var token = await _accessTokenFactory.GetAccessToken();

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_apiUrl);
            httpClient.SetBearerToken(token.AccessToken);
            return httpClient;
        }

        public async Task<TraderesourcesApiClient> CreateClient() {
            var httpClient = await createHttpClientWithToken();

            return new TraderesourcesApiClient(_apiUrl, httpClient);
        }


        public string GetTraderesourcesUrl(ObjectType objectType) {
            return $"{_apiUrl}/Traderesources/get/{objectType.ToString()}";
        }
    }

    public class ApiConfig {
        public string AuthorityUrl { get; set; }
        public string ApiUrl { get; set; }
        public string ClientId { get; set; }
        public string Password { get; set; }
    }
}
