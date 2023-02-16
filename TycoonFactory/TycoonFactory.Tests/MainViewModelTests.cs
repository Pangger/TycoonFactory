using Microsoft.Extensions.Logging;
using Moq;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using TycoonFactory.Core.Managers.Interfaces;
using TycoonFactory.DAL.Entities;
using TycoonFactory.DAL.Entities.Enums;
using TycoonFactory.DAL.Repositories.Interfaces;
using TycoonFactory.ViewModels;
using TycoonFactory.ViewModels.Messages;
using TycoonFactory.ViewModels.UiModels;

namespace TycoonFactory.Tests;

[TestFixture]
public class MainViewModelTests
{
    private MainViewModel _mainViewModel;
    private AddActivityViewModel _addActivityViewModel;
    private Mock<IActivityRepository> _activityRepositoryMock;

    [OneTimeSetUp]
    public void Setup()
    {
        var loggerFactoryMock = new Mock<ILoggerFactory>();
        var navigationServiceMock = new Mock<IMvxNavigationService>();

        var androidRepositoryMock = new Mock<IAndroidRepository>();
        _activityRepositoryMock = new Mock<IActivityRepository>();
        var activityManagerMock = new Mock<IActivityManager>();
        var messenger = new MvxMessengerHub();

        activityManagerMock.Setup(manager => manager.OnSuccess(It.IsAny<Action<Activity>>()))
            .Returns(activityManagerMock.Object);
        activityManagerMock.Setup(manager => manager.ScheduleActivityAsync(It.IsAny<Activity>(),
            It.IsAny<IProgress<string>>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        androidRepositoryMock.Setup(repo => repo.ListTopAndroids()).Returns(Task.FromResult(new List<Android>()));

        _mainViewModel = new MainViewModel(
            loggerFactoryMock.Object,
            navigationServiceMock.Object,
            androidRepositoryMock.Object,
            _activityRepositoryMock.Object,
            activityManagerMock.Object,
            messenger
        );

        _addActivityViewModel = new AddActivityViewModel(
            loggerFactoryMock.Object,
            navigationServiceMock.Object,
            androidRepositoryMock.Object,
            messenger);
    }

    [Test, Order(1)]
    public async Task AddAndroidTest()
    {
        if (_mainViewModel.AddAndroidCommand.CanExecute())
        {
            await _mainViewModel.AddAndroidCommand.ExecuteAsync();
        }

        Assert.True(_mainViewModel.AddAndroidCommand.CanExecute());
        Assert.That(_mainViewModel.Androids.Count, Is.EqualTo(1), "Android wasn`t added");
    }

    [Test, Order(2)]
    public void CreateBuildComponentActivityTest()
    {
        var startDateTime = DateTime.Now;
        var endDateTime = DateTime.Now.AddHours(1);
        var activityType = ActivityType.BuildComponent;
        var android = _mainViewModel.Androids.FirstOrDefault();
        var activityToAdd = new Activity()
        {
            Id = 1,
            Type = activityType,
            StartDateTime = startDateTime,
            EndDateTime = endDateTime,
            Status = ActivityStatus.InProgress,
            Androids = new List<ActivityAndroid>()
            {
                new()
                {
                    AndroidId = android.Id,
                    Android = android,
                    ActivityId = 1
                }
            }
        };

        _activityRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Activity>()))
            .Returns(Task.FromResult(activityToAdd));

        if (android == null)
            Assert.Fail("Androids collection is empty");

        if (_addActivityViewModel.AddActivityCommand.CanExecute())
        {
            _addActivityViewModel.Activity = new ActivityUiModel(activityToAdd);
            _addActivityViewModel.AvailableAndroids.Add(new ComboItem<Android>(android) {IsChecked = true});

            _addActivityViewModel.AddActivityCommand.Execute();
        }

        var activity = _mainViewModel.Activities.FirstOrDefault();
        Assert.NotNull(activity, "Activity is null");

        Assert.That(activity.StartDateTime.Date, Is.EqualTo(startDateTime.Date), "StartDateTime.Date is different");
        Assert.That(activity.StartDateTime.Hour, Is.EqualTo(startDateTime.Hour), "StartDateTime.Hour is different");
        Assert.That(activity.StartDateTime.Minute, Is.EqualTo(startDateTime.Minute), "StartDateTime.Minute is different");
        Assert.That(activity.EndDateTime.Date, Is.EqualTo(endDateTime.Date), "EndDateTime.Date is different");
        Assert.That(activity.EndDateTime.Hour, Is.EqualTo(endDateTime.Hour), "EndDateTime.Hour is different");
        Assert.That(activity.EndDateTime.Minute, Is.EqualTo(endDateTime.Minute), "EndDateTime.Minute is different");
        Assert.That(activity.Type, Is.EqualTo(activityType), "Activity is different");
    }

