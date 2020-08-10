using System.Collections;
using UnityEngine;
public static class TextMeshColorFade
{
    public static IEnumerator FadeOut(TextMesh component, float fadingTime, float delay = 0, float toAlpha = 0)
    {
        yield return new WaitForSeconds(delay);

        Color startColor = component.color;
        Color endColor = startColor * new Color(1, 1, 1, toAlpha);
        float frameCount = fadingTime / Time.deltaTime;
        float frames = 0;

        while (frameCount >= frames)
        {
            var t = frames++ / frameCount;
            component.color = Color.Lerp(startColor, endColor, t);

            yield return null;
        }

        component.color = endColor;
    }

    public static IEnumerator FadeIn(TextMesh component, float fadingTime, float delay = 0, float toAlpha = 1)
    {
        yield return new WaitForSeconds(delay);

        Color startColor = component.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, toAlpha);
        float frameCount = fadingTime / .017f;
        float frames = 0;

        while (frameCount >= frames)
        {
            var t = frames++ / frameCount;
            component.color = Color.Lerp(startColor, endColor, t);

            yield return null;
        }

        component.color = endColor;
    }
}