using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TycoonFactory.DAL.Entities;
using TycoonFactory.DAL.Repositories.Interfaces;

namespace TycoonFactory.DAL.Repositories;

public class AndroidRepository : IAndroidRepository
{
    public async Task<List<Android>> ListAsync()
    {
        await using var context = new TycoonFactoryContext();
        return await context.Androids
            .Include(a => a.Activities)
            .ThenInclude(aa => aa.Activity)
            .ToListAsync();
    }

    public async Task<List<Android>> ListAsync(Expression<Func<Android, bool>> predicate)
    {
        await using var context = new TycoonFactoryContext();
        return await context.Androids
            .Include(a => a.Activities)
            .ThenInclude(aa => aa.Activity)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<List<Android>> ListTopAndroids()
    {
        await using var context = new TycoonFactoryContext();
        return await context.Androids
            .Include(a => a.Activities.Where(w =>
                w.Activity.StartDateTime > DateTime.Today && w.Activity.StartDateTime <= DateTime.Today.AddDays(7)))
            .ThenInclude(aa => aa.Activity)
            .Where(a => a.Activities.Any())
            .OrderByDescending(a => a.Activities.Count())
            .Take(10)
            .ToListAsync();
    }

    public async Task<Android> AddAsync(Android android)
    {
        await using var context = new TycoonFactoryContext();
        var dbSet = context.Set<Android>();
        dbSet.Attach(android);
        var entity = await context.AddAsync(android);
        await context.SaveChangesAsync();
        return entity.Entity;
    }

    public async Task RemoveAsync(Android android)
    {
        await using var context = new TycoonFactoryContext();
        context.Androids.Remove(android);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Android android)
    {
        await using var context = new TycoonFactoryContext();
        context.Set<Android>().Update(android);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(List<Android> androids)
    {
        await using var context = new TycoonFactoryContext();
        foreach (var android in androids)
        {
            context.Set<Android>().Update(android);
        }

        await context.AddRangeAsync();
    }
}