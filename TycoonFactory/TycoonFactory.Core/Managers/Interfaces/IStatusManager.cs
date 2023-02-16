using TycoonFactory.DAL.Entities;

namespace TycoonFactory.Core.Managers.Interfaces;

public interface IStatusManager<out TManagerType>
{
    TManagerType OnCancel(Action onCancel);
    TManagerType OnSuccess(Action<Activity> onSuccess);
}