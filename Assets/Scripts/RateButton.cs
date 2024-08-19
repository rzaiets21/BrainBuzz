using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RateButton : MonoBehaviour
{
    [SerializeField] private Graphic[] graphics;
    [SerializeField] private Color interactableColor;
    [SerializeField] private Color unInteractableColor;
    
    private Button _button;

    public Action onClick;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnButtonClick);
    }
    
    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnButtonClick);
    }

    public void SetInteractable(bool state, bool changeColorState = true)
    {
        _button.interactable = state;
        
        if (changeColorState)
            foreach (var graphic in graphics)
            {
                graphic.color = state ? interactableColor : unInteractableColor;
            }
    }

    private void OnButtonClick()
    {
        onClick?.Invoke();
    }
}
