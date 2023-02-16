using TycoonFactory.DAL.Entities;

namespace TycoonFactory.DAL.Repositories.Interfaces;

public interface IActivityRepository
{
    Task<List<Activity>> ListAsync();
    Task<Activity> AddAsync(Activity activity);
    Task UpdateAsync(Activity activity);
    Task RemoveAsync(Activity activity);
}