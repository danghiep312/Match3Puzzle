using System;
using UnityEngine;

public enum AudioType
{
    Sound,
    Music
}

[System.Serializable]
public class Audio
{
    public string name;
    public AudioType audioType;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1;
    [Range(.1f, 3f)] public float pitch = 1;
    /// <summary>
    /// If true, the audio is music, if false, the audio is sound
    /// </summary>
    public bool loop;
    [HideInInspector] public AudioSource source;
}

public class AudioManager : Singleton<AudioManager>
{
    public Audio[] audios;

    public bool soundOn;
    public bool musicOn;

    public override void Awake()
    {
        base.Awake();
        foreach (Audio audio in audios)
        {
            audio.source = gameObject.AddComponent<AudioSource>();
            audio.source.clip = audio.clip;
            audio.source.volume = audio.volume;
            audio.source.pitch = audio.pitch;
            audio.source.loop = audio.loop;
        }
        
        soundOn = PlayerPrefs.GetInt("Sound", 1) == 1;
        musicOn = PlayerPrefs.GetInt("Music", 1) == 1;

    }
    
    public void Play(string name)
    {
        Audio audio = Array.Find(audios, sound => sound.name == name);
        if (audio == null)
        {
            Common.LogWarning(this, "Can't find audio with name: " + name);
        }
        
        
        if (audio.audioType == AudioType.Sound && !soundOn) return;
        if (audio.audioType == AudioType.Music && !musicOn) return;
        if (audio.audioType == AudioType.Sound) audio.source.PlayOneShot(audio.clip);
        else audio.source.Play();
    }

    public void Stop(string name)
    {
        Audio audio = Array.Find(audios, sound => sound.name == name);
        if (audio == null)
        {
            Common.LogWarning(this, "Can't find audio with name: " + name);
        }
        
        audio.source.Stop();
    }
    
    
    public void ToggleSound()
    {
        soundOn = !soundOn;
        PlayerPrefs.SetInt("Sound", soundOn ? 1 : 0);
    }
    
    public void ToggleMusic()
    {
        musicOn = !musicOn;
        if (!musicOn)
            Stop("Bg");
        else
            Play("Bg");
        PlayerPrefs.SetInt("Music", musicOn ? 1 : 0);
    }
    
    public void ChangePitch(float pitch)
    {
        foreach (Audio audio in audios)
        {
            audio.source.pitch = pitch;
        }
    }
}
