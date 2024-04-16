using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyboardButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    [SerializeField] private Color interactableColor;
    [SerializeField] private Color unInteractableColor;
    
    [SerializeField] private KeyboardButtonType buttonType;
    [SerializeField] private GameObject selected;
    [SerializeField] private Image background;
    [SerializeField] private GameObject icon;
    
    private bool _isClicked;
    private bool _interactable;

    public event Action<KeyboardButtonType> onPointerDown;
    public event Action<KeyboardButtonType> onPointerUp;

    public KeyboardButtonType KeyboardButtonType => buttonType;
    
    private bool IsClicked
    {
        get => _isClicked;
        set
        {
            _isClicked = value;
            
            if(selected)
                selected.SetActive(_isClicked);
        }
    }

    public void SetInteractable(bool state)
    {
        _interactable = state;
    }
    
    public void SetActive(bool state)
    {
        _interactable = state;
        background.color = _interactable ? interactableColor : unInteractableColor;
        icon.SetActive(state);
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if(!_interactable)
            return;
        
        if(IsClicked)
            return;
        
        IsClicked = true;
        onPointerDown?.Invoke(buttonType);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(!IsClicked)
            return;
        
        IsClicked = false;
        onPointerUp?.Invoke(buttonType);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!IsClicked)
            return;
        
        IsClicked = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!_interactable)
            return;
        
        if (!Input.GetMouseButton(0))
            return;
        
        if(IsClicked)
            return;

        if(eventData.pointerPress == null || !eventData.pointerPress.TryGetComponent<KeyboardButton>(out var keyboardButton))
            return;
        
        eventData.pointerPress = gameObject;

        IsClicked = true;
    }
}

public enum KeyboardButtonType
{
    None,
    Q,
    W,
    E,
    R,
    T,
    Y,
    U,
    I,
    O,
    P,
    A,
    S,
    D,
    F,
    G,
    H,
    J,
    K,
    L,
    Z,
    X,
    C,
    V,
    B,
    N,
    M,
    Backspace
}