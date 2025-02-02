using Block_Country_IP.Models;
using Block_Country_IP.Repository.IRepo;
using System.Collections.Concurrent;

namespace Block_Country_IP.Repository
{
    public class CountryRepository : Repository<CountryInfo>, ICountryRepository
    {
        public CountryRepository()
        {
          
        }
        public async Task<int> CleanupExpiredBlocksAsync()
        {
            var now = DateTime.UtcNow;
            int removedCount = 0;

            foreach (var key in _data.Keys.ToList())
            {
                if (_data.TryGetValue(key, out var countryInfo) &&
                    countryInfo.Temp &&
                    countryInfo.ReleaseTime.HasValue &&
                    countryInfo.ReleaseTime.Value >= now)
                {
                    if (_data.TryRemove(key, out _)) removedCount++;
                }
            }

            return removedCount;
        }

        public async Task<bool> AddAsync(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                throw new ArgumentException("Country code cannot be empty.");

            // Create a new CountryInfo object with default values
            var countryInfo = new CountryInfo
            {
                ContryCode = countryCode,
                Temp = false, // Default to permanent block
                ReleaseTime = null
            };

            return await base.AddAsync(countryInfo);
        }
        public Task<bool> AddTemporaryAsync(string countryCode, TimeSpan duration)
        {
            // Temporary block logic
            var countryInfo = new CountryInfo
            {
                ContryCode = countryCode,
                Temp = true,
                ReleaseTime = DateTime.UtcNow.Add(duration)
            };
            return Task.FromResult(_data.TryAdd(countryCode, countryInfo));
        }
    }

    }
