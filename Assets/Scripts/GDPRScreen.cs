using UnityEngine;
using UnityEngine.UI;

public class GDPRScreen : Screen
{
    private const string GDPRShownKey = "GDPRIsShown";

    [SerializeField] private ScreensController screensController;
    [SerializeField] private GDPRManager gdprManager;
    [SerializeField] private Button acceptButton;
    
    private void OnEnable()
    {
        acceptButton.onClick.AddListener(OnClickAcceptButton);
    }

    private void OnDisable()
    {
        acceptButton.onClick.RemoveListener(OnClickAcceptButton);
    }

    private void OnClickAcceptButton()
    {
        acceptButton.interactable = false;
        var color = acceptButton.targetGraphic.color;
        color.a = 0.75f;
        acceptButton.targetGraphic.color = color;
        acceptButton.GetComponent<BeatingItem>().enabled = false;
        
        gdprManager.StartShowGDPR(OnFormShown);
    }

    private void OnFormShown()
    {
        PlayerPrefs.SetInt(GDPRShownKey, 1);
        screensController.ShowStartedScreen();
    }
}
