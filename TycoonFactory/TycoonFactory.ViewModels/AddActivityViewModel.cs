using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using TycoonFactory.DAL.Entities;
using TycoonFactory.DAL.Entities.Enums;
using TycoonFactory.DAL.Repositories.Interfaces;
using TycoonFactory.ViewModels.Messages;
using TycoonFactory.ViewModels.UiModels;

namespace TycoonFactory.ViewModels;

public class ComboItemAndroid : ComboItem<Android>
{
    public ComboItemAndroid(Android model) : base(model)
    {
    }
}

public class AddActivityViewModel : MvxNavigationViewModel<ActivityUiModel>
{
    #region Private fields

    private readonly IAndroidRepository _androidRepository;
    private readonly IMvxMessenger _mvxMessenger;
    private List<Android> _androids = new();
    private readonly List<ValidationResult> _errors = new List<ValidationResult>();

    #endregion

    #region Commands

    public MvxCommand AddActivityCommand { get; }
    public MvxAsyncCommand CloseCommand { get; }

    #endregion

    #region Properties

    public bool IsCreateMode
    {
        get => _isCreateMode;
        set => SetProperty(ref _isCreateMode, value);
    }

    private bool _isCreateMode;

    public DateTime StartDate
    {
        get => _startDate;
        set => SetProperty(ref _startDate, value, OnDateChanged);
    }

    private DateTime _startDate = DateTime.Today;

    public DateTime EndDate
    {
        get => _endDate;
        set => SetProperty(ref _endDate, value, OnDateChanged);
    }

    private DateTime _endDate = DateTime.Today;

    public ActivityTimeUiModel StartTime
    {
        get => _startTime;
        set => SetProperty(ref _startTime, value, OnDateChanged);
    }

    private ActivityTimeUiModel _startTime = new(DateTime.Now);

    public ActivityTimeUiModel EndTime
    {
        get => _endTime;
        set => SetProperty(ref _endTime, value, OnDateChanged);
    }

    private ActivityTimeUiModel _endTime = new(DateTime.Now.AddHours(1));

    public DateTime StartDateTime => StartDate.Add(StartTime.GetTimeSpan());
    public DateTime EndDateTime => EndDate.Add(EndTime.GetTimeSpan());

    public ActivityUiModel Activity
    {
        get => _activity;
        set => SetProperty(ref _activity, value);
    }

    private ActivityUiModel _activity;

    public ObservableCollection<ComboItem<Android>> AvailableAndroids { get; } = new();

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    private string _errorMessage;

    #endregion

    #region Constructor

    public AddActivityViewModel(
        ILoggerFactory logFactory,
        IMvxNavigationService navigationService,
        IAndroidRepository androidRepository,
        IMvxMessenger mvxMessenger)
        : base(logFactory, navigationService)
    {
        _androidRepository = androidRepository;
        _mvxMessenger = mvxMessenger;

        AddActivityCommand = new MvxCommand(AddActivityExecute);
        CloseCommand = new MvxAsyncCommand(CloseExecute);

        IsCreateMode = true;
        Activity = new ActivityUiModel(new Activity());
    }

    #endregion

    #region Command methods

    private void AddActivityExecute()
    {
        Activity.StartDateTime = StartDateTime;
        Activity.EndDateTime = EndDateTime;

        if (IsCreateMode)
        {
            Activity.Androids.Clear();
            Activity.Androids
                .AddRange(AvailableAndroids.Where(w => w.IsChecked).Select(android =>
                    new ActivityAndroid
                    {
                        ActivityId = Activity.Model.Id,
                        AndroidId = android.Model.Id,
                        Android = android.Model
                    }));
        }

        if (Validator.TryValidateObject(_activity, new ValidationContext(_activity), _errors))
        {
            if (IsCreateMode)
                _mvxMessenger.Publish(new AddActivityMessage(this, Activity.Model));
            else
                _mvxMessenger.Publish(new EditActivityMessage(this, Activity.Model));

            NavigationService.Close(this);
        }
        else
        {
            ErrorMessage = _errors.Any() ? _errors.First().ErrorMessage : "Fatal error";
            _errors.Clear();
        }
    }

    private Task CloseExecute()
    {
        return NavigationService.Close(this);
    }

    #endregion

    #region Helper methods

    public override void Prepare(ActivityUiModel? parameter)
    {
        if (parameter != null)
        {
            IsCreateMode = false;
            Activity = parameter;
            
            StartDate = parameter.StartDateTime.Date;
            EndDate = parameter.EndDateTime.Date;
            StartTime = new ActivityTimeUiModel(parameter.StartDateTime);
            EndTime = new ActivityTimeUiModel(parameter.EndDateTime);
        }
    }

    public override async Task Initialize()
    {
        _androids = await _androidRepository.ListAsync();
        FillAvailableAndroids();
    }

    private IEnumerable<Android> GetAvailableAndroids()
    {
        var restHoursForCurrentActivity = Activity.Type == ActivityType.BuildComponent ? 2 : 4;

        return _androids.Where(w =>
            w.Activities.All(a =>
                (StartDateTime < a.Activity.StartDateTime &&
                 EndDateTime.AddHours(restHoursForCurrentActivity) < a.Activity.StartDateTime)
                || (StartDateTime >
                    a.Activity.EndDateTime.AddHours(a.Activity.Type == ActivityType.BuildComponent ? 2 : 4) &&
                    EndDateTime > a.Activity.EndDateTime)));
    }

    private void OnDateChanged()
    {
        FillAvailableAndroids();
    }

    private void FillAvailableAndroids()
    {
        AvailableAndroids.Clear();
        foreach (var android in GetAvailableAndroids().Select(s => new ComboItem<Android>(s)))
        {
            AvailableAndroids.Add(android);
        }
    }

    #endregion
}