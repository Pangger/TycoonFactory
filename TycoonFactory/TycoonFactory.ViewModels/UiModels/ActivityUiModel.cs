using MvvmCross.ViewModels;
using TycoonFactory.DAL.Entities;
using TycoonFactory.DAL.Entities.Enums;
using TycoonFactory.ViewModels.Validators;

namespace TycoonFactory.ViewModels.UiModels;

[ActivityValidation]
public class ActivityUiModel : MvxNotifyPropertyChanged
{
    private readonly Activity _model;

    public int Id => _model.Id;
    
    public ActivityType Type
    {
        get => _model.Type;
        set
        {
            
            _model.Type = value;
            RaisePropertyChanged();
        }
    }

    public DateTime StartDateTime
    {
        get => _model.StartDateTime;
        set
        {
            _model.StartDateTime = value;
            RaisePropertyChanged();
        }
    }
    
    public DateTime EndDateTime
    {
        get => _model.EndDateTime;
        set
        {
            _model.EndDateTime = value;
            RaisePropertyChanged();
        }
    }
    
    public List<ActivityAndroid> Androids => _model.Androids;

    public Activity Model => _model;

    public string RemainingTime
    {
        get => _remainingTime;
        set => SetProperty(ref _remainingTime, value);
    }
    private string _remainingTime;

    public ActivityUiModel(Activity activity)
    {
        _model = activity;
    }
}