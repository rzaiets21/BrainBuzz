using System;
using UnityEngine;

public class ParticleSizeFitter : MonoBehaviour
{
    [SerializeField] private bool x;
    [SerializeField] private bool y;
    
    private void Start()
    {
        var parent = transform.parent as RectTransform;
        var size = parent.sizeDelta;
        
        var letterParticleSystem = GetComponent<ParticleSystem>();
        var shape = letterParticleSystem.shape;
        shape.scale = new Vector3(x ? size.x : 1, y ? size.y : 1, 1);
    }
}