using Block_Country_IP.Models;
using Block_Country_IP.Repository.IRepo.Block_Country_IP.Repository.IRepo;

namespace Block_Country_IP.Repository.IRepo
{
    public interface ICountryRepository : IRepository<CountryInfo>
    {
        Task<int> CleanupExpiredBlocksAsync();
        public Task<bool> AddAsync(string countryCode);
        public Task<bool> AddTemporaryAsync(string countryCode, TimeSpan duration);
    }
}