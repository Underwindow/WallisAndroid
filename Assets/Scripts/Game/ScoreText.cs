using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreText : MonoBehaviour
{
    public Text scoreText;
    public Color bonusColor;
    public float bonusTime;
    public int bonusFontSize;

    private bool isNewBonus = false;
    private int prevBonus = 1;
    private bool isReady = true;

    private Color defaultColor;
    private int defaultFontSize;

    private void Start()
    {
        defaultColor = scoreText.color;
        defaultFontSize = scoreText.fontSize;
    }


    // Update is called once per frame
    void Update()
    {
        isNewBonus = GameManager.BOOST && prevBonus < GameManager.SCORE_BONUS;

        prevBonus = GameManager.SCORE_BONUS;

        var multiplier = "";
        if (isNewBonus)
            ScoreTextEffect(bonusColor);
        if (GameManager.BOOST)
            multiplier = "x" + (GameManager.SCORE_BONUS + 1).ToString();

        scoreText.color = Color.Lerp(scoreText.color, defaultColor, .15f);
        scoreText.fontSize = Mathf.Clamp(scoreText.fontSize - 2, defaultFontSize, bonusFontSize);

        scoreText.text = $"{GameManager.SCORE}\n{multiplier}";
    }

    public void ScoreTextEffect(Color color)
    {
        scoreText.color = color;
        scoreText.fontSize = bonusFontSize;

        if (isReady)
        {
            isReady = false;
            StartCoroutine(SetDefaultsAfter(bonusTime));
        }
    }

    // Restart game after time
    IEnumerator SetDefaultsAfter(float time)
    {
        yield return new WaitForSeconds(time);

        scoreText.color = defaultColor;
        scoreText.fontSize = defaultFontSize;
        isReady = true;
    }
}
