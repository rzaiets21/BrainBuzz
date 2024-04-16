using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using DG.Tweening;
using UnityEngine;

public class BeatingItem : MonoBehaviour
{
    [SerializeField] private float startDelay;
    [SerializeField] private float beatingDuration;
    [SerializeField] private RectTransform rectTransform;

    [SerializeField] private ShinyEffectForUGUI _shinyEffectForUGUI;
    
    private Tween _beatingTween;
    private Tween _shinyTween;
    
    private void OnEnable()
    {
        StartBeating();
    }

    private void OnDisable()
    {
        StopBeating();
    }

    public void StartBeating()
    {
        _beatingTween = DOVirtual.DelayedCall(startDelay / 2f, () =>
        {
            if (_shinyEffectForUGUI != null)
            {
                _shinyTween = DOVirtual.Float(0, 1, beatingDuration * 8, x => _shinyEffectForUGUI.location = x)
                    .OnComplete(() => _shinyEffectForUGUI.location = 0).SetLink(gameObject, LinkBehaviour.CompleteAndKillOnDisable);
            }
            
            rectTransform.DOScale(1.2f, beatingDuration).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                rectTransform.DOScale(1.1f, beatingDuration).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutQuad);
            });
        }).OnComplete(BeatingTween).SetLink(gameObject, LinkBehaviour.CompleteAndKillOnDisable);
    }

    public void StopBeating()
    {
        if (_beatingTween != null)
        {
            if(_beatingTween.IsPlaying())
                _beatingTween.Kill(true);
            if(_beatingTween.IsActive())
                _beatingTween.Kill(true);
        }
        
        if (_shinyTween != null)
        {
            if(_shinyTween.IsPlaying())
                _shinyTween.Kill(true);
            if(_shinyTween.IsActive())
                _shinyTween.Kill(true);
        }
    }
    
    private void BeatingTween()
    {
        _beatingTween = DOVirtual.DelayedCall(startDelay, () =>
        {
            if (_shinyEffectForUGUI != null)
            {
                _shinyTween = DOVirtual.Float(0, 1, beatingDuration * 8, x => _shinyEffectForUGUI.location = x)
                    .OnComplete(() => _shinyEffectForUGUI.location = 0).SetLink(gameObject, LinkBehaviour.CompleteAndKillOnDisable);
            }
            
            rectTransform.DOScale(1.2f, beatingDuration).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                rectTransform.DOScale(1.1f, beatingDuration).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutQuad);
            });
        }).SetLoops(-1).SetLink(gameObject, LinkBehaviour.CompleteAndKillOnDisable);
    }
}
