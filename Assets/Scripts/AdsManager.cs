using System;
using System.Collections;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    [SerializeField] private string rewardId;
    
    private static AdsManager _instance;

    private Coroutine _delayCoroutine;
    private RewardedAd _rewardedAd;

    private int _rewardedCount;
    
    public static bool RewardAdsIsReady => _instance._rewardedAd != null && _instance._rewardedAd.CanShowAd();
    
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
    }

    private void Start()
    {
        _rewardedCount = PlayerPrefs.GetInt("RewardedCount", 0);
        Init();
    }

    private void Init()
    {
        MobileAds.Initialize(status =>
        {
            LoadRewardedAd();
        });
    }

    private void OnDestroy()
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        if (_delayCoroutine != null)
            StopCoroutine(_delayCoroutine);
    }

    private void LoadRewardedAd()
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }
        
        var adRequest = new AdRequest();

        RewardedAd.Load(rewardId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    LoadRewardedAd(2f);
                    return;
                }

                _rewardedAd = ad;
                RegisterReloadHandler(_rewardedAd);
            });
    }

    private void LoadRewardedAd(float delay)
    {
        if(_delayCoroutine != null)
            StopCoroutine(_delayCoroutine);
        
        _delayCoroutine = StartCoroutine(DelayCoroutine(delay, LoadRewardedAd));
    }
    
    private void RegisterReloadHandler(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            LoadRewardedAd(1.5f);
        };
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            LoadRewardedAd(1.5f);
        };
    }
    
    public static void ShowRewardedAd(Action onRewardComplete)
    {
        if (_instance._rewardedAd != null && _instance._rewardedAd.CanShowAd())
        {
            _instance._rewardedAd.Show((Reward reward) =>
            {
                _instance._rewardedCount++;
                Analytics.LogEvent(Analytics.RewardAmount, _instance._rewardedCount);
                onRewardComplete?.Invoke();
            });
        }
    }

    private IEnumerator DelayCoroutine(float delay, Action onComplete = null)
    {
        yield return new WaitForSeconds(delay);
        onComplete?.Invoke();
        
        _delayCoroutine = null;
    }
}
