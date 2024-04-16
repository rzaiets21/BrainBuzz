using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Keyboard : MonoBehaviour
{
    [SerializeField] private LettersArea lettersArea;
    [SerializeField] private KeyboardButton[] keyboardButtons;

    private Dictionary<int, List<KeyboardButton>> powerupsLines;
    
    public event Action<KeyboardButtonType> OnKeyboardButtonUp;
    public event Action<KeyboardButtonType> OnKeyboardButtonDown;

    private void Awake()
    {
        powerupsLines = new Dictionary<int, List<KeyboardButton>>();
    }

    private void OnEnable()
    {
        foreach (var keyboardButton in keyboardButtons)
        {
            keyboardButton.onPointerUp += OnPointerUp;
            keyboardButton.onPointerDown += OnPointerDown;
        }

        lettersArea.OnLineSelected += OnLineSelected;
    }
    
    private void OnDisable()
    {
        foreach (var keyboardButton in keyboardButtons)
        {
            keyboardButton.onPointerUp -= OnPointerUp;
            keyboardButton.onPointerDown -= OnPointerDown;
        }
        
        lettersArea.OnLineSelected -= OnLineSelected;
    }

    public void ClearFilters()
    {
        powerupsLines.Clear();
    }
    
    public void AddFilter(int lineIndex, List<char> chars)
    {
        var buttons = new List<KeyboardButton>();
        foreach (var c in chars)
        {
            var button = keyboardButtons.FirstOrDefault(x => x.KeyboardButtonType.ToString() == c.ToString());
            
            if(button == null)
                continue;
            
            buttons.Add(button);
        }
        
        powerupsLines.Add(lineIndex, buttons);
    }

    public void SetInteractable(bool state)
    {
        foreach (var keyboardButton in keyboardButtons)
        {
            keyboardButton.SetInteractable(state);
        }
    }
    
    private void OnPointerUp(KeyboardButtonType keyboardButtonType)
    {
        OnKeyboardButtonUp?.Invoke(keyboardButtonType);
    }
    
    private void OnPointerDown(KeyboardButtonType keyboardButtonType)
    {
        OnKeyboardButtonDown?.Invoke(keyboardButtonType);
    }

    private void OnLineSelected(LettersLine line)
    {
        var lineIndex = line.LineIndex;
        if (!powerupsLines.TryGetValue(lineIndex, out var buttons))
        {
            foreach (var button in keyboardButtons)
            {
                button.SetActive(true);
            }
            return;
        }

        foreach (var button in keyboardButtons)
        {
            if(button.KeyboardButtonType == KeyboardButtonType.Backspace)
                continue;
            
            if(buttons.Contains(button))
                continue;
            
            button.SetActive(false);
        }

        foreach (var button in buttons)
        {
            button.SetActive(true);
        }
    }
}