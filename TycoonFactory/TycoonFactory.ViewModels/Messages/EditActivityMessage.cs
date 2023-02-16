using MvvmCross.Plugin.Messenger;
using TycoonFactory.DAL.Entities;

namespace TycoonFactory.ViewModels.Messages;

public class EditActivityMessage : MvxMessage
{
    public Activity Activity { get; set; }
    
    public EditActivityMessage(object sender, Activity activity) : base(sender)
    {
        Activity = activity;
    }
}