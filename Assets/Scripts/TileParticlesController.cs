using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using VFX;

public class TileParticlesController : MonoBehaviour
{
    //[SerializeField] private TileParticles tileParticlesPrefab;
    [SerializeField] private RectTransform container;

    [SerializeField] private VFXBase correctAnswerPrefab;
    [SerializeField] private VFXBase explosionPrefab;
    [SerializeField] private VFXBase firstCorrectAnswerPrefab;
    
    private List<VFXBase> _correctAnswersPool = new List<VFXBase>();
    private List<VFXBase> _explosionPool = new List<VFXBase>();
    private List<VFXBase> _firstCorrectAnswersPool = new List<VFXBase>();

    private float _scaleMultiplier;
    
    public static TileParticlesController Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Init(Vector2 cellSize)
    {
        // container.SetAsLastSibling();
        // tileParticlesPrefab.GetComponent<RectTransform>().sizeDelta = cellSize;
        //
        // for (int i = 0; i < 10; i++)
        // {
        //     var prefab = Instantiate(tileParticlesPrefab, container);
        //     prefab.gameObject.SetActive(false);
        //     _pool.Add(prefab);
        // }
    }

    public void Init(float scaleMultiplier)
    {
        _scaleMultiplier = scaleMultiplier;
        
        container.SetAsLastSibling();
        correctAnswerPrefab.Init(scaleMultiplier);
        firstCorrectAnswerPrefab.Init(scaleMultiplier);
        explosionPrefab.Init(scaleMultiplier);

        // for (int i = 0; i < 10; i++)
        // {
        //     var prefab = Instantiate(tileParticlesPrefab, container);
        //     prefab.gameObject.SetActive(false);
        //     _pool.Add(prefab);
        // }
    }
    
    // public void ShowParticle(LetterHolderBase currentHolder, TileParticleType particleType, Action onComplete = null)
    // {
    //     var position = currentHolder.rect.anchoredPosition;
    //     
    //     var tileParticle = GetParticleFromPool();
    //     
    //     var tileParticleTransform = tileParticle.GetComponent<RectTransform>();
    //     
    //     tileParticleTransform.anchoredPosition = position;
    //     
    //     tileParticle.gameObject.SetActive(true);
    //     
    //     tileParticle.ShowParticle(particleType, () =>
    //     {
    //         tileParticle.gameObject.SetActive(false);
    //         onComplete?.Invoke();
    //     });
    // }
    
    public void ShowParticle(LetterHolderBase currentHolder, VFXType particleType, Action onComplete = null)
    {
        var position = currentHolder.rect.anchoredPosition;
        
        var tileParticle = GetParticleFromPool(particleType);
        
        tileParticle.SetPosition(position);
        tileParticle.gameObject.SetActive(true);
        
        tileParticle.ShowParticle(() =>
        {
            //tileParticle.gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }

    // private TileParticles GetParticleFromPool()
    // {
    //     var tileParticle = _pool.FirstOrDefault(x => !x.IsPlaying);
    //
    //     if (tileParticle != null) return tileParticle;
    //     
    //     tileParticle = Instantiate(tileParticlesPrefab, container);
    //     _pool.Add(tileParticle);
    //
    //     return tileParticle;
    // }

    private VFXBase GetParticleFromPool(VFXType particleType)
    {
        var targetList = particleType switch
        {
            VFXType.FirstCorrectAnswer => _firstCorrectAnswersPool,
            VFXType.CorrectAnswer => _correctAnswersPool,
            VFXType.Explosion => _explosionPool,
            _=> throw new NotImplementedException()
        };
        
        var tileParticle = targetList.FirstOrDefault(x => !x.IsPlaying);

        if (tileParticle != null)
        {
            tileParticle.Init(_scaleMultiplier);
            return tileParticle;
        }
        
        var tileParticlesPrefab = particleType switch
        {
            VFXType.FirstCorrectAnswer => firstCorrectAnswerPrefab,
            VFXType.CorrectAnswer => correctAnswerPrefab,
            VFXType.Explosion => explosionPrefab,
            _=> throw new NotImplementedException()
        };
        
        tileParticle = Instantiate(tileParticlesPrefab, container);
        targetList.Add(tileParticle);
        
        tileParticle.Init(_scaleMultiplier);

        return tileParticle;
    }
}
