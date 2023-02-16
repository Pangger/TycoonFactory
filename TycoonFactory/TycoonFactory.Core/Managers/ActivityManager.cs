using TycoonFactory.Core.Managers.Interfaces;
using TycoonFactory.DAL.Entities;

namespace TycoonFactory.Core.Managers;

public class ActivityManager : IActivityManager
{
    private readonly IAndroidManager _androidManager;

    private Action _onCancel;
    private Action<Activity> _onSuccess;

    public ActivityManager(
        IAndroidManager androidManager)
    {
        _androidManager = androidManager;
    }

    public Task ScheduleActivityAsync(Activity activity,
        IProgress<string>? progress = default,
        CancellationToken cancellationToken = default)
        => Task.Factory.StartNew(async () =>
        {
            try
            {
                if (activity.StartDateTime > DateTime.Now)
                {
                    progress?.Report($"Starts at {activity.StartDateTime}");
                    await Task.Delay(activity.StartDateTime - DateTime.Now, cancellationToken);
                }

                await Task.Factory.StartNew(async () =>
                {
                    await _androidManager
                        .OnCancel(_onCancel)
                        .OnSuccess(_onSuccess)
                        .StartWorkAsync(activity.Androids.Select(s => s.Android),
                            activity.EndDateTime,
                            progress,
                            cancellationToken);
                }, TaskCreationOptions.AttachedToParent);
            }
            catch (TaskCanceledException)
            {
                _onCancel?.Invoke();
            }
        });


    public IActivityManager OnCancel(Action onCancel)
    {
        _onCancel = onCancel;
        return this;
    }

    public IActivityManager OnSuccess(Action<Activity> onSuccess)
    {
        _onSuccess = onSuccess;
        return this;
    }
}