using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsCounter;

    private const int UpdateRate = 4; // 4 updates per sec.

    private int   _frameCount;
    private float _deltaTime;
    private float _fps;

    private void LateUpdate()
    {
        _deltaTime += Time.unscaledDeltaTime;
        _frameCount++;

        if (_deltaTime > 1f / UpdateRate)
        {
            _fps = _frameCount / _deltaTime;

            fpsCounter.text = $"FPS: {Mathf.RoundToInt(_fps)}";

            _deltaTime = 0f;
            _frameCount = 0;
        }
    }
}
