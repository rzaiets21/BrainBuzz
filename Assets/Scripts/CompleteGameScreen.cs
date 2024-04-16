using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CompleteGameScreen : Screen
{
    private const float ProgressBarDuration = 0.75f;
    private const string GameStreakKey = "CompletedGameStreak";

    [SerializeField] private AudioClip levelCompleted;
    
    [SerializeField] private float showGiftBoxDuration;
    [SerializeField] private RectTransform giftBox;
    
    [SerializeField] private TextMeshProUGUI currentStreakValue;
    [SerializeField] private ProgressBar progressBar;

    [SerializeField] private CanvasGroup shine;
    [SerializeField] private Image shineBg;
    
    [SerializeField] private RateQuestionController rateQuestionController;
    [SerializeField] private ScreensController screensController;
    [SerializeField] private GameManager gameManager;
    
    [SerializeField] private Image taskImage;
    [SerializeField] private TextMeshProUGUI answer;

    private int _completeStreak;

    private void OnEnable()
    {
        var completeStreak = PlayerPrefs.GetInt(GameStreakKey, 0);
        completeStreak = Mathf.Clamp(completeStreak - 1, 0, 10);
        currentStreakValue.SetText(completeStreak.ToString());
        progressBar.SetValue(completeStreak / 5f);
    }

    protected override void OnShown()
    {
        AudioController.Instance.Play(SoundType.Sounds, levelCompleted);
        
        _completeStreak = PlayerPrefs.GetInt(GameStreakKey, 0);
        var levelData = gameManager.LevelData;
        var question = levelData.Questions.First(x => x.LevelImage == 0);

        if (question == null)
            throw new NullReferenceException();

        StartCoroutine(ProgressBarCoroutine());
        rateQuestionController.ResetState();
        
        //storageController.Get(question.ImageUrl, sprite => taskImage.sprite = sprite);
        answer.text = question.Answer.ToLower().FirstCharacterToUpper();
    }

    protected override void OnHidden()
    {
    }

    private IEnumerator ProgressBarCoroutine()
    {
        SetInteractable(_completeStreak >= 5);
        var progressBarValue = progressBar.Value;
        var progressBarTargetValue = _completeStreak / 5f;
        var time = 0f;
        while (time < ProgressBarDuration)
        {
            time = Mathf.Clamp(time + Time.deltaTime, 0f, ProgressBarDuration);
            var t = time / ProgressBarDuration;
            var value = Mathf.Lerp(progressBarValue, progressBarTargetValue, t);
            progressBar.SetValue(value);

            yield return null;
        }
        
        currentStreakValue.SetText(_completeStreak.ToString());
        if (_completeStreak >= 5)
        {
            ShowGiftBoxAnimation();
        }
    }
    
    [ContextMenu("ShowGiftAnimation")]
    private void ShowGiftBoxAnimation()
    {
        shineBg.transform.localScale = Vector3.one;
        
        var currentPosY = giftBox.anchoredPosition.y;
        shineBg.DOFade(0.8f, showGiftBoxDuration);
        shineBg.transform.DOScale(2.2f, showGiftBoxDuration * 2);
        shine.DOFade(0.9f, showGiftBoxDuration);
        
        giftBox.DOAnchorPosY(currentPosY - 50f, showGiftBoxDuration);
        giftBox.DOScaleX(1.35f, showGiftBoxDuration);
        giftBox.DOScaleY(0.75f, showGiftBoxDuration).OnComplete(() =>
        {
            shineBg.DOFade(0, showGiftBoxDuration * 3);
            
            giftBox.DOAnchorPosY(currentPosY + 50f, showGiftBoxDuration);
            giftBox.DOScaleY(1.35f, showGiftBoxDuration);
            giftBox.DOScaleX(0.75f, showGiftBoxDuration).OnComplete(() =>
            {
                shine.DOFade(0f, showGiftBoxDuration * 2f);
                giftBox.DOAnchorPosY(currentPosY, showGiftBoxDuration);
                giftBox.DOScaleY(1f, showGiftBoxDuration);
                giftBox.DOScaleX(1f, showGiftBoxDuration).OnComplete(GiveReward);
            });
        });
    }

    private void GiveReward()
    {
        if (_completeStreak >= 5)
        {
            _completeStreak -= 5;
            PlayerPrefs.SetInt(GameStreakKey, _completeStreak);
            progressBar.SetValue(_completeStreak / 5f);
            currentStreakValue.SetText(_completeStreak.ToString());
            screensController.ShowScreen(Screens.Reward);
            return;
        }
    }
    
    public void ClickContinueButton()
    {
        gameManager.LoadNextLevel();
        screensController.ShowScreen(Screens.Game, onComplete: gameManager.StartGame);
        //screensController.ShowScreen(Screens.MainMenu);
    }
}