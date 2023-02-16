using TycoonFactory.Core.Managers.Interfaces;
using TycoonFactory.DAL.Entities;
using Timer = System.Timers.Timer;

namespace TycoonFactory.Core.Managers;

public class AndroidManager : IAndroidManager
{
    private Action _onCancel;
    private Action<Activity> _onSuccess;
    
    public Task StartWorkAsync(
        IEnumerable<Android> androids, 
        DateTime endDateTime,
        IProgress<string>? progress = default,
        CancellationToken cancellationToken = default)
    {
        var stopTimer = new Timer {AutoReset = false, Interval = (endDateTime - DateTime.Now).TotalMilliseconds};

        var progressTimer = new Timer();
        progressTimer.Interval = 1;
        progressTimer.Elapsed += (sender, e) =>
        {
            if (cancellationToken.IsCancellationRequested)
            {
                progressTimer.Stop();
                stopTimer.Stop();
                _onCancel?.Invoke();
                return;
            }

            progress?.Report((endDateTime - DateTime.Now).ToString("dd\\:hh\\:mm\\:ss"));
        };

        stopTimer.Elapsed += (_, _) =>
        {
            progressTimer.Stop();
            _onSuccess?.Invoke(androids.First().Activities.First().Activity);
        };
        progressTimer.Start();
        stopTimer.Start();
        return Task.CompletedTask;
    }

    public IAndroidManager OnCancel(Action onCancel)
    {
        _onCancel = onCancel;
        return this;
    }

    public IAndroidManager OnSuccess(Action<Activity> onSuccess)
    {
        _onSuccess = onSuccess;
        return this;
    }
}