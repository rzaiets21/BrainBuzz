using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class ScreensController : MonoBehaviour
{
    private const string GDPRShownKey = "GDPRIsShown";
    
    [SerializeField] private float transitionDuration;
    
    [SerializeField] private Screens startedScreen;
    [SerializeField] private ScreenInfo[] screens;

    private Screens _previousScreen;
    private Screens _currentScreen;
    
    private Vector2 _canvasSize;

    public Screens CurrentScreen => _currentScreen;

    public event Action<Screens> onScreenShown;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        var canvasRect = GetComponent<RectTransform>();
        _canvasSize = canvasRect.sizeDelta;

        var gdprIsShown = PlayerPrefs.GetInt(GDPRShownKey, 0);
        if (gdprIsShown == 0)
        {
            ShowScreen(Screens.GDPR);
            return;
        }
        
        ShowScreen(startedScreen);
    }

    public bool IsActive(Screens screenType)
    {
        if(screenType == Screens.None)
            return false;
        
        return _currentScreen == screenType;
    }

    public void ShowPreviousScreen()
    {
        if (_previousScreen == Screens.None)
        {
            ShowScreen(Screens.MainMenu, ScreenTransition.LeftToRight);
            return;
        }

        var targetScreen = _previousScreen;
        ShowScreen(targetScreen, ScreenTransition.LeftToRight);
        
        _previousScreen = Screens.None;
    }

    public void ShowStartedScreen()
    {
        ShowScreen(startedScreen);
    }
    
    public void ShowScreen(Screens screenType, ScreenTransition screenTransition = ScreenTransition.None, Action onComplete = null, bool hidePrevious = true)
    {
        if(screenType == Screens.None)
        {
            return;
        }
        
        var screenInfo = screens.First(x => x.ScreenType == screenType);

        var targetScreen = screenInfo.Screen;
        
        var lastOpenedScreen = screens.FirstOrDefault(x => x.ScreenType == _currentScreen)?.Screen;
        
        if(lastOpenedScreen != null)
            lastOpenedScreen.SetInteractable(false);
        
        _previousScreen = _currentScreen;
        _currentScreen = screenType;
        
        // if(screenType != Screens.Loading)
        //     screenBase.rect.SetSiblingIndex(transform.childCount - 4);
        
        onScreenShown?.Invoke(screenType); //TODO Make good solution
        
        switch (screenTransition)
        {
            case ScreenTransition.LeftToRight:
                var targetPosition = new Vector2(_canvasSize.x, 0);
                var currentPosition = lastOpenedScreen.rect.anchoredPosition;
                
                targetScreen.Show(false);
                
                lastOpenedScreen.rect.DOAnchorPos(targetPosition, transitionDuration).SetEase(Ease.InOutQuad).OnComplete(() =>
                {
                    if(lastOpenedScreen != null && hidePrevious)
                    {
                        lastOpenedScreen.Hide();
                        lastOpenedScreen.rect.anchoredPosition = currentPosition;
                    }
                    targetScreen.SetInteractable(true);

                    onComplete?.Invoke();
                    onScreenShown?.Invoke(screenType);
                    
                    // if(screenType != Screens.Loading)
                    //     targetScreen.rect.SetAsLastSibling();
                });
                break;
            case ScreenTransition.RightToLeft:
                // if(screenType != Screens.Loading)
                //     targetScreen.rect.SetAsLastSibling();
                
                targetPosition = targetScreen.rect.anchoredPosition;
                currentPosition = new Vector2(_canvasSize.x, 0);
                targetScreen.rect.anchoredPosition = currentPosition;
                
                targetScreen.Show(false);
                
                targetScreen.rect.DOAnchorPos(targetPosition, transitionDuration).SetEase(Ease.InOutQuad).OnComplete(() =>
                {
                    if(lastOpenedScreen != null && hidePrevious)
                        lastOpenedScreen.Hide();
                    targetScreen.SetInteractable(true);

                    onComplete?.Invoke();
                    onScreenShown?.Invoke(screenType);
                });
                break;
            default:
                if(lastOpenedScreen != null && hidePrevious)
                {
                    lastOpenedScreen.Hide();
                }
                targetScreen.Show();
                
                // if(screenType != Screens.Loading)
                //     targetScreen.rect.SetAsLastSibling();
                
                onComplete?.Invoke();
                onScreenShown?.Invoke(screenType);
                
                break;
        }
    }
}
