using TycoonFactory.DAL.Entities;

namespace TycoonFactory.Core.Managers.Interfaces;

public interface IAndroidManager : IStatusManager<IAndroidManager>
{
    Task StartWorkAsync(IEnumerable<Android> androids, DateTime endDateTime, IProgress<string>? progress = default, CancellationToken cancellationToken = default);
}