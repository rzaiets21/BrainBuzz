using UnityEngine;
using UnityEngine.UI;

public class GDPRScreen : Screen
{
    private const string GDPRShownKey = "GDPRIsShown";

    [SerializeField] private ScreensController screensController;
    [SerializeField] private GDPRManager gdprManager;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button privacyButton;
    [SerializeField] private Button termsButton;
    
    private void OnEnable()
    {
        acceptButton.onClick.AddListener(OnClickAcceptButton);
        privacyButton.onClick.AddListener(OnClickPrivacyButton);
        termsButton.onClick.AddListener(OnClickTermsButton);
    }

    private void OnDisable()
    {
        acceptButton.onClick.RemoveListener(OnClickAcceptButton);
        privacyButton.onClick.RemoveListener(OnClickPrivacyButton);
        termsButton.onClick.RemoveListener(OnClickTermsButton);
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

    private void OnClickPrivacyButton()
    {
        Application.OpenURL("https://brainbuzz.apppage.net/privacy-policy");
    }

    private void OnClickTermsButton()
    {
        Application.OpenURL("https://brainbuzz.apppage.net/terms");
    }

    private void OnFormShown()
    {
        PlayerPrefs.SetInt(GDPRShownKey, 1);
        screensController.ShowStartedScreen();
    }
}
