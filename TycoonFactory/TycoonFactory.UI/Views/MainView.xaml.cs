using MvvmCross.Platforms.Wpf.Presenters.Attributes;
using MvvmCross.Platforms.Wpf.Views;
using MvvmCross.ViewModels;
using TycoonFactory.ViewModels;

namespace TycoonFactory.UI.Views;

[MvxContentPresentation(WindowIdentifier = nameof(MainWindow), StackNavigation = true)]
[MvxViewFor(typeof(MainViewModel))]
public partial class MainView : MvxWpfView
{
    public MainView()
    {
        InitializeComponent();
    }
}