using System;
using Newtonsoft.Json;
using UnityEngine;

public static class Settings
{
    private static SystemData _systemData;
    private static bool _isLoaded;

    public static bool MusicIsOn
    {
        get
        {
            if (!_isLoaded)
            {
                LoadSettingsData();
            }

            return _systemData.MusicIsOn;
        }
        set
        {
            _systemData.MusicIsOn = value;
            SaveSettingsData();
        }
    }

    public static bool SoundsIsOn
    {
        get
        {
            if (!_isLoaded)
            {
                LoadSettingsData();
            }

            return _systemData.SoundsIsOn;
        }
        set
        {
            _systemData.SoundsIsOn = value;
            SaveSettingsData();
        }
    }

    public static bool HapticsIsOn
    {
        get
        {
            if (!_isLoaded)
            {
                LoadSettingsData();
            }

            return _systemData.HapticsIsOn;
        }
        set
        {
            _systemData.HapticsIsOn = value;
            SaveSettingsData();
        }
    }

    private static void LoadSettingsData()
    {
        if (!PlayerPrefs.HasKey("Settings"))
        {
            _systemData = new SystemData()
            {
                MusicIsOn = true,
                SoundsIsOn = true,
                HapticsIsOn = true
            };

            SaveSettingsData();
            _isLoaded = true;
            return;
        }
        
        var json = PlayerPrefs.GetString("Settings");
        _systemData = JsonConvert.DeserializeObject<SystemData>(json);
        _isLoaded = true;
    }
    
    private static void SaveSettingsData()
    {
        var json = JsonConvert.SerializeObject(_systemData);
        PlayerPrefs.SetString("Settings", json);
    }
    
    [Serializable]
    private class SystemData
    {
        public bool MusicIsOn;
        public bool SoundsIsOn;
        public bool HapticsIsOn;
    }
}