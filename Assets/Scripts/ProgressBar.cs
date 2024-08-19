using System;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private RectTransform fillArea;
    [SerializeField] private RectTransform fillImage;

    private float _fillSize;
    
    private bool _isInit;
    
    [SerializeField, Range(0, 1)] private float _value;

    public float Value
    {
        get => _value;
        private set
        {
            _value = value;
            FillImage();
        }
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if(_isInit)
            return;

        _fillSize = fillArea.rect.width;
        _isInit = true;
    }
    
    /// <summary>
    /// Set progress bar value
    /// </summary>
    /// <param name="value">Must be from 0 to 1</param>
    public void SetValue(float value)
    {
        if(!_isInit)
            Init();
        
        Value = Mathf.Clamp(value, 0, 1);
    }

    private void FillImage()
    {
        if(!_isInit)
            Init();
        
        var currentSize = fillImage.sizeDelta;
        currentSize.x = Value * _fillSize;
        fillImage.sizeDelta = currentSize;
    }
}
