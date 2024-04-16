using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileParticlesController : MonoBehaviour
{
    [SerializeField] private TileParticles tileParticlesPrefab;
    [SerializeField] private RectTransform container;

    private List<TileParticles> _pool = new List<TileParticles>();

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
        container.SetAsLastSibling();
        tileParticlesPrefab.GetComponent<RectTransform>().sizeDelta = cellSize;

        for (int i = 0; i < 10; i++)
        {
            var prefab = Instantiate(tileParticlesPrefab, container);
            prefab.gameObject.SetActive(false);
            _pool.Add(prefab);
        }
    }
    
    public void ShowParticle(LetterHolderBase currentHolder, TileParticleType particleType, Action onComplete = null)
    {
        var position = currentHolder.rect.anchoredPosition;
        
        var tileParticle = GetParticleFromPool();
        
        var tileParticleTransform = tileParticle.GetComponent<RectTransform>();
        
        tileParticleTransform.anchoredPosition = position;
        
        tileParticle.gameObject.SetActive(true);
        
        tileParticle.ShowParticle(particleType, () =>
        {
            tileParticle.gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }

    private TileParticles GetParticleFromPool()
    {
        var tileParticle = _pool.FirstOrDefault(x => !x.IsPlaying);

        if (tileParticle != null) return tileParticle;
        
        tileParticle = Instantiate(tileParticlesPrefab, container);
        _pool.Add(tileParticle);

        return tileParticle;
    }
}
