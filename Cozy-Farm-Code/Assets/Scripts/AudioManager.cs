using System;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;

    public static AudioManager instance;

    void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
        
    }

     void Start()
    {
        Play("Background");
    }


    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }

    public void Mute(bool isMuted)
    {
        foreach (Sound s in sounds)
        {
            s.source.mute = isMuted;
        }
    }

    public void OnMuteButtonClicked()
    {
        bool isMuted = sounds[0].source.mute; // Check the current mute state of any sound
        Mute(!isMuted); // Toggle mute
    }


    
}
