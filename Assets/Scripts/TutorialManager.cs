using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;

public class TutorialManager : MonoBehaviour
{
    public UnityEvent tutorialBegins;
    public UnityEvent rotationComplete;
    private int rotationId = 0;
    public UnityEvent movementComplete;
    public UnityEvent pauseComplete;
    private UnityEvent boostComplete = new UnityEvent();
    public event EventHandler<AchievePropertyEventArgs> tutorialCompleted;
    public event EventHandler<AchievePropertyEventArgs> tutorialSkipped;

    public Text finalText;

    public Image joystickImage;
    public Image blackTransition;

    public List<Image> gestures;
    public Image pausePlace;
    bool paused = false;

    public List<string> rotations;

    public List<RotatingDirection> rotationsSeq;

    private Coroutine stageCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("TutorialShowed", 1);

        tutorialBegins.Invoke();
        boostComplete.AddListener(Boost);
        FindObjectOfType<FigureMotion>().FigureRotated += Rotation;
        FindObjectOfType<WallBuilder>().WallPassed += OnWallPassed;
        FindObjectOfType<Joystick>().MovementApplied.AddListener(Movement);
        FindObjectOfType<PauseEvent>().gamePaused.AddListener(Pause);
        FindObjectsOfType<FigureElement>().ToList().ForEach(el => el.enabled = false);
    }

    public void BeginTutorial()
    {
        GameManager.SPEED = 0;
        StartCoroutine(ImageAlphaFade.FadeOut(joystickImage, 0f));
        stageCoroutine = StartCoroutine(ShowAnimatedGuide(1, rotations[rotationId]));
    }

    public void EndTutorial(string messageText)
    {
        blackTransition.gameObject.SetActive(blackTransition.enabled = true);

        finalText.text = messageText;

        GameManager.SPEED = 0;
        StartCoroutine(LoadScene("Menu", 1.2f));
    }

    public void SkipTutorial(string messageText)
    {
        tutorialSkipped?.Invoke(this, new AchievePropertyEventArgs(AchievementPropName.TutorialSkipped, true));
        EndTutorial(messageText);
    }

    public void Reload()
    {
        StartCoroutine(LoadScene("Tutorial", 0f));
    }


    IEnumerator LoadScene(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }


    public void Rotation(object sender, RotationEventArgs e)
    {
        if (e.Direction != rotationsSeq[rotationId])
            return;

        GestureSuccess(rotations[rotationId]);
        GameManager.SPEED = 20;
        rotationId++;

        if (rotationId == rotations.Count())
        {
            rotationComplete.Invoke();
            rotationComplete = null;
            rotationId = 0;

            FindObjectOfType<FigureMotion>().FigureRotated -= Rotation;

            var wall = FindObjectOfType<WallBuilder>();
            wall.holePosition = Vector2.one;
            wall.rotatingDirection = RotatingDirection.None;

            StartCoroutine(ImageAlphaFade.FadeIn(joystickImage, .23f, 1, 1));
            stageCoroutine = StartCoroutine(ShowAnimatedGuide(1.5f, "drag"));
            return;
        }
        else
        {
            stageCoroutine = StartCoroutine(ShowAnimatedGuide(1.5f, rotations[rotationId]));
            FindObjectOfType<WallBuilder>().rotatingDirection = rotationsSeq[rotationId];
        }
    }

    private void OnWallPassed(object sender, WallPassingEventArgs e)
    {
        GameManager.SPEED = 0;
        if (rotationComplete != null)
        {
            FindObjectsOfType<FigureElement>().ToList().ForEach(el => el.enabled = false);
            FindObjectOfType<FigureMotion>().FigureRotated += Rotation;
        }
        if (GameManager.BOOST && boostComplete != null)
        {
            boostComplete.Invoke();
        }
    }

    public void Movement()
    {
        GestureSuccess("drag");
        GameManager.SPEED = 20;
        var wall = FindObjectOfType<WallBuilder>();
        wall.holePosition = Vector2.one;
        wall.rotatingDirection = RotatingDirection.None;

        FindObjectOfType<Joystick>().MovementApplied.RemoveListener(Movement);
        movementComplete.Invoke();

        StartCoroutine(ImageAlphaFade.FadeIn(pausePlace, .23f, .22f, 1));
        stageCoroutine = StartCoroutine(ShowAnimatedGuide(1.5f, "tap"));
    }

    public void Pause()
    {
        paused = !paused;
        if (paused)
        {
            GestureSuccess("tap");
            StartCoroutine(ImageAlphaFade.FadeOut(pausePlace, 1));
            FindObjectOfType<PauseEvent>().gamePaused.RemoveListener(Pause);
        }
        else
        {
            pauseComplete.Invoke();

            var wall = FindObjectOfType<WallBuilder>();
            wall.wallDistance = 30;
            GameManager.SPEED = 0;
            stageCoroutine = StartCoroutine(ShowAnimatedGuide(.3f, "hold"));
        }
    }

    public void Boost()
    {
        if (boostComplete == null)
            return;
        GestureSuccess("hold");

        stageCoroutine = null;
        boostComplete = null;

        tutorialCompleted?.Invoke(this, new AchievePropertyEventArgs(AchievementPropName.TutorialCompleted, true));
        EndTutorial("TUTORIAL COMPLETED");
    }

    void GestureSuccess(string gestureName)
    {
        Image currImg = gestures
            .Find(g => g.sprite.name.Contains(gestureName));
        currImg
            .GetComponent<Animator>()
            .enabled = false;
        currImg.color = Color.green;

        StartCoroutine(ImageAlphaFade.FadeOut(currImg, .3f));
        StopCoroutine(stageCoroutine);
    }

    public IEnumerator ShowAnimatedGuide(float delay, string guideName)
    {
        yield return new WaitForSeconds(delay);

        Image img = gestures
            .Find(g => g.sprite.name.Contains(guideName));
        img.enabled = true;
        img.GetComponent<Animator>().enabled = true;
    }
}
