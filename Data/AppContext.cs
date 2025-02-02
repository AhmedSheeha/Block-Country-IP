using System.Collections.Concurrent;
using System.Linq.Expressions;
using Block_Country_IP.Models;

namespace Block_Country_IP.Data
{
    public class AppData
    {
        private readonly ConcurrentDictionary<string, CountryInfo> _blockedCountries = new ConcurrentDictionary<string, CountryInfo>();

        public Task<bool> AddAsync(CountryInfo countryInfo)
        {
            if (countryInfo == null || string.IsNullOrWhiteSpace(countryInfo.ContryCode))
                throw new ArgumentException("Invalid country information.");

            bool added = _blockedCountries.TryAdd(countryInfo.ContryCode, countryInfo);
            return Task.FromResult(added);
        }

        public Task<bool> DeleteAsync(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                throw new ArgumentException("Country code cannot be null or empty.");

            bool removed = _blockedCountries.TryRemove(countryCode, out _);
            return Task.FromResult(removed);
        }

        public Task<bool> ExistsAsync(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                throw new ArgumentException("Country code cannot be null or empty.");

            bool exists = _blockedCountries.ContainsKey(countryCode);
            return Task.FromResult(exists);
        }

        public Task<IEnumerable<CountryInfo>> GetPageAsync(
            Expression<Func<CountryInfo, bool>> filter = null,
            int pageSize = 10,
            int pageNumber = 1)
        {
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
            if (pageNumber <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than zero.");

            IEnumerable<CountryInfo> query = _blockedCountries.Values;

            if (filter != null)
            {
                var compiledFilter = filter.Compile();
                query = query.Where(compiledFilter);
            }

            var result = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Task.FromResult((IEnumerable<CountryInfo>)result);
        }
        public Task<int> CleanupExpiredBlocksAsync()
        {
            var now = DateTime.UtcNow;
            int removedCount = 0;

            foreach (var key in _blockedCountries.Keys)
            {
                if (_blockedCountries.TryGetValue(key, out var countryInfo) &&
                    countryInfo.Temp &&
                    countryInfo.ReleaseTime.HasValue &&
                    countryInfo.ReleaseTime.Value >= now)
                {
                    if (_blockedCountries.TryRemove(key, out _))
                    {
                        removedCount++;
                    }
                }
            }

            return Task.FromResult(removedCount);
        }
    }
}
