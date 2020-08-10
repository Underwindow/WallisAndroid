using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using UnityEngine.Android;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;

public class WallisAuth : MonoBehaviour
{
    public static WallisAuth Instance { get; private set; }
    public bool IsAuthenticated 
        => PlayGamesPlatform.Instance.IsAuthenticated();
    public string UserInfo
        => IsAuthenticated
        ? $"Username: {Social.localUser.userName}\n" +
          $"User ID: {Social.localUser.id}\n" +
          $"IsUnderage: {Social.localUser.underage}"
        : "User isn't logged in";

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
    }

    void Start()
    {
        FindObjectOfType<LevelLoader>().LoadLevel("Menu");
    }

    public void Authenticate()
        => Social.localUser.Authenticate((bool success) => {
            //var debugText = FindObjectOfType<DebugText>()?.debugText;
            //debugText.text = "Authentication " + (success ? "successful" : "failed");
            if (success)
            {
                Debug.Log("Authentication successful");
                Debug.Log(UserInfo);
                //debugText.text += $"\n{UserInfo}";
                GetLeaderboardData(GPGSIds.leaderboard_testleaderboard);
            }
            else
                Debug.Log("Authentication failed");
        });

    public string GetLeaderboardScoreData(string leaderboardId)
    {
        string scoreData = "load failed";

        try
        {
            PlayGamesPlatform.Instance.LoadScores(
                leaderboardId,
                LeaderboardStart.PlayerCentered,
                1,
                LeaderboardCollection.Public,
                LeaderboardTimeSpan.AllTime,
                (LeaderboardScoreData data) =>
            {
                var formattedData =
                    $"data.Valid:{data.Valid}" +
                    $"data.Id:{data.Id}" +
                    $"data.PlayerScore:{data.PlayerScore}" +
                    $"data.PlayerScore.userID:{data.PlayerScore.userID}" +
                    $"data.PlayerScore.formattedValue:{data.PlayerScore.formattedValue}";

                scoreData = formattedData;
                        //var debugText = FindObjectOfType<DebugText>()?.debugText.text;
                        //debugText = formattedData;
                        Debug.Log(formattedData);
            });
        }
        catch (System.NullReferenceException)
        {
            print("load failed");
        }

        FindObjectOfType<DebugText>().debugText.text = scoreData;
        //        Social.LoadScores(
        //leaderboardId,
        //(IScore[] data) => {
        //    var scoresList = data.ToList();
        //    var userScoreData = scoresList.Find(score => score.userID == Social.localUser.id);
        //    var formattedData =
        //        $"date:{userScoreData.date}" +
        //        $"rank:{userScoreData.rank}" +
        //        $"userID:{userScoreData.userID}" +
        //        $"value:{userScoreData.value}" +
        //        $"formattedValue:{userScoreData.formattedValue}";

        //    scoreData = formattedData;
        //    Debug.Log(formattedData);
        //});

        return scoreData;
    }

    public void ShowLeaderboardUI(string id)
    {
#if UNITY_ANDROID
        PlayGamesPlatform.Instance.ShowLeaderboardUI(id);
#elif UNITY_IPHONE
        Social.ShowLeaderboardUI();
#endif
    }

    public void ReportScore(int score, string id) 
        => Social.ReportScore(score, id, (bool success) =>
        {
            //var debugText = FindObjectOfType<DebugText>().debugText;
                //debugText.text = "Social.ReportScore " + (success ? "successful" : "failed");
        });

    //public string getAndroidEmailAccounts()
    //{
    //    AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    //    AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

    //    AndroidJavaClass ACManager = new AndroidJavaClass("android.accounts.AccountManager");
    //    AndroidJavaObject ACManagerAct = ACManager.CallStatic<AndroidJavaObject>("get", currentActivity);

    //    AndroidJavaObject accounts = ACManagerAct.Call<AndroidJavaObject>("getAccountsByType", "com.google");
    //    AndroidJavaObject[] accountArray = AndroidJNIHelper.ConvertFromJNIArray<AndroidJavaObject[]>(accounts.GetRawObject());

    //    string[] accountName = new string[accountArray.Length];

    //    for (int i = 0; i < accountName.Length; i++)
    //    {
    //        accountName[i] = accountArray[i].Get<string>("name");
    //    }

    //    return accountName[0];
    //}

    private void OnApplicationQuit() 
        => PlayGamesPlatform.Instance.SignOut();

    private void GetLeaderboardData(string id)
    {
        PlayGamesPlatform.Instance.LoadScores(id, LeaderboardStart.PlayerCentered, 1, LeaderboardCollection.Public, LeaderboardTimeSpan.AllTime, data => {
            StartCoroutine(IEGetLeaderboardPlayers(data));
        });
    }

    private IEnumerator IEGetLeaderboardPlayers(LeaderboardScoreData data)
    {
        var debugText = FindObjectOfType<DebugText>().debugText;
        if (!data.Valid || (data.Status != ResponseStatus.Success && data.Status != ResponseStatus.SuccessWithStale))
        {
            Debug.Log(debugText.text = "DEBUG+: Valid: " + data.Valid + ", Status: " + data.Status.ToString());
            yield break;
        }
        else Debug.Log(debugText.text = "DEBUG+: Scores loaded successful!\n");

        List<string> usersIDs = new List<string>(data.Scores.Select(score => score.userID));
        foreach (var score in data.Scores)
        {
            if (Social.localUser.id == score.userID)
                debugText.text += $"Rank-{score.rank} score: {score.value}";
        }
    }
}
