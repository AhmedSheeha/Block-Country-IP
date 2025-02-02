using System.Text.Json.Serialization;

namespace Block_Country_IP.Utility
{
    public class IpGeolocationResponse
    {
        [JsonPropertyName("country_code2")]
        public string CountryCode { get; set; }

        [JsonPropertyName("country_name")]
        public string CountryName { get; set; }

        [JsonPropertyName("state_prov")]
        public string Region { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; set; }

        [JsonPropertyName("ip")]
        public string Ip { get; set; }

        // Add other fields as needed
    }
}