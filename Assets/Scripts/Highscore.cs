using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Highscore : MonoBehaviour
{
    public TextMeshProUGUI hstext;
    public GameObject newHighScoreText;
    public string hsSpritetag;
    public UnityEvent NewHighscore;

    // Start is called before the first frame update
    void Start()
    {

        var highscore = PlayerPrefs.GetInt("Highscore", 0);
        if (highscore == 0)
            hstext.text = "";
        else
        {
            if (PlayerPrefs.GetInt("NewRecord", 0) == 1)
            {
                NewHighscore?.Invoke();
                ReportScore(highscore);

                PlayerPrefs.SetInt("NewRecord", 0);
            }

            hstext.text = hsSpritetag + highscore.ToString();
        }
    }

    void ReportScore(int score)
    {
        Debug.Log("ReportScore");

        if (!WallisAuth.Instance.IsAuthenticated)
            return;

        WallisAuth.Instance.ReportScore(score, GPGSIds.leaderboard_testleaderboard);
    }
}