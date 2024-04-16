using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RewardScreen : Screen
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private ScreensController screensController;
    
    [SerializeField] private Button claimButton;
    [SerializeField] private TextMeshProUGUI countText;
    
    [SerializeField] private RewardObject coinImage;
    [SerializeField] private RewardPowerupObject[] powerupsImages;

    private void OnEnable()
    {
        claimButton.onClick.AddListener(OnClaimButtonClick);
    }

    private void OnDisable()
    {
        claimButton.onClick.RemoveListener(OnClaimButtonClick);
    }

    protected override void OnShown()
    {
        var rewardCount = 0;
        
        rewardCount = Random.Range(50, 151);
        coinImage.SetCountText(rewardCount);
        PlayerInventory.Instance.Add(rewardCount, false);

        for (int i = 0; i < 2; i++)
        {
            var powerupType = Random.Range(0, 4);
            rewardCount = Random.Range(1, 3);
            
            Debug.LogError(powerupType);
            powerupsImages[i].SetRewardType(powerupType).SetCountText(rewardCount);
            PlayerInventory.Instance.Add((PowerupType)(powerupType + 10), rewardCount);   
        }
    }

    private void OnClaimButtonClick()
    {
        PlayerInventory.Instance.Update();
        
        gameManager.LoadNextLevel();
        screensController.ShowScreen(Screens.Game, onComplete: gameManager.StartGame);
        //screensController.ShowScreen(Screens.MainMenu);
    }

    protected override void OnHidden()
    {
        coinImage.enabled = false;
        foreach (var powerupsImage in powerupsImages)
        {
            powerupsImage.enabled = false;
        }
    }
}
