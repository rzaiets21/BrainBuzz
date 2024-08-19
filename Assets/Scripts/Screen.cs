using System.Collections.Generic;
using UnityEngine;

public class Screen : ScreenBase
{

}

public abstract class ScreenBase : MonoBehaviour
{
    [SerializeField] private bool deactivate;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Canvas parentCanvas;

    private RectTransform _rect;
    
    public RectTransform rect
    {
        get
        {
            if(_rect == null)
                _rect = GetComponent<RectTransform>();

            return _rect;
        }
    }

    public void SetInteractable(bool state)
    {
        canvasGroup.interactable = state;
    }
    
    public void Show(bool interactable = true)
    {
        if (!deactivate && parentCanvas != null)
            parentCanvas.enabled = true;
        canvasGroup.SetActive(true, deactivate);
        canvasGroup.interactable = interactable;
        OnShown();
    }

    protected virtual void OnShown() { }

    public void Hide()
    {
        if (!deactivate && parentCanvas != null)
            parentCanvas.enabled = false;
        canvasGroup.SetActive(false, deactivate);
        OnHidden();
    }
    
    protected virtual void OnHidden() { }
}