using System;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    public Text levelText;
    public event EventHandler<AchievePropertyEventArgs> MaxLevelReached;

    private void Start()
    {
        levelText.text = $"LEVEL {GameManager.LEVEL}";
        FindObjectOfType<WallBuilder>().LevelPassed += OnNextLevel;
    }
    //// Update is called once per frame
    //void Update()
    //{
    //    if (GameManager.LEVEL != GameManager.MAX_LEVEL)
    //        levelText.text = $"LEVEL {GameManager.LEVEL}";
    //    else
    //        levelText.text = $"ENDLESS";
    //}

    void OnNextLevel(object sender, EventArgs args)
    {
        GameManager.LEVEL++;

        if (GameManager.LEVEL != GameManager.MAX_LEVEL)
            levelText.text = $"LEVEL {GameManager.LEVEL}";
        else
        {
            FindObjectOfType<WallBuilder>().LevelPassed -= OnNextLevel;
            levelText.text = $"ENDLESS";

            MaxLevelReached?.Invoke(this, new AchievePropertyEventArgs(AchievementPropName.MaxLevelReached, true));
            MaxLevelReached = null;
        }
    }
}
