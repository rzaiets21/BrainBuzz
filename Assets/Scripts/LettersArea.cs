using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CandyCoded.HapticFeedback.Android;
using DG.Tweening;
using Extensions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LettersArea : MonoBehaviour
{
    private string SavedModelKey => $"Group_{_levelData.GroupId}/Level_{_levelData.LevelId}";
    
    [SerializeField] private PowerupsController powerupsController;
    
    [SerializeField] private float focusSpeed;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private TileParticlesController tileParticlesController;
    [SerializeField] private RevealLettersController revealLettersController;
    
    [SerializeField] private RectTransform lettersContainer;
    
    [SerializeField] private RectTransform selectedRect;
    
    [SerializeField] private LetterHolderBase letterHolderPrefab;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;

    [SerializeField] private Keyboard keyboard;

    [SerializeField] private AudioClip letterHolderClick;
    [SerializeField] private AudioClip keyboardClick;
    [SerializeField] private AudioClip verticalLineCompleted;
    [SerializeField] private AudioClip[] completedLineSounds;
    [SerializeField] private AudioClip incorrectLineSound;
    [SerializeField] private AudioClip levelCompleted;
    
    private LettersLine[] _lines;

    private LetterHolderBase _selectedLetterHolder;
    private LettersLine _selectedLine;
    private LettersLine _verticalLine;

    private Coroutine _focusCoroutine;

    private Dictionary<int, bool> _checkingLinesCoroutines;

    private bool _interact = false;
    private int _currentLevel;

    private SavedLevelModel _savedLevelModel;
    
    public event Action<LetterHolderBase> onLetterHolderClick;
    public event Action<LettersLine> OnLineSelected;
    public event Action<bool> OnGameCompleted;

    private Coroutine _showGameCoroutine;
    private Coroutine _clearLineCoroutine;

    private LevelData _levelData;
    
    public bool Interact => _interact;
    
    private void Awake()
    {
        _checkingLinesCoroutines = new Dictionary<int, bool>();
    }

    private void OnDestroy()
    {

    }

    private void SaveGameInfo()
    {
        if(_savedLevelModel == null)
            return;
        
        var json = JsonConvert.SerializeObject(_savedLevelModel);
        PlayerPrefs.SetString(SavedModelKey, json);
    }
    
    private void LoadSavedInfo()
    {
        var json = PlayerPrefs.GetString(SavedModelKey, string.Empty);
        if (string.IsNullOrWhiteSpace(json))
        {
            _savedLevelModel = null;
            return;
        }

        _savedLevelModel = JsonConvert.DeserializeObject<SavedLevelModel>(json);
    }
    
    public void Init(LevelData levelData)
    {
        if(_levelData == levelData)
            return;
        
        _levelData = levelData;
        
        LoadSavedInfo();
        
        if (_lines is { Length: > 0 })
        {
            Clear();
        }
        
        _currentLevel = levelData.LevelId;

        var hasSaves = _savedLevelModel != null && _savedLevelModel.LevelId == _currentLevel;
        
        var linesCount = levelData.Questions.Count;

        _lines = new LettersLine[linesCount];
        LettersLine verticalLine = null;

        if (!hasSaves)
            _savedLevelModel = new SavedLevelModel(_currentLevel, linesCount - 1, levelData.Questions[0].Answer.Length);
        
        for (int i = 0; i < linesCount; i++)
        {
            var level = levelData.Questions[i];

            var lineIndex = i;

            var answer = level.Answer.Replace(" ", "");
            var levelImage = level.LevelImage - 1;

            var lettersCount = answer.Length;
            
            var line = _lines[i] = new LettersLine(lettersCount, i);
            
            if(level.LevelImage <= 0)
            {
                verticalLine = line;
                continue;
            }
            
            for (int l = 0; l < answer.Length; l++)
            {
                var letterHolder = Instantiate(letterHolderPrefab, lettersContainer);
                line.Letters[l] = letterHolder;

                var defaultColor = l == levelImage ? HolderColor.VerticalLine : HolderColor.Default;
                letterHolder.Init(lineIndex, l, answer[l], defaultColor);
                letterHolder.onClick += OnLetterHolderClick;
                letterHolder.onLetterComplete += OnLetterComplete;

                if (hasSaves)
                {
                    var isComplete = _savedLevelModel.LinesInfo[i, l] == 1;
                    if(isComplete)
                    {
                        letterHolder.RevealLetter(false);
                    }
                }
            }
        }

        if (verticalLine != null)
        {
            for (int i = 0; i < linesCount; i++)
            {
                var level = levelData.Questions[i];
                var levelImage = level.LevelImage - 1;
                
                if(levelImage < 0)
                {
                    continue;
                }

                verticalLine.Letters[i] = _lines[i].Letters[levelImage];
            }

            _verticalLine = verticalLine;
        }

        selectedRect.gameObject.SetActive(false);
        
        CalculateGridSize();
        AdjustSlotsPositions();
        
        tileParticlesController.Init(gridLayoutGroup.cellSize);
        revealLettersController.Init(gridLayoutGroup.cellSize);
        
        keyboard.SetInteractable(true);
        _interact = true;
    }

    public void ShowGame()
    {
        SelectLine(focus: false);
        SelectLetterHolder(_selectedLetterHolder != null && _selectedLetterHolder.LineIndex == _selectedLine.LineIndex ? _selectedLetterHolder.Index : -1);
        _showGameCoroutine = StartCoroutine(ShowGameCoroutine());
    }

    private IEnumerator ShowGameCoroutine()
    {
        scrollRect.normalizedPosition = Vector2.one;
        var lastLetterHolder = _lines[^2].Letters[0];
        
        yield return new WaitForSeconds(0.5f);
        
        yield return StartCoroutine(scrollRect.FocusOnItemCoroutine(lastLetterHolder.rect, focusSpeed));
        yield return new WaitForSeconds(0.6f);
        
        yield return StartCoroutine(scrollRect.FocusOnItemCoroutine(_selectedLetterHolder.rect, focusSpeed));

        _showGameCoroutine = null;
    }
    
    private void CalculateGridSize()
    {
        var size = gridLayoutGroup.cellSize;
        var selectedSize = selectedRect.sizeDelta;
        var ratio = size.y / size.x;

        var oldWidth = size.x;

        var firstLine = _lines[0];
        
        var lettersCount = firstLine.Letters.Length;
        size.x = (lettersContainer.rect.width - gridLayoutGroup.padding.left - gridLayoutGroup.padding.right
                  - (gridLayoutGroup.spacing.x * (lettersCount - 1))) / lettersCount;
        size.y = size.x * ratio;

        selectedSize *= (size.x / oldWidth);
        gridLayoutGroup.cellSize = size;
        selectedRect.sizeDelta = selectedSize;
    }
    
    private void AdjustSlotsPositions()
    {
        var size = gridLayoutGroup.cellSize;

        var letterContainerSize = new Vector2(0, 0);
        var linesCount = _lines.Length - 1;

        letterContainerSize.y = gridLayoutGroup.padding.top + gridLayoutGroup.padding.bottom +
                                gridLayoutGroup.spacing.y * (linesCount - 1) + size.y * linesCount;

        var firstLine = _lines[0];
        var lettersCount = firstLine.Letters.Length;
        lettersContainer.sizeDelta = letterContainerSize;

        var startY = -size.y / 2 - gridLayoutGroup.padding.top;
        for (int i = 0; i < linesCount; i++)
        {
            var startX = size.x / 2 + gridLayoutGroup.padding.left;
            var line = _lines[i];
            for (int j = 0; j < lettersCount; j++)
            {
                var holder = line.Letters[j];
                holder.rect.sizeDelta = size;
                holder.rect.anchoredPosition = new Vector2(startX, startY);

                startX += gridLayoutGroup.spacing.x + size.x;
            }
            
            var temp = startY - gridLayoutGroup.spacing.y - size.y;
            startY = temp;
        }
    }

    private void OnEnable()
    {
        powerupsController.onPowerupSelected += OnPowerupSelected;
    }

    private void OnDisable()
    {
        if (_lines != null)
            foreach (var line in _lines)
            {
                if (line.Letters == null)
                    continue;

                foreach (var letterHolder in line.Letters)
                {
                    letterHolder.onClick -= OnLetterHolderClick;
                    letterHolder.onLetterComplete -= OnLetterComplete;
                }
            }

        powerupsController.onPowerupSelected -= OnPowerupSelected;
        
        if(_lines == null || _lines.All(x => x.IsComplete()))
            return;
        SaveGameInfo();
    }

    public LettersLine GetLine(int index)
    {
        return _lines[index];
    }
    
    public bool TryUsePowerup(PowerupType powerupType, LetterHolderBase target)
    {
        return UsePowerup(powerupType, target);
    }
    
    private bool UsePowerup(PowerupType powerupType, LetterHolderBase target)
    {
        LettersLine line = null;
        
        switch (powerupType)
        {
            case PowerupType.Letter:
                if (target.IsComplete())
                {
                    return false;
                }
#if UNITY_ANDROID
                if(Settings.HapticsIsOn)
                    HapticFeedback.PerformHapticFeedback(HapticFeedbackConstants.KEYBOARD_PRESS);
#endif
                target.RevealLetter(false);
                
                line = _lines[target.LineIndex];
                
                if(target == _selectedLetterHolder)
                {
                    _selectedLetterHolder.SetColor(HolderColor.SelectedLine);
                    
                    if(!line.HasEmptyLetter())
                        CheckLineComplete(line);
                    else
                        SelectNextLetterHolder();
                }
                else if(!line.HasEmptyLetter())
                        CheckLineComplete(line);
                return true;
            case PowerupType.Random:
                var letters = _lines.SelectMany(x => x.Letters.Where(l => !l.IsComplete())).ToArray();
                var length = letters.Length;
                var min = Mathf.Clamp(Random.Range(5, 9), 0, length);
                var randomLetters = letters.GetRandomElements(min);
                var rectTransform = scrollRect.GetComponent<RectTransform>();
                var position = rectTransform.TransformPoint(rectTransform.anchoredPosition);
                
                foreach (var randomLetter in randomLetters)
                {
                    var letter = randomLetter.TargetLetter;
                    revealLettersController.RevealLetter(randomLetter, letter, randomLetter, () =>
                    {
#if UNITY_ANDROID
                        if(Settings.HapticsIsOn)
                            HapticFeedback.PerformHapticFeedback(HapticFeedbackConstants.KEYBOARD_PRESS);
#endif
                        var holderLine = _lines[randomLetter.LineIndex];
                        if(!holderLine.HasEmptyLetter())
                            CheckLineComplete(holderLine);
                    });
                }
                return true;
            case PowerupType.Keyboard:
                var chars = new List<char>();
                var filterLine = _lines[target.LineIndex];

                if (filterLine.IsComplete())
                    return false;
                
                foreach (var letter in filterLine.Letters)
                {
                    if(chars.Contains(letter.TargetLetter))
                        continue;
                    
                    chars.Add(letter.TargetLetter);
                }
                keyboard.AddFilter(target.LineIndex, chars);

#if UNITY_ANDROID
                if(Settings.HapticsIsOn)
                    HapticFeedback.PerformHapticFeedback(HapticFeedbackConstants.KEYBOARD_RELEASE);
#endif
                
                SelectLine(filterLine);
                SelectLetterHolder();
                return true;
            case PowerupType.Line:
#if UNITY_ANDROID
                if(Settings.HapticsIsOn)
                    HapticFeedback.PerformHapticFeedback(HapticFeedbackConstants.KEYBOARD_RELEASE);
#endif                
                line = _lines[target.LineIndex];

                if (line.IsComplete())
                    return false;
                
                line.Reveal();
                CheckLineComplete(line);
                return true;
            default:
                return false;
        }
    }
    
    public void SetLetter(char letter)
    {
        if(_selectedLetterHolder == null)
            return;
        
        if(!_interact)
            return;

        if (_selectedLetterHolder.IsComplete())
        {
            if (_selectedLine.IsComplete())
            {
                SelectNextLine();
                return;
            }
            
            SelectNextLetterHolder();
            return;
        }

        if (!scrollRect.InFocus(_selectedLetterHolder.rect))
        {
            if (_focusCoroutine != null)
            {
                StopCoroutine(_focusCoroutine);
            }

            _focusCoroutine = StartCoroutine(scrollRect.FocusOnItemCoroutine(_selectedLetterHolder.rect, focusSpeed * 2));
        }
        
        _selectedLetterHolder.SetLetterText(letter);
        OnSetLetter();
    }

    public void ClearSelectedLetter()
    {
        if(!_interact)
            return;
        
        if (_clearLineCoroutine != null)
        {
            StopCoroutine(_clearLineCoroutine);
            _clearLineCoroutine = null;
        }
        
        if(_selectedLetterHolder == null)
            return;

        if (_selectedLetterHolder.IsEmpty())
        {
            SelectPreviousLetterHolder();
        }
        
        _selectedLetterHolder.Clear();
    }

    public void ClearLine()
    {
        if (_clearLineCoroutine != null)
        {
            StopCoroutine(_clearLineCoroutine);
            _clearLineCoroutine = null;
        }
        
        if(_selectedLetterHolder == null)
            return;

        _clearLineCoroutine = StartCoroutine(ClearLineCoroutine());
    }

    private IEnumerator ClearLineCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        var lastHolder = _selectedLetterHolder;
        _selectedLetterHolder.Clear();
        
        AudioController.Instance.Play(SoundType.Sounds, keyboardClick);

        var itsDifferent = true;
        
        while (itsDifferent)
        {
            yield return new WaitForSeconds(0.01f);
            
            SelectPreviousLetterHolder();
            _selectedLetterHolder.Clear();
            lastHolder = _selectedLetterHolder;
            itsDifferent = lastHolder == _selectedLetterHolder;
            AudioController.Instance.Play(SoundType.Sounds, keyboardClick);
            yield return null;
        }
        
        _clearLineCoroutine = null;
    }
    
    private void CheckLineComplete(LettersLine line, LetterHolderBase letterHolder = null, bool revealLetters = true)
    {
        if (_checkingLinesCoroutines.TryGetValue(line.LineIndex, out var state) && state)
            return;
        
        StartCoroutine(CheckCompleteCoroutine(line, letterHolder, revealLetters));
        // foreach (var letterHolder in _selectedLine.Letters)
        // {
        //     if (!letterHolder.CheckIsComplete())
        //     {
        //         letterHolder.Clear();
        //         continue;
        //     }
        //     
        //     letterHolder.SetComplete();
        // }
        
        // if(_selectedLine.IsComplete())
        //     SelectNextLine();
        // else
        //     SelectLetterHolder();
    }

    [ContextMenu("Complete game animation")]
    private void CompleteGame()
    {
        PlayerPrefs.SetInt($"Completed-Group_{_levelData.GroupId}/Level_{_levelData.LevelId}", 1);
        SaveGameInfo();
        keyboard.SetInteractable(false);
        keyboard.ClearFilters();
        
        //selectedRect.SetParent(transform);
        //selectedRect.gameObject.SetActive(false);
        _interact = false;
        _selectedLetterHolder = null;
        _selectedLine = null;
        
        StartCoroutine(CompleteGameCoroutine());
    }

    private IEnumerator CompleteGameCoroutine()
    {
        OnGameCompleted?.Invoke(true);
        yield return new WaitForSeconds(0.25f);
        
        var firstLetterHolder = _lines[0].Letters[0];
        var firstLetterRect = firstLetterHolder.GetComponent<RectTransform>();
        yield return StartCoroutine(scrollRect.FocusOnItemCoroutine(firstLetterRect, focusSpeed - 1.5f));
        yield return new WaitForSeconds(0.15f);

        var animationCompleted = false;
        var list = new List<(int, int)>();
        list.Add(new ValueTuple<int, int>(0, 0));

        AudioController.Instance.Play(SoundType.Sounds, levelCompleted);
        var time = levelCompleted.length;
        
        if (!_verticalLine.Contains(firstLetterHolder))
        {
            //animation
            //firstLetterHolder.SetColor(HolderColor.Selected);
            firstLetterHolder.ShowCompleteParticle();
        }
        else
        {
            firstLetterHolder.transform.DOScale(1.1f, 0.35f);
        }

        var lastLetterHolder = _lines[^2].Letters[^1];
        var lastLetterRect = lastLetterHolder.GetComponent<RectTransform>();
        //StartCoroutine(scrollRect.FocusOnItemCoroutine(lastLetterRect, 0.75f, 0f, OnCompleteFinishAnimation));
        StartCoroutine(scrollRect.FocusOnItemByTimeCoroutine(lastLetterRect, time / 1.25f, 0f, null));

        var delay = (time / 2.5f) / (_lines.Length - 1);
        
        while (!animationCompleted)
        {
            yield return new WaitForSeconds(delay);
            
            var listCopy = list.ToArray();
            list.Clear(); 
            
            for (int i = 0; i < listCopy.Length; i++)
            {
                var pair = listCopy[i];

                var right = pair.Item2 + 1 >= _lines[pair.Item1].Letters.Length ? null : _lines[pair.Item1].Letters[pair.Item2 + 1];
                var down = pair.Item1 + 1 >= _lines.Length - 1 ? null : _lines[pair.Item1 + 1].Letters[pair.Item2];

                if(right != null) 
                {
                    right.ShowCompleteParticle();
                    if (_verticalLine.Contains(right))
                    {
                        right.transform.DOScale(1.1f, 0.35f);
                        //animation
                        //right.SetColor(HolderColor.Selected);
                    }
                    
                    var nextRightLetter = new ValueTuple<int, int>(right.LineIndex, right.Index);
                    if(!list.Contains(nextRightLetter))
                        list.Add(nextRightLetter);
                }

                if(down != null)
                {
                    down.ShowCompleteParticle();
                    if (_verticalLine.Contains(down))
                    {
                        //animation
                        down.transform.DOScale(1.1f, 0.35f);
                    }
                    
                    var nextDownLetter = new ValueTuple<int, int>(down.LineIndex, down.Index);
                    if(!list.Contains(nextDownLetter))
                        list.Add(nextDownLetter);
                }
            }

            if (list.Count == 0)
                animationCompleted = true;
        }

        yield return new WaitForSeconds(0.35f);
        OnCompleteFinishAnimation();
    }

    private void OnCompleteFinishAnimation()
    {
        OnGameCompleted?.Invoke(false);
    }

    public void Clear()
    {
        for (int i = 0; i < _lines.Length - 1; i++)
        {
            var line = _lines[i];
            foreach (var letterHolder in line.Letters)
            {
                Destroy(letterHolder.gameObject);
            }
        }

        _lines = null;
        
        _checkingLinesCoroutines.Clear();
    }

    private bool CheckGameComplete()
    {
        if(_lines.All(x => x.IsComplete()))
        {
            _selectedLine.SetDefaultColor();
            CompleteGame();
            Debug.Log("Game complete");
            return true;
        }

        return false;
    }
    
    public void SelectNextLine()
    {
        if(CheckGameComplete())
            return;
        
        _selectedLine.SetDefaultColor();
        
        var lastSelected = _selectedLine;
        var currentIndex = _selectedLine.LineIndex;
        var linesCount = _lines.Length;
        do
        {
            if (lastSelected == _verticalLine)
            {
                currentIndex = _selectedLetterHolder.LineIndex;
            }
            
            currentIndex++;

            if (currentIndex >= linesCount - 1)
                currentIndex = 0;

            lastSelected = _lines[currentIndex];
        } while (lastSelected.IsComplete());
        
        SelectLine(currentIndex);
        SelectLetterHolder();
    }

    public void SelectPreviousLine()
    {
        if(_lines.All(x => x.IsComplete()))
        {
            Debug.Log("Game complete");
            return;
        }
        
        _selectedLine.SetDefaultColor();
        
        var lastSelected = _selectedLine;
        var currentIndex = _selectedLine.LineIndex;
        var linesCount = _lines.Length;
        do
        {
            if (lastSelected == _verticalLine)
            {
                currentIndex = _selectedLetterHolder.LineIndex;
            }
            
            currentIndex--;

            if (currentIndex < 0)
                currentIndex = linesCount - 2;

            lastSelected = _lines[currentIndex];
        } while (lastSelected.IsComplete());
        
        SelectLine(currentIndex);
        SelectLetterHolder();
    }
    
    private void SelectNextLetterHolder()
    {
        var lettersCount = _selectedLine.Letters.Length;
        var currentIndex = _selectedLine == _verticalLine ? _selectedLetterHolder.LineIndex + 1 : _selectedLetterHolder.Index + 1;
            
        if (currentIndex >= lettersCount)
            currentIndex = 0;
        
        var lastSelected = _selectedLine.Letters[currentIndex];
        
        while (lastSelected.IsComplete() && _selectedLine.HasEmptyLetter())
        {
            currentIndex++;
            
            if (currentIndex >= lettersCount)
                currentIndex = 0;
            
            lastSelected = _selectedLine.Letters[currentIndex];
        }
        
        SelectLetterHolder(currentIndex);
    }

    private void SelectPreviousLetterHolder()
    {
        var currentIndex = _selectedLine == _verticalLine ? _selectedLetterHolder.LineIndex : _selectedLetterHolder.Index;
        LetterHolderBase lastSelected;

        var hasPreviousHolder = true;
        
        do
        {
            currentIndex--;

            if (currentIndex < 0)
            {
                hasPreviousHolder = false;
                currentIndex = 0;
            }

            lastSelected = _selectedLine.Letters[currentIndex];
        } while (lastSelected.IsComplete() && hasPreviousHolder);
        
        SelectLetterHolder(hasPreviousHolder ? currentIndex : -1);
    }
    
    private void OnSetLetter()
    {
        _selectedLetterHolder.SetColor(HolderColor.SelectedLine);

        if (_verticalLine.Contains(_selectedLetterHolder) && !_verticalLine.HasEmptyLetter())
        {
            CheckLineComplete(_verticalLine, _selectedLetterHolder, false);
        }
        
        if (!_selectedLine.HasEmptyLetter())
        {
            CheckLineComplete(_selectedLine);
            return;
        }

        SelectNextLetterHolder();
    }

    private void OnLetterComplete(LetterHolderBase letterHolder)
    {
        _savedLevelModel.LinesInfo[letterHolder.LineIndex, letterHolder.Index] = 1;
        
        SaveGameInfo();
    }
    
    private void OnLetterHolderClick(LetterHolderBase letterHolder)
    {
        if(powerupsController.SelectedPowerup)
        {
            onLetterHolderClick?.Invoke(letterHolder);
            return;
        }
        
        if(!_interact)
            return;

        AudioController.Instance.Play(SoundType.Sounds,letterHolderClick);
        
        if(letterHolder == _selectedLetterHolder)
        {
            if (!_verticalLine.Letters.Contains(letterHolder)) return;
            
            if (_verticalLine == _selectedLine)
            {
                SelectLine(letterHolder.LineIndex);
                SelectLetterHolder(letterHolder.Index);
                return;
            }
            
            SelectLine(_verticalLine);
            SelectLetterHolder(letterHolder);
            return;
        }
        
        SelectLine(letterHolder.LineIndex);
        SelectLetterHolder(letterHolder.Index);
    }

    private void SelectLetterHolder(int index = -1)
    {
        if(_selectedLine == null)
            return;

        if (index < 0)
            index = _selectedLine.Letters.FirstOrDefault(x => !x.IsComplete() && !x.Revealing)!.Index;

        var targetLetterHolder = _selectedLine.Letters[index];

        SelectLetterHolder(targetLetterHolder);
    }

    private void SelectLetterHolder(LetterHolderBase targetLetterHolder)
    {
        if (_selectedLetterHolder != null && _selectedLetterHolder != targetLetterHolder)
        {
            _selectedLetterHolder.SetColor(HolderColor.SelectedLine);
        }
        
        _selectedLetterHolder = targetLetterHolder;

        var select = !targetLetterHolder.IsComplete() || (_selectedLine == _verticalLine);
        
        selectedRect.gameObject.SetActive(false);
        if(select)
        {
            //selectedRect.SetParent(_selectedLetterHolder.transform);
            //selectedRect.SetAsFirstSibling();
            //selectedRect.anchoredPosition = Vector2.zero;
            _selectedLetterHolder.SetColor(HolderColor.Selected);
        }

        if (_selectedLine != _verticalLine) return;
        
        if (_focusCoroutine != null)
        {
            StopCoroutine(_focusCoroutine);
            _focusCoroutine = null;
        }

        _focusCoroutine = StartCoroutine(scrollRect.FocusOnItemCoroutine(_selectedLetterHolder.rect, focusSpeed));
    }
    
    private void SelectLine(int index = -1, bool focus = true)
    {
        if(index < 0)
        {
            for (int i = 0; i < _lines.Length; i++)
            {
                if (_lines[i].IsComplete())
                    continue;

                index = i;
                break;
            }
        }

        var targetLine = _lines[index];
        SelectLine(targetLine, focus);
    }

    private void SelectLine(LettersLine targetLine, bool focus = true)
    {
        if (_selectedLine != null && _selectedLine != targetLine)
        {
            _selectedLine.SetDefaultColor();
            _selectedLetterHolder = null;
        }
        
        _selectedLine = targetLine;
        foreach (var letterHolder in _selectedLine.Letters)
        {
            letterHolder.SetColor(HolderColor.SelectedLine);
        }

        if (_focusCoroutine != null)
        {
            StopCoroutine(_focusCoroutine);
            _focusCoroutine = null;
        }

        if(_selectedLine != _verticalLine && focus)
            _focusCoroutine = StartCoroutine(scrollRect.FocusOnItemCoroutine(_selectedLine.Letters[0].rect, focusSpeed, 0.1f));
        
        OnLineSelected?.Invoke(_selectedLine);
    }

    private IEnumerator CheckCompleteCoroutine(LettersLine line, LetterHolderBase letterHolder, bool revealLetters)
    {
        if (line == _verticalLine && _selectedLine != _verticalLine)
        {
            
        }
        else
        {
            _interact = false;
        }
        
        
        if (!_checkingLinesCoroutines.TryGetValue(line.LineIndex, out _))
        {
            _checkingLinesCoroutines.Add(line.LineIndex, true);
        }

        _checkingLinesCoroutines[line.LineIndex] = true;
        
        var inCorrectCount = line.Letters.Count(x => !x.CheckIsComplete());
        var inCompletedCount = line.Letters.Count(x => !x.IsComplete());
        var answerIsCorrect = line.Letters.All(x => x.CheckIsComplete());
        var setComplete = answerIsCorrect || (inCompletedCount > 4 && inCorrectCount < 3 && _currentLevel <= 5);
        
        var delay = new WaitForSeconds(0.02f);
        var checkDelay = new WaitForSeconds(0.035f);

        var isSelectedLine = line == _selectedLine;
        
        var startedHolder = isSelectedLine ? _selectedLetterHolder : letterHolder == null ? line.Letters[0] : letterHolder;

#if UNITY_ANDROID
        if(!answerIsCorrect)
            if(Settings.HapticsIsOn)
                HapticFeedback.PerformHapticFeedback(HapticFeedbackConstants.EDGE_RELEASE);
#endif
        // if(line != _verticalLine && setComplete)
        // {
        //     var lineIndex = line.LineIndex;
        //     for (int i = 0; i < line.Letters.Length; i++)
        //     {
        //         var letter = line.Letters[i];
        //         _savedLevelModel.LinesInfo[lineIndex, i] = (byte)(letter.CheckIsComplete() ? 1 : 0);
        //     }
        // }

        // if (!startedHolder.CheckIsComplete())
        // {
        //     startedHolder.SetColor(LetterHolderColor.inCorrect);
        // }
        if (setComplete && startedHolder.CheckIsComplete())
        {
            if(answerIsCorrect)
            {
                startedHolder.ShowFinalParticle(true);
                AudioController.Instance.Play(SoundType.Sounds, line == _verticalLine ? verticalLineCompleted : completedLineSounds[Random.Range(0, completedLineSounds.Length)]);
                yield return new WaitForSeconds(0.4f);
                if(revealLetters)
                    RevealLetter(startedHolder);
            }
            startedHolder.SetComplete(!answerIsCorrect, answerIsCorrect);
        }
        else
        {
            AudioController.Instance.Play(SoundType.Sounds, incorrectLineSound);
            startedHolder.SetColor(HolderColor.InCorrect);
        }

        if(answerIsCorrect)
            yield return checkDelay;
        
        yield return delay;
        
        var nextHolder = startedHolder;
        var prevHolder = startedHolder;

        while (line.Next(nextHolder) != null || line.Previous(prevHolder) != null)
        {
            nextHolder = line.Next(nextHolder);
            prevHolder = line.Previous(prevHolder);

            if (nextHolder != null)
            {
                if (setComplete && nextHolder.CheckIsComplete())
                {
                    if(answerIsCorrect)
                    {
                        nextHolder.ShowFinalParticle();
                        if(revealLetters)
                            RevealLetter(nextHolder);
                    }
                    nextHolder.SetComplete(!answerIsCorrect);
                }
                else
                {
                    nextHolder.SetColor(HolderColor.InCorrect);
                }
            }

            if (prevHolder != null)
            {
                if (setComplete && prevHolder.CheckIsComplete())
                {
                    if(answerIsCorrect)
                    {
                        prevHolder.ShowFinalParticle();
                        if(revealLetters)
                            RevealLetter(prevHolder);
                    }
                    prevHolder.SetComplete(!answerIsCorrect);
                }
                else
                {
                    prevHolder.SetColor(HolderColor.InCorrect);
                }
            }
            
            if(answerIsCorrect)
            {
                yield return checkDelay;
            }
            
            yield return delay;
            yield return null;
        }
        
        yield return delay;
        
        if(!answerIsCorrect)
        {
            if (!startedHolder.IsComplete())
                startedHolder.Clear();
            startedHolder.SetColor(HolderColor.SelectedLine);

            yield return delay;

            nextHolder = startedHolder;
            prevHolder = startedHolder;

            while (line.Next(nextHolder) != null || line.Previous(prevHolder) != null)
            {
                nextHolder = line.Next(nextHolder);
                prevHolder = line.Previous(prevHolder);

                if (nextHolder != null)
                {
                    if (!nextHolder.IsComplete())
                        nextHolder.Clear();
                    nextHolder.SetColor(HolderColor.SelectedLine);
                }

                if (prevHolder != null)
                {
                    if (!prevHolder.IsComplete())
                        prevHolder.Clear();
                    prevHolder.SetColor(HolderColor.SelectedLine);
                }
                
                yield return delay;
                yield return null;
            }
        }
        
        Debug.Log("Checking complete");
        _checkingLinesCoroutines[line.LineIndex] = false;
        
        if (isSelectedLine)
        {
            if(_selectedLine.IsComplete())
            {
                // startedHolder.ShowFinalParticle(true);
                // yield return delay;
                //
                // nextHolder = startedHolder;
                // prevHolder = startedHolder;
                //
                // while (_selectedLine.Next(nextHolder) != null || _selectedLine.Previous(prevHolder) != null)
                // {
                //     nextHolder = _selectedLine.Next(nextHolder);
                //     prevHolder = _selectedLine.Previous(prevHolder);
                //
                //     if (nextHolder != null)
                //     {
                //         nextHolder.ShowFinalParticle();
                //     }
                //
                //     if (prevHolder != null)
                //     {
                //         prevHolder.ShowFinalParticle();
                //     }
                //
                //     yield return delay;
                //     yield return delay;
                //     yield return null;
                // }
            
                SelectNextLine();
            }
            else
                SelectLetterHolder();

            _interact = _lines.Any(x => !x.IsComplete());
        }
        else
        {
            _interact = !CheckGameComplete();
        }
        
        yield return null;
    }

    private void RevealLetter(LetterHolderBase holder)
    {
        var lines = _lines.Where(x => x.LineIndex != _selectedLine.LineIndex && x.Letters.Count(l => !l.CheckIsComplete() && !l.Revealing) > 2).ToArray();
        if(!lines.Any())
            return;

        var letter = holder.TargetLetter;
        
        var targetLines = lines.Where(x => x.Letters.Any(l => l.TargetLetter == letter && !l.CheckIsComplete() && !l.Revealing)).ToArray();
        if(!targetLines.Any())
            return;

        var randomLine = targetLines.GetRandomElement();
        var randomLetterHolder = randomLine.Letters.GetRandomElement(l => l.TargetLetter == letter && !l.CheckIsComplete() && !l.Revealing);
        revealLettersController.RevealLetter(holder, letter, randomLetterHolder, () =>
        {
#if UNITY_ANDROID
            if(Settings.HapticsIsOn)
                HapticFeedback.PerformHapticFeedback(HapticFeedbackConstants.KEYBOARD_PRESS);
#endif
        });
    }

    private void OnPowerupSelected(bool state)
    {
        if(state && !_interact)
            return;

        //_interact = !state;
    }
}

