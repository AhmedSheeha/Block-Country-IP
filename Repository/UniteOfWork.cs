using Block_Country_IP.Data;
using Block_Country_IP.Models;
using Block_Country_IP.Repository.IRepo;
using Block_Country_IP.Repository.IRepo.Block_Country_IP.Repository.IRepo;

namespace Block_Country_IP.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CountryRepository _countryRepository;

        public UnitOfWork()
        {
            _countryRepository = new CountryRepository();
        }

        public ICountryRepository Countries => _countryRepository;
    }
}
