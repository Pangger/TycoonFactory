using TycoonFactory.DAL.Entities.Enums;

namespace TycoonFactory.DAL.Entities;

public class Activity
{
    public Activity()
    {
        Androids = new List<ActivityAndroid>();
    }

    public int Id { get; set; }
    public ActivityType Type { get; set; }
    public ActivityStatus Status { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public List<ActivityAndroid> Androids { get; set; }
    
}