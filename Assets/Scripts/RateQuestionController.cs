using System;
using UnityEngine;

public class RateQuestionController : MonoBehaviour
{
    [SerializeField] private GameObject mainButton;
    [SerializeField] private GameObject secondButton;
    [SerializeField] private GameObject rateUsArea;
    
    [SerializeField] private RateButton likeButton;
    [SerializeField] private RateButton dislikeButton;

    private void OnEnable()
    {
        likeButton.onClick += OnClickLikeButton;
        dislikeButton.onClick += OnClickDislikeButton;
    }

    private void OnDisable()
    {
        likeButton.onClick -= OnClickLikeButton;
        dislikeButton.onClick -= OnClickDislikeButton;
    }

    public void ResetState()
    {
        mainButton.SetActive(true);
        secondButton.SetActive(false);
        rateUsArea.SetActive(true);
        // likeButton.gameObject.SetActive(true);
        // dislikeButton.gameObject.SetActive(true);
        // likeButton.SetInteractable(true);
        // dislikeButton.SetInteractable(true);
    }
    
    private void OnClickLikeButton()
    {
        mainButton.SetActive(false);
        secondButton.SetActive(true);
        rateUsArea.SetActive(false);
        // likeButton.gameObject.SetActive(false);
        // dislikeButton.gameObject.SetActive(false);
        // likeButton.SetInteractable(false, false);
        // dislikeButton.SetInteractable(false);
        
        Analytics.LogEvent(Analytics.RateQuestion, "like");
    }

    private void OnClickDislikeButton()
    {
        mainButton.SetActive(false);
        secondButton.SetActive(true);
        rateUsArea.SetActive(false);
        // likeButton.gameObject.SetActive(false);
        // dislikeButton.gameObject.SetActive(false);
        // dislikeButton.SetInteractable(false, false);
        // likeButton.SetInteractable(false);
        
        Analytics.LogEvent(Analytics.RateQuestion, "dislike");
    }
}