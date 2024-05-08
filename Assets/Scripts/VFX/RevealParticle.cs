using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace VFX
{
    public sealed class RevealParticle : VFXBase
    {
        [SerializeField] private float fadeDelay;
        [SerializeField] private float endingDelay;
        [SerializeField] private float tileCanvasGroupFadingDuration;

        [SerializeField] private CanvasGroup tileCanvasGroup;
        [SerializeField] private GameObject landParticle;
        [SerializeField] private GameObject speckleParticle;
        [SerializeField] private GameObject validateTileParticle;
        
        [SerializeField] private float moveDuration = 1f;
        [SerializeField] private TextMeshProUGUI letterText;

        private Vector2 _targetPosition;

        public Action onGetPoint;
        
        public void SetLetter(char letter)
        {
            letterText.text = letter.ToString();
        }

        public void SetTarget(Vector2 position)
        {
            _targetPosition = position;
        }
        
        public override void ShowParticle(Action onComplete = null)
        {
            IsPlaying = true;
            
            gameObject.SetActive(true);
            tileCanvasGroup.alpha = 1f;
            
            rectTransform.localScale = Vector3.zero;
            rectTransform.DOAnchorPos(_targetPosition, moveDuration);
            rectTransform.DOScale(1.35f * scaleMultiplier, moveDuration).OnComplete(() =>
            {
                rectTransform.DOScale(1f * scaleMultiplier, moveDuration / 2f).OnComplete(() =>
                {
                    landParticle.gameObject.SetActive(true);
                    speckleParticle.gameObject.SetActive(true);
                    validateTileParticle.gameObject.SetActive(true);

                    onGetPoint?.Invoke();
                    
                    DOVirtual.DelayedCall(fadeDelay, () =>
                    {
                        tileCanvasGroup.DOFade(0f, tileCanvasGroupFadingDuration + endingDelay).OnComplete(() =>
                        {
                            DOVirtual.DelayedCall(1f, () =>
                            {
                                IsPlaying = false;
                                onGetPoint = null;
                                gameObject.SetActive(false);
                            });
                            onComplete?.Invoke();
                        });
                    });
                });
            });
        }
    }
}