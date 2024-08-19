using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using UnityEngine;

public class Analytics : MonoBehaviour
{
    public static string LevelCompleted = "level_completed";
    public static string RewardAmount = "reward_amount";
    public static string InterAmount = "interstitial_amount";
    public static string RateQuestion = "rate_question";
    
    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if(dependencyStatus == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
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
}