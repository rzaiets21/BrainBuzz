using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GroupLevel : MonoBehaviour
{
    private string SavedModelKey => $"Group_{_levelData.GroupId}/Level_{_levelData.LevelId}";
    
    [SerializeField] private GameObject progressObject;
    [SerializeField] private GameObject completeObject;

    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private Button playButton;

    [SerializeField] private ProgressBar progressBar;

    public event Action<LevelData> onPlayButtonClick;

    private LevelData _levelData;
    
    private void OnEnable()
    {
        playButton.onClick.AddListener(OnPlayButtonClick);
    }

    private void OnDisable()
    {
        playButton.onClick.AddListener(OnPlayButtonClick);
    }

    public void Init(LevelData levelData)
    {
        _levelData = levelData;

        var isCompleted = false;
        
        if (!PlayerPrefs.HasKey(SavedModelKey))
        {
            progressObject.gameObject.SetActive(true);
            completeObject.gameObject.SetActive(false);
            
            progressBar.SetValue(0.5f);
        }
        else
        {
            var savedDataRaw = PlayerPrefs.GetString(SavedModelKey);
            var savedData = JsonConvert.DeserializeObject<SavedLevelModel>(savedDataRaw);

            var linesCount = _levelData.Questions.Count - 1;
            var answerLength = _levelData.Questions[0].Answer.Length;
            
            var slotsCount = savedData.LinesInfo.LongLength;
            var completedSlots = 0;
            
            for (int i = 0; i < linesCount; i++)
            {
                for (int j = 0; j < answerLength; j++)
                {
                    if(savedData.LinesInfo[i, j] != 1)
                        continue;

                    completedSlots++;
                }
            }

            isCompleted = completedSlots == slotsCount;

            progressObject.gameObject.SetActive(!isCompleted);
            completeObject.gameObject.SetActive(isCompleted);

            var progressValue = completedSlots / ((float)slotsCount * 2);
            progressBar.SetValue(progressValue + 0.5f);
        }

        playButton.interactable = !isCompleted;
        
        levelNameText.text = $"{_levelData.LevelId}. {(isCompleted ? _levelData.Questions.Last().Answer : $"Level {_levelData.LevelId}")}";
    }

    private void OnPlayButtonClick()
    {
        onPlayButtonClick?.Invoke(_levelData);
    }
}
