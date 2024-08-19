using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RevealLetterHolder : LetterHolderBase
{
    private const string RevealAnimationTrigger = "Reveal";
    
    [SerializeField] private Animator animator;
    
    public void Init(float moveDuration)
    {
        var animation = animator.runtimeAnimatorController.animationClips.First(x => x.name == "RevealTileAnimation");
        var speed = animation.length / moveDuration;
        animator.speed = speed;
    }
    
    public void StartAnimation()
    {
        animator.SetTrigger(RevealAnimationTrigger);
    }
}
