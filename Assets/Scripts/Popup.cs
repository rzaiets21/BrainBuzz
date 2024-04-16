using DG.Tweening;
using UnityEngine;

public class Popup : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float showDuration;
    [SerializeField] private RectTransform container;
    
    private bool _inAnimation;
    
    public void Show()
    {
        if(_inAnimation)
            return;

        OnShown();

        _inAnimation = true;
        //canvasGroup.alpha = 1f;
        canvasGroup.gameObject.SetActive(true);
        canvasGroup.blocksRaycasts = true;
        container.DOScale(1f, showDuration).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            canvasGroup.interactable = true;
            _inAnimation = false;
        });
    }

    protected virtual void OnShown()
    {
        
    }

    protected void Hide()
    {
        Hide(false);
    }
    
    protected void Hide(bool immediate)
    {
        if(_inAnimation)
            return;

        canvasGroup.interactable = false;

        if (!immediate)
        {
            _inAnimation = true;
            container.DOScale(0f, showDuration).SetEase(Ease.InOutCubic).OnComplete(() =>
            {
                canvasGroup.gameObject.SetActive(false);
                canvasGroup.blocksRaycasts = false;
                _inAnimation = false;
            });
            
            return;
        }

        container.localScale = Vector3.zero;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.gameObject.SetActive(false);
    }
}