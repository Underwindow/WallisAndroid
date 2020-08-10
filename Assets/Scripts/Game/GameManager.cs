using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public GameObject cam;
    public float camSmoothSpeed = 0.025f;
    public Vector3 camOffset;

    public const float START_SPEED = 9f;
    public const int MAX_LEVEL = 10;

    #region static variables
    static public float SPEED = START_SPEED;
    static public int SCORE = 0;
    static public int SCORE_BONUS = 1;
    static public bool BOOST = false;
    static public int LEVEL = 1;
    #endregion

    const float SPEED_INCREASER = 2f;
    bool gameOver = false;
    public UnityEvent GameOver;
    //public event EventHandler<AchievePropertyEventArgs> MaxLevelReached;

    private GameObject currFigure;
    [HideInInspector]
    public Vector3 prevFigureStartPos = Vector3.zero;

    public string gameplaySoundName;

    IEnumerator audioCoroutine;
    Sound gameplaySound;
    AudioManager audioManager;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        currFigure = FindObjectOfType<FigureMotion>().gameObject;

        var wallBuilder = FindObjectOfType<WallBuilder>();
        wallBuilder.LevelPassed += OnNextLevel;
        wallBuilder.WallPassed += SaveFigurePos;
    }

    void Update()
    {
        if (audioCoroutine == null)
        {
            gameplaySound = audioManager.GetSound(gameplaySoundName);
            StartCoroutine(audioCoroutine = AudioFade.FadeIn(gameplaySound, 5f, Mathf.SmoothStep, .5f, audioManager.userVolume));
        }
        if (!gameOver && currFigure == null)
        {
            var figureMotion = FindObjectOfType<FigureMotion>();
            currFigure = figureMotion.gameObject;
            prevFigureStartPos = figureMotion.startPos;
        }
        CameraFollowFigure();

    }

    void SaveFigurePos(object sender, WallPassingEventArgs e)
    {
        prevFigureStartPos = e.FigurePrevPosition;
    }

    // Update Camera following
    void LateUpdate()
    {
        //CameraFollowFigure();
    }

    void OnNextLevel(object sender, LevelPassingEventArgs e)
    {
        SPEED += BOOST? 0 : SPEED_INCREASER;  
    }

    void CameraFollowFigure()
    {
        Vector3 figurePos;

        try
        {
            figurePos = currFigure.transform.position;
        }
        catch (Exception)
        {
            figurePos = prevFigureStartPos;
        }
        Vector3 desiredPosition = figurePos + camOffset + (
            BOOST
            ? new Vector3(UnityEngine.Random.Range(-.1f, .1f), UnityEngine.Random.Range(-.1f, .1f))
            : Vector3.zero
            );
        Vector3 smoothedPosition = Vector3.Slerp(cam.transform.position, desiredPosition, camSmoothSpeed);
        cam.transform.position = smoothedPosition;
    }

    public void GamePause(bool pause)
    {
        StopCoroutine(audioCoroutine);

        if (pause)
        {
            SPEED = 0;
            audioCoroutine = AudioFade.FadeOut(gameplaySound, 0f, Mathf.Lerp);
        }
        else
        {
            SetLevelSpeed();
            audioCoroutine = AudioFade.FadeIn(gameplaySound, .3f, Mathf.Lerp, audioManager.userVolume);
        }

        StartCoroutine(audioCoroutine);
    }

    static public void SetLevelSpeed()
    {
        SPEED = START_SPEED + SPEED_INCREASER * (LEVEL - 1);
    }

    #region Game ending
    public void EndGame()
    {
        gameOver = true;
        StartCoroutine(AudioFade.FadeOut(gameplaySound, 1f, Mathf.Lerp));
    }

    public void SaveScore()
    {
        var balance = PlayerPrefs.GetInt("Balance", 0);
        PlayerPrefs.SetInt("Balance", balance + SCORE);

        var highscore = PlayerPrefs.GetInt("Highscore", 0);
        if (SCORE > highscore)
        {
            PlayerPrefs.SetInt("Highscore", SCORE);
            PlayerPrefs.SetInt("NewRecord", 1);
        }
    }
    #endregion

    private void OnDestroy()
    {
        LEVEL = 1;
        SCORE = 0;
        SPEED = START_SPEED;
    }
}
