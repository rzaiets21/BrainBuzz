using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using VFX;

public class RevealLettersController : MonoBehaviour
{
    private const float MoveDuration = 1f;

    [SerializeField] private AudioClip audioClip;
    // [SerializeField] private LetterHolderBase letterHolderPrefab;
    [SerializeField] private RectTransform container;
    [SerializeField] private RevealParticle revealParticlePrefab;
    
    private List<RevealParticle> _pool = new List<RevealParticle>();

    private float _scaleMultiplier;
    
    // private List<LetterHolderBase> _pool = new List<LetterHolderBase>();

    // public void Init(Vector2 cellSize)
    // {
    //     container.SetAsLastSibling();
    //     letterHolderPrefab.GetComponent<RectTransform>().sizeDelta = cellSize;
    //
    //     for (int i = 0; i < 10; i++)
    //     {
    //         var prefab = Instantiate(letterHolderPrefab, container);
    //         if (prefab is RevealLetterHolder revealLetterHolder)
    //         {
    //             revealLetterHolder.Init(MoveDuration);
    //         }
    //         
    //         _pool.Add(prefab);
    //     }
    // }

    public void Init(float scaleMultiplier)
    {
        _scaleMultiplier = scaleMultiplier;
        
        container.SetAsLastSibling();
        revealParticlePrefab.Init(scaleMultiplier);
    }
    
    // public void RevealLetter(LetterHolderBase currentholder, char letter, LetterHolderBase target, Action onComplete = null)
    // {
    //     var position = currentholder.rect.anchoredPosition;
    //     target.Revealing = true;
    //     
    //     var letterHolder = GetHolderFromPool();
    //     letterHolder.SetLetterText(letter);
    //
    //     if (Mathf.Abs(currentholder.LineIndex - target.LineIndex) >= 3)
    //     {
    //         ((RevealLetterHolder)letterHolder).Init(MoveDuration);
    //     }
    //
    //     var letterHolderTransform = letterHolder.rect;
    //     var targetPosition = target.rect.anchoredPosition;
    //     
    //     letterHolderTransform.anchoredPosition = position;
    //     letterHolder.gameObject.SetActive(true);
    //
    //     if (letterHolder is RevealLetterHolder revealLetterHolder)
    //     {
    //         revealLetterHolder.StartAnimation();
    //     }
    //     
    //     letterHolderTransform.DOAnchorPos(targetPosition, MoveDuration + .05f)
    //         // .OnUpdate(() =>
    //         // {
    //         //     targetPosition = target.transform.localPosition;
    //         // })
    //         .OnComplete(() =>
    //         {
    //             OnLetterRevealed(letterHolder, target);
    //             onComplete?.Invoke();
    //         });
    // }
    
    public void RevealLetter(LetterHolderBase currentholder, char letter, LetterHolderBase target, Action onComplete = null)
    {
        var position = currentholder.rect.anchoredPosition;
        target.Revealing = true;
        
        var revealParticle = GetHolderFromPool();
        revealParticle.SetLetter(letter);
        
        var targetPosition = target.rect.anchoredPosition;
        
        revealParticle.SetPosition(position);
        revealParticle.gameObject.SetActive(true);
        
        revealParticle.SetTarget(targetPosition);
        revealParticle.onGetPoint = () => OnLetterRevealed(revealParticle, target);
        revealParticle.ShowParticle(() =>
        {
            onComplete?.Invoke();
        });
    }

    // private void OnLetterRevealed(LetterHolderBase current, LetterHolderBase target)
    // {
    //     //var canvasGroup = current.GetComponent<CanvasGroup>();
    //     //var particle = current.GetComponentInChildren<ParticleSystem>();
    //     //var particleRenderer = particle.GetComponent<ParticleSystemRenderer>();
    //     //particleRenderer.material.DOFade(0, 0.5f);
    //     //canvasGroup.DOFade(0, 0.5f).OnComplete(() => );
    //     current.gameObject.SetActive(false);
    //     target.RevealLetter(true, false);
    //     AudioController.Instance.Play(SoundType.Sounds, audioClip);
    // }
    
    private void OnLetterRevealed(RevealParticle current, LetterHolderBase target)
    {
        //var canvasGroup = current.GetComponent<CanvasGroup>();
        //var particle = current.GetComponentInChildren<ParticleSystem>();
        //var particleRenderer = particle.GetComponent<ParticleSystemRenderer>();
        //particleRenderer.material.DOFade(0, 0.5f);
        //canvasGroup.DOFade(0, 0.5f).OnComplete(() => );
        target.RevealLetter(true, false);
        AudioController.Instance.Play(SoundType.Sounds, audioClip);
    }
    
    private RevealParticle GetHolderFromPool()
    {
        var letterHolder = _pool.FirstOrDefault(x => !x.IsPlaying);
        
        if(letterHolder == null)
        {
            letterHolder = Instantiate(revealParticlePrefab, container);
            _pool.Add(letterHolder);
        }

        letterHolder.Init(_scaleMultiplier);
        return letterHolder;
    }

    // private LetterHolderBase GetHolderFromPool()
    // {
    //     var letterHolder = _pool.FirstOrDefault(x => !x.gameObject.activeSelf);
    //     
    //     if(letterHolder == null)
    //     {
    //         letterHolder = Instantiate(letterHolderPrefab, container);
    //         if (letterHolder is RevealLetterHolder revealLetterHolder)
    //         {
    //             revealLetterHolder.Init(MoveDuration);
    //         }
    //         _pool.Add(letterHolder);
    //     }
    //     
    //     //letterHolder.GetComponent<CanvasGroup>().alpha = 1f;
    //     
    //     //var particle = letterHolder.GetComponentInChildren<ParticleSystem>();
    //     //var particleRenderer = particle.GetComponent<ParticleSystemRenderer>();
    //     //var material = particleRenderer.material;
    //     //var particleColor = material.color;
    //     //particleColor.a = 1f;
    //     //material.color = particleColor;
    //
    //     return letterHolder;
    // }
}