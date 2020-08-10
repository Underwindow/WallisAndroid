using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class ImageAlphaFade
{
    public static IEnumerator FadeOut(Image image, float fadingTime, float toAlpha = 0, float delay = 0)
    {
        yield return new WaitForSeconds(delay);

        Color startColor = image.color;
        Color endColor = startColor * new Color(1, 1, 1, toAlpha);
        float frameCount = fadingTime / Time.deltaTime;
        float frames = 0;

        while (frameCount >= frames)
        {
            var t = frames++ / frameCount;
            image.color = Color.Lerp(startColor, endColor, t);

            yield return null;
        }

        image.color = endColor;
    }

    public static IEnumerator FadeIn(Image image, float fadingTime, float toAlpha = 1, float delay = 0)
    {
        yield return new WaitForSeconds(delay);

        Color endColor = new Color(image.color.r, image.color.g, image.color.b, toAlpha);
        Color startColor = image.color;
        float frameCount = fadingTime / .017f;
        float frames = 0;

        while (frameCount >= frames)
        {
            var t = frames++ / frameCount;
            image.color = Color.Lerp(startColor, endColor, t);

            yield return null;
        }

        image.color = endColor;
    }
}