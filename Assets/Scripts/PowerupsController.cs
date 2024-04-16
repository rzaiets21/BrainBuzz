using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PowerupsController : MonoBehaviour
{
    [SerializeField] private PowerupPanel powerupPanel;
    [SerializeField] private PowerupInfo[] powerupsInfo;
    [SerializeField] private Image currentPowerupIcon;
    
    [SerializeField] private LettersArea lettersArea;
    [SerializeField] private Powerup[] powerups;

    [SerializeField]private PowerupType _currentPowerup;
    private PowerupType _lastPowerup;

    private Dictionary<PowerupType, int> _powerupLayers = new Dictionary<PowerupType, int>()
    {
        {PowerupType.Letter, 7},
        {PowerupType.Random, 6},
        {PowerupType.Keyboard, 7},
        {PowerupType.Line, 7},
    };

    public bool SelectedPowerup => _currentPowerup != PowerupType.None;
    
    private int _currentLayer;
    private LetterHolderBase _selectedLetterHolder;
    private LetterHolderBase _lastSelectedLetterHolder;
    
    public event Action<bool> onPowerupSelected;
    
    private void OnEnable()
    {
        foreach (var powerup in powerups)
        {
            powerup.onSelect += OnPowerupSelected;
            powerup.onDeselect += OnPowerupDeselected;
            powerup.IsInteract += IsInteract;
        }

        lettersArea.onLetterHolderClick += OnLetterHolderSelected;
    }

    private void OnDisable()
    {
        foreach (var powerup in powerups)
        {
            powerup.onSelect -= OnPowerupSelected;
            powerup.onDeselect -= OnPowerupDeselected;
            powerup.IsInteract -= IsInteract;
        }
        
        lettersArea.onLetterHolderClick -= OnLetterHolderSelected;
    }
    
    private void OnLetterHolderSelected(LetterHolderBase letterHolder)
    {
        if(_currentPowerup == PowerupType.None)
            return;

        UsePowerup(letterHolder);
    }

    private bool IsInteract() => lettersArea.Interact;

    public void DeselectCurrent()
    {
        if (_currentPowerup == PowerupType.None)
        {
            return;
        }
        
        var currentPowerup = powerups.First(x => x.PowerupType == _currentPowerup);
        currentPowerup.Deselect(false);
    }
    
    private void DeselectLastLetterHolder()
    {
        if (_lastSelectedLetterHolder != null)
        {
            switch (_lastPowerup)
            {
                case PowerupType.Random:
                    break;
                default:
                    switch (_lastPowerup)
                    {
                        case PowerupType.Letter:
                            _lastSelectedLetterHolder.RestoreColor();
                            break;
                        case PowerupType.Line:
                        case PowerupType.Keyboard:
                            var line = lettersArea.GetLine(_lastSelectedLetterHolder.LineIndex);
                            line.RestoreColor();
                            break;
                    }

                    break;
            }

            _lastSelectedLetterHolder = null;
        }
    }

    private void UsePowerup(LetterHolderBase letterHolder)
    {
        var powerup = _currentPowerup;
        
        var currentPowerup = powerups.First(x => x.PowerupType == _currentPowerup);
        
        if (!TryUsePowerup(powerup, letterHolder))
        {
            currentPowerup.Deselect(false);
            Debug.LogError("Powerup is not used");
            return;
        }
                
        if(PlayerInventory.Instance.HasPowerup(_currentPowerup))
            PlayerInventory.Instance.Consume(_currentPowerup);
        else
            PlayerInventory.Instance.Consume(currentPowerup.Price);
        
                
        currentPowerup.Deselect(true);
        _currentPowerup = PowerupType.None;
        _selectedLetterHolder = null;
    }
    
    private bool TryUsePowerup(PowerupType powerupType, LetterHolderBase letterHolder)
    {
        return lettersArea.TryUsePowerup(powerupType, letterHolder);
    }
    
    private void OnPowerupSelected(Powerup powerup)
    {
        if(_currentPowerup != PowerupType.None)
        {
            var currentPowerup = powerups.First(x => x.PowerupType == _currentPowerup);
            currentPowerup.Deselect(false);
        }
        
        var powerupType = powerup.PowerupType;
        
        _currentPowerup = powerupType;
        _lastPowerup = _currentPowerup;
        _currentLayer = _powerupLayers[_currentPowerup];

        var powerupInfo = powerupsInfo.First(x => x.PowerupType == powerupType);
        
        powerupPanel.Show(powerupInfo);
        
        onPowerupSelected?.Invoke(true);
    }
    
    private void OnPowerupDeselected()
    {
        if(_currentPowerup == PowerupType.None)
            return;

        _currentPowerup = PowerupType.None;
        
        onPowerupSelected?.Invoke(false);
        //UsePowerup();
    }
}

[Serializable]
public class PowerupInfo
{
    public PowerupType PowerupType;
    public Sprite Icon;
    public string Description;
}