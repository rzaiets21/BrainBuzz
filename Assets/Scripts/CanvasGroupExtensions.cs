using UnityEngine;

public static class CanvasGroupExtensions
{
    public static void SetActive(this CanvasGroup canvasGroup, bool state, bool deactivate = true)
    {
        //canvasGroup.alpha = state ? 1f : 0f;
        if(deactivate)
            canvasGroup.gameObject.SetActive(state);
        canvasGroup.interactable = state;
        canvasGroup.blocksRaycasts = state;
    }
}