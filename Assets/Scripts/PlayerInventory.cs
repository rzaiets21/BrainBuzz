using System;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class PlayerInventory
{
    [JsonIgnore] private const string PlayerInventoryKey = "Player_Inventory";
    
    public int _coins;
    
    public PowerupProductInfo[] _powerups = new PowerupProductInfo[4];

    [JsonIgnore] private static PlayerInventory _instance;

    [JsonIgnore]
    public static PlayerInventory Instance
    {
        get
        {
            if (_instance == null)
            {
                if (PlayerPrefs.HasKey(PlayerInventoryKey))
                {
                    var json = PlayerPrefs.GetString(PlayerInventoryKey);
                    _instance = JsonConvert.DeserializeObject<PlayerInventory>(json);
                }
                else
                {
                    _instance = Default();
                }
            }

            return _instance;
        }
    }

    public event Action<int> OnCoinsValueChanged;
    public event Action<PowerupType, int> OnPowerupValueChanged;  
    
    private static PlayerInventory Default()
    {
        return new PlayerInventory()
        {
            _coins = 100,
            _powerups = new PowerupProductInfo[4]
            {
                new PowerupProductInfo
                {
                    PowerupType = PowerupType.Letter,
                    count = 2
                },
                new PowerupProductInfo
                {
                    PowerupType = PowerupType.Random,
                    count = 2
                },
                new PowerupProductInfo
                {
                    PowerupType = PowerupType.Keyboard,
                    count = 2
                },
                new PowerupProductInfo
                {
                    PowerupType = PowerupType.Line,
                    count = 2
                }
            }
        };
    }
    
    public void Add(int value, bool notify = true)
    {
        _coins += value;
        if(notify)
            OnCoinsValueChanged?.Invoke(_coins);

        var json = JsonConvert.SerializeObject(_instance);
        PlayerPrefs.SetString(PlayerInventoryKey, json);
    }
    
    public void Add(PowerupType powerupType, int value, bool notify = true)
    {
        Debug.Log(powerupType);
        Debug.Log(_powerups.Length);
        var powerupInfo = _powerups.First(x => x.PowerupType == powerupType);
        powerupInfo.count += value;
        
        if(notify)
            OnPowerupValueChanged?.Invoke(powerupInfo.PowerupType, powerupInfo.count);
        
        var json = JsonConvert.SerializeObject(_instance);
        PlayerPrefs.SetString(PlayerInventoryKey, json);
    }

    public bool HasEnoughCoins(int value)
    {
        return _coins >= value;
    }

    public bool HasPowerup(PowerupType powerupType)
    {
        var powerupInfo = _powerups.First(x => x.PowerupType == powerupType);
        return powerupInfo.count > 0;
    }

    public void Consume(int value)
    {
        _coins = Mathf.Clamp(_coins - value, 0, _coins + value);
        OnCoinsValueChanged?.Invoke(_coins);
        var json = JsonConvert.SerializeObject(_instance);
        PlayerPrefs.SetString(PlayerInventoryKey, json);
    }

    public void Consume(PowerupType powerupType)
    {
        var powerupInfo = _powerups.First(x => x.PowerupType == powerupType);
        powerupInfo.count--;
        
        OnPowerupValueChanged?.Invoke(powerupInfo.PowerupType, powerupInfo.count);
        var json = JsonConvert.SerializeObject(_instance);
        PlayerPrefs.SetString(PlayerInventoryKey, json);
    }

    public void Update()
    {
        OnCoinsValueChanged?.Invoke(_coins);
        foreach (var powerup in _powerups)
        {
            OnPowerupValueChanged?.Invoke(powerup.PowerupType, powerup.count);
        }
    }
}

