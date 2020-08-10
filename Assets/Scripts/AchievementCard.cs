using UnityEngine;

public class AchievementCard : MonoBehaviour
{
    public AchievementName achievementName;
    public GameObject locker;
    public GameObject newItemIcon;

    // Start is called before the first frame update

    private void OnEnable()
    {
        bool achieved = PlayerPrefs.GetInt(achievementName.ToString(), 0) == 1;
        ShowAchievement(achieved);
        
        var achievementsList = PlayerPrefs.GetString("NewAchievementsList", "");
        print(achievementsList);
        bool isNewAchievement = achievementsList.Contains(achievementName.ToString());
        ShowNewIcon(isNewAchievement);
        
        if (isNewAchievement)
            PlayerPrefs.SetString("NewAchievementsList", achievementsList.Replace(achievementName.ToString(), ""));
    }

    void ShowAchievement(bool visible)
    {
        locker.SetActive(!visible);
    }

    void ShowNewIcon(bool visible)
    {
        newItemIcon.SetActive(visible);
    }
}