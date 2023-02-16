using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using TycoonFactory.Core.Managers.Interfaces;
using TycoonFactory.DAL.Entities;
using TycoonFactory.DAL.Entities.Enums;
using TycoonFactory.DAL.Repositories.Interfaces;
using TycoonFactory.ViewModels.Messages;
using TycoonFactory.ViewModels.UiModels;

namespace TycoonFactory.ViewModels;

public class MainViewModel : MvxNavigationViewModel
{
    #region Private fields

    private readonly IAndroidRepository _androidRepository;
    private readonly IActivityRepository _activityRepository;
    private readonly IActivityManager _activityManager;

    private readonly Dictionary<int, CancellationTokenSource> _runningActivities = new();

    #endregion

    #region Commands

    public MvxAsyncCommand AddAndroidCommand { get; }
    public MvxAsyncCommand<Android> RemoveAndroidCommand { get; }
    public MvxAsyncCommand AddActivityCommand { get; }
    public MvxAsyncCommand<ActivityUiModel> EditActivityCommand { get; set; }
    public MvxAsyncCommand<ActivityUiModel> CancelActivityCommand { get; }

    #endregion

    #region Properties

    public ObservableCollection<Android> Androids { get; } = new();
    public ObservableCollection<ActivityUiModel> Activities { get; } = new();
    public ObservableCollection<Android> TopAndroids { get; } = new();

    #endregion

    #region Constructor

    public MainViewModel(
        ILoggerFactory logFactory,
        IMvxNavigationService navigationService,
        IAndroidRepository androidRepository,
        IActivityRepository activityRepository,
        IActivityManager activityManager,
        IMvxMessenger mvxMessenger)
        : base(logFactory, navigationService)
    {
        _androidRepository = androidRepository;
        _activityRepository = activityRepository;
        _activityManager = activityManager;

        AddAndroidCommand = new MvxAsyncCommand(AddAndroidExecute);
        RemoveAndroidCommand = new MvxAsyncCommand<Android>(RemoveAndroidExecute);
        AddActivityCommand = new MvxAsyncCommand(AddActivityExecute);
        EditActivityCommand = new MvxAsyncCommand<ActivityUiModel>(EditActivityExecute);
        CancelActivityCommand = new MvxAsyncCommand<ActivityUiModel>(CancelActivityExecute);

        Androids.CollectionChanged += OnCollectionChanged;
        Activities.CollectionChanged += OnCollectionChanged;

        mvxMessenger.Subscribe<AddActivityMessage>(OnActivityAdded, MvxReference.Strong);
        mvxMessenger.Subscribe<EditActivityMessage>(OnActivityEdited, MvxReference.Strong);
    }

    #endregion

    #region Command methods

    private async Task AddAndroidExecute()
    {
        var entity = new Android()
        {
            Name = $"Beep bop #{new Random().Next(1000, 10000)}"
        };
        await _androidRepository.AddAsync(entity);

        Androids.Add(entity);
    }

    private async Task RemoveAndroidExecute(Android android)
    {
        if (Activities.Any(a => a.Model.Androids.Any(aa => aa.ActivityId == android.Id)))
            return;
        await _androidRepository.RemoveAsync(android);
        Androids.Remove(android);
    }

    private Task AddActivityExecute()
    {
        return NavigationService.Navigate<AddActivityViewModel>();
    }

    private Task EditActivityExecute(ActivityUiModel activity)
    {
        return NavigationService.Navigate<AddActivityViewModel, ActivityUiModel>(activity);
    }

    private async Task CancelActivityExecute(ActivityUiModel activity)
    {
        if (TryCancelActivity(activity.Id))
        {
            _runningActivities[activity.Id].Cancel();
            activity.Model.Status = ActivityStatus.Canceled;
            await _activityRepository.UpdateAsync(activity.Model);
            Activities.Remove(activity);
        }
    }

    #endregion

    #region Helper methods

    private async Task RefreshTopAndroids()
    {
        var androids = await _androidRepository.ListTopAndroids();
        TopAndroids.Clear();
        foreach (var android in androids)
        {
            TopAndroids.Add(android);
        }
    }

    private async void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => await RefreshTopAndroids();

    private async void OnActivityAdded(AddActivityMessage message)
    {
        var activity = await _activityRepository.AddAsync(message.Activity);

        var activityUiModel = new ActivityUiModel(activity);

        Activities.Add(activityUiModel);

        await ScheduleActivityAsync(activityUiModel);
    }

    private async void OnActivityEdited(EditActivityMessage message)
    {
        var activityUiModel = Activities.FirstOrDefault(a => a.Id == message.Activity.Id);

        if (activityUiModel != null)
        {
            if (TryCancelActivity(activityUiModel.Id))
            {
                activityUiModel.StartDateTime = message.Activity.StartDateTime;
                activityUiModel.EndDateTime = message.Activity.EndDateTime;

                await _activityRepository.UpdateAsync(activityUiModel.Model);

                await ScheduleActivityAsync(activityUiModel);
            }
        }
    }

    private async Task ScheduleActivityAsync(ActivityUiModel activityUiModel)
    {
        async void OnSuccess(Activity activity)
        {
            activity.Status = ActivityStatus.Completed;
            await _activityRepository.UpdateAsync(activity);
            await AsyncDispatcher.ExecuteOnMainThreadAsync(() => Activities.Remove(Activities.FirstOrDefault(f => f.Id == activity.Id)));
        }

        var progress = new Progress<string>(s => activityUiModel.RemainingTime = s);
        var cancellationTokenSource = new CancellationTokenSource();

        _runningActivities.TryAdd(activityUiModel.Model.Id, cancellationTokenSource);

        await _activityManager
            .OnSuccess(OnSuccess)
            .ScheduleActivityAsync(activityUiModel.Model,
                progress,
                cancellationTokenSource.Token);
    }

    private bool TryCancelActivity(int activityId)
    {
        if (_runningActivities.TryGetValue(activityId, out var cancellationTokenSource))
        {
            cancellationTokenSource.Cancel();
            return true;
        }

        return false;
    }

    #endregion
}