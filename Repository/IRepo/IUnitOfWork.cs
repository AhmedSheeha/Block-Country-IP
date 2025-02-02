using Block_Country_IP.Models;
using Block_Country_IP.Repository.IRepo.Block_Country_IP.Repository.IRepo;

namespace Block_Country_IP.Repository.IRepo
{
    public interface IUnitOfWork
    {
        ICountryRepository Countries { get; }
        
    }

}
