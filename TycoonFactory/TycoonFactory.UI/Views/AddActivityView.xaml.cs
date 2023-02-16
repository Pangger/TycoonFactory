using System.Windows.Controls;
using MvvmCross.Platforms.Wpf.Presenters.Attributes;
using MvvmCross.Platforms.Wpf.Views;
using MvvmCross.ViewModels;
using TycoonFactory.ViewModels;

namespace TycoonFactory.UI.Views;

[MvxContentPresentation(WindowIdentifier = nameof(MainWindow), StackNavigation = true)]
[MvxViewFor(typeof(AddActivityViewModel))]
public partial class AddActivityView : MvxWpfView
{
    public AddActivityView()
    {
        InitializeComponent();
    }
}