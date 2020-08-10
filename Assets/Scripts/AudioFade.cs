using UnityEngine;
using System.Collections;
using System;

public class AudioFade
{
    public static IEnumerator FadeOut(Sound sound, float fadingTime, Func<float, float, float, float> Interpolate, float delay = 0)
    {
        yield return new WaitForSeconds(delay);

        float startVolume = sound.source.volume;
        float frameCount = fadingTime / Time.deltaTime;
        float frames = 0;

        while ( frameCount >= frames )
        {
            var t = frames++ / frameCount;
            sound.source.volume = Interpolate(startVolume, 0, t);

            yield return null;
        }

        sound.source.volume = 0;
        sound.source.Pause();
    }
    public static IEnumerator FadeIn(Sound sound, float fadingTime, Func<float, float, float, float> Interpolate, float delay = 0, float userVolume = 1)
    {
        yield return new WaitForSeconds(delay);

        sound.source.Play();
        sound.source.volume = 0;

        float resultVolume = sound.defaultVolume * userVolume;
        float frameCount = fadingTime / .017f;
        float frames = 0;

        while (frameCount >= frames)
        {
            var t = frames++ / frameCount;
            sound.source.volume = Interpolate(0, resultVolume, t);
            yield return null;
        }

        sound.source.volume = resultVolume;
    }
}