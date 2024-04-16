using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HapticFeedbacks = CandyCoded.HapticFeedback.HapticFeedback;

public class Test : MonoBehaviour
{
    [SerializeField] private Transform point1;
    [SerializeField] private Transform point2;
    [SerializeField] private Transform objectToMove;
    [SerializeField] private float duration;

    [SerializeField] private Animator revealLetterAnimator;
    
    public void Start()
    {
        var animation = revealLetterAnimator.runtimeAnimatorController.animationClips.First(x => x.name == "RevealTileAnimation");
        var speed = animation.length / duration;
        revealLetterAnimator.speed = speed;
    }

    [ContextMenu("StartMove")]
    public void StartMove()
    {
        var animation = revealLetterAnimator.runtimeAnimatorController.animationClips.First(x => x.name == "RevealTileAnimation");
        var speed = animation.length / duration;
        revealLetterAnimator.speed = speed;
        
        Debug.LogError(animation.length);
        
        revealLetterAnimator.SetTrigger("Reveal");
        StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        var movingIsComplete = false;
        var pos1 = point1.position;
        var pos2 = point2.position;

        var time = 0f;
        while (!movingIsComplete)
        {
            time += Time.deltaTime;
            var t = time / duration;
            var position = Vector3.Lerp(pos1, pos2, t);
            objectToMove.position = position;

            yield return null;

            movingIsComplete = t >= 1;
        }
    }
}
