using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFX
{
    public abstract class VFXBase : MonoBehaviour
    {
        [SerializeField] protected RectTransform rectTransform;
        
        public bool IsPlaying { get; protected set; }
        
        protected float scaleMultiplier;
        
        public void Init(float scaleMultiplier)
        {
            this.scaleMultiplier = scaleMultiplier;

            rectTransform.localScale = Vector3.one * scaleMultiplier;
        }

        public void SetPosition(Vector2 position)
        {
            rectTransform.anchoredPosition = position;
        }
        
        public virtual void OnInited(){ }

        [ContextMenu("Show Particle")]
        public void Show()
        {
            ShowParticle();
        }
        
        public abstract void ShowParticle(Action onComplete = null);
    }
}
