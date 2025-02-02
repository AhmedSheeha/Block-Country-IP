using Block_Country_IP.Repository.IRepo;
using Block_Country_IP.Utility;

public class IpBlockService
{
    private readonly IpGeolocationService _ipGeolocationService;
    private readonly ICountryRepository _countryRepository;

    public IpBlockService(
        IpGeolocationService ipGeolocationService,
        ICountryRepository countryRepository)
    {
        _ipGeolocationService = ipGeolocationService;
        _countryRepository = countryRepository;
    }

    public async Task<bool> IsIpBlockedAsync(string ipAddress)
    {
        var ipInfo = await _ipGeolocationService.GetIpInfoAsync(ipAddress);
        return await _countryRepository.ExistsAsync(ipInfo.CountryCode);
    }
}