using System;
using System.Collections.Generic;
using UnityEngine;

using UnityEventHandler = UnityEventHandler<AchievePropertyEventArgs>;

public enum AchievementPropName
{
    MaxLevelReached,
    NoBoostsToMaxLevel,
    TutorialSkipped,
    TutorialCompleted,
    EasterEggOpened,
    FirstWallPassedOnBoost,
    GameOver,
    NoDisBoostingToLastLevel
}

public enum AchievementName
{
    LostVirginity,
    SpeedLover,
    EasterEgg,
    ExpandingBrain,
    QuiterYouGo,
    LightAndDarkness,
    SkilledPlayer
}

public class AchievePropertyEventArgs : EventArgs
{
    public AchievementPropName Name;
    public Dictionary<AchievementPropName, bool> props;
    public bool Completed { get; set; }

    public AchievePropertyEventArgs(AchievementPropName name, bool completed = false)
    {
        Name = name;
        Completed = completed;
    }

    public AchievePropertyEventArgs(Dictionary<AchievementPropName, bool> props)
    {
        this.props = props;
    }
}

public class UnlockAchievementEventArgs : EventArgs
{
    public AchievementName Name;

    public UnlockAchievementEventArgs(AchievementName name)
    {
        Name = name;
    }
}

public class Achievement : MonoBehaviour
{
    public AchievementName achievementName;
    public AchievementProperty[] properties;
    public Dictionary<AchievementPropName, AchievementProperty> propDict = new Dictionary<AchievementPropName, AchievementProperty>();
    public event EventHandler<UnlockAchievementEventArgs> Unlocked;

    private bool achieved = false;
    private List<UnityEventHandler> unityEventHandlers = new List<UnityEventHandler>();

    private void Start()
    {
        achieved = PlayerPrefs.GetInt(achievementName.ToString(), 0) == 1;
        if (achieved)
            return;

        foreach (var prop in properties)
            propDict.Add(prop.Name, prop);

        UnityEngine.Object obj = FindObjectOfType<Level>();
        if (obj != null)
        {
            (obj as Level).MaxLevelReached += Achieve;
            var unityEventHandler = new UnityEventHandler(
                    FindObjectOfType<GameManager>().GameOver,
                    new AchievePropertyEventArgs(AchievementPropName.GameOver, true)
                    );
            unityEventHandler.eventHandler += Achieve;
            unityEventHandlers.Add(unityEventHandler);
        }

        if ((obj = FindObjectOfType<Booster>()) != null)
        {
            (obj as Booster).BoostEnabled += Achieve;
            (obj as Booster).BoostDisabled += Achieve;
        }

        if ((obj = FindObjectOfType<WallBuilder>()) != null)
            (obj as WallBuilder).WallPassed += Achieve;
        
        if ((obj = FindObjectOfType<TutorialManager>()) != null)
        {
            (obj as TutorialManager).tutorialCompleted += Achieve;
            (obj as TutorialManager).tutorialSkipped += Achieve;
        }

        if ((obj = FindObjectOfType<MenuManager>()) != null)
        {
            (obj as MenuManager).EasterEggFound += Achieve;
        }
    }

    public void Achieve(object sender, AchievePropertyEventArgs propertyArgs)
    {
        if (achieved)
            return;

        if (!propDict.ContainsKey(propertyArgs.Name))
            return;


        if (propDict[propertyArgs.Name].Invoked)
            return;
        propDict[propertyArgs.Name].Completed = propertyArgs.Completed;
        propDict[propertyArgs.Name].Invoked = true;

        foreach (var prop in propDict.Values)
            if (!prop.Invoked)
                return;

        TryCompleteAchievement();
    }

    public void TryCompleteAchievement()
    {
        foreach (var prop in propDict.Values)
            if (!prop.Completed)
                return;

        Unlocked?.Invoke(this, new UnlockAchievementEventArgs(achievementName));
        achieved = true;
        PlayerPrefs.SetInt(achievementName.ToString(), 1);
    }

    private void OnDestroy()
    {
        if (achieved)
            return;

        TryCompleteAchievement();
    }
}
