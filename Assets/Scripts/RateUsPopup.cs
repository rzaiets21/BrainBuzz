using System;
using System.Collections;
using Google.Play.Review;
using UnityEngine;
using UnityEngine.UI;

public class RateUsPopup : Popup
{
    [SerializeField] private Color inactiveColor;
    [SerializeField] private Button[] starButtons;

    [SerializeField] private Button rateButton;
    
    private int _rate = 5;

    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;
    
    private Coroutine _coroutine;

    private void Start()
    {
        _coroutine = StartCoroutine(InitReview());
    }

    private void OnEnable()
    {
        rateButton.onClick.AddListener(OnRateButtonClick);
        for (int i = 0; i < starButtons.Length; i++)
        {
            var rate = i;
            starButtons[i].onClick.AddListener(() => OnRateStarClick(rate));
        }
    }

    private void OnDisable()
    {
        rateButton.onClick.RemoveListener(OnRateButtonClick);
        foreach (var starButton in starButtons)
        {
            starButton.onClick.RemoveAllListeners();
        }
    }

    private void OnDestroy()
    {
        StopCoroutine(_coroutine);
    }

    protected override void OnShown()
    {
        foreach (var starButton in starButtons)
        {
            starButton.image.color = Color.white;
        }
    }

    private IEnumerator InitReview(bool force = false)
    {
        if (_reviewManager == null) _reviewManager = new ReviewManager();

        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            if (force) DirectlyOpen();
            yield break;
        }

        _playReviewInfo = requestFlowOperation.GetResult();
    }
    
    public IEnumerator LaunchReview()
    {
        if (_playReviewInfo == null)
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            yield return StartCoroutine(InitReview(true));
        }

        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null;
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            DirectlyOpen();
            yield break;
        }
    }
    
    private void DirectlyOpen() { Application.OpenURL($"https://play.google.com/store/apps/details?id={Application.identifier}"); }
    
    private void OnRateButtonClick()
    {
        StartCoroutine(LaunchReview());
    }
    
    private void OnRateStarClick(int rate)
    {
        _rate = rate + 1;
        for (int i = 0; i < starButtons.Length; i++)
        {
            starButtons[i].image.color = i <= rate ? Color.white : inactiveColor;
        }
    }
}
