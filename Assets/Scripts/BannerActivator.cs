using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerActivator : MonoBehaviour
{
    private const string CompletedLevels = "CompletedLevels";
    
    [SerializeField] private ScreensController screensController;

    private void OnEnable()
    {
        screensController.onScreenShown += OnScreenShown;
    }
    
    private void OnDisable()
    {
        screensController.onScreenShown -= OnScreenShown;
    }

    private void ShowBanner()
    {
        if(AdsManager.Instance.BannerIsShown)
            return;
     
        var completedLevels = PlayerPrefs.GetInt(CompletedLevels, 0);
        if(completedLevels > 0) 
            AdsManager.Instance.ShowBanner();
    }
    
    private void OnScreenShown(Screens screen)
    {
        if(screen != Screens.MainMenu && screen != Screens.Game)
            return;
        ShowBanner();
    }
}
