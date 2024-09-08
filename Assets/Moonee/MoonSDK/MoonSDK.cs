using com.adjust.sdk;
using GameAnalyticsSDK;
using Moonee.MoonSDK.Internal;
using System;
using UnityEngine;

public static class MoonSDK
{
    public const string Version = "1.0.4";

    public static void TrackCustomEvent(string eventName)
    {
        GameAnalytics.NewDesignEvent(eventName);
    }
    private static void TrackLevelEvents(LevelStatus eventType, int levelIndex)
    {
        string outputValue = "level" + String.Format("{0:D4}", levelIndex);

        if (eventType == LevelStatus.start)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, outputValue);
        }
        else if (eventType == LevelStatus.fail)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, outputValue);
        }
        else if (eventType == LevelStatus.complete)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, outputValue);
        }
    }
    public static void SendLevelDataCompleteEvent(LevelStatus status, int levelNumber, LevelResult result, bool if_continue, int coinsAmount, int moves = 0)
    {
        MoonSDKSettings settings = MoonSDKSettings.Load();
        PlaytimeCalculator.levelEndDateTime = DateTime.Now;
        TimeSpan levelPlaytime = DateTime.Now - PlaytimeCalculator.levelStartDateTime;
        double secs = levelPlaytime.TotalSeconds;

        if (secs < 0)
        {
            secs *= -1;
        }
        AdjustEvent adjustEvent = new AdjustEvent(settings.adjustLevelDataCompleteEventToken);

        adjustEvent.addCallbackParameter("status", $"{status}");
        adjustEvent.addCallbackParameter("level_number_end", $"{levelNumber}");
        adjustEvent.addCallbackParameter("result", $"{result}");
        adjustEvent.addCallbackParameter("level_playtime", $"{Mathf.RoundToInt((float)secs)}");
        adjustEvent.addCallbackParameter("if_continue", $"{if_continue}");
        adjustEvent.addCallbackParameter("coins_amount", $"{coinsAmount}");
        if (moves >= 0) adjustEvent.addCallbackParameter("moves", $"{moves}");

        Adjust.trackEvent(adjustEvent);

        TrackLevelEvents(status, levelNumber);
    }
    public static void SendLevelDataStartEvent(int leveNumberStart, int coinsAmount, string purchaseIDs)
    {
        MoonSDKSettings settings = MoonSDKSettings.Load();

        PlaytimeCalculator.levelStartDateTime = DateTime.Now;
        AdjustEvent adjustEvent = new AdjustEvent(settings.adjustLevelDataStartEventToken);

        if (PlaytimeCalculator.levelEndDateTime == default(DateTime))
        {
            TimeSpan timeSpan = DateTime.Now - PlaytimeCalculator.sessionStartDateTime;
            PlaytimeCalculator.notLevelTotalTime = timeSpan.TotalSeconds;
        }
        else
        {
            TimeSpan timeSpan = PlaytimeCalculator.levelStartDateTime - PlaytimeCalculator.levelEndDateTime;
            PlaytimeCalculator.notLevelTotalTime = timeSpan.TotalSeconds;
        }
        adjustEvent.addCallbackParameter("level_number_start", leveNumberStart.ToString());
        adjustEvent.addCallbackParameter("time_in_store", Mathf.RoundToInt((float)PlaytimeCalculator.totalTimeInStore).ToString());
        adjustEvent.addCallbackParameter("not_level_time", Mathf.RoundToInt((float)PlaytimeCalculator.notLevelTotalTime).ToString());
        adjustEvent.addCallbackParameter("coins_amount", coinsAmount.ToString());
        adjustEvent.addCallbackParameter("last_purchase_ID", purchaseIDs);
        if(PlaytimeCalculator.sessionNumber == 1) adjustEvent.addCallbackParameter("moon_sdk_version", Version);

        PlaytimeCalculator.totalTimeInStore = 0;
        PlaytimeCalculator.notLevelTotalTime = 0;

        TrackLevelEvents(LevelStatus.start, leveNumberStart);
        Adjust.trackEvent(adjustEvent);
    }
}
public enum LevelStatus
{
    start,
    fail,
    complete
}
public enum LevelResult
{
    none,
    win,
    fail
}
public enum iAPType
{
    product,
    subscription
}