using Block_Country_IP.Models;
using Block_Country_IP.Repository.IRepo;
using Block_Country_IP.Utility;
using Microsoft.AspNetCore.Mvc;

namespace Block_Country_IP.Controllers
{
    [ApiController]
    [Route("api/countries")]
    public class CountryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CountryController> _logger;
        private readonly IpGeolocationService _ipGeolocationService;


        public CountryController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IpGeolocationService ipGeolocationService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _ipGeolocationService = ipGeolocationService;
        }

        [HttpPost("{countryCode}")]
        public async Task<IActionResult> BlockCountry(string countryCode)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            try
            {
                bool added = await _unitOfWork.Countries.AddAsync(countryCode);

                if (!added)
                {
                    _logger.LogWarning("Failed to block country. Details: " +
                        "IP={IpAddress}, " +
                        "Timestamp={Timestamp}, " +
                        "CountryCode={CountryCode}, " +
                        "BlockedStatus={BlockedStatus}, " +
                        "UserAgent={UserAgent}",
                        ipAddress,
                        DateTime.UtcNow,
                        countryCode,
                        false, // BlockedStatus
                        userAgent);
                }

                return Ok(new { added });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error blocking country. Details: " +
                    "IP={IpAddress}, " +
                    "Timestamp={Timestamp}, " +
                    "CountryCode={CountryCode}, " +
                    "BlockedStatus={BlockedStatus}, " +
                    "UserAgent={UserAgent}",
                    ipAddress,
                    DateTime.UtcNow,
                    countryCode,
                    false, // BlockedStatus
                    userAgent);

                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpDelete("Unblock")]
        public async Task<IActionResult> UnblockCountry(string countryCode)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            try
            {
                bool removed = await _unitOfWork.Countries.DeleteAsync(countryCode);


                if (!removed)
                {
                    _logger.LogWarning("Failed to unblock country. Details: " +
                        "IP={IpAddress}, " +
                        "Timestamp={Timestamp}, " +
                        "CountryCode={CountryCode}, " +
                        "BlockedStatus={BlockedStatus}, " +
                        "UserAgent={UserAgent}",
                        ipAddress,
                        DateTime.UtcNow,
                        countryCode,
                        true, // BlockedStatus (still blocked)
                        userAgent);
                }

                return Ok(new { removed });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unblocking country. Details: " +
                    "IP={IpAddress}, " +
                    "Timestamp={Timestamp}, " +
                    "CountryCode={CountryCode}, " +
                    "BlockedStatus={BlockedStatus}, " +
                    "UserAgent={UserAgent}",
                    ipAddress,
                    DateTime.UtcNow,
                    countryCode,
                    true, // BlockedStatus (still blocked)
                    userAgent);

                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet("page")]
        public async Task<IActionResult> GetCountriesPage(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
        {
            try
            {
                var countries = await _unitOfWork.Countries.GetPageAsync(pageNumber, pageSize);
                return Ok(countries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving countries page. PageNumber={PageNumber}, PageSize={PageSize}", pageNumber, pageSize);
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet("ipinfo")]
        public async Task<IActionResult> GetIpInfo([FromQuery] string? ipAddress = null)
        {
            try
            {
                // If no IP address is provided, use the caller's IP address
                if(ipAddress == null)
                    ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                if (string.IsNullOrWhiteSpace(ipAddress))
                {
                    _logger.LogWarning("No IP address provided and caller IP could not be determined.");
                    return BadRequest("IP address is required.");
                }

                // Get country information for the IP address
                var ipInfo = await _ipGeolocationService.GetIpInfoAsync(ipAddress);

                return Ok(ipInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving IP info. IP={IpAddress}", ipAddress);
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet("checkblock")]
        public async Task<IActionResult> CheckIpBlock([FromQuery] string ipAddress = null)
        {
            try
            {
                // If no IP address is provided, use the caller's IP address
                ipAddress ??= HttpContext.Connection.RemoteIpAddress?.ToString();

                if (string.IsNullOrWhiteSpace(ipAddress))
                {
                    _logger.LogWarning("No IP address provided and caller IP could not be determined.");
                    return BadRequest("IP address is required.");
                }

                // Check if the IP is blocked
                var country = (await _ipGeolocationService.GetIpInfoAsync(ipAddress)).CountryCode;
                bool isBlocked = await _unitOfWork.Countries.ExistsAsync(country);

                return Ok(new { ipAddress, isBlocked });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking IP block status. IP={IpAddress}", ipAddress);
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpPost("temporary/{countryCode}")]
        public async Task<IActionResult> BlockTemporarily(
       string countryCode,
       [FromQuery] int hours = 24)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            try
            {
                bool added = await _unitOfWork.Countries.AddTemporaryAsync(countryCode, TimeSpan.FromHours(hours));


                if (!added)
                {
                    _logger.LogWarning("Failed to temporarily block country. Details: " +
                        "IP={IpAddress}, " +
                        "Timestamp={Timestamp}, " +
                        "CountryCode={CountryCode}, " +
                        "BlockedStatus={BlockedStatus}, " +
                        "UserAgent={UserAgent}",
                        ipAddress,
                        DateTime.UtcNow,
                        countryCode,
                        false, // BlockedStatus
                        userAgent);
                }

                return Ok(new { added });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error temporarily blocking country. Details: " +
                    "IP={IpAddress}, " +
                    "Timestamp={Timestamp}, " +
                    "CountryCode={CountryCode}, " +
                    "BlockedStatus={BlockedStatus}, " +
                    "UserAgent={UserAgent}",
                    ipAddress,
                    DateTime.UtcNow,
                    countryCode,
                    false, // BlockedStatus
                    userAgent);

                return StatusCode(500, new { error = ex.Message });
            }
        }

    }
}
