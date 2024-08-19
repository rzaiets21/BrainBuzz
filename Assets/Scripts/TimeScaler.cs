using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaler : MonoBehaviour
{
    [Range(0, 1), SerializeField] private float scaleTime = 1f;

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Init()
    {
        new GameObject("Time Scaler").AddComponent<TimeScaler>();
    }
#endif
    
    void Update()
    {
        Time.timeScale = scaleTime;
    }
}
