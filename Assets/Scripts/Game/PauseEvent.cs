using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PauseEvent : MonoBehaviour
{
    public UnityEvent gamePaused;
    public UnityEvent gameResumed;

    bool paused = false;

    Image fadeImage;
    Color startColor;
    public Color pauseColor;
    List<Vector3> velocityList;


    void Start()
    {
        fadeImage = GetComponent<Image>();
        startColor = fadeImage.color;
        if (gamePaused == null)
            gamePaused = new UnityEvent();
        if (gameResumed == null)
            gameResumed = new UnityEvent();
    }

    void Update()
    {
        if (paused)
            fadeImage.color = Color.Lerp(fadeImage.color, pauseColor, .2f);
    }

    public void Pause()
    {
        if (paused)
        {
            var rbs = FindObjectsOfType<Rigidbody>();
            for (int i = 0; i < velocityList.Count; i++)
            {
                rbs[i].velocity = velocityList[i];
                rbs[i].WakeUp();
            }

            gameResumed.Invoke();
            fadeImage.color = startColor;
        }
        else
        {
            velocityList = new List<Vector3>();

            var rbs = FindObjectsOfType<Rigidbody>();
            foreach (var rb in rbs)
            {
                velocityList.Add(rb.velocity);
                rb.Sleep();
            }

            gamePaused.Invoke();
            //StartCoroutine(ImageAlphaFade.FadeIn(fadeImage, .5f, .5f));
        }
        paused = !paused;
    }
}
