using MvvmCross.Plugin.Messenger;
using TycoonFactory.DAL.Entities;
using TycoonFactory.ViewModels.UiModels;

namespace TycoonFactory.ViewModels.Messages;

public class AddActivityMessage : MvxMessage
{
    public Activity Activity { get; set; }
    
    public AddActivityMessage(object sender, Activity activity) : base(sender)
    {
        Activity = activity;
    }
}