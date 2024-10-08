using System;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using UnityEngine;

public class Analytics : MonoBehaviour
{
    public static string LevelStartKey = "level_start";
    public static string LevelCompletedKey = "level_completed";
    public static string RewardAmount = "reward_amount";
    public static string InterAmount = "interstitial_amount";
    public static string RateQuestion = "rate_question";

    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
    }

    public static void SetContinueLevel(bool value)
    {
        PlayerPrefs.SetInt("isContinueLevel", value? 1 : 0);
        PlayerPrefs.Save();
    }
    
    public static void SaveLastPurchase(string value)
    {
        PlayerPrefs.SetString("LastPurchase", value);
        PlayerPrefs.Save();
    }

    public static void LogEvent(string @event, int param)
    {
        Debug.Log("Log event");
        FirebaseAnalytics.LogEvent(@event, new Parameter("value", param));
    }

    public static void LogEvent(string @event, string param)
    {
        Debug.Log("Log event");
        FirebaseAnalytics.LogEvent(@event, new Parameter("value", param));
    }

    public static void LevelStart(int value)
    {
        FirebaseAnalytics.LogEvent(LevelStartKey, new Parameter("value", value));
        MoonSDK.SendLevelDataStartEvent(value, PlayerInventory.Instance._coins, GetLastPurchase());
    }

    public static void LevelCompleted(bool win, int value)
    {
        var levelStatus = win ? LevelStatus.complete : LevelStatus.fail;
        FirebaseAnalytics.LogEvent(LevelCompletedKey, new Parameter("value", value));
        MoonSDK.SendLevelDataCompleteEvent(levelStatus, value, LevelResult.win, IsContinueLevel(), PlayerInventory.Instance._coins);
    }

    private static string GetLastPurchase()
    {
        var lastPurchase = PlayerPrefs.GetString("LastPurchase", String.Empty);
        SaveLastPurchase(String.Empty);
        return lastPurchase;
    }

    private static bool IsContinueLevel()
    {
        return PlayerPrefs.GetInt("isContinueLevel", 0) == 1;
    }
}