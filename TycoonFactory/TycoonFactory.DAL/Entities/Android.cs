using System.Text.RegularExpressions;
using TycoonFactory.DAL.Entities.Enums;

namespace TycoonFactory.DAL.Entities;

public class Android
{
    public Android()
    {
        Activities = new List<ActivityAndroid>();
    }
    
    public int Id { get; set; }
    public string Name { get; set; }
    public List<ActivityAndroid> Activities { get; set; }

    public override string ToString()
    {
        return Name;
    }
}