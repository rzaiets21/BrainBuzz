using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopPanel : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Button watchAdsButton;
    
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private RectTransform coinsIcon;
    [SerializeField] private ScreensController screensController;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject particlesPrefab;
    [SerializeField] private RectTransform particlesContainer;
    [SerializeField] private BeatingItem beatingCoin;
    [SerializeField] private BeatingItem beatingPlus;

    private Tween _coinTween;
    private List<GameObject> _particlesPool = new List<GameObject>();
    
    private int _currentCoinUIValue;
    public int CurrentCoinUIValue => _currentCoinUIValue;
    
    private void Start()
    {
        StartCoroutine(ShowWatchAdsButton());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void OnEnable()
    {
        watchAdsButton.onClick.AddListener(ShowReward);
        screensController.onScreenShown += OnScreenShow;
        PlayerInventory.Instance.OnCoinsValueChanged += UpdateCoinsValue;
        PlayerInventory.Instance.Update();
    }

    private void OnDisable()
    {
        watchAdsButton.onClick.RemoveListener(ShowReward);
        screensController.onScreenShown -= OnScreenShow;
        PlayerInventory.Instance.OnCoinsValueChanged -= UpdateCoinsValue;
    }

    public void OnClickBack()
    {
        if (screensController.CurrentScreen == Screens.Game)
        {
            screensController.ShowScreen(Screens.MainMenu);
            return;
        }
        
        screensController.ShowPreviousScreen();
    }

    public void ShowStore()
    {
        if(screensController.CurrentScreen is Screens.Store)
            return;
        
        screensController.ShowScreen(Screens.Store, ScreenTransition.RightToLeft, hidePrevious: false);
    }

    private void OnScreenShow(Screens screenType)
    {
        var showBackButton = screenType is Screens.Game or Screens.Store;
        
        backButton.SetActive(showBackButton);
        label.gameObject.SetActive(showBackButton);
        if (showBackButton)
        {
            label.SetText(screenType is Screens.Game ? $"Group {gameManager.CurrentLevel}" : "Store");
        }
        
        canvasGroup.SetActive(screenType != Screens.Loading);
    }

    public void UpdateCoinsValue(int value)
    {
        _currentCoinUIValue = value;
        coinsText.SetText(value.ToString());
    }

    public void UpdateCoinsValueWithParticles(int value)
    {
        UpdateCoinsValue(value);
        TickCoin();
    }

    private void TickCoin()
    {
        beatingCoin.StopBeating();
        beatingPlus.StopBeating();
        
        if (_coinTween != null)
        {
            if (_coinTween.IsActive())
            {
                _coinTween.Kill();
            }
            if (_coinTween.IsPlaying())
            {
                _coinTween.Kill();
            }

            coinsIcon.localScale = Vector3.one;
        }

        _coinTween = coinsIcon.DOScale(1.15f, 0.075f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
        {
            beatingCoin.StartBeating();
            beatingPlus.StartBeating();
        });
        var particle = GetParticleFromPool();
        particle.SetActive(true);
        DOVirtual.DelayedCall(0.6f, () => particle.SetActive(false));
    }

    private GameObject GetParticleFromPool()
    {
        var particle = _particlesPool.FirstOrDefault(x => !x.activeSelf);
        if (particle == null)
        {
            particle = Instantiate(particlesPrefab, particlesContainer);
            _particlesPool.Add(particle);
        }

        return particle;
    }

    private IEnumerator ShowWatchAdsButton()
    {
        yield return new WaitForSeconds(2f);
        
        var adsIsReady = AdsManager.RewardAdsIsReady;
        watchAdsButton.gameObject.SetActive(adsIsReady);
        
        while (!adsIsReady)
        {
            yield return new WaitForSeconds(2f);
            
            adsIsReady = AdsManager.RewardAdsIsReady;
            watchAdsButton.gameObject.SetActive(adsIsReady);
            
            yield return null;
        }
    }
    
    private void ShowReward()
    {
        AdsManager.ShowRewardedAd(OnRewarded);
    }

    private void OnRewarded()
    {
        StartCoroutine(ShowWatchAdsButton());
        PlayerInventory.Instance.Add(25);
    }
}