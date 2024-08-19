using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LayoutGroupDeactivator : MonoBehaviour
{
    [SerializeField] private float delay;
    
    private void Start()
    {
        DOVirtual.DelayedCall(delay, DeactivateContentSizeFitter).SetLink(gameObject, LinkBehaviour.CompleteAndKillOnDisable);
        DOVirtual.DelayedCall(delay + 1, DeactivateLayoutGroup).SetLink(gameObject, LinkBehaviour.CompleteAndKillOnDisable);
    }

    private void DeactivateContentSizeFitter()
    {
        if (TryGetComponent<ContentSizeFitter>(out var contentSizeFitter))
        {
            contentSizeFitter.enabled = false;
        }
    }

    private void DeactivateLayoutGroup()
    {
        if (TryGetComponent<LayoutGroup>(out var layoutGroup))
        {
            layoutGroup.enabled = false;
        }
    }
}
