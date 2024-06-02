using System;
using System.Collections;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    [SerializeField] private bool test;
    
    [SerializeField] private string rewardId;
    [SerializeField] private string interstitialId;
    [SerializeField] private string bannerId;
    
    private static AdsManager _instance;

    private Coroutine _rewardedDelayCoroutine;
    private Coroutine _interstitialDelayCoroutine;
    private RewardedAd _rewardedAd;
    private InterstitialAd _interstitialAd;
    private BannerView _bannerView;

    private int _rewardedCount;
    private bool _bannerIsShown;
    
    public static bool RewardAdsIsReady => _instance._rewardedAd != null && _instance._rewardedAd.CanShowAd();
    public static bool BannerIsShown => _instance._bannerIsShown;
    public static float BannerHeight => _instance._bannerView.GetHeightInPixels();

    public static event Action OnBannerShown;
    
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

#if UNITY_ANDROID
        rewardId = "ca-app-pub-3940256099942544/5224354917";
        interstitialId = "ca-app-pub-3940256099942544/1033173712";
        bannerId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IOS
        rewardId = "ca-app-pub-3940256099942544/1712485313";
        interstitialId = "ca-app-pub-3940256099942544/4411468910";
        bannerId = "ca-app-pub-3940256099942544/2934735716";
#endif
        
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
        });
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
    
    private void LoadInterstitialAd()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        var adRequest = new AdRequest();

        InterstitialAd.Load(interstitialId, adRequest,
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
    {
        Debug.Log("Creating banner view");

        if (_bannerView != null)
        {
            DestroyBannerView();
        }

        _bannerView = new BannerView(bannerId, AdSize.Banner, AdPosition.Bottom);
        
        _bannerView.OnBannerAdLoaded += OnBannerAdLoaded;
        _bannerView.OnBannerAdLoadFailed += OnBannerAdLoadFailed;
    }
    
    private void DestroyBannerView()
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
    
    public static void ShowBanner()
    {
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
    
    public static void ShowInterstitialAd()
    {
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
