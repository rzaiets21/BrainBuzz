using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerupPanel : MonoBehaviour
{
    [SerializeField] private Image[] blockers;
    [SerializeField] private PowerupsController powerupsController;
    [SerializeField] private CanvasGroup canvasGroup;
    
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI description;

    private void OnEnable()
    {
        powerupsController.onPowerupSelected += OnPowerupSelected;
        foreach (var blocker in blockers)
        {
            blocker.GetComponent<Button>().onClick.AddListener(DeselectCurrentPowerup);
        }
    }

    private void OnDisable()
    {
        powerupsController.onPowerupSelected -= OnPowerupSelected;
        foreach (var blocker in blockers)
        {
            blocker.GetComponent<Button>().onClick.RemoveListener(DeselectCurrentPowerup);
        }
    }

    public void Show(PowerupInfo powerupInfo)
    {
        foreach (var blocker in blockers)
        {
            blocker.enabled = true;
        }
        image.sprite = powerupInfo.Icon;
        description.SetText(powerupInfo.Description.Replace("\\n", "\n"));
        canvasGroup.SetActive(true);
    }

    private void Hide()
    {
        foreach (var blocker in blockers)
        {
            blocker.enabled = false;
        }
        canvasGroup.SetActive(false);
    }

    private void OnPowerupSelected(bool state)
    {
        if(state)
            return;

        Hide();
    }

    private void DeselectCurrentPowerup()
    {
        powerupsController.DeselectCurrent();
    }
}
