using Block_Country_IP.Data;
using Block_Country_IP.Models;
using Block_Country_IP.Repository.IRepo.Block_Country_IP.Repository.IRepo;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Block_Country_IP.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly ConcurrentDictionary<string, TEntity> _data = new();

        public virtual Task<bool> AddAsync(TEntity entity)
        {
            // Assume entity has an "Id" property for simplicity
            var id = (entity as dynamic)?.ContryCode; // Replace with your key logic
            if (string.IsNullOrEmpty(id)) throw new ArgumentException("Invalid entity ID.");
            return Task.FromResult(_data.TryAdd(id, entity));
        }

        public virtual Task<bool> DeleteAsync(string id)
            => Task.FromResult(_data.TryRemove(id, out _));

        public virtual Task<bool> ExistsAsync(string id)
            => Task.FromResult(_data.ContainsKey(id));

        public virtual Task<IEnumerable<TEntity>> GetPageAsync(
    int pageSize = 10,
    int pageNumber = 1,
    Expression<Func<TEntity, bool>>? filter = null)
{
    if (pageSize <= 0)
        throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
    if (pageNumber <= 0)
        throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than zero.");

    // Get the values from the ConcurrentDictionary
    var query = _data.Values.AsQueryable();

    // Apply the filter if provided
    if (filter != null)
    {
        query = query.Where(filter);
    }

    // Order the results for consistent pagination (e.g., by key or another property)
    query = query.OrderBy(e => e.GetHashCode()); // Example: Order by hash code (replace with a real property)

    // Paginate the results
    var result = query
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToList();

    return Task.FromResult((IEnumerable<TEntity>)result);
}
    }
}
