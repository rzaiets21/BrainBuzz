using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class RevealLettersController : MonoBehaviour
{
    private const float MoveDuration = 1f;

    [SerializeField] private AudioClip audioClip;
    [SerializeField] private LetterHolderBase letterHolderPrefab;
    [SerializeField] private RectTransform container;

    private List<LetterHolderBase> _pool = new List<LetterHolderBase>();

    public void Init(Vector2 cellSize)
    {
        container.SetAsLastSibling();
        letterHolderPrefab.GetComponent<RectTransform>().sizeDelta = cellSize;

        for (int i = 0; i < 10; i++)
        {
            var prefab = Instantiate(letterHolderPrefab, container);
            if (prefab is RevealLetterHolder revealLetterHolder)
            {
                revealLetterHolder.Init(MoveDuration);
            }
            
            _pool.Add(prefab);
        }
    }
    
    public void RevealLetter(LetterHolderBase currentholder, char letter, LetterHolderBase target, Action onComplete = null)
    {
        var position = currentholder.rect.anchoredPosition;
        target.Revealing = true;
        
        var letterHolder = GetHolderFromPool();
        letterHolder.SetLetterText(letter);

        if (Mathf.Abs(currentholder.LineIndex - target.LineIndex) >= 3)
        {
            ((RevealLetterHolder)letterHolder).Init(MoveDuration);
        }

        var letterHolderTransform = letterHolder.rect;
        var targetPosition = target.rect.anchoredPosition;
        
        letterHolderTransform.anchoredPosition = position;
        letterHolder.gameObject.SetActive(true);

        if (letterHolder is RevealLetterHolder revealLetterHolder)
        {
            revealLetterHolder.StartAnimation();
        }
        
        letterHolderTransform.DOAnchorPos(targetPosition, MoveDuration + .05f)
            // .OnUpdate(() =>
            // {
            //     targetPosition = target.transform.localPosition;
            // })
            .OnComplete(() =>
            {
                OnLetterRevealed(letterHolder, target);
                onComplete?.Invoke();
            });
    }

    private void OnLetterRevealed(LetterHolderBase current, LetterHolderBase target)
    {
        //var canvasGroup = current.GetComponent<CanvasGroup>();
        //var particle = current.GetComponentInChildren<ParticleSystem>();
        //var particleRenderer = particle.GetComponent<ParticleSystemRenderer>();
        //particleRenderer.material.DOFade(0, 0.5f);
        //canvasGroup.DOFade(0, 0.5f).OnComplete(() => );
        current.gameObject.SetActive(false);
        target.RevealLetter(true, false);
        AudioController.Instance.Play(SoundType.Sounds, audioClip);
    }

    private LetterHolderBase GetHolderFromPool()
    {
        var letterHolder = _pool.FirstOrDefault(x => !x.gameObject.activeSelf);
        
        if(letterHolder == null)
        {
            letterHolder = Instantiate(letterHolderPrefab, container);
            if (letterHolder is RevealLetterHolder revealLetterHolder)
            {
                revealLetterHolder.Init(MoveDuration);
            }
            _pool.Add(letterHolder);
        }
        
        //letterHolder.GetComponent<CanvasGroup>().alpha = 1f;
        
        //var particle = letterHolder.GetComponentInChildren<ParticleSystem>();
        //var particleRenderer = particle.GetComponent<ParticleSystemRenderer>();
        //var material = particleRenderer.material;
        //var particleColor = material.color;
        //particleColor.a = 1f;
        //material.color = particleColor;

        return letterHolder;
    }
}