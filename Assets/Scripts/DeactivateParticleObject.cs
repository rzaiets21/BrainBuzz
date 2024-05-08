using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DeactivateParticleObject : MonoBehaviour
{
    [SerializeField] private float offset;
    [SerializeField] private ParticleSystem particleSystem;

    private Tween _deactivateTween;
    
    private void OnEnable()
    {
        var main = particleSystem.main;
        
        _deactivateTween = DOVirtual.DelayedCall(main.duration + main.startLifetime.constantMax + offset, () =>
        {
            gameObject.SetActive(false);
        });
    }

    private void OnDisable()
    {
        if (_deactivateTween != null)
        {
            if(_deactivateTween.IsPlaying())
                _deactivateTween.Kill(true);
            if(_deactivateTween.IsActive())
                _deactivateTween.Kill(true);
        }
    }
}
