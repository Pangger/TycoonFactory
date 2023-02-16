using TycoonFactory.DAL.Entities;

namespace TycoonFactory.Core.Managers.Interfaces;

public interface IActivityManager : IStatusManager<IActivityManager>
{
    Task ScheduleActivityAsync(Activity activity, IProgress<string>? progress, CancellationToken cancellationToken);
}