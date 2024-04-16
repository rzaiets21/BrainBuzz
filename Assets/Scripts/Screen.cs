using System.Collections.Generic;
using UnityEngine;

public class Screen : ScreenBase
{

}

public abstract class ScreenBase : MonoBehaviour
{
    [SerializeField] private bool deactivate;
    [SerializeField] private CanvasGroup canvasGroup;

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
        canvasGroup.SetActive(true, deactivate);
        canvasGroup.interactable = interactable;
        OnShown();
    }

    protected virtual void OnShown() { }

    public void Hide()
    {
        canvasGroup.SetActive(false, deactivate);
        OnHidden();
    }
    
    protected virtual void OnHidden() { }
}