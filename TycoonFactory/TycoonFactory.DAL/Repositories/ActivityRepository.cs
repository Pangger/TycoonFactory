using Microsoft.EntityFrameworkCore;
using TycoonFactory.DAL.Entities;
using TycoonFactory.DAL.Repositories.Interfaces;

namespace TycoonFactory.DAL.Repositories;

public class ActivityRepository : IActivityRepository
{
    public async Task<List<Activity>> ListAsync()
    {
        await using var context = new TycoonFactoryContext();
        return await context.Activities
            .Include(a => a.Androids)
                .ThenInclude(aa => aa.Android)
            .ToListAsync().ConfigureAwait(false);
    }

    public async Task<Activity> AddAsync(Activity activity)
    {
        await using var context = new TycoonFactoryContext();
        var dbSet = context.Set<Activity>();
        dbSet.Attach(activity);
        var entity = await context.AddAsync(activity);
        await context.SaveChangesAsync();
        return entity.Entity;
    }

    public async Task UpdateAsync(Activity activity)
    {
        await using var context = new TycoonFactoryContext();
        context.Set<Activity>().Update(activity);
        await context.SaveChangesAsync();
    }

    public async Task RemoveAsync(Activity activity)
    {
        await using var context = new TycoonFactoryContext();
        context.Remove(activity);
        await context.SaveChangesAsync();
    }
}