using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace VFX
{
    public sealed class ExplosionParticle : VFXBase
    {
        [SerializeField] private float glowFadeDuration;
        [SerializeField] private float glowFadeOutDuration;
        [SerializeField] private float glowScaleDuration;
        [SerializeField] private Image glow;
        
        public override void ShowParticle(Action onComplete = null)
        {
            IsPlaying = true;
            
            var glowColor = glow.color;
            glowColor.a = 0;
            glow.color = glowColor;
                
            glow.rectTransform.localScale = Vector3.one;
                
            glow.rectTransform.DOScale(1.25f, glowScaleDuration);
            glow.DOFade(0.65f, glowFadeDuration).OnComplete(() =>
            {
                glow.DOFade(0f, glowFadeOutDuration).OnComplete(() =>
                {
                    DOVirtual.DelayedCall(1f, () =>
                    {
                        IsPlaying = false;
                        gameObject.SetActive(false);
                    });
                    onComplete?.Invoke();
                });
            });
        }
    }
}