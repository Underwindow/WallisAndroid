using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;


public class AchievementNotifier : MonoBehaviour
{
    public GameObject badge;
    public Text badgeText;

    private int count;

    // Start is called before the first frame update
    void Start()
    {
        count = PlayerPrefs.GetInt("NewAchievementsCount", 0);
        ShowNewAchievementsCount();

        var achievements = FindObjectsOfType<Achievement>().ToList();
        foreach (var achievement in achievements)
            achievement.Unlocked += OnAchievementUnlocked;
    }

    void OnAchievementUnlocked(object sender, UnlockAchievementEventArgs args)
    {
        PlayerPrefs.SetInt("NewAchievementsCount", ++count);
        Debug.Log($"{args.Name} achievement unlocked! Count: {count}");

        var list = PlayerPrefs.GetString("NewAchievementsList", "");
        PlayerPrefs.SetString("NewAchievementsList", list + args.Name.ToString());
        
        ShowNewAchievementsCount();
    }

    public void CheckNewAchievements()
    {
        count = 0;
        PlayerPrefs.SetInt("NewAchievementsCount", count);
        ShowNewAchievementsCount();
    }

    public void ShowNewAchievementsCount()
    {
        if (badge != null)
        {
            if (count == 0)
                badge.SetActive(false);
            else
            {
                badge.SetActive(true);
                badgeText.text = count.ToString();
            }
        }
    }
}
