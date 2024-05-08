using System;
using UnityEngine;

public class TileParticles : MonoBehaviour
{
    // private const string CompleteParticleTrigger = "CompleteParticle";
    // private const string CompleteParticleBlinkTrigger = "CompleteParticleBlink";
    // private const string CompleteRevealAnimationTrigger = "CompleteRevealAnimation";

    [SerializeField] private Animator animator;

    public bool IsPlaying { get; private set; }
    
    private Action _onAnimationComplete;

    public void ShowParticle(TileParticleType particleType, Action onAnimationComplete = null)
    {
        IsPlaying = true;
        
        animator.SetTrigger(particleType.ToString());

        _onAnimationComplete = onAnimationComplete;
    }

    public void OnAnimationComplete()
    {
        _onAnimationComplete?.Invoke();
        _onAnimationComplete = null;

        IsPlaying = false;
    }
}

public enum TileParticleType
{
    CompleteParticle,
    CompleteParticleBlink, 
    CompleteRevealAnimation,
    CompleteLevel
}

public enum VFXType
{
    CorrectAnswer,
    FirstCorrectAnswer, 
    Explosion
}
