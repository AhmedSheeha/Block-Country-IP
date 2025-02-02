using System.Text.Json;

namespace Block_Country_IP.Utility
{
    public class IpGeolocationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public IpGeolocationService(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["IPGeolocation:ApiKey"];
        }

        public async Task<IpGeolocationResponse> GetIpInfoAsync(string ipAddress)
        {
            var response = await _httpClient.GetAsync($"?apiKey={_apiKey}&ip={ipAddress}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IpGeolocationResponse>(json);
        }
    }
}
