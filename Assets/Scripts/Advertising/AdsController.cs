using UnityEngine;
using System;
using System.Collections;
using GameAnalyticsSDK;
using GoogleMobileAds.Api;
using UnityEngine;

namespace Advertising
{
    public class AdsController: MonoBehaviour
    {
#if UNITY_ANDROID
        private string _adUnitId = "ca-app-pub-1268870789270764~1753194350";
        private string _rewardId = "ca-app-pub-1268870789270764/6371096217";
        private string _interstitialId = "ca-app-pub-1268870789270764/2759154267";
        private string _bannerId = "ca-app-pub-1268870789270764/2713152308";
#elif UNITY_IPHONE
        private string _rewardAdUnitId = "ca-app-pub-1268870789270764/1723977024";
        private string _rewardId = "ca-app-pub-1268870789270764/1723977024";
        private string _interstitialId = "ca-app-pub-1268870789270764/2957167648";
        private string _bannerId = "ca-app-pub-1268870789270764/4270249313";
#else
            private string _adUnitId = "unused";
#endif

        private bool _isInitialized;
        private RewardedAd _rewardedAd;
        private InterstitialAd _interstitialAd;
        private static AdsController _instance;
        private Action _rewardAction;

        public Action OnAdsOpened;
        public Action OnAdsClosed;
        public Action OnAdsFailed;

        public static AdsController Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject gameSystemObject = new GameObject("AdsController");
                    _instance = gameSystemObject.AddComponent<AdsController>();
                }

                return _instance;
            }
        }

        public bool IsInitialized => _isInitialized;
        
        public bool IsRewardedLoaded()
        {
#if UNITY_EDITOR
            return true;
#endif
            if (_rewardedAd == null)
            {
                Debug.LogWarning("Rewarded ad is null.");
                return false;
            }

            bool canShowAd = _rewardedAd.CanShowAd();
            Debug.Log($"Rewarded ad CanShowAd: {canShowAd}");
            return canShowAd;
        }
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            StartCoroutine(Initialize());
        }
        
        private IEnumerator Initialize()
        {
            MobileAds.Initialize(initStatus => { _isInitialized = true; });
            yield return new WaitUntil(() => _isInitialized);
            MobileAds.RaiseAdEventsOnUnityMainThread = true;
            LoadRewardedAd();
            LoadInterstitialAd();
        }

        private void RegisterEventHandlers(RewardedAd ad)
        {
            ad.OnAdFullScreenContentOpened += AdFullScreenContentOpened;
            ad.OnAdFullScreenContentClosed += AdFullScreenContentClosed;
            ad.OnAdFullScreenContentFailed += AdFullScreenContentFailed;
        }
        
        private void UnregisterEventHandlers(RewardedAd ad)
        {
            ad.OnAdFullScreenContentOpened -= AdFullScreenContentOpened;
            ad.OnAdFullScreenContentClosed -= AdFullScreenContentClosed;
            ad.OnAdFullScreenContentFailed -= AdFullScreenContentFailed;
        }
        
        private void RegisterEventHandlers(InterstitialAd ad)
        {
            ad.OnAdFullScreenContentOpened += AdFullScreenContentOpened;
            ad.OnAdFullScreenContentClosed += InterstitialAdFullScreenContentClosed;
            ad.OnAdFullScreenContentFailed += InterstitialAdFullScreenContentFailed;
        }
        
        private void UnregisterEventHandlers(InterstitialAd ad)
        {
            ad.OnAdFullScreenContentOpened -= AdFullScreenContentOpened;
            ad.OnAdFullScreenContentClosed -= InterstitialAdFullScreenContentClosed;
            ad.OnAdFullScreenContentFailed -= InterstitialAdFullScreenContentFailed;
        }
        
        private void AdFullScreenContentOpened()
        {
            OnAdsOpened?.Invoke();
        }

        private void AdFullScreenContentClosed()
        {
            LoadRewardedAd();
            OnAdsClosed?.Invoke();
        }

        private void AdFullScreenContentFailed(AdError error)
        {
            OnAdsFailed?.Invoke();
            LoadRewardedAd();
        }
        
        private void InterstitialAdFullScreenContentClosed()
        {
            LoadInterstitialAd();
            OnAdsClosed?.Invoke();
        }

        private void InterstitialAdFullScreenContentFailed(AdError error)
        {
            OnAdsFailed?.Invoke();
            LoadInterstitialAd();
        }

        private void LoadRewardedAd()
        {
            if (_rewardedAd != null)
            {
                UnregisterEventHandlers(_rewardedAd); 
                _rewardedAd.Destroy();
                _rewardedAd = null;
                _rewardAction = null;
            }

            var adRequest = new AdRequest();
            RewardedAd.Load(_rewardAdUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " + "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad successfully loaded.");
                _rewardedAd = ad;
                RegisterEventHandlers(_rewardedAd);
            });
        }
        
        public void ShowInterstitialAd()
        {
#if UNITY_EDITOR
            Debug.Log("ShowInterstitialAd");
#endif
            if (IsAdsFree())
            {
                return;
            }
        
            if (_interstitialAd != null && _interstitialAd.CanShowAd())
            {
                _interstitialAd.Show();
            }
            else
            {
                LoadInterstitialAd();
            }
        }
        
        private void LoadInterstitialAd()
        {
            if (_interstitialAd != null)
            {
                _interstitialAd.Destroy();
                _interstitialAd = null;
            }

            var adRequest = new AdRequest();
            InterstitialAd.Load(_interstitialId, adRequest, (InterstitialAd ad, LoadAdError error) =>
                {
                    if (error != null || ad == null)
                    {
                        Debug.LogError("interstitial ad failed to load an ad " + "with error : " + error);
                        LoadInterstitialAd();
                        return;
                    }

                    Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo());

                    _interstitialAd = ad;
                    RegisterEventHandlers(_interstitialAd);
                });
        }

        public void ShowRewardedAd(Action action)
        {
#if UNITY_EDITOR
            Debug.Log("ShowRewardedAd");
#endif
            _rewardAction = action;
            if (_rewardedAd != null && _rewardedAd.CanShowAd())
            {
                _rewardedAd.Show((Reward reward) =>
                {
                    _rewardAction?.Invoke();
                    _rewardAction = null;
                });
            }
#if UNITY_EDITOR
            _rewardAction?.Invoke();
            _rewardAction = null;
#endif
        }

        private bool IsAdsFree()
        {
            return PlayerPrefs.GetInt("AdsFree", 0) == 1;
        }
    }
}