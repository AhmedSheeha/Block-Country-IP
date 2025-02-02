namespace Block_Country_IP.Repository.IRepo
{
    using System.Linq.Expressions;

    namespace Block_Country_IP.Repository.IRepo
    {
        public interface IRepository<TEntity> where TEntity : class
        {
            Task<bool> AddAsync(TEntity entity);
            Task<bool> DeleteAsync(string id);
            Task<bool> ExistsAsync(string id);
            Task<IEnumerable<TEntity>> GetPageAsync(
               
                int pageSize = 10,
                int pageNumber = 1, Expression<Func<TEntity, bool>> filter = null);
        }
    }

}
