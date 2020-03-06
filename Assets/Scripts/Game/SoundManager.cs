using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;
    AudioSource musicAudioSource;
    List<AudioSource> sfx = new List<AudioSource>();

    [SerializeField]
    Sound[] sounds;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        musicAudioSource = GetComponent<AudioSource>();
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicAudioSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        }
        
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            _go.transform.parent = transform;
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
            sfx.Add(_go.GetComponent<AudioSource>());
        }
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1.0f));
        }
        
    }

    public void Play(string soundName)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == soundName)
            {
                sounds[i].Play();
                return;
            }
        }
    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        musicAudioSource.volume = value;
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        foreach (AudioSource source in sfx)
        {
            source.volume = value;
        }
    }
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public AudioSource source;
    float volume = 1;

    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;

    }



    public void Play()
    {
        source.Play();
    }
}