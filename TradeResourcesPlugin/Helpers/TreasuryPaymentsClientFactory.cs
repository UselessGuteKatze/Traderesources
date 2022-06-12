using IdentityModel.Client;
using Microsoft.Extensions.Options;
using PaymentsApi.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Yoda.Application.Helpers;

namespace TradeResourcesPlugin.Helpers {
    public interface ITreasuryPaymentsClientFactory {
        public Task<PaymentsApiClient> CreateClientAsync();
    }
    public class TreasuryPaymentsClientFactory : ITreasuryPaymentsClientFactory {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AccessTokenFactory _accessTokenFactory;
        private readonly IOptions<TreasuryPaymentsApiClientConfig> _config;
        public TreasuryPaymentsClientFactory(IHttpClientFactory httpClientFactory, IOptions<TreasuryPaymentsApiClientConfig> config) {
            _httpClientFactory = httpClientFactory;
            _accessTokenFactory = new AccessTokenFactory(
                httpClientFactory,
                config.Value.AuthorityUrl,
                config.Value.ClientId,
                config.Value.Secret,
                scope: "TreasuryPaymentsApi",
                60
            );
            _config = config;
        }
        public async Task<PaymentsApiClient> CreateClientAsync() {
            var token = await _accessTokenFactory.GetAccessToken();
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.SetBearerToken(token.AccessToken);
            return new PaymentsApiClient(httpClient) {
                BaseUrl = _config.Value.ApiUrl
            };
        }
    }

    public class TreasuryPaymentsApiClientConfig {
        public string AuthorityUrl { get; set; }
        public string ApiUrl { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
    }

}
