using Moonee.MoonSDK.Internal;
using UnityEngine;
using System;
using com.adjust.sdk;

public class PlaytimeCalculator : MonoBehaviour
{
    MoonSDKSettings settings;
    public static bool isCalculatingTimer = true;

    public static int sessionNumber = 0;
    public static double currentTotalSessionTime;
    public static double currentTotalAdsTime;

    private float sessionEventTimer;
    private float sessionEventInterval;

    public static double totalTimeInStore;
    public static double notLevelTotalTime;


    public static DateTime levelStartDateTime;
    public static DateTime levelEndDateTime;
    public static DateTime sessionStartDateTime;
    public static DateTime openStoreDateTime;
    public static DateTime closeStoreDateTime;
    public static DateTime adStartDateTime;
    public static DateTime adFinishDateTime;

    public static bool isOnFocusFromAds;

    private DateTime OutOfFocusDateTime;
    private DateTime InFocusDateTime;

    private void Start()
    {
        settings = MoonSDKSettings.Load();

        sessionNumber = (int)PlayerPrefs.GetFloat("sessionNumber", 0);

        settings = MoonSDKSettings.Load();

        if (settings.AdjustBasic == false)
        {
            Destroy(this);
            return;
        }
        sessionNumber += 1;
        PlayerPrefs.SetFloat("sessionNumber", sessionNumber);
        sessionStartDateTime = DateTime.Now;

        sessionEventTimer = sessionEventInterval;
    }
    private void Update()
    {
        //if (isCalculatingTimer)
        //{
        //    currentTotalSessionTime += Time.deltaTime;
        //}
        //if (sessionEventTimer >= 0)
        //{
        //    sessionEventTimer -= Time.deltaTime;
        //}
        //else
        //{
        //    sessionEventTimer = sessionEventInterval;
        //    SendSessionEvent();
        //}
    }
    private void SendSessionEvent()
    {
        AdjustEvent adjustEvent = new AdjustEvent(settings.moonSession);

        adjustEvent.addCallbackParameter("session", $"{sessionNumber}");
        adjustEvent.addCallbackParameter("playtime", $"{Mathf.RoundToInt((float)currentTotalSessionTime)}");
        if (sessionNumber == 1)
        {
            adjustEvent.addCallbackParameter("moon_sdk_version", $"{MoonSDK.Version}");
        }
        adjustEvent.addCallbackParameter("ads_time", $"{Mathf.RoundToInt((float)currentTotalAdsTime)}");

        Adjust.trackEvent(adjustEvent);
    }
    private void OnApplicationFocus(bool focus)
    {
        isCalculatingTimer = focus;

        if (focus == false)
        {
            OutOfFocusDateTime = DateTime.Now;
        }
        else
        {
            InFocusDateTime = DateTime.Now;

            SendEventToAdjust();
        }
    }
    private void OnDestroy()
    {
        SendEventToAdjust();
    }
    private void SendEventToAdjust()
    {
        TimeSpan timeSpan = default;
        if (OutOfFocusDateTime != default && InFocusDateTime != default && isOnFocusFromAds == true)
        {
            timeSpan = InFocusDateTime - OutOfFocusDateTime;
            currentTotalSessionTime += timeSpan.TotalSeconds;
            currentTotalAdsTime += timeSpan.TotalSeconds;
            isOnFocusFromAds = false;
        }

        if (isOnFocusFromAds == false)
        {
            if (timeSpan.TotalSeconds > 1800)
            {
                currentTotalSessionTime = 0;
                sessionNumber++;
            }
        }
    }
    private void OnApplicationPause(bool pause)
    {
        //isCalculatingTimer = !pause;
    }
}