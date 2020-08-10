using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public string introAudio;
    public Image blackScreen;
    public Image loading;
    public float sensitivity;
    public float camRotationSpeed;
    public event EventHandler<AchievePropertyEventArgs> EasterEggFound;
    private Camera mainCamera;
    private float volume;

    private TouchManager touchManager;
    private AudioManager audioManager;
    private IEnumerator audioFadeIn;

    List<HeldTouch> touches => 
        touchManager
            .touches
            .Where(touch => touch.target == TouchTarget.Screen)
            .OrderBy(touch => touch.id)
            .ToList();

    Sound mainTheme;

    private void Start()
    {
        mainCamera = Camera.main;
        audioManager = FindObjectOfType<AudioManager>();
        touchManager = FindObjectOfType<TouchManager>();
        var sliders = (Resources.FindObjectsOfTypeAll(typeof(AdjustSlider)) as AdjustSlider[]).ToList();

        sliders
            .Where(slider => slider.valueName == "Sensitivity")
            .First()
            .defaultValue = sensitivity;
        sliders
            .Where(slider => slider.valueName == "Volume")
            .First()
            .defaultValue = audioManager.defaultUserVolume;

        volume = PlayerPrefs.GetFloat("Volume", audioManager.defaultUserVolume);

        WallisAuth.Instance.Authenticate();
        //WallisAuth.Instance.GetLeaderboardScoreData(GPGSIds.leaderboard_testleaderboard);
    }

    private void Update()
    {
        if (mainTheme == null)
        {
            mainTheme = audioManager.GetSound(introAudio);
            StartCoroutine(audioFadeIn = AudioFade.FadeIn(mainTheme, .5f, Mathf.SmoothStep, 0, volume));
        }
        if (touches.Any())
        {
            var touch = touches[0];
            if (touch.Phase == TouchPhase.Began)
            {
                RaycastHit hit;
                Ray ray = mainCamera.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out hit) && LayerMask.LayerToName(hit.transform.gameObject.layer) == "EasterEgg")
                {
                    OpenURL("https://goo.gl/maps/Ewr4fmzCVYuG6y4CA");

                    EasterEggFound?.Invoke(this, new AchievePropertyEventArgs(AchievementPropName.EasterEggOpened, true));
                    EasterEggFound = null;
                }
            }

            mainCamera.transform.Rotate(Vector2.Perpendicular(touch.deltaPosition), -touch.deltaPosition.magnitude * Time.deltaTime);
        }
        mainCamera.transform.rotation *= Quaternion.Euler(Vector3.one * camRotationSpeed * Time.deltaTime);
    }

    public void Play(string sceneName)
    {
        StopCoroutine(audioFadeIn);
        StartCoroutine(AudioFade.FadeOut(mainTheme, .5f, Mathf.Lerp));
        StartCoroutine(ImageAlphaFade.FadeIn(blackScreen, .5f));

        FindObjectOfType<LevelLoader>().LoadLevel(sceneName, .5f);
        touchManager.enabled = false;
    }

    public void PlayGame()
    {
        if (PlayerPrefs.GetInt("TutorialShowed", 0) == 1)
            Play("Game");
        else
            Play("Tutorial");
    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }

    public void AdjustSensitivity(float sliderValue)
    {
        sensitivity = sliderValue;
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
    }

    public void AdjustVolume(float sliderValue)
    {
        volume = sliderValue;
        PlayerPrefs.SetFloat("Volume", volume);
        var sounds = audioManager.sounds;

        foreach (var sound in sounds)
            sound.source.volume = sound.defaultVolume * volume;
    }

    public void ShowLeaderboard()
    {
        Debug.Log("ShowLeaderboard");

        if (WallisAuth.Instance.IsAuthenticated)
            WallisAuth.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_testleaderboard);
    }
}