[Serializable]
public class LettersLine
{
    public LetterHolderBase[] Letters;
    public readonly int LineIndex;

    public LettersLine()
    {
        
    }
    
    public LettersLine(int length, int lineIndex)
    {
        Letters = new LetterHolderBase[length];
        LineIndex = lineIndex;
    }

    public bool Contains(LetterHolderBase letterHolder)
    {
        return Letters.Any(x => x.Index == letterHolder.Index && x.LineIndex == letterHolder.LineIndex);
    }
    
    public bool HasEmptyLetter()
    {
        return Letters.Any(x => x.IsEmpty());
    }

    public bool IsComplete()
    {
        return Letters.All(x => x.IsComplete());
    }

    public void RestoreColor()
    {
        foreach (var letterHolder in Letters)
        {
            letterHolder.RestoreColor();
        }
    }

    public void SetColor(HolderColor color)
    {
        foreach (var letterHolder in Letters)
        {
            letterHolder.SetColor(color);
        }
    }
    
    public void SetDefaultColor()
    {
        if(Letters == null)
            return;
        
        foreach (var letterHolder in Letters)
        {
            letterHolder.SetDefaultColor();
        }
    }

    public void Reveal()
    {
        foreach (var letter in Letters)
        {
            letter.RevealLetter(false);
        }
    }
    
    public LetterHolderBase Next(LetterHolderBase letterHolder)
    {
        if (letterHolder == null || !Letters.Contains(letterHolder))
            return null;

        var index = Array.IndexOf(Letters, letterHolder) + 1;
        return index >= Letters.Length ? null : Letters[index];
    }

    public LetterHolderBase Previous(LetterHolderBase letterHolder)
    {
        if (letterHolder == null || !Letters.Contains(letterHolder))
            return null;

        
        var index = Array.IndexOf(Letters, letterHolder) - 1;
        return index < 0 ? null : Letters[index];
    }
}

[Serializable]
public class SavedLevelModel
{
    public int LevelId;
    public byte[,] LinesInfo;

    public SavedLevelModel()
    {
        
    }
    
    public SavedLevelModel(int levelId, int linesCount, int answerLength)
    {
        LevelId = levelId;
        LinesInfo = new byte[linesCount, answerLength];
    }
}