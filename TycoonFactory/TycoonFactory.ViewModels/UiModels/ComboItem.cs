using MvvmCross.ViewModels;

namespace TycoonFactory.ViewModels.UiModels;

public class ComboItem<T> : MvxNotifyPropertyChanged
{
    public T Model { get; set; }

    public bool IsChecked
    {
        get => _isChecked;
        set => SetProperty(ref _isChecked, value);
    }
    private bool _isChecked;

    public ComboItem(T model)
    {
        Model = model;
    }
}