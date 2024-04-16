using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GroupPopup : Popup
{
    [SerializeField] private ScreensController screensController;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GroupLevel[] groupLevels;
    [SerializeField] private Button closeButton;

    [SerializeField] private TextMeshProUGUI groupName;

    private void OnEnable()
    {
        foreach (var groupLevel in groupLevels)
        {
            groupLevel.onPlayButtonClick += OnPlayButtonClick;
        }
        
        closeButton.onClick.AddListener(Hide);
    }

    private void OnDisable()
    {
        foreach (var groupLevel in groupLevels)
        {
            groupLevel.onPlayButtonClick -= OnPlayButtonClick;
        }
        
        closeButton.onClick.RemoveListener(Hide);
    }

    public void Init()
    {
        var groupData = gameManager.GetGroupData();
        var levelsCount = groupData.Levels.Count;

        groupName.text = groupData.GroupName;
        
        for (int i = 0; i < levelsCount; i++)
        {
            var groupLevel = groupLevels[i];
            var levelData = groupData.Levels[i];
            groupLevel.Init(levelData);
        }
    }
    
    protected override void OnShown()
    {

    }

    private void OnPlayButtonClick(LevelData levelData)
    {
        gameManager.LoadLevel(levelData);
        Hide(true);
        screensController.ShowScreen(Screens.Game, onComplete: gameManager.StartGame);
    }
}