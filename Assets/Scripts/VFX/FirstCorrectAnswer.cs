using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace VFX
{
    public class FirstCorrectAnswer : VFXBase
    {
        [SerializeField] private float delay;
        [SerializeField] private float brilhoDelay;
        
        [SerializeField] private float ballsDuration;
        [SerializeField] private float ballsRotateDuration;
        [SerializeField] private float glowFadeDuration;
        [SerializeField] private float glowFadeOutDuration;
        [SerializeField] private float glowScaleDuration;
        [SerializeField] private float ringFadeDuration;
        [SerializeField] private float ringFadeOutDuration;
        [SerializeField] private float ringScaleDuration;
        [SerializeField] private float brilhoFadeDuration;
        [SerializeField] private float brilhoFadeOutDuration;
        [SerializeField] private float brilhoScaleDuration;
        
        [SerializeField] private GameObject particle;
        
        [SerializeField] private Image glow;
        [SerializeField] private Image ring;
        [SerializeField] private Image brilho;
        
        [SerializeField] private RectTransform ballsHolder;
        [SerializeField] private Image[] balls;
        
        public override void ShowParticle(Action onComplete = null)
        {
            IsPlaying = true;
            
            particle.SetActive(false);
            
            for (int i = 0; i < balls.Length; i++)
            {
                var ball = balls[i];
                var ballRect = ball.rectTransform;
                ball.color = new Color(1, 1, 1, 0);
                ballRect.localScale = Vector3.zero;

                ball.DOFade(1f, ballsDuration);
                ballRect.DOScale(0.75f, ballsDuration).OnComplete(() =>
                {
                    ball.color = new Color(1, 1, 1, 0);
                    ballRect.localScale = Vector3.zero;
                });
            }
            
            ballsHolder.DOBlendableLocalRotateBy(Vector3.forward * 90f, ballsRotateDuration);
            
            DOVirtual.DelayedCall(brilhoDelay, () =>
            {
                brilho.rectTransform.DOScale(1.2f, brilhoScaleDuration);
                brilho.DOFade(1f, brilhoFadeDuration).OnComplete(() =>
                {
                    brilho.DOFade(0f, brilhoFadeOutDuration);
                });
            });

            var finishDelay = Mathf.Max(ringScaleDuration + ringFadeDuration + ringFadeOutDuration,
                glowScaleDuration + glowFadeDuration + glowFadeOutDuration) + delay;
            
            DOVirtual.DelayedCall(delay, () =>
            {
                particle.SetActive(true);
                
                var glowColor = glow.color;
                glowColor.a = 0;
                glow.color = glowColor;
                ring.color = new Color(1, 1, 1, 0);
                brilho.color = new Color(1, 1, 1, 0);
                
                ring.rectTransform.localScale = Vector3.one;
                glow.rectTransform.localScale = Vector3.one;
                brilho.rectTransform.localScale = Vector3.one;

                ring.rectTransform.DOScale(1.6f, ringScaleDuration);
                ring.DOFade(1f, ringFadeDuration).OnComplete(() =>
                {
                    ring.DOFade(0f, ringFadeOutDuration);
                });
                
                glow.rectTransform.DOScale(1.25f, glowScaleDuration);
                glow.DOFade(0.65f, glowFadeDuration).OnComplete(() =>
                {
                    glow.DOFade(0f, glowFadeOutDuration);
                });
            });
            
            DOVirtual.DelayedCall(finishDelay, () =>
            {
                DOVirtual.DelayedCall(1f, () =>
                {
                    IsPlaying = false;
                    gameObject.SetActive(false);
                });
                onComplete?.Invoke();
            });
        }
    }
}