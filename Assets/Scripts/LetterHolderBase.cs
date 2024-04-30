using System;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LetterHolderBase : MonoBehaviour
{
    private const char Empty = ' ';
    
    [SerializeField] private Color textColor;
    [SerializeField] private Color completeTextColor;
    
    [SerializeField] protected Image baseImage;
    [SerializeField] private TextMeshProUGUI text;

    private char _targetLetter;
    private char _currentLetter;
    private HolderColor _defaultColor;
    private HolderColor _currentColor;
    private HolderColor _previousColor;

    private bool _isComplete;

    public bool Revealing;
    
    public int LineIndex { get; private set; }
    public int Index { get; private set; }

    private RectTransform _rect;

    public RectTransform rect
    {
        get
        {
            if(_rect == null)
                _rect = GetComponent<RectTransform>();

            return _rect;
        }
    }
    
    public HolderColor CurrentColor => _currentColor;

    public char TargetLetter => _targetLetter;
    
    public event Action<LetterHolderBase> onClick;
    public event Action<LetterHolderBase> onLetterComplete;

    public void Select()
    {
        onClick?.Invoke(this);
    }
    
    public void Init(int lineIndex, int index, char targetLetter, HolderColor defaultColor)
    {
        LineIndex = lineIndex;
        Index = index;
        _targetLetter = targetLetter;
        _defaultColor = defaultColor;

        _currentLetter = Empty;
        text.text = string.Empty;
        
        text.color = textColor;
        
        SetColor(defaultColor);

        OnInit();
    }

    protected virtual void OnInit()
    {
        
    }
    
    public void RestoreColor()
    {
        SetColor(_previousColor);
    }
    
    public void SetColor(HolderColor color)
    {
        _previousColor = _currentColor;
        _currentColor = color;
        OnColorChanged();
    }

    protected virtual void OnColorChanged(){ }
    
    public bool CheckIsComplete()
    {
        if (_currentLetter != _targetLetter) return false;
        
        return true;
    }

    public void RevealLetter(bool showParticles, bool checkComplete = true)
    {
        if(checkComplete && _isComplete)
            return;
        
        SetLetterText(_targetLetter);
        SetComplete(false);
        
        if(showParticles)
            ShowRevealParticles();
    }
    
    public virtual void SetComplete(bool showParticle, bool lastLetterEdited = false)
    {
        _isComplete = true;
        text.color = completeTextColor;
        
        onLetterComplete?.Invoke(this);
    }

    public virtual void ShowFinalParticle(bool lastLetterEdited = false)
    {

    }

    public virtual void ShowCompleteParticle()
    {

    }

    public virtual void ShowRevealParticles()
    {

    }
    
    public void SetLetterText(char letter)
    {
        if(IsComplete())
            return;
        
        _currentLetter = letter;
        text.text = _currentLetter.ToString();
    }

    public void SetDefaultColor()
    {
        SetColor(_defaultColor);
    }
    
    public void Clear()
    {
        _currentLetter = Empty;
        text.text = string.Empty;
    }

    public bool IsEmpty()
    {
        return _currentLetter == Empty;
    }

    public bool IsComplete() => _isComplete;
}

public enum HolderColor
{
    None = 0,
    Selected,
    SelectedLine,
    Default,
    LetterPowerup,
    KeyboardPowerup,
    RevealLettersPowerup,
    LinePowerup,
    InCorrect,
    VerticalLine
}