/*
using System;
using System.Collections;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    [SerializeField] private bool test;
    
    private string _rewardId;
    private string _interstitialId;
    private string _bannerId;
    
    private static AdsManager _instance;

    private Coroutine _rewardedDelayCoroutine;
    private Coroutine _interstitialDelayCoroutine;
    private RewardedAd _rewardedAd;
    private InterstitialAd _interstitialAd;
    private BannerView _bannerView;

    private int _rewardedCount;
    private bool _bannerIsShown;
    
    public static bool RewardAdsIsReady => _instance._rewardedAd != null && _instance._rewardedAd.CanShowAd();
    public bool BannerIsShown => _instance._bannerIsShown;
    public static float BannerHeight => _instance._bannerView.GetHeightInPixels();

    public static AdsManager Instance => _instance;

    public static event Action OnBannerShown;
    
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

#if UNITY_ANDROID
        _rewardId = "ca-app-pub-1268870789270764/6371096217";
        _interstitialId = "ca-app-pub-1268870789270764/2759154267";
        _bannerId = "ca-app-pub-1268870789270764/2713152308";
#elif UNITY_IOS
        _rewardId = "ca-app-pub-1268870789270764/1723977024";
        _interstitialId = "ca-app-pub-1268870789270764/2957167648";
        _bannerId = "ca-app-pub-1268870789270764/4270249313";
#endif
        
        _instance = this;
    }

    private void Start()
    {
        _rewardedCount = PlayerPrefs.GetInt("RewardedCount", 0);
        Init();
    }

    public bool IsAdsFree()
    {
        return PlayerPrefs.GetInt("AdsFree", 0) == 1;
    }

    private void Init()
    {
        StartCoroutine(InitAds());
    }

    private IEnumerator InitAds()
    {
        bool isInitialized = false;
        MobileAds.Initialize(status => 
        {
            isInitialized = true;
        });

        yield return new WaitUntil(() => isInitialized);
        var requestConfiguration = MobileAds.GetRequestConfiguration();
            
        if(requestConfiguration != null)
        {
            var deviceId = SystemInfo.deviceUniqueIdentifier;
#if UNITY_ANDROID
            requestConfiguration.TestDeviceIds.Add(deviceId.ToUpper());
#elif UNITY_IOS
            requestConfiguration.TestDeviceIds.Add(deviceId);
#endif
            MobileAds.SetRequestConfiguration(requestConfiguration);
        }
            
        LoadRewardedAd();
        LoadInterstitialAd();
        CreateBannerView();
    }

    private void OnDestroy()
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        if (_rewardedDelayCoroutine != null)
            StopCoroutine(_rewardedDelayCoroutine);
    }

    private void LoadRewardedAd()
    {
       
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }
        
        var adRequest = new AdRequest();

        RewardedAd.Load(_rewardId, adRequest,
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
    
    private void LoadInterstitialAd()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        var adRequest = new AdRequest();

        InterstitialAd.Load(_interstitialId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    LoadInterstitialAd(2f);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                _interstitialAd = ad;
                RegisterReloadHandler(_interstitialAd);
            });
    }

    public void CreateBannerView()
    { Debug.Log("Creating banner view");

        if (_bannerView != null)
        {
            DestroyBannerView();
        }

        if (IsAdsFree())
        {
            return;
        }

        _bannerView = new BannerView(_bannerId, AdSize.Banner, AdPosition.Bottom);
        
        _bannerView.OnBannerAdLoaded += OnBannerAdLoaded;
        _bannerView.OnBannerAdLoadFailed += OnBannerAdLoadFailed;
    }
    
    public void DestroyBannerView()
    {
        if (_bannerView == null) return;
        
        Debug.Log("Destroying banner view.");
        _bannerView.Destroy();
        _bannerView = null;
    }
    
    private void LoadRewardedAd(float delay)
    {
        if(_rewardedDelayCoroutine != null)
            StopCoroutine(_rewardedDelayCoroutine);
        
        _rewardedDelayCoroutine = StartCoroutine(DelayCoroutine(delay, LoadRewardedAd));
    }

    private void LoadInterstitialAd(float delay)
    {
        if(_interstitialDelayCoroutine != null)
            StopCoroutine(_interstitialDelayCoroutine);
        
        _interstitialDelayCoroutine = StartCoroutine(DelayCoroutine(delay, LoadInterstitialAd));
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
    
    private void RegisterReloadHandler(InterstitialAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            LoadInterstitialAd(1.5f);
        };
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            LoadInterstitialAd(1.5f);
        };
    }
    
    public void ShowBanner()
    {
        if (IsAdsFree())
        {
            return;
        }
        
        if(_instance._bannerView == null)
        {
            Debug.LogError("IS NULL");
            _instance.CreateBannerView();
        }

        var adRequest = new AdRequest();
        
        Debug.Log("Loading banner ad.");
        _instance._bannerView.LoadAd(adRequest);
    }
    
    private void OnBannerAdLoaded()
    {
        _bannerIsShown = true;
        OnBannerShown?.Invoke();
    }

    private void OnBannerAdLoadFailed(LoadAdError error)
    {
        _bannerIsShown = false;
    }
    
    public void ShowRewardedAd(Action onRewardComplete)
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
    
    public void ShowInterstitialAd()
    {
        if (IsAdsFree())
        {
            return;
        }
        
        if (_instance._interstitialAd != null && _instance._interstitialAd.CanShowAd())
        {
            _instance._interstitialAd.Show();
        }
    }

    private IEnumerator DelayCoroutine(float delay, Action onComplete = null)
    {
        yield return new WaitForSeconds(delay);
        onComplete?.Invoke();
        
        _rewardedDelayCoroutine = null;
    }
}
*/
