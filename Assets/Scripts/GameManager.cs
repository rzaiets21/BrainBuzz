using System;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private const string PlayerPrefsLevelKey = "GroupData";
    private const string GameStreakKey = "CompletedGameStreak";

    [SerializeField] private TextAsset[] textAssets;
    
    [SerializeField] private AudioClip[] keyboardClick;
    [SerializeField] private ScreensController screensController;
    [SerializeField] private LoadingScreen loadingScreen;
    
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    
    // [SerializeField] private Image taskImage;
    [SerializeField] private TextMeshProUGUI questionText;
    
    [SerializeField] private LettersArea lettersArea;
    [SerializeField] private Keyboard keyboard;

    private GroupData _groupData;
    private LevelData _levelData;
    
    private bool _isInit;
    private int _currentGroup;

    private int currentGroup
    {
        get => _currentGroup;
        set
        {
            _currentGroup = value;

            if (_currentGroup > textAssets.Length)
            {
                _currentGroup = 1;
                for (int i = 0; i < textAssets.Length; i++)
                {
                    PlayerPrefs.DeleteKey($"Completed-Group_{i + 1}/Level_{1}");
                    PlayerPrefs.DeleteKey($"Completed-Group_{i + 1}/Level_{2}");
                    PlayerPrefs.DeleteKey($"Completed-Group_{i + 1}/Level_{3}");
                    PlayerPrefs.DeleteKey($"Completed-Group_{i + 1}/Level_{4}");
                    PlayerPrefs.DeleteKey($"Completed-Group_{i + 1}/Level_{5}");
                    
                    PlayerPrefs.DeleteKey($"Group_{i + 1}/Level_{1}");
                    PlayerPrefs.DeleteKey($"Group_{i + 1}/Level_{2}");
                    PlayerPrefs.DeleteKey($"Group_{i + 1}/Level_{3}");
                    PlayerPrefs.DeleteKey($"Group_{i + 1}/Level_{4}");
                    PlayerPrefs.DeleteKey($"Group_{i + 1}/Level_{5}");
                }
            }
        }
    }
    
    public int CurrentGroup => _currentGroup;
    public int CurrentLevel => _levelData.LevelId;
    private int _currentGameStreak;

    public LevelData LevelData => _levelData;
    
    public GroupData GetGroupData(bool update = false)
    {
        if(!_isInit)
            Init();
        
        if(_groupData == null || (update && _groupData.GroupId != currentGroup))
        {
            var json = textAssets[currentGroup - 1].text;
            
            // using (FileStream fs = new FileStream($"jar:file://{Application.dataPath}!/assets//Resources/Levels/Level_{currentLevel}.json", FileMode.Open))
            // {
            //     using (StreamReader reader = new StreamReader(fs))
            //     {
            //         json = reader.ReadToEnd();
            //     }
            // }

            _groupData = JsonConvert.DeserializeObject<GroupData>(json);
        }

        return _groupData;
    }
    
    public void LoadGame()
    {
        LoadLevel(currentGroup);
    }

    public void StartGame()
    {
        lettersArea.ShowGame();
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if(_isInit)
            return;
        
        if (currentGroup <= 0)
        {
            currentGroup = PlayerPrefs.GetInt(PlayerPrefsLevelKey, 1);
        }
        
        var json = textAssets[currentGroup - 1].text;
        _groupData = JsonConvert.DeserializeObject<GroupData>(json);

        _currentGameStreak = PlayerPrefs.GetInt(GameStreakKey, 0);

        _isInit = true;
    }
    
    private void OnEnable()
    {
        leftButton.onClick.AddListener(OnClickToLeftButton);
        rightButton.onClick.AddListener(OnClickToRightButton);
        
        lettersArea.OnLineSelected += OnLineSelected;
        lettersArea.OnGameCompleted += OnLevelCompleted;
        keyboard.OnKeyboardButtonUp += OnKeyboardButtonUp;
        keyboard.OnKeyboardButtonDown += OnKeyboardButtonDown;
    }

    private void OnDisable()
    {
        leftButton.onClick.RemoveListener(OnClickToLeftButton);
        rightButton.onClick.RemoveListener(OnClickToRightButton);
        
        lettersArea.OnLineSelected -= OnLineSelected;
        lettersArea.OnGameCompleted -= OnLevelCompleted;
        keyboard.OnKeyboardButtonUp -= OnKeyboardButtonUp;
        keyboard.OnKeyboardButtonDown -= OnKeyboardButtonDown;
    }

    private void OnLevelCompleted(bool stage)
    {
        if(stage)
        {
            Analytics.LogEvent(Analytics.LevelCompleted, currentGroup);
            
            if(_groupData.IsCompleted())
            {
                currentGroup++;
                PlayerPrefs.SetInt(PlayerPrefsLevelKey, currentGroup);
            }

            _currentGameStreak = PlayerPrefs.GetInt(GameStreakKey, 0);
            _currentGameStreak++;
            PlayerPrefs.SetInt(GameStreakKey, _currentGameStreak);
            
            return;
        }
        
        screensController.ShowScreen(Screens.Complete, ScreenTransition.RightToLeft);
    }

    private void ClearLevel()
    {
        lettersArea.Clear();
    }
    
    private void OnLineSelected(LettersLine line)
    {
        var index = line.LineIndex;
        var isVerticalLine = index == _levelData.Questions.Count - 1;
        var question = string.Empty;
        if (!isVerticalLine)
        {
            question = _levelData.Questions[index].Question;
            questionText.text = question.FirstCharacterToUpper();
            return;
        }
        
        foreach (var letterHolderBase in line.Letters)
        {
            question += letterHolderBase.IsComplete() ? letterHolderBase.TargetLetter.ToString().ToUpper() : "_";
        }

        questionText.text = question;
        // var imageUrl = _levelData.Questions[index].ImageUrl;
        // storageController.Get(imageUrl, sprite =>
        // {
        //     taskImage.sprite = sprite;
        // });
    }
    
    private void SelectNext(int direction = 1)
    {
        if(direction > 0)
            lettersArea.SelectNextLine();
        else 
            lettersArea.SelectPreviousLine();
    }
    
    public void LoadLevel(int level)
    {
        LoadLevel(_groupData.Levels[level]);
    }
    
    public void LoadNextLevel()
    {
        GetGroupData(true);
        var levelData = _groupData.Levels.FirstOrDefault(x => !x.IsCompleted());
        if (levelData == null)
        {
            currentGroup++;
            LoadNextLevel();
            return;
        }
        LoadLevel(levelData);
    }

    public void LoadLevel(LevelData levelData)
    {
        _levelData = levelData;
        lettersArea.Init(_levelData);
    }

    private void OnClickToLeftButton()
    {
        SelectNext(-1);
    }

    private void OnClickToRightButton()
    {
        SelectNext();
    }
    
    private void OnKeyboardButtonUp(KeyboardButtonType keyboardButtonType)
    {
        switch (keyboardButtonType)
        {
            case KeyboardButtonType.None:
                throw new NullReferenceException();
            case KeyboardButtonType.Backspace:
                lettersArea.ClearSelectedLetter();
                break;
            default:
                var letter = keyboardButtonType.ToString();
                lettersArea.SetLetter(letter[0]);
                break;
        }
        
        AudioController.Instance.Play(SoundType.Sounds, keyboardClick[Random.Range(0, keyboardClick.Length)]);
    }
    
    private void OnKeyboardButtonDown(KeyboardButtonType keyboardButtonType)
    {
        if(keyboardButtonType != KeyboardButtonType.Backspace)
            return;
        
        lettersArea.ClearLine();
    }
}
