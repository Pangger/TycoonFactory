using System.Linq.Expressions;
using TycoonFactory.DAL.Entities;

namespace TycoonFactory.DAL.Repositories.Interfaces;

public interface IAndroidRepository
{
    Task<List<Android>> ListAsync();
    Task<List<Android>> ListAsync(Expression<Func<Android, bool>> predicate);
    Task<List<Android>> ListTopAndroids();
    Task<Android> AddAsync(Android android);
    Task RemoveAsync(Android android);
    Task UpdateAsync(Android android);
    Task UpdateAsync(List<Android> androids);
}