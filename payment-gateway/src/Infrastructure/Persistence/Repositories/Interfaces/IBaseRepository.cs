namespace Infrastructure.Persistence.Repositories.Interfaces;

public interface IBaseRepository<T>
{
    Task<int> AddAsync(T en);

    Task<int> UpdateAsync(T en);

    Task<int> DeleteAsync(T en);

    Task<IEnumerable<T>> GetListAsync(string condition, object param);
}