    [Test, Order(3)]
    public void EditActivityTest()
    {
        if(!_mainViewModel.Activities.Any())
            Assert.Fail("Activities collection is empty");

        var activityToEdit = _mainViewModel.Activities.First();
        
        var editStartDateTime = activityToEdit.StartDateTime.AddHours(1).AddMinutes(25);
        var editEndDateTime = activityToEdit.EndDateTime.AddDays(1).AddHours(2).AddMinutes(45);

        _addActivityViewModel.Activity = activityToEdit;
        _addActivityViewModel.StartDate = editStartDateTime.Date;
        _addActivityViewModel.EndDate = editEndDateTime.Date;
        _addActivityViewModel.StartTime = new ActivityTimeUiModel(editStartDateTime);
        _addActivityViewModel.EndTime = new ActivityTimeUiModel(editEndDateTime);
        
        if (_addActivityViewModel.AddActivityCommand.CanExecute())
        {
            _addActivityViewModel.AddActivityCommand.Execute();
        }

        var activity = _mainViewModel.Activities.FirstOrDefault();
        Assert.NotNull(activity);

        Assert.That(activity.StartDateTime.Date, Is.EqualTo(editStartDateTime.Date), "StartDateTime.Date is different");
        Assert.That(activity.StartDateTime.Hour, Is.EqualTo(editStartDateTime.Hour), "StartDateTime.Hour is different");
        Assert.That(activity.StartDateTime.Minute, Is.EqualTo(editStartDateTime.Minute),"StartDateTime.Minute is different");
        Assert.That(activity.EndDateTime.Date, Is.EqualTo(editEndDateTime.Date), "EndDateTime.Date is different");
        Assert.That(activity.EndDateTime.Hour, Is.EqualTo(editEndDateTime.Hour), "EndDateTime.Hour is different");
        Assert.That(activity.EndDateTime.Minute, Is.EqualTo(editEndDateTime.Minute), "EndDateTime.Minute is different");
    }
    
    [Test, Order(4)]
    public void CancelActivityTest()
    {
        if(!_mainViewModel.Activities.Any())
            Assert.Fail("Activities collection is empty");

        var activityToCancel = _mainViewModel.Activities.First();

        if (_mainViewModel.CancelActivityCommand.CanExecute(activityToCancel))
        {
            _mainViewModel.CancelActivityCommand.Execute(activityToCancel);
        }
        
        Assert.False(_mainViewModel.Activities.Contains(activityToCancel), "Activity wasn`t deleted");
    }

    [Test, Order(5)]
    public async Task RemoveAndroidTest()
    {
        if (!_mainViewModel.Androids.Any())
            Assert.Fail("Androids collection is empty");

        var androidToDelete = _mainViewModel.Androids.First();

        if (_mainViewModel.RemoveAndroidCommand.CanExecute(androidToDelete))
        {
            await _mainViewModel.RemoveAndroidCommand.ExecuteAsync(androidToDelete);
        }
        
        Assert.False(_mainViewModel.Androids.Contains(androidToDelete), "Androids wasn`t deleted");
    }
}