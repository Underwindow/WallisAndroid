using System;
using UnityEngine;

public class Booster : MonoBehaviour
{
    public float maxSpeed;
    public float increaser;
    public float boostDelay;
    public event EventHandler<AchievePropertyEventArgs> BoostEnabled;
    public event EventHandler<AchievePropertyEventArgs> BoostDisabled;

    [SerializeField]
    Vector3 newMainCamRot;
    private float startFOV;
    private Transform mainCam;
    private Vector3 camRot;

    Joystick joystick;

    bool holdsFinger;
    bool isMaxLevel = false;
    bool isGameOver = false;
    bool isGamePaused = false;

    bool BoostReady
    {
        get
        {
            HeldTouch touch;
            return (isMaxLevel && !isGameOver && !isGamePaused) || (
                FingerExists(out touch) && 
                touch.ExistenceTime > boostDelay && 
                (holdsFinger ? true : GameManager.BOOST)
                );
        }
    }

    bool FingerExists(out HeldTouch touch) => (touch = joystick.GetFinger()) != null;

    void Start()
    {
        joystick = FindObjectOfType<Joystick>();
        mainCam = Camera.main.gameObject.transform;
        camRot = mainCam.rotation.eulerAngles;
        startFOV = Camera.main.fieldOfView;
        FindObjectOfType<Level>().MaxLevelReached += OnMaxLevelReached;
        FindObjectOfType<GameManager>().GameOver.AddListener(OnGameOver);
        FindObjectOfType<PauseEvent>().gamePaused.AddListener(OnPauseEvent);
        FindObjectOfType<PauseEvent>().gameResumed.AddListener(OnPauseEvent);
        FindObjectOfType<WallBuilder>().WallPassed += OnWallPassed;
    }

    void OnMaxLevelReached(object sender, EventArgs args) 
        => isMaxLevel = true;
    void OnGameOver()
        => isGameOver = true;
    void OnPauseEvent()
        => isGamePaused = !isGamePaused;
    void OnWallPassed(object sender, EventArgs args)
        => GameManager.SCORE_BONUS += GameManager.BOOST? 1 : 0;

    void Update()
    {
        if (!GameManager.BOOST)
            ObserveFinger();

        if (BoostReady)
            Boost();
        else
            ReduceBoosting();
    }

    void ObserveFinger()
    {
        HeldTouch touch; 
        if (FingerExists(out touch))
            if (touch.Phase == TouchPhase.Began)
                holdsFinger = true;
            else if (touch.Phase == TouchPhase.Ended || (touch.position - touch.startPosition).magnitude > 20f)
                holdsFinger = false;
    }

    void Boost()
    {
        if (!GameManager.BOOST)
        {
            GameManager.BOOST = true;
            GameManager.SCORE_BONUS += 1;

            BoostEnabled?.Invoke(this, new AchievePropertyEventArgs(AchievementPropName.NoBoostsToMaxLevel, GameManager.LEVEL >= GameManager.MAX_LEVEL));

            FindObjectOfType<AudioManager>().Play("Boost");
        }

        GameManager.SPEED = Mathf.Lerp(GameManager.SPEED, maxSpeed, increaser);

        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 100, increaser);
        mainCam.rotation = Quaternion.Lerp(mainCam.rotation, Quaternion.Euler(newMainCamRot), increaser);
    }

    void ReduceBoosting()
    {
        if (GameManager.BOOST)
        {
            GameManager.BOOST = false;
            GameManager.SCORE_BONUS = 1;

            if (GameManager.SPEED > 0)
                GameManager.SetLevelSpeed();
            
            BoostDisabled?.Invoke(this, new AchievePropertyEventArgs(AchievementPropName.NoDisBoostingToLastLevel, GameManager.LEVEL >= GameManager.MAX_LEVEL));
        }

        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, startFOV, increaser * .5f);
        mainCam.rotation = Quaternion.Lerp(mainCam.rotation, Quaternion.Euler(camRot), increaser);
    }
}
