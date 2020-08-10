using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Outro : MonoBehaviour
{
    public float loadMenuDelay;
    public float outroTime;
    public Image outroImage;
    public GameObject wallis;
    public GameObject specialThanks;

    AudioManager audioManager;

    private bool showCaptions = false;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        var gameOverSnd = audioManager.Play("GameOver");
        StartCoroutine(AudioFade.FadeOut(gameOverSnd, loadMenuDelay, Mathf.Lerp));

        if (SceneManager.GetActiveScene().name == "Game" &&
            GameManager.LEVEL == 10 && 
            PlayerPrefs.GetInt("CaptionsShowed", 0) == 0
            )
        {
            StartCoroutine(OutroCoroutine());
            showCaptions = true;
        }

        var loadingTime = showCaptions ? outroTime : loadMenuDelay;
        FindObjectOfType<LevelLoader>().LoadLevel("Menu", loadingTime);

        //if (Random.Range(0, 101) > 75 && !showCaptions)
        //    AdManager.Instance.InterstitialShow(loadingTime);
        if (!showCaptions)
            AdManager.Instance.InterstitialShow(loadingTime);
    }

    IEnumerator OutroCoroutine()
    {
        PlayerPrefs.SetInt("CaptionsShowed", 1);
        StartCoroutine(AudioFade.FadeIn(audioManager.GetSound("Outro"), 2, Mathf.Lerp, 0.5f, audioManager.userVolume));
        StartCoroutine(AudioFade.FadeOut(audioManager.GetSound("Outro"), 2.5f, Mathf.Lerp, outroTime - 3));
        StartCoroutine(ImageAlphaFade.FadeIn(outroImage, 0.5f, 1, outroTime - 1f));

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(TextMeshColorFade.FadeIn(wallis.GetComponent<TextMesh>(), 2));
        StartCoroutine(TextMeshColorFade.FadeIn(specialThanks.GetComponent<TextMesh>(), 1));

        StartCoroutine(TextMeshColorFade.FadeOut(wallis.GetComponent<TextMesh>(), 1, 9));

        while (true)
        {
            wallis.transform.position += 15 * wallis.transform.TransformDirection(Vector3.forward) * Time.deltaTime;
            specialThanks.transform.position += 3 * specialThanks.transform.TransformDirection(Vector3.up) * Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
