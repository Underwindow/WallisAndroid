using UnityEngine;
using System;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    [HideInInspector]
    public List<Sound> playingSounds;

    public float defaultUserVolume;

    [HideInInspector]
    public float userVolume;
    // Start is called before the first frame update
    void Awake()
    {
        userVolume = PlayerPrefs.GetFloat("Volume", defaultUserVolume);
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.defaultVolume * userVolume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
    }

    public Sound Play (string name)
    {
        var s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
        if (!playingSounds.Contains(s))
            playingSounds.Add(s);

        return s;
    }

    public Sound GetSound(string name)
    {
        return Array.Find(sounds, sound => sound.name == name);
    }
}
