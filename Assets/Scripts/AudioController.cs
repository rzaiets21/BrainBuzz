using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Transform soundsRect;
    [SerializeField] private AudioSource soundAudioSourcePrefab;
    [SerializeField] private AudioSource musicAudioSource;

    private List<AudioSource> _soundsPool = new();

    private bool _soundsMuted;
    private bool _musicMuted;
    
    public static AudioController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(this.gameObject);
            return;
        }

        Instance = this;
        _soundsMuted = !Settings.SoundsIsOn;
        _musicMuted = !Settings.MusicIsOn;

        SetMute(SoundType.Sounds, _soundsMuted);
        SetMute(SoundType.Music, _musicMuted);
    }
    
    public void Play(SoundType soundType, AudioClip audioClip, bool loop = false)
    {
        switch (soundType)
        {
            case SoundType.Sounds:
                var soundSource = _soundsPool.FirstOrDefault(x => !x.isPlaying);
                if (soundSource == null)
                {
                    soundSource = Instantiate(soundAudioSourcePrefab, soundsRect);
                    _soundsPool.Add(soundSource);
                }
                soundSource.clip = audioClip;
                soundSource.loop = loop;
                soundSource.Play();
                break;
            case SoundType.Music:
                musicAudioSource.clip = audioClip;
                musicAudioSource.loop = loop;
                musicAudioSource.Play();
                break;
        }
    }

    public void SetMute(SoundType soundType, bool state)
    {
        Debug.LogError($"{soundType} - {state}");
        audioMixer.SetFloat($"{soundType.ToString()}Volume", state ? -80 : 0);
    }
}

public enum SoundType
{
    Sounds = 1,
    Music = 2
}
