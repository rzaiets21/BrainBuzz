using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : Screen
{
    [SerializeField] private TextMeshProUGUI playButtonText;
    
    [SerializeField] private ScreensController screensController;
    [SerializeField] private GroupPopup _groupPopup;
    [SerializeField] private LoadingScreen loadingScreen;
    [SerializeField] private GameManager gameManager;

    [SerializeField] private Image taskImage;

    public void LoadGame()
    {
        screensController.ShowScreen(Screens.Game, onComplete: gameManager.StartGame);
    }

    protected override void OnShown()
    {
        var groupData = gameManager.GetGroupData(true);
        _groupPopup.Init();
        playButtonText.text = $"Group {groupData.GroupId}";
    }
}
