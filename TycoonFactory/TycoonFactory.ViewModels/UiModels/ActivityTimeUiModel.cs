using System.Runtime.InteropServices.JavaScript;
using MvvmCross.ViewModels;

namespace TycoonFactory.ViewModels.UiModels;

public class ActivityTimeUiModel : MvxNotifyPropertyChanged
{
    public int Hours
    {
        get => _hours;
        set => SetProperty(ref _hours, value);
    }
    private int _hours;
    
    public int Minutes
    {
        get => _minutes;
        set => SetProperty(ref _minutes, value);
    }
    private int _minutes;

    public TimeSpan GetTimeSpan() => new TimeSpan(Hours, Minutes, 0);

    public ActivityTimeUiModel(DateTime dateTime)
    {
        Hours = dateTime.Hour;
        Minutes = dateTime.Minute;
    }
}