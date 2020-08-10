using UnityEngine;

[System.Serializable]
public class AchievementProperty 
{
    public AchievementPropName Name;
    public bool Completed;
    [HideInInspector]
    public bool Invoked;

    public AchievementProperty(AchievementPropName name, bool completed)
    {
        Name = name;
        Completed = completed;
        Invoked = false;
    }

    public override string ToString()
    {
        return $"AchievementProp.{Name}, invoked?{Invoked}, completed?{Completed}";
    }
}
