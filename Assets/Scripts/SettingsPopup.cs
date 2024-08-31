using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : Popup
{
    [SerializeField] private AudioController audioController;

    [SerializeField] private AudioClip toggleClick;
    
    [SerializeField] private Button blocker;
    [SerializeField] private Button closeButton;
    [SerializeField] private Toggle soundsToggle;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle hapticsToggle;
    
    [SerializeField] private Button privacyButton;
    [SerializeField] private Button termsButton;

    private bool _playSound;
    
    private void Start()
    {
        soundsToggle.isOn = Settings.SoundsIsOn;
        musicToggle.isOn = Settings.MusicIsOn;
        hapticsToggle.isOn = Settings.HapticsIsOn;

        _playSound = true;
    }

    private void OnEnable()
    {
        blocker.onClick.AddListener(Hide);
        closeButton.onClick.AddListener(Hide);
        
        soundsToggle.onValueChanged.AddListener(SetMuteSounds);
        musicToggle.onValueChanged.AddListener(SetMuteMusic);
        hapticsToggle.onValueChanged.AddListener(EnableHaptics);
        
        privacyButton.onClick.AddListener(OnClickPrivacyButton);
        termsButton.onClick.AddListener(OnClickTermsButton);
    }
    
    private void OnDisable()
    {
        blocker.onClick.RemoveListener(Hide);
        closeButton.onClick.RemoveListener(Hide);
        
        soundsToggle.onValueChanged.RemoveListener(SetMuteSounds);
        musicToggle.onValueChanged.RemoveListener(SetMuteMusic);
        hapticsToggle.onValueChanged.RemoveListener(EnableHaptics);
        
        privacyButton.onClick.RemoveListener(OnClickPrivacyButton);
        termsButton.onClick.RemoveListener(OnClickTermsButton);
    }

    private void OnClickPrivacyButton()
    {
        Application.OpenURL("https://brainbuzz.apppage.net/privacy-policy");
    }

    private void OnClickTermsButton()
    {
        Application.OpenURL("https://brainbuzz.apppage.net/terms");
    }
    
    private void SetMuteSounds(bool state)
    {
        audioController.SetMute(SoundType.Sounds, !state);
        if(_playSound)
            audioController.Play(SoundType.Sounds, toggleClick);
        Settings.SoundsIsOn = state;
    }

    private void SetMuteMusic(bool state)
    {
        audioController.SetMute(SoundType.Music, !state);
        if(_playSound)
            audioController.Play(SoundType.Sounds, toggleClick);
        Settings.MusicIsOn = state;
    }

    private void EnableHaptics(bool state)
    {
        if(_playSound)
            audioController.Play(SoundType.Sounds, toggleClick);
        Settings.HapticsIsOn = state;
    }
}